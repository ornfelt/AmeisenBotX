using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects;
using AmeisenBotX.Core.Engines.Combat.Helpers.Healing;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Spells.Objects;
using AmeisenBotX.Core.Managers.Character.Talents.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow335a.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace AmeisenBotX.Core.Engines.Combat.Classes.Jannis.Wotlk335a
{
    public class DruidRestoration : BasicCombatClass
    {
        /// <summary>
        /// Initializes a new instance of the DruidRestoration class, with the specified bot.
        /// </summary>
        /// <param name="bot">The bot instance.</param>
        public DruidRestoration(AmeisenBotInterfaces bot) : base(bot)
        {
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Druid335a.TreeOfLife, () => Bot.Objects.PartymemberGuids.Any() && TryCastSpell(Druid335a.TreeOfLife, Bot.Wow.PlayerGuid, true)));
            MyAuraManager.Jobs.Add(new KeepActiveAuraJob(bot.Db, Druid335a.MarkOfTheWild, () => TryCastSpell(Druid335a.MarkOfTheWild, Bot.Wow.PlayerGuid, true)));

            GroupAuraManager.SpellsToKeepActiveOnParty.Add((Druid335a.MarkOfTheWild, (spellName, guid) => TryCastSpell(spellName, guid, true)));

            HealingManager = new(bot, (string spellName, ulong guid) => { return TryCastSpell(spellName, guid); });

            // make sure all new spells get added to the healing manager
            Bot.Character.SpellBook.OnSpellBookUpdate += () =>
            {
                if (Bot.Character.SpellBook.TryGetSpellByName(Druid335a.Nourish, out Spell spellNourish))
                {
                    HealingManager.AddSpell(spellNourish);
                }

                if (Bot.Character.SpellBook.TryGetSpellByName(Druid335a.HealingTouch, out Spell spellHealingTouch))
                {
                    HealingManager.AddSpell(spellHealingTouch);
                }

                if (Bot.Character.SpellBook.TryGetSpellByName(Druid335a.Regrowth, out Spell spellRegrowth))
                {
                    HealingManager.AddSpell(spellRegrowth);
                }
            };

            SpellAbortFunctions.Add(HealingManager.ShouldAbortCasting);

            SwiftmendEvent = new(TimeSpan.FromSeconds(15));
        }

        /// <summary>
        /// Overrides the Description property and returns the string "FCFS based CombatClass for the Druid Restoration spec."
        /// </summary>
        public override string Description => "FCFS based CombatClass for the Druid Restoration spec.";

        /// <summary>
        /// Gets or sets the display name for the Druid in Restoration Spec.
        /// </summary>
        public override string DisplayName2 => "Druid Restoration";

        /// <summary>
        /// Gets a value indicating whether this object handles movement.
        /// </summary>
        /// <value>
        ///   <c>false</c> indicating that this object does not handle movement.
        /// </value>
        public override bool HandlesMovement => false;

        /// <summary>
        /// Gets a value indicating whether the object is a melee weapon.
        /// </summary>
        public override bool IsMelee => false;

        /// <summary>
        /// Gets or sets the item comparator for comparing attributes of items.
        /// </summary>
        public override IItemComparator ItemComparator { get; set; } = new BasicComparator
                (
                    new() { WowArmorType.Shield },
                    new() { WowWeaponType.Sword, WowWeaponType.Mace, WowWeaponType.Axe },
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

        /// <summary>
        /// Gets or sets the role of the character as a healer in the World of Warcraft game.
        /// </summary>
        public override WowRole Role => WowRole.Heal;

        /// This code initializes the `Talents` property with a new `TalentTree` object. The `TalentTree` object contains three properties: `Tree1`, `Tree2`, and `Tree3`. 
        public override TalentTree Talents { get; } = new()
        {
            Tree1 = new()
            {
                { 2, new(1, 2, 5) },
                { 3, new(1, 3, 3) },
                { 4, new(1, 4, 2) },
                { 8, new(1, 8, 1) },
            },
            Tree2 = new(),
            Tree3 = new()
            {
                { 1, new(3, 1, 2) },
                { 2, new(3, 2, 3) },
                { 5, new(3, 5, 3) },
                { 6, new(3, 6, 3) },
                { 7, new(3, 7, 3) },
                { 8, new(3, 8, 1) },
                { 9, new(3, 9, 2) },
                { 11, new(3, 11, 3) },
                { 12, new(3, 12, 1) },
                { 13, new(3, 13, 5) },
                { 14, new(3, 14, 2) },
                { 16, new(3, 16, 5) },
                { 17, new(3, 17, 3) },
                { 18, new(3, 18, 1) },
                { 20, new(3, 20, 5) },
                { 21, new(3, 21, 3) },
                { 22, new(3, 22, 3) },
                { 23, new(3, 23, 1) },
                { 24, new(3, 24, 3) },
                { 25, new(3, 25, 2) },
                { 26, new(3, 26, 5) },
                { 27, new(3, 27, 1) },
            },
        };

        /// This property overrides the base class's "UseAutoAttacks" property and sets it to false.
        public override bool UseAutoAttacks => false;

        /// <summary>
        /// Gets the version number of the code.
        /// </summary>
        public override string Version => "1.1";

        /// Indicates that the enemy cannot be walked behind.
        public override bool WalkBehindEnemy => false;

        /// <summary>
        /// Gets or sets the WoW class, which is a Druid.
        /// </summary>
        public override WowClass WowClass => WowClass.Druid;

        /// <summary>
        /// Gets or sets the WoW version for the code, which is set to World of Warcraft: Wrath of the Lich King (3.3.5a).
        /// </summary>
        public override WowVersion WowVersion => WowVersion.WotLK335a;

        /// Gets the private instance of the HealingManager class.
        private HealingManager HealingManager { get; }

        /// Gets the TimegatedEvent for Swiftmend.
        private TimegatedEvent SwiftmendEvent { get; }

        /// This method executes a series of actions based on the current state of the game. It first checks if the player's mana percentage is below 30.0 and if the Innervate spell can be cast, it returns. If the player's health percentage is below 50.0 and the Barkskin spell can be cast, it returns. If there are party members present who are not dead, it checks if healing is needed and returns if true. If the player is solo and their health percentage is below 75.0, it checks if healing is needed and returns if true. If a target is found, it checks if the target does not have the Moonfire aura, and if so, it tries to cast the Moonfire spell on the target and returns. It then tries to cast the Starfire spell on the target and returns if successful. Finally, it tries to cast the Wrath spell on the target and returns if successful.
        public override void Execute()
        {
            base.Execute();

            if (Bot.Player.ManaPercentage < 30.0
                && TryCastSpell(Druid335a.Innervate, 0, true))
            {
                return;
            }

            if (Bot.Player.HealthPercentage < 50.0
                && TryCastSpell(Druid335a.Barkskin, 0, true))
            {
                return;
            }

            if (Bot.Objects.Partymembers.Any(e => !e.IsDead))
            {
                if (NeedToHealSomeone())
                {
                    return;
                }
            }
            else
            {
                // when we're solo, we don't need to heal as much as we would do in a dungeon group
                if (Bot.Player.HealthPercentage < 75.0 && NeedToHealSomeone())
                {
                    return;
                }

                if (TryFindTarget(TargetProviderDps, out _))
                {
                    if (!Bot.Target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Druid335a.Moonfire)
                        && TryCastSpell(Druid335a.Moonfire, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }

                    if (TryCastSpell(Druid335a.Starfire, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }

                    if (TryCastSpell(Druid335a.Wrath, Bot.Wow.TargetGuid, true))
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Loads the specified objects into the current instance.
        /// </summary>
        /// <param name="objects">The dictionary of objects to load.</param>
        public override void Load(Dictionary<string, JsonElement> objects)
        {
            base.Load(objects);

            if (objects.TryGetValue("HealingManager", out JsonElement elementHealingManager))
            {
                HealingManager.Load(elementHealingManager.To<Dictionary<string, JsonElement>>());
            }
        }

        /// Executes the OutOfCombatExecute() method.
        ///
        /// If there is a need to heal someone or handle dead party members,
        /// the method will return without further execution.
        public override void OutOfCombatExecute()
        {
            base.OutOfCombatExecute();

            if (NeedToHealSomeone()
                || HandleDeadPartymembers(Druid335a.Revive))
            {
                return;
            }
        }

        /// <summary>
        /// Saves the state of the HealingManager and returns a dictionary containing the saved data.
        /// </summary>
        /// <returns>A dictionary with the saved data.</returns>
        public override Dictionary<string, object> Save()
        {
            Dictionary<string, object> s = base.Save();
            s.Add("HealingManager", HealingManager.Save());
            return s;
        }

        ///<summary>Checks if there is a need to heal someone.</summary>
        private bool NeedToHealSomeone()
        {
            if (TargetProviderHeal.Get(out IEnumerable<IWowUnit> unitsToHeal))
            {
                if (unitsToHeal.Count(e => e.HealthPercentage < 40.0) > 3
                    && TryCastSpell(Druid335a.Tranquility, 0, true))
                {
                    return true;
                }

                IWowUnit target = unitsToHeal.First();

                if (target.HealthPercentage < 90.0
                    && target.HealthPercentage > 75.0
                    && unitsToHeal.Count(e => e.HealthPercentage < 90.0) > 1
                    && TryCastSpell(Druid335a.WildGrowth, target.Guid, true))
                {
                    return true;
                }

                if (target.HealthPercentage < 20.0
                    && TryCastSpell(Druid335a.NaturesSwiftness, target.Guid, true)
                    && TryCastSpell(Druid335a.HealingTouch, target.Guid, true))
                {
                    return true;
                }

                if (target.HealthPercentage < 50.0
                    && (target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Druid335a.Regrowth) || target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Druid335a.Rejuvenation))
                    && SwiftmendEvent.Ready
                    && TryCastSpell(Druid335a.Swiftmend, target.Guid, true)
                    && SwiftmendEvent.Run())
                {
                    return true;
                }

                if (target.HealthPercentage < 95.0
                    && target.HealthPercentage > 70.0
                    && !target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Druid335a.Rejuvenation)
                    && TryCastSpell(Druid335a.Rejuvenation, target.Guid, true))
                {
                    return true;
                }

                if (target.HealthPercentage < 98.0
                    && target.HealthPercentage > 70.0
                    && !target.Auras.Any(e => Bot.Db.GetSpellName(e.SpellId) == Druid335a.Lifebloom)
                    && TryCastSpell(Druid335a.Lifebloom, target.Guid, true))
                {
                    return true;
                }

                if (HealingManager.Tick())
                {
                    return true;
                }
            }

            return false;
        }
    }
}