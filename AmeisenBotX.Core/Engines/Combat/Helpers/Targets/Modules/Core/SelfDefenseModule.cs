using AmeisenBotX.Wow.Objects;

namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Modules.Core
{
    /// <summary>
    /// PERFECTED Self-defense module with comprehensive edge case handling.
    /// Prioritizes enemies directly attacking the player.
    /// Always active - survival is paramount.
    /// </summary>
    public class SelfDefenseModule : ITargetSelectionModule
    {
        public string Name => "SelfDefense";

        public bool IsActive(AmeisenBotInterfaces bot)
        {
            // Always active - self-defense is universal
            // Even if player is null, module should be safe to call
            return true;
        }

        public float GetPriorityBonus(IWowUnit target, AmeisenBotInterfaces bot)
        {
            // Bulletproof null checks
            if (bot?.Player == null || target == null)
            {
                return 0f;
            }

            // Edge case: Target has no target (not attacking anyone)
            if (target.TargetGuid == 0)
            {
                return 0f;
            }

            // Edge case: Player is dead - no defensive priority needed
            if (bot.Player.IsDead)
            {
                return 0f;
            }

            // CRITICAL: Enemy is attacking US directly
            if (target.TargetGuid == bot.Player.Guid)
            {
                // Extra priority if target is also in combat with us
                if (target.IsInCombat)
                {
                    return 100f; // Maximum priority - active threat!
                }

                return 90f; // High priority - targeting us but not yet attacking
            }

            return 0f;
        }
    }
}
