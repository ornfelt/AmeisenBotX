using System.Collections.Generic;

/// <summary>
/// Represents a namespace for managing World of Warcraft inventory items.
/// </summary>
namespace AmeisenBotX.Core.Managers.Character.Inventory.Objects
{
    /// <summary>
    /// Represents a Wow inventory item.
    /// </summary>
    /// <summary>
    /// Gets the bag ID of the item.
    /// </summary>
    /// <summary>
    /// Gets the bag slot of the item.
    /// </summary>
    /// <summary>
    /// Gets the count of the item.
    /// </summary>
    /// <summary>
    /// Gets the durability of the item.
    /// </summary>
    /// <summary>
    /// Gets the equip location of the item.
    /// </summary>
    /// <summary>
    /// Gets the equip slot of the item.
    /// </summary>
    /// <summary>
    /// Gets the ID of the item.
    /// </summary>
    /// <summary>
    /// Gets the item level of the item.
    /// </summary>
    /// <summary>
    /// Gets the item link of the item.
    /// </summary>
    /// <summary>
    /// Gets the item quality of the item.
    /// </summary>
    /// <summary>
    /// Gets the maximum durability of the item.
    /// </summary>
    /// <summary>
    /// Gets the maximum stack count of the item.
    /// </summary>
    /// <summary>
    /// Gets the name of the item.
    /// </summary>
    /// <summary>
    /// Gets the price of the item.
    /// </summary>
    /// <summary>
    /// Gets the required level of the item.
    /// </summary>
    /// <summary>
    /// Gets the stats of the item.
    /// </summary>
    /// <summary>
    /// Gets the subtype of the item.
    /// </summary>
    /// <summary>
    /// Gets the type of the item.
    /// </summary>
    public interface IWowInventoryItem
    {
        /// <summary>
        /// Gets the identifier of the bag.
        /// </summary>
        int BagId { get; }

        /// <summary>
        /// Gets the bag slot.
        /// </summary>
        int BagSlot { get; }

        /// <summary>
        /// Gets the count of the elements.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets the durability of the object.
        /// </summary>
        int Durability { get; }

        /// <summary>
        /// Gets the location of the equipment.
        /// </summary>
        string EquipLocation { get; }

        /// <summary>
        /// Gets the equipment slot.
        /// </summary>
        int EquipSlot { get; }

        /// <summary>
        /// Gets the Id value.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Gets the level of the item.
        /// </summary>
        int ItemLevel { get; }

        /// <summary>
        /// Gets or sets the link of the item.
        /// </summary>
        string ItemLink { get; }

        /// <summary>
        /// Gets the quality of the item.
        /// </summary>
        int ItemQuality { get; }

        /// <summary>
        /// Gets the maximum durability.
        /// </summary>
        int MaxDurability { get; }

        /// <summary>
        /// Gets the maximum number of elements that the stack can hold.
        /// </summary>
        int MaxStack { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the price of the item.
        /// </summary>
        int Price { get; }

        /// <summary>
        /// Gets the required level.
        /// </summary>
        int RequiredLevel { get; }

        /// <summary>
        /// Gets the dictionary of statistics.
        /// </summary>
        Dictionary<string, string> Stats { get; }

        /// <summary>
        /// Gets the subtype of the string property.
        /// </summary>
        string Subtype { get; }

        /// <summary>
        /// Gets the type of the string.
        /// </summary>
        string Type { get; }
    }
}