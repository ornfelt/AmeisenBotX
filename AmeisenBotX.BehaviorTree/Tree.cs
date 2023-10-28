using AmeisenBotX.BehaviorTree.Enums;
using AmeisenBotX.BehaviorTree.Interfaces;
using AmeisenBotX.BehaviorTree.Objects;
using System;

/// <summary>
/// Represents a behavior tree that operates on a given blackboard of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the blackboard this behavior tree operates on.</typeparam>
namespace AmeisenBotX.BehaviorTree
{
    /// <summary>
    /// Represents a behavior tree that operates on a given blackboard of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the blackboard this behavior tree operates on.</typeparam>
    public class BehaviorTree<T> where T : IBlackboard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BehaviorTree{T}"/> class without blackboard update frequency.
        /// </summary>
        /// <param name="node">The root node of this behavior tree.</param>
        /// <param name="blackboard">The blackboard used by this behavior tree.</param>
        /// <param name="resumeOngoingNodes">Indicates whether the tree should resume from ongoing nodes.</param>
        public BehaviorTree(INode<T> node, T blackboard, bool resumeOngoingNodes = false)
        {
            RootNode = node;
            Blackboard = blackboard;
            ResumeOngoingNodes = resumeOngoingNodes;

            BlackboardUpdateEnabled = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BehaviorTree{T}"/> class with blackboard update frequency.
        /// </summary>
        /// <param name="node">The root node of this behavior tree.</param>
        /// <param name="blackboard">The blackboard used by this behavior tree.</param>
        /// <param name="blackboardUpdateTime">The frequency at which the blackboard should be updated.</param>
        /// <param name="resumeOngoingNodes">Indicates whether the tree should resume from ongoing nodes.</param>
        public BehaviorTree(INode<T> node, T blackboard, TimeSpan blackboardUpdateTime, bool resumeOngoingNodes = false)
        {
            RootNode = node;
            Blackboard = blackboard;
            BlackboardUpdateTime = blackboardUpdateTime;
            ResumeOngoingNodes = resumeOngoingNodes;

            BlackboardUpdateEnabled = true;
        }

        /// <summary>
        /// Gets or sets the blackboard used by this behavior tree.
        /// </summary>
        public T Blackboard { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether blackboard updating is enabled.
        /// </summary>
        public bool BlackboardUpdateEnabled { get; set; }

        /// <summary>
        /// Gets or sets the frequency at which the blackboard should be updated.
        /// </summary>
        public TimeSpan BlackboardUpdateTime { get; set; }

        /// <summary>
        /// Gets or sets the time of the last blackboard update.
        /// </summary>
        public DateTime LastBlackBoardUpdate { get; set; }

        /// <summary>
        /// Gets the ongoing node being executed, if any.
        /// </summary>
        public INode<T> OngoingNode { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tree should resume from ongoing nodes.
        /// </summary>
        public bool ResumeOngoingNodes { get; set; }

        /// <summary>
        /// Gets or sets the root node of this behavior tree.
        /// </summary>
        public INode<T> RootNode { get; set; }

        /// <summary>
        /// Executes a single tick of the behavior tree.
        /// </summary>
        /// <returns>The status of the executed node.</returns>
        public BtStatus Tick()
        {
            if (BlackboardUpdateEnabled && LastBlackBoardUpdate + BlackboardUpdateTime < DateTime.Now)
            {
                Blackboard.Update();
                LastBlackBoardUpdate = DateTime.Now;
            }

            if (ResumeOngoingNodes)
            {
                BtStatus status;

                if (OngoingNode != null)
                {
                    status = OngoingNode.Execute(Blackboard);

                    if (status is BtStatus.Failed or BtStatus.Success)
                    {
                        OngoingNode = null;
                    }
                }
                else
                {
                    status = RootNode.Execute(Blackboard);

                    if (status is BtStatus.Ongoing)
                    {
                        OngoingNode = RootNode.GetNodeToExecute(Blackboard);
                    }
                }

                return status;
            }
            else
            {
                return RootNode.GetNodeToExecute(Blackboard).Execute(Blackboard);
            }
        }
    }

    /// <summary>
    /// Represents a behavior tree without an associated blackboard.
    /// </summary>
    public class Tree
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tree"/> class.
        /// </summary>
        /// <param name="node">The root node of this behavior tree.</param>
        /// <param name="resumeOngoingNodes">Indicates whether the tree should resume from ongoing nodes.</param>
        public Tree(INode node, bool resumeOngoingNodes = false)
        {
            RootNode = node;
            ResumeOngoingNodes = resumeOngoingNodes;
        }

        /// <summary>
        /// Gets the ongoing node being executed, if any.
        /// </summary>
        public INode OngoingNode { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tree should resume from ongoing nodes.
        /// </summary>
        public bool ResumeOngoingNodes { get; set; }

        /// <summary>
        /// Gets or sets the root node of this behavior tree.
        /// </summary>
        public INode RootNode { get; set; }

        /// <summary>
        /// Executes a single tick of the behavior tree.
        /// </summary>
        /// <returns>The status of the executed node.</returns>
        public BtStatus Tick()
        {
            if (ResumeOngoingNodes)
            {
                BtStatus status;

                if (OngoingNode != null)
                {
                    status = OngoingNode.Execute();

                    if (status is BtStatus.Failed or BtStatus.Success)
                    {
                        OngoingNode = null;
                    }
                }
                else
                {
                    status = RootNode.Execute();

                    if (status is BtStatus.Ongoing)
                    {
                        OngoingNode = RootNode.GetNodeToExecute();
                    }
                }

                return status;
            }
            else
            {
                return RootNode.GetNodeToExecute().Execute();
            }
        }
    }
}