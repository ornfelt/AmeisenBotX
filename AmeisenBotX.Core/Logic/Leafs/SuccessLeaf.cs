using AmeisenBotX.BehaviorTree.Enums;
using AmeisenBotX.BehaviorTree.Objects;
using System;

/// <summary>
/// Represents a leaf node in a tree structure that signifies a successful operation.
/// </summary>
namespace AmeisenBotX.Core.Logic.Leafs
{
    /// <summary>
    /// Represents a leaf node in a tree structure that signifies a successful operation.
    /// </summary>
    public class SuccessLeaf : INode
    {
        /// <summary>
        /// Initializes a new instance of the SuccessLeaf class.
        /// </summary>
        /// <param name="action">The action to be performed.</param>
        public SuccessLeaf(Action action = null)
        {
            Action = action;
        }

        /// <summary>
        /// Gets the private action.
        /// </summary>
        private Action Action { get; }

        /// <summary>
        /// Executes the specified action and returns a BtStatus of Success.
        /// </summary>
        public BtStatus Execute()
        {
            Action?.Invoke();
            return BtStatus.Success;
        }

        /// <summary>
        /// Retrieves the node to execute.
        /// </summary>
        /// <returns>The node to execute.</returns>
        public INode GetNodeToExecute()
        {
            return this;
        }
    }
}