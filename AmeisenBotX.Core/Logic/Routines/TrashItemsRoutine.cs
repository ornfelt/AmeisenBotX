using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using AmeisenBotX.Logging;

namespace AmeisenBotX.Core.Logic.Routines
{
    /// <summary>
    /// Smart trash routine using shared ItemEvaluator logic.
    /// Deletes the lowest quality/value disposable item to make bag space.
    /// Used before quest turn-ins when inventory is full.
    /// </summary>
    public static class TrashItemsRoutine
    {
        /// <summary>
        /// Tries to delete one trash item to make bag space.
        /// Uses normal mode (gray items only).
        /// </summary>
        public static bool TryDeleteOneItem(AmeisenBotInterfaces bot, AmeisenBotConfig config)
        {
            return TryDeleteOneItem(bot, config, DisposeMode.Trash);
        }

        /// <summary>
        /// Tries to delete one item to make bag space.
        /// Aggressive mode deletes gray AND white items if needed.
        /// </summary>
        public static bool TryDeleteOneItem(AmeisenBotInterfaces bot, AmeisenBotConfig config, DisposeMode mode)
        {
            IWowInventoryItem itemToDelete = ItemEvaluator.GetBestItemToDispose(bot, config, mode);

            if (itemToDelete != null)
            {
                AmeisenLogger.I.Log("TrashRoutine", $"Deleting to make space: {itemToDelete.Name} (Quality: {itemToDelete.ItemQuality}, Value: {itemToDelete.Price})");
                bot.Wow.DeleteItemByName(itemToDelete.Name);
                return true;
            }

            // If normal mode found nothing, try aggressive mode
            if (mode == DisposeMode.Trash)
            {
                itemToDelete = ItemEvaluator.GetBestItemToDispose(bot, config, DisposeMode.TrashAggressive);
                if (itemToDelete != null)
                {
                    AmeisenLogger.I.Log("TrashRoutine", $"Aggressively deleting: {itemToDelete.Name} (Quality: {itemToDelete.ItemQuality})");
                    bot.Wow.DeleteItemByName(itemToDelete.Name);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Tries to make the specified number of free bag slots.
        /// </summary>
        public static int TryMakeBagSpace(AmeisenBotInterfaces bot, AmeisenBotConfig config, int requiredSlots)
        {
            int deleted = 0;

            while (bot.Character.Inventory.FreeBagSlots < requiredSlots && deleted < requiredSlots)
            {
                if (!TryDeleteOneItem(bot, config, DisposeMode.TrashAggressive))
                {
                    break; // Nothing left to delete
                }

                deleted++;
                bot.Character.Inventory.Update();
            }

            return deleted;
        }
    }
}
