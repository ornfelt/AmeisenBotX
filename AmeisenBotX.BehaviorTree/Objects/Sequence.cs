using AmeisenBotX.BehaviorTree.Enums;

namespace AmeisenBotX.BehaviorTree.Objects
{
    /// <summary>
    /// Executes a sequence of nodes until all nodes returned success. If a node fails or the
    /// sequence finished, it gets resetted.
    /// </summary>
    public class Sequence(params INode[] children) : IComposite
    {
        public string Name { get; } = null;

        public INode[] Children { get; } = children;

        public int Counter { get; private set; }

        /// <summary>
        /// Resets the sequence counter. Call this when the sequence is preempted
        /// by a higher-priority branch to prevent stale state.
        /// </summary>
        public void Reset() => Counter = 0;

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

        public INode GetNodeToExecute()
        {
            return this;
        }
    }

    public class Sequence<T>(params INode<T>[] children) : IComposite<T>
    {
        public string Name { get; } = null;

        public INode<T>[] Children { get; } = children;

        public int Counter { get; private set; }

        /// <summary>
        /// Resets the sequence counter. Call this when the sequence is preempted
        /// by a higher-priority branch to prevent stale state.
        /// </summary>
        public void Reset() => Counter = 0;

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

        public INode<T> GetNodeToExecute(T blackboard)
        {
            return this;
        }
    }
}
