using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;

namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Validation.Basic
{
    /// <summary>
    /// Determines if the given unit is valid. A valid unit should be alive, attackable, and not a critter.
    /// </summary>
    public class IsValidAliveTargetValidator : ITargetValidator
    {
        /// <summary>
        /// Determines if the given unit is valid. A valid unit should be alive, attackable, and not a critter.
        /// </summary>
        /// <param name="unit">The unit to validate.</param>
        /// <returns>True if the unit is valid, otherwise False.</returns>
        public bool IsValid(IWowUnit unit)
        {
            // unit should be alive, attackable and no critter
            return IWowUnit.IsValidAlive(unit)
                && unit.ReadType() is not WowCreatureType.Critter;
        }
    }
}