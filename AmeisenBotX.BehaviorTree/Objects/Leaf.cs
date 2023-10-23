using AmeisenBotX.BehaviorTree.Enums;
using System;

namespace AmeisenBotX.BehaviorTree.Objects
{
    /// <summary>
    /// Represents a leaf node in a BehaviorTree. A leaf node is a terminal node that performs 
    /// an action and returns a status.
    /// </summary>
    public class Leaf : INode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Leaf"/> class with a specified action.
        /// </summary>
        /// <param name="behaviorTreeAction">The action to be performed when the node is executed.</param>
        public Leaf(Func<BtStatus> behaviorTreeAction) : base()
        {
            BehaviorTreeAction = behaviorTreeAction;
        }

        /// <summary>
        /// Gets or sets the action associated with this leaf node.
        /// </summary>
        public Func<BtStatus> BehaviorTreeAction { get; set; }

        /// <summary>
        /// Executes the associated action and returns its status.
        /// </summary>
        /// <returns>The status of the executed action.</returns>
        public BtStatus Execute()
        {
            return BehaviorTreeAction();
        }

        /// <summary>
        /// Retrieves the current node to be executed, which is this leaf node itself.
        /// </summary>
        /// <returns>The current leaf node.</returns>
        public INode GetNodeToExecute()
        {
            return this;
        }
    }

    /// <summary>
    /// Represents a leaf node in a BehaviorTree with an associated blackboard. A leaf node is a terminal 
    /// node that performs an action using the blackboard and returns a status.
    /// </summary>
    /// <typeparam name="T">The type of the blackboard.</typeparam>
    public class Leaf<T> : INode<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Leaf{T}"/> class with a specified action.
        /// </summary>
        /// <param name="behaviorTreeAction">The action to be performed using the blackboard when the node is executed.</param>
        public Leaf(Func<T, BtStatus> behaviorTreeAction) : base()
        {
            BehaviorTreeAction = behaviorTreeAction;
        }

        /// <summary>
        /// Gets or sets the action associated with this leaf node, which uses a blackboard argument.
        /// </summary>
        public Func<T, BtStatus> BehaviorTreeAction { get; set; }

        /// <summary>
        /// Executes the associated action using the provided blackboard and returns its status.
        /// </summary>
        /// <param name="blackboard">The blackboard of type <typeparamref name="T"/>.</param>
        /// <returns>The status of the executed action.</returns>
        public BtStatus Execute(T blackboard)
        {
            return BehaviorTreeAction(blackboard);
        }

        /// <summary>
        /// Retrieves the current node to be executed using the provided blackboard, which is this leaf node itself.
        /// </summary>
        /// <param name="blackboard">The blackboard of type <typeparamref name="T"/>.</param>
        /// <returns>The current leaf node.</returns>
        public INode<T> GetNodeToExecute(T blackboard)
        {
            return this;
        }
    }
}