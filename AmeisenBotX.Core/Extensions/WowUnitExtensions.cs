using AmeisenBotX.Wow.Cache;
using AmeisenBotX.Wow.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Extensions
{
    /// <summary>
    /// Extension methods for IWowUnit to simplify aura checking patterns.
    /// </summary>
    public static class WowUnitExtensions
    {
        /// <summary>
        /// Checks if the unit has an aura with the specified name.
        /// </summary>
        /// <param name="unit">The unit to check.</param>
        /// <param name="db">The database for spell name lookup.</param>
        /// <param name="auraName">The name of the aura to check for.</param>
        /// <returns>True if the unit has the aura, false otherwise.</returns>
        public static bool HasAuraByName(this IWowUnit unit, IAmeisenBotDb db, string auraName)
        {
            return unit?.Auras?.Any(e => db.GetSpellName(e.SpellId) == auraName) ?? false;
        }

        /// <summary>
        /// Checks if the unit has any of the specified auras by name.
        /// </summary>
        /// <param name="unit">The unit to check.</param>
        /// <param name="db">The database for spell name lookup.</param>
        /// <param name="auraNames">The names of the auras to check for.</param>
        /// <returns>True if the unit has any of the auras, false otherwise.</returns>
        public static bool HasAnyAuraByName(this IWowUnit unit, IAmeisenBotDb db, params string[] auraNames)
        {
            if (unit?.Auras == null || auraNames == null || auraNames.Length == 0)
            {
                return false;
            }

            HashSet<string> auraNameSet = new(auraNames, StringComparer.Ordinal);
            return unit.Auras.Any(e => auraNameSet.Contains(db.GetSpellName(e.SpellId)));
        }

        /// <summary>
        /// Checks if the unit is affected by any snare or root effect.
        /// </summary>
        /// <param name="unit">The unit to check.</param>
        /// <param name="db">The database for spell name lookup.</param>
        /// <returns>True if the unit is snared or rooted, false otherwise.</returns>
        public static bool IsSnaredOrRooted(this IWowUnit unit, IAmeisenBotDb db)
        {
            return unit.HasAnyAuraByName(db,
                "Frost Nova",
                "Frost Trap Aura",
                "Hamstring",
                "Concussive Shot",
                "Frostbolt",
                "Frost Shock",
                "Frostfire Bolt",
                "Slow",
                "Entangling Roots");
        }
    }
}
