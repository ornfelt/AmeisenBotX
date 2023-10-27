using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Movement.Enums;

namespace AmeisenBotX.Core.Logic.Routines
{
    public static class CommonRoutines
    {
        /// <summary>
        /// Moves the bot to the target position if the target is not in line of sight or
        /// if the distance to the target position is greater than the specified range.
        /// Returns true if the bot successfully starts moving towards the target position,
        /// otherwise returns false.
        /// </summary>
        public static bool MoveToTarget(AmeisenBotInterfaces bot, Vector3 position, float range, MovementAction action = MovementAction.Move)
        {
            if (!bot.Objects.IsTargetInLineOfSight || bot.Player.DistanceTo(position) > range)
            {
                bot.Movement.SetMovementAction(action, position);
                return true;
            }

            return false;
        }
    }
}