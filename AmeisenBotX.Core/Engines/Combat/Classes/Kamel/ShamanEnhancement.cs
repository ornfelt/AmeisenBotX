using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.WowWotlk.Constants.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Kamel
{
    internal class ShamanEnhancement : BasicKamelClass
    {
        // All spell constants moved to AmeisenBotX.WowWotlk.Constants.ShamanWotlk

        public ShamanEnhancement(AmeisenBotInterfaces bot) : base()
        {
            Bot = bot;

            //Shield
            spellCoolDown.Add(ShamanWotlk.LightningShield, DateTime.Now);

            //Weapon Enhancement
            spellCoolDown.Add(ShamanWotlk.WindfuryBuff, DateTime.Now);
            spellCoolDown.Add(ShamanWotlk.FlametongueBuff, DateTime.Now);
            spellCoolDown.Add(ShamanWotlk.FlametongueWeapon, DateTime.Now);
            spellCoolDown.Add(ShamanWotlk.WindfuryWeapon, DateTime.Now);

            //Heal Spells
            spellCoolDown.Add(ShamanWotlk.HealingWave, DateTime.Now);

            //Totem
            spellCoolDown.Add(ShamanWotlk.FireElementalTotem, DateTime.Now);
            spellCoolDown.Add(ShamanWotlk.EarthElementalTotem, DateTime.Now);
            spellCoolDown.Add(ShamanWotlk.GroundingTotem, DateTime.Now);
            spellCoolDown.Add(ShamanWotlk.EarthbindTotem, DateTime.Now);

            //Attack Spells
            spellCoolDown.Add(ShamanWotlk.LightningBolt, DateTime.Now);
            spellCoolDown.Add(ShamanWotlk.LavaLash, DateTime.Now);
            spellCoolDown.Add(ShamanWotlk.Stormstrike, DateTime.Now);
            spellCoolDown.Add(ShamanWotlk.FlameShock, DateTime.Now);
            spellCoolDown.Add(ShamanWotlk.FrostShock, DateTime.Now);
            spellCoolDown.Add(ShamanWotlk.EarthShock, DateTime.Now);
            spellCoolDown.Add(ShamanWotlk.FeralSpirit, DateTime.Now);

            //Stunns|Interrupting
            spellCoolDown.Add(ShamanWotlk.WindShear, DateTime.Now);
            spellCoolDown.Add(ShamanWotlk.Purge, DateTime.Now);

            //Buff
            spellCoolDown.Add(ShamanWotlk.ShamanisticRage, DateTime.Now);

            //Event
            EnhancementEvent = new(TimeSpan.FromSeconds(2));
            PurgeEvent = new(TimeSpan.FromSeconds(1));
        }

        public override string Author => "Lukas";

        public override Dictionary<string, dynamic> C { get; set; } = [];

        public override string Description => "Shaman Enhancement";

        public override string DisplayName => "Shaman Enhancement";

        //Event
        public TimegatedEvent EnhancementEvent { get; private set; }

        public override bool HandlesMovement => false;

        public override bool IsMelee => true;

        public override IItemComparator ItemComparator { get; set; } = new BasicIntellectComparator([WowArmorType.Shield], [WowWeaponType.AxeTwoHand, WowWeaponType.MaceTwoHand, WowWeaponType.SwordTwoHand]);

        public TimegatedEvent PurgeEvent { get; private set; }

        public override WowRole Role => WowRole.Dps;

        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 2, new(1, 2, 5) },
                { 3, new(1, 3, 3) },
                { 5, new(1, 5, 3) },
                { 8, new(1, 8, 5) },
            },
            Tree2 = new()
            {
                { 3, new(2, 3, 5) },
                { 5, new(2, 5, 5) },
                { 7, new(2, 7, 3) },
                { 8, new(2, 8, 3) },
                { 9, new(2, 9, 1) },
                { 11, new(2, 11, 5) },
                { 13, new(2, 13, 2) },
                { 14, new(2, 14, 1) },
                { 15, new(2, 15, 3) },
                { 16, new(2, 16, 3) },
                { 17, new(2, 17, 3) },
                { 19, new(2, 19, 3) },
                { 20, new(2, 20, 1) },
                { 21, new(2, 21, 1) },
                { 22, new(2, 22, 3) },
                { 23, new(2, 23, 1) },
                { 24, new(2, 24, 2) },
                { 25, new(2, 25, 3) },
                { 26, new(2, 26, 1) },
                { 28, new(2, 28, 5) },
                { 29, new(2, 29, 1) },
            },
            Tree3 = [],
        };

        public override bool UseAutoAttacks => true;

        public override string Version => "1.0";

        public override bool WalkBehindEnemy => false;

        public override WowClass WowClass => WowClass.Shaman;

        public override void ExecuteCC()
        {
            StartAttack();
        }

        public override void OutOfCombatExecute()
        {
            Shield();
            WeaponEnhancement();
            RevivePartyMember(ancestralSpiritSpell);
            Targetselection();
            StartAttack();
        }

        private void Shield()
        {
            if (!Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == ShamanWotlk.LightningShield) && CustomCastSpellMana(ShamanWotlk.LightningShield))
            {
                return;
            }
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

                    if (Bot.Player.Auras.FirstOrDefault(e => Bot.Db.GetSpellName(e.SpellId) == ShamanWotlk.MaelstromWeapon).StackCount >= 5
                    && ((Bot.Player.HealthPercentage >= 50 && CustomCastSpellMana(ShamanWotlk.LightningBolt)) || CustomCastSpellMana(ShamanWotlk.HealingWave)))
                    {
                        return;
                    }
                    if (Bot.Target.IsCasting && CustomCastSpellMana(ShamanWotlk.WindShear))
                    {
                        return;
                    }
                    // Note: Enemy buff checks kept as inline strings
                    if (PurgeEvent.Run() &&
                        (Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == "Mana Shield")
                      || Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == PriestWotlk.PowerWordShield)
                      || Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == PriestWotlk.Renew)
                      || Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == ShamanWotlk.Riptide)
                      || Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == ShamanWotlk.EarthShield)) && CustomCastSpellMana(ShamanWotlk.Purge))
                    {
                        return;
                    }
                    if (TotemItemCheck() && CustomCastSpellMana(ShamanWotlk.FireElementalTotem))
                    {
                        return;
                    }
                    if (TotemItemCheck() && CustomCastSpellMana(ShamanWotlk.EarthElementalTotem))
                    {
                        return;
                    }
                    if (CustomCastSpellMana(ShamanWotlk.LavaLash))
                    {
                        return;
                    }
                    if (CustomCastSpellMana(ShamanWotlk.Stormstrike))
                    {
                        return;
                    }
                    if (CustomCastSpellMana(ShamanWotlk.FeralSpirit))
                    {
                        return;
                    }
                    if (!Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == ShamanWotlk.FlameShock) && CustomCastSpellMana(ShamanWotlk.FlameShock))
                    {
                        return;
                    }
                    if (CustomCastSpellMana(ShamanWotlk.FrostShock))
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

        private void WeaponEnhancement()
        {
            if (EnhancementEvent.Run())
            {
                if (CheckForWeaponEnchantment(WowEquipmentSlot.INVSLOT_MAINHAND, ShamanWotlk.WindfuryBuff, ShamanWotlk.WindfuryWeapon))
                {
                    return;
                }

                if (CheckForWeaponEnchantment(WowEquipmentSlot.INVSLOT_OFFHAND, ShamanWotlk.FlametongueBuff, ShamanWotlk.FlametongueWeapon))
                {
                    return;
                }
            }
        }
    }
}
