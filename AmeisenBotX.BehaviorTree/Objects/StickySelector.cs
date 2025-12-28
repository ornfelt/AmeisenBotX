using AmeisenBotX.BehaviorTree.Enums;
using System;
using System.Linq;

namespace AmeisenBotX.BehaviorTree.Objects
{
    /// <summary>
    /// A selector that "sticks" to a selected branch until it completes or times out.
    /// Prevents oscillation by not re-evaluating conditions while a branch is ongoing.
    /// 
    /// Behavior:
    /// 1. Evaluate conditions to find matching branch
    /// 2. Execute that branch and track its state
    /// 3. While branch returns Ongoing, keep executing it without re-evaluating conditions
    /// 4. Only when branch completes (Success/Failed) or times out, re-evaluate
    /// </summary>
    public class StickySelector : IComposite
    {
        /// <summary>
        /// Create a StickySelector with fallback and condition-node pairs.
        /// </summary>
        /// <param name="maxStickyDuration">Maximum time to stick to a branch before forcing re-evaluation.</param>
        /// <param name="fallbackNode">Node to execute when no conditions match.</param>
        /// <param name="conditionNodePairs">Pairs of (condition, node) to evaluate in order.</param>
        public StickySelector(TimeSpan maxStickyDuration, INode fallbackNode, params (Func<bool> condition, INode node)[] conditionNodePairs)
        {
            MaxStickyDuration = maxStickyDuration;
            FallbackNode = fallbackNode;
            ConditionNodePairs = conditionNodePairs;
            Children = [fallbackNode, .. conditionNodePairs.Select(p => p.node)];
        }

        /// <summary>
        /// Create a StickySelector with default 30 second sticky duration.
        /// </summary>
        public StickySelector(INode fallbackNode, params (Func<bool> condition, INode node)[] conditionNodePairs)
            : this(TimeSpan.FromSeconds(30), fallbackNode, conditionNodePairs)
        {
        }

        public INode[] Children { get; }
        public (Func<bool> condition, INode node)[] ConditionNodePairs { get; }
        public INode FallbackNode { get; }
        public TimeSpan MaxStickyDuration { get; }
        public string Name { get; } = "StickySelector";

        // Sticky state
        private INode _currentBranch;
        private DateTime _branchStartTime;

        public BtStatus Execute()
        {
            // Check if we have a sticky branch that's still valid
            if (_currentBranch != null)
            {
                // Check timeout
                if (DateTime.UtcNow - _branchStartTime > MaxStickyDuration)
                {
                    // Timeout - clear sticky state and re-evaluate
                    _currentBranch = null;
                }
                else
                {
                    // Execute sticky branch
                    BtStatus status = _currentBranch.Execute();

                    // If branch is still ongoing, stay with it
                    if (status == BtStatus.Ongoing)
                    {
                        return BtStatus.Ongoing;
                    }

                    // Branch completed - clear sticky state
                    _currentBranch = null;

                    // Return the final status (caller can decide what to do)
                    return status;
                }
            }

            // No sticky branch - evaluate conditions fresh
            INode selectedNode = EvaluateConditions();

            if (selectedNode != null)
            {
                // Start tracking this branch
                _currentBranch = selectedNode;
                _branchStartTime = DateTime.UtcNow;

                // Execute the selected branch
                BtStatus status = selectedNode.Execute();

                // If completed immediately, don't keep it sticky
                if (status != BtStatus.Ongoing)
                {
                    _currentBranch = null;
                }

                return status;
            }

            // No conditions matched - use fallback
            return FallbackNode.Execute();
        }

        public INode GetNodeToExecute()
        {
            // If sticky, return the sticky branch
            if (_currentBranch != null && DateTime.UtcNow - _branchStartTime <= MaxStickyDuration)
            {
                return _currentBranch;
            }

            // Otherwise, evaluate
            return EvaluateConditions() ?? FallbackNode;
        }

        /// <summary>
        /// Force clear the sticky state. Use for emergency interrupts.
        /// </summary>
        public void ClearSticky()
        {
            _currentBranch = null;
        }

        private INode EvaluateConditions()
        {
            for (int i = 0; i < ConditionNodePairs.Length; i++)
            {
                if (ConditionNodePairs[i].condition())
                {
                    return ConditionNodePairs[i].node;
                }
            }
            return null;
        }
    }

    /// <summary>
    /// Generic version of StickySelector with blackboard support.
    /// </summary>
    public class StickySelector<T> : IComposite<T>
    {
        public StickySelector(TimeSpan maxStickyDuration, INode<T> fallbackNode, params (Func<T, bool> condition, INode<T> node)[] conditionNodePairs)
        {
            MaxStickyDuration = maxStickyDuration;
            FallbackNode = fallbackNode;
            ConditionNodePairs = conditionNodePairs;
            Children = [fallbackNode, .. conditionNodePairs.Select(p => p.node)];
        }

        public StickySelector(INode<T> fallbackNode, params (Func<T, bool> condition, INode<T> node)[] conditionNodePairs)
            : this(TimeSpan.FromSeconds(30), fallbackNode, conditionNodePairs)
        {
        }

        public INode<T>[] Children { get; }
        public (Func<T, bool> condition, INode<T> node)[] ConditionNodePairs { get; }
        public INode<T> FallbackNode { get; }
        public TimeSpan MaxStickyDuration { get; }
        public string Name { get; } = "StickySelector";

        private INode<T> _currentBranch;
        private DateTime _branchStartTime;

        public BtStatus Execute(T blackboard)
        {
            if (_currentBranch != null)
            {
                if (DateTime.UtcNow - _branchStartTime > MaxStickyDuration)
                {
                    _currentBranch = null;
                }
                else
                {
                    BtStatus status = _currentBranch.Execute(blackboard);
                    if (status == BtStatus.Ongoing)
                    {
                        return BtStatus.Ongoing;
                    }
                    _currentBranch = null;
                    return status;
                }
            }

            INode<T> selectedNode = EvaluateConditions(blackboard);

            if (selectedNode != null)
            {
                _currentBranch = selectedNode;
                _branchStartTime = DateTime.UtcNow;

                BtStatus status = selectedNode.Execute(blackboard);
                if (status != BtStatus.Ongoing)
                {
                    _currentBranch = null;
                }
                return status;
            }

            return FallbackNode.Execute(blackboard);
        }

        public INode<T> GetNodeToExecute(T blackboard)
        {
            return _currentBranch != null && DateTime.UtcNow - _branchStartTime <= MaxStickyDuration
                ? _currentBranch
                : EvaluateConditions(blackboard) ?? FallbackNode;
        }

        public void ClearSticky()
        {
            _currentBranch = null;
        }

        private INode<T> EvaluateConditions(T blackboard)
        {
            for (int i = 0; i < ConditionNodePairs.Length; i++)
            {
                if (ConditionNodePairs[i].condition(blackboard))
                {
                    return ConditionNodePairs[i].node;
                }
            }
            return null;
        }
    }
}
