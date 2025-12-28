using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.WowWotlk.Constants.Classes;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Jannis.Wotlk335a
{
    public class ShamanEnhancement : BasicCombatClass
    {
        public ShamanEnhancement(AmeisenBotInterfaces bot) : base(bot)
        {
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, ShamanWotlk.LightningShield, () => Bot.Player.ManaPercentage > 60.0 && TryCastSpell(ShamanWotlk.LightningShield, 0, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, ShamanWotlk.WaterShield, () => Bot.Player.ManaPercentage < 20.0 && TryCastSpell(ShamanWotlk.WaterShield, 0, true)));

            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, ShamanWotlk.FlameShock, () => TryCastSpell(ShamanWotlk.FlameShock, Bot.Wow.TargetGuid, true)));

            InterruptManager.InterruptSpells = new()
            {
                { 0, (x) => TryCastSpell(ShamanWotlk.WindShear, x.Guid, true) },
                { 1, (x) => TryCastSpell(ShamanWotlk.Hex, x.Guid, true) }
            };
        }

        public override string Description => "FCFS based CombatClass for the Enhancement Shaman spec.";

        public override string DisplayName2 => "Shaman Enhancement";

        public override bool HandlesMovement => false;

        public override bool IsMelee => true;

        public override IItemComparator ItemComparator { get; set; } = new BasicIntellectComparator([WowArmorType.Shield], [WowWeaponType.AxeTwoHand, WowWeaponType.MaceTwoHand, WowWeaponType.SwordTwoHand]);

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

        public override WowVersion WowVersion => WowVersion.WotLK335a;

        private bool HexedTarget { get; set; }

        public override void Execute()
        {
            base.Execute();

            if (TryFindTarget(TargetProviderDps, out _))
            {
                if (CheckForWeaponEnchantment(WowEquipmentSlot.INVSLOT_MAINHAND, ShamanWotlk.FlametongueBuff, ShamanWotlk.FlametongueWeapon)
                    || CheckForWeaponEnchantment(WowEquipmentSlot.INVSLOT_OFFHAND, ShamanWotlk.WindfuryBuff, ShamanWotlk.WindfuryWeapon))
                {
                    return;
                }

                if (Bot.Player.HealthPercentage < 30
                    && Bot.Target.Type == WowObjectType.Player
                    && TryCastSpell(ShamanWotlk.Hex, Bot.Wow.TargetGuid, true))
                {
                    HexedTarget = true;
                    return;
                }

                if (Bot.Player.HealthPercentage < 60
                    && TryCastSpell(ShamanWotlk.HealingWave, Bot.Wow.PlayerGuid, true))
                {
                    return;
                }

                if (Bot.Target != null)
                {
                    if ((Bot.Target.MaxHealth > 10000000
                            && Bot.Target.HealthPercentage < 25
                            && TryCastSpell(ShamanWotlk.Heroism, 0))
                        || TryCastSpell(ShamanWotlk.Stormstrike, Bot.Wow.TargetGuid, true)
                        || TryCastSpell(ShamanWotlk.LavaLash, Bot.Wow.TargetGuid, true)
                        || TryCastSpell(ShamanWotlk.EarthShock, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }

                    if (Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == ShamanWotlk.MaelstromWeapon)
                        && Bot.Player.Auras.FirstOrDefault(e => Bot.Db.GetSpellName(e.SpellId) == ShamanWotlk.MaelstromWeapon).StackCount >= 5
                        && TryCastSpell(ShamanWotlk.LightningBolt, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }
                }
            }
        }

        public override void OutOfCombatExecute()
        {
            base.OutOfCombatExecute();

            if (HandleDeadPartymembers(ShamanWotlk.AncestralSpirit))
            {
                return;
            }

            if (CheckForWeaponEnchantment(WowEquipmentSlot.INVSLOT_MAINHAND, ShamanWotlk.FlametongueBuff, ShamanWotlk.FlametongueWeapon)
                || CheckForWeaponEnchantment(WowEquipmentSlot.INVSLOT_OFFHAND, ShamanWotlk.WindfuryBuff, ShamanWotlk.WindfuryWeapon))
            {
                return;
            }

            HexedTarget = false;
        }
    }
}
