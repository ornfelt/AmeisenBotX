using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Spells.Objects;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Kamel
{
    /// <summary>
    /// Represents a Warrior specialization focused on fury and close combat.
    /// </summary>
    internal class WarriorFury : BasicKamelClass
    {
        /// <summary>
        /// Represents the spell for a battle shout.
        /// </summary>
        private const string battleShoutSpell = "Battle Shout";
        /// <summary>
        /// The constant string representing the spell "Battle Stance".
        /// </summary>
        private const string battleStanceSpell = "Battle Stance";
        /// <summary>
        /// The name of the berserker rage spell.
        /// </summary>
        private const string berserkerRageSpell = "Berserker Rage";
        /// <summary>
        /// Represents the spell name for Berserker Stance.
        /// </summary>
        private const string berserkerStanceSpell = "Berserker Stance";
        /// <summary>
        /// The name of the bloodrage spell.
        /// </summary>
        private const string bloodrageSpell = "Bloodrage";

        /// <summary>
        /// The name of the bloodthirst spell.
        /// </summary>
        //Spells
        private const string bloodthirstSpell = "Bloodthirst";

        /// <summary>
        /// The constant string that represents the spell "Charge".
        /// </summary>
        private const string chargeSpell = "Charge";
        /// <summary>
        /// Represents the name of the "Cleave" spell.
        /// </summary>
        private const string cleaveSpell = "Cleave";
        /// <summary>
        /// The name of the Commanding Shout spell.
        /// </summary>
        private const string commandingShoutSpell = "Commanding Shout";
        /// <summary>
        /// The name of the death wish spell.
        /// </summary>
        private const string deathWishSpell = "Death Wish";

        /// <summary>
        /// The spell used for activating defensive stance.
        /// </summary>
        //Stances
        private const string defensiveStanceSpell = "Defensive Stance";

        /// <summary>
        /// Represents the name of the "Disarm" spell.
        /// </summary>
        private const string disarmSpell = "Disarm";
        /// <summary>
        /// The spell name for Enraged Regeneration.
        /// </summary>
        private const string enragedregenerationSpell = "Enraged Regeneration";
        /// <summary>
        /// The constant string representing the action to execute a spell.
        /// </summary>
        private const string executeSpell = "Execute";
        /// <summary>
        /// The hamstringSpell constant represents the spell "Hamstring".
        /// </summary>
        private const string hamstringSpell = "Hamstring";
        /// <summary>
        /// The name of the heroic fury spell.
        /// </summary>
        private const string heroicFurySpell = "Heroic Fury";
        /// <summary>
        /// The name of the heroic strike spell.
        /// </summary>
        private const string heroicStrikeSpell = "Heroic Strike";
        /// <summary>
        /// The name of the spell "Heroic Throw".
        /// </summary>
        private const string heroicThrowSpell = "Heroic Throw";
        /// <summary>
        /// Represents the constant string value "Intercept".
        /// </summary>
        private const string interceptSpell = "Intercept";
        /// <summary>
        /// The name of the intimidating shout spell.
        /// </summary>
        private const string intimidatingShoutSpell = "Intimidating Shout";
        /// <summary>
        /// The constant value representing the "Pummel" spell.
        /// </summary>
        private const string pummelSpell = "Pummel";
        /// <summary>
        /// Represents the name of the "Recklessness" spell.
        /// </summary>
        private const string recklessnessSpell = "Recklessness";
        /// <summary>
        /// The name of the "Rend" spell.
        /// </summary>
        private const string rendSpell = "Rend";

        /// <summary>
        /// The name of the retaliation spell.
        /// </summary>
        //Buffs||Defensive||Enrage
        private const string retaliationSpell = "Retaliation";

        /// <summary>
        /// Represents the name of the spell "Shattering Throw".
        /// </summary>
        private const string ShatteringThrowSpell = "Shattering Throw";
        /// <summary>
        /// Represents the constant string value "Shoot" used for shooting spells.
        /// </summary>
        private const string ShootSpell = "Shoot";
        /// <summary>
        /// The name of the Slam spell.
        /// </summary>
        private const string slamSpell = "Slam";
        /// <summary>
        /// The name of the spell 'Victory Rush'.
        /// </summary>
        private const string victoryRushSpell = "Victory Rush";
        /// <summary>
        /// Represents the name of the whirlwind spell.
        /// </summary>
        private const string whirlwindSpell = "Whirlwind";

        /// <summary>
        /// Initializes a new instance of the WarriorFury class.
        /// Sets the provided bot and initializes spell cooldowns for various spells, stances, buffs,
        /// and defensive abilities. Also sets up time events for Heroic Strike, Victory Rush, Rend, and Execute.
        /// </summary>
        public WarriorFury(AmeisenBotInterfaces bot) : base()
        {
            Bot = bot;
            spellCoolDown.Add(ShootSpell, DateTime.Now);
            //Stances
            spellCoolDown.Add(defensiveStanceSpell, DateTime.Now);
            spellCoolDown.Add(battleStanceSpell, DateTime.Now);
            spellCoolDown.Add(berserkerStanceSpell, DateTime.Now);
            //Spells
            spellCoolDown.Add(heroicStrikeSpell, DateTime.Now);
            spellCoolDown.Add(interceptSpell, DateTime.Now);
            spellCoolDown.Add(heroicThrowSpell, DateTime.Now);
            spellCoolDown.Add(ShatteringThrowSpell, DateTime.Now);
            spellCoolDown.Add(executeSpell, DateTime.Now);
            spellCoolDown.Add(pummelSpell, DateTime.Now);
            spellCoolDown.Add(bloodthirstSpell, DateTime.Now);
            spellCoolDown.Add(slamSpell, DateTime.Now);
            spellCoolDown.Add(whirlwindSpell, DateTime.Now);
            spellCoolDown.Add(disarmSpell, DateTime.Now);
            spellCoolDown.Add(rendSpell, DateTime.Now);
            spellCoolDown.Add(hamstringSpell, DateTime.Now);
            spellCoolDown.Add(victoryRushSpell, DateTime.Now);
            spellCoolDown.Add(chargeSpell, DateTime.Now);
            spellCoolDown.Add(cleaveSpell, DateTime.Now);
            //Buffs||Defensive||Enrage
            spellCoolDown.Add(intimidatingShoutSpell, DateTime.Now);
            spellCoolDown.Add(retaliationSpell, DateTime.Now);
            spellCoolDown.Add(enragedregenerationSpell, DateTime.Now);
            spellCoolDown.Add(bloodrageSpell, DateTime.Now);
            spellCoolDown.Add(commandingShoutSpell, DateTime.Now);
            spellCoolDown.Add(recklessnessSpell, DateTime.Now);
            spellCoolDown.Add(heroicFurySpell, DateTime.Now);
            spellCoolDown.Add(berserkerRageSpell, DateTime.Now);
            spellCoolDown.Add(deathWishSpell, DateTime.Now);
            spellCoolDown.Add(battleShoutSpell, DateTime.Now);

            //Time event
            HeroicStrikeEvent = new(TimeSpan.FromSeconds(2));
            VictoryRushEvent = new(TimeSpan.FromSeconds(5));
            RendEvent = new(TimeSpan.FromSeconds(6));
            ExecuteEvent = new(TimeSpan.FromSeconds(1));
        }

        ///<summary>Returns the name of the author of the code. </summary>
        ///<returns>A string containing the name of the author.</returns>
        public override string Author => "Lukas";

        /// <summary>
        /// Gets or sets the dictionary of string keys and dynamic values.
        /// </summary>
        public override Dictionary<string, dynamic> C { get; set; } = new Dictionary<string, dynamic>();

        /// <summary>
        /// Gets the description of the Warrior Fury.
        /// </summary>
        /// <returns>The description of the Warrior Fury.</returns>
        public override string Description => "Warrior Fury";

        /// <summary>
        /// Gets the display name for the Warrior Fury Final.
        /// </summary>
        public override string DisplayName => "Warrior Fury Final";

        /// <summary>
        /// Gets or sets the TimegatedEvent that will be executed by the ExecuteEvent property.
        /// </summary>
        public TimegatedEvent ExecuteEvent { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance handles movement.
        /// </summary>
        /// <value>
        ///   <c>false</c> if this instance does not handle movement; otherwise, <c>true</c>.
        /// </value>
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets or sets the TimegatedEvent for the HeroicStrikeEvent.
        /// </summary>
        public TimegatedEvent HeroicStrikeEvent { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this entity is melee.
        /// </summary>
        public override bool IsMelee => true;

        /// <summary>
        /// Gets or sets the item comparator for this object.
        /// The item comparator is set to a BasicStrengthComparator with specific armor and weapon types.
        /// - Armor types: Shield
        /// - Weapon types: Sword, Mace, Axe, Staff, Dagger
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicStrengthComparator(new() { WowArmorType.Shield }, new() { WowWeaponType.Sword, WowWeaponType.Mace, WowWeaponType.Axe, WowWeaponType.Staff, WowWeaponType.Dagger });

        /// <summary>
        /// Gets or sets the time-gated event for rendering.
        /// </summary>
        //Time event
        public TimegatedEvent RendEvent { get; private set; }

        /// <summary>
        /// Gets the role of the character as Dps.
        /// </summary>
        public override WowRole Role => WowRole.Dps;

        /// <summary>
        /// Represents the talent tree for a specific character. 
        /// The TalentTree property is overridden to initialize a new TalentTree object with predefined values.
        /// The TalentTree object contains three trees: Tree1, Tree2, and Tree3.
        /// Each tree is a dictionary that maps an integer (key) to a Talent object (value).
        /// Tree1 contains the following key-value pairs:
        /// Key 1: Talent with ID 1, requiredLevel 1, pointsInvested 3.
        /// Key 3: Talent with ID 1, requiredLevel 3, pointsInvested 2.
        /// Key 5: Talent with ID 1, requiredLevel 5, pointsInvested 2.
        /// Key 6: Talent with ID 1, requiredLevel 6, pointsInvested 3.
        /// Key 9: Talent with ID 1, requiredLevel 9, pointsInvested 2.
        /// Key 10: Talent with ID 1, requiredLevel 10, pointsInvested 3.
        /// Key 11: Talent with ID 1, requiredLevel 11, pointsInvested 3.
        /// 
        /// Tree2 contains the following key-value pairs:
        /// Key 1: Talent with ID 2, requiredLevel 1, pointsInvested 3.
        /// Key 3: Talent with ID 2, requiredLevel 3, pointsInvested 5.
        /// Key 5: Talent with ID 2, requiredLevel 5, pointsInvested 5.
        /// Key 6: Talent with ID 2, requiredLevel 6, pointsInvested 3.
        /// Key 10: Talent with ID 2, requiredLevel 10, pointsInvested 5.
        /// Key 13: Talent with ID 2, requiredLevel 13, pointsInvested 3.
        /// Key 14: Talent with ID 2, requiredLevel 14, pointsInvested 1.
        /// Key 16: Talent with ID 2, requiredLevel 16, pointsInvested 1.
        /// Key 17: Talent with ID 2, requiredLevel 17, pointsInvested 5.
        /// Key 18: Talent with ID 2, requiredLevel 18, pointsInvested 3.
        /// Key 19: Talent with ID 2, requiredLevel 19, pointsInvested 1.
        /// Key 20: Talent with ID 2, requiredLevel 20, pointsInvested 2.
        /// Key 22: Talent with ID 2, requiredLevel 22, pointsInvested 5.
        /// Key 23: Talent with ID 2, requiredLevel 23, pointsInvested 1.
        /// Key 24: Talent with ID 2, requiredLevel 24, pointsInvested 1.
        /// Key 25: Talent with ID 2, requiredLevel 25, pointsInvested 3.
        /// Key 26: Talent with ID 2, requiredLevel 26, pointsInvested 5.
        /// Key 27: Talent with ID 2, requiredLevel 27, pointsInvested 1.
        ///  
        /// Tree3 is an empty TalentTree object.
        /// </summary>
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 1, new(1, 1, 3) },
                { 3, new(1, 3, 2) },
                { 5, new(1, 5, 2) },
                { 6, new(1, 6, 3) },
                { 9, new(1, 9, 2) },
                { 10, new(1, 10, 3) },
                { 11, new(1, 11, 3) },
            },
            Tree2 = new()
            {
                { 1, new(2, 1, 3) },
                { 3, new(2, 3, 5) },
                { 5, new(2, 5, 5) },
                { 6, new(2, 6, 3) },
                { 10, new(2, 10, 5) },
                { 13, new(2, 13, 3) },
                { 14, new(2, 14, 1) },
                { 16, new(2, 16, 1) },
                { 17, new(2, 17, 5) },
                { 18, new(2, 18, 3) },
                { 19, new(2, 19, 1) },
                { 20, new(2, 20, 2) },
                { 22, new(2, 22, 5) },
                { 23, new(2, 23, 1) },
                { 24, new(2, 24, 1) },
                { 25, new(2, 25, 3) },
                { 26, new(2, 26, 5) },
                { 27, new(2, 27, 1) },
            },
            Tree3 = new(),
        };

        /// <summary>
        /// Gets or sets a value indicating whether this character uses auto attacks.
        /// </summary>
        public override bool UseAutoAttacks => true;

        /// <summary>
        /// Gets the version of the code.
        /// </summary>
        public override string Version => "3.0";

        /// <summary>
        /// Gets or sets the TimegatedEvent for Victory Rush.
        /// </summary>
        public TimegatedEvent VictoryRushEvent { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player can walk behind enemies.
        /// </summary>
        /// <returns>False, as the player cannot walk behind enemies.</returns>
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the Wow class for this instance, which is set to Warrior.
        /// </summary>
        public override WowClass WowClass => WowClass.Warrior;

        /// <summary>
        /// Executes the CC attack by calling the StartAttack method.
        /// </summary>
        public override void ExecuteCC()
        {
            StartAttack();
        }

        /// <summary>
        /// Executes the actions when the character is out of combat.
        /// </summary>
        public override void OutOfCombatExecute()
        {
            Targetselection();
            StartAttack();
        }

        /// <summary>
        ///  CustomCastSpell function is used to cast a specific spell by name. By default, the spell is casted in "Berserker Stance" unless a different stance is specified.
        /// </summary>
        /// <param name="spellName">The name of the spell to be casted.</param>
        /// <param name="stance">The stance in which the spell should be casted (defaults to "Berserker Stance").</param>
        /// <returns>True if the spell was successfully casted, False otherwise.</returns>
        private bool CustomCastSpell(string spellName, string stance = "Berserker Stance")
        {
            if (!Bot.Character.SpellBook.IsSpellKnown(stance))
            {
                stance = "Battle Stance";
            }

            if (Bot.Character.SpellBook.IsSpellKnown(spellName))
            {
                if (Bot.Target != null)
                {
                    double distance = Bot.Player.Position.GetDistance(Bot.Target.Position);
                    Spell spell = Bot.Character.SpellBook.GetSpellByName(spellName);

                    if ((Bot.Player.Rage >= spell.Costs && IsSpellReady(spellName)))
                    {
                        if ((spell.MinRange == 0 && spell.MaxRange == 0) || (spell.MinRange <= distance && spell.MaxRange >= distance))
                        {
                            if (!Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == stance))
                            {
                                Bot.Wow.CastSpell(stance);
                                return true;
                            }
                            else
                            {
                                Bot.Wow.CastSpell(spellName);
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Starts the attack sequence for the bot.
        /// </summary>
        private void StartAttack()
        {
            if (Bot.Wow.TargetGuid != 0)
            {
                ChangeTargetToAttack();

                if (Bot.Db.GetReaction(Bot.Player, Bot.Target) == WowUnitReaction.Friendly)
                {
                    Bot.Wow.ClearTarget();
                    return;
                }

                if (Bot.Player.IsInMeleeRange(Bot.Target))
                {
                    if (!Bot.Player.IsAutoAttacking && AutoAttackEvent.Run())
                    {
                        Bot.Wow.StartAutoAttack();
                    }

                    if (CustomCastSpell(bloodrageSpell))
                    {
                        return;
                    }

                    if (CustomCastSpell(berserkerRageSpell))
                    {
                        return;
                    }

                    if (CustomCastSpell(deathWishSpell))
                    {
                        return;
                    }

                    if (Bot.Target.IsCasting && CustomCastSpell(pummelSpell))
                    {
                        return;
                    }

                    if (Bot.Target.GetType() == typeof(IWowPlayer) && !Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Hamstring") && CustomCastSpell(hamstringSpell))
                    {
                        return;
                    }

                    if (Bot.Target.HealthPercentage <= 20 && CustomCastSpell(executeSpell))
                    {
                        return;
                    }

                    if (Bot.Player.HealthPercentage <= 50 && (Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Bloodrage") || Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Recklessness") || Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Berserker Rage")))
                    {
                        if (CustomCastSpell(enragedregenerationSpell))
                        {
                            return;
                        }
                    }

                    if ((Bot.Player.HealthPercentage <= 30) || (Bot.Target.GetType() == typeof(IWowPlayer)) && CustomCastSpell(intimidatingShoutSpell))
                    {
                        return;
                    }

                    if (Bot.Player.HealthPercentage <= 60 && CustomCastSpell(retaliationSpell, battleStanceSpell))
                    {
                        return;
                    }

                    if (Bot.Target.GetType() == typeof(IWowPlayer) && CustomCastSpell(disarmSpell, defensiveStanceSpell))
                    {
                        return;
                    }

                    if (Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Slam!") && CustomCastSpell(slamSpell) && CustomCastSpell(recklessnessSpell))
                    {
                        return;
                    }

                    if (CustomCastSpell(whirlwindSpell))
                    {
                        return;
                    }

                    if (CustomCastSpell(bloodthirstSpell))
                    {
                        return;
                    }

                    if (VictoryRushEvent.Run() && CustomCastSpell(victoryRushSpell))
                    {
                        return;
                    }

                    if (RendEvent.Run() && !Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Rend") && CustomCastSpell(rendSpell))
                    {
                        return;
                    }

                    if (HeroicStrikeEvent.Run() && Bot.Player.Rage >= 60 && CustomCastSpell(heroicStrikeSpell))
                    {
                        return;
                    }

                    IEnumerable<IWowUnit> unitsNearPlayer = Bot.GetNearEnemies<IWowUnit>(Bot.Player.Position, 5);

                    if (unitsNearPlayer != null)
                    {
                        if (unitsNearPlayer.Count() >= 3 && Bot.Player.Rage >= 50 && CustomCastSpell(cleaveSpell))
                        {
                            return;
                        }
                    }

                    if (!Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Battle Shout") && CustomCastSpell(battleShoutSpell))
                    {
                        return;
                    }
                }
                else//Range
                {
                    if ((Bot.Player.IsDazed
                        || Bot.Player.IsFleeing
                        || Bot.Player.IsInfluenced
                        || Bot.Player.IsPossessed)
                        || Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Frost Nova")
                        || Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Frost Trap Aura")
                        || Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Hamstring")
                        || Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Concussive Shot")
                        || Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Frostbolt")
                        || Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Frost Shock")
                        || Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Frostfire Bolt")
                        || Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Slow")
                        || Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Entangling Roots"))
                    {
                        if (CustomCastSpell(heroicFurySpell))
                        {
                            return;
                        }
                    }
                    if (Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Entangling Roots")
                        || Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Frost Nova"))
                    {
                        if (Bot.Movement.Status != Movement.Enums.MovementAction.None)
                        {
                            Bot.Wow.StopClickToMove();
                            Bot.Movement.Reset();
                        }

                        if (CustomCastSpell(ShootSpell))
                        {
                            return;
                        }

                        if (CustomCastSpell(ShatteringThrowSpell, battleStanceSpell))
                        {
                            return;
                        }
                    }
                    if (CustomCastSpell(interceptSpell))
                    {
                        return;
                    }
                    if (CustomCastSpell(chargeSpell, battleStanceSpell))
                    {
                        return;
                    }
                    if (CustomCastSpell(heroicThrowSpell))
                    {
                        return;
                    }
                }
            }
            else
            {
                Targetselection();
            }
        }
    }
}