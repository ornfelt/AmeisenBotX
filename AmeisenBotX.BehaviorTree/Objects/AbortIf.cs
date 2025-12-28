using AmeisenBotX.BehaviorTree.Enums;
using System;

namespace AmeisenBotX.BehaviorTree.Objects
{
    /// <summary>
    /// Decorator that aborts child execution if a condition becomes true.
    /// Use for interrupting lower-priority actions when higher-priority needs arise.
    /// </summary>
    public class AbortIf(Func<bool> abortCondition, INode child) : INode
    {
        public string Name { get; } = null;

        public Func<bool> AbortCondition { get; } = abortCondition;

        public INode Child { get; } = child;

        public BtStatus Execute()
        {
            return AbortCondition() ? BtStatus.Failed : Child.Execute();
        }

        public INode GetNodeToExecute() => this;
    }

    /// <summary>
    /// Blackboard-aware abort decorator.
    /// </summary>
    public class AbortIf<T>(Func<T, bool> abortCondition, INode<T> child) : INode<T>
    {
        public string Name { get; } = null;

        public Func<T, bool> AbortCondition { get; } = abortCondition;

        public INode<T> Child { get; } = child;

        public BtStatus Execute(T blackboard)
        {
            return AbortCondition(blackboard) ? BtStatus.Failed : Child.Execute(blackboard);
        }

        public INode<T> GetNodeToExecute(T blackboard) => this;
    }
}

