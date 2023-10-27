using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Raw.Enums;
using AmeisenBotX.Wow.Objects.Raw.SubStructs;
using AmeisenBotX.Wow335a.Objects.Descriptors;
using System;
using System.Collections.Generic;

namespace AmeisenBotX.Wow335a.Objects
{
    /// <summary>
    /// Represents a World of Warcraft item in the version 3.3.5a of the game.
    /// </summary>
    [Serializable]
    public class WowItem335a : WowObject335a, IWowItem
    {
        /// <summary>
        /// Gets or sets the count value.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the list of item enchantments.
        /// </summary>
        public List<ItemEnchantment> ItemEnchantments { get; private set; }

        /// <summary>
        /// Gets or sets the value representing the owner.
        /// </summary>
        public ulong Owner { get; set; }

        /// <summary>
        /// Retrieves a collection of strings representing the enchantments of an item. 
        /// </summary>
        /// <remarks>
        /// This method iterates through the <see cref="ItemEnchantments"/> collection and tries to look up the corresponding enchantment text using the <see cref="WowEnchantmentHelper.TryLookupEnchantment"/> method. 
        /// If a match is found, the enchantment text is added to the <see cref="enchantments"/> list. 
        /// Finally, the method returns the list of enchantment strings.
        /// </remarks>
        public IEnumerable<string> GetEnchantmentStrings()
        {
            List<string> enchantments = new();

            foreach (ItemEnchantment itemEnch in ItemEnchantments)
            {
                if (WowEnchantmentHelper.TryLookupEnchantment(itemEnch.Id, out string text))
                {
                    enchantments.Add(text);
                }
            }

            return enchantments;
        }

        /// <summary>
        /// Overrides the default ToString method to return a string representation of the object's properties.
        /// </summary>
        /// <returns>Returns a string containing the item's GUID, entry ID, owner, and count.</returns>
        public override string ToString()
        {
            return $"Item: [{Guid}] ({EntryId}) Owner: {Owner} Count: {Count}";
        }

        /// <summary>
        /// Overrides the update method to update the object's properties using the data from the memory.
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (Memory.Read(DescriptorAddress + WowObjectDescriptor335a.EndOffset, out WowItemDescriptor335a objPtr))
            {
                Count = objPtr.StackCount;
                Owner = objPtr.Owner;

                ItemEnchantments = new List<ItemEnchantment>
                {
                    objPtr.Enchantment1,
                    objPtr.Enchantment2,
                    objPtr.Enchantment3,
                    objPtr.Enchantment4,
                    objPtr.Enchantment5,
                    objPtr.Enchantment6,
                    objPtr.Enchantment7,
                    objPtr.Enchantment8,
                    objPtr.Enchantment9,
                    objPtr.Enchantment10,
                    objPtr.Enchantment11,
                    objPtr.Enchantment12,
                };
            }
        }
    }
}