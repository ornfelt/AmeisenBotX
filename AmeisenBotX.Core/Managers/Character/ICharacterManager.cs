using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Managers.Character.Comparators;
using AmeisenBotX.Core.Managers.Character.Inventory;
using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using AmeisenBotX.Core.Managers.Character.Spells;
using AmeisenBotX.Core.Managers.Character.Talents;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;

/// <summary>
/// Namespace for managing character information and actions.
/// </summary>
namespace AmeisenBotX.Core.Managers.Character
{
    /// <summary>
    /// Interface for managing character information and actions.
    /// </summary>
    public interface ICharacterManager
    {
        /// <summary>
        /// Gets the equipment of the character.
        /// </summary>
        CharacterEquipment Equipment { get; }

        /// <summary>
        /// Gets the character's inventory.
        /// </summary>
        CharacterInventory Inventory { get; }

        /// <summary>
        /// Gets or sets the item comparator.
        /// </summary>
        IItemComparator ItemComparator { get; set; }

        /// <summary>
        /// Gets or sets a list of WowEquipmentSlot objects representing the item slots to be skipped.
        /// </summary>
        List<WowEquipmentSlot> ItemSlotsToSkip { get; set; }

        /// <summary>
        /// Gets or sets the last level that was trained.
        /// </summary>
        int LastLevelTrained { get; set; }

        /// <summary>
        /// Gets or sets the amount of money.
        /// </summary>
        int Money { get; }

        /// <summary>
        /// Gets the collection of WowMount objects.
        /// </summary>
        IEnumerable<WowMount> Mounts { get; }

        /// <summary>
        /// Gets the dictionary of skills where the key is a string and the value is a tuple of two integers.
        /// </summary>
        Dictionary<string, (int, int)> Skills { get; }

        /// <summary>
        /// Gets the SpellBook object.
        /// </summary>
        SpellBook SpellBook { get; }

        /// <summary>
        /// Gets the TalentManager property which represents the talent management system.
        /// </summary>
        TalentManager TalentManager { get; }

        /// <summary>
        /// Checks if the specified item type is present in the bag.
        /// </summary>
        /// <typeparam name="T">The type of item to check.</typeparam>
        /// <param name="needsToBeUseable">Optional. Specifies whether the item needs to be usable.</param>
        /// <returns>True if the item type is found in the bag, false otherwise.</returns>
        bool HasItemTypeInBag<T>(bool needsToBeUseable = false);

        /// <summary>
        /// Determines if the character is able to use the specified World of Warcraft armor item.
        /// </summary>
        /// <param name="item">The World of Warcraft armor item to check.</param>
        /// <returns>True if the character is able to use the armor item, otherwise false.</returns>
        bool IsAbleToUseArmor(WowArmor item);

        /// <summary>
        /// Determines if the specified WowWeapon item can be used.
        /// </summary>
        /// <param name="item">The WowWeapon item to be checked.</param>
        /// <returns>True if the item can be used; otherwise, false.</returns>
        bool IsAbleToUseWeapon(WowWeapon item);

        /// <summary>
        /// Determines if the provided inventory item is an improvement, and returns the item to replace if true.
        /// </summary>
        bool IsItemAnImprovement(IWowInventoryItem item, out IWowInventoryItem itemToReplace);

        /// <summary>
        /// Jump() performs a jump action.
        /// </summary>
        void Jump();

        /// <summary>
        /// Moves an object to a specified position.
        /// </summary>
        /// <param name="pos">The position the object should move to.</param>
        /// <param name="turnSpeed">The speed at which the object should turn while moving. Default value is 20.9.</param>
        /// <param name="distance">The minimum distance required for the object to reach the destination. Default value is 0.1.</param>
        void MoveToPosition(Vector3 pos, float turnSpeed = 20.9f, float distance = 0.1f);

        /// <summary>
        /// Updates all data.
        /// </summary>
        void UpdateAll();

        /// <summary>
        /// Updates the bags.
        /// </summary>
        void UpdateBags();

        /// <summary>
        /// Updates the gear.
        /// </summary>
        void UpdateGear();
    }
}