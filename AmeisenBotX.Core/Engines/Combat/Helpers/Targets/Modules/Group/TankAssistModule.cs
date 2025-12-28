using AmeisenBotX.Core.Managers.Party;
using AmeisenBotX.Wow.Objects;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Modules.Group
{
    /// <summary>
    /// PERFECTED Tank assist module with comprehensive validation.
    /// DPS follows tank's target for proper threat management.
    /// Only active in groups with tanks present.
    /// </summary>
    public class TankAssistModule : ITargetSelectionModule
    {
        public string Name => "TankAssist";

        public bool IsActive(AmeisenBotInterfaces bot)
        {
            // Comprehensive activation checks
            if (bot?.Party == null)
            {
                return false;
            }

            if (bot.Objects?.Partymembers == null || !bot.Objects.Partymembers.Any())
            {
                return false;
            }

            // Edge case: Party exists but no tanks assigned
            IEnumerable<PartyMemberInfo> tanks = bot.Party.GetTanks();
            return tanks != null && tanks.Any();
        }

        public float GetPriorityBonus(IWowUnit target, AmeisenBotInterfaces bot)
        {
            // Bulletproof null checks
            if (bot?.Party == null || bot.Objects?.Partymembers == null || target == null)
            {
                return 0f;
            }

            // Edge case: Target is dead (tank might still have it targeted)
            if (target.IsDead)
            {
                return 0f;
            }

            IEnumerable<PartyMemberInfo> tanks = bot.Party.GetTanks();
            if (tanks == null)
            {
                return 0f;
            }

            foreach (PartyMemberInfo tankInfo in tanks)
            {
                // Edge case: Tank info is null
                if (tankInfo == null)
                {
                    continue;
                }

                // Find the actual tank player object
                IWowUnit tankPlayer = bot.Objects.Partymembers.FirstOrDefault(p => p.Guid == tankInfo.Guid);

                // Edge case: Tank not found in party members (left group, disconnected)
                if (tankPlayer == null)
                {
                    continue;
                }

                // Edge case: Tank is dead
                if (tankPlayer.IsDead)
                {
                    continue;
                }

                // Edge case: Tank has no target (not yet engaged)
                if (tankPlayer.TargetGuid == 0)
                {
                    continue;
                }

                // CRITICAL: Tank is actively fighting this target
                if (tankPlayer.TargetGuid == target.Guid)
                {
                    // Extra priority if tank is in combat (actively tanking)
                    if (tankPlayer.IsInCombat)
                    {
                        return 90f; // Very high priority - assist the tank!
                    }

                    return 75f; // High priority - tank targeting but not yet in combat
                }

                // Edge case: Tank lost threat, target is now attacking tank
                if (target.TargetGuid == tankInfo.Guid)
                {
                    return 70f; // High priority - help tank regain/hold threat
                }
            }

            return 0f;
        }
    }
}
