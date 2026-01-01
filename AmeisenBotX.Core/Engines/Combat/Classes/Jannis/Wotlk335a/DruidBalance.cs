using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.WowWotlk.Constants.Classes;
using System;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Jannis.Wotlk335a
{
    [CombatClassMetadata("[WotLK335a] Druid Balance", "Jannis")]
    public class DruidBalance : BasicCombatClass
    {
        public DruidBalance(AmeisenBotInterfaces bot) : base(bot)
        {
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, DruidWotlk.MoonkinForm, () => TryCastSpell(DruidWotlk.MoonkinForm, 0, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, DruidWotlk.Thorns, () => TryCastSpell(DruidWotlk.Thorns, 0, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, DruidWotlk.MarkOfTheWild, () => TryCastSpell(DruidWotlk.MarkOfTheWild, Bot.Wow.PlayerGuid, true, 0, true)));

            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, DruidWotlk.Moonfire, () => LunarEclipse && TryCastSpell(DruidWotlk.Moonfire, Bot.Wow.TargetGuid, true)));
            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, DruidWotlk.InsectSwarm, () => SolarEclipse && TryCastSpell(DruidWotlk.InsectSwarm, Bot.Wow.TargetGuid, true)));

            InterruptManager.InterruptSpells = new()
            {
                { 0, (x) => TryCastSpell(DruidWotlk.FaerieFire, x.Guid, true) },
            };

            GroupAuraManager.SpellsToKeepActiveOnParty.Add((DruidWotlk.MarkOfTheWild, (spellName, guid) => TryCastSpell(spellName, guid, true)));

            SolarEclipse = false;
            LunarEclipse = true;
        }

        public override string Description => "FCFS based CombatClass for the Balance (Owl) Druid spec.";

        public override string DisplayName2 => "Druid Balance";

        public override bool HandlesMovement => false;

        public override bool IsMelee => false;

        public override IItemComparator ItemComparator { get; set; } = new BasicIntellectComparator([WowArmorType.Shield], [WowWeaponType.Sword, WowWeaponType.Mace, WowWeaponType.Axe]);

        public DateTime LastEclipseCheck { get; private set; }

        public bool LunarEclipse { get; set; }

        public override WowRole Role => WowRole.Dps;

        public bool SolarEclipse { get; set; }

        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 1, new(1, 1, 5) },
                { 3, new(1, 3, 1) },
                { 4, new(1, 4, 2) },
                { 5, new(1, 5, 2) },
                { 7, new(1, 7, 3) },
                { 8, new(1, 8, 1) },
                { 9, new(1, 9, 2) },
                { 10, new(1, 10, 5) },
                { 11, new(1, 11, 3) },
                { 12, new(1, 12, 3) },
                { 13, new(1, 13, 1) },
                { 16, new(1, 16, 3) },
                { 17, new(1, 17, 2) },
                { 18, new(1, 18, 1) },
                { 19, new(1, 19, 3) },
                { 20, new(1, 20, 3) },
                { 22, new(1, 22, 5) },
                { 23, new(1, 23, 3) },
                { 25, new(1, 25, 1) },
                { 26, new(1, 26, 2) },
                { 27, new(1, 27, 3) },
                { 28, new(1, 28, 1) },
            },
            Tree2 = [],
            Tree3 = new()
            {
                { 1, new(3, 1, 2) },
                { 3, new(3, 3, 5) },
                { 6, new(3, 6, 3) },
                { 7, new(3, 7, 3) },
                { 8, new(3, 8, 1) },
                { 9, new(3, 9, 2) },
            },
        };

        public override bool UseAutoAttacks => false;

        public override string Version => "1.0";

        public override bool WalkBehindEnemy => false;

        public override WowClass WowClass => WowClass.Druid;

        public override WowVersion WowVersion => WowVersion.WotLK335a;

        public override void Execute()
        {
            base.Execute();

            CheckForEclipseProcs();

            if (TryFindTarget(TargetProviderDps, out _))
            {
                if (TryCastSpell(DruidWotlk.NaturesGrasp, 0))
                {
                    return;
                }

                double distance = Bot.Target.Position.GetDistance(Bot.Player.Position);

                if (distance < 12.0
                    && Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == DruidWotlk.EntanglingRoots)
                    && TryCastSpellDk(DruidWotlk.EntanglingRoots, Bot.Wow.TargetGuid, false, false, true))
                {
                    return;
                }

                if (NeedToHealMySelf())
                {
                    return;
                }

                if ((Bot.Player.ManaPercentage < 30
                        && TryCastSpell(DruidWotlk.Innervate, 0))
                    || (Bot.Player.HealthPercentage < 70
                        && TryCastSpell(DruidWotlk.Barkskin, 0, true))
                    || (LunarEclipse
                        && TryCastSpell(DruidWotlk.Starfire, Bot.Wow.TargetGuid, true))
                    || (SolarEclipse
                        && TryCastSpell(DruidWotlk.Wrath, Bot.Wow.TargetGuid, true))
                    || (Bot.Objects.All.OfType<IWowUnit>().Count(e => !e.IsInCombat && Bot.Player.Position.GetDistance(e.Position) < 35) < 4
                        && TryCastSpell(DruidWotlk.Starfall, Bot.Wow.TargetGuid, true)))
                {
                    return;
                }

                if (TryCastSpell(DruidWotlk.ForceOfNature, 0, true))
                {
                    Bot.Wow.ClickOnTerrain(Bot.Player.Position);
                }
            }
        }

        public override void OutOfCombatExecute()
        {
            base.OutOfCombatExecute();

            if (NeedToHealMySelf())
            {
                return;
            }
        }

        private bool CheckForEclipseProcs()
        {
            if (Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == DruidWotlk.EclipseLunar))
            {
                SolarEclipse = false;
                LunarEclipse = true;
            }
            else if (Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == DruidWotlk.EclipseSolar))
            {
                SolarEclipse = true;
                LunarEclipse = false;
            }

            LastEclipseCheck = DateTime.UtcNow;
            return false;
        }

        private bool NeedToHealMySelf()
        {
            return (Bot.Player.HealthPercentage < 60
                && !Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == DruidWotlk.Rejuvenation)
                && TryCastSpell(DruidWotlk.Rejuvenation, 0, true))
|| (Bot.Player.HealthPercentage < 40
                && TryCastSpell(DruidWotlk.HealingTouch, 0, true));
        }
    }
}
