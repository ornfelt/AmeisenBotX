using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
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
    /// Optimized Protection Warrior for WotLK 3.3.5.
    /// Features: Heroic Throw pull, Vigilance, Intervene for party protection, better defensive management.
    /// </summary>
    [CombatClassMetadata("[WotLK335a] Warrior Protection", "Jannis")]
    public class WarriorProtection : BasicCombatClass
    {
        public WarriorProtection(AmeisenBotInterfaces bot) : base(bot)
        {
            // ===== CONFIGURABLES =====
            Configurables.TryAdd("VigilanceEnabled", true);
            Configurables.TryAdd("HeroicThrowPull", true);
            Configurables.TryAdd("UseIntervene", true);
            Configurables.TryAdd("ShieldBlockRage", 20);
            Configurables.TryAdd("HeroicStrikeRage", 50);

            // ===== SELF BUFFS =====
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, WarriorWotlk.CommandingShout, () => TryCastSpell(WarriorWotlk.CommandingShout, 0, true)));

            // ===== TARGET DEBUFFS =====
            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, WarriorWotlk.DemoralizingShout, () => TryCastSpell(WarriorWotlk.DemoralizingShout, Bot.Wow.TargetGuid, true)));
            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, WarriorWotlk.ThunderClap, () => TryCastSpellWarrior(WarriorWotlk.ThunderClap, WarriorWotlk.DefensiveStance, Bot.Wow.TargetGuid, true)));

            // ===== INTERRUPTS =====
            InterruptManager.InterruptSpells = new()
            {
                { 0, (x) => TryCastSpellWarrior(WarriorWotlk.ShieldBash, WarriorWotlk.DefensiveStance, x.Guid, true) },
                { 1, (x) => TryCastSpell(WarriorWotlk.ConcussionBlow, x.Guid, true) },
                { 2, (x) => TryCastSpellWarrior(WarriorWotlk.SpellReflection, WarriorWotlk.DefensiveStance, 0) }
            };

            HeroicStrikeEvent = new(TimeSpan.FromSeconds(2));
            VigilanceEvent = new(TimeSpan.FromSeconds(5));
        }

        public override string Description => "Optimized Protection Warrior with Heroic Throw pull, Vigilance, Intervene, and smart defensive cooldowns.";

        public override string DisplayName2 => "Warrior Protection";

        public override WowSpecialization Specialization => WowSpecialization.WarriorProtection;

        public override bool HandlesMovement => true;

        public override bool IsMelee => true;

        public override IItemComparator ItemComparator { get; set; } = new BasicStaminaComparator(
        [
            WowArmorType.Idol,
            WowArmorType.Libram,
            WowArmorType.Sigil,
            WowArmorType.Totem,
            WowArmorType.Cloth,
            WowArmorType.Leather
        ],
        [
            WowWeaponType.SwordTwoHand,
            WowWeaponType.MaceTwoHand,
            WowWeaponType.AxeTwoHand,
            WowWeaponType.Misc,
            WowWeaponType.Staff,
            WowWeaponType.Polearm,
            WowWeaponType.Thrown,
            WowWeaponType.Wand,
            WowWeaponType.Dagger
        ]);

        public override WowRole Role => WowRole.Tank;

        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 1, new(1, 1, 3) },  // Improved Heroic Strike
                { 2, new(1, 2, 5) },  // Deflection
                { 4, new(1, 4, 2) },  // Improved Charge
                { 9, new(1, 9, 2) },  // Improved Thunder Clap
                { 10, new(1, 10, 3) }, // Incite
            },
            Tree2 = new()
            {
                { 1, new(2, 1, 3) },  // Armored to the Teeth
            },
            Tree3 = new()
            {
                { 2, new(3, 2, 5) },  // Anticipation
                { 3, new(3, 3, 3) },  // Shield Specialization
                { 4, new(3, 4, 3) },  // Improved Revenge
                { 5, new(3, 5, 5) },  // Shield Mastery
                { 6, new(3, 6, 1) },  // Toughness
                { 7, new(3, 7, 2) },  // Improved Spell Reflection
                { 8, new(3, 8, 2) },  // Improved Disarm
                { 9, new(3, 9, 5) },  // Puncture
                { 13, new(3, 13, 2) }, // Gag Order
                { 14, new(3, 14, 1) }, // Last Stand
                { 15, new(3, 15, 2) }, // Improved Defensive Stance
                { 16, new(3, 16, 5) }, // Vitality
                { 17, new(3, 17, 2) }, // Warbringer
                { 18, new(3, 18, 1) }, // Devastate
                { 20, new(3, 20, 3) }, // Critical Block
                { 22, new(3, 22, 1) }, // Sword and Board
                { 23, new(3, 23, 1) }, // Damage Shield
                { 24, new(3, 24, 3) }, // One-Handed Weapon Specialization
                { 25, new(3, 25, 3) }, // Improved Defensive Stance
                { 26, new(3, 26, 2) }, // Vigilance
                { 27, new(3, 27, 1) }, // Shockwave
            },
        };

        public override bool UseAutoAttacks => true;

        public override string Version => "2.0";

        public override bool WalkBehindEnemy => false;

        public override WowClass WowClass => WowClass.Warrior;

        public override WowVersion WowVersion => WowVersion.WotLK335a;

        private TimegatedEvent HeroicStrikeEvent { get; }
        private TimegatedEvent VigilanceEvent { get; }

        public override void Execute()
        {
            base.Execute();

            if (TryFindTarget(TargetProviderTank, out _))
            {
                // ===== FEAR/DAZE BREAK =====
                if ((Bot.Player.IsFleeing || Bot.Player.IsDazed || Bot.Player.IsDisarmed)
                    && TryCastSpell(WarriorWotlk.BerserkerRage, 0, false))
                {
                    return;
                }

                float distanceToTarget = Bot.Target.Position.GetDistance(Bot.Player.Position);

                // ===== VIGILANCE MANAGEMENT =====
                // ===== VIGILANCE MANAGEMENT =====
                if (Configurables["VigilanceEnabled"] && VigilanceEvent.Run())
                {
                    ManageVigilance();
                }

                // ===== MOVEMENT & POSITIONING =====
                if (!Bot.Tactic.PreventMovement)
                {
                    IWowUnit targetOfTarget = Bot.GetWowObjectByGuid<IWowUnit>(Bot.Target.TargetGuid);

                    if (targetOfTarget != null && targetOfTarget.Guid == Bot.Player.Guid)
                    {
                        // We have aggro - position between party and enemy
                        Vector3 direction = Bot.Player.Position - Bot.Objects.CenterPartyPosition;
                        direction.Normalize();

                        Vector3 bestTankingSpot = Bot.Objects.CenterPartyPosition + (direction * 12.0f);
                        float distanceToBestTankingSpot = Bot.Player.DistanceTo(bestTankingSpot);

                        if (distanceToBestTankingSpot > 4.0f)
                        {
                            Bot.Movement.SetMovementAction(MovementAction.Move, Bot.Target.Position);
                        }
                    }
                    else
                    {
                        // Target not on us - need to get aggro
                        if (distanceToTarget > Bot.Player.MeleeRangeTo(Bot.Target))
                        {
                            Bot.Movement.SetMovementAction(MovementAction.Chase, Bot.Target.Position);
                        }
                    }
                }

                // ===== INTERVENE - Protect Party Members =====
                if (Configurables["UseIntervene"])
                {
                    IWowUnit partyMemberInDanger = Bot.Objects.Partymembers
                        .FirstOrDefault(e => e != null && !e.IsDead && e.Guid != Bot.Player.Guid
                            && e.HealthPercentage < 50
                            && e.Position.GetDistance(Bot.Player.Position) < 25
                            && e.Position.GetDistance(Bot.Player.Position) > 8);

                    if (partyMemberInDanger != null
                        && TryCastSpellWarrior(WarriorWotlk.Intervene, WarriorWotlk.DefensiveStance, partyMemberInDanger.Guid))
                    {
                        return;
                    }
                }

                // ===== RANGED COMBAT (Before Charge Range) =====
                if (distanceToTarget is > (float)25.0 and < (float)30.0)
                {
                    // Heroic Throw for ranged pull
                    if (Configurables["HeroicThrowPull"]
                        && TryCastSpell(WarriorWotlk.HeroicThrow, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }
                }

                // ===== GAP CLOSER =====
                if (distanceToTarget > 8.0)
                {
                    // Charge in Battle Stance
                    if (TryCastSpellWarrior(WarriorWotlk.Charge, WarriorWotlk.BattleStance, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }

                    // Intercept as backup (Berserker Stance)
                    if (distanceToTarget > 8.0 && distanceToTarget < 25.0
                        && TryCastSpellWarrior(WarriorWotlk.Intercept, WarriorWotlk.BerserkerStance, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }
                }
                else
                {
                    // ===== MELEE COMBAT =====

                    // Heroic Strike as rage dump (off-GCD)
                    if (Bot.Player.Rage > Configurables["HeroicStrikeRage"] && HeroicStrikeEvent.Run())
                    {
                        TryCastSpell(WarriorWotlk.HeroicStrike, Bot.Wow.TargetGuid, true);
                    }

                    int nearEnemies = Bot.GetNearEnemies<IWowUnit>(Bot.Player.Position, 10.0f).Count();

                    // ===== TAUNT MANAGEMENT =====
                    if (Bot.Target.TargetGuid != Bot.Wow.PlayerGuid)
                    {
                        // Target not attacking us
                        if ((nearEnemies > 3 && TryCastSpell(WarriorWotlk.ChallengingShout, 0, true))
                            || TryCastSpellWarrior(WarriorWotlk.Taunt, WarriorWotlk.DefensiveStance, Bot.Wow.TargetGuid)
                            || TryCastSpell(WarriorWotlk.MockingBlow, Bot.Wow.TargetGuid, true))
                        {
                            return;
                        }
                    }

                    // ===== DEFENSIVE COOLDOWNS =====
                    if (ExecuteDefensiveCooldowns(nearEnemies))
                    {
                        return;
                    }

                    // ===== SPELL INTERRUPT/REFLECT =====
                    if (Bot.Target.IsCasting)
                    {
                        if (TryCastSpellWarrior(WarriorWotlk.ShieldBash, WarriorWotlk.DefensiveStance, Bot.Wow.TargetGuid)
                            || TryCastSpellWarrior(WarriorWotlk.SpellReflection, WarriorWotlk.DefensiveStance, 0))
                        {
                            return;
                        }
                    }

                    // ===== RAGE GENERATION =====
                    if (Bot.Player.HealthPercentage > 50 && Bot.Player.Rage < 30
                        && TryCastSpell(WarriorWotlk.Bloodrage, 0))
                    {
                        return;
                    }

                    // ===== DAMAGE ROTATION =====
                    if (ExecuteDamageRotation(nearEnemies))
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Manages Vigilance on highest threat party member.
        /// </summary>
        private bool ManageVigilance()
        {
            // Put Vigilance on party member doing most DPS (usually highest threat after tank)
            IWowUnit vigilanceTarget = Bot.Objects.Partymembers
                .Where(e => e != null && !e.IsDead && e.Guid != Bot.Player.Guid)
                .OrderByDescending(e => e.Level) // Rough approximation for DPS
                .FirstOrDefault();

            return vigilanceTarget != null
                && !vigilanceTarget.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == WarriorWotlk.Vigilance)
                && TryCastSpellWarrior(WarriorWotlk.Vigilance, WarriorWotlk.DefensiveStance, vigilanceTarget.Guid);
        }

        /// <summary>
        /// Execute defensive cooldowns based on health and situation.
        /// </summary>
        private bool ExecuteDefensiveCooldowns(int nearEnemies)
        {
            // Critical health - use all cooldowns
            if (Bot.Player.HealthPercentage < 25.0)
            {
                if (TryCastSpell(WarriorWotlk.LastStand, 0)
                    || TryCastSpellWarrior(WarriorWotlk.ShieldWall, WarriorWotlk.DefensiveStance, 0)
                    || TryCastSpell(WarriorWotlk.EnragedRegeneration, 0))
                {
                    return true;
                }
            }

            // Medium health - Shield Wall or Shield Block
            if (Bot.Player.HealthPercentage < 40.0)
            {
                if (TryCastSpellWarrior(WarriorWotlk.ShieldWall, WarriorWotlk.DefensiveStance, 0)
                    || TryCastSpellWarrior(WarriorWotlk.ShieldBlock, WarriorWotlk.DefensiveStance, Bot.Wow.TargetGuid, true))
                {
                    return true;
                }
            }

            // Use Shield Block proactively when rage available
            if (Bot.Player.Rage > Configurables["ShieldBlockRage"]
                && TryCastSpellWarrior(WarriorWotlk.ShieldBlock, WarriorWotlk.DefensiveStance, Bot.Wow.TargetGuid, true))
            {
                return true;
            }

            // Many enemies - Retaliation for extra damage
            return nearEnemies > 3 && Bot.Player.HealthPercentage > 50
                && TryCastSpellWarrior(WarriorWotlk.Retaliation, WarriorWotlk.BattleStance, 0);
        }

        /// <summary>
        /// Main damage rotation for threat generation.
        /// </summary>
        private bool ExecuteDamageRotation(int nearEnemies)
        {
            // Priority: Shield Slam > Revenge > Shockwave (AoE) > Devastate

            // Shield Slam - Highest threat, high priority
            if (TryCastSpell(WarriorWotlk.ShieldSlam, Bot.Wow.TargetGuid, true))
            {
                return true;
            }

            // Revenge - Free damage/threat when it procs
            if (TryCastSpellWarrior(WarriorWotlk.Revenge, WarriorWotlk.DefensiveStance, Bot.Wow.TargetGuid, true))
            {
                return true;
            }

            // AoE abilities for multiple enemies
            if (nearEnemies > 2 || Bot.Player.Rage > 40)
            {
                if (TryCastSpell(WarriorWotlk.Shockwave, Bot.Wow.TargetGuid, true))
                {
                    return true;
                }

                if (nearEnemies > 2
                    && TryCastSpellWarrior(WarriorWotlk.ThunderClap, WarriorWotlk.DefensiveStance, Bot.Wow.TargetGuid, true))
                {
                    return true;
                }

                // Cleave for multi-target (off-GCD substitute for Heroic Strike)
                if (nearEnemies > 2 && Bot.Player.Rage > 50 && HeroicStrikeEvent.Run())
                {
                    TryCastSpell(WarriorWotlk.Cleave, Bot.Wow.TargetGuid, true);
                }
            }

            // Concussion Blow for extra stun/threat
            if (TryCastSpell(WarriorWotlk.ConcussionBlow, Bot.Wow.TargetGuid, true))
            {
                return true;
            }

            // Devastate - Filler ability, builds Sunder Armor stacks
            if (TryCastSpell(WarriorWotlk.Devastate, Bot.Wow.TargetGuid, true))
            {
                return true;
            }

            // Victory Rush after kill
            return TryCastSpell(WarriorWotlk.VictoryRush, Bot.Wow.TargetGuid, true);
        }
    }
}
