using AmeisenBotX.Core.Managers.Character.Inventory.Objects;

namespace AmeisenBotX.Core.Managers.Character.Comparators
{
    /// <summary>
    /// Determines if the provided current item is better than the provided another item.
    /// </summary>
    public class ItemLevelComparator : IItemComparator
    {
        /// <summary>
        /// Determines if the provided current item is better than the provided another item.
        /// </summary>
        /// <param name="current">The current item to compare</param>
        /// <param name="item">The item to compare against</param>
        /// <returns>True if the current item is better than the another item, otherwise false</returns>
        public bool IsBetter(IWowInventoryItem current, IWowInventoryItem item)
        {
            return current == null || current.ItemLevel < item.ItemLevel;
        }

        /// <summary>
        /// Checks if the specified item is a blacklisted item.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>Returns true if the item is blacklisted, false otherwise.</returns>
        public bool IsBlacklistedItem(IWowInventoryItem item)
        {
            return false;
        }
    }
}