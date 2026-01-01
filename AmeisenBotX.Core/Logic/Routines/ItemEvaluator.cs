using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using AmeisenBotX.Wow;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Logic.Routines
{
    /// <summary>
    /// Advanced item evaluation system that scores items based on:
    /// - Class compatibility (can we use this armor/weapon type?)
    /// - Stat priorities for our class/spec
    /// - Profession relevance (tools, materials)
    /// - Item type (consumables, quest items, etc.)
    /// 
    /// Higher score = more valuable to keep, Lower score = dispose first
    /// </summary>
    public static class ItemEvaluator
    {
        #region Constants

        // Never dispose these items (infinite keep score)
        public static readonly HashSet<string> ProfessionToolNames = new(StringComparer.OrdinalIgnoreCase)
        {
            "Mining Pick", "Gnomish Army Knife", "Skinning Knife", "Finkle's Skinner",
            "Zulian Slicer", "Fishing Pole", "Strong Fishing Pole", "Big Iron Fishing Pole",
            "Seth's Graphite Fishing Pole", "Nat Pagle's Extreme Angler FC-5000",
            "Blacksmith Hammer", "Arclight Spanner", "Gyromatic Micro-Adjustor",
            "Virtuoso Inking Set"
        };

        // Critical items that should NEVER be deleted under any circumstances
        private static readonly HashSet<string> CriticalItemNames =
        [
            // Hearthstones
            "Hearthstone", "Ruhestein", "Pierre de foyer", "Камень возвращения",
            // Mounts (some are items)
            "Riding Turtle", "Magic Rooster Egg", "Big Love Rocket",
            // Special items
            "Argent Crusader's Tabard", "Tabard of the Argent Crusade",
            "Innkeeper's Daughter", "Ruby Slippers", "Blessed Medallion of Karabor",
            // Class items
            "Soul Shard", "Seelensplitter", "Shard of the Defiler",
            // Shaman totems (old system)
            "Earth Totem", "Fire Totem", "Water Totem", "Air Totem",
            // Warlock soul shards
            "Soul Shard Bag",
            // Keys
            "Skeleton Key", "Truesilver Skeleton Key", "Arcanite Skeleton Key", "Titanium Skeleton Key"
        ];

        // Critical item IDs (for items that might have localized names)
        private static readonly HashSet<int> CriticalItemIds =
        [
            6948,   // Hearthstone
            44314,  // Wrapped Gift (Brewfest)
            49040,  // Jeeves
            40772,  // MOLL-E
            21711,  // Lunar Festival Invitation
            44049,  // Damaged Necklace (quest starter)
        ];


        // Armor class restrictions now in WowClassHelper.ArmorTypeClasses

        #endregion

        #region Public API

        /// <summary>
        /// Evaluates all items in inventory and returns scored list.
        /// Can be called on bag updates for UI display.
        /// </summary>
        public static List<ItemScore> EvaluateInventory(AmeisenBotInterfaces bot, AmeisenBotConfig config)
        {
            return bot.Character.Inventory.Items
                .Select(item => new ItemScore(item, CalculateItemScore(bot, config, item)))
                .OrderBy(s => s.Score)
                .ToList();
        }

        /// <summary>
        /// Evaluates a single item and returns its score.
        /// </summary>
        public static ItemScore EvaluateItem(AmeisenBotInterfaces bot, AmeisenBotConfig config, IWowInventoryItem item)
        {
            return new ItemScore(item, CalculateItemScore(bot, config, item));
        }

        /// <summary>
        /// Gets items sorted by disposal priority (lowest score first).
        /// </summary>
        public static IEnumerable<IWowInventoryItem> GetItemsByDisposalPriority(AmeisenBotInterfaces bot, AmeisenBotConfig config)
        {
            return EvaluateInventory(bot, config)
                .Where(s => s.Score < 1000) // Exclude protected items
                .OrderBy(s => s.Score)
                .Select(s => s.Item);
        }

        /// <summary>
        /// Gets the best item to dispose of (sell or trash).
        /// </summary>
        public static IWowInventoryItem GetBestItemToDispose(AmeisenBotInterfaces bot, AmeisenBotConfig config, DisposeMode mode)
        {
            return GetItemsByDisposalPriority(bot, config)
                .FirstOrDefault(item => CanDisposeItem(bot, config, item, mode));
        }

        /// <summary>
        /// Calculates a comprehensive score for Sorting and UI.
        /// Agnostic of Class/Spec - focuses on objective Value (Quality, iLvl, Price).
        /// </summary>
        public static double CalculateSortScore(AmeisenBotInterfaces bot, AmeisenBotConfig config, IWowInventoryItem item)
        {
            // 1. Protected Items (Hearthstone, etc) -> Top Priority (10,000+)
            if (IsProtectedItem(bot, config, item))
            {
                if (CriticalItemNames.Contains(item.Name) || CriticalItemIds.Contains(item.Id))
                {
                    return 20_000; // Super Critical (Heartshtone always #1)
                }

                // Protected items sorted by Quality -> Name
                return 10_000 + (item.ItemQuality * 1000) + item.ItemLevel;
            }

            double score = 0;

            // 2. Item Quality (The Primary Bracket)
            // Gap of 1000 ensures hierarchy (Max iLvl is ~284, plus type bonus ~400, leaves room).
            // Poor(0)=0, Common(1)=1000, Green(2)=2000, Blue(3)=3000, Epic(4)=4000
            score += item.ItemQuality * 1000;

            // 3. Item Type Bonus (Secondary Bracket within Quality)
            // Range 0-400
            string type = item.Type?.ToLowerInvariant() ?? "";
            string sub = item.Subtype?.ToLowerInvariant() ?? "";

            if (type is "armor" or "weapon")
            {
                score += 400;
            }
            else if (type is "container" or "bag")
            {
                score += 350;
            }
            else if (type is "recipe")
            {
                score += 300;
            }
            else if (type is "gem" || sub.Contains("enchant"))
            {
                score += 250;
            }
            else if (type is "trade goods" or "tradeskill")
            {
                score += 200;
            }
            else if (type is "quest")
            {
                score += 150;
            }
            else if (type is "consumable")
            {
                score += 100;
            }
            else if (type is "key" or "miscellaneous")
            {
                score += 50;
            }

            // 4. Item Level (Power differentiation)
            // Adds 0-300 points usually.
            score += item.ItemLevel;

            // 5. Vendor Price (Economy differentiation)
            // Logarithmic: 1g = 1pt, 100g = 2pts. Small tie-breaker.
            if (item.Price > 0)
            {
                score += Math.Log10(item.Price);
            }

            // 6. Stack Completeness (Organization)
            // Adds 0-1 point.
            if (item.MaxStack > 1)
            {
                score += (double)item.Count / item.MaxStack;
            }

            return score;
        }

        /// <summary>
        /// Calculates score for Auto-Equip decisions.
        /// Heavily weighted by Class/Spec suitability.
        /// </summary>
        public static double CalculateEquipScore(AmeisenBotInterfaces bot, AmeisenBotConfig config, IWowInventoryItem item)
        {
            // ===== BASE SCORE FROM STATS & TYPE =====
            double score = 0;

            // 1. Quality & iLvl Base (Ensure higher level items generally win)
            score += item.ItemQuality * 20;
            score += item.ItemLevel;

            // 2. Class Compatibility (Can we use it? Is it our armor type?)
            score += GetClassCompatibilityScore(bot, item);

            // 3. Stat Weights (The meat of the decision)
            score += GetStatValueScore(bot, item);

            // 4. Weapon DPS (Crucial for weapons)
            if (item is WowWeapon weapon)
            {
                // DPS is usually the most important stat for weapons
                // Approximate DPS from stats isn't easy, but usually iLvl covers it.
                // If we could access Min/Max damage/Speed, we'd add it here.
                // For now, iLvl + Stats is the proxy.
            }

            return Math.Max(0, score);
        }

        /// <summary>
        /// Deprecated wrapper for backwards compatibility or default generic value.
        /// Prefer CalculateSortScore for UI/Sorting and CalculateEquipScore for Upgrades.
        /// </summary>
        public static double CalculateItemScore(AmeisenBotInterfaces bot, AmeisenBotConfig config, IWowInventoryItem item)
        {
            return CalculateSortScore(bot, config, item);
        }

        /// <summary>
        /// Determines if an item can be disposed of in the given mode.
        /// </summary>
        public static bool CanDisposeItem(AmeisenBotInterfaces bot, AmeisenBotConfig config, IWowInventoryItem item, DisposeMode mode)
        {
            // Use SortScore for disposal because it handles protected items (score 10,000+) correctly
            double score = CalculateSortScore(bot, config, item);

            // Protected items (score >= 10,000) are never disposed
            return score < 10_000 && mode switch
            {
                // Trash: gray items only (they don't need to have vendor value)
                DisposeMode.Trash => item.ItemQuality <= (int)WowItemQuality.Poor,
                // TrashAggressive: gray + white items
                DisposeMode.TrashAggressive => item.ItemQuality <= (int)WowItemQuality.Common,
                // Sell: based on config quality settings, must have price
                DisposeMode.Sell => item.Price > 0 && ShouldSellByQuality(config, item),
                _ => false
            };
        }

        /// <summary>
        /// Quick check if item should be kept (protected from all disposal).
        /// </summary>
        public static bool ShouldKeepItem(AmeisenBotInterfaces bot, AmeisenBotConfig config, IWowInventoryItem item)
        {
            return CalculateSortScore(bot, config, item) >= 10_000;
        }

        /// <summary>
        /// Compares two items for the same slot - returns true if newItem is better.
        /// MUST use CalculateEquipScore for combat relevance.
        /// </summary>
        public static bool IsUpgrade(AmeisenBotInterfaces bot, AmeisenBotConfig config, IWowInventoryItem newItem, IWowInventoryItem currentItem)
        {
            double newScore = CalculateEquipScore(bot, config, newItem);
            double currentScore = CalculateEquipScore(bot, config, currentItem);
            return newScore > currentScore;
        }

        #endregion

        #region Scoring Components

        private static bool IsProtectedItem(AmeisenBotInterfaces bot, AmeisenBotConfig config, IWowInventoryItem item)
        {
            // Critical items (Hearthstone, etc.) - NEVER delete these
            if (CriticalItemNames.Contains(item.Name) || CriticalItemIds.Contains(item.Id))
            {
                return true;
            }

            // Blacklisted items (user config)
            if (config.ItemSellBlacklist.Any(e => e.Equals(item.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            // Profession tools
            if (ProfessionToolNames.Contains(item.Name))
            {
                return true;
            }

            // Quest items
            if (IsQuestItem(item))
            {
                return true;
            }

            // Consumables (food, water, potions, bandages)
            if (IsConsumable(item))
            {
                return true;
            }

            // Hunter ammo
            if (bot.Objects.Player?.Class == WowClass.Hunter && item.GetType() == typeof(WowProjectile))
            {
                return true;
            }

            // Profession Materials Check
            return IsProfessionMaterial(bot, item);
        }

        private static bool IsProfessionMaterial(AmeisenBotInterfaces bot, IWowInventoryItem item)
        {
            if (item.Type?.ToLowerInvariant() is not "trade goods" and not "tradeskill")
            {
                return false;
            }

            string name = item.Name.ToLowerInvariant();
            string sub = item.Subtype?.ToLowerInvariant() ?? "";

            // Mining / Blacksmithing / Engineering / Jewelcrafting
            bool hasMetalProf = HasSkill(bot, "Mining") || HasSkill(bot, "Blacksmithing") || HasSkill(bot, "Engineering") || HasSkill(bot, "Jewelcrafting");
            if (hasMetalProf)
            {
                if (name.Contains("ore") || name.Contains("bar") || name.Contains("stone") || sub.Contains("metal") || sub.Contains("stone"))
                {
                    return true;
                }

                if (HasSkill(bot, "Engineering") && (name.Contains("blasting") || name.Contains("powder") || name.Contains("gizmo")))
                {
                    return true;
                }

                if (HasSkill(bot, "Jewelcrafting") && (name.Contains("gem") || name.Contains("jewel") || sub.Contains("jewel")))
                {
                    return true;
                }
            }

            // Herbalism / Alchemy / Inscription
            bool hasHerbProf = HasSkill(bot, "Herbalism") || HasSkill(bot, "Alchemy") || HasSkill(bot, "Inscription");
            if (hasHerbProf)
            {
                if (sub.Contains("herb") || name.Contains("leaf") || name.Contains("root") || name.Contains("petal") || name.Contains("herb"))
                {
                    return true;
                }

                if (HasSkill(bot, "Inscription") && (name.Contains("pigment") || name.Contains("ink")))
                {
                    return true;
                }
            }

            // Skinning / Leatherworking
            bool hasLeatherProf = HasSkill(bot, "Skinning") || HasSkill(bot, "Leatherworking");
            if (hasLeatherProf)
            {
                if (name.Contains("leather") || name.Contains("hide") || name.Contains("scale") || name.Contains("scrap") || sub.Contains("leather"))
                {
                    return true;
                }
            }

            // Tailoring
            if (HasSkill(bot, "Tailoring") || HasSkill(bot, "First Aid"))
            {
                if (name.Contains("cloth") || name.Contains("bolt") || name.Contains("linen") || name.Contains("wool") || name.Contains("silk") || name.Contains("mageweave") || name.Contains("runecloth"))
                {
                    return true;
                }
            }

            // Enchanting
            if (HasSkill(bot, "Enchanting"))
            {
                if (name.Contains("dust") || name.Contains("essence") || name.Contains("shard") || name.Contains("crystal"))
                {
                    return true;
                }
            }

            // Cooking
            if (HasSkill(bot, "Cooking"))
            {
                if (name.Contains("meat") || name.Contains("egg") || name.Contains("flesh") || sub.Contains("cooking"))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasSkill(AmeisenBotInterfaces bot, string skillName)
        {
            return bot.Character.Skills.ContainsKey(skillName);
        }

        private static double GetItemTypeScore(AmeisenBotInterfaces bot, IWowInventoryItem item)
        {
            string type = item.Type?.ToLowerInvariant() ?? "";

            // Equipment we can use is valuable
            if (type is "armor" or "weapon")
            {
                // Fix: Don't consider profession tools as combat weapons
                if (ProfessionToolNames.Contains(item.Name))
                {
                    return -100; // Penalize tools so they aren't equipped
                }

                // Check if we can equip it
                if (item is WowArmor armor && bot.Character.IsAbleToUseArmor(armor))
                {
                    return 30;
                }

                if (item is WowWeapon weapon && bot.Character.IsAbleToUseWeapon(weapon))
                {
                    return 30;
                }
                // Unusable equipment scores low
                return -20;
            }

            // Trade goods for our professions
            if (type is "trade goods" or "tradeskill")
            {
                return 15;
            }

            // Reagents, keys, misc
            return type is "reagent" or "key" ? 20 : 0;
        }

        private static double GetClassCompatibilityScore(AmeisenBotInterfaces bot, IWowInventoryItem item)
        {
            if (bot.Objects.Player == null)
            {
                return 0;
            }

            WowClass playerClass = bot.Objects.Player.Class;

            if (item is WowArmor armor)
            {
                // Check if this is our preferred armor type
                WowArmorType preferredArmor = WowClassHelper.GetPreferredArmorType(playerClass, bot.Objects.Player.Level);
                if (armor.ArmorType == preferredArmor)
                {
                    return 25;
                }

                // Usable but not preferred
                if (WowClassHelper.CanClassWearArmor(playerClass, armor.ArmorType))
                {
                    return 10;
                }

                // Can't use at all
                return -30;
            }

            if (item is WowWeapon weapon)
            {
                // Check if weapon type matches class
                return bot.Character.IsAbleToUseWeapon(weapon) ? 20 : -30;
            }

            return 0;
        }

        private static double GetStatValueScore(AmeisenBotInterfaces bot, IWowInventoryItem item)
        {
            // Get spec-specific stat weights using enum
            WowSpecialization spec = bot.CombatClass?.Specialization ?? WowSpecialization.None;
            Dictionary<string, double> weights = null;

            if (spec != WowSpecialization.None)
            {
                weights = SpecStatWeights.GetWeights(spec);
            }

            // Fallback to role-based defaults
            if (weights == null)
            {
                WowRole role = bot.CombatClass?.Role ?? WowRole.Dps;
                weights = role switch
                {
                    WowRole.Tank => SpecStatWeights.DefaultTankWeights,
                    WowRole.Heal => SpecStatWeights.DefaultHealWeights,
                    _ => SpecStatWeights.DefaultDpsWeights
                };
            }

            double statScore = 0;

            // Parse item stats from name/subtype
            string itemText = $"{item.Name} {item.Subtype}".ToLowerInvariant();

            foreach (KeyValuePair<string, double> kvp in weights)
            {
                string statName = kvp.Key.ToLowerInvariant();
                if (itemText.Contains(statName))
                {
                    statScore += kvp.Value * 8; // Higher multiplier for accuracy
                }
            }

            // Check for "of the [suffix]" names (e.g., "of the Bear", "of the Eagle")
            statScore += GetSuffixStatScore(itemText, weights);

            return statScore;
        }

        private static double GetSuffixStatScore(string itemText, Dictionary<string, double> weights)
        {
            double score = 0;

            // Common WoW item suffixes and their primary stats
            if (itemText.Contains("of the bear") || itemText.Contains("of the soldier"))
            {
                // Stamina + Strength
                if (weights.TryGetValue("Stamina", out double stam))
                {
                    score += stam * 5;
                }

                if (weights.TryGetValue("Strength", out double str))
                {
                    score += str * 5;
                }
            }
            else if (itemText.Contains("of the eagle") || itemText.Contains("of the falcon"))
            {
                // Intellect + Stamina
                if (weights.TryGetValue("Intellect", out double intel))
                {
                    score += intel * 5;
                }

                if (weights.TryGetValue("Stamina", out double stam))
                {
                    score += stam * 3;
                }
            }
            else if (itemText.Contains("of the monkey") || itemText.Contains("of the bandit"))
            {
                // Agility + Stamina
                if (weights.TryGetValue("Agility", out double agi))
                {
                    score += agi * 5;
                }

                if (weights.TryGetValue("Stamina", out double stam))
                {
                    score += stam * 3;
                }
            }
            else if (itemText.Contains("of the tiger") || itemText.Contains("of the champion"))
            {
                // Agility + Strength
                if (weights.TryGetValue("Agility", out double agi))
                {
                    score += agi * 4;
                }

                if (weights.TryGetValue("Strength", out double str))
                {
                    score += str * 4;
                }
            }
            else if (itemText.Contains("of the owl") || itemText.Contains("of the prophet"))
            {
                // Intellect + Spirit
                if (weights.TryGetValue("Intellect", out double intel))
                {
                    score += intel * 5;
                }

                if (weights.TryGetValue("Spirit", out double spirit))
                {
                    score += spirit * 4;
                }
            }
            else if (itemText.Contains("of the whale") || itemText.Contains("of the elder"))
            {
                // Stamina + Spirit
                if (weights.TryGetValue("Stamina", out double stam))
                {
                    score += stam * 4;
                }

                if (weights.TryGetValue("Spirit", out double spirit))
                {
                    score += spirit * 4;
                }
            }
            else if (itemText.Contains("of the boar") || itemText.Contains("of the knight"))
            {
                // Strength + Spirit
                if (weights.TryGetValue("Strength", out double str))
                {
                    score += str * 5;
                }

                if (weights.TryGetValue("Spirit", out double spirit))
                {
                    score += spirit * 2;
                }
            }
            else if (itemText.Contains("of the gorilla") || itemText.Contains("of the beast"))
            {
                // Strength + Intellect
                if (weights.TryGetValue("Strength", out double str))
                {
                    score += str * 4;
                }

                if (weights.TryGetValue("Intellect", out double intel))
                {
                    score += intel * 3;
                }
            }
            else if (itemText.Contains("of power") || itemText.Contains("of striking"))
            {
                // Attack Power
                if (weights.TryGetValue("AttackPower", out double ap))
                {
                    score += ap * 8;
                }
            }
            else if (itemText.Contains("of healing") || itemText.Contains("of restoration"))
            {
                // Spell Power (healing)
                if (weights.TryGetValue("SpellPower", out double sp))
                {
                    score += sp * 8;
                }
            }

            return score;
        }

        private static double GetProfessionRelevanceScore(AmeisenBotInterfaces bot, IWowInventoryItem item)
        {
            HashSet<string> professions = GetCharacterProfessions(bot);
            if (professions.Count == 0)
            {
                return 0;
            }

            string name = item.Name?.ToLowerInvariant() ?? "";
            string subtype = item.Subtype?.ToLowerInvariant() ?? "";
            string combined = name + " " + subtype;

            // Mining/Blacksmithing/Engineering/Jewelcrafting
            if ((combined.Contains("ore") || combined.Contains("bar") || combined.Contains("stone") || combined.Contains("metal"))
                && (professions.Contains("Mining") || professions.Contains("Blacksmithing")
                    || professions.Contains("Engineering") || professions.Contains("Jewelcrafting")))
            {
                return 25;
            }

            // Herbalism/Alchemy/Inscription
            if ((combined.Contains("herb") || combined.Contains("lotus") || combined.Contains("bloom") || combined.Contains("leaf"))
                && (professions.Contains("Herbalism") || professions.Contains("Alchemy") || professions.Contains("Inscription")))
            {
                return 25;
            }

            // Skinning/Leatherworking
            if ((combined.Contains("leather") || combined.Contains("hide") || combined.Contains("scale"))
                && (professions.Contains("Skinning") || professions.Contains("Leatherworking")))
            {
                return 25;
            }

            // Tailoring/First Aid
            if ((combined.Contains("cloth") || combined.Contains("silk") || combined.Contains("linen") || combined.Contains("wool"))
                && (professions.Contains("Tailoring") || professions.Contains("First Aid")))
            {
                return 25;
            }

            // Enchanting
            if ((combined.Contains("dust") || combined.Contains("essence") || combined.Contains("shard") || combined.Contains("crystal"))
                && professions.Contains("Enchanting"))
            {
                return 25;
            }

            // Jewelcrafting
            if ((combined.Contains("gem") || combined.Contains("jewel"))
                && professions.Contains("Jewelcrafting"))
            {
                return 25;
            }

            // Cooking
            return (combined.Contains("meat") || combined.Contains("fish") || combined.Contains("spice") || combined.Contains("egg"))
                && professions.Contains("Cooking")
                ? 20
                : 0;
        }

        // GetPreferredArmorType moved to WowClassHelper.GetPreferredArmorType

        #endregion

        #region Helpers

        public static HashSet<string> GetCharacterProfessions(AmeisenBotInterfaces bot)
        {
            HashSet<string> professions = [];
            string[] allProfessions =
            [
                "Alchemy", "Blacksmithing", "Enchanting", "Engineering",
                "Herbalism", "Inscription", "Jewelcrafting", "Leatherworking",
                "Mining", "Skinning", "Tailoring", "Cooking", "First Aid", "Fishing"
            ];

            foreach (string prof in allProfessions)
            {
                if (bot.Character.Skills.ContainsKey(prof))
                {
                    professions.Add(prof);
                }
            }

            return professions;
        }

        public static bool IsQuestItem(IWowInventoryItem item)
        {
            return string.Equals(item.Type, "Quest", StringComparison.OrdinalIgnoreCase)
                || item.ItemQuality == 7;
        }

        public static bool IsConsumable(IWowInventoryItem item)
        {
            string type = item.Type?.ToLowerInvariant() ?? "";
            string subtype = item.Subtype?.ToLowerInvariant() ?? "";
            return type == "consumable"
                || subtype.Contains("food") || subtype.Contains("drink")
                || subtype.Contains("potion") || subtype.Contains("elixir")
                || subtype.Contains("flask") || subtype.Contains("bandage");
        }

        public static bool IsUnusableEquipment(AmeisenBotInterfaces bot, IWowInventoryItem item)
        {
            return item is WowArmor armor
                ? !bot.Character.IsAbleToUseArmor(armor)
                : item is WowWeapon weapon && !bot.Character.IsAbleToUseWeapon(weapon);
        }

        private static bool ShouldSellByQuality(AmeisenBotConfig config, IWowInventoryItem item)
        {
            return (config.SellGrayItems && item.ItemQuality == (int)WowItemQuality.Poor)
                || (config.SellWhiteItems && item.ItemQuality == (int)WowItemQuality.Common)
                || (config.SellGreenItems && item.ItemQuality == (int)WowItemQuality.Uncommon)
                || (config.SellBlueItems && item.ItemQuality == (int)WowItemQuality.Rare)
                || (config.SellPurpleItems && item.ItemQuality == (int)WowItemQuality.Epic);
        }

        #endregion
    }

    /// <summary>
    /// Represents an item with its calculated score.
    /// </summary>
    public class ItemScore
    {
        public IWowInventoryItem Item { get; }
        public double Score { get; }

        public ItemScore(IWowInventoryItem item, double score)
        {
            Item = item;
            Score = score;
        }

        public override string ToString() => $"{Item.Name}: {Score:F1}";
    }

    public enum DisposeMode
    {
        /// <summary>Only gray items</summary>
        Trash,
        /// <summary>Gray and white items (for making bag space)</summary>
        TrashAggressive,
        /// <summary>Based on config quality settings</summary>
        Sell
    }
}
