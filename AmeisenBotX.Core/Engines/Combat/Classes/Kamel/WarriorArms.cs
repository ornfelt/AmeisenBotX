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
    internal class WarriorArms : BasicKamelClass
    {
        /// <summary>
        /// Represents the name of the "Battle Shout" spell.
        /// </summary>
        private const string battleShoutSpell = "Battle Shout";

        /// <summary>
        /// The spell name for battle stance.
        /// </summary>
        private const string battleStanceSpell = "Battle Stance";

        /// <summary>
        /// The name of the Berserker Rage spell.
        /// </summary>
        private const string berserkerRageSpell = "Berserker Rage";

        ///<summary>
        /// The name of the berserker stance spell.
        ///</summary>
        private const string berserkerStanceSpell = "Berserker Stance";

        /// <summary>
        /// Represents the name of the Bladestorm spell.
        /// </summary>
        private const string BladestormSpell = "Bladestorm";

        /// <summary>
        /// Represents the spell name "Bloodrage".
        /// </summary>
        private const string bloodrageSpell = "Bloodrage";

        /// <summary>
        /// The constant string for the spell "Charge".
        /// </summary>
        //Spells
        private const string chargeSpell = "Charge";

        /// <summary>
        /// Represents the name of the cleave spell.
        /// </summary>
        private const string cleaveSpell = "Cleave";

        /// <summary>
        /// The name of the commanding shout spell.
        /// </summary>
        private const string commandingShoutSpell = "Commanding Shout";

        /// <summary>
        /// Represents the name of the "Death Wish" spell.
        /// </summary>
        private const string deathWishSpell = "Death Wish";

        /// <summary>
        /// The spell used for activating the defensive stance.
        /// </summary>
        //Stances
        private const string defensiveStanceSpell = "Defensive Stance";

        /// <summary>
        /// Constant string representing the "Disarm" spell.
        /// </summary>
        private const string disarmSpell = "Disarm";
        /// <summary>
        /// The name of the Enraged Regeneration spell.
        /// </summary>
        private const string enragedregenerationSpell = "Enraged Regeneration";
        /// <summary>
        /// Represents the name of the spell to be executed.
        /// </summary>
        private const string executeSpell = "Execute";
        /// <summary>
        /// Represents the name of the hamstring spell.
        /// </summary>
        private const string hamstringSpell = "Hamstring";
        /// <summary>
        /// Represents the name of the heroic fury spell.
        /// </summary>
        private const string heroicFurySpell = "Heroic Fury";
        /// <summary>
        /// The name of the heroic strike spell.
        /// </summary>
        private const string heroicStrikeSpell = "Heroic Strike";
        /// <summary>
        /// The name of the heroic throw spell.
        /// </summary>
        private const string heroicThrowSpell = "Heroic Throw";
        /// <summary>
        /// Represents the name of the intercept spell.
        /// </summary>
        private const string interceptSpell = "Intercept";
        /// <summary>
        /// The name of the intimidating shout spell.
        /// </summary>
        private const string intimidatingShoutSpell = "Intimidating Shout";
        /// <summary>
        /// The name of the spell "Mortal Strike".
        /// </summary>
        private const string MortalStrikeSpell = "Mortal Strike";
        /// <summary>
        /// The name of the overpower spell.
        /// </summary>
        private const string OverpowerSpell = "Overpower";
        /// <summary>
        /// The name of the pummel spell.
        /// </summary>
        private const string pummelSpell = "Pummel";
        /// <summary>
        /// The constant string representing the spell "Recklessness".
        /// </summary>
        private const string recklessnessSpell = "Recklessness";
        /// <summary>
        /// The constant string representation of the spell "Rend".
        /// </summary>
        private const string rendSpell = "Rend";

        /// <summary>
        /// The name of the retaliation spell for buffs, defensive, and enrage.
        /// </summary>
        //Buffs||Defensive||Enrage
        private const string retaliationSpell = "Retaliation";

        /// <summary>
        /// Represents the name of the Slam spell.
        /// </summary>
        private const string slamSpell = "Slam";
        /// <summary>
        /// Represents the victory rush spell.
        /// </summary>
        private const string victoryRushSpell = "Victory Rush";

        /// <summary>
        /// Initializes a new instance of the WarriorArms class with the specified bot.
        /// </summary>
        /// <param name="bot">The bot object that the WarriorArms instance will interact with.</param>
        public WarriorArms(AmeisenBotInterfaces bot) : base()
        {
            Bot = bot;
            //Stances
            spellCoolDown.Add(defensiveStanceSpell, DateTime.Now);
            spellCoolDown.Add(battleStanceSpell, DateTime.Now);
            spellCoolDown.Add(berserkerStanceSpell, DateTime.Now);
            //Spells
            spellCoolDown.Add(heroicStrikeSpell, DateTime.Now);
            spellCoolDown.Add(BladestormSpell, DateTime.Now);
            spellCoolDown.Add(OverpowerSpell, DateTime.Now);
            spellCoolDown.Add(MortalStrikeSpell, DateTime.Now);
            spellCoolDown.Add(interceptSpell, DateTime.Now);
            spellCoolDown.Add(heroicThrowSpell, DateTime.Now);
            spellCoolDown.Add(executeSpell, DateTime.Now);
            spellCoolDown.Add(pummelSpell, DateTime.Now);
            spellCoolDown.Add(slamSpell, DateTime.Now);
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
            RendEvent = new(TimeSpan.FromSeconds(3));
            ExecuteEvent = new(TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Gets or sets the author of the code.
        /// </summary>
        /// <value>
        /// The author of the code.
        /// </value>
        public override string Author => "Lukas";

        /// <summary>
        /// Gets or sets the dictionary of strings and dynamic values.
        /// </summary>
        public override Dictionary<string, dynamic> C { get; set; } = new Dictionary<string, dynamic>();

        /// <summary>
        /// Gets the description of the warrior's arms.
        /// </summary>
        /// <returns>The description of the arms as a string.</returns>
        public override string Description => "Warrior Arms";

        /// <summary>
        /// Gets or sets the display name for the Warrior Arms Beta.
        /// </summary>
        public override string DisplayName => "Warrior Arms Beta";

        /// <summary>
        /// Gets or sets the event representing the execution of a time-gated event.
        /// </summary>
        public TimegatedEvent ExecuteEvent { get; private set; }

        /// <summary>
        /// This property indicates that the class does not handle movement.
        /// </summary>
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets or sets the timegated event for performing a heroic strike.
        /// </summary>
        public TimegatedEvent HeroicStrikeEvent { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the character is a melee character.
        /// </summary>
        public override bool IsMelee => true;

        /// <summary>
        /// Gets or sets the item comparator used for comparing items.
        /// The default value is a BasicStrengthComparator initialized with WowArmorType.Shield and WowWeaponType.Sword, WowWeaponType.Mace, WowWeaponType.Axe, WowWeaponType.Staff, WowWeaponType.Dagger.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicStrengthComparator(new() { WowArmorType.Shield }, new() { WowWeaponType.Sword, WowWeaponType.Mace, WowWeaponType.Axe, WowWeaponType.Staff, WowWeaponType.Dagger });

        /// <summary>
        /// Time event property that represents a time-gated event.
        /// </summary>
        //Time event
        public TimegatedEvent RendEvent { get; private set; }

        /// <summary>
        /// Gets the role of the character as a DPS (Damage Per Second) role.
        /// </summary>
        public override WowRole Role => WowRole.Dps;

        /// <summary>
        /// Gets the talents for the TalentTree.
        /// </summary>
        /// <value>
        /// The TalentTree object that contains the talent data.
        /// </value>
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 1, new(1, 1, 3) },
                { 3, new(1, 3, 2) },
                { 4, new(1, 4, 2) },
                { 6, new(1, 6, 3) },
                { 7, new(1, 7, 2) },
                { 8, new(1, 8, 1) },
                { 9, new(1, 9, 2) },
                { 10, new(1, 10, 3) },
                { 11, new(1, 11, 3) },
                { 12, new(1, 12, 3) },
                { 13, new(1, 13, 5) },
                { 14, new(1, 14, 1) },
                { 17, new(1, 17, 2) },
                { 19, new(1, 19, 2) },
                { 21, new(1, 21, 1) },
                { 22, new(1, 22, 2) },
                { 24, new(1, 24, 1) },
                { 25, new(1, 25, 3) },
                { 26, new(1, 26, 2) },
                { 27, new(1, 27, 3) },
                { 28, new(1, 28, 1) },
                { 29, new(1, 29, 2) },
                { 30, new(1, 30, 5) },
                { 31, new(1, 31, 1) },
            },
            Tree2 = new()
            {
                { 1, new(2, 1, 3) },
                { 2, new(2, 2, 2) },
                { 3, new(2, 3, 5) },
                { 5, new(2, 5, 5) },
                { 7, new(2, 7, 1) },
            },
            Tree3 = new(),
        };

        /// <summary>
        /// Gets or sets a value indicating whether this character is able to use auto attacks.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the character is able to use auto attacks; otherwise, <c>false</c>.
        /// </value>
        public override bool UseAutoAttacks => true;

        /// <summary>
        /// Gets the version string of the code.
        /// </summary>
        public override string Version => "1.0";

        /// <summary>
        /// Gets or sets the TimegatedEvent for the VictoryRushEvent.
        /// </summary>
        public TimegatedEvent VictoryRushEvent { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the character can walk behind an enemy.
        /// </summary>
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the WowClass property, specifying that the class is a Warrior.
        /// </summary>
        public override WowClass WowClass => WowClass.Warrior;

        /// <summary>
        /// Executes the ExecuteCC method.
        /// </summary>
        public override void ExecuteCC()
        {
            StartAttack();
        }

        /// <summary>
        /// Executes the out-of-combat actions.
        /// </summary>
        public override void OutOfCombatExecute()
        {
            Targetselection();
            StartAttack();
        }

        /// <summary>
        /// Custom method to cast a spell with the given spell name and stance. By default, the stance is set to "Battle Stance".
        /// </summary>
        /// <param name="spellName">The name of the spell to cast.</param>
        /// <param name="stance">The stance to be in while casting the spell (default value: "Battle Stance").</param>
        /// <returns>Returns true if the spell is successfully cast, otherwise returns false.</returns>
        private bool CustomCastSpell(string spellName, string stance = "Battle Stance")
        {
            if (Bot.Character.SpellBook.IsSpellKnown(spellName))
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

            return false;
        }

        /// <summary>
        /// Method to initiate attack on the target.
        /// </summary>
        private void StartAttack()
        {
            if (Bot.Wow.TargetGuid != 0 && Bot.Target != null)
            {
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

                    if (Bot.Target.IsCasting && CustomCastSpell(pummelSpell))
                    {
                        return;
                    }

                    if (CustomCastSpell(bloodrageSpell))
                    {
                        return;
                    }

                    if (CustomCastSpell(berserkerRageSpell))
                    {
                        return;
                    }

                    if (CustomCastSpell(recklessnessSpell, berserkerStanceSpell))
                    {
                        return;
                    }

                    if (Bot.Player.HealthPercentage <= 50 && CustomCastSpell(intimidatingShoutSpell))
                    {
                        return;
                    }

                    if (Bot.Player.HealthPercentage <= 60 && CustomCastSpell(retaliationSpell, battleStanceSpell))
                    {
                        return;
                    }

                    if (Bot.Player.HealthPercentage <= 50 && Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Enrage") && CustomCastSpell(enragedregenerationSpell))
                    {
                        return;
                    }

                    if (Bot.Target.GetType() == typeof(IWowPlayer) && CustomCastSpell(disarmSpell, defensiveStanceSpell))
                    {
                        return;
                    }

                    if (Bot.Target.GetType() == typeof(IWowPlayer) && !Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Hamstring") && CustomCastSpell(hamstringSpell))
                    {
                        return;
                    }

                    if (VictoryRushEvent.Run() && CustomCastSpell(victoryRushSpell))
                    {
                        return;
                    }

                    if ((Bot.Target.HealthPercentage <= 20 && CustomCastSpell(executeSpell)) || (Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Sudden Death") && CustomCastSpell(executeSpell)))
                    {
                        return;
                    }

                    if (RendEvent.Run() && !Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Rend") && CustomCastSpell(rendSpell))
                    {
                        return;
                    }

                    if (Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Taste for Blood") && CustomCastSpell(OverpowerSpell))
                    {
                        return;
                    }

                    if (CustomCastSpell(MortalStrikeSpell))
                    {
                        return;
                    }

                    if (CustomCastSpell(BladestormSpell))
                    {
                        return;
                    }

                    if (!Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Battle Shout") && CustomCastSpell(battleShoutSpell))
                    {
                        return;
                    }
                }
                else//Range
                {
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