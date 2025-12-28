using AmeisenBotX.Wow.Objects;
using System;

namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Validation.Basic
{
    /// <summary>
    /// Validates that a target is within reasonable reach, including 3D distance checks.
    /// Prevents targeting mobs on bridges, different floors, or vertically unreachable positions.
    /// </summary>
    public class IsWithinReachTargetValidator(AmeisenBotInterfaces bot) : ITargetValidator
    {
        private AmeisenBotInterfaces Bot { get; } = bot;

        // Maximum horizontal (2D) distance for target selection
        private const float MaxHorizontalDistance = 50f;

        // Maximum vertical (Z-axis) distance - prevents targeting mobs on bridges/floors above
        private const float MaxVerticalDistance = 10f;

        // Maximum total 3D distance
        private const float MaxCombatReach = 100f;

        public bool IsValid(IWowUnit unit)
        {
            if (Bot.Player == null)
            {
                return false;
            }

            // Check horizontal (2D) distance
            float distance2D = unit.DistanceTo(Bot.Player);
            if (distance2D > MaxHorizontalDistance)
            {
                return false;
            }

            // CRITICAL: Check vertical (Z-axis) distance
            // This prevents targeting mobs on bridges/floors above or below
            float zDiff = Math.Abs(unit.Position.Z - Bot.Player.Position.Z);
            if (zDiff > MaxVerticalDistance)
            {
                return false;
            }

            // Check total 3D distance for extra safety
            float distance3D = unit.Position.GetDistance(Bot.Player.Position);
            return distance3D <= MaxCombatReach;
        }
    }
}
