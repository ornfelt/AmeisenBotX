using AmeisenBotX.Core.Engines.Combat.Helpers;
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
    [CombatClassMetadata("[WotLK335a] Warlock Demonology", "Jannis")]
    public class WarlockDemonology : BasicCombatClass
    {
        public WarlockDemonology(AmeisenBotInterfaces bot) : base(bot)
        {
            PetManager = new PetManager
            (
                Bot,
                TimeSpan.FromSeconds(1),
                null,
                () => (Bot.Character.SpellBook.IsSpellKnown(WarlockWotlk.SummonFelguard)
                       && Bot.Character.Inventory.HasItemByName("Soul Shard")
                       && TryCastSpell(WarlockWotlk.SummonFelguard, 0, true))
                   || (Bot.Character.SpellBook.IsSpellKnown(WarlockWotlk.SummonImp)
                       && TryCastSpell(WarlockWotlk.SummonImp, 0, true)),
                () => (Bot.Character.SpellBook.IsSpellKnown(WarlockWotlk.SummonFelguard)
                       && Bot.Character.Inventory.HasItemByName("Soul Shard")
                       && TryCastSpell(WarlockWotlk.SummonFelguard, 0, true))
                   || (Bot.Character.SpellBook.IsSpellKnown(WarlockWotlk.SummonImp)
                       && TryCastSpell(WarlockWotlk.SummonImp, 0, true))
            );

            MyAuraManager.Jobs.Add(new KeepBestActiveAuraJob(bot.Db,
            [
                (WarlockWotlk.FelArmor, () => TryCastSpell(WarlockWotlk.FelArmor, 0, true)),
                (WarlockWotlk.DemonArmor, () => TryCastSpell(WarlockWotlk.DemonArmor, 0, true)),
                (WarlockWotlk.DemonSkin, () => TryCastSpell(WarlockWotlk.DemonSkin, 0, true)),
            ]));

            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, WarlockWotlk.Corruption, () => Bot.Target != null && !Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == WarlockWotlk.SeedOfCorruption) && TryCastSpell(WarlockWotlk.Corruption, Bot.Wow.TargetGuid, true)));
            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, WarlockWotlk.CurseOfTongues, () => TryCastSpell(WarlockWotlk.CurseOfTongues, Bot.Wow.TargetGuid, true)));
            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, WarlockWotlk.Immolate, () => TryCastSpell(WarlockWotlk.Immolate, Bot.Wow.TargetGuid, true)));
        }

        public override string Description => "FCFS based CombatClass for the Demonology Warlock spec.";

        public override string DisplayName2 => "Warlock Demonology";

        public override bool HandlesMovement => false;

        public override bool IsMelee => false;

        public override IItemComparator ItemComparator { get; set; } = new BasicIntellectComparator([WowArmorType.Shield]);

        public PetManager PetManager { get; private set; }

        public override WowRole Role => WowRole.Dps;

        public override TalentTree Talents { get; } = new()
        {
            Tree1 = [],
            Tree2 = new()
            {
                { 3, new(2, 3, 3) },
                { 4, new(2, 4, 2) },
                { 6, new(2, 6, 3) },
                { 7, new(2, 7, 3) },
                { 9, new(2, 9, 1) },
                { 10, new(2, 10, 1) },
                { 11, new(2, 11, 3) },
                { 12, new(2, 12, 5) },
                { 13, new(2, 13, 2) },
                { 15, new(2, 15, 2) },
                { 16, new(2, 16, 5) },
                { 17, new(2, 17, 3) },
                { 19, new(2, 19, 1) },
                { 20, new(2, 20, 3) },
                { 21, new(2, 21, 5) },
                { 22, new(2, 22, 2) },
                { 23, new(2, 23, 3) },
                { 24, new(2, 24, 1) },
                { 25, new(2, 25, 3) },
                { 26, new(2, 26, 5) },
                { 27, new(2, 27, 1) },
            },
            Tree3 = new()
            {
                { 1, new(3, 1, 5) },
                { 2, new(3, 2, 5) },
                { 8, new(3, 8, 4) },
            },
        };

        public override bool UseAutoAttacks => false;

        public override string Version => "1.0";

        public override bool WalkBehindEnemy => false;

        public override WowClass WowClass => WowClass.Warlock;

        public override WowVersion WowVersion => WowVersion.WotLK335a;

        private DateTime LastFearAttempt { get; set; }

        public override void Execute()
        {
            base.Execute();

            if (TryFindTarget(TargetProviderDps, out _))
            {
                if (PetManager.Tick()) { return; }

                if ((Bot.Player.ManaPercentage < 75.0 && Bot.Player.HealthPercentage > 60.0 && TryCastSpell(WarlockWotlk.LifeTap, 0))
                    || (Bot.Player.HealthPercentage < 80.0 && TryCastSpell(WarlockWotlk.DeathCoil, Bot.Wow.TargetGuid, true))
                    || (Bot.Player.HealthPercentage < 50.0 && TryCastSpell(WarlockWotlk.DrainLife, Bot.Wow.TargetGuid, true))
                    || TryCastSpell(WarlockWotlk.Metamorphosis, 0)
                    || (Bot.Objects.Pet?.Health > 0 && TryCastSpell(WarlockWotlk.DemonicEmpowerment, 0)))
                {
                    return;
                }

                if (Bot.Target != null)
                {
                    if (Bot.Target.GetType() == typeof(IWowPlayer))
                    {
                        if (DateTime.UtcNow - LastFearAttempt > TimeSpan.FromSeconds(5)
                            && ((Bot.Player.Position.GetDistance(Bot.Target.Position) < 6.0f && TryCastSpell(WarlockWotlk.HowlOfTerror, 0, true))
                            || (Bot.Player.Position.GetDistance(Bot.Target.Position) < 12.0f && TryCastSpell(WarlockWotlk.Fear, Bot.Wow.TargetGuid, true))))
                        {
                            LastFearAttempt = DateTime.UtcNow;
                            return;
                        }
                    }

                    if (Bot.Character.Inventory.Items.Count(e => e.Name.Equals("Soul Shard", StringComparison.OrdinalIgnoreCase)) < 5.0
                        && Bot.Target.HealthPercentage < 25.0
                        && TryCastSpell(WarlockWotlk.DrainSoul, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }
                }

                if (Bot.GetNearEnemies<IWowUnit>(Bot.Target.Position, 16.0f).Count() > 2
                    && ((!Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == WarlockWotlk.SeedOfCorruption)
                        && TryCastSpell(WarlockWotlk.SeedOfCorruption, Bot.Wow.TargetGuid, true))
                    || TryCastAoeSpell(WarlockWotlk.RainOfFire, Bot.Wow.TargetGuid, true)))
                {
                    return;
                }

                bool hasDecimation = Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == WarlockWotlk.Decimation);
                bool hasMoltenCore = Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == WarlockWotlk.MoltenCore);

                if (hasDecimation && hasMoltenCore && TryCastSpell(WarlockWotlk.Soulfire, Bot.Wow.TargetGuid, true))
                {
                    return;
                }
                else if (hasDecimation && TryCastSpell(WarlockWotlk.Soulfire, Bot.Wow.TargetGuid, true))
                {
                    return;
                }
                else if (hasMoltenCore && TryCastSpell(WarlockWotlk.Incinerate, Bot.Wow.TargetGuid, true))
                {
                    return;
                }
                else if (TryCastSpell(WarlockWotlk.ShadowBolt, Bot.Wow.TargetGuid, true))
                {
                    return;
                }
            }
        }

        public override void OutOfCombatExecute()
        {
            base.OutOfCombatExecute();

            if (PetManager.Tick())
            {
                return;
            }
        }
    }
}
