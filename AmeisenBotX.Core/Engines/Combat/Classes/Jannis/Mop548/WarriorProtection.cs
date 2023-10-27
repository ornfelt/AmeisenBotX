using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow548.Constants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Jannis.Mop548
{
    /// <summary>
    /// The WarriorProtection class represents a combat class for warriors specializing in protection role in battles.
    /// </summary>
    public class WarriorProtection : BasicCombatClass
    {
        /// <summary>
        /// Initializes a new instance of the WarriorProtection class.
        /// </summary>
        /// <param name="bot">The bot instance to be passed as a parameter.</param>
        public WarriorProtection(AmeisenBotInterfaces bot) : base(bot)
        {
            Configurables.TryAdd("FartOnCharge", false);

            InterruptManager.InterruptSpells = new()
            {
                { 0, (x) => TryCastSpell(Warrior548.Pummel, x.Guid, true) },
                { 1, (x) => TryCastSpell(Warrior548.DragonRoar, x.Guid, true) },
            };
        }

        /// <summary>
        /// Returns the description of the Beta CombatClass for the Protection Warrior spec.
        /// </summary>
        /// <value>
        /// "Beta CombatClass for the Protection Warrior spec. For Dungeons and Questing"
        /// </value>
        public override string Description => "Beta CombatClass for the Protection Warrior spec. For Dungeons and Questing";

        /// <summary>
        /// Gets or sets the display name for the warrior's protection class.
        /// </summary>
        public override string DisplayName2 => "Warrior Protection";

        /// <summary>
        /// Indicates that the method overrides the base class and handles movement.
        /// </summary>
        public override bool HandlesMovement => true;

        /// <summary>
        /// Gets a value indicating whether this is a melee attack.
        /// </summary>
        public override bool IsMelee => true;

        /// <summary>
        /// Gets or sets the item comparator used for comparing items.
        /// </summary>
        /// <value>A <see cref="IItemComparator"/> object.</value>
        public override IItemComparator ItemComparator { get; set; } = new BasicComparator
                (
                    new() { WowArmorType.Cloth, WowArmorType.Leather },
                    new()
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
                    },
                    new Dictionary<string, double>()
                    {
                { "ITEM_MOD_STAMINA_SHORT", 1.0 },
                { "ITEM_MOD_PARRY_RATING_SHORT", 1.0 },
                { "ITEM_MOD_DODGE_RATING_SHORT", 0.6 },
                { "ITEM_MOD_MASTERY_RATING_SHORT", 0.5},
                { "ITEM_MOD_CRIT_RATING_SHORT", 0.3 },
                { "ITEM_MOD_STRENGHT_SHORT", 0.2 },
                { "ITEM_MOD_HASTE_RATING_SHORT", 0.1 },
                    }
                );

        /// <summary>
        /// Gets or sets the role of this character in the game "World of Warcraft" as a Tank.
        /// </summary>
        public override WowRole Role => WowRole.Tank;

        /// <summary>
        /// Gets or sets the talent trees.
        /// </summary>
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
            },
            Tree2 = new()
            {
            },
            Tree3 = new()
            {
            },
        };

        /// The property indicates whether the object should use auto attacks.
        public override bool UseAutoAttacks => true;

        /// <summary>
        /// Gets the version of the object.
        /// </summary>
        public override string Version => "1.0";

        /// <summary>
        /// Gets or sets a value indicating whether the player can walk behind the enemy.
        /// </summary>
        /// <returns>Always returns false.</returns>
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the WoW class associated with this instance, which is Warrior.
        /// </summary>
        public override WowClass WowClass => WowClass.Warrior;

        /// <summary>
        /// Gets or sets the World of Warcraft version, which is set to Mist of Pandaria (548).
        /// </summary>
        public override WowVersion WowVersion => WowVersion.MoP548;

        /// <summary>
        /// Gets or sets the last time the fart occurred.
        /// </summary>
        private DateTime LastFarted { get; set; }

        /// <summary>
        /// Executes the actions for the bot.
        /// </summary>
        public override void Execute()
        {
            base.Execute();

            if (TryFindTarget(TargetProviderTank, out _))
            {
                float distanceToTarget = Bot.Target.Position.GetDistance(Bot.Player.Position);

                if (!Bot.Tactic.PreventMovement)
                {
                    IWowUnit targetOfTarget = Bot.GetWowObjectByGuid<IWowUnit>(Bot.Target.TargetGuid);

                    if (targetOfTarget != null && targetOfTarget.Guid == Bot.Player.Guid)
                    {
                        // if we have aggro, pull the unit to the best tanking spot
                        if (Bot.Objects.Partymembers.Any())
                        {
                            Vector3 direction = Bot.Player.Position - Bot.Objects.CenterPartyPosition;
                            direction.Normalize();

                            Vector3 bestTankingSpot = Bot.Objects.CenterPartyPosition + (direction * 12.0f);

                            if (Bot.Player.DistanceTo(bestTankingSpot) > 2.75f)
                            {
                                Bot.Movement.SetMovementAction(MovementAction.Move, Bot.Target.Position);
                            }
                        }
                        else
                        {
                            if (Bot.Player.DistanceTo(Bot.Target.Position) > Bot.Player.MeleeRangeTo(Bot.Target))
                            {
                                Bot.Movement.SetMovementAction(MovementAction.Move, Bot.Target.Position);
                            }
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

                // go all ham, maybe filter for bossfights?
                TryCastSpell(Warrior548.BattleShout, 0);
                TryCastSpell(Warrior548.Recklessness, 0);

                // is anyone casting stuff on me, try to reflect
                IEnumerable<IWowUnit> castingUnits = Bot.Objects.All.OfType<IWowUnit>().Where(e => e.IsCasting && e.TargetGuid == Bot.Player.Guid && e.DistanceTo(Bot.Player) < 38.0f);

                if (castingUnits.Any())
                {
                    TryCastSpell(Warrior548.SpellReflection, 0);
                }

                if (distanceToTarget > 8.0f)
                {
                    if (TryCastSpell(Warrior548.Charge, Bot.Wow.TargetGuid))
                    {
                        if (Configurables["FartOnCharge"] && DateTime.UtcNow - LastFarted > TimeSpan.FromSeconds(8))
                        {
                            LastFarted = DateTime.UtcNow;
                            Bot.Wow.SendChatMessage("/rude");
                        }

                        return;
                    }

                    if (TryCastSpell(Warrior548.HeroicThrow, Bot.Wow.TargetGuid))
                    {
                        return;
                    }
                }
                else
                {
                    if (Bot.Player.HealthPercentage < 50.0
                        && TryCastSpell(Warrior548.ShieldBlock, 0))
                    {
                        return;
                    }

                    if (Bot.Player.HealthPercentage < 35.0)
                    {
                        TryCastSpell(Warrior548.ShieldWall, 0);
                    }

                    if (Bot.Player.HealthPercentage < 25.0)
                    {
                        TryCastSpell(Warrior548.LastStand, 0);
                    }

                    if (Bot.Target.HealthPercentage < 20.0
                        && TryCastSpell(Warrior548.Execute, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }

                    if (TryCastSpell(Warrior548.ShieldSlam, Bot.Wow.TargetGuid))
                    {
                        return;
                    }

                    if (TryCastSpell(Warrior548.Revenge, Bot.Wow.TargetGuid))
                    {
                        return;
                    }

                    bool hasVictoriousBuff = Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Warrior548.Victorious);

                    if (hasVictoriousBuff
                        && Bot.Player.HealthPercentage < 80.0
                        && TryCastSpell(Warrior548.VictoryRush, Bot.Wow.TargetGuid))
                    {
                        return;
                    }

                    if (Bot.Target.TargetGuid != Bot.Wow.PlayerGuid
                        && TryCastSpell(Warrior548.Taunt, Bot.Wow.TargetGuid))
                    {
                        return;
                    }

                    if (!Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Warrior548.DemoralizingShout)
                        && TryCastSpell(Warrior548.DemoralizingShout, Bot.Wow.TargetGuid))
                    {
                        return;
                    }

                    if (!Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Warrior548.PiercingHowl)
                        && TryCastSpell(Warrior548.PiercingHowl, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }

                    // 60 rage is used because we want to still be able to instantly cast Execute
                    // below 30% hp
                    int rageToSave = Bot.Target.HealthPercentage < 30.0 ? 30 : 0;
                    int nearEnemies = Bot.GetNearEnemies<IWowUnit>(Bot.Player.Position, 10.0f).Count();

                    if ((nearEnemies > 2 || Bot.Player.Rage > (rageToSave + 30))
                        && TryCastSpell(Warrior548.ThunderClap, Bot.Wow.TargetGuid))
                    {
                        return;
                    }

                    if (!Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Warrior548.WeakenedAmor && e.StackCount < 3)
                        && (TryCastSpell(Warrior548.SunderAmor, Bot.Wow.TargetGuid, true, Bot.Player.Rage - 15))
                            || TryCastSpell(Warrior548.Devastate, Bot.Wow.TargetGuid, true, Bot.Player.Rage - 15))
                    {
                        return;
                    }

                    if (nearEnemies > 1
                        && TryCastSpell(Warrior548.DragonRoar, Bot.Wow.TargetGuid))
                    {
                        return;
                    }

                    if ((nearEnemies > 1 || Bot.Player.Rage > (rageToSave + 30))
                        && TryCastSpell(Warrior548.Cleave, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }

                    if (TryCastSpell(Warrior548.HeroicStrike, Bot.Wow.TargetGuid, true, Bot.Player.Rage - 30))
                    {
                        return;
                    }

                    // when we got nothing to do, use Victory Rush
                    if (hasVictoriousBuff
                        && TryCastSpell(Warrior548.VictoryRush, Bot.Wow.TargetGuid))
                    {
                        return;
                    }
                }
            }
        }
    }
}