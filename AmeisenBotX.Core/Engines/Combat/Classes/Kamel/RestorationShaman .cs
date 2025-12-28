using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.WowWotlk.Constants.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Kamel
{
    internal class RestorationShaman : BasicKamelClass
    {
        // All spell constants moved to AmeisenBotX.WowWotlk.Constants.ShamanWotlk

        public RestorationShaman(AmeisenBotInterfaces bot) : base()
        {
            Bot = bot;

            //Spells / DMG
            spellCoolDown.Add(ShamanWotlk.LightningBolt, DateTime.Now);
            spellCoolDown.Add(ShamanWotlk.FlameShock, DateTime.Now);
            spellCoolDown.Add(ShamanWotlk.EarthShock, DateTime.Now);

            //Spells
            spellCoolDown.Add(ShamanWotlk.HealingWave, DateTime.Now);
            spellCoolDown.Add(ShamanWotlk.LesserHealingWave, DateTime.Now);
            spellCoolDown.Add(ShamanWotlk.Riptide, DateTime.Now);
            spellCoolDown.Add(ShamanWotlk.WaterShield, DateTime.Now);
            spellCoolDown.Add(ShamanWotlk.LightningShield, DateTime.Now);
            spellCoolDown.Add(ShamanWotlk.ChainHeal, DateTime.Now);
            spellCoolDown.Add(ShamanWotlk.EarthlivingBuff, DateTime.Now);
            spellCoolDown.Add(ShamanWotlk.EarthlivingWeapon, DateTime.Now);
            spellCoolDown.Add(ShamanWotlk.WindShear, DateTime.Now);

            //CD|Buffs
            spellCoolDown.Add(ShamanWotlk.NaturesSwiftness, DateTime.Now);
            spellCoolDown.Add(ShamanWotlk.Heroism, DateTime.Now);
            spellCoolDown.Add(ShamanWotlk.Bloodlust, DateTime.Now);
            spellCoolDown.Add(ShamanWotlk.TidalForce, DateTime.Now);
            spellCoolDown.Add(ShamanWotlk.EarthShield, DateTime.Now);

            //Totem
            spellCoolDown.Add(ShamanWotlk.WindfuryTotem, DateTime.Now);
            spellCoolDown.Add(ShamanWotlk.StrengthOfEarthTotem, DateTime.Now);
            spellCoolDown.Add(ShamanWotlk.ManaSpringTotem, DateTime.Now);
            spellCoolDown.Add(ShamanWotlk.ManaTideTotem, DateTime.Now);
            spellCoolDown.Add(ShamanWotlk.CallOfTheElements, DateTime.Now);

            //Time event
            EarthShieldEvent = new(TimeSpan.FromSeconds(7));
            ManaTideTotemEvent = new(TimeSpan.FromSeconds(12));
            TotemcastEvent = new(TimeSpan.FromSeconds(4));
        }

        public override string Author => "Lukas";

        public override Dictionary<string, dynamic> C { get; set; } = [];

        public override string Description => "Resto Shaman";

        public override string DisplayName => "Shaman Restoration";

        //Time event
        public TimegatedEvent EarthShieldEvent { get; private set; }

        public override bool HandlesMovement => false;

        public override bool IsMelee => false;

        public override IItemComparator ItemComparator { get; set; } = new BasicSpiritComparator([WowArmorType.Shield], [WowWeaponType.Sword, WowWeaponType.Mace, WowWeaponType.Axe]);

        public TimegatedEvent ManaTideTotemEvent { get; private set; }

        public override WowRole Role => WowRole.Heal;

        public override TalentTree Talents { get; } = new()
        {
            Tree1 = [],
            Tree2 = new()
            {
                { 3, new(2, 3, 5) },
                { 5, new(2, 5, 5) },
                { 7, new(2, 7, 3) },
                { 8, new(2, 8, 1) },
            },
            Tree3 = new()
            {
                { 1, new(3, 1, 5) },
                { 5, new(3, 5, 5) },
                { 6, new(3, 6, 3) },
                { 7, new(3, 7, 3) },
                { 8, new(3, 8, 1) },
                { 9, new(3, 9, 3) },
                { 10, new(3, 10, 3) },
                { 11, new(3, 11, 5) },
                { 12, new(3, 12, 3) },
                { 13, new(3, 13, 1) },
                { 15, new(3, 15, 5) },
                { 17, new(3, 17, 1) },
                { 19, new(3, 19, 2) },
                { 20, new(3, 20, 2) },
                { 21, new(3, 21, 3) },
                { 22, new(3, 22, 3) },
                { 23, new(3, 23, 1) },
                { 24, new(3, 24, 2) },
                { 25, new(3, 25, 5) },
                { 26, new(3, 26, 1) },
            },
        };

        public bool TargetIsInRange { get; set; }

        public TimegatedEvent TotemcastEvent { get; private set; }

        public override bool UseAutoAttacks => false;

        public bool UseSpellOnlyInCombat { get; private set; }

        public override string Version => "2.1";

        public override bool WalkBehindEnemy => false;

        public override WowClass WowClass => WowClass.Shaman;

        public override void ExecuteCC()
        {
            UseSpellOnlyInCombat = true;
            Shield();
            StartHeal();
        }

        public override void OutOfCombatExecute()
        {
            RevivePartyMember(ancestralSpiritSpell);

            if (CheckForWeaponEnchantment(WowEquipmentSlot.INVSLOT_MAINHAND, ShamanWotlk.EarthlivingBuff, ShamanWotlk.EarthlivingWeapon))
            {
                return;
            }
            UseSpellOnlyInCombat = false;
            Shield();
            StartHeal();
        }

        private void Shield()
        {
            if (!Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == ShamanWotlk.WaterShield) && CustomCastSpellMana(ShamanWotlk.WaterShield))
            {
                return;
            }
        }

        private void StartHeal()
        {
            List<IWowUnit> partyMemberToHeal =
            [
.. Bot.Objects.Partymembers,                 //healableUnits.AddRange(Bot.ObjectManager.PartyPets);
                Bot.Player
            ];

            partyMemberToHeal = [.. partyMemberToHeal.Where(e => e.HealthPercentage <= 94 && !e.IsDead).OrderBy(e => e.HealthPercentage)];

            if (partyMemberToHeal.Count > 0)
            {
                if (Bot.Wow.TargetGuid != partyMemberToHeal.FirstOrDefault().Guid)
                {
                    Bot.Wow.ChangeTarget(partyMemberToHeal.FirstOrDefault().Guid);
                }

                if (Bot.Wow.TargetGuid != 0 && Bot.Target != null)
                {
                    TargetIsInRange = Bot.Player.Position.GetDistance(Bot.GetWowObjectByGuid<IWowUnit>(partyMemberToHeal.FirstOrDefault().Guid).Position) <= 30;
                    if (TargetIsInRange)
                    {
                        if (!TargetInLineOfSight)
                        {
                            return;
                        }
                        if (Bot.Movement.Status != Movement.Enums.MovementAction.None)
                        {
                            Bot.Wow.StopClickToMove();
                            Bot.Movement.Reset();
                        }

                        if (Bot.Target != null && Bot.Target.HealthPercentage >= 90)
                        {
                            Bot.Wow.LuaDoString("SpellStopCasting()");
                            return;
                        }

                        if (UseSpellOnlyInCombat && Bot.Player.HealthPercentage < 20 && CustomCastSpellMana(ShamanWotlk.Heroism))
                        {
                            return;
                        }

                        if (UseSpellOnlyInCombat && Bot.Target.HealthPercentage < 20 && CustomCastSpellMana(ShamanWotlk.NaturesSwiftness) && CustomCastSpellMana(ShamanWotlk.HealingWave))
                        {
                            return;
                        }

                        if (UseSpellOnlyInCombat && Bot.Target.HealthPercentage < 40 && CustomCastSpellMana(ShamanWotlk.TidalForce))
                        {
                            return;
                        }

                        //Race Draenei
                        if (Bot.Player.Race == WowRace.Draenei && Bot.Target.HealthPercentage < 50 && CustomCastSpellMana(RacialsWotlk.GiftOfTheNaaru))
                        {
                            return;
                        }

                        if (Bot.Target.HealthPercentage <= 50 && CustomCastSpellMana(ShamanWotlk.HealingWave))
                        {
                            return;
                        }

                        if (Bot.Target.HealthPercentage <= 75 && CustomCastSpellMana(ShamanWotlk.LesserHealingWave))
                        {
                            return;
                        }

                        if (partyMemberToHeal.Count >= 4 && Bot.Target.HealthPercentage >= 80 && CustomCastSpellMana(ShamanWotlk.ChainHeal))
                        {
                            return;
                        }

                        if (UseSpellOnlyInCombat && EarthShieldEvent.Run() && !Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == ShamanWotlk.EarthShield) && !Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == ShamanWotlk.WaterShield) && Bot.Target.HealthPercentage < 90 && CustomCastSpellMana(ShamanWotlk.EarthShield))
                        {
                            return;
                        }

                        if (!Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == ShamanWotlk.Riptide) && Bot.Target.HealthPercentage < 90 && CustomCastSpellMana(ShamanWotlk.Riptide))
                        {
                            return;
                        }
                    }

                    if (TotemcastEvent.Run() && TotemItemCheck())
                    {
                        if (Bot.Player.ManaPercentage <= 10 && CustomCastSpellMana(ShamanWotlk.ManaTideTotem))
                        {
                            return;
                        }
                    }
                }
            }
            else
            {
                TotemItemCheck();

                if (TotemcastEvent.Run() && TotemItemCheck())
                {
                    if (Bot.Player.ManaPercentage >= 50
                        && !Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == ShamanWotlk.WindfuryTotem)
                        && !Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Stoneskin")
                        && !Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Flametongue Totem")
                        && CustomCastSpellMana(ShamanWotlk.CallOfTheElements))
                    {
                        return;
                    }
                }

                if (TargetSelectEvent.Run())
                {
                    IWowUnit nearTarget = Bot.GetNearEnemies<IWowUnit>(Bot.Player.Position, 30)
                    .Where(e => e.IsInCombat && !e.IsNotAttackable && e.IsCasting && Bot.Db.GetUnitName(Bot.Target, out string name) && name != "The Lich King" && !(Bot.Objects.MapId == WowMapId.DrakTharonKeep && e.CurrentlyChannelingSpellId == 47346))
                    .OrderBy(e => e.Position.GetDistance(Bot.Player.Position))
                    .FirstOrDefault();

                    if (Bot.Wow.TargetGuid != 0 && Bot.Target != null && nearTarget != null)
                    {
                        Bot.Wow.ChangeTarget(nearTarget.Guid);

                        if (!TargetInLineOfSight)
                        {
                            return;
                        }
                        if (Bot.Movement.Status != Movement.Enums.MovementAction.None)
                        {
                            Bot.Wow.StopClickToMove();
                            Bot.Movement.Reset();
                        }
                        if (UseSpellOnlyInCombat && Bot.Target.IsCasting && CustomCastSpellMana(ShamanWotlk.WindShear))
                        {
                            return;
                        }
                        if (UseSpellOnlyInCombat && Bot.Player.ManaPercentage >= 80 && CustomCastSpellMana(ShamanWotlk.FlameShock))
                        {
                            return;
                        }
                    }
                }
            }
        }
    }
}
