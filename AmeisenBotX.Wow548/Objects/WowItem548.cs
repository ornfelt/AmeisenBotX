using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Raw.Enums;
using AmeisenBotX.Wow.Objects.Raw.SubStructs;
using AmeisenBotX.Wow548.Objects.Descriptors;

/// <summary>
/// Represents a namespace for World of Warcraft objects related to version 5.4.8.
/// </summary>
namespace AmeisenBotX.Wow548.Objects
{
    /// <summary>
    /// Represents a World of Warcraft item with a specific item descriptor.
    /// </summary>
    [Serializable]
    public unsafe class WowItem548 : WowObject548, IWowItem
    {
        /// <summary>
        /// Gets or sets the protected ItemDescriptor of type WowItemDescriptor548.
        /// </summary>
        protected WowItemDescriptor548? ItemDescriptor;

        /// <summary>
        /// Gets the count of the stack for the item descriptor.
        /// </summary>
        public int Count => GetItemDescriptor().StackCount;

        /// <summary>
        /// Gets or sets the list of item enchantments.
        /// </summary>
        public List<ItemEnchantment> ItemEnchantments { get; private set; }

        /// <summary>
        /// Gets the owner of the item descriptor.
        /// </summary>
        public ulong Owner => GetItemDescriptor().Owner;

        /// <summary>
        /// Retrieves a collection of strings representing the enchantments associated with an item.
        /// </summary>
        /// <returns>
        /// An IEnumerable containing the enchantment strings.
        /// </returns>
        public IEnumerable<string> GetEnchantmentStrings()
        {
            List<string> enchantments = new();

            for (int i = 0; i < ItemEnchantments.Count; ++i)
            {
                if (WowEnchantmentHelper.TryLookupEnchantment(ItemEnchantments[i].Id, out string text))
                {
                    enchantments.Add(text);
                }
            }

            return enchantments;
        }

        ///<summary>Returns a string representing the current object in the format: Item: [Guid] (EntryId) Owner: Owner Count: Count.</summary>
        public override string ToString()
        {
            return $"Item: [{Guid}] ({EntryId}) Owner: {Owner} Count: {Count}";
        }

        /// <summary>
        /// Updates the object and its item enchantments.
        /// </summary>
        public override void Update()
        {
            base.Update();

            // ItemEnchantments = new() { objPtr.Enchantment1, objPtr.Enchantment2,
            // objPtr.Enchantment3, objPtr.Enchantment4, objPtr.Enchantment5, objPtr.Enchantment6,
            // objPtr.Enchantment7, objPtr.Enchantment8, objPtr.Enchantment9, objPtr.Enchantment10,
            // objPtr.Enchantment11, objPtr.Enchantment12, };
        }

        /// <summary>
        /// Retrieves the WowItemDescriptor548 associated with the current instance.
        /// If the ItemDescriptor is not null, it returns the existing ItemDescriptor.
        /// Otherwise, it reads the memory at the DescriptorAddress plus the size of WowObjectDescriptor548,
        /// assigns the result to ItemDescriptor, and returns it. If the memory reading fails,
        /// it creates a new instance of WowItemDescriptor548 and returns it.
        /// </summary>
        protected WowItemDescriptor548 GetItemDescriptor()
        {
            return ItemDescriptor ??= Memory.Read(DescriptorAddress + sizeof(WowObjectDescriptor548), out WowItemDescriptor548 objPtr) ? objPtr : new();
        }
    }
}