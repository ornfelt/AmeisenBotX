using AmeisenBotX.BehaviorTree.Enums;
using AmeisenBotX.BehaviorTree.Objects;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Wow.Objects;
using System;

/// <summary>
/// Represents a class that moves the bot to a leaf node in a tree structure.
/// </summary>
namespace AmeisenBotX.Core.Logic.Leafs
{
    /// <summary>
    /// Represents a class that moves the bot to a leaf node in a tree structure.
    /// </summary>
    public class MoveToLeaf : INode
    {
        /// <summary>
        /// Initializes a new instance of the MoveToLeaf class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object representing the bot.</param>
        /// <param name="getUnit">The function to get the IWowUnit.</param>
        /// <param name="child">The optional INode child.</param>
        /// <param name="maxDistance">The maximum distance for moving to the leaf, defaulted to 3.2f.</param>
        public MoveToLeaf(AmeisenBotInterfaces bot, Func<IWowUnit> getUnit, INode child = null, float maxDistance = 3.2f)
        {
            Bot = bot;
            GetUnit = getUnit;
            Child = child;
            MaxDistance = maxDistance;
        }

        /// <summary>
        /// Gets or sets the AmeisenBotInterfaces Bot.
        /// </summary>
        protected AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets or sets the child node.
        /// </summary>
        protected INode Child { get; set; }

        /// <summary>
        /// Gets the func that returns an object of type IWowUnit.
        /// </summary>
        protected Func<IWowUnit> GetUnit { get; }

        /// <summary>
        /// Gets the maximum distance.
        /// </summary>
        protected float MaxDistance { get; }

        ///<summary>
        ///Gets or sets a value indicating whether the movement needs to be stopped.
        ///</summary>
        protected bool NeedToStopMoving { get; set; }

        /// <summary>
        /// Executes the behavior tree node.
        /// </summary>
        /// <returns>
        /// Returns the status of the behavior tree execution.
        /// </returns>
        public virtual BtStatus Execute()
        {
            IWowUnit unit = GetUnit();

            if (unit == null)
            {
                return BtStatus.Failed;
            }

            if (Bot.Player.DistanceTo(unit) > MaxDistance)
            {
                Bot.Movement.SetMovementAction(MovementAction.Move, unit.Position);
                NeedToStopMoving = true;
                return BtStatus.Ongoing;
            }

            if (NeedToStopMoving)
            {
                NeedToStopMoving = false;
                Bot.Movement.StopMovement();
            }

            return Child?.Execute() ?? BtStatus.Success;
        }

        /// <summary>
        /// Retrieves the current instance of the node to execute.
        /// </summary>
        public INode GetNodeToExecute()
        {
            return this;
        }
    }
}