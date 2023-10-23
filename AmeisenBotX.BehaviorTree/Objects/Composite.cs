namespace AmeisenBotX.BehaviorTree.Objects
{
    /// <summary>
    /// Represents a composite node in a BehaviorTree, which can have multiple child nodes.
    /// </summary>
    public interface IComposite : INode
    {
        /// <summary>
        /// Gets an array of child nodes that are part of this composite node.
        /// </summary>
        INode[] Children { get; }
    }

    /// <summary>
    /// Represents a composite node in a BehaviorTree with an associated blackboard of type <typeparamref name="T"/>,
    /// which can have multiple child nodes.
    /// </summary>
    /// <typeparam name="T">The type of the blackboard.</typeparam>
    public interface IComposite<T> : INode<T>
    {
        /// <summary>
        /// Gets an array of child nodes that are part of this composite node.
        /// </summary>
        INode<T>[] Children { get; }
    }
}