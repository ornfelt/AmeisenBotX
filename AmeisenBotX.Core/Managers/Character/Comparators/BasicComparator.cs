using AmeisenBotX.Core.Managers.Character.Comparators.Objects;
using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Managers.Character.Comparators
{
    /// <summary>
    /// Represents a basic item comparator that compares items based on their armor and weapon types.
    /// </summary>
    public class BasicComparator : IItemComparator
    {
        /// <summary>
        /// Initializes a new instance of the BasicComparator class with the specified armor type blacklist and weapon type blacklist.
        /// </summary>
        /// <param name="armorTypeBlacklist">The list of WowArmorType objects representing the armor types to blacklist.</param>
        /// <param name="weaponTypeBlacklist">The list of WowWeaponType objects representing the weapon types to blacklist.</param>
        public BasicComparator(List<WowArmorType> armorTypeBlacklist, List<WowWeaponType> weaponTypeBlacklist)
        {
            ArmorTypeBlacklist = armorTypeBlacklist;
            WeaponTypeBlacklist = weaponTypeBlacklist;
        }

        /// <summary>
        /// Initializes a new instance of the BasicComparator class.
        /// </summary>
        /// <param name="armorTypeBlacklist">The list of armor types to be blacklisted.</param>
        /// <param name="weaponTypeBlacklist">The list of weapon types to be blacklisted.</param>
        /// <param name="statPriorities">The dictionary of stat priorities.</param>
        public BasicComparator(List<WowArmorType> armorTypeBlacklist, List<WowWeaponType> weaponTypeBlacklist, Dictionary<string, double> statPriorities)
        {
            ArmorTypeBlacklist = armorTypeBlacklist;
            WeaponTypeBlacklist = weaponTypeBlacklist;
            GearscoreFactory = new(statPriorities);
        }

        ///<summary>
        /// Gets or sets the GearscoreFactory property.
        ///</summary>
        protected GearscoreFactory GearscoreFactory { get; set; }

        /// <summary>
        /// Gets or sets the list of WowArmorTypes that are blacklisted.
        /// </summary>
        private List<WowArmorType> ArmorTypeBlacklist { get; }

        /// <summary>
        /// Gets or sets the list of weapon types that are blacklisted.
        /// </summary>
        /// <value>
        /// The list of weapon types that are blacklisted.
        /// </value>
        private List<WowWeaponType> WeaponTypeBlacklist { get; }

        /// <summary>
        /// Determines if the given inventory item is better than the current inventory item based on gear score and blacklisted armor or weapon types.
        /// </summary>
        /// <param name="current">The current inventory item.</param>
        /// <param name="item">The inventory item to compare.</param>
        /// <returns>True if the given item is better than the current item, false otherwise.</returns>
        public bool IsBetter(IWowInventoryItem current, IWowInventoryItem item)
        {
            if ((ArmorTypeBlacklist != null && item.GetType() == typeof(WowArmor) && ArmorTypeBlacklist.Contains(((WowArmor)item).ArmorType))
                || (WeaponTypeBlacklist != null && item.GetType() == typeof(WowWeapon) && WeaponTypeBlacklist.Contains(((WowWeapon)item).WeaponType)))
            {
                return false;
            }

            double scoreCurrent = GearscoreFactory.Calculate(current);
            double scoreNew = GearscoreFactory.Calculate(item);
            return scoreCurrent < scoreNew;
        }

        /// <summary>
        /// Checks if the provided item is in the blacklist based on its type and specific type properties.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>True if the item is blacklisted, false otherwise.</returns>
        public bool IsBlacklistedItem(IWowInventoryItem item)
        {
            if (ArmorTypeBlacklist != null && string.Equals(item.Type, "Armor", StringComparison.OrdinalIgnoreCase) && ArmorTypeBlacklist.Contains(((WowArmor)item).ArmorType))
            {
                return true;
            }
            else if (WeaponTypeBlacklist != null && string.Equals(item.Type, "Weapon", StringComparison.OrdinalIgnoreCase) && WeaponTypeBlacklist.Contains(((WowWeapon)item).WeaponType))
            {
                return true;
            }

            return false;
        }
    }
}