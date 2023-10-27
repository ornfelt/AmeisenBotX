using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Linq;

namespace AmeisenBotX.Core.Logic.Routines
{
    public static class SellItemsRoutine
    {
        /// <summary>
        /// This method is used to sell items from the bot's inventory.
        /// It creates a copy of items to prevent updates while selling.
        /// It iterates through each item in the inventory and checks if it should be sold based on the configured item sell blacklist 
        /// and the item quality settings in the AmeisenBotConfig object.
        /// If the item is an improvement for the character, it equips the new item and sells the previous one.
        /// If the bot's class is a Hunter and the item to sell is a WowProjectile, it skips to the next item.
        /// Finally, it uses the container item and confirms the static popup for each item to sell.
        /// After selling all items, it clears the current target.
        /// </summary>
        public static void Run(AmeisenBotInterfaces bot, AmeisenBotConfig config)
        {
            // create a copy of items here to prevent updates while selling
            foreach (IWowInventoryItem item in bot.Character.Inventory.Items
                .Where(e => e is { Price: > 0 })
                .ToList())
            {
                IWowInventoryItem itemToSell = item;

                if (config.ItemSellBlacklist.Any(e => e.Equals(item.Name, StringComparison.OrdinalIgnoreCase))
                    || !config.SellGrayItems && item.ItemQuality == (int)WowItemQuality.Poor
                    || !config.SellWhiteItems && item.ItemQuality == (int)WowItemQuality.Common
                    || !config.SellGreenItems && item.ItemQuality == (int)WowItemQuality.Uncommon
                    || !config.SellBlueItems && item.ItemQuality == (int)WowItemQuality.Rare
                    || !config.SellPurpleItems && item.ItemQuality == (int)WowItemQuality.Epic)
                {
                    continue;
                }

                if (bot.Character.IsItemAnImprovement(itemToSell, out IWowInventoryItem itemToReplace)
                    && itemToReplace != null)
                {
                    // equip item and sell the other after
                    itemToSell = itemToReplace;
                    bot.Wow.EquipItem(item.Name, itemToReplace.EquipSlot);
                }

                if (bot.Objects.Player.Class == WowClass.Hunter &&
                    itemToSell.GetType() == typeof(WowProjectile))
                {
                    continue;
                }

                bot.Wow.UseContainerItem(itemToSell.BagId, itemToSell.BagSlot);
                bot.Wow.CofirmStaticPopup();
            }
            bot.Wow.ClearTarget();
        }
    }
}