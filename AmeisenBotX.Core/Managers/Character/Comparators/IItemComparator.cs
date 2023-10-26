using AmeisenBotX.Core.Managers.Character.Inventory.Objects;

namespace AmeisenBotX.Core.Managers.Character.Comparators
{
    /// <summary>
    /// Represents an interface for comparing World of Warcraft inventory items.
    /// </summary>
    public interface IItemComparator
    {
        /// <summary>
        /// Determines if the specified item is better than the current item.
        /// </summary>
        /// <param name="current">The current item.</param>
        /// <param name="item">The item to compare.</param>
        /// <returns>True if the specified item is better, otherwise false.</returns>
        bool IsBetter(IWowInventoryItem current, IWowInventoryItem item);

        /// <summary>
        /// Determines if the specified item is a blacklisted item in the World of Warcraft inventory.
        /// </summary>
        /// <param name="item">The item to be checked.</param>
        /// <returns>True if the item is blacklisted, false otherwise.</returns>
        bool IsBlacklistedItem(IWowInventoryItem item);
    }
}