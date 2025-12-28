using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.WowWotlk.Constants.Classes;
using System;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Jannis.Wotlk335a
{
    /// <summary>
    /// Comprehensive Assassination Rogue CombatClass for WotLK 3.3.5.
    /// Supports both leveling (Sinister Strike fallback) and max-level gameplay (Mutilate/Envenom).
    /// Features: Stealth openers, Hunger for Blood management, defensive cooldowns, and AoE support.
    /// </summary>
    public class RogueAssassination : BasicCombatClass
    {
        // Cooldown tracking for Vanish offensive usage
        private DateTime lastVanishTime = DateTime.MinValue;
        private static readonly TimeSpan VanishCooldown = TimeSpan.FromMinutes(3);

        // Smart stealth detection ranges
        private const float EnemyPlayerDetectionRange = 35.0f;
        private const float HostileNpcDetectionRange = 15.0f;
        private const float TargetingDetectionRange = 30.0f;

        public RogueAssassination(AmeisenBotInterfaces bot) : base(bot)
        {
            // ===== AURA MANAGEMENT =====
            // Keep Slice and Dice active (refreshed by Envenom via Cut to the Chase talent)
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, RogueWotlk.SliceAndDice, () =>
                Bot.Player.ComboPoints >= 1 && TryCastSpellRogue(RogueWotlk.SliceAndDice, 0, true, true, 1)));

            // Keep Hunger for Blood active (requires bleed on target)
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, RogueWotlk.HungerForBlood, () =>
                TargetHasBleed() && TryCastSpellRogue(RogueWotlk.HungerForBlood, 0, true)));

            // Cold Blood for burst damage
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, RogueWotlk.ColdBlood, () =>
                Bot.Player.ComboPoints >= 4 && TryCastSpellRogue(RogueWotlk.ColdBlood, 0, true)));

            // ===== TARGET DEBUFFS =====
            // Keep Rupture on target for Hunger for Blood activation
            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, RogueWotlk.Rupture, () =>
                !TargetHasBleed() && Bot.Player.ComboPoints >= 1 && TryCastSpellRogue(RogueWotlk.Rupture, Bot.Wow.TargetGuid, true, true, 1)));

            // ===== INTERRUPT =====
            InterruptManager.InterruptSpells = new()
            {
                { 0, (x) => TryCastSpellRogue(RogueWotlk.Kick, x.Guid, true) },
                { 1, (x) => TryCastSpellRogue(RogueWotlk.KidneyShot, x.Guid, true, true, 2) }
            };
        }

        public override string Description => "Complete Assassination Rogue with Stealth openers, Hunger for Blood, Envenom rotation, defensives, and AoE. Supports leveling and max-level gameplay.";

        public override string DisplayName2 => "Rogue Assassination";

        public override WowSpecialization Specialization => WowSpecialization.RogueAssassination;

        public override bool HandlesMovement => false;

        public override bool IsMelee => true;

        public override IItemComparator ItemComparator { get; set; } = new BasicAgilityComparator([WowArmorType.Shield]);

        public override WowRole Role => WowRole.Dps;

        public override TalentTree Talents { get; } = new()
        {
            // Assassination Tree (51 points)
            Tree1 = new()
            {
                { 1, new(1, 1, 5) },   // Improved Eviscerate
                { 3, new(1, 3, 2) },   // Malice
                { 4, new(1, 4, 5) },   // Ruthlessness
                { 5, new(1, 5, 2) },   // Blood Spatter
                { 6, new(1, 6, 3) },   // Puncturing Wounds
                { 9, new(1, 9, 5) },   // Lethality
                { 10, new(1, 10, 3) }, // Vile Poisons
                { 11, new(1, 11, 5) }, // Improved Poisons
                { 13, new(1, 13, 1) }, // Cold Blood
                { 14, new(1, 14, 3) }, // Quick Recovery
                { 16, new(1, 16, 5) }, // Seal Fate
                { 17, new(1, 17, 2) }, // Murder
                { 19, new(1, 19, 1) }, // Overkill
                { 21, new(1, 21, 3) }, // Focused Attacks
                { 22, new(1, 22, 3) }, // Find Weakness
                { 23, new(1, 23, 3) }, // Master Poisoner
                { 24, new(1, 24, 1) }, // Mutilate
                { 26, new(1, 26, 5) }, // Cut to the Chase
                { 27, new(1, 27, 1) }, // Hunger for Blood
            },
            // Combat Tree (13 points)
            Tree2 = new()
            {
                { 2, new(2, 2, 5) },   // Improved Sinister Strike
                { 3, new(2, 3, 5) },   // Dual Wield Specialization
                { 6, new(2, 6, 3) },   // Precision
            },
            // Subtlety Tree (7 points)
            Tree3 = new()
            {
                { 1, new(3, 1, 5) },   // Relentless Strikes
                { 3, new(3, 3, 2) },   // Opportunity
            },
        };

        public override bool UseAutoAttacks => true;

        public override string Version => "1.0";

        public override bool WalkBehindEnemy => true;

        public override WowClass WowClass => WowClass.Rogue;

        public override WowVersion WowVersion => WowVersion.WotLK335a;

        /// <summary>
        /// Checks if the current target has a bleed effect (required for Hunger for Blood).
        /// </summary>
        private bool TargetHasBleed()
        {
            if (Bot.Target == null)
            {
                return false;
            }

            // Check for common bleed debuffs
            string[] bleedEffects = [RogueWotlk.Rupture, RogueWotlk.Garrote, "Deep Wound", "Rend", "Lacerate", "Rake", "Rip", "Mangle"];
            return Bot.Target.Auras.Any(a => bleedEffects.Contains(Bot.Db.GetSpellName(a.SpellId)));
        }

        /// <summary>
        /// Checks if we are in stealth.
        /// </summary>
        private bool IsStealthed()
        {
            return Bot.Player.Auras.Any(a => Bot.Db.GetSpellName(a.SpellId) is RogueWotlk.Stealth
                                          or RogueWotlk.Vanish);
        }

        /// <summary>
        /// Checks if the Overkill buff is active (20 sec after breaking stealth).
        /// </summary>
        private bool HasOverkillBuff()
        {
            return Bot.Player.Auras.Any(a => Bot.Db.GetSpellName(a.SpellId) == RogueWotlk.Overkill);
        }

        /// <summary>
        /// Count nearby enemies for AoE decisions.
        /// </summary>
        private int GetNearbyEnemyCount()
        {
            return Bot.GetNearEnemies<IWowUnit>(Bot.Player.Position, 8.0f).Count();
        }

        /// <summary>
        /// Determines if stealth is tactically valuable right now.
        /// Only stealth when combat is imminent to avoid the 30% movement speed penalty.
        /// </summary>
        private bool ShouldEnterStealth()
        {
            // 1. Stealth if we have a hostile target selected (ready to open)
            if (Bot.Target != null && !Bot.Target.IsDead
                && Bot.Db.GetReaction(Bot.Player, Bot.Target) is WowUnitReaction.Hostile or WowUnitReaction.Neutral)
            {
                return true;
            }

            // 2. Stealth if enemy players are nearby (PvP awareness)
            if (Bot.GetNearEnemies<IWowPlayer>(Bot.Player.Position, EnemyPlayerDetectionRange).Any())
            {
                return true;
            }

            // 3. Stealth if hostile NPCs are within engagement range (35y = pull range)
            if (Bot.GetNearEnemies<IWowUnit>(Bot.Player.Position, HostileNpcDetectionRange).Any())
            {
                return true;
            }

            // 4. Stealth if any enemy is targeting us or party members
            if (Bot.GetEnemiesInCombatWithParty<IWowUnit>(Bot.Player.Position, TargetingDetectionRange).Any())
            {
                return true;
            }

            // 5. Stealth if a party member is about to initiate combat (has hostile target or is attacking)
            return IsPartyMemberEngaging();
        }

        /// <summary>
        /// Checks if any party member has a hostile target or is attacking.
        /// </summary>
        private bool IsPartyMemberEngaging()
        {
            return Bot.Objects.Partymembers
                .Where(e => e.Guid != Bot.Player.Guid
                    && e.DistanceTo(Bot.Player) < TargetingDetectionRange)
                .Any(partyMember =>
                {
                    // Check if party member has a hostile target (even if not attacking yet)
                    IWowUnit target = Bot.GetWowObjectByGuid<IWowUnit>(partyMember.TargetGuid);
                    if (target != null && !target.IsDead
                        && Bot.Db.GetReaction(Bot.Player, target) is WowUnitReaction.Hostile or WowUnitReaction.Neutral)
                    {
                        // Stealth if they're casting, attacking, OR just targeting (about to pull)
                        return partyMember.IsCasting || partyMember.IsAutoAttacking
                            || target.DistanceTo(partyMember) < 30.0f; // Close to target = about to engage
                    }
                    return false;
                });
        }

        /// <summary>
        /// Check if Mutilate is available (level 50+).
        /// </summary>
        private bool HasMutilate()
        {
            return Bot.Character.SpellBook.IsSpellKnown(RogueWotlk.Mutilate);
        }

        /// <summary>
        /// Check if Envenom is available (level 62+).
        /// </summary>
        private bool HasEnvenom()
        {
            return Bot.Character.SpellBook.IsSpellKnown(RogueWotlk.Envenom);
        }

        public override void Execute()
        {
            base.Execute();

            if (TryFindTarget(TargetProviderDps, out _))
            {
                // ===== PRE-COMBAT STEALTH =====
                // Combat routine may start before we're actually in combat (e.g., party pulling)
                // Use this window to stealth for opener
                if (!Bot.Player.IsInCombat && !IsStealthed() && !Bot.Player.IsMounted
                    && Bot.Player.HealthPercentage > 50
                    && TryCastSpellRogue(RogueWotlk.Stealth, 0, false))
                {
                    return;
                }

                // ===== POSITIONING =====
                if (!Bot.Tactic.PreventMovement && Bot.Target != null)
                {
                    float dx = MathF.Cos(Bot.Target.Rotation);
                    float dy = MathF.Sin(Bot.Target.Rotation);

                    float backX = Bot.Target.Position.X - (dx * 1.5f);
                    float backY = Bot.Target.Position.Y - (dy * 1.5f);

                    Vector3 behindPos = new(backX, backY, Bot.Target.Position.Z);

                    // Always try to stick behind the target for Backstab/Mutilate bonus
                    if (Bot.Player.DistanceTo(behindPos) > Bot.Player.MeleeRangeTo(Bot.Target))
                    {
                        Bot.Movement.SetMovementAction(MovementAction.Chase, behindPos);
                    }
                }

                // ===== STEALTH OPENER =====
                if (IsStealthed() && Bot.Target != null)
                {
                    if (ExecuteStealthOpener())
                    {
                        return;
                    }
                }

                // ===== DEFENSIVE COOLDOWNS =====
                if (ExecuteDefensives())
                {
                    return;
                }

                // ===== GAP CLOSER =====
                if (Bot.Target != null && Bot.Target.Position.GetDistance(Bot.Player.Position) > 16
                    && TryCastSpellRogue(RogueWotlk.Sprint, 0, true))
                {
                    return;
                }

                // ===== AOE =====
                if (GetNearbyEnemyCount() >= 3
                    && TryCastSpellRogue(RogueWotlk.FanOfKnives, 0, true))
                {
                    return;
                }

                // ===== OFFENSIVE VANISH (for Overkill buff) =====
                // Only use Vanish offensively when we have high energy and no Overkill buff
                if (!HasOverkillBuff()
                    && Bot.Player.EnergyPercentage > 80
                    && (DateTime.UtcNow - lastVanishTime) > VanishCooldown
                    && Bot.Character.SpellBook.IsSpellKnown(RogueWotlk.Vanish)
                    && !CooldownManager.IsSpellOnCooldown(RogueWotlk.Vanish))
                {
                    // Use Vanish -> immediate Garrote for bleed + Overkill
                    if (TryCastSpellRogue(RogueWotlk.Vanish, 0, true))
                    {
                        lastVanishTime = DateTime.UtcNow;
                        return;
                    }
                }

                // ===== MAIN ROTATION =====
                if (ExecuteMainRotation())
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Execute stealth opener abilities.
        /// Priority: Garrote (for bleed) > Cheap Shot (if needs stun) > Ambush (damage)
        /// </summary>
        private bool ExecuteStealthOpener()
        {
            // Garrote first - applies bleed for Hunger for Blood
            if (!TargetHasBleed()
                && TryCastSpellRogue(RogueWotlk.Garrote, Bot.Wow.TargetGuid, true))
            {
                return true;
            }

            // Cheap Shot for stun (good for leveling/solo)
            if (Bot.Target.TargetGuid != Bot.Wow.PlayerGuid  // Target is not attacking us
                && TryCastSpellRogue(RogueWotlk.CheapShot, Bot.Wow.TargetGuid, true))
            {
                return true;
            }

            // Ambush for burst damage (requires dagger in main hand and being behind target)
            return TryCastSpellRogue(RogueWotlk.Ambush, Bot.Wow.TargetGuid, true);
        }

        /// <summary>
        /// Execute defensive cooldowns based on health thresholds.
        /// </summary>
        private bool ExecuteDefensives()
        {
            // Critical health - Vanish to escape
            if (Bot.Player.HealthPercentage < 15
                && TryCastSpellRogue(RogueWotlk.Vanish, 0, true))
            {
                lastVanishTime = DateTime.UtcNow;
                return true;
            }

            // Cloak of Shadows for magic damage / debuffs
            if (Bot.Player.HealthPercentage < 25
                && TryCastSpellRogue(RogueWotlk.CloakOfShadows, 0, true))
            {
                return true;
            }

            // Evasion for melee damage reduction
            if (Bot.Player.HealthPercentage < 40
                && TryCastSpellRogue(RogueWotlk.Evasion, 0, true))
            {
                return true;
            }

            // Blind the target if we're low and need time to recover
            return Bot.Player.HealthPercentage < 20
                && Bot.Target != null
                && TryCastSpellRogue(RogueWotlk.Blind, Bot.Wow.TargetGuid, true);
        }

        /// <summary>
        /// Main combat rotation.
        /// </summary>
        private bool ExecuteMainRotation()
        {
            // ===== FINISHERS (4-5 Combo Points) =====
            if (Bot.Player.ComboPoints >= 4)
            {
                // Envenom is our main finisher at max level (Cut to the Chase refreshes SnD)
                if (HasEnvenom()
                    && TryCastSpellRogue(RogueWotlk.Envenom, Bot.Wow.TargetGuid, true, true, 4))
                {
                    return true;
                }

                // Eviscerate as fallback for lower levels
                if (TryCastSpellRogue(RogueWotlk.Eviscerate, Bot.Wow.TargetGuid, true, true, 4))
                {
                    return true;
                }
            }

            // ===== COMBO POINT GENERATORS =====
            // Mutilate is our main generator at level 50+
            if (HasMutilate()
                && TryCastSpellRogue(RogueWotlk.Mutilate, Bot.Wow.TargetGuid, true))
            {
                return true;
            }

            // Backstab when behind target (good damage)
            if (TryCastSpellRogue(RogueWotlk.Backstab, Bot.Wow.TargetGuid, true))
            {
                return true;
            }

            // Sinister Strike as fallback (always works, no positioning required)
            return TryCastSpellRogue(RogueWotlk.SinisterStrike, Bot.Wow.TargetGuid, true);
        }

        public override void OutOfCombatExecute()
        {
            base.OutOfCombatExecute();

            // Only enter stealth when tactically valuable (avoids 30% movement speed penalty)
            if (!IsStealthed()
                && !Bot.Player.IsMounted
                && ShouldEnterStealth()
                && TryCastSpellRogue(RogueWotlk.Stealth, 0, false))
            {
                return;
            }
        }
    }
}
