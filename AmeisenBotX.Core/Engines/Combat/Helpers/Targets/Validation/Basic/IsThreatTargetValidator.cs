using AmeisenBotX.Wow.Objects;

namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Validation.Basic
{
    /// <summary>
    /// Determines if the specified WoW unit is currently in combat.
    /// </summary>
    public class IsInCombatTargetValidator : ITargetValidator
    {
        /// <summary>
        /// Determines if the specified WoW unit is currently in combat.
        /// </summary>
        /// <param name="unit">The WoW unit to check for in-combat status.</param>
        /// <returns>True if the WoW unit is in combat; otherwise, false.</returns>
        public bool IsValid(IWowUnit unit)
        {
            return unit.IsInCombat;
        }
    }
}