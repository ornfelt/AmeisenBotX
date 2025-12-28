using AmeisenBotX.Wow.Objects;

namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Modules.Core
{
    /// <summary>
    /// PERFECTED Execute priority module with validation for edge cases.
    /// Prioritizes low-HP targets for quick kills.
    /// Always active - execute priority is universal.
    /// </summary>
    public class ExecuteModule : ITargetSelectionModule
    {
        public string Name => "Execute";

        public bool IsActive(AmeisenBotInterfaces bot)
        {
            // Always active - execute priority benefits all scenarios
            return true;
        }

        public float GetPriorityBonus(IWowUnit target, AmeisenBotInterfaces bot)
        {
            // Bulletproof null checks
            if (target == null)
            {
                return 0f;
            }

            // Edge case: Target is already dead
            if (target.IsDead)
            {
                return 0f;
            }

            // Edge case: Invalid HP percentage (< 0 or > 100)
            int healthPct = (int)target.HealthPercentage;
            if (healthPct is < 0 or > 100)
            {
                return 0f; // Invalid data - skip
            }

            // Edge case: Exactly 0 HP but not flagged as dead (edge case in some situations)
            if (target.Health == 0)
            {
                return 0f;
            }

            // Execute range (<10% HP) - FINISH THEM!
            if (healthPct < 10)
            {
                return 80f; // Very high priority - one more hit!
            }

            // Killable range (<20% HP) - almost dead
            if (healthPct < 20)
            {
                return 50f; // High priority - clean up kills
            }

            // Low HP (<35%) - wounded
            if (healthPct < 35)
            {
                return 25f; // Moderate priority
            }

            return 0f;
        }
    }
}
