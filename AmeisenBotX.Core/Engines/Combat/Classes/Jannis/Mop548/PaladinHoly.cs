using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Combat.Helpers.Healing;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Spells.Objects;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow548.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Jannis.Mop548
{
    public class PaladinHoly : BasicCombatClass
    {
        /// <summary>
        /// Constructor for the PaladinHoly class.
        /// Initializes configurable values and sets up event handlers for spell updates and beacon changes.
        /// </summary>
        public PaladinHoly(AmeisenBotInterfaces bot) : base(bot)
        {
            Configurables.TryAdd("AttackInGroups", true);
            Configurables.TryAdd("AttackInGroupsUntilManaPercent", 85.0);
            Configurables.TryAdd("AttackInGroupsCloseCombat", false);
            Configurables.TryAdd("BeaconOfLightSelfHealth", 85.0);
            Configurables.TryAdd("BeaconOfLightPartyHealth", 85.0);
            Configurables.TryAdd("DivinePleaMana", 60.0);

            GroupAuraManager.SpellsToKeepActiveOnParty.Add((Paladin548.BlessingOfKings, (spellName, guid) => TryCastSpell(spellName, guid, true)));

            HealingManager = new(bot, (string spellName, ulong guid) => { return TryCastSpell(spellName, guid); });

            // make sure all new spells get added to the healing manager
            Bot.Character.SpellBook.OnSpellBookUpdate += () =>
            {
                if (Bot.Character.SpellBook.TryGetSpellByName(Paladin548.FlashOfLight, out Spell spellFlashOfLight))
                {
                    HealingManager.AddSpell(spellFlashOfLight);
                }

                if (Bot.Character.SpellBook.TryGetSpellByName(Paladin548.HolyShock, out Spell spellHolyShock))
                {
                    HealingManager.AddSpell(spellHolyShock);
                }

                if (Bot.Character.SpellBook.TryGetSpellByName(Paladin548.HolyLight, out Spell spellHolyLight))
                {
                    HealingManager.AddSpell(spellHolyLight);
                }

                if (Bot.Character.SpellBook.TryGetSpellByName(Paladin548.DivineLight, out Spell spellDivineLight))
                {
                    HealingManager.AddSpell(spellDivineLight);
                }
            };

            InterruptManager.InterruptSpells = new()
            {
                { 0, (x) => TryCastSpell(Paladin548.FistOfJustice, x.Guid, true) },
                { 1, (x) => TryCastSpell(Paladin548.HammerOfJustice, x.Guid, true) },
                { 2, (x) => TryCastSpell(Paladin548.Rebuke, x.Guid, true) },
            };

            // SpellAbortFunctions.Add(HealingManager.ShouldAbortCasting);
            ChangeBeaconEvent = new(TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Gets the description of the Beta CombatClass for the Holy Paladin spec.
        /// </summary>
        public override string Description => "Beta CombatClass for the Holy Paladin spec.";

        /// <summary>
        /// Gets the display name for a Paladin Holy.
        /// </summary>
        public override string DisplayName2 => "Paladin Holy";

        /// <summary>
        /// Gets or sets a value indicating whether this object handles movement.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this object handles movement; otherwise, <c>false</c>.
        /// </value>
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets a value indicating whether this entity is not melee.
        /// </summary>
        public override bool IsMelee => false;

        /// <summary>
        /// Gets or sets the item comparator for comparing IItems.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicComparator
                (
                    new() { WowArmorType.Cloth, WowArmorType.Leather },
                    new() { WowWeaponType.AxeTwoHand, WowWeaponType.MaceTwoHand, WowWeaponType.SwordTwoHand },
                    new Dictionary<string, double>()
                    {
                { "ITEM_MOD_INTELLECT_SHORT", 1.0 },
                { "ITEM_MOD_SPELL_POWER_SHORT", 1.0 },
                { "ITEM_MOD_SPIRIT_SHORT", 0.75 },
                { "ITEM_MOD_HASTE_RATING_SHORT", 0.5},
                { "ITEM_MOD_MASTERY_RATING_SHORT", 0.25 },
                { "ITEM_MOD_CRIT_RATING_SHORT", 0.125 },
                    }
                );

        /// <summary>
        /// Gets or sets the role of the player as a healer in the game World of Warcraft.
        /// </summary>
        public override WowRole Role => WowRole.Heal;

        /// <summary>
        /// Gets or sets the talent tree.
        /// </summary>
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
            },
            Tree2 = new()
            {
            },
            Tree3 = new()
            {
            },
        };

        /// <summary>
        /// Indicates that auto attacks should not be used.
        /// </summary>
        public override bool UseAutoAttacks => false;

        /// This property returns the version number of the code as a string.
        public override string Version => "1.0";

        /// Determines whether the enemy does not have the ability to be walked behind.
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the WowClass property to the value WowClass.Paladin,
        /// indicating that the object represents a Paladin in the World of Warcraft.
        /// </summary>
        public override WowClass WowClass => WowClass.Paladin;

        /// <summary>
        /// Gets or sets the World of Warcraft version as MoP548.
        /// </summary>
        public override WowVersion WowVersion => WowVersion.MoP548;

        /// <summary>
        /// Gets or sets the TimegatedEvent for changing the beacon.
        /// </summary>
        private TimegatedEvent ChangeBeaconEvent { get; }

        /// Gets the private HealingManager property.
        private HealingManager HealingManager { get; }

        /// <summary>
        /// Executes the specified action sequence for a Paladin character.
        /// The sequence includes healing, protection, and combat actions.
        /// </summary>
        public override void Execute()
        {
            base.Execute();

            IEnumerable<IWowUnit> validPartymembers = Bot.Objects.Partymembers.Where(e => IWowUnit.IsValidAliveInCombat(e));

            IWowUnit dyingUnit = validPartymembers.FirstOrDefault(e => e.HealthPercentage < 14.0 && !e.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Paladin548.Forbearance));

            if (dyingUnit != null)
            {
                if (TryCastSpell(Paladin548.LayOnHands, dyingUnit.Guid, true))
                {
                    return;
                }

                if (TryCastSpell(Paladin548.HandOfProtection, dyingUnit.Guid, true))
                {
                    return;
                }
            }

            IWowUnit shieldWorthyUnit = validPartymembers.FirstOrDefault(e => e.HealthPercentage < 20.0 && !e.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Paladin548.Forbearance));

            if (shieldWorthyUnit != null && TryCastSpell(Paladin548.DivineShield, shieldWorthyUnit.Guid, true))
            {
                return;
            }

            if (HealingManager.Tick())
            {
                return;
            }
            else
            {
                if (!validPartymembers.Any(e => e.HealthPercentage < 85.0))
                {
                    IEnumerable<IWowUnit> lowHpUnits = validPartymembers.Where(e => e.HealthPercentage < 95.0);

                    if (lowHpUnits.Any() && TryCastSpell(Paladin548.DivineProtection, lowHpUnits.First().Guid, true))
                    {
                        return;
                    }
                }

                IWowUnit movementImpairedUnit = validPartymembers.FirstOrDefault(e => e.IsConfused || e.IsDazed);

                if (movementImpairedUnit != null && TryCastSpell(Paladin548.HandOfFreedom, movementImpairedUnit.Guid, true))
                {
                    return;
                }

                if (Bot.Player.ManaPercentage < Configurables["DivinePleaMana"]
                    && TryCastSpell(Paladin548.DivinePlea, 0, true))
                {
                    return;
                }

                if (ChangeBeaconEvent.Ready)
                {
                    if (Bot.Player.HealthPercentage < Configurables["BeaconOfLightSelfHealth"])
                    {
                        // keep beacon of light on us to reduce healing ourself
                        if (!Bot.Player.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Paladin548.BeaconOfLight)
                            && TryCastSpell(Paladin548.BeaconOfLight, Bot.Player.Guid, true))
                        {
                            ChangeBeaconEvent.Run();
                            return;
                        }
                    }
                    else
                    {
                        if (validPartymembers.Any())
                        {
                            IWowUnit t = validPartymembers.OrderBy(e => e.HealthPercentage)
                                .Skip(1)
                                .FirstOrDefault(e => e.HealthPercentage < Configurables["BeaconOfLightPartyHealth"]);

                            // keep beacon of light on second lowest target
                            if (t != null
                                && !t.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Paladin548.BeaconOfLight)
                                && TryCastSpell(Paladin548.BeaconOfLight, t.Guid, true))
                            {
                                ChangeBeaconEvent.Run();
                                return;
                            }
                        }
                    }
                }

                bool isAlone = !validPartymembers.Any(e => e.Guid != Bot.Player.Guid);

                if ((isAlone || (Configurables["AttackInGroups"] && Configurables["AttackInGroupsUntilManaPercent"] < Bot.Player.ManaPercentage))
                    && TryFindTarget(TargetProviderDps, out _))
                {
                    if (Bot.Player.HolyPower > 0
                        && Bot.Player.HealthPercentage < 85.0
                        && TryCastAoeSpell(Paladin548.WordOfGlory, Bot.Player.Guid))
                    {
                        return;
                    }

                    // either we are alone or allowed to go close combat in groups
                    if (isAlone || Configurables["AttackInGroupsCloseCombat"])
                    {
                        if (Bot.Player.IsInMeleeRange(Bot.Target))
                        {
                            if (EventAutoAttack.Run())
                            {
                                Bot.Wow.StartAutoAttack();
                            }

                            if (TryCastSpell(Paladin548.CrusaderStrike, Bot.Wow.TargetGuid, true))
                            {
                                return;
                            }
                        }
                        else
                        {
                            if (Bot.Target.HealthPercentage < 20.0
                                && TryCastSpell(Paladin548.HammerOfWrath, Bot.Wow.TargetGuid, true))
                            {
                                return;
                            }

                            if (TryCastSpell(Paladin548.Judgment, Bot.Wow.TargetGuid, true))
                            {
                                return;
                            }

                            if (!Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Paladin548.Denounce)
                                && TryCastSpell(Paladin548.Denounce, Bot.Wow.TargetGuid, true))
                            {
                                return;
                            }

                            Bot.Movement.SetMovementAction(MovementAction.Move, Bot.Target.Position);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads the objects from the given dictionary, including the HealingManager if it exists.
        /// </summary>
        public override void Load(Dictionary<string, JsonElement> objects)
        {
            base.Load(objects);

            if (objects.TryGetValue("HealingManager", out JsonElement elementHealingManager))
            {
                HealingManager.Load(elementHealingManager.To<Dictionary<string, JsonElement>>());
            }
        }

        /// <summary>
        /// Executes the OutOfCombatExecute method.
        /// </summary>
        public override void OutOfCombatExecute()
        {
            base.OutOfCombatExecute();

            if (HealingManager.Tick())
            {
                return;
            }
        }

        /// <summary>
        /// Saves the data of the HealingManager object and returns it as a dictionary.
        /// </summary>
        /// <returns>A dictionary containing the saved data.</returns>
        public override Dictionary<string, object> Save()
        {
            Dictionary<string, object> s = base.Save();
            s.Add("HealingManager", HealingManager.Save());
            return s;
        }
    }
}