using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow.Objects.Raw.SubStructs;
using System.Collections.Generic;

namespace AmeisenBotX.Wow.Objects
{
    /// <summary>
    /// Represents an item in the World of Warcraft game.
    /// </summary>
    /// <seealso cref="IWowObject" />
    public interface IWowItem : IWowObject
    {
        /// <summary>
        /// Gets the count of items.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets or sets the list of ItemEnchantments.
        /// </summary>
        List<ItemEnchantment> ItemEnchantments { get; }

        /// <summary>
        /// Gets the owner of the code.
        /// </summary>
        ulong Owner { get; }

        /// <summary>
        /// Gets the type of the WowObject as an Item.
        /// </summary>
        public new WowObjectType Type => WowObjectType.Item;

        /// <summary>
        /// Retrieves a collection of enchantment strings.
        /// </summary>
        /// <returns>An enumerable collection of strings representing enchantments.</returns>
        IEnumerable<string> GetEnchantmentStrings();
    }
}