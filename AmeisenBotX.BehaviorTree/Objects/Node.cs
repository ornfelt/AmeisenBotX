using AmeisenBotX.BehaviorTree.Enums;

namespace AmeisenBotX.BehaviorTree.Objects
{
    public interface INode
    {
        /// <summary>
        /// Optional name for debugging and tracing. Can be null.
        /// </summary>
        string Name { get; }

        BtStatus Execute();

        INode GetNodeToExecute();
    }

    public interface INode<T>
    {
        /// <summary>
        /// Optional name for debugging and tracing. Can be null.
        /// </summary>
        string Name { get; }

        BtStatus Execute(T blackboard);

        INode<T> GetNodeToExecute(T blackboard);
    }
}
