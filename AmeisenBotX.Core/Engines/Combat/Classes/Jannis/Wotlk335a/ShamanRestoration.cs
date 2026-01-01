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
    /// Optimized Restoration Shaman for WotLK 3.3.5.
    /// Features: Chain Heal prioritization, Earth Shield on tank, Riptide pre-hotting,
    /// Nature's Swiftness emergency heals, and intelligent totem management.
    /// </summary>
    [CombatClassMetadata("[WotLK335a] Shaman Restoration", "Jannis")]
    public class ShamanRestoration : BasicCombatClass
    {
        public ShamanRestoration(AmeisenBotInterfaces bot) : base(bot)
        {
            // ===== CONFIGURABLES =====
            Configurables.TryAdd("EarthShieldTarget", "Tank"); // Tank, LowestHealth, or Self
            Configurables.TryAdd("ChainHealMinTargets", 3);
            Configurables.TryAdd("RiptideThreshold", 85.0);
            Configurables.TryAdd("EmergencyHealThreshold", 25.0);
            Configurables.TryAdd("ManaTideThreshold", 40.0);

            // ===== SELF BUFFS =====
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, ShamanWotlk.WaterShield, () => TryCastSpell(ShamanWotlk.WaterShield, 0, true)));

            // ===== HEALING MANAGER =====
            HealingManager = new(bot, (string spellName, ulong guid) => { return TryCastSpell(spellName, guid); });

            Bot.Character.SpellBook.OnSpellBookUpdate += () =>
            {
                if (Bot.Character.SpellBook.TryGetSpellByName(ShamanWotlk.HealingWave, out Spell spellHealingWave))
                {
                    HealingManager.AddSpell(spellHealingWave);
                }

                if (Bot.Character.SpellBook.TryGetSpellByName(ShamanWotlk.LesserHealingWave, out Spell spellLesserHealingWave))
                {
                    HealingManager.AddSpell(spellLesserHealingWave);
                }

                if (Bot.Character.SpellBook.TryGetSpellByName(ShamanWotlk.ChainHeal, out Spell spellChainHeal))
                {
                    HealingManager.AddSpell(spellChainHeal);
                }
            };

            SpellAbortFunctions.Add(HealingManager.ShouldAbortCasting);

            EarthShieldEvent = new(TimeSpan.FromSeconds(30));
            RiptideEvent = new(TimeSpan.FromSeconds(6));
        }

        public override string Description => "Optimized Restoration Shaman with Chain Heal prioritization, Earth Shield management, and emergency healing.";

        public override string DisplayName2 => "Shaman Restoration";

        public override WowSpecialization Specialization => WowSpecialization.ShamanRestoration;

        public override bool HandlesMovement => false;

        public override bool IsMelee => false;

        public override IItemComparator ItemComparator { get; set; } = new BasicComparator
        (
            [WowArmorType.Shield],
            [WowWeaponType.SwordTwoHand, WowWeaponType.MaceTwoHand, WowWeaponType.AxeTwoHand],
            new Dictionary<string, double>()
            {
                { "ITEM_MOD_CRIT_RATING_SHORT", 0.9 },
                { "ITEM_MOD_INTELLECT_SHORT", 0.5 },
                { "ITEM_MOD_SPELL_POWER_SHORT", 1.0 },
                { "ITEM_MOD_HASTE_RATING_SHORT", 1.2 },
                { "ITEM_MOD_POWER_REGEN0_SHORT", 0.8 },
            }
        );

        public override WowRole Role => WowRole.Heal;

        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 2, new(1, 2, 5) }, // Tidal Force
            },
            Tree2 = new()
            {
                { 3, new(2, 3, 5) }, // Improved Healing Wave
                { 5, new(2, 5, 5) }, // Tidal Mastery
                { 7, new(2, 7, 3) }, // Healing Focus
                { 8, new(2, 8, 1) }, // Healing Stream Totem
            },
            Tree3 = new()
            {
                { 1, new(3, 1, 5) },  // Improved Water Shield
                { 5, new(3, 5, 5) },  // Tidal Focus
                { 6, new(3, 6, 3) },  // Improved Reincarnation
                { 7, new(3, 7, 3) },  // Healing Way
                { 8, new(3, 8, 1) },  // Nature's Swiftness
                { 9, new(3, 9, 3) },  // Focused Mind
                { 10, new(3, 10, 3) }, // Purification
                { 11, new(3, 11, 5) }, // Mana Tide Totem
                { 12, new(3, 12, 3) }, // Cleanse Spirit
                { 13, new(3, 13, 1) }, // Blessing of the Eternals
                { 15, new(3, 15, 5) }, // Improved Chain Heal
                { 17, new(3, 17, 1) }, // Earth Shield
                { 19, new(3, 19, 2) }, // Improved Earth Shield
                { 20, new(3, 20, 2) }, // Nature's Blessing
                { 21, new(3, 21, 3) }, // Ancestral Awakening
                { 22, new(3, 22, 3) }, // Tidal Waves
                { 23, new(3, 23, 1) }, // Riptide
                { 24, new(3, 24, 2) }, // Improved Riptide
                { 25, new(3, 25, 5) }, // Tidal Mastery
                { 26, new(3, 26, 1) }, // Earthliving
            },
        };

        public override bool UseAutoAttacks => false;

        public override string Version => "2.0";

        public override bool WalkBehindEnemy => false;

        public override WowClass WowClass => WowClass.Shaman;

        public override WowVersion WowVersion => WowVersion.WotLK335a;

        private HealingManager HealingManager { get; }
        private TimegatedEvent EarthShieldEvent { get; }
        private TimegatedEvent RiptideEvent { get; }

        public override void Execute()
        {
            base.Execute();

            // ===== MANA MANAGEMENT =====
            // Mana Tide Totem when mana is low
            if (Bot.Player.ManaPercentage < Configurables["ManaTideThreshold"]
                && TryCastSpell(ShamanWotlk.TidalForce, 0, true))
            {
                return;
            }

            // ===== EMERGENCY COOLDOWNS =====
            if (ExecuteEmergencyCooldowns())
            {
                return;
            }

            // ===== EARTH SHIELD MANAGEMENT =====
            if (EarthShieldEvent.Ready && ManageEarthShield())
            {
                EarthShieldEvent.Run();
                return;
            }

            // ===== HEALING =====
            if (NeedToHealSomeone())
            {
                return;
            }

            // ===== DPS WHEN NOT HEALING =====
            if (TryFindTarget(TargetProviderDps, out _))
            {
                // Flame Shock for DoT
                if (!Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == ShamanWotlk.FlameShock)
                    && TryCastSpell(ShamanWotlk.FlameShock, Bot.Wow.TargetGuid, true))
                {
                    return;
                }

                // Lightning Bolt as filler
                if (TryCastSpell(ShamanWotlk.LightningBolt, Bot.Wow.TargetGuid, true))
                {
                    return;
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

            if (CheckForWeaponEnchantment(WowEquipmentSlot.INVSLOT_MAINHAND, ShamanWotlk.EarthlivingBuff, ShamanWotlk.EarthlivingWeapon))
            {
                return;
            }

            // Pre-shield tank out of combat
            if (ManageEarthShield())
            {
                return;
            }

            if (NeedToHealSomeone())
            {
                return;
            }
        }

        /// <summary>
        /// Emergency cooldowns for critical healing situations.
        /// </summary>
        private bool ExecuteEmergencyCooldowns()
        {
            IWowUnit lowestHealthTarget = Bot.Objects.Partymembers
                .Where(e => e != null && !e.IsDead)
                .OrderBy(e => e.HealthPercentage)
                .FirstOrDefault();

            if (lowestHealthTarget != null && lowestHealthTarget.HealthPercentage < Configurables["EmergencyHealThreshold"])
            {
                // Nature's Swiftness + Healing Wave for instant big heal
                if (TryCastSpell(ShamanWotlk.NaturesSwiftness, 0, true))
                {
                    TryCastSpell(ShamanWotlk.HealingWave, lowestHealthTarget.Guid, true);
                    return true;
                }

                // Tidal Force for crit bonus
                if (TryCastSpell(ShamanWotlk.TidalForce, 0, true))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Keep Earth Shield on tank or lowest health target.
        /// </summary>
        private bool ManageEarthShield()
        {
            // Find tank or lowest health target
            IWowUnit earthShieldTarget = Bot.Objects.Partymembers
                .Where(e => e != null && !e.IsDead)
                .OrderBy(e => e.HealthPercentage)
                .FirstOrDefault();

            return earthShieldTarget != null
                && !earthShieldTarget.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == ShamanWotlk.EarthShield)
                && TryCastSpell(ShamanWotlk.EarthShield, earthShieldTarget.Guid, true);
        }

        private bool NeedToHealSomeone()
        {
            if (TargetProviderHeal.Get(out IEnumerable<IWowUnit> unitsToHeal))
            {
                IWowUnit target = unitsToHeal.FirstOrDefault();

                if (target == null)
                {
                    return false;
                }

                int damagedCount = unitsToHeal.Count(e => e.HealthPercentage < 90.0);

                // ===== CHAIN HEAL for multiple damaged targets =====
                if (damagedCount >= (int)Configurables["ChainHealMinTargets"]
                    && TryCastSpell(ShamanWotlk.ChainHeal, target.Guid, true))
                {
                    return true;
                }

                // ===== RIPTIDE for pre-hotting (Tidal Waves proc) =====
                if (target.HealthPercentage < Configurables["RiptideThreshold"]
                    && RiptideEvent.Ready
                    && !target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == ShamanWotlk.Riptide)
                    && TryCastSpell(ShamanWotlk.Riptide, target.Guid, true))
                {
                    RiptideEvent.Run();
                    return true;
                }

                // ===== HEALING WAVE for single target big heals =====
                if (target.HealthPercentage < 50.0
                    && TryCastSpell(ShamanWotlk.HealingWave, target.Guid, true))
                {
                    return true;
                }

                // ===== LESSER HEALING WAVE for quick single target =====
                if (target.HealthPercentage < 75.0
                    && TryCastSpell(ShamanWotlk.LesserHealingWave, target.Guid, true))
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
    }
}
