using AmeisenBotX.BehaviorTree.Enums;

namespace AmeisenBotX.BehaviorTree.Objects
{
    /// <summary>
    /// Executes all children concurrently and returns based on the success policy.
    /// </summary>
    public class Parallel(ParallelPolicy policy, params INode[] children) : IComposite
    {
        public string Name { get; } = null;

        public INode[] Children { get; } = children;

        public ParallelPolicy Policy { get; } = policy;

        public BtStatus Execute()
        {
            int successCount = 0;
            int failureCount = 0;
            int ongoingCount = 0;

            foreach (INode child in Children)
            {
                switch (child.Execute())
                {
                    case BtStatus.Success:
                        successCount++;
                        break;
                    case BtStatus.Failed:
                        failureCount++;
                        break;
                    case BtStatus.Ongoing:
                        ongoingCount++;
                        break;
                }
            }

            return Policy switch
            {
                ParallelPolicy.RequireOne => successCount > 0 ? BtStatus.Success
                    : ongoingCount > 0 ? BtStatus.Ongoing
                    : BtStatus.Failed,
                ParallelPolicy.RequireAll => successCount == Children.Length ? BtStatus.Success
                    : failureCount > 0 ? BtStatus.Failed
                    : BtStatus.Ongoing,
                _ => BtStatus.Failed
            };
        }

        public INode GetNodeToExecute() => this;
    }

    /// <summary>
    /// Blackboard-aware parallel node.
    /// </summary>
    public class Parallel<T>(ParallelPolicy policy, params INode<T>[] children) : IComposite<T>
    {
        public string Name { get; } = null;

        public INode<T>[] Children { get; } = children;

        public ParallelPolicy Policy { get; } = policy;

        public BtStatus Execute(T blackboard)
        {
            int successCount = 0;
            int failureCount = 0;
            int ongoingCount = 0;

            foreach (INode<T> child in Children)
            {
                switch (child.Execute(blackboard))
                {
                    case BtStatus.Success:
                        successCount++;
                        break;
                    case BtStatus.Failed:
                        failureCount++;
                        break;
                    case BtStatus.Ongoing:
                        ongoingCount++;
                        break;
                }
            }

            return Policy switch
            {
                ParallelPolicy.RequireOne => successCount > 0 ? BtStatus.Success
                    : ongoingCount > 0 ? BtStatus.Ongoing
                    : BtStatus.Failed,
                ParallelPolicy.RequireAll => successCount == Children.Length ? BtStatus.Success
                    : failureCount > 0 ? BtStatus.Failed
                    : BtStatus.Ongoing,
                _ => BtStatus.Failed
            };
        }

        public INode<T> GetNodeToExecute(T blackboard) => this;
    }

    /// <summary>
    /// Policy for determining Parallel node success.
    /// </summary>
    public enum ParallelPolicy
    {
        /// <summary>
        /// Succeeds if any child succeeds.
        /// </summary>
        RequireOne,

        /// <summary>
        /// Succeeds only if all children succeed.
        /// </summary>
        RequireAll
    }
}

