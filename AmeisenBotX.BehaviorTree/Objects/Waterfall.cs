using AmeisenBotX.BehaviorTree.Enums;
using System;

/// <summary>
/// Namespace containing objects related to the behavior tree implementation for the AmeisenBotX project.
/// </summary>
namespace AmeisenBotX.BehaviorTree.Objects
{
    /// <summary>
    /// Special selector that runs the first node where the condition returns true. If none returned
    /// true, the fallbackNode will be executed
    /// </summary>
    public class Waterfall : IComposite
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Waterfall"/> class.
        /// </summary>
        /// <param name="fallbackNode">The node to execute if no conditions are met.</param>
        /// <param name="conditionNodePairs">Pairs of conditions and associated nodes.</param>
        public Waterfall(INode fallbackNode, params (Func<bool> condition, INode node)[] conditionNodePairs) : base()
        {
            FallbackNode = fallbackNode;
            ConditionNodePairs = conditionNodePairs;
        }

        /// <summary>
        /// Gets the child nodes associated with this composite.
        /// </summary>
        public INode[] Children { get; }

        /// <summary>
        /// Gets the pairs of conditions and associated nodes.
        /// </summary>
        public (Func<bool> condition, INode node)[] ConditionNodePairs { get; }

        /// <summary>
        /// Gets the node to be executed if no conditions are met.
        /// </summary>
        public INode FallbackNode { get; }

        /// <inheritdoc />
        public BtStatus Execute()
        {
            return GetNodeToExecute().Execute();
        }

        /// <inheritdoc />
        public INode GetNodeToExecute()
        {
            for (int i = 0; i < ConditionNodePairs.Length; ++i)
            {
                if (ConditionNodePairs[i].condition())
                {
                    return ConditionNodePairs[i].node;
                }
            }

            return FallbackNode;
        }
    }

    /// <summary>
    /// Executes nodes based on conditions provided in pairs. The first node whose associated condition returns true is executed. 
    /// If no condition returns true, a fallback node is executed.
    /// </summary>
    /// <typeparam name="T">The type of the blackboard.</typeparam>
    public class Waterfall<T> : IComposite<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Waterfall{T}"/> class.
        /// </summary>
        /// <param name="fallbackNode">The node to execute if no conditions are met.</param>
        /// <param name="conditionNodePairs">Pairs of conditions and associated nodes.</param>
        public Waterfall(INode<T> fallbackNode, params (Func<T, bool> condition, INode<T> node)[] conditionNodePairs) : base()
        {
            FallbackNode = fallbackNode;
            ConditionNodePairs = conditionNodePairs;
        }


        /// <summary>
        /// Gets the child nodes associated with this composite.
        /// </summary>
        public INode<T>[] Children { get; }

        /// <summary>
        /// Gets the pairs of conditions and associated nodes.
        /// </summary>
        public (Func<T, bool> condition, INode<T> node)[] ConditionNodePairs { get; }

        /// <summary>
        /// Gets the node to be executed if no conditions are met.
        /// </summary>
        public INode<T> FallbackNode { get; }

        /// <inheritdoc />
        public BtStatus Execute(T blackboard)
        {
            return GetNodeToExecute(blackboard).Execute(blackboard);
        }

        /// <inheritdoc />
        public INode<T> GetNodeToExecute(T blackboard)
        {
            for (int i = 0; i < ConditionNodePairs.Length; ++i)
            {
                if (ConditionNodePairs[i].condition(blackboard))
                {
                    return ConditionNodePairs[i].node;
                }
            }

            return FallbackNode;
        }
    }
}