using AmeisenBotX.Wow.Objects;
using System;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Modules.Group
{
    /// <summary>
    /// PERFECTED Focus fire module with validation and dead member filtering.
    /// Bonus for targeting same enemy as party members.
    /// Only active in groups - promotes synergy and faster kills.
    /// </summary>
    public class FocusFireModule : ITargetSelectionModule
    {
        public string Name => "FocusFire";

        public bool IsActive(AmeisenBotInterfaces bot)
        {
            // Comprehensive activation checks
            if (bot?.Objects?.Partymembers == null)
            {
                return false;
            }

            // Edge case: Party exists but no members (solo player in "party" state)
            return bot.Objects.Partymembers.Any();
        }

        public float GetPriorityBonus(IWowUnit target, AmeisenBotInterfaces bot)
        {
            // Bulletproof null checks
            if (bot?.Objects?.Partymembers == null || target == null)
            {
                return 0f;
            }

            // Edge case: Target is dead (party might still have it targeted)
            if (target.IsDead)
            {
                return 0f;
            }

            try
            {
                // Count how many party members are actively attacking this target
                int partyAlsoAttacking = bot.Objects.Partymembers
                    .Where(pm => pm != null)                  // Edge case: Null party member
                    .Where(pm => !pm.IsDead)                  // Edge case: Dead party member
                    .Where(pm => pm.TargetGuid == target.Guid) // Same target
                    .Where(pm => pm.IsInCombat)               // Actually fighting (not just targeted)
                    .Count();

                if (partyAlsoAttacking == 0)
                {
                    return 0f;
                }

                // Stacking bonus: more party members = higher priority
                // +15 per party member attacking same target
                // Cap at 4 party members (60 max bonus) to prevent excessive stacking
                int cappedCount = Math.Min(partyAlsoAttacking, 4);
                return cappedCount * 15f;
            }
            catch (Exception)
            {
                // Edge case: Enumeration fails (party state changing mid-check)
                return 0f;
            }
        }
    }
}
