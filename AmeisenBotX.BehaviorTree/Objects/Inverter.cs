using AmeisenBotX.BehaviorTree.Enums;

/// <summary>
/// Represents a node that inverts the status of its child node.
/// </summary>
namespace AmeisenBotX.BehaviorTree.Objects
{
    /// <summary>
    /// Inverts the status of a node. Ongoing state will remain Onging.
    /// </summary>
    public class Inverter : INode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Inverter"/> class with a given child node.
        /// </summary>
        /// <param name="child">The child node whose status will be inverted.</param>
        public Inverter(INode child) : base()
        {
            Child = child;
        }

        /// <summary>
        /// Gets the child node of the inverter.
        /// </summary>
        public INode Child { get; }

        /// <summary>
        /// Executes the child node and inverts its success or failure status.
        /// </summary>
        /// <returns>The inverted status of the executed child node.</returns>
        public BtStatus Execute()
        {
            BtStatus status = Child.Execute();

            if (status == BtStatus.Success)
            {
                status = BtStatus.Failed;
            }
            else if (status == BtStatus.Failed)
            {
                status = BtStatus.Success;
            }

            return status;
        }

        /// <summary>
        /// Retrieves the child node to be executed.
        /// </summary>
        /// <returns>The child node of the inverter.</returns>
        public INode GetNodeToExecute()
        {
            return Child;
        }
    }

    /// <summary>
    /// Represents a BehaviorTree node with an associated blackboard that inverts the success 
    /// or failure status of its child node. The ongoing status remains unchanged.
    /// </summary>
    /// <typeparam name="T">The type of the blackboard.</typeparam>
    public class Inverter<T> : INode<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Inverter{T}"/> class with a given child node.
        /// </summary>
        /// <param name="child">The child node whose status will be inverted, with the blackboard as an argument.</param>
        public Inverter(INode<T> child) : base()
        {
            Child = child;
        }

        /// <summary>
        /// Gets the child node of the inverter.
        /// </summary>
        public INode<T> Child { get; }

        /// <summary>
        /// Executes the child node using the provided blackboard and inverts its success or failure status.
        /// </summary>
        /// <param name="blackboard">The blackboard of type <typeparamref name="T"/>.</param>
        /// <returns>The inverted status of the executed child node.</returns>
        public BtStatus Execute(T blackboard)
        {
            BtStatus status = Child.Execute(blackboard);

            if (status == BtStatus.Success)
            {
                status = BtStatus.Failed;
            }
            else if (status == BtStatus.Failed)
            {
                status = BtStatus.Success;
            }

            return status;
        }

        /// <summary>
        /// Retrieves the child node to be executed, using the provided blackboard.
        /// </summary>
        /// <param name="blackboard">The blackboard of type <typeparamref name="T"/>.</param>
        /// <returns>The child node of the inverter.</returns>
        public INode<T> GetNodeToExecute(T blackboard)
        {
            return Child;
        }
    }
}