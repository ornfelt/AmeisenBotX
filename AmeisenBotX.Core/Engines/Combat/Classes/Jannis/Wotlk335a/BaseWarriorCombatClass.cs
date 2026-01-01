using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.WowWotlk.Constants.Classes;
using System;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Jannis.Wotlk335a
{
    /// <summary>
    /// Base class for WotLK 3.3.5a Warrior DPS specs (Arms/Fury).
    /// Provides common functionality for shouts, debuffs, interrupts, and gap closers.
    /// </summary>
    public abstract class BaseWarriorCombatClass : BasicCombatClass
    {
        protected BaseWarriorCombatClass(AmeisenBotInterfaces bot) : base(bot)
        {
            // Common self-buff: Battle Shout
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, WarriorWotlk.BattleShout, 
                () => TryCastSpell(WarriorWotlk.BattleShout, 0, true)));

            // Common target debuffs for PvP
            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, WarriorWotlk.Hamstring, 
                () => Bot.Target?.Type == WowObjectType.Player && TryCastSpell(WarriorWotlk.Hamstring, Bot.Wow.TargetGuid, true)));
            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, WarriorWotlk.Rend, 
                () => Bot.Target?.Type == WowObjectType.Player && Bot.Player.Rage > 75 && TryCastSpell(WarriorWotlk.Rend, Bot.Wow.TargetGuid, true)));

            // Common interrupt configuration
            InterruptManager.InterruptSpells = new()
            {
                { 0, (x) => TryCastSpellWarrior(WarriorWotlk.IntimidatingShout, WarriorWotlk.BerserkerStance, x.Guid, true) },
                { 1, (x) => TryCastSpellWarrior(WarriorWotlk.IntimidatingShout, WarriorWotlk.BattleStance, x.Guid, true) }
            };

            // Heroic Strike throttle event (shared across specs)
            HeroicStrikeEvent = new(TimeSpan.FromSeconds(2));
        }

        // Common Warrior properties
        public override bool IsMelee => true;
        public override bool UseAutoAttacks => true;
        public override bool WalkBehindEnemy => false;
        public override WowClass WowClass => WowClass.Warrior;
        public override WowVersion WowVersion => WowVersion.WotLK335a;

        /// <summary>
        /// Throttle event to prevent Heroic Strike spam.
        /// </summary>
        protected TimegatedEvent HeroicStrikeEvent { get; }

        /// <summary>
        /// Attempts to break fear/daze/confuse effects using Heroic Fury or Berserker Rage.
        /// </summary>
        /// <returns>True if a break ability was cast.</returns>
        protected bool TryBreakCrowdControl()
        {
            if (Bot.Player.IsDazed || Bot.Player.IsConfused || Bot.Player.IsPossessed || Bot.Player.IsFleeing)
            {
                return TryCastSpell(WarriorWotlk.HeroicFury, 0)
                    || TryCastSpell(WarriorWotlk.BerserkerRage, 0);
            }
            return false;
        }

        /// <summary>
        /// Attempts to close gap using Charge (Battle Stance) or Intercept (Berserker Stance).
        /// </summary>
        /// <returns>True if a gap closer was used.</returns>
        protected bool TryCloseGap()
        {
            return TryCastSpellWarrior(WarriorWotlk.Charge, WarriorWotlk.BattleStance, Bot.Wow.TargetGuid, true)
                || TryCastSpellWarrior(WarriorWotlk.Intercept, WarriorWotlk.BerserkerStance, Bot.Wow.TargetGuid, true);
        }

        /// <summary>
        /// Attempts to execute on low-health targets.
        /// </summary>
        /// <param name="stance">The stance to use for Execute.</param>
        /// <returns>True if Execute was cast.</returns>
        protected bool TryExecute(string stance)
        {
            return Bot.Player.Rage > 25
                && Bot.Target != null
                && Bot.Target.HealthPercentage < 20
                && TryCastSpellWarrior(WarriorWotlk.Execute, stance, Bot.Wow.TargetGuid, true);
        }

        /// <summary>
        /// Attempts to generate rage using Bloodrage (when health permits).
        /// </summary>
        /// <returns>True if Bloodrage was cast.</returns>
        protected bool TryGenerateRage()
        {
            return Bot.Player.HealthPercentage > 50
                && Bot.Player.Rage < 30
                && TryCastSpell(WarriorWotlk.Bloodrage, 0);
        }
    }
}
