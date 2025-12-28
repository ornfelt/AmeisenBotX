using AmeisenBotX.BehaviorTree.Enums;
using System;

namespace AmeisenBotX.BehaviorTree.Objects
{
    /// <summary>
    /// Decorator that fails if child doesn't complete within the time limit.
    /// Prevents behaviors from getting stuck indefinitely.
    /// </summary>
    public class TimeLimit(TimeSpan limit, INode child) : INode
    {
        public string Name { get; } = null;

        public INode Child { get; } = child;

        public TimeSpan Limit { get; } = limit;

        private DateTime? StartTime { get; set; }

        public BtStatus Execute()
        {
            StartTime ??= DateTime.Now;

            if (DateTime.Now - StartTime > Limit)
            {
                StartTime = null;
                return BtStatus.Failed;
            }

            BtStatus result = Child.Execute();

            if (result != BtStatus.Ongoing)
            {
                StartTime = null;
            }

            return result;
        }

        public INode GetNodeToExecute() => this;

        /// <summary>
        /// Resets the time limit timer.
        /// </summary>
        public void Reset() => StartTime = null;
    }

    /// <summary>
    /// Blackboard-aware time limit decorator.
    /// </summary>
    public class TimeLimit<T>(TimeSpan limit, INode<T> child) : INode<T>
    {
        public string Name { get; } = null;

        public INode<T> Child { get; } = child;

        public TimeSpan Limit { get; } = limit;

        private DateTime? StartTime { get; set; }

        public BtStatus Execute(T blackboard)
        {
            StartTime ??= DateTime.Now;

            if (DateTime.Now - StartTime > Limit)
            {
                StartTime = null;
                return BtStatus.Failed;
            }

            BtStatus result = Child.Execute(blackboard);

            if (result != BtStatus.Ongoing)
            {
                StartTime = null;
            }

            return result;
        }

        public INode<T> GetNodeToExecute(T blackboard) => this;

        public void Reset() => StartTime = null;
    }
}


