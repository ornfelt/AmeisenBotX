using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Engines.Combat.Helpers.Healing;
using AmeisenBotX.Core.Engines.Movement.Enums;
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
    /// Optimized Holy Paladin for WotLK 3.3.5.
    /// Features: Intelligent healing, Sacred Shield, Hand of Sacrifice, Beacon management, and mana conservation.
    /// </summary>
    /// </summary>
    [CombatClassMetadata("[WotLK335a] Paladin Holy", "Jannis")]
    public class PaladinHoly : BasicCombatClass
    {
        public PaladinHoly(AmeisenBotInterfaces bot) : base(bot)
        {
            // ===== CONFIGURABLES =====
            Configurables.TryAdd("AttackInGroups", true);
            Configurables.TryAdd("AttackInGroupsUntilManaPercent", 85.0);
            Configurables.TryAdd("AttackInGroupsCloseCombat", false);
            Configurables.TryAdd("BeaconOfLightSelfHealth", 85.0);
            Configurables.TryAdd("BeaconOfLightPartyHealth", 85.0);
            Configurables.TryAdd("DivinePleaMana", 60.0);
            Configurables.TryAdd("DivineIlluminationManaAbove", 20.0);
            Configurables.TryAdd("DivineIlluminationManaUntil", 50.0);
            Configurables.TryAdd("SacredShieldHealth", 90.0);
            Configurables.TryAdd("HandOfSacrificeHealth", 30.0);
            Configurables.TryAdd("LayOnHandsHealth", 15.0);

            // ===== SELF BUFFS =====
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, PaladinWotlk.BlessingOfWisdom, () => TryCastSpell(PaladinWotlk.BlessingOfWisdom, Bot.Wow.PlayerGuid, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, PaladinWotlk.ConcentrationAura, () =>
                Bot.Character.SpellBook.IsSpellKnown(PaladinWotlk.ConcentrationAura) && TryCastSpell(PaladinWotlk.ConcentrationAura, Bot.Wow.PlayerGuid, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, PaladinWotlk.DevotionAura, () =>
                !Bot.Character.SpellBook.IsSpellKnown(PaladinWotlk.ConcentrationAura) && TryCastSpell(PaladinWotlk.DevotionAura, Bot.Wow.PlayerGuid, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, PaladinWotlk.SealOfWisdom, () =>
                Bot.Character.SpellBook.IsSpellKnown(PaladinWotlk.SealOfWisdom) && TryCastSpell(PaladinWotlk.SealOfWisdom, Bot.Wow.PlayerGuid, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, PaladinWotlk.SealOfLight, () =>
                !Bot.Character.SpellBook.IsSpellKnown(PaladinWotlk.SealOfWisdom) &&
                Bot.Character.SpellBook.IsSpellKnown(PaladinWotlk.SealOfLight) &&
                TryCastSpell(PaladinWotlk.SealOfLight, Bot.Wow.PlayerGuid, true)));

            // ===== GROUP BUFFS =====
            GroupAuraManager.SpellsToKeepActiveOnParty.Add((PaladinWotlk.BlessingOfWisdom, (spellName, guid) => TryCastSpell(spellName, guid, true)));

            // ===== HEALING MANAGER =====
            HealingManager = new(bot, (string spellName, ulong guid) => { return TryCastSpell(spellName, guid); });

            // Register healing spells when spellbook updates
            Bot.Character.SpellBook.OnSpellBookUpdate += () =>
            {
                if (Bot.Character.SpellBook.TryGetSpellByName(PaladinWotlk.FlashOfLight, out Spell spellFlashOfLight))
                {
                    HealingManager.AddSpell(spellFlashOfLight);
                }

                if (Bot.Character.SpellBook.TryGetSpellByName(PaladinWotlk.HolyLight, out Spell spellHolyLight))
                {
                    HealingManager.AddSpell(spellHolyLight);
                }

                if (Bot.Character.SpellBook.TryGetSpellByName(PaladinWotlk.HolyShock, out Spell spellHolyShock))
                {
                    HealingManager.AddSpell(spellHolyShock);
                }

                if (Bot.Character.SpellBook.TryGetSpellByName(PaladinWotlk.LayOnHands, out Spell spellLayOnHands))
                {
                    HealingManager.AddSpell(spellLayOnHands);
                }
            };

            SpellAbortFunctions.Add(HealingManager.ShouldAbortCasting);

            ChangeBeaconEvent = new(TimeSpan.FromSeconds(1));
            SacredShieldEvent = new(TimeSpan.FromSeconds(2));
        }

        public override string Description => "Optimized Holy Paladin with Sacred Shield, Hand of Sacrifice, intelligent Beacon management, and mana conservation.";

        public override string DisplayName2 => "Paladin Holy";

        public override WowSpecialization Specialization => WowSpecialization.PaladinHoly;

        public override bool HandlesMovement => false;

        public override bool IsMelee => false;

        public override IItemComparator ItemComparator { get; set; } = new BasicComparator
        (
            null,
            [WowWeaponType.AxeTwoHand, WowWeaponType.MaceTwoHand, WowWeaponType.SwordTwoHand],
            new Dictionary<string, double>()
            {
                { "ITEM_MOD_CRIT_RATING_SHORT", 0.88 },
                { "ITEM_MOD_INTELLECT_SHORT", 0.2 },
                { "ITEM_MOD_SPELL_POWER_SHORT", 0.68 },
                { "ITEM_MOD_HASTE_RATING_SHORT", 0.71},
            }
        );

        public override WowRole Role => WowRole.Heal;

        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 1, new(1, 1, 5) },  // Spiritual Focus
                { 3, new(1, 3, 3) },  // Healing Light
                { 4, new(1, 4, 5) },  // Divine Intellect
                { 6, new(1, 6, 1) },  // Aura Mastery
                { 7, new(1, 7, 5) },  // Illumination
                { 8, new(1, 8, 1) },  // Improved Lay on Hands
                { 10, new(1, 10, 2) }, // Improved Concentration Aura
                { 13, new(1, 13, 1) }, // Divine Favor
                { 14, new(1, 14, 3) }, // Sanctified Light
                { 16, new(1, 16, 5) }, // Holy Power
                { 17, new(1, 17, 3) }, // Light's Grace
                { 18, new(1, 18, 1) }, // Holy Shock
                { 21, new(1, 21, 5) }, // Holy Guidance
                { 22, new(1, 22, 1) }, // Divine Illumination
                { 23, new(1, 23, 5) }, // Judgements of the Pure
                { 24, new(1, 24, 2) }, // Infusion of Light
                { 25, new(1, 25, 2) }, // Enlightened Judgements
                { 26, new(1, 26, 1) }, // Beacon of Light
            },
            Tree2 = new()
            {
                { 1, new(2, 1, 5) },  // Divine Strength
            },
            Tree3 = new()
            {
                { 2, new(3, 2, 5) },  // Toughness
                { 4, new(3, 4, 3) },  // Improved Righteous Fury
                { 5, new(3, 5, 2) },  // Guardian's Favor
                { 7, new(3, 7, 5) },  // Anticipation
            },
        };

        public override bool UseAutoAttacks => false;

        public override string Version => "2.0";

        public override bool WalkBehindEnemy => false;

        public override WowClass WowClass => WowClass.Paladin;

        public override WowVersion WowVersion => WowVersion.WotLK335a;

        private TimegatedEvent ChangeBeaconEvent { get; }
        private TimegatedEvent SacredShieldEvent { get; }
        private HealingManager HealingManager { get; }

        public override void Execute()
        {
            base.Execute();

            // ===== MANA MANAGEMENT =====
            if (Bot.Player.ManaPercentage < Configurables["DivineIlluminationManaUntil"]
               && Bot.Player.ManaPercentage > Configurables["DivineIlluminationManaAbove"]
               && TryCastSpell(PaladinWotlk.DivineIllumination, 0, true))
            {
                return;
            }

            if (Bot.Player.ManaPercentage < Configurables["DivinePleaMana"]
                && TryCastSpell(PaladinWotlk.DivinePlea, 0, true))
            {
                return;
            }

            // ===== EMERGENCY COOLDOWNS =====
            if (ExecuteEmergencyCooldowns())
            {
                return;
            }

            // ===== BEACON OF LIGHT MANAGEMENT =====
            if (ChangeBeaconEvent.Run())
            {
                if (ManageBeaconOfLight())
                {
                    return;
                }
            }

            // ===== SACRED SHIELD MANAGEMENT =====
            if (SacredShieldEvent.Run())
            {
                if (ManageSacredShield())
                {
                    return;
                }
            }

            // ===== HEALING =====
            if (NeedToHealSomeone())
            {
                return;
            }
            else
            {
                // ===== DPS WHEN NOT HEALING =====
                bool isAlone = !Bot.Objects.Partymembers.Any(e => e.Guid != Bot.Player.Guid);

                if ((isAlone || (Configurables["AttackInGroups"] && Configurables["AttackInGroupsUntilManaPercent"] < Bot.Player.ManaPercentage))
                    && TryFindTarget(TargetProviderDps, out _))
                {
                    // Use Judgement for mana return and damage
                    if ((Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == PaladinWotlk.SealOfWisdom)
                        || Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == PaladinWotlk.SealOfLight))
                        && TryCastSpell(PaladinWotlk.JudgementOfLight, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }

                    // Holy Shock for DPS (instant)
                    if (TryCastSpell(PaladinWotlk.HolyShock, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }

                    // Exorcism (Art of War proc makes it instant)
                    if (TryCastSpell(PaladinWotlk.Exorcism, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }

                    // Hammer of Wrath on low health targets
                    if (Bot.Target != null && Bot.Target.HealthPercentage < 20
                        && TryCastSpell(PaladinWotlk.HammerOfWrath, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }

                    // Go melee if alone or allowed
                    if (isAlone || Configurables["AttackInGroupsCloseCombat"])
                    {
                        if (!Bot.Player.IsAutoAttacking
                            && Bot.Player.IsInMeleeRange(Bot.Target)
                            && EventAutoAttack.Run())
                        {
                            Bot.Wow.StartAutoAttack();
                            return;
                        }
                        else if (!Bot.Player.IsInMeleeRange(Bot.Target))
                        {
                            Bot.Movement.SetMovementAction(MovementAction.Move, Bot.Target.Position);
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Emergency cooldowns for critical situations.
        /// </summary>
        private bool ExecuteEmergencyCooldowns()
        {
            // Lay on Hands for near-death situations
            IWowUnit lowestHealthTarget = Bot.Objects.Partymembers
                .Where(e => e != null && !e.IsDead)
                .OrderBy(e => e.HealthPercentage)
                .FirstOrDefault();

            if (lowestHealthTarget != null && lowestHealthTarget.HealthPercentage < Configurables["LayOnHandsHealth"])
            {
                if (TryCastSpell(PaladinWotlk.LayOnHands, lowestHealthTarget.Guid, true))
                {
                    return true;
                }
            }

            // Hand of Sacrifice on tank taking heavy damage
            IWowUnit tank = Bot.Objects.Partymembers
                .FirstOrDefault(e => e != null && !e.IsDead && e.HealthPercentage < Configurables["HandOfSacrificeHealth"]);

            if (tank != null && tank.Guid != Bot.Player.Guid
                && Bot.Player.HealthPercentage > 70
                && !tank.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == PaladinWotlk.HandOfSacrifice)
                && TryCastSpell(PaladinWotlk.HandOfSacrifice, tank.Guid, true))
            {
                return true;
            }

            // Divine Shield for self if critically low
            if (Bot.Player.HealthPercentage < 20
                && TryCastSpell(PaladinWotlk.DivineShield, 0, true))
            {
                return true;
            }

            // Divine Favor before big heal
            return lowestHealthTarget != null && lowestHealthTarget.HealthPercentage < 40
                && TryCastSpell(PaladinWotlk.DivineFavor, 0, true);
        }

        /// <summary>
        /// Manages Beacon of Light placement.
        /// </summary>
        private bool ManageBeaconOfLight()
        {
            if (Bot.Player.HealthPercentage < Configurables["BeaconOfLightSelfHealth"])
            {
                // Keep Beacon on self when we're taking damage
                if (!Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == PaladinWotlk.BeaconOfLight)
                    && TryCastSpell(PaladinWotlk.BeaconOfLight, Bot.Player.Guid, true))
                {
                    ChangeBeaconEvent.Run();
                    return true;
                }
            }
            else
            {
                IEnumerable<IWowUnit> healableTargets = Bot.Wow.ObjectProvider.Partymembers
                    .Where(e => e != null && !e.IsDead)
                    .OrderBy(e => e.HealthPercentage);

                if (healableTargets.Count() > 1)
                {
                    // Put Beacon on second-lowest health target (we heal lowest directly)
                    IWowUnit t = healableTargets.Skip(1).FirstOrDefault(e => e.HealthPercentage < Configurables["BeaconOfLightPartyHealth"]);

                    if (t != null
                        && !t.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == PaladinWotlk.BeaconOfLight)
                        && TryCastSpell(PaladinWotlk.BeaconOfLight, t.Guid, true))
                    {
                        ChangeBeaconEvent.Run();
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Manages Sacred Shield - keep on tank or lowest health target.
        /// </summary>
        private bool ManageSacredShield()
        {
            IWowUnit shieldTarget = Bot.Objects.Partymembers
                .Where(e => e != null && !e.IsDead && e.HealthPercentage < Configurables["SacredShieldHealth"])
                .OrderBy(e => e.HealthPercentage)
                .FirstOrDefault();

            if (shieldTarget != null
                && !shieldTarget.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == PaladinWotlk.SacredShield)
                && TryCastSpell(PaladinWotlk.SacredShield, shieldTarget.Guid, true))
            {
                SacredShieldEvent.Run();
                return true;
            }

            return false;
        }

        private bool NeedToHealSomeone()
        {
            return HealingManager.Tick();
        }

        public override void OutOfCombatExecute()
        {
            base.OutOfCombatExecute();

            if (NeedToHealSomeone())
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
    }
}
