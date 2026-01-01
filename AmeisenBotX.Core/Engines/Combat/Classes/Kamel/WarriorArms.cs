using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.WowWotlk.Constants.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Kamel
{
    [CombatClassMetadata("Warrior Arms Beta", "Lukas")]
    internal class WarriorArms : BaseKamelWarriorClass
    {
        public WarriorArms(AmeisenBotInterfaces bot) : base(bot)
        {
            // Arms-specific spell cooldowns
            spellCoolDown.Add(WarriorWotlk.Bladestorm, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.Overpower, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.MortalStrike, DateTime.Now);

            // Arms-specific event timing
            RendEvent = new(TimeSpan.FromSeconds(3));
        }

        public override string Author => "Lukas";

        public override Dictionary<string, dynamic> C { get; set; } = [];

        public override string Description => "Warrior Arms";

        public override string DisplayName => "Warrior Arms Beta";

        public TimegatedEvent RendEvent { get; private set; }

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
            Tree3 = [],
        };

        public override string Version => "1.0";

        /// <summary>
        /// Arms-specific CustomCastSpell defaults to Battle Stance.
        /// </summary>
        private bool CustomCastSpell(string spellName, string stance = WarriorWotlk.BattleStance)
        {
            return base.CustomCastSpell(spellName, stance);
        }

        protected override void StartAttack()
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

                    if (Bot.Target.IsCasting && CustomCastSpell(WarriorWotlk.Pummel))
                    {
                        return;
                    }

                    if (CustomCastSpell(WarriorWotlk.Bloodrage))
                    {
                        return;
                    }

                    if (CustomCastSpell(WarriorWotlk.BerserkerRage))
                    {
                        return;
                    }

                    if (CustomCastSpell(WarriorWotlk.Recklessness, WarriorWotlk.BerserkerStance))
                    {
                        return;
                    }

                    if (Bot.Player.HealthPercentage <= 50 && CustomCastSpell(WarriorWotlk.IntimidatingShout))
                    {
                        return;
                    }

                    if (Bot.Player.HealthPercentage <= 60 && CustomCastSpell(WarriorWotlk.Retaliation, WarriorWotlk.BattleStance))
                    {
                        return;
                    }

                    if (Bot.Player.HealthPercentage <= 50 && Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Enrage") && CustomCastSpell(WarriorWotlk.EnragedRegeneration))
                    {
                        return;
                    }

                    if (Bot.Target.GetType() == typeof(IWowPlayer) && CustomCastSpell(WarriorWotlk.Disarm, WarriorWotlk.DefensiveStance))
                    {
                        return;
                    }

                    if (Bot.Target.GetType() == typeof(IWowPlayer) && !Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == WarriorWotlk.Hamstring) && CustomCastSpell(WarriorWotlk.Hamstring))
                    {
                        return;
                    }

                    if (VictoryRushEvent.Run() && CustomCastSpell(WarriorWotlk.VictoryRush))
                    {
                        return;
                    }

                    if ((Bot.Target.HealthPercentage <= 20 && CustomCastSpell(WarriorWotlk.Execute)) || (Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Sudden Death") && CustomCastSpell(WarriorWotlk.Execute)))
                    {
                        return;
                    }

                    if (RendEvent.Run() && !Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == WarriorWotlk.Rend) && CustomCastSpell(WarriorWotlk.Rend))
                    {
                        return;
                    }

                    if (Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Taste for Blood") && CustomCastSpell(WarriorWotlk.Overpower))
                    {
                        return;
                    }

                    if (CustomCastSpell(WarriorWotlk.MortalStrike))
                    {
                        return;
                    }

                    if (CustomCastSpell(WarriorWotlk.Bladestorm))
                    {
                        return;
                    }

                    if (!Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == WarriorWotlk.BattleShout) && CustomCastSpell(WarriorWotlk.BattleShout))
                    {
                        return;
                    }
                }
                else // Out of range
                {
                    if (CustomCastSpell(WarriorWotlk.Intercept))
                    {
                        return;
                    }
                    if (CustomCastSpell(WarriorWotlk.Charge, WarriorWotlk.BattleStance))
                    {
                        return;
                    }
                    if (CustomCastSpell(WarriorWotlk.HeroicThrow))
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
