using AmeisenBotX.BehaviorTree.Enums;
using System;

namespace AmeisenBotX.BehaviorTree.Objects
{
    /// <summary>
    /// Selector that executes a TryGet-style condition. If the condition succeeds and returns
    /// a value, passes that value to the success node factory. Otherwise executes the fallback.
    /// This eliminates the need for mutable fields to pass data from conditions to actions.
    /// </summary>
    /// <typeparam name="TOutput">Type of value produced by the condition</typeparam>
    public class TrySelector<TOutput>(
        Func<(bool success, TOutput value)> tryCondition,
        Func<TOutput, INode> successNodeFactory,
        INode fallbackNode = null) : INode
    {
        public Func<(bool success, TOutput value)> TryCondition { get; } = tryCondition;

        public Func<TOutput, INode> SuccessNodeFactory { get; } = successNodeFactory;

        public INode FallbackNode { get; } = fallbackNode;

        public string Name { get; } = null;

        public BtStatus Execute()
        {
            (bool success, TOutput value) = TryCondition();

            return success ? SuccessNodeFactory(value).Execute() : FallbackNode?.Execute() ?? BtStatus.Failed;
        }

        public INode GetNodeToExecute() => this;
    }

    /// <summary>
    /// Blackboard-aware version of TrySelector.
    /// </summary>
    /// <typeparam name="TOutput">Type of value produced by the condition</typeparam>
    /// <typeparam name="TBlackboard">Type of blackboard</typeparam>
    public class TrySelector<TOutput, TBlackboard>(
        Func<TBlackboard, (bool success, TOutput value)> tryCondition,
        Func<TOutput, INode<TBlackboard>> successNodeFactory,
        INode<TBlackboard> fallbackNode = null) : INode<TBlackboard>
    {
        public Func<TBlackboard, (bool success, TOutput value)> TryCondition { get; } = tryCondition;

        public Func<TOutput, INode<TBlackboard>> SuccessNodeFactory { get; } = successNodeFactory;

        public INode<TBlackboard> FallbackNode { get; } = fallbackNode;

        public string Name { get; } = null;

        public BtStatus Execute(TBlackboard blackboard)
        {
            (bool success, TOutput value) = TryCondition(blackboard);

            return success ? SuccessNodeFactory(value).Execute(blackboard) : FallbackNode?.Execute(blackboard) ?? BtStatus.Failed;
        }

        public INode<TBlackboard> GetNodeToExecute(TBlackboard blackboard) => this;
    }
}

