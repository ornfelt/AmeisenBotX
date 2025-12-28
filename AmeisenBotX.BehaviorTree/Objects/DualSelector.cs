using AmeisenBotX.BehaviorTree.Enums;
using System;

namespace AmeisenBotX.BehaviorTree.Objects
{
    /// <summary>
    /// Special selector that executes nodeNone when input is 0|0, nodeA when input is 1|0, nodeB
    /// when input is 0|1 and nodeBoth when input is 1|1.
    /// </summary>
    public class DualSelector(Func<bool> conditionA, Func<bool> conditionB, INode nodeNone, INode nodeA, INode nodeB, INode nodeBoth) : IComposite
    {
        public string Name { get; } = null;

        public INode[] Children { get; } = [nodeNone, nodeA, nodeB, nodeBoth];

        public Func<bool> ConditionA { get; } = conditionA;

        public Func<bool> ConditionB { get; } = conditionB;

        public BtStatus Execute()
        {
            return GetNodeToExecute().Execute();
        }

        public INode GetNodeToExecute()
        {
            // Cache condition results to prevent multiple evaluations
            bool a = ConditionA();
            bool b = ConditionB();

            return a && b ? Children[3]
                 : a && !b ? Children[1]
                 : !a && b ? Children[2]
                 : Children[0];
        }
    }

    public class DualSelector<T>(Func<T, bool> conditionA, Func<T, bool> conditionB, INode<T> nodeNone, INode<T> nodeA, INode<T> nodeB, INode<T> nodeBoth) : IComposite<T>
    {
        public string Name { get; } = null;

        public INode<T>[] Children { get; } = [nodeNone, nodeA, nodeB, nodeBoth];

        public Func<T, bool> ConditionA { get; } = conditionA;

        public Func<T, bool> ConditionB { get; } = conditionB;

        public BtStatus Execute(T blackboard)
        {
            return GetNodeToExecute(blackboard).Execute(blackboard);
        }

        public INode<T> GetNodeToExecute(T blackboard)
        {
            // Cache condition results to prevent multiple evaluations
            bool a = ConditionA(blackboard);
            bool b = ConditionB(blackboard);

            return a && b ? Children[3]
                 : a && !b ? Children[1]
                 : !a && b ? Children[2]
                 : Children[0];
        }
    }
}
