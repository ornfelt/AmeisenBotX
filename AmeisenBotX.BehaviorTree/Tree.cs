using AmeisenBotX.BehaviorTree.Enums;
using AmeisenBotX.BehaviorTree.Interfaces;
using AmeisenBotX.BehaviorTree.Objects;
using System;

namespace AmeisenBotX.BehaviorTree
{
    public class BehaviorTree<T> where T : IBlackboard
    {
        public BehaviorTree(INode<T> node, T blackboard, bool resumeOngoingNodes = false)
        {
            RootNode = node;
            Blackboard = blackboard;
            ResumeOngoingNodes = resumeOngoingNodes;

            BlackboardUpdateEnabled = false;
        }

        public BehaviorTree(INode<T> node, T blackboard, TimeSpan blackboardUpdateTime, bool resumeOngoingNodes = false)
        {
            RootNode = node;
            Blackboard = blackboard;
            BlackboardUpdateTime = blackboardUpdateTime;
            ResumeOngoingNodes = resumeOngoingNodes;

            BlackboardUpdateEnabled = true;
        }

        public T Blackboard { get; set; }

        public bool BlackboardUpdateEnabled { get; set; }

        public TimeSpan BlackboardUpdateTime { get; set; }

        public DateTime LastBlackBoardUpdate { get; set; }

        public INode<T> OngoingNode { get; private set; }

        public bool ResumeOngoingNodes { get; set; }

        public INode<T> RootNode { get; set; }

        public BtStatus Tick()
        {
            if (BlackboardUpdateEnabled)
            {
                DateTime now = DateTime.Now;
                if (LastBlackBoardUpdate + BlackboardUpdateTime < now)
                {
                    Blackboard.Update();
                    LastBlackBoardUpdate = now;
                }
            }

            if (ResumeOngoingNodes)
            {
                BtStatus status;

                if (OngoingNode != null)
                {
                    status = OngoingNode.Execute(Blackboard);

                    if (status is BtStatus.Failed or BtStatus.Success)
                    {
                        OngoingNode = null;
                    }
                }
                else
                {
                    status = RootNode.Execute(Blackboard);

                    if (status is BtStatus.Ongoing)
                    {
                        OngoingNode = RootNode.GetNodeToExecute(Blackboard);
                    }
                }

                return status;
            }
            else
            {
                return RootNode.GetNodeToExecute(Blackboard).Execute(Blackboard);
            }
        }
    }

    public class Tree(INode node, bool resumeOngoingNodes = false)
    {
        public INode OngoingNode { get; private set; }

        public bool ResumeOngoingNodes { get; set; } = resumeOngoingNodes;

        public INode RootNode { get; set; } = node;

        /// <summary>
        /// Last executed node for debugging purposes.
        /// </summary>
        public INode LastExecutedNode { get; private set; }

        /// <summary>
        /// Last status returned from Tick() for debugging.
        /// </summary>
        public BtStatus LastStatus { get; private set; }

        public BtStatus Tick()
        {
            if (ResumeOngoingNodes)
            {
                BtStatus status;

                if (OngoingNode != null)
                {
                    // LastExecutedNode = GetDeepestNode(OngoingNode); // REMOVED: Expensive debug logic
                    status = OngoingNode.Execute();

                    if (status is BtStatus.Failed or BtStatus.Success)
                    {
                        OngoingNode = null;
                    }
                }
                else
                {
                    status = RootNode.Execute();
                    // LastExecutedNode = GetDeepestNode(RootNode.GetNodeToExecute()); // REMOVED: Expensive debug logic

                    if (status is BtStatus.Ongoing)
                    {
                        OngoingNode = RootNode.GetNodeToExecute();
                    }
                }

                LastStatus = status;
                return status;
            }
            else
            {
                INode nodeToExecute = RootNode.GetNodeToExecute();
                // LastExecutedNode = GetDeepestNode(nodeToExecute); // REMOVED: Expensive debug logic
                LastStatus = nodeToExecute.Execute();
                return LastStatus;
            }
        }

        /// <summary>
        /// Recursively finds the deepest node in the execution chain.
        /// This helps the debugger show the actual leaf being executed rather than wrapper nodes.
        /// </summary>
        private static INode GetDeepestNode(INode node)
        {
            // Keep drilling down until we find a leaf or a named node
            INode current = node;
            int maxDepth = 20; // Prevent infinite loops

            for (int i = 0; i < maxDepth; i++)
            {
                // If we found a named node, prefer it
                if (!string.IsNullOrEmpty(current.Name))
                {
                    return current;
                }

                // Try to go deeper
                INode next = current.GetNodeToExecute();

                // If we can't go deeper or we're at the same node, stop
                if (next == null || next == current)
                {
                    return current;
                }

                current = next;
            }

            return current;
        }
    }
}