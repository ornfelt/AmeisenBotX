using AmeisenBotX.Core.Managers.Party;
using AmeisenBotX.Wow.Objects;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Priority.Special
{
    /// <summary>
    /// Tank Assist Prioritizer: DPS bots attack what the tank is fighting.
    /// Prevents pulling extra mobs and ensures synergy with tank's threat.
    /// </summary>
    public class TankAssistTargetPrioritizer(AmeisenBotInterfaces bot) : ITargetPrioritizer
    {
        private AmeisenBotInterfaces Bot { get; } = bot;

        public bool HasPriority(IWowUnit unit)
        {
            // Skip if not in a party (solo mode)
            if (Bot.Party == null || !Bot.Objects.Partymembers.Any())
            {
                return false;
            }

            // Find the tank
            List<PartyMemberInfo> tanks = Bot.Party.GetTanks().ToList();
            if (tanks.Count == 0)
            {
                return false; // No tank in party, fall back to normal targeting
            }

            // Check each tank (in case of multiple tanks)
            foreach (PartyMemberInfo tankInfo in tanks)
            {
                // Find tank player object from party members
                IWowUnit tankPlayer = Bot.Objects.Partymembers.FirstOrDefault(p => p.Guid == tankInfo.Guid);
                if (tankPlayer == null || tankPlayer.IsDead)
                {
                    continue;
                }

                // CRITICAL: Return true if this unit is tank's target or attacking tank
                if (unit.Guid == tankPlayer.TargetGuid)
                {
                    return true; // Tank's current target = highest priority
                }

                // Tank has threat on this unit (unit is attacking tank)
                if (unit.TargetGuid == tankInfo.Guid)
                {
                    return true; // Tank is being attacked = high priority
                }

                // Check if tank is auto-attacking this unit
                if (tankPlayer.IsAutoAttacking && tankPlayer.TargetGuid == unit.Guid)
                {
                    return true; // Tank is building threat = medium-high priority
                }
            }

            // Not related to tank's combat
            return false;
        }
    }
}
