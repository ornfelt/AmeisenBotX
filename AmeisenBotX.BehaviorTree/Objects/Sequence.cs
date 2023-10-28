using AmeisenBotX.BehaviorTree.Enums;

/// summary>
/// Executes a sequence of nodes until all nodes returned success. If a node fails or the
/// sequence finished, it gets resetted.
/// </summary>
namespace AmeisenBotX.BehaviorTree.Objects
{
    /// <summary>
    /// Executes a sequence of nodes until all nodes returned success. If a node fails or the
    /// sequence finished, it gets resetted.
    /// </summary>
    public class Sequence : IComposite
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sequence"/> class with the given child nodes.
        /// </summary>
        /// <param name="children">The nodes to execute in sequence.</param>
        public Sequence(params INode[] children)
        {
            Children = children;
        }

        /// <summary>
        /// Gets the child nodes associated with this sequence.
        /// </summary>
        public INode[] Children { get; }

        /// <summary>
        /// Gets the counter indicating the current position in the sequence.
        /// </summary>
        public int Counter { get; private set; }

        /// <inheritdoc />
        public BtStatus Execute()
        {
            if (Counter == Children.Length)
            {
                Counter = 0;
                return BtStatus.Success;
            }

            BtStatus status = Children[Counter].Execute();

            if (status == BtStatus.Success)
            {
                if (Counter < Children.Length)
                {
                    ++Counter;

                    if (Counter == Children.Length)
                    {
                        Counter = 0;
                        return BtStatus.Success;
                    }
                }
            }
            else if (status == BtStatus.Failed)
            {
                Counter = 0;
                return BtStatus.Failed;
            }

            return BtStatus.Ongoing;
        }

        /// <inheritdoc />
        public INode GetNodeToExecute()
        {
            return this;
        }
    }

    /// <summary>
    /// Represents a sequence node in a BehaviorTree with an associated blackboard of type <typeparamref name="T"/>.
    /// The node attempts to execute its children in sequence using the blackboard.
    /// If any child fails, the sequence fails and is reset. If all children succeed, the sequence succeeds.
    /// </summary>
    /// <typeparam name="T">The type of the blackboard associated with the node.</typeparam>
    public class Sequence<T> : IComposite<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sequence{T}"/> class with the given child nodes.
        /// </summary>
        /// <param name="children">The nodes to execute in sequence.</param>
        public Sequence(params INode<T>[] children)
        {
            Children = children;
        }

        /// <summary>
        /// Gets the child nodes associated with this sequence.
        /// </summary>
        public INode<T>[] Children { get; }

        /// <summary>
        /// Gets the counter indicating the current position in the sequence.
        /// </summary>
        public int Counter { get; private set; }

        /// <inheritdoc />
        public BtStatus Execute(T blackboard)
        {
            if (Counter == Children.Length)
            {
                Counter = 0;
                return BtStatus.Success;
            }

            BtStatus status = Children[Counter].Execute(blackboard);

            if (status == BtStatus.Success)
            {
                if (Counter < Children.Length)
                {
                    ++Counter;

                    if (Counter == Children.Length)
                    {
                        Counter = 0;
                        return BtStatus.Success;
                    }
                }
            }
            else if (status == BtStatus.Failed)
            {
                Counter = 0;
                return BtStatus.Failed;
            }

            return BtStatus.Ongoing;
        }

        /// <inheritdoc /> 
        public INode<T> GetNodeToExecute(T blackboard)
        {
            return this;
        }
    }
}