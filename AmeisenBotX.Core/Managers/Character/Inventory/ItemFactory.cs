using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AmeisenBotX.Core.Managers.Character.Inventory
{
    /// <summary>
    /// Builds a specific item based on the type of the basic item.
    /// </summary>
    public static class ItemFactory
    {
        /// <summary>
        /// Builds a specific item based on the type of the basic item.
        /// </summary>
        /// <param name="basicItem">The basic item to build a specific item from.</param>
        /// <returns>A specific item based on the type of the basic item.</returns>
        public static WowBasicItem BuildSpecificItem(WowBasicItem basicItem)
        {
            if (basicItem == null)
            {
                throw new ArgumentNullException(nameof(basicItem), "basicItem cannot be null");
            }

            if (basicItem.Type == null)
            {
                return basicItem;
            }

            return basicItem.Type.ToUpper(CultureInfo.InvariantCulture) switch
            {
                "ARMOR" => new WowArmor(basicItem),
                "CONSUMABLE" => new WowConsumable(basicItem),
                "CONTAINER" => new WowContainer(basicItem),
                "GEM" => new WowGem(basicItem),
                "KEY" => new WowKey(basicItem),
                "MISCELLANEOUS" => new WowMiscellaneousItem(basicItem),
                "MONEY" => new WowMoneyItem(basicItem),
                "PROJECTILE" => new WowProjectile(basicItem),
                "QUEST" => new WowQuestItem(basicItem),
                "QUIVER" => new WowQuiver(basicItem),
                "REAGENT" => new WowReagent(basicItem),
                "RECIPE" => new WowRecipe(basicItem),
                "TRADE GOODS" => new WowTradeGoods(basicItem),
                "WEAPON" => new WowWeapon(basicItem),
                _ => basicItem,
            };
        }

        /// <summary>
        /// Parses a JSON string into a WowBasicItem object using JsonSerializer.
        /// </summary>
        /// <param name="json">The JSON string to parse.</param>
        /// <returns>The parsed WowBasicItem object.</returns>
        public static WowBasicItem ParseItem(string json)
        {
            return JsonSerializer.Deserialize<WowBasicItem>(json, new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            });
        }

        /// <summary>
        /// Parses a JSON string into a list of WowBasicItem objects.
        /// </summary>
        /// <param name="json">The JSON string to parse.</param>
        /// <returns>A list of WowBasicItem objects.</returns>
        public static List<WowBasicItem> ParseItemList(string json)
        {
            return JsonSerializer.Deserialize<List<WowBasicItem>>(json, new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            });
        }
    }
}