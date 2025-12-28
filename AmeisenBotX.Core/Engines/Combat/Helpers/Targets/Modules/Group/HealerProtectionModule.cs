using AmeisenBotX.Core.Managers.Party;
using AmeisenBotX.Wow.Objects;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Modules.Group
{
    /// <summary>
    /// PERFECTED Healer protection module with comprehensive safety checks.
    /// Prioritizes enemies attacking healers to prevent deaths.
    /// Only active in groups with healers present.
    /// </summary>
    public class HealerProtectionModule : ITargetSelectionModule
    {
        public string Name => "HealerProtection";

        public bool IsActive(AmeisenBotInterfaces bot)
        {
            // Comprehensive activation checks
            if (bot?.Party == null)
            {
                return false;
            }

            // Edge case: Party exists but no healers assigned
            IEnumerable<PartyMemberInfo> healers = bot.Party.GetHealers();
            return healers != null && healers.Any();
        }

        public float GetPriorityBonus(IWowUnit target, AmeisenBotInterfaces bot)
        {
            // Bulletproof null checks
            if (bot?.Party == null || target == null)
            {
                return 0f;
            }

            // Edge case: Target has no target (not attacking anyone)
            if (target.TargetGuid == 0)
            {
                return 0f;
            }

            // Edge case: Target is dead
            if (target.IsDead)
            {
                return 0f;
            }

            IEnumerable<PartyMemberInfo> healers = bot.Party.GetHealers();
            if (healers == null)
            {
                return 0f;
            }

            foreach (PartyMemberInfo healerInfo in healers)
            {
                // Edge case: Healer info is null
                if (healerInfo == null)
                {
                    continue;
                }

                // Edge case: Check if healer is still alive (via party members)
                if (bot.Objects?.Partymembers != null)
                {
                    IWowUnit healerPlayer = bot.Objects.Partymembers.FirstOrDefault(p => p.Guid == healerInfo.Guid);

                    // Edge case: Healer not found (left group, disconnected)
                    if (healerPlayer == null)
                    {
                        continue;
                    }

                    // Edge case: Healer is already dead - no need to protect
                    if (healerPlayer.IsDead)
                    {
                        continue;
                    }
                }

                // CRITICAL: Enemy is attacking our healer
                if (target.TargetGuid == healerInfo.Guid)
                {
                    // Extra priority if target is in melee range of healer (imminent danger)
                    if (bot.Objects?.Partymembers != null)
                    {
                        IWowUnit healerPlayer = bot.Objects.Partymembers.FirstOrDefault(p => p.Guid == healerInfo.Guid);
                        if (healerPlayer != null)
                        {
                            float distanceToHealer = target.DistanceTo(healerPlayer);
                            if (distanceToHealer < 10f)
                            {
                                return 80f; // CRITICAL - melee range, healer in immediate danger!
                            }
                        }
                    }

                    return 75f; // Very high priority - protect the healer!
                }
            }

            return 0f;
        }
    }
}
