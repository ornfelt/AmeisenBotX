using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Linq;

/// <summary>
/// Represents an idle action where the bot sits on a chair.
/// </summary>
namespace AmeisenBotX.Core.Logic.Idle.Actions
{
    /// <summary>
    /// Represents an idle action where the bot sits on a chair.
    /// </summary>
    public class SitToChairIdleAction : IIdleAction
    {
        /// <summary>
        /// Initializes a new instance of the SitToChairIdleAction class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object.</param>
        /// <param name="maxDistance">The maximum distance.</param>
        public SitToChairIdleAction(AmeisenBotInterfaces bot, double maxDistance)
        {
            Bot = bot;
            MaxDistance = maxDistance;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the mode is in Autopilot Only.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the mode is in Autopilot Only; otherwise, <c>false</c>.
        /// </value>
        public bool AutopilotOnly => false;

        /// <summary>
        /// Gets the AmeisenBotInterfaces instance of the Bot.
        /// </summary>
        public AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets or sets the cooldown time for a specific operation.
        /// </summary>
        public DateTime Cooldown { get; set; }

        /// <summary>
        /// Gets the maximum cooldown duration in milliseconds.
        /// </summary>
        public int MaxCooldown => 69 * 1000;

        /// <summary>
        /// Gets the maximum duration in milliseconds, which is 90 seconds.
        /// </summary>
        public int MaxDuration => 90 * 1000;

        /// <summary>
        /// Gets the minimum cooldown in milliseconds.
        /// </summary>
        public int MinCooldown => 29 * 1000;

        /// <summary>
        /// Gets the minimum duration in milliseconds.
        /// </summary>
        public int MinDuration => 25 * 1000;

        /// <summary>
        /// Gets or sets the current seat of the WoW game object.
        /// </summary>
        private IWowGameobject CurrentSeat { get; set; }

        /// <summary>
        /// Gets the maximum distance.
        /// </summary>
        private double MaxDistance { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the object has sat down.
        /// </summary>
        private bool SatDown { get; set; }

        /// <summary>
        /// Attempts to find a nearby chair for the bot to sit on.
        /// If a suitable chair is found, sets CurrentSeat and returns true.
        /// If no chair is found, returns false.
        /// </summary>
        public bool Enter()
        {
            SatDown = false;

            // get the center from where to cal the distance, this is needed to prevent going out of
            // the follow trigger radius, which would cause a suspicous loop of running around
            Vector3 originPos = Bot.Player.Position; // StateMachine.Get<StateFollowing>().IsUnitToFollowThere(out IWowUnit unit, false) ? unit.Position : Bot.Player.Position;

            IWowGameobject seat = Bot.Objects.All.OfType<IWowGameobject>()
                .OrderBy(e => e.Position.GetDistance(originPos))
                .FirstOrDefault(e => e.GameObjectType == WowGameObjectType.Chair
                    // make sure no one sits on the chair besides ourself
                    && !Bot.Objects.All.OfType<IWowUnit>()
                        .Where(e => e.Guid != Bot.Wow.PlayerGuid)
                        .Any(x => e.Position.GetDistance(x.Position) < 0.6f)
                    && e.Position.GetDistance(originPos) < MaxDistance - 0.2f);

            if (seat != null)
            {
                CurrentSeat = seat;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Executes the action of sitting down in the current seat.
        /// </summary>
        public void Execute()
        {
            if (!SatDown)
            {
                if (CurrentSeat.Position.GetDistance(Bot.Player.Position) > 1.5f)
                {
                    Bot.Movement.SetMovementAction(MovementAction.Move, CurrentSeat.Position);
                }
                else
                {
                    Bot.Movement.StopMovement();
                    Bot.Wow.InteractWithObject(CurrentSeat);

                    SatDown = true;
                }
            }
        }

        /// <summary>
        /// Converts the object to a string representation.
        /// </summary>
        /// <returns>
        /// If the property AutopilotOnly is true, returns a string representation of "🤖 Sit to Chairs". 
        /// Otherwise, returns a string representation of "Sit to Chairs".
        /// </returns>
        public override string ToString()
        {
            return $"{(AutopilotOnly ? "(🤖) " : "")}Sit to Chairs";
        }
    }
}