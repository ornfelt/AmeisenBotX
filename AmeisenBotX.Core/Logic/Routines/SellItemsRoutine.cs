using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Logic.Routines
{
    /// <summary>
    /// Smart sell routine using shared ItemEvaluator logic.
    /// Sells everything that is disposable, keeps profession tools/materials/consumables.
    /// </summary>
    public static class SellItemsRoutine
    {
        public static void Run(AmeisenBotInterfaces bot, AmeisenBotConfig config, List<IWowInventoryItem> itemsToSell)
        {
            // Create a copy of items to prevent updates while selling
            foreach (IWowInventoryItem item in itemsToSell)
            {
                // Sell the item
                bot.Wow.UseContainerItem(item.BagId, item.BagSlot);
                bot.Wow.ConfirmStaticPopup();
            }

        }

        public static List<IWowInventoryItem> GetSellableItems(AmeisenBotInterfaces bot, AmeisenBotConfig config)
        {
            return bot.Character.Inventory.Items
                .Where(items => items.Price > 0)
                .Where(item => ItemEvaluator.CanDisposeItem(bot, config, item, DisposeMode.Sell))
                .Where(item => !bot.Character.IsItemAnImprovement(item, out _))
                .ToList();
        }
    }
}
