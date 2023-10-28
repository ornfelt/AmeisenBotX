using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;

/// <summary>
/// Represents a class that prioritizes targets within a dungeon environment.
/// </summary>
namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Priority.Special
{
    /// <summary>
    /// Represents a class that prioritizes targets within a dungeon environment.
    /// </summary>
    public class DungeonTargetPrioritizer : ITargetPrioritizer
    {
        /// <summary>
        /// Initializes a new instance of the DungeonTargetPrioritizer class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces instance.</param>
        public DungeonTargetPrioritizer(AmeisenBotInterfaces bot)
        {
            Bot = bot;

            // add per map validation functions here, lambda should return true if the unit has
            // pririty, false if not
            Priorities = new()
            {
                { WowMapId.UtgardeKeep, UtgardeKeepIsIceblock },
            };
        }

        /// <summary>
        /// Gets the private instance of the <see cref="AmeisenBotInterfaces"/> Bot.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets the dictionary of priorities for each WowMapId.
        /// </summary>
        private Dictionary<WowMapId, Func<IWowUnit, bool>> Priorities { get; }

        /// <summary>
        /// Checks if the given unit has priority based on the map ID and associated priorities.
        /// </summary>
        /// <param name="unit">The unit to check for priority.</param>
        /// <returns>True if the unit has priority, false otherwise.</returns>
        public bool HasPriority(IWowUnit unit)
        {
            if (Priorities.TryGetValue(Bot.Objects.MapId, out Func<IWowUnit, bool> hasPriority))
            {
                return hasPriority(unit);
            }

            // no entry found, skip validation
            return false;
        }

        /// <summary>
        /// Determines if the given unit is inside the Iceblock of Utgarde Keep.
        /// </summary>
        /// <param name="unit">The unit to check.</param>
        /// <returns>True if the unit is inside the Iceblock, false otherwise.</returns>
        private bool UtgardeKeepIsIceblock(IWowUnit unit)
        {
            return Bot.Db.GetUnitName(unit, out string name) && name == "Frozen Tomb"; // TODO: find display id
        }
    }
}