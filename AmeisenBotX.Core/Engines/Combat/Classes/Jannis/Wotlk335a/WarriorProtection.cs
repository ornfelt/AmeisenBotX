﻿using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow335a.Constants;
using System;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Jannis.Wotlk335a
{
    /// <summary>
    /// Represents a warrior class specialization focused on providing protection and defense for allies.
    /// </summary>
    public class WarriorProtection : BasicCombatClass
    {
        /// <summary>
        /// Initializes a new instance of the WarriorProtection class with the specified bot.
        /// </summary>
        /// <param name="bot">The bot to use for the WarriorProtection class.</param>
        public WarriorProtection(AmeisenBotInterfaces bot) : base(bot)
        {
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Warrior335a.CommandingShout, () => TryCastSpell(Warrior335a.CommandingShout, 0, true)));

            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Warrior335a.DemoralizingShout, () => TryCastSpell(Warrior335a.DemoralizingShout, Bot.Wow.TargetGuid, true)));

            InterruptManager.InterruptSpells = new()
            {
                { 0, (x) => TryCastSpellWarrior(Warrior335a.ShieldBash, Warrior335a.DefensiveStance, x.Guid, true) },
                { 1, (x) => TryCastSpell(Warrior335a.ConcussionBlow, x.Guid, true) }
            };

            HeroicStrikeEvent = new(TimeSpan.FromSeconds(2));
        }

        /// <summary>
        /// Returns the description for the Protection Warrior spec CombatClass. 
        /// This CombatClass is specifically designed for leveling and is suitable for Dungeons and Questing.
        /// </summary>
        public override string Description => "Leveling ready CombatClass for the Protection Warrior spec. For Dungeons and Questing";

        /// <summary>
        /// Gets or sets the display name for the Warrior Protection class.
        /// </summary>
        public override string DisplayName2 => "Warrior Protection";

        /// <summary>
        /// Gets or sets a value indicating whether this object handles movement.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this object handles movement; otherwise, <c>false</c>.
        /// </value>
        public override bool HandlesMovement => true;

        /// <summary>
        /// Gets a value indicating whether this character is a melee fighter.
        /// </summary>
        /// <returns>True, indicating that this character is a melee fighter.</returns>
        public override bool IsMelee => true;

        /// <summary>
        /// Gets or sets the item comparator. The item comparator is used to compare items based on their stats.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicStaminaComparator(new()
        {
            WowArmorType.Idol,
            WowArmorType.Libram,
            WowArmorType.Sigil,
            WowArmorType.Totem,
            WowArmorType.Cloth,
            WowArmorType.Leather
        }, new()
        {
            WowWeaponType.SwordTwoHand,
            WowWeaponType.MaceTwoHand,
            WowWeaponType.AxeTwoHand,
            WowWeaponType.Misc,
            WowWeaponType.Staff,
            WowWeaponType.Polearm,
            WowWeaponType.Thrown,
            WowWeaponType.Wand,
            WowWeaponType.Dagger
        });

        /// <summary>
        /// Gets the role of the character as a tank in the World of Warcraft.
        /// </summary>
        public override WowRole Role => WowRole.Tank;

        /// <summary>
        /// Gets or sets the talent tree.
        /// </summary>
        /// <value>
        /// The talent tree.
        /// </value>
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 1, new(1, 1, 3) },
                { 2, new(1, 2, 5) },
                { 4, new(1, 4, 2) },
                { 9, new(1, 9, 2) },
                { 10, new(1, 10, 3) },
            },
            Tree2 = new()
            {
                { 1, new(2, 1, 3) },
            },
            Tree3 = new()
            {
                { 2, new(3, 2, 5) },
                { 3, new(3, 3, 3) },
                { 4, new(3, 4, 3) },
                { 5, new(3, 5, 5) },
                { 6, new(3, 6, 1) },
                { 7, new(3, 7, 2) },
                { 8, new(3, 8, 2) },
                { 9, new(3, 9, 5) },
                { 13, new(3, 13, 2) },
                { 14, new(3, 14, 1) },
                { 15, new(3, 15, 2) },
                { 16, new(3, 16, 5) },
                { 17, new(3, 17, 2) },
                { 18, new(3, 18, 1) },
                { 20, new(3, 20, 3) },
                { 22, new(3, 22, 1) },
                { 23, new(3, 23, 1) },
                { 24, new(3, 24, 3) },
                { 25, new(3, 25, 3) },
                { 26, new(3, 26, 2) },
                { 27, new(3, 27, 1) },
            },
        };

        /// <summary>
        /// Gets or sets a value indicating whether the object can use auto attacks.
        /// </summary>
        /// <returns>True if the object can use auto attacks; otherwise, False.</returns>
        public override bool UseAutoAttacks => true;

        /// <summary>
        /// Gets the version of the code.
        /// </summary>
        /// <returns>The version string, "1.1".</returns>
        public override string Version => "1.1";

        /// <summary>
        /// Gets or sets a value indicating whether the character can walk behind an enemy.
        /// </summary>
        /// <value>
        /// <c>false</c> indicating that the character cannot walk behind an enemy; otherwise, <c>true</c>.
        /// </value>
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the WoW class of type Warrior.
        /// </summary>
        public override WowClass WowClass => WowClass.Warrior;

        /// <summary>
        /// Gets the World of Warcraft version, which is set to Wrath
        /// of the Lich King 3.3.5a (WotLK335a).
        /// </summary>
        public override WowVersion WowVersion => WowVersion.WotLK335a;

        /// <summary>
        /// Gets or sets the TimegatedEvent object representing the Heroic Strike event.
        /// </summary>
        private TimegatedEvent HeroicStrikeEvent { get; }

        ///<summary>
        /// Executes the warrior's combat rotation for tanking.
        ///</summary>
        public override void Execute()
        {
            base.Execute();

            if (TryFindTarget(TargetProviderTank, out _))
            {
                if ((Bot.Player.IsFleeing
                    || Bot.Player.IsDazed
                    || Bot.Player.IsDisarmed)
                    && TryCastSpell(Warrior335a.BerserkerRage, 0, false))
                {
                    return;
                }

                float distanceToTarget = Bot.Target.Position.GetDistance(Bot.Player.Position);

                if (!Bot.Tactic.PreventMovement)
                {
                    IWowUnit targetOfTarget = Bot.GetWowObjectByGuid<IWowUnit>(Bot.Target.TargetGuid);

                    if (targetOfTarget != null && targetOfTarget.Guid == Bot.Player.Guid)
                    {
                        // if we have aggro, pull the unit to the best tanking spot
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
                        // target is not targeting us, we need to get aggro
                        if (distanceToTarget > Bot.Player.MeleeRangeTo(Bot.Target))
                        {
                            Bot.Movement.SetMovementAction(MovementAction.Chase, Bot.Target.Position);
                        }
                    }
                }

                if (distanceToTarget > 8.0)
                {
                    if (TryCastSpellWarrior(Warrior335a.Charge, Warrior335a.BattleStance, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }
                }
                else
                {
                    if (Bot.Player.Rage > 40
                        && HeroicStrikeEvent.Run())
                    {
                        TryCastSpell(Warrior335a.HeroicStrike, Bot.Wow.TargetGuid, true);
                    }

                    int nearEnemies = Bot.GetNearEnemies<IWowUnit>(Bot.Player.Position, 10.0f).Count();

                    if ((nearEnemies > 2 || Bot.Player.Rage > 40)
                        && TryCastSpellWarrior(Warrior335a.ThunderClap, Warrior335a.DefensiveStance, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }

                    if (Bot.Target.TargetGuid != Bot.Wow.PlayerGuid
                        && ((Bot.GetEnemiesInCombatWithMe<IWowUnit>(Bot.Player.Position, 12.0f).Count(e => Bot.Target.Position.GetDistance(e.Position) < 10.0) > 3 && TryCastSpell(Warrior335a.ChallengingShout, 0, true))
                            || TryCastSpellWarrior(Warrior335a.Taunt, Warrior335a.DefensiveStance, Bot.Wow.TargetGuid)))
                    {
                        return;
                    }

                    if (Bot.Player.HealthPercentage < 25.0
                        && TryCastSpellWarrior(Warrior335a.Retaliation, Warrior335a.BattleStance, 0))
                    {
                        return;
                    }

                    if (Bot.Player.HealthPercentage < 40.0
                        && (TryCastSpell(Warrior335a.LastStand, 0)
                            || TryCastSpellWarrior(Warrior335a.ShieldWall, Warrior335a.DefensiveStance, 0)
                            || TryCastSpellWarrior(Warrior335a.ShieldBlock, Warrior335a.DefensiveStance, Bot.Wow.TargetGuid, true)))
                    {
                        return;
                    }

                    if (Bot.Target.IsCasting
                        && (TryCastSpellWarrior(Warrior335a.ShieldBash, Warrior335a.DefensiveStance, Bot.Wow.TargetGuid)
                            || TryCastSpellWarrior(Warrior335a.SpellReflection, Warrior335a.DefensiveStance, 0)))
                    {
                        return;
                    }

                    if (Bot.Player.HealthPercentage > 50.0
                        && TryCastSpell(Warrior335a.Bloodrage, 0))
                    {
                        return;
                    }

                    if (TryCastSpell(Warrior335a.ShieldSlam, Bot.Wow.TargetGuid, true)
                        || TryCastSpell(Warrior335a.MockingBlow, Bot.Wow.TargetGuid, true)
                        || ((nearEnemies > 2 || Bot.Player.Rage > 40)
                            && TryCastSpell(Warrior335a.Shockwave, Bot.Wow.TargetGuid, true))
                        || TryCastSpell(Warrior335a.Devastate, Bot.Wow.TargetGuid, true)
                        || TryCastSpellWarrior(Warrior335a.Revenge, Warrior335a.DefensiveStance, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }
                }
            }
        }
    }
}