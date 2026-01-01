using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Spells.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.WowWotlk.Constants.Classes;
using System;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Kamel
{
    /// <summary>
    /// Base class for Kamel Warrior DPS specs (Arms/Fury).
    /// Provides common spell cooldown initialization, properties, and the CustomCastSpell method.
    /// </summary>
    internal abstract class BaseKamelWarriorClass : BasicKamelClass
    {
        protected BaseKamelWarriorClass(AmeisenBotInterfaces bot) : base()
        {
            Bot = bot;

            // Common stance cooldowns
            spellCoolDown.Add(WarriorWotlk.DefensiveStance, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.BattleStance, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.BerserkerStance, DateTime.Now);

            // Common spell cooldowns
            spellCoolDown.Add(WarriorWotlk.HeroicStrike, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.Intercept, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.HeroicThrow, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.Execute, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.Pummel, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.Slam, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.Disarm, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.Rend, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.Hamstring, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.VictoryRush, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.Charge, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.Cleave, DateTime.Now);

            // Common buff/defensive/enrage cooldowns
            spellCoolDown.Add(WarriorWotlk.IntimidatingShout, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.Retaliation, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.EnragedRegeneration, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.Bloodrage, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.CommandingShout, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.Recklessness, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.HeroicFury, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.BerserkerRage, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.DeathWish, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.BattleShout, DateTime.Now);

            // Common timegated events
            HeroicStrikeEvent = new(TimeSpan.FromSeconds(2));
            VictoryRushEvent = new(TimeSpan.FromSeconds(5));
            ExecuteEvent = new(TimeSpan.FromSeconds(1));
        }

        // Common Warrior properties
        public override bool HandlesMovement => false;
        public override bool IsMelee => true;
        public override IItemComparator ItemComparator { get; set; } = new BasicStrengthComparator(
            [WowArmorType.Shield], 
            [WowWeaponType.Sword, WowWeaponType.Mace, WowWeaponType.Axe, WowWeaponType.Staff, WowWeaponType.Dagger]);
        public override WowRole Role => WowRole.Dps;
        public override bool UseAutoAttacks => true;
        public override bool WalkBehindEnemy => false;
        public override WowClass WowClass => WowClass.Warrior;

        // Common timegated events
        public TimegatedEvent ExecuteEvent { get; protected set; }
        public TimegatedEvent HeroicStrikeEvent { get; protected set; }
        public TimegatedEvent VictoryRushEvent { get; protected set; }

        public override void ExecuteCC()
        {
            StartAttack();
        }

        public override void OutOfCombatExecute()
        {
            Targetselection();
            StartAttack();
        }

        /// <summary>
        /// Attempts to cast a Warrior spell, switching stance if needed.
        /// </summary>
        protected bool CustomCastSpell(string spellName, string stance)
        {
            if (!Bot.Character.SpellBook.IsSpellKnown(stance))
            {
                stance = WarriorWotlk.BattleStance;
            }

            if (Bot.Character.SpellBook.IsSpellKnown(spellName) && Bot.Target != null)
            {
                double distance = Bot.Player.Position.GetDistance(Bot.Target.Position);
                Spell spell = Bot.Character.SpellBook.GetSpellByName(spellName);

                if (Bot.Player.Rage >= spell.Costs && IsSpellReady(spellName))
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
        /// Main attack logic, must be implemented by derived classes.
        /// </summary>
        protected abstract void StartAttack();
    }
}
