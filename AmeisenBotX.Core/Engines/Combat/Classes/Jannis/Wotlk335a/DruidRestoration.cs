using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Engines.Combat.Helpers.Healing;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Spells.Objects;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.WowWotlk.Constants.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Jannis.Wotlk335a
{
    /// <summary>
    /// Optimized Restoration Druid for WotLK 3.3.5.
    /// Features: Intelligent HoT stacking (Rejuv > Regrowth > Lifebloom), Wild Growth for AoE,
    /// Swiftmend for emergency, Nourish for direct heals with HoT bonus.
    /// </summary>
    [CombatClassMetadata("[WotLK335a] Druid Restoration", "Jannis")]
    public class DruidRestoration : BasicCombatClass
    {
        public DruidRestoration(AmeisenBotInterfaces bot) : base(bot)
        {
            // ===== CONFIGURABLES =====
            Configurables.TryAdd("WildGrowthMinTargets", 3);
            Configurables.TryAdd("TreeFormInGroup", true);
            Configurables.TryAdd("LifebloomStackTarget", "Tank");
            Configurables.TryAdd("SwiftmendThreshold", 50.0);
            Configurables.TryAdd("TranquilityThreshold", 40.0);

            // ===== SELF BUFFS =====
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, DruidWotlk.TreeOfLife, () =>
                Bot.Objects.PartymemberGuids.Any() && Configurables["TreeFormInGroup"]
                && TryCastSpell(DruidWotlk.TreeOfLife, Bot.Wow.PlayerGuid, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, DruidWotlk.MarkOfTheWild, () => TryCastSpell(DruidWotlk.MarkOfTheWild, Bot.Wow.PlayerGuid, true)));

            // ===== GROUP BUFFS =====
            GroupAuraManager.SpellsToKeepActiveOnParty.Add((DruidWotlk.MarkOfTheWild, (spellName, guid) => TryCastSpell(spellName, guid, true)));

            // ===== HEALING MANAGER =====
            HealingManager = new(bot, (string spellName, ulong guid) => { return TryCastSpell(spellName, guid); });

            Bot.Character.SpellBook.OnSpellBookUpdate += () =>
            {
                if (Bot.Character.SpellBook.TryGetSpellByName(DruidWotlk.Nourish, out Spell spellNourish))
                {
                    HealingManager.AddSpell(spellNourish);
                }

                if (Bot.Character.SpellBook.TryGetSpellByName(DruidWotlk.HealingTouch, out Spell spellHealingTouch))
                {
                    HealingManager.AddSpell(spellHealingTouch);
                }

                if (Bot.Character.SpellBook.TryGetSpellByName(DruidWotlk.Regrowth, out Spell spellRegrowth))
                {
                    HealingManager.AddSpell(spellRegrowth);
                }
            };

            SpellAbortFunctions.Add(HealingManager.ShouldAbortCasting);

            SwiftmendEvent = new(TimeSpan.FromSeconds(15));
            WildGrowthEvent = new(TimeSpan.FromSeconds(6));
            LifebloomRefreshEvent = new(TimeSpan.FromSeconds(8));
        }

        public override string Description => "Optimized Restoration Druid with intelligent HoT stacking, Wild Growth for AoE, and Swiftmend emergency heals.";

        public override string DisplayName2 => "Druid Restoration";

        public override WowSpecialization Specialization => WowSpecialization.DruidRestoration;

        public override bool HandlesMovement => false;

        public override bool IsMelee => false;

        public override IItemComparator ItemComparator { get; set; } = new BasicComparator
        (
            [WowArmorType.Shield],
            [WowWeaponType.Sword, WowWeaponType.Mace, WowWeaponType.Axe],
            new Dictionary<string, double>()
            {
                { "ITEM_MOD_CRIT_RATING_SHORT", 1.2 },
                { "ITEM_MOD_INTELLECT_SHORT", 1.0 },
                { "ITEM_MOD_SPELL_POWER_SHORT", 1.6 },
                { "ITEM_MOD_HASTE_RATING_SHORT", 1.8 },
                { "ITEM_MOD_SPIRIT_SHORT ", 1.4 },
                { "ITEM_MOD_POWER_REGEN0_SHORT", 1.4 },
            }
        );

        public override WowRole Role => WowRole.Heal;

        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 2, new(1, 2, 5) }, // Genesis
                { 3, new(1, 3, 3) }, // Moonglow
                { 4, new(1, 4, 2) }, // Natures Majesty
                { 8, new(1, 8, 1) }, // Natures Grace
            },
            Tree2 = [],
            Tree3 = new()
            {
                { 1, new(3, 1, 2) },  // Improved Mark of the Wild
                { 2, new(3, 2, 3) },  // Natures Focus
                { 5, new(3, 5, 3) },  // Subtlety
                { 6, new(3, 6, 3) },  // Natural Shapeshifter
                { 7, new(3, 7, 3) },  // Intensity
                { 8, new(3, 8, 1) },  // Omen of Clarity
                { 9, new(3, 9, 2) },  // Master Shapeshifter
                { 11, new(3, 11, 3) }, // Tranquil Spirit
                { 12, new(3, 12, 1) }, // Improved Rejuvenation
                { 13, new(3, 13, 5) }, // Natures Swiftness
                { 14, new(3, 14, 2) }, // Gift of Nature
                { 16, new(3, 16, 5) }, // Empowered Touch
                { 17, new(3, 17, 3) }, // Improved Regrowth
                { 18, new(3, 18, 1) }, // Living Spirit
                { 20, new(3, 20, 5) }, // Swiftmend
                { 21, new(3, 21, 3) }, // Natural Perfection
                { 22, new(3, 22, 3) }, // Empowered Rejuvenation
                { 23, new(3, 23, 1) }, // Living Seed
                { 24, new(3, 24, 3) }, // Revitalize
                { 25, new(3, 25, 2) }, // Tree of Life
                { 26, new(3, 26, 5) }, // Improved Tree of Life
                { 27, new(3, 27, 1) }, // Wild Growth
            },
        };

        public override bool UseAutoAttacks => false;

        public override string Version => "2.0";

        public override bool WalkBehindEnemy => false;

        public override WowClass WowClass => WowClass.Druid;

        public override WowVersion WowVersion => WowVersion.WotLK335a;

        private HealingManager HealingManager { get; }
        private TimegatedEvent SwiftmendEvent { get; }
        private TimegatedEvent WildGrowthEvent { get; }
        private TimegatedEvent LifebloomRefreshEvent { get; }

        public override void Execute()
        {
            base.Execute();

            // ===== SELF DEFENSE =====
            if (Bot.Player.HealthPercentage < 50.0
                && TryCastSpell(DruidWotlk.Barkskin, 0, true))
            {
                return;
            }

            // ===== MANA MANAGEMENT =====
            if (Bot.Player.ManaPercentage < 30.0
                && TryCastSpell(DruidWotlk.Innervate, 0, true))
            {
                return;
            }

            // ===== HEALING =====
            if (NeedToHealSomeone())
            {
                return;
            }

            // ===== DPS WHEN NOT HEALING =====
            if (!Bot.Objects.Partymembers.Any(e => !e.IsDead && e.HealthPercentage < 90))
            {
                if (TryFindTarget(TargetProviderDps, out _))
                {
                    // Moonfire for instant damage
                    if (!Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == DruidWotlk.Moonfire)
                        && TryCastSpell(DruidWotlk.Moonfire, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }

                    // Wrath as filler
                    if (TryCastSpell(DruidWotlk.Wrath, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }
                }
            }
        }

        public override void OutOfCombatExecute()
        {
            base.OutOfCombatExecute();

            if (NeedToHealSomeone()
                || HandleDeadPartymembers(DruidWotlk.Revive))
            {
                return;
            }
        }

        public override void Load(Dictionary<string, JsonElement> objects)
        {
            base.Load(objects);

            if (objects.TryGetValue("HealingManager", out JsonElement elementHealingManager))
            {
                HealingManager.Load(elementHealingManager.To<Dictionary<string, JsonElement>>());
            }
        }

        public override Dictionary<string, object> Save()
        {
            Dictionary<string, object> s = base.Save();
            s.Add("HealingManager", HealingManager.Save());
            return s;
        }

        /// <summary>
        /// Check if target has any HoT active (for Nourish bonus).
        /// </summary>
        private bool TargetHasHoT(IWowUnit target)
        {
            return target.Auras.Any(e =>
                Bot.Db.GetSpellName(e.SpellId) is DruidWotlk.Rejuvenation or
                DruidWotlk.Regrowth or
                DruidWotlk.Lifebloom or
                DruidWotlk.WildGrowth);
        }

        private bool NeedToHealSomeone()
        {
            if (TargetProviderHeal.Get(out IEnumerable<IWowUnit> unitsToHeal))
            {
                int damagedCount = unitsToHeal.Count(e => e.HealthPercentage < 90.0);
                int criticalCount = unitsToHeal.Count(e => e.HealthPercentage < (double)Configurables["TranquilityThreshold"]);
                IWowUnit target = unitsToHeal.FirstOrDefault();

                if (target == null)
                {
                    return false;
                }

                // ===== TRANQUILITY for raid-wide emergency =====
                if (criticalCount > 3
                    && TryCastSpell(DruidWotlk.Tranquility, 0, true))
                {
                    return true;
                }

                // ===== WILD GROWTH for group healing =====
                if (damagedCount >= (int)Configurables["WildGrowthMinTargets"]
                    && WildGrowthEvent.Ready
                    && TryCastSpell(DruidWotlk.WildGrowth, target.Guid, true))
                {
                    WildGrowthEvent.Run();
                    return true;
                }

                // ===== NATURE'S SWIFTNESS + HEALING TOUCH for emergency =====
                if (target.HealthPercentage < 25.0
                    && TryCastSpell(DruidWotlk.NaturesSwiftness, 0, true))
                {
                    TryCastSpell(DruidWotlk.HealingTouch, target.Guid, true);
                    return true;
                }

                // ===== SWIFTMEND for emergency (requires Rejuv or Regrowth) =====
                if (target.HealthPercentage < Configurables["SwiftmendThreshold"]
                    && (target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == DruidWotlk.Regrowth)
                        || target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == DruidWotlk.Rejuvenation))
                    && SwiftmendEvent.Ready
                    && TryCastSpell(DruidWotlk.Swiftmend, target.Guid, true))
                {
                    SwiftmendEvent.Run();
                    return true;
                }

                // ===== LIFEBLOOM (3-stack rolling on tank) =====
                IWowUnit tankTarget = unitsToHeal.OrderBy(e => e.HealthPercentage).FirstOrDefault();
                if (tankTarget != null
                    && tankTarget.HealthPercentage < 98.0
                    && LifebloomRefreshEvent.Ready
                    && TryCastSpell(DruidWotlk.Lifebloom, tankTarget.Guid, true))
                {
                    LifebloomRefreshEvent.Run();
                    return true;
                }

                // ===== REJUVENATION (primary HoT) =====
                if (target.HealthPercentage < 95.0
                    && !target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == DruidWotlk.Rejuvenation)
                    && TryCastSpell(DruidWotlk.Rejuvenation, target.Guid, true))
                {
                    return true;
                }

                // ===== REGROWTH (secondary HoT + direct heal) =====
                if (target.HealthPercentage < 70.0
                    && !target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == DruidWotlk.Regrowth)
                    && TryCastSpell(DruidWotlk.Regrowth, target.Guid, true))
                {
                    return true;
                }

                // ===== NOURISH (best when target has HoTs) =====
                if (target.HealthPercentage < 60.0
                    && TargetHasHoT(target)
                    && TryCastSpell(DruidWotlk.Nourish, target.Guid, true))
                {
                    return true;
                }

                // ===== Use HealingManager for intelligent spell selection =====
                if (HealingManager.Tick())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
