using AmeisenBotX.BehaviorTree.Enums;
using System;

namespace AmeisenBotX.BehaviorTree.Objects
{
    /// <summary>
    /// Executes a when input is true and b when it is false.
    /// </summary>
    public class Selector : IComposite
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Selector"/> class with the given condition and child nodes.
        /// </summary>
        /// <param name="condition">The condition to evaluate.</param>
        /// <param name="nodeA">The node to execute when the condition is true.</param>
        /// <param name="nodeB">The node to execute when the condition is false.</param>
        public Selector(Func<bool> condition, INode nodeA, INode nodeB) : base()
        {
            Condition = condition;
            Children = new INode[] { nodeA, nodeB };
        }

        /// <summary>
        /// Gets the child nodes associated with this selector.
        /// </summary>
        public INode[] Children { get; }

        /// <summary>
        /// Gets the condition based on which a child node will be executed.
        /// </summary>
        public Func<bool> Condition { get; }

        /// <inheritdoc />
        public BtStatus Execute()
        {
            return GetNodeToExecute().Execute();
        }

        /// <inheritdoc />
        public INode GetNodeToExecute()
        {
            return Condition() ? Children[0] : Children[1];
        }
    }

    /// <summary>
    /// Represents a selector node in a BehaviorTree with an associated blackboard of type <typeparamref name="T"/>.
    /// The node determines which child to execute based on a given condition.
    /// When the condition is true, it executes the first child, and when false, it executes the second child.
    /// </summary>
    /// <typeparam name="T">The type of the blackboard associated with the node.</typeparam>
    public class Selector<T> : IComposite<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Selector{T}"/> class with the given condition and child nodes.
        /// </summary>
        /// <param name="condition">The condition to evaluate using the blackboard.</param>
        /// <param name="nodeA">The node to execute when the condition is true.</param>
        /// <param name="nodeB">The node to execute when the condition is false.</param>
        public Selector(Func<T, bool> condition, INode<T> nodeA, INode<T> nodeB) : base()
        {
            Condition = condition;
            Children = new INode<T>[] { nodeA, nodeB };
        }

        /// <summary>
        /// Gets the child nodes associated with this selector.
        /// </summary>
        public INode<T>[] Children { get; }

        /// <summary>
        /// Gets the condition based on which a child node will be executed.
        /// </summary>
        public Func<T, bool> Condition { get; }

        /// <inheritdoc />
        public BtStatus Execute(T blackboard)
        {
            return GetNodeToExecute(blackboard).Execute(blackboard);
        }

        /// <inheritdoc />
        public INode<T> GetNodeToExecute(T blackboard)
        {
            return Condition(blackboard) ? Children[0] : Children[1];
        }
    }
}