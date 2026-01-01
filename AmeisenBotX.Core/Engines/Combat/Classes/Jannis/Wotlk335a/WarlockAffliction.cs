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
    /// <summary>
    /// Optimized Affliction Warlock for WotLK 3.3.5.
    /// Features: Shadow Trance (Nightfall) proc usage, Dark Pact for mana, Soulshatter for aggro, 
    /// Health Funnel for pet, and optimized DoT management.
    /// </summary>
    [CombatClassMetadata("[WotLK335a] Warlock Affliction", "Jannis")]
    public class WarlockAffliction : BasicCombatClass
    {
        public WarlockAffliction(AmeisenBotInterfaces bot) : base(bot)
        {
            // ===== CONFIGURABLES =====
            Configurables.TryAdd("LifeTapHealthThreshold", 50.0);
            Configurables.TryAdd("LifeTapManaThreshold", 70.0);
            Configurables.TryAdd("DarkPactManaThreshold", 60.0);
            Configurables.TryAdd("HealthFunnelPetHealth", 40.0);
            Configurables.TryAdd("DrainSoulHealthThreshold", 25.0);
            Configurables.TryAdd("SoulshatterThreatThreshold", 80.0);
            Configurables.TryAdd("FearPvPEnabled", true);
            Configurables.TryAdd("MinSoulShards", 5);

            // ===== PET MANAGER =====
            PetManager = new PetManager
            (
                Bot,
                TimeSpan.FromSeconds(1),
                null,
                () => (Bot.Character.SpellBook.IsSpellKnown(WarlockWotlk.SummonFelhunter)
                       && Bot.Character.Inventory.HasItemByName("Soul Shard")
                       && TryCastSpell(WarlockWotlk.SummonFelhunter, 0))
                   || (Bot.Character.SpellBook.IsSpellKnown(WarlockWotlk.SummonImp)
                       && TryCastSpell(WarlockWotlk.SummonImp, 0)),
                () => (Bot.Character.SpellBook.IsSpellKnown(WarlockWotlk.SummonFelhunter)
                       && Bot.Character.Inventory.HasItemByName("Soul Shard")
                       && TryCastSpell(WarlockWotlk.SummonFelhunter, 0))
                   || (Bot.Character.SpellBook.IsSpellKnown(WarlockWotlk.SummonImp)
                       && TryCastSpell(WarlockWotlk.SummonImp, 0))
            );

            // ===== SELF BUFFS =====
            MyAuraManager.Jobs.Add(new KeepBestActiveAuraJob(bot.Db,
            [
                (WarlockWotlk.FelArmor, () => TryCastSpell(WarlockWotlk.FelArmor, 0, true)),
                (WarlockWotlk.DemonArmor, () => TryCastSpell(WarlockWotlk.DemonArmor, 0, true)),
                (WarlockWotlk.DemonSkin, () => TryCastSpell(WarlockWotlk.DemonSkin, 0, true)),
            ]));

            // ===== TARGET DEBUFFS (DoT Priority) =====
            // Haunt first (increases DoT damage)
            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, WarlockWotlk.Haunt, () =>
                Bot.Character.SpellBook.IsSpellKnown(WarlockWotlk.Haunt) && TryCastSpell(WarlockWotlk.Haunt, Bot.Wow.TargetGuid, true)));

            // Unstable Affliction (main damage)
            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, WarlockWotlk.UnstableAffliction, () =>
                TryCastSpell(WarlockWotlk.UnstableAffliction, Bot.Wow.TargetGuid, true)));

            // Corruption (instant, refreshed by Haunt talent)
            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, WarlockWotlk.Corruption, () =>
                Bot.Target != null && !Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == WarlockWotlk.SeedOfCorruption)
                && TryCastSpell(WarlockWotlk.Corruption, Bot.Wow.TargetGuid, true)));

            // Curse of Agony
            TargetAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, WarlockWotlk.CurseOfAgony, () =>
                TryCastSpell(WarlockWotlk.CurseOfAgony, Bot.Wow.TargetGuid, true)));
        }

        public override string Description => "Optimized Affliction Warlock with Shadow Trance procs, Dark Pact, Soulshatter, Health Funnel, and intelligent DoT management.";

        public override string DisplayName2 => "Warlock Affliction";

        public override WowSpecialization Specialization => WowSpecialization.WarlockAffliction;

        public override bool HandlesMovement => false;

        public override bool IsMelee => false;

        public override IItemComparator ItemComparator { get; set; } = new BasicIntellectComparator([WowArmorType.Shield]);

        public PetManager PetManager { get; private set; }

        public override WowRole Role => WowRole.Dps;

        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 1, new(1, 1, 2) },  // Improved Curse of Agony
                { 2, new(1, 2, 3) },  // Suppression
                { 3, new(1, 3, 5) },  // Improved Corruption
                { 7, new(1, 7, 2) },  // Improved Life Tap
                { 8, new(1, 8, 2) },  // Soul Siphon
                { 9, new(1, 9, 3) },  // Improved Fear
                { 12, new(1, 12, 2) }, // Nightfall (Shadow Trance proc)
                { 13, new(1, 13, 3) }, // Empowered Corruption
                { 14, new(1, 14, 5) }, // Shadow Embrace
                { 15, new(1, 15, 1) }, // Siphon Life
                { 17, new(1, 17, 2) }, // Improved Felhunter
                { 18, new(1, 18, 5) }, // Shadow Mastery
                { 19, new(1, 19, 3) }, // Eradication
                { 20, new(1, 20, 5) }, // Contagion
                { 21, new(1, 21, 1) }, // Dark Pact
                { 23, new(1, 23, 3) }, // Malediction
                { 24, new(1, 24, 3) }, // Death's Embrace
                { 25, new(1, 25, 1) }, // Unstable Affliction
                { 26, new(1, 26, 1) }, // Pandemic
                { 27, new(1, 27, 5) }, // Everlasting Affliction
                { 28, new(1, 28, 1) }, // Haunt
            },
            Tree2 = [],
            Tree3 = new()
            {
                { 1, new(3, 1, 5) },  // Improved Shadow Bolt
                { 2, new(3, 2, 5) },  // Bane
                { 8, new(3, 8, 5) },  // Ruin (increased crit damage)
                { 9, new(3, 9, 1) },  // Intensity
            },
        };

        public override bool UseAutoAttacks => false;

        public override string Version => "2.0";

        public override bool WalkBehindEnemy => false;

        public override WowClass WowClass => WowClass.Warlock;

        public override WowVersion WowVersion => WowVersion.WotLK335a;

        private DateTime LastFearAttempt { get; set; }

        /// <summary>
        /// Checks if Shadow Trance (Nightfall) proc is active for instant Shadow Bolt.
        /// </summary>
        private bool HasShadowTrance()
        {
            return Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) is WarlockWotlk.ShadowTrance
                or "Nightfall");
        }

        /// <summary>
        /// Count current Soul Shards in inventory.
        /// </summary>
        private int GetSoulShardCount()
        {
            return Bot.Character.Inventory.Items.Count(e => e.Name.Equals("Soul Shard", StringComparison.OrdinalIgnoreCase));
        }

        public override void Execute()
        {
            base.Execute();

            if (TryFindTarget(TargetProviderDps, out _))
            {
                // ===== PET MANAGEMENT =====
                if (PetManager.Tick()) { return; }

                // ===== PET HEALING =====
                if (Bot.Pet != null && Bot.Pet.HealthPercentage < Configurables["HealthFunnelPetHealth"]
                    && Bot.Player.HealthPercentage > 60
                    && TryCastSpell(WarlockWotlk.HealthFunnel, 0))
                {
                    return;
                }

                // ===== AGGRO MANAGEMENT =====
                // Soulshatter when threat is high
                if (Bot.Target != null && Bot.Target.TargetGuid == Bot.Wow.PlayerGuid
                    && Bot.Player.HealthPercentage < Configurables["SoulshatterThreatThreshold"]
                    && Bot.Character.Inventory.HasItemByName("Soul Shard")
                    && TryCastSpell(WarlockWotlk.Soulshatter, 0))
                {
                    return;
                }

                // ===== MANA MANAGEMENT =====
                // Dark Pact - steal mana from pet (preferred, no health cost)
                if (Bot.Player.ManaPercentage < Configurables["DarkPactManaThreshold"]
                    && Bot.Pet != null && Bot.Pet.ManaPercentage > 30
                    && TryCastSpell(WarlockWotlk.DarkPact, 0))
                {
                    return;
                }

                // Life Tap - convert health to mana
                if (Bot.Player.ManaPercentage < Configurables["LifeTapManaThreshold"]
                    && Bot.Player.HealthPercentage > Configurables["LifeTapHealthThreshold"]
                    && TryCastSpell(WarlockWotlk.LifeTap, 0))
                {
                    return;
                }

                // ===== SELF HEALING =====
                // Death Coil for emergency heal + damage
                if (Bot.Player.HealthPercentage < 40
                    && TryCastSpell(WarlockWotlk.DeathCoil, Bot.Wow.TargetGuid, true))
                {
                    return;
                }

                // Drain Life when low health
                if (Bot.Player.HealthPercentage < 35
                    && TryCastSpell(WarlockWotlk.DrainLife, Bot.Wow.TargetGuid, true))
                {
                    return;
                }

                if (Bot.Target != null)
                {
                    // ===== PVP FEAR =====
                    if (Configurables["FearPvPEnabled"] && Bot.Target.GetType() == typeof(IWowPlayer))
                    {
                        if (DateTime.UtcNow - LastFearAttempt > TimeSpan.FromSeconds(5)
                            && ((Bot.Player.Position.GetDistance(Bot.Target.Position) < 6.0f && TryCastSpell(WarlockWotlk.HowlOfTerror, 0, true))
                            || (Bot.Player.Position.GetDistance(Bot.Target.Position) < 12.0f && TryCastSpell(WarlockWotlk.Fear, Bot.Wow.TargetGuid, true))))
                        {
                            LastFearAttempt = DateTime.UtcNow;
                            return;
                        }
                    }

                    // ===== EXECUTE PHASE - Drain Soul =====
                    if (Bot.Target.HealthPercentage < Configurables["DrainSoulHealthThreshold"]
                        && GetSoulShardCount() < Configurables["MinSoulShards"]
                        && TryCastSpell(WarlockWotlk.DrainSoul, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }
                }

                // ===== SHADOW TRANCE PROC - Instant Shadow Bolt! =====
                // This is the highest priority damage ability when procced
                if (HasShadowTrance()
                    && TryCastSpell(WarlockWotlk.ShadowBolt, Bot.Wow.TargetGuid, true))
                {
                    return;
                }

                // ===== AOE =====
                int nearEnemies = Bot.GetNearEnemies<IWowUnit>(Bot.Target.Position, 16.0f).Count();
                if (nearEnemies > 2)
                {
                    // Seed of Corruption for AoE
                    if (!Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == WarlockWotlk.SeedOfCorruption)
                        && TryCastSpell(WarlockWotlk.SeedOfCorruption, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }

                    // Shadowflame for close-range AoE
                    if (Bot.Player.Position.GetDistance(Bot.Target.Position) < 8.0f
                        && TryCastSpell(WarlockWotlk.Shadowflame, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }

                    // Rain of Fire as last resort AoE
                    if (TryCastAoeSpell(WarlockWotlk.RainOfFire, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }
                }

                // ===== FILLER - Shadow Bolt =====
                if (TryCastSpell(WarlockWotlk.ShadowBolt, Bot.Wow.TargetGuid, true))
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

            // Pet healing out of combat
            if (Bot.Pet != null && Bot.Pet.HealthPercentage < 80
                && Bot.Player.HealthPercentage > 70
                && TryCastSpell(WarlockWotlk.HealthFunnel, 0))
            {
                return;
            }

            // Life Tap to restore mana out of combat (will regen health)
            if (Bot.Player.ManaPercentage < 80
                && Bot.Player.HealthPercentage > 80
                && TryCastSpell(WarlockWotlk.LifeTap, 0))
            {
                return;
            }
        }
    }
}
