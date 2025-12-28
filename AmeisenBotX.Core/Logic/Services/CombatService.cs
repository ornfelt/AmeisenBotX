using AmeisenBotX.Common.Utils;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Logic.Services
{
    /// <summary>
    /// Service for combat decisions.
    /// Manages combat state detection and target referencing.
    /// </summary>
    public class CombatService(AmeisenBotInterfaces bot, AmeisenBotConfig config)
    {
        private readonly AmeisenBotInterfaces Bot = bot;
        private readonly AmeisenBotConfig Config = config;

        // Combat detection throttle
        private readonly TimegatedEvent CombatCheckEvent = new(TimingConfig.CombatStateUpdate);

        // State
        public bool ShouldFight { get; private set; }
        public bool ArePartymembersInFight { get; private set; }

        /// <summary>Reason for current decision - for debugging.</summary>
        public string LastReason { get; private set; } = "None";

        public void CheckCombatState()
        {
            // Only run throttled combat checks, unless strictly forced?
            // For now keeping throttle to match original behavior performance-wise.
            if (!CombatCheckEvent.Run())
            {
                return;
            }

            // Self in combat
            bool selfInCombat = Bot.Player?.IsInCombat ?? false;

            // Party members in combat (within support range)
            bool partymembersInCombat = Bot.Objects.Partymembers
                .Any(e => e.IsInCombat && e.DistanceTo(Bot.Player) < Config.SupportRange);

            // Enemies in combat with party
            bool enemiesInCombatWithParty = Bot.GetEnemiesOrNeutralsInCombatWithParty<IWowUnit>(
                Bot.Player.Position, Config.SupportRange).Any();

            // Party member actively engaging a hostile target
            bool partymemberEngaging = CheckPartymemberEngaging();

            ArePartymembersInFight = partymembersInCombat || enemiesInCombatWithParty || partymemberEngaging;
            ShouldFight = selfInCombat || ArePartymembersInFight;

            // Track reason for debugging
            LastReason = ShouldFight
                ? selfInCombat ? "Self in combat" :
                             partymembersInCombat ? "Party member in combat" :
                             enemiesInCombatWithParty ? "Enemies attacking party" :
                             partymemberEngaging ? "Party member engaging enemy" : "Unknown"
                : "No combat";
        }

        /// <summary>
        /// Check if any party member is actively engaging an enemy.
        /// Also assists with target acquisition.
        /// </summary>
        private bool CheckPartymemberEngaging()
        {
            if (Bot.Player == null)
            {
                return false;
            }

            IEnumerable<IWowUnit> engagingPartyMembers = Bot.Objects.Partymembers
                .Where(e => e.Guid != Bot.Player.Guid
                    && e.DistanceTo(Bot.Player) < Config.SupportRange
                    && (e.IsCasting || e.IsAutoAttacking || e.IsInCombat));

            foreach (IWowUnit partyMember in engagingPartyMembers)
            {
                IWowUnit targetOfPartyMember = Bot.GetWowObjectByGuid<IWowUnit>(partyMember.TargetGuid);

                if (targetOfPartyMember != null
                    && !targetOfPartyMember.IsDead
                    && Bot.Db.GetReaction(Bot.Player, targetOfPartyMember) is WowUnitReaction.Hostile or WowUnitReaction.Neutral)
                {
                    // Assist with targeting if we have no valid target
                    // Note: This side effect is kept from original module logic.
                    if (Bot.Target == null || Bot.Target.IsDead)
                    {
                        Bot.Wow.ChangeTarget(targetOfPartyMember.Guid);
                    }
                    return true;
                }
            }

            return false;
        }

        public void Reset()
        {
            ShouldFight = false;
            ArePartymembersInFight = false;
        }
    }
}
