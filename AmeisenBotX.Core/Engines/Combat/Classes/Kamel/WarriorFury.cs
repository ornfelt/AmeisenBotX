using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Spells.Objects;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.WowWotlk.Constants.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Kamel
{
    internal class WarriorFury : BasicKamelClass
    {
        // All spell constants moved to AmeisenBotX.WowWotlk.Constants.WarriorWotlk

        public WarriorFury(AmeisenBotInterfaces bot) : base()
        {
            Bot = bot;
            spellCoolDown.Add(WarriorWotlk.Shoot, DateTime.Now);
            //Stances
            spellCoolDown.Add(WarriorWotlk.DefensiveStance, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.BattleStance, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.BerserkerStance, DateTime.Now);
            //Spells
            spellCoolDown.Add(WarriorWotlk.HeroicStrike, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.Intercept, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.HeroicThrow, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.ShatteringThrow, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.Execute, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.Pummel, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.Bloodthirst, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.Slam, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.Whirlwind, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.Disarm, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.Rend, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.Hamstring, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.VictoryRush, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.Charge, DateTime.Now);
            spellCoolDown.Add(WarriorWotlk.Cleave, DateTime.Now);
            //Buffs||Defensive||Enrage
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

            //Time event
            HeroicStrikeEvent = new(TimeSpan.FromSeconds(2));
            VictoryRushEvent = new(TimeSpan.FromSeconds(5));
            RendEvent = new(TimeSpan.FromSeconds(6));
            ExecuteEvent = new(TimeSpan.FromSeconds(1));
        }

        public override string Author => "Lukas";

        public override Dictionary<string, dynamic> C { get; set; } = [];

        public override string Description => "Warrior Fury";

        public override string DisplayName => "Warrior Fury Final";

        public TimegatedEvent ExecuteEvent { get; private set; }

        public override bool HandlesMovement => false;

        public TimegatedEvent HeroicStrikeEvent { get; private set; }

        public override bool IsMelee => true;

        public override IItemComparator ItemComparator { get; set; } = new BasicStrengthComparator([WowArmorType.Shield], [WowWeaponType.Sword, WowWeaponType.Mace, WowWeaponType.Axe, WowWeaponType.Staff, WowWeaponType.Dagger]);

        //Time event
        public TimegatedEvent RendEvent { get; private set; }

        public override WowRole Role => WowRole.Dps;

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
            Tree3 = [],
        };

        public override bool UseAutoAttacks => true;

        public override string Version => "3.0";

        public TimegatedEvent VictoryRushEvent { get; private set; }

        public override bool WalkBehindEnemy => false;

        public override WowClass WowClass => WowClass.Warrior;

        public override void ExecuteCC()
        {
            StartAttack();
        }

        public override void OutOfCombatExecute()
        {
            Targetselection();
            StartAttack();
        }

        private bool CustomCastSpell(string spellName, string stance = WarriorWotlk.BerserkerStance)
        {
            if (!Bot.Character.SpellBook.IsSpellKnown(stance))
            {
                stance = WarriorWotlk.BattleStance;
            }

            if (Bot.Character.SpellBook.IsSpellKnown(spellName))
            {
                if (Bot.Target != null)
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
            }

            return false;
        }

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

                    if (CustomCastSpell(WarriorWotlk.Bloodrage))
                    {
                        return;
                    }

                    if (CustomCastSpell(WarriorWotlk.BerserkerRage))
                    {
                        return;
                    }

                    if (CustomCastSpell(WarriorWotlk.DeathWish))
                    {
                        return;
                    }

                    if (Bot.Target.IsCasting && CustomCastSpell(WarriorWotlk.Pummel))
                    {
                        return;
                    }

                    if (Bot.Target.GetType() == typeof(IWowPlayer) && !Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == WarriorWotlk.Hamstring) && CustomCastSpell(WarriorWotlk.Hamstring))
                    {
                        return;
                    }

                    if (Bot.Target.HealthPercentage <= 20 && CustomCastSpell(WarriorWotlk.Execute))
                    {
                        return;
                    }

                    if (Bot.Player.HealthPercentage <= 50 && (Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == WarriorWotlk.Bloodrage) || Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == WarriorWotlk.Recklessness) || Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == WarriorWotlk.BerserkerRage)))
                    {
                        if (CustomCastSpell(WarriorWotlk.EnragedRegeneration))
                        {
                            return;
                        }
                    }

                    if ((Bot.Player.HealthPercentage <= 30) || ((Bot.Target.GetType() == typeof(IWowPlayer)) && CustomCastSpell(WarriorWotlk.IntimidatingShout)))
                    {
                        return;
                    }

                    if (Bot.Player.HealthPercentage <= 60 && CustomCastSpell(WarriorWotlk.Retaliation, WarriorWotlk.BattleStance))
                    {
                        return;
                    }

                    if (Bot.Target.GetType() == typeof(IWowPlayer) && CustomCastSpell(WarriorWotlk.Disarm, WarriorWotlk.DefensiveStance))
                    {
                        return;
                    }

                    if (Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Slam!") && CustomCastSpell(WarriorWotlk.Slam) && CustomCastSpell(WarriorWotlk.Recklessness))
                    {
                        return;
                    }

                    if (CustomCastSpell(WarriorWotlk.Whirlwind))
                    {
                        return;
                    }

                    if (CustomCastSpell(WarriorWotlk.Bloodthirst))
                    {
                        return;
                    }

                    if (VictoryRushEvent.Run() && CustomCastSpell(WarriorWotlk.VictoryRush))
                    {
                        return;
                    }

                    if (RendEvent.Run() && !Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == WarriorWotlk.Rend) && CustomCastSpell(WarriorWotlk.Rend))
                    {
                        return;
                    }

                    if (HeroicStrikeEvent.Run() && Bot.Player.Rage >= 60 && CustomCastSpell(WarriorWotlk.HeroicStrike))
                    {
                        return;
                    }

                    IEnumerable<IWowUnit> unitsNearPlayer = Bot.GetNearEnemies<IWowUnit>(Bot.Player.Position, 5);

                    if (unitsNearPlayer != null)
                    {
                        if (unitsNearPlayer.Count() >= 3 && Bot.Player.Rage >= 50 && CustomCastSpell(WarriorWotlk.Cleave))
                        {
                            return;
                        }
                    }

                    if (!Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == WarriorWotlk.BattleShout) && CustomCastSpell(WarriorWotlk.BattleShout))
                    {
                        return;
                    }
                }
                else//Range
                {
                    if (Bot.Player.IsDazed
                        || Bot.Player.IsFleeing
                        || Bot.Player.IsInfluenced
                        || Bot.Player.IsPossessed
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
                        if (CustomCastSpell(WarriorWotlk.HeroicFury))
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

                        if (CustomCastSpell(WarriorWotlk.Shoot))
                        {
                            return;
                        }

                        if (CustomCastSpell(WarriorWotlk.ShatteringThrow, WarriorWotlk.BattleStance))
                        {
                            return;
                        }
                    }
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
