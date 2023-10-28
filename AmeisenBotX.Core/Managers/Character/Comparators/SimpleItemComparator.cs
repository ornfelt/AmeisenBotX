using AmeisenBotX.Core.Managers.Character.Comparators.Objects;
using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using System.Collections.Generic;

/// <summary>
/// Namespace for managing character comparators in the AmeisenBotX.Core.Managers.Character.Comparators namespace.
/// </summary>
namespace AmeisenBotX.Core.Managers.Character.Comparators
{
    /// <summary>
    /// Initializes a new instance of the SimpleItemComparator class.
    /// </summary>
    public class SimpleItemComparator : IItemComparator
    {
        /// <summary>
        /// Initializes a new instance of the SimpleItemComparator class.
        /// </summary>
        /// <param name="characterManager">The DefaultCharacterManager to be used.</param>
        /// <param name="statPriorities">The stat priorities for the SimpleItemComparator.</param>
        public SimpleItemComparator(DefaultCharacterManager characterManager, Dictionary<string, double> statPriorities)
        {
            // This introduces a cyclic dependency. Are we fine with that?
            CharacterManager = characterManager;
            GearscoreFactory = new(statPriorities);
        }

        /// <summary>
        /// Gets or sets the instance of the GearscoreFactory.
        /// </summary>
        protected GearscoreFactory GearscoreFactory { get; set; }

        /// <summary>
        /// Gets the private instance of DefaultCharacterManager.
        /// </summary>
        private DefaultCharacterManager CharacterManager { get; }

        /// <summary>
        /// Determines if the specified item is better than the current item based on gear score.
        /// </summary>
        /// <param name="current">The current inventory item.</param>
        /// <param name="item">The new inventory item.</param>
        /// <returns>True if the new item has a higher gear score, otherwise false.</returns>
        public bool IsBetter(IWowInventoryItem current, IWowInventoryItem item)
        {
            if (!CharacterManager.IsAbleToUseItem(item))
            {
                return false;
            }

            double scoreCurrent = GearscoreFactory.Calculate(current);
            double scoreNew = GearscoreFactory.Calculate(item);
            return scoreCurrent < scoreNew;
        }

        /// <summary>
        /// Determines whether the provided WowInventoryItem is a blacklisted item.
        /// </summary>
        /// <param name="item">The WowInventoryItem to check.</param>
        /// <returns>True if the item is blacklisted, false otherwise.</returns>
        public bool IsBlacklistedItem(IWowInventoryItem item)
        {
            return !CharacterManager.IsAbleToUseItem(item);
        }
    }
}