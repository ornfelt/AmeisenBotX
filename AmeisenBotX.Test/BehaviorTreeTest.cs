using AmeisenBotX.BehaviorTree;
using AmeisenBotX.BehaviorTree.Enums;
using AmeisenBotX.BehaviorTree.Interfaces;
using AmeisenBotX.BehaviorTree.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AmeisenBotX.Test
{
    /// <summary>
    /// Tests the behavior of a DualSelector tree.
    /// The tree has four leaf nodes: treeResult00, treeResult10, treeResult01, and treeResult11.
    /// The tree selects between two nodes based on the values of FirstNode and SecondFirstNode in the testBlackboard.
    /// If FirstNode is false, it selects treeResult00 and treeResult01.
    /// If SecondFirstNode is false, it selects treeResult00 and treeResult10.
    /// If both FirstNode and SecondFirstNode are false, it selects treeResult00.
    /// If both FirstNode and SecondFirstNode are true, it selects treeResult11.
    /// </summary>
    [TestClass]
    public class BehaviorTreeTest
    {
        /// <summary>
        /// Tests the behavior of a DualSelector tree.
        /// The tree has four leaf nodes: treeResult00, treeResult10, treeResult01, and treeResult11.
        /// The tree selects between two nodes based on the values of FirstNode and SecondFirstNode in the testBlackboard.
        /// If FirstNode is false, it selects treeResult00 and treeResult01.
        /// If SecondFirstNode is false, it selects treeResult00 and treeResult10.
        /// If both FirstNode and SecondFirstNode are true, it selects all four nodes.
        /// The test checks the values of treeResult variables after each tick of the tree.
        /// </summary>
        [TestMethod]
        public void DualSelectorTreeTest()
        {
            int treeResult00 = 0;
            int treeResult10 = 0;
            int treeResult01 = 0;
            int treeResult11 = 0;

            TestBlackboard testBlackboard = new();

            BehaviorTree<TestBlackboard> tree = new
            (
                new DualSelector<TestBlackboard>
                (
                    (blackboard) => blackboard.FirstNode,
                    (blackboard) => blackboard.SecondFirstNode,
                    new Leaf<TestBlackboard>
                    (
                        (blackboard) =>
                        {
                            ++treeResult00;
                            return BtStatus.Success;
                        }
                    ),
                    new Leaf<TestBlackboard>
                    (
                        (blackboard) =>
                        {
                            ++treeResult10;
                            return BtStatus.Success;
                        }
                    ),
                    new Leaf<TestBlackboard>
                    (
                        (blackboard) =>
                        {
                            ++treeResult01;
                            return BtStatus.Success;
                        }
                    ),
                    new Leaf<TestBlackboard>
                    (
                        (blackboard) =>
                        {
                            ++treeResult11;
                            return BtStatus.Success;
                        }
                    )
                ),
                testBlackboard
            );

            testBlackboard.FirstNode = false;
            testBlackboard.SecondFirstNode = false;

            tree.Tick();

            Assert.AreEqual(1, treeResult00);
            Assert.AreEqual(0, treeResult10);
            Assert.AreEqual(0, treeResult01);
            Assert.AreEqual(0, treeResult11);

            testBlackboard.FirstNode = true;

            tree.Tick();

            Assert.AreEqual(1, treeResult00);
            Assert.AreEqual(1, treeResult10);
            Assert.AreEqual(0, treeResult01);
            Assert.AreEqual(0, treeResult11);

            testBlackboard.FirstNode = false;
            testBlackboard.SecondFirstNode = true;

            tree.Tick();

            Assert.AreEqual(1, treeResult00);
            Assert.AreEqual(1, treeResult10);
            Assert.AreEqual(1, treeResult01);
            Assert.AreEqual(0, treeResult11);

            testBlackboard.FirstNode = true;
            testBlackboard.SecondFirstNode = true;

            tree.Tick();

            Assert.AreEqual(1, treeResult00);
            Assert.AreEqual(1, treeResult10);
            Assert.AreEqual(1, treeResult01);
            Assert.AreEqual(1, treeResult11);
        }

        /// <summary>
        /// Tests the functionality of inverting the behavior tree.
        /// </summary>
        [TestMethod]
        public void InverterTreeTest()
        {
            int treeResult0 = 0;
            int treeResult1 = 0;

            TestBlackboard testBlackboard = new()
            {
                FirstNode = true
            };

            BehaviorTree<TestBlackboard> tree = new
            (
                new Sequence<TestBlackboard>
                (
                    new Inverter<TestBlackboard>
                    (
                        new Leaf<TestBlackboard>
                        (
                            (blackboard) =>
                            {
                                ++treeResult0;
                                return BtStatus.Success;
                            }
                        )
                    ),
                    new Leaf<TestBlackboard>
                    (
                        (blackboard) =>
                        {
                            ++treeResult1;
                            return BtStatus.Success;
                        }
                    )
                ),
                testBlackboard
            );

            tree.Tick();

            Assert.AreEqual(1, treeResult0);
            Assert.AreEqual(0, treeResult1);

            tree.Tick();
            tree.Tick();

            Assert.AreEqual(3, treeResult0);
            Assert.AreEqual(0, treeResult1);
        }

        /// <summary>
        /// Tests the behavior of a nested tree.
        /// </summary>
        [TestMethod]
        public void NestedTreeTest()
        {
            int treeResult0 = 0;
            int treeResult1 = 0;
            int treeResult01 = 0;

            TestBlackboard testBlackboard = new()
            {
                FirstNode = true,
                SecondFirstNode = true
            };

            BehaviorTree<TestBlackboard> tree = new
            (
                new Selector<TestBlackboard>
                (
                    (blackboard) => blackboard.FirstNode,
                    new Leaf<TestBlackboard>
                    (
                        (blackboard) =>
                        {
                            ++treeResult0;
                            return BtStatus.Success;
                        }
                    ),
                    new Selector<TestBlackboard>
                    (
                        (blackboard) => blackboard.SecondFirstNode,
                        new Leaf<TestBlackboard>
                        (
                            (blackboard) =>
                            {
                                ++treeResult01;
                                return BtStatus.Success;
                            }
                        ),
                        new Leaf<TestBlackboard>
                        (
                            (blackboard) =>
                            {
                                ++treeResult1;
                                return BtStatus.Success;
                            }
                        )
                    )
                ),
                testBlackboard
            );

            tree.Tick();

            Assert.AreEqual(1, treeResult0);
            Assert.AreEqual(0, treeResult1);
            Assert.AreEqual(0, treeResult01);

            testBlackboard.FirstNode = false;

            tree.Tick();

            Assert.AreEqual(1, treeResult0);
            Assert.AreEqual(0, treeResult1);
            Assert.AreEqual(1, treeResult01);

            testBlackboard.SecondFirstNode = false;

            tree.Tick();

            Assert.AreEqual(1, treeResult0);
            Assert.AreEqual(1, treeResult1);
            Assert.AreEqual(1, treeResult01);
        }

        /// <summary>
        /// Tests the behavior of the ongoing tree.
        /// </summary>
        [TestMethod]
        public void OngoingTreeTest()
        {
            int treeResult0 = 0;
            int treeResult1 = 0;
            int treeResult2 = 0;

            TestBlackboard testBlackboard = new();

            BehaviorTree<TestBlackboard> tree = new
            (
                new Sequence<TestBlackboard>
                (
                    new Leaf<TestBlackboard>
                    (
                        (blackboard) =>
                        {
                            ++treeResult1;
                            return BtStatus.Success;
                        }
                    ),
                    new Leaf<TestBlackboard>
                    (
                        (blackboard) =>
                        {
                            ++treeResult0;
                            return BtStatus.Success;
                        }
                    ),
                    new Leaf<TestBlackboard>
                    (
                        (blackboard) =>
                        {
                            ++treeResult2;
                            return treeResult2 < 5 ? BtStatus.Ongoing : BtStatus.Success;
                        }
                    ),
                    new Leaf<TestBlackboard>
                    (
                        (blackboard) =>
                        {
                            ++treeResult0;
                            return BtStatus.Success;
                        }
                    )
                ),
                testBlackboard
            );

            tree.Tick();

            Assert.AreEqual(0, treeResult0);
            Assert.AreEqual(1, treeResult1);
            Assert.AreEqual(0, treeResult2);

            tree.Tick();

            Assert.AreEqual(1, treeResult0);
            Assert.AreEqual(1, treeResult1);
            Assert.AreEqual(0, treeResult2);

            tree.Tick();

            Assert.AreEqual(1, treeResult0);
            Assert.AreEqual(1, treeResult1);
            Assert.AreEqual(1, treeResult2);

            tree.Tick();

            Assert.AreEqual(1, treeResult0);
            Assert.AreEqual(1, treeResult1);
            Assert.AreEqual(2, treeResult2);

            tree.Tick();

            Assert.AreEqual(1, treeResult0);
            Assert.AreEqual(1, treeResult1);
            Assert.AreEqual(3, treeResult2);

            tree.Tick();

            Assert.AreEqual(1, treeResult0);
            Assert.AreEqual(1, treeResult1);
            Assert.AreEqual(4, treeResult2);

            tree.Tick();

            Assert.AreEqual(1, treeResult0);
            Assert.AreEqual(1, treeResult1);
            Assert.AreEqual(5, treeResult2);

            tree.Tick();

            Assert.AreEqual(2, treeResult0);
            Assert.AreEqual(1, treeResult1);
            Assert.AreEqual(5, treeResult2);
        }

        /// <summary>
        /// Tests the behavior of resuming an ongoing behavior tree.
        /// </summary>
        [TestMethod]
        public void OngoingTreeTestResume()
        {
            int treeResult0 = 0;
            int treeResult1 = 0;
            int treeResult2 = 0;

            TestBlackboard testBlackboard = new();

            BehaviorTree<TestBlackboard> tree = new
            (
                new Sequence<TestBlackboard>
                (
                    new Leaf<TestBlackboard>
                    (
                        (blackboard) =>
                        {
                            ++treeResult1;
                            return BtStatus.Success;
                        }
                    ),
                    new Leaf<TestBlackboard>
                    (
                        (blackboard) =>
                        {
                            ++treeResult0;
                            return BtStatus.Success;
                        }
                    ),
                    new Leaf<TestBlackboard>
                    (
                        (blackboard) =>
                        {
                            ++treeResult2;
                            return treeResult2 < 5 ? BtStatus.Ongoing : BtStatus.Success;
                        }
                    ),
                    new Leaf<TestBlackboard>
                    (
                        (blackboard) =>
                        {
                            ++treeResult0;
                            return BtStatus.Success;
                        }
                    )
                ),
                testBlackboard,
                true
            );

            Assert.AreEqual(true, tree.ResumeOngoingNodes);
            Assert.AreEqual(null, tree.OngoingNode);

            tree.Tick();

            Assert.AreEqual(0, treeResult0);
            Assert.AreEqual(1, treeResult1);
            Assert.AreEqual(0, treeResult2);

            Assert.AreNotEqual(null, tree.OngoingNode);

            tree.Tick();

            Assert.AreEqual(1, treeResult0);
            Assert.AreEqual(1, treeResult1);
            Assert.AreEqual(0, treeResult2);

            Assert.AreNotEqual(null, tree.OngoingNode);

            tree.Tick();

            Assert.AreEqual(1, treeResult0);
            Assert.AreEqual(1, treeResult1);
            Assert.AreEqual(1, treeResult2);

            Assert.AreNotEqual(null, tree.OngoingNode);

            tree.Tick();

            Assert.AreEqual(1, treeResult0);
            Assert.AreEqual(1, treeResult1);
            Assert.AreEqual(2, treeResult2);

            Assert.AreNotEqual(null, tree.OngoingNode);

            tree.Tick();

            Assert.AreEqual(1, treeResult0);
            Assert.AreEqual(1, treeResult1);
            Assert.AreEqual(3, treeResult2);

            Assert.AreNotEqual(null, tree.OngoingNode);

            tree.Tick();

            Assert.AreEqual(1, treeResult0);
            Assert.AreEqual(1, treeResult1);
            Assert.AreEqual(4, treeResult2);

            Assert.AreNotEqual(null, tree.OngoingNode);

            tree.Tick();

            Assert.AreEqual(1, treeResult0);
            Assert.AreEqual(1, treeResult1);
            Assert.AreEqual(5, treeResult2);

            Assert.AreNotEqual(null, tree.OngoingNode);

            tree.Tick();

            Assert.AreEqual(2, treeResult0);
            Assert.AreEqual(1, treeResult1);
            Assert.AreEqual(5, treeResult2);

            Assert.AreEqual(null, tree.OngoingNode);
        }

        /// <summary>
        /// This method tests the behavior of the Sequence tree in a specific scenario.
        /// </summary>
        [TestMethod]
        public void SequenceTreeTest()
        {
            int treeResult0 = 0;
            int treeResult1 = 0;
            int treeResult2 = 0;

            TestBlackboard testBlackboard = new();

            BehaviorTree<TestBlackboard> tree = new
            (
                new Sequence<TestBlackboard>
                (
                    new Leaf<TestBlackboard>
                    (
                        (blackboard) =>
                        {
                            ++treeResult1;
                            return BtStatus.Success;
                        }
                    ),
                    new Leaf<TestBlackboard>
                    (
                        (blackboard) =>
                        {
                            ++treeResult0;
                            return BtStatus.Success;
                        }
                    ),
                    new Leaf<TestBlackboard>
                    (
                        (blackboard) =>
                        {
                            ++treeResult2;
                            return BtStatus.Failed;
                        }
                    ),
                    new Leaf<TestBlackboard>
                    (
                        (blackboard) =>
                        {
                            ++treeResult0;
                            return BtStatus.Success;
                        }
                    )
                ),
                testBlackboard
            );

            tree.Tick();

            Assert.AreEqual(0, treeResult0);
            Assert.AreEqual(1, treeResult1);
            Assert.AreEqual(0, treeResult2);

            tree.Tick();

            Assert.AreEqual(1, treeResult0);
            Assert.AreEqual(1, treeResult1);
            Assert.AreEqual(0, treeResult2);

            tree.Tick();

            Assert.AreEqual(1, treeResult0);
            Assert.AreEqual(1, treeResult1);
            Assert.AreEqual(1, treeResult2);

            tree.Tick();

            Assert.AreEqual(1, treeResult0);
            Assert.AreEqual(2, treeResult1);
            Assert.AreEqual(1, treeResult2);
        }

        /// <summary>
        /// XML comment for the SimpleTreeTest method.
        /// </summary>
        [TestMethod]
        public void SimpleTreeTest()
        {
            int treeResult0 = 0;
            int treeResult1 = 0;

            TestBlackboard testBlackboard = new()
            {
                FirstNode = true
            };

            BehaviorTree<TestBlackboard> tree = new
            (
                new Selector<TestBlackboard>
                (
                    (blackboard) => blackboard.FirstNode,
                    new Leaf<TestBlackboard>
                    (
                        (blackboard) =>
                        {
                            ++treeResult0;
                            return BtStatus.Success;
                        }
                    ),
                    new Leaf<TestBlackboard>
                    (
                        (blackboard) =>
                        {
                            ++treeResult1;
                            return BtStatus.Success;
                        }
                    )
                ),
                testBlackboard
            );

            tree.Tick();

            Assert.AreEqual(1, treeResult0);
            Assert.AreEqual(0, treeResult1);

            testBlackboard.FirstNode = false;

            tree.Tick();
            tree.Tick();

            Assert.AreEqual(1, treeResult0);
            Assert.AreEqual(2, treeResult1);
        }

        /// <summary>
        /// This is a unit test method for testing the WaterfallTree.
        /// </summary>
        [TestMethod]
        public void WaterfallTreeTest()
        {
            int treeResult0 = 0;
            int treeResult1 = 0;
            int treeResult2 = 0;

            TestBlackboard testBlackboard = new() { };

            BehaviorTree<TestBlackboard> tree = new
            (
                new Waterfall<TestBlackboard>
                (
                    new Leaf<TestBlackboard>((b) => { ++treeResult2; return BtStatus.Success; }),
                    (
                        (blackboard) => treeResult0 == 0,
                        new Leaf<TestBlackboard>
                        (
                            (blackboard) =>
                            {
                                ++treeResult0;
                                return BtStatus.Success;
                            }
                        )
                    ),
                    (
                        (blackboard) => treeResult0 == 1,
                        new Leaf<TestBlackboard>
                        (
                            (blackboard) =>
                            {
                                ++treeResult0;
                                return BtStatus.Success;
                            }
                        )
                    ),
                    (
                        (blackboard) => treeResult0 == 2,
                        new Leaf<TestBlackboard>
                        (
                            (blackboard) =>
                            {
                                ++treeResult1;
                                return BtStatus.Success;
                            }
                        )
                    )
                ),
                testBlackboard
            );

            tree.Tick();

            Assert.AreEqual(1, treeResult0);
            Assert.AreEqual(0, treeResult1);
            Assert.AreEqual(0, treeResult2);

            tree.Tick();
            tree.Tick();

            Assert.AreEqual(2, treeResult0);
            Assert.AreEqual(1, treeResult1);
            Assert.AreEqual(0, treeResult2);

            ++treeResult0;

            tree.Tick();
            tree.Tick();

            Assert.AreEqual(3, treeResult0);
            Assert.AreEqual(1, treeResult1);
            Assert.AreEqual(2, treeResult2);
        }
    }

    /// <summary>
    /// Represents a class that implements the IBlackboard interface for testing purposes.
    /// </summary>
    public class TestBlackboard : IBlackboard
    {
        /// <summary>
        /// Gets or sets a value indicating whether this is the first node.
        /// </summary>
        public bool FirstNode;
        /// <summary>
        /// Represents the boolean value indicating whether the second first node is present.
        /// </summary>
        public bool SecondFirstNode;

        /// <summary>
        /// This method updates a particular element.
        /// </summary>
        public void Update()
        {
        }
    }
}