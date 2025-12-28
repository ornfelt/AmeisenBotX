using AmeisenBotX.BehaviorTree.Enums;
using System;

namespace AmeisenBotX.BehaviorTree.Objects
{
    /// <summary>
    /// Decorator that prevents execution of child until cooldown period has elapsed since last success.
    /// Use this to rate-limit expensive operations.
    /// </summary>
    public class Cooldown(TimeSpan cooldown, INode child) : INode
    {
        public string Name { get; } = null;

        public INode Child { get; } = child;

        public TimeSpan CooldownDuration { get; } = cooldown;

        private DateTime LastSuccess { get; set; }

        public BtStatus Execute()
        {
            if (DateTime.Now - LastSuccess < CooldownDuration)
            {
                return BtStatus.Failed;
            }

            BtStatus result = Child.Execute();

            if (result == BtStatus.Success)
            {
                LastSuccess = DateTime.Now;
            }

            return result;
        }

        public INode GetNodeToExecute() => this;

        /// <summary>
        /// Resets the cooldown, allowing immediate execution.
        /// </summary>
        public void Reset() => LastSuccess = default;
    }

    /// <summary>
    /// Blackboard-aware cooldown decorator.
    /// </summary>
    public class Cooldown<T>(TimeSpan cooldown, INode<T> child) : INode<T>
    {
        public string Name { get; } = null;

        public INode<T> Child { get; } = child;

        public TimeSpan CooldownDuration { get; } = cooldown;

        private DateTime LastSuccess { get; set; }

        public BtStatus Execute(T blackboard)
        {
            if (DateTime.Now - LastSuccess < CooldownDuration)
            {
                return BtStatus.Failed;
            }

            BtStatus result = Child.Execute(blackboard);

            if (result == BtStatus.Success)
            {
                LastSuccess = DateTime.Now;
            }

            return result;
        }

        public INode<T> GetNodeToExecute(T blackboard) => this;

        public void Reset() => LastSuccess = default;
    }
}

