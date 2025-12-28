using AmeisenBotX.Wow.Objects;

namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Modules.Core
{
    /// <summary>
    /// PERFECTED Proximity module with position validation.
    /// Soft preference for nearby targets.
    /// Always active - proximity is always relevant.
    /// </summary>
    public class ProximityModule : ITargetSelectionModule
    {
        public string Name => "Proximity";

        public bool IsActive(AmeisenBotInterfaces bot)
        {
            // Always active - proximity is always relevant
            return true;
        }

        public float GetPriorityBonus(IWowUnit target, AmeisenBotInterfaces bot)
        {
            // Bulletproof null checks
            if (bot?.Player == null || target == null)
            {
                return 0f;
            }

            float distance = target.DistanceTo(bot.Player);

            // Edge case: Negative distance (should never happen, but defensive)
            if (distance < 0f)
            {
                return 0f;
            }

            // Edge case: Unreasonably far (> 100 yards) - likely not a valid target
            if (distance > 100f)
            {
                return 0f;
            }

            // Melee range - immediate threat or opportunity
            if (distance < 10f)
            {
                return 20f;
            }

            //Close range - easily reachable
            if (distance < 20f)
            {
                return 15f;
            }

            // Medium range - reasonable distance
            if (distance < 30f)
            {
                return 10f;
            }

            // Far range - soft bonus
            if (distance < 40f)
            {
                return 5f;
            }

            // Very far - minimal consideration
            return 0f;
        }
    }
}
