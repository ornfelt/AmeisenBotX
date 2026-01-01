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
    [CombatClassMetadata("Paladin Protection", "Lukas")]
    internal class PaladinProtection : BasicKamelClass
    {
        // All spell constants moved to AmeisenBotX.WowWotlk.Constants.PaladinWotlk

        public PaladinProtection(AmeisenBotInterfaces bot) : base()
        {
            Bot = bot;

            //Spell
            spellCoolDown.Add(PaladinWotlk.AvengersShield, DateTime.Now);
            spellCoolDown.Add(PaladinWotlk.Consecration, DateTime.Now);
            spellCoolDown.Add(PaladinWotlk.JudgementOfLight, DateTime.Now);
            spellCoolDown.Add(PaladinWotlk.HolyShield, DateTime.Now);
            spellCoolDown.Add(PaladinWotlk.HammerOfTheRighteous, DateTime.Now);
            spellCoolDown.Add(PaladinWotlk.HammerOfWrath, DateTime.Now);
            spellCoolDown.Add(PaladinWotlk.Exorcism, DateTime.Now);
            spellCoolDown.Add(PaladinWotlk.DivineProtection, DateTime.Now);
            spellCoolDown.Add(PaladinWotlk.HandOfReckoning, DateTime.Now);
            spellCoolDown.Add(PaladinWotlk.HammerOfJustice, DateTime.Now);
            spellCoolDown.Add(PaladinWotlk.LayOnHands, DateTime.Now);
            spellCoolDown.Add(PaladinWotlk.HolyLight, DateTime.Now);
            spellCoolDown.Add(PaladinWotlk.AvengingWrath, DateTime.Now);
            spellCoolDown.Add(PaladinWotlk.DivinePlea, DateTime.Now);
            spellCoolDown.Add(PaladinWotlk.SacredShield, DateTime.Now);

            //Buff
            spellCoolDown.Add(PaladinWotlk.BlessingOfKings, DateTime.Now);
            spellCoolDown.Add(PaladinWotlk.SealOfLight, DateTime.Now);
            spellCoolDown.Add(PaladinWotlk.SealOfWisdom, DateTime.Now);
            spellCoolDown.Add(PaladinWotlk.DevotionAura, DateTime.Now);
            spellCoolDown.Add(PaladinWotlk.RighteousFury, DateTime.Now);

            //Time event
            ShieldEvent = new(TimeSpan.FromSeconds(8));
        }

        public override string Author => "Lukas";

        public override Dictionary<string, dynamic> C { get; set; } = [];

        public override string Description => "Paladin Protection 1.0";

        public override string DisplayName => "Paladin Protection";

        public TimegatedEvent ExecuteEvent { get; private set; }

        public override bool HandlesMovement => false;

        public override bool IsMelee => true;

        public override IItemComparator ItemComparator { get; set; } = new BasicStaminaComparator([WowArmorType.Shield], [WowWeaponType.Sword, WowWeaponType.Mace, WowWeaponType.Axe, WowWeaponType.Staff, WowWeaponType.Dagger]);

        public override WowRole Role => WowRole.Tank;

        public TimegatedEvent ShieldEvent { get; private set; }

        public override TalentTree Talents { get; } = new()
        {
            Tree1 = [],
            Tree2 = new()
            {
                { 2, new(2, 2, 5) },
                { 5, new(2, 5, 5) },
                { 6, new(2, 6, 1) },
                { 7, new(2, 7, 3) },
                { 8, new(2, 8, 5) },
                { 9, new(2, 9, 2) },
                { 11, new(2, 11, 3) },
                { 12, new(2, 12, 1) },
                { 14, new(2, 14, 2) },
                { 15, new(2, 15, 3) },
                { 16, new(2, 16, 1) },
                { 17, new(2, 17, 1) },
                { 18, new(2, 18, 3) },
                { 19, new(2, 19, 3) },
                { 20, new(2, 20, 3) },
                { 21, new(2, 21, 3) },
                { 22, new(2, 22, 1) },
                { 23, new(2, 23, 2) },
                { 24, new(2, 24, 3) },
                { 25, new(2, 25, 2) },
                { 26, new(2, 26, 1) },
            },
            Tree3 = new()
            {
                { 1, new(3, 1, 5) },
                { 3, new(3, 3, 2) },
                { 4, new(3, 4, 3) },
                { 7, new(3, 7, 5) },
                { 12, new(3, 12, 3) },
            },
        };

        public override bool UseAutoAttacks => true;

        public override string Version => "1.0";

        public override bool WalkBehindEnemy => false;

        public override WowClass WowClass => WowClass.Paladin;

        public override void ExecuteCC()
        {
            StartAttack();
        }

        public override void OutOfCombatExecute()
        {
            RevivePartyMember(redemptionSpell);
            BuffManager();
            TargetselectionTank();
            StartAttack();
        }

        private void BuffManager()
        {
            if (TargetSelectEvent.Run())
            {
                List<IWowUnit> CastBuff =
                [
.. Bot.Objects.Partymembers,                     Bot.Player
                ];

                CastBuff =
                [
                    .. CastBuff.Where(e => !e.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == PaladinWotlk.BlessingOfKings) && !e.IsDead).OrderBy(e => e.HealthPercentage),
                ];

                if (CastBuff != null)
                {
                    if (CastBuff.Count > 0)
                    {
                        if (Bot.Wow.TargetGuid != CastBuff.FirstOrDefault().Guid)
                        {
                            Bot.Wow.ChangeTarget(CastBuff.FirstOrDefault().Guid);
                        }
                    }
                    if (Bot.Wow.TargetGuid != 0 && Bot.Target != null)
                    {
                        if (!TargetInLineOfSight)
                        {
                            return;
                        }
                        if (!Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == PaladinWotlk.BlessingOfKings) && CustomCastSpell(PaladinWotlk.BlessingOfKings))
                        {
                            return;
                        }
                    }
                }
            }
            if (!Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == PaladinWotlk.SealOfWisdom) && CustomCastSpell(PaladinWotlk.SealOfWisdom))
            {
                return;
            }
            if (!Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == PaladinWotlk.DevotionAura) && CustomCastSpell(PaladinWotlk.DevotionAura))
            {
                return;
            }
            if (!Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == PaladinWotlk.RighteousFury) && CustomCastSpell(PaladinWotlk.RighteousFury))
            {
                return;
            }
        }

        private bool CustomCastSpell(string spellName)
        {
            if (Bot.Character.SpellBook.IsSpellKnown(spellName))
            {
                if (Bot.Target != null)
                {
                    double distance = Bot.Player.Position.GetDistance(Bot.Target.Position);
                    Spell spell = Bot.Character.SpellBook.GetSpellByName(spellName);

                    if (Bot.Player.Mana >= spell.Costs && IsSpellReady(spellName))
                    {
                        if ((spell.MinRange == 0 && spell.MaxRange == 0) || (spell.MinRange <= distance && spell.MaxRange >= distance))
                        {
                            Bot.Wow.CastSpell(spellName);
                            return true;
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
                if (Bot.Wow.TargetGuid != Bot.Wow.PlayerGuid)
                {
                    TargetselectionTank();
                }

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

                    if ((Bot.Player.IsConfused || Bot.Player.IsSilenced || Bot.Player.IsDazed) && CustomCastSpell(RacialsWotlk.EveryManForHimself))
                    {
                        return;
                    }

                    if (CustomCastSpell(PaladinWotlk.AvengingWrath))
                    {
                        return;
                    }

                    if (Bot.Player.ManaPercentage <= 20 && CustomCastSpell(PaladinWotlk.DivinePlea))
                    {
                        return;
                    }

                    if (ShieldEvent.Run() && CustomCastSpell(PaladinWotlk.SacredShield))
                    {
                        return;
                    }

                    if (Bot.Player.HealthPercentage <= 15 && CustomCastSpell(PaladinWotlk.LayOnHands))
                    {
                        return;
                    }
                    if (Bot.Player.HealthPercentage <= 25 && CustomCastSpell(PaladinWotlk.HolyLight))
                    {
                        return;
                    }
                    if (Bot.Player.HealthPercentage <= 50 && CustomCastSpell(PaladinWotlk.DivineProtection))
                    {
                        return;
                    }
                    if (Bot.Target.HealthPercentage <= 20 && CustomCastSpell(PaladinWotlk.HammerOfWrath))
                    {
                        return;
                    }
                    if ((Bot.Target.HealthPercentage <= 20 || Bot.Player.HealthPercentage <= 30 || Bot.Target.IsCasting) && CustomCastSpell(PaladinWotlk.HammerOfJustice))
                    {
                        return;
                    }
                    if (Bot.Db.GetUnitName(Bot.Target, out string name) && name != "Anub'Rekhan" && CustomCastSpell(PaladinWotlk.HandOfReckoning))
                    {
                        return;
                    }
                    if (CustomCastSpell(PaladinWotlk.AvengersShield))
                    {
                        return;
                    }
                    if (CustomCastSpell(PaladinWotlk.Consecration))
                    {
                        return;
                    }
                    if (CustomCastSpell(PaladinWotlk.JudgementOfLight))
                    {
                        return;
                    }
                    if (CustomCastSpell(PaladinWotlk.HolyShield))
                    {
                        return;
                    }
                    if (CustomCastSpell(PaladinWotlk.Exorcism))
                    {
                        return;
                    }
                }
                else//Range
                {
                    if (CustomCastSpell(PaladinWotlk.AvengersShield))
                    {
                        return;
                    }
                }
            }
            else
            {
                TargetselectionTank();
            }
        }
    }
}
