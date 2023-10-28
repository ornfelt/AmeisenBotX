using AmeisenBotX.BehaviorTree.Enums;

/// <summary>
/// Represents a node in a BehaviorTree. This interface defines the core methods required for a BehaviorTree node's execution.
/// </summary>
namespace AmeisenBotX.BehaviorTree.Objects
{
    /// <summary>
    /// Represents a node in a BehaviorTree. This interface defines the core methods required for a BehaviorTree node's execution.
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// Executes the behavior associated with the node and returns its status.
        /// </summary>
        /// <returns>The status of the executed behavior.</returns>
        BtStatus Execute();

        /// <summary>
        /// Retrieves the next node to be executed in the BehaviorTree structure.
        /// </summary>
        /// <returns>The next node to be executed.</returns>
        INode GetNodeToExecute();
    }

    /// <summary>
    /// Represents a node in a BehaviorTree with an associated blackboard of type <typeparamref name="T"/>.
    /// This interface defines the core methods required for a BehaviorTree node's execution using a blackboard.
    /// </summary>
    /// <typeparam name="T">The type of the blackboard associated with the node.</typeparam>
    public interface INode<T>
    {
        /// <summary>
        /// Executes the behavior associated with the node using the provided blackboard and returns its status.
        /// </summary>
        /// <param name="blackboard">The blackboard of type <typeparamref name="T"/>.</param>
        /// <returns>The status of the executed behavior.</returns>
        BtStatus Execute(T blackboard);

        /// <summary>
        /// Retrieves the next node to be executed in the BehaviorTree structure using the provided blackboard.
        /// </summary>
        /// <param name="blackboard">The blackboard of type <typeparamref name="T"/>.</param>
        /// <returns>The next node to be executed.</returns>
        INode<T> GetNodeToExecute(T blackboard);
    }
}