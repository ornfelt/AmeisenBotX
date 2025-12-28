using AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Priority.Basic;
using AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Priority.Special;
using AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Validation.Basic;
using AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Validation.Special;
using AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Validation.Util;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Logics.Dps
{
    /// <summary>
    /// SOTA DPS target selection with sticky targeting and pre-combat detection.
    /// - Detects threats before combat starts
    /// - Sticky targeting prevents thrashing
    /// - Distance is a soft factor, not a hard limit
    /// </summary>
    public class SimpleDpsTargetSelectionLogic : BasicTargetSelectionLogic
    {
        // ===== SCORING WEIGHTS =====
        private const float THREAT_WEIGHT = 4.0f;       // Targets attacking us/party (highest!)
        private const float PROTECTION_WEIGHT = 3.5f;   // Targets attacking our healer/caster
        private const float KILLABLE_WEIGHT = 2.5f;     // Low HP = quick kill
        private const float TYPE_WEIGHT = 2.0f;         // Healer > Caster > Melee
        private const float FOCUS_WEIGHT = 1.5f;        // Party attacking = synergy
        private const float DISTANCE_WEIGHT = 0.5f;     // Soft preference for closer (not a limit!)
        private const float INCOMING_THREAT_WEIGHT = 3.0f; // Enemies heading towards us

        // Sticky targeting threshold: only switch if new target is X% better
        private const float STICKY_THRESHOLD = 0.2f; // 20% better required to switch
        private ulong currentTargetGuid = 0;
        private DateTime lastTargetSwitch = DateTime.MinValue;
        private static readonly TimeSpan MinTargetSwitchInterval = TimeSpan.FromSeconds(1);

        // CC spell names to ignore
        private static readonly HashSet<string> CCSpellNames =
        [
            "Polymorph", "Hex", "Hibernate", "Sap", "Freezing Trap",
            "Fear", "Psychic Scream", "Seduction", "Mind Control",
            "Shackle Undead", "Repentance", "Banish", "Cyclone"
        ];

        private static readonly HashSet<WowClass> HealerClasses = [WowClass.Priest, WowClass.Paladin, WowClass.Shaman, WowClass.Druid];
        private static readonly HashSet<WowClass> CasterClasses = [WowClass.Mage, WowClass.Warlock, WowClass.Priest, WowClass.Shaman, WowClass.Druid];

        // MODULAR TARGET SELECTION
        private TargetSelectionManager SelectionManager { get; }

        public SimpleDpsTargetSelectionLogic(AmeisenBotInterfaces bot) : base(bot)
        {
            // Initialize modular target selection system
            SelectionManager = new TargetSelectionManager(bot);

            // NOTE: Removed IsInCombatTargetValidator - we want pre-combat detection!
            TargetValidator.Add(new IsAttackableTargetValidator(bot));
            TargetValidator.Add(new IsThreatTargetValidator(bot));
            TargetValidator.Add(new DungeonTargetValidator(bot));
            TargetValidator.Add(new CachedTargetValidator(new IsReachableTargetValidator(bot), TimeSpan.FromSeconds(4)));

            // CRITICAL: Tank assist FIRST = highest priority in groups
            TargetPrioritizer.Add(new TankAssistTargetPrioritizer(bot));
            TargetPrioritizer.Add(new ListTargetPrioritizer());
            TargetPrioritizer.Add(new DungeonTargetPrioritizer(bot));
        }

        public override bool SelectTarget(out IEnumerable<IWowUnit> possibleTargets)
        {
            // Get all valid hostile units that won't cause accidental pulls
            List<IWowUnit> allValidUnits = Bot.Objects.All.OfType<IWowUnit>()
                .Where(e => TargetValidator.IsValid(e) && !IsCrowdControlled(e) && IsSafeToAttack(e))
                .ToList();

            if (allValidUnits.Count == 0)
            {
                possibleTargets = null;
                currentTargetGuid = 0;
                return false;
            }

            // MODULAR SELECTION: Delegate to SelectionManager
            IWowUnit bestTarget = SelectionManager.SelectBestTarget(allValidUnits);

            if (bestTarget == null)
            {
                // No valid target found by modules - return null to prevent targeting loop
                possibleTargets = null;
                currentTargetGuid = 0;
                return false;
            }

            // STICKY TARGETING: Keep current target unless switching is beneficial
            // Re-evaluate with sticky logic if we have a current target
            if (currentTargetGuid != 0 && DateTime.UtcNow - lastTargetSwitch < MinTargetSwitchInterval)
            {
                // Check if current target is still in valid targets
                IWowUnit currentTarget = allValidUnits.FirstOrDefault(u => u.Guid == currentTargetGuid);
                if (currentTarget != null)
                {
                    // Keep current target for stability (only switch after interval)
                    bestTarget = currentTarget;
                }
            }

            // Update sticky target tracking
            if (bestTarget.Guid != currentTargetGuid)
            {
                currentTargetGuid = bestTarget.Guid;
                lastTargetSwitch = DateTime.UtcNow;
            }

            // CRITICAL FIX: Return sorted list with BEST TARGET FIRST
            // The caller (TryFindTarget) uses FirstOrDefault() to get the target
            possibleTargets = allValidUnits
                .OrderByDescending(u => u.Guid == bestTarget.Guid) // Best target first
                .ThenBy(u => u.DistanceTo(Bot.Player));            // Then by distance

            return true;
        }

        private float ScoreTarget(IWowUnit unit)
        {
            float score = 0f;

            // ===== THREAT: Is it attacking us or party? (HIGHEST PRIORITY) =====
            if (unit.TargetGuid == Bot.Player.Guid)
            {
                score += THREAT_WEIGHT * 3f; // Attacking us = CRITICAL (increased from 2f to 3f)
            }
            else if (Bot.Objects.PartymemberGuids.Contains(unit.TargetGuid))
            {
                score += THREAT_WEIGHT;
            }

            // ===== INCOMING THREAT: Hostile and near party (pre-combat detection) =====
            // This helps detect enemies before they attack
            if (unit.IsInCombat && unit.TargetGuid != 0)
            {
                // Already handled above
            }
            else if (IsIncomingThreat(unit))
            {
                score += INCOMING_THREAT_WEIGHT; // About to attack us
            }

            // ===== PROTECTION: Is it attacking our healer/caster? =====
            score += PROTECTION_WEIGHT * GetProtectionScore(unit);

            // ===== KILLABLE: Low HP = finish it off =====
            // Enhanced: Extra bonus for very low HP (execute priority)
            float healthRatio = (100f - (float)unit.HealthPercentage) / 100f;
            if (unit.HealthPercentage < 10f)
            {
                healthRatio *= 3f; // Execute priority: <10% HP gets massive bonus
            }
            else if (unit.HealthPercentage < 20f)
            {
                healthRatio *= 2f; // Finish-off priority: <20% HP gets double bonus
            }
            score += KILLABLE_WEIGHT * healthRatio;

            // ===== TYPE: Healer > Caster > Melee =====
            score += TYPE_WEIGHT * (GetTargetTypePriorityScore(unit) / 5f);

            // ===== FOCUS: Party synergy bonus =====
            score += FOCUS_WEIGHT * GetPartyFocusScore(unit);

            // ===== DISTANCE: Soft preference (NOT a hard limit!) =====
            // Further targets get slightly lower score, but are still valid
            float distance = unit.DistanceTo(Bot.Player);
            float distanceScore = Math.Max(0f, 1f - (distance / 100f)); // 100f = very soft falloff
            score += DISTANCE_WEIGHT * distanceScore;

            return score;
        }

        /// <summary>
        /// Detects enemies that are likely about to attack us (pre-combat).
        /// </summary>
        private bool IsIncomingThreat(IWowUnit unit)
        {
            if (Bot.Player == null)
            {
                return false;
            }

            // Enemy is hostile and close to us or party
            float distanceToPlayer = unit.DistanceTo(Bot.Player);
            if (distanceToPlayer < 40f)
            {
                return true;
            }

            // Enemy is targeting someone in our party (about to attack)
            return Bot.Objects.PartymemberGuids.Any(g =>
                Bot.GetWowObjectByGuid<IWowUnit>(g) is IWowUnit member &&
                unit.DistanceTo(member) < 40f);
        }

        private float GetProtectionScore(IWowUnit unit)
        {
            if (unit.TargetGuid == 0)
            {
                return 0f;
            }

            if (!Bot.Objects.PartymemberGuids.Contains(unit.TargetGuid))
            {
                return 0f;
            }

            if (Bot.Party != null)
            {
                if (Bot.Party.IsHealer(unit.TargetGuid))
                {
                    return 1.0f;
                }

                if (Bot.Party.IsCaster(unit.TargetGuid))
                {
                    return 0.7f;
                }
            }
            else
            {
                IWowUnit victim = Bot.GetWowObjectByGuid<IWowUnit>(unit.TargetGuid);
                if (victim is IWowPlayer player)
                {
                    if (HealerClasses.Contains(player.Class))
                    {
                        return 1.0f;
                    }

                    if (CasterClasses.Contains(player.Class))
                    {
                        return 0.7f;
                    }
                }
            }
            return 0f;
        }

        private float GetTargetTypePriorityScore(IWowUnit unit)
        {
            return unit is IWowPlayer player
                ? HealerClasses.Contains(player.Class) ? 5f : CasterClasses.Contains(player.Class) ? 4f : 2f
                : unit.IsCasting || unit.CurrentlyCastingSpellId != 0 ? 4f : unit.MaxMana > 0 && unit.ManaPercentage > 30f ? 3f : 1f;
        }

        private float GetPartyFocusScore(IWowUnit unit)
        {
            if (!Bot.Objects.Partymembers.Any())
            {
                return 0f;
            }

            int focusCount = Bot.Objects.Partymembers.Count(p => p.Guid != Bot.Player.Guid && p.TargetGuid == unit.Guid);
            return Math.Min(1f, focusCount * 0.25f);
        }

        private bool IsCrowdControlled(IWowUnit unit)
        {
            return unit.Auras.Any(aura => CCSpellNames.Contains(Bot.Db.GetSpellName(aura.SpellId)));
        }

        /// <summary>
        /// Prevents accidental pulls. Target is safe to attack if:
        /// 1. Unit is in combat with us or our party (attacking or being attacked)
        /// 2. Unit is being pulled by a party member (casting at it)
        /// 3. Player has this unit targeted (manual selection or combat class chose it)
        /// 4. We are solo (no party) and unit is hostile
        /// </summary>
        private bool IsSafeToAttack(IWowUnit unit)
        {
            // If player has this unit targeted (via manual selection or combat class), always allow
            if (Bot.Player.TargetGuid == unit.Guid)
            {
                return true;
            }

            // Already in combat with us or party = safe
            if (unit.IsInCombat)
            {
                // Unit is targeting player or party
                if (unit.TargetGuid == Bot.Player.Guid)
                {
                    return true;
                }

                if (Bot.Objects.PartymemberGuids.Contains(unit.TargetGuid))
                {
                    return true;
                }

                if (Bot.Objects.PartyPetGuids.Contains(unit.TargetGuid))
                {
                    return true;
                }

                // Party/pets are targeting/attacking this unit
                if (Bot.Objects.Partymembers.Any(p => p.TargetGuid == unit.Guid))
                {
                    return true;
                }

                if (Bot.Objects.PartyPets.Any(p => p.TargetGuid == unit.Guid))
                {
                    return true;
                }
            }

            // Not in combat yet - check if party is intentionally pulling it
            // (party member casting with this unit as target)
            foreach (IWowUnit member in Bot.Objects.Partymembers)
            {
                if (member.TargetGuid == unit.Guid && member.IsCasting)
                {
                    return true; // Party member is casting at this target = intentional pull
                }
            }

            // Check if player is pulling it (casting at it)
            if (Bot.Player.TargetGuid == unit.Guid && Bot.Player.IsCasting)
            {
                return true;
            }

            // Solo mode (no party) - allow attacking any hostile unit
            return !Bot.Objects.Partymembers.Any() && Bot.IsHostileReaction(unit);
        }
    }
}
