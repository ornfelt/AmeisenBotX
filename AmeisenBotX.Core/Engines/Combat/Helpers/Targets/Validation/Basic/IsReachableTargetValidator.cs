using AmeisenBotX.Common.Math;
using AmeisenBotX.Wow.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Represents a validator that checks if a target is reachable within a specified maximum distance.
/// </summary>
namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Validation.Basic
{
    /// <summary>
    /// Represents a validator that checks if a target is reachable within a specified maximum distance.
    /// </summary>
    public class IsReachableTargetValidator : ITargetValidator
    {
        /// <summary>
        /// Constructs a new instance of the IsReachableTargetValidator class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces instance.</param>
        /// <param name="maxDistance">The maximum distance for the target to be considered reachable. Default value is 80.0f.</param>
        public IsReachableTargetValidator(AmeisenBotInterfaces bot, float maxDistance = 80.0f)
        {
            Bot = bot;
            MaxDistance = maxDistance;
        }

        /// <summary>
        /// Gets the Bot object.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets the maximum distance.
        /// </summary>
        private float MaxDistance { get; }

        /// <summary>
        /// Determines if the given unit is valid.
        /// </summary>
        /// <param name="unit">The unit to be validated.</param>
        /// <returns>True if the unit is valid, otherwise false.</returns>
        public bool IsValid(IWowUnit unit)
        {
            // unit needs to be reachable (path end must be in combat reach for melees) and the path
            // should be shorter than MaxDistance
            return IsInRangePathfinding(unit);
        }

        /// <summary>
        /// Determines if the path has reached the specified unit.
        /// </summary>
        /// <param name="position">The position to check from.</param>
        /// <param name="unit">The unit to check against.</param>
        /// <returns><c>true</c> if the path has reached the unit, otherwise <c>false</c>.</returns>
        private bool HasPathReachedUnit(Vector3 position, IWowUnit unit)
        {
            if (Bot.CombatClass.IsMelee)
            {
                // last node should be in combat reach and not too far above
                return position.GetDistance2D(unit.Position) <= Bot.Player.MeleeRangeTo(unit)
                    && MathF.Abs(position.Z - unit.Position.Z) < 2.5f;
            }
            else
            {
                // TODO: best way should be line of sight test? skipped at the moment due to too
                // much calls
                return Bot.Player.DistanceTo(unit) <= 40.0f;
            }
        }

        /// <summary>
        /// Checks if the provided unit is within range for pathfinding.
        /// </summary>
        /// <param name="unit">The unit to check the range for.</param>
        /// <returns>True if the unit is within range, false otherwise.</returns>
        private bool IsInRangePathfinding(IWowUnit unit)
        {
            float distance = 0;
            IEnumerable<Vector3> path = Bot.PathfindingHandler.GetPath((int)Bot.Objects.MapId, Bot.Objects.Player.Position, unit.Position);

            if (path != null && path.Any() && HasPathReachedUnit(path.Last(), unit))
            {
                for (int i = 0; i < path.Count() - 1; ++i)
                {
                    distance += path.ElementAt(i).GetDistance(path.ElementAt(i + 1));

                    if (distance > MaxDistance)
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }
}