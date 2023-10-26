using AmeisenBotX.Wow.Objects.Raw.SubStructs;
using System.Runtime.InteropServices;

namespace AmeisenBotX.Wow335a.Objects.Descriptors
{
    /// <summary>
    /// Represents a WowItemDescriptor335a struct, which contains information about a World of Warcraft item.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct WowItemDescriptor335a
    {
        /// <summary>
        /// Gets or sets the owner of the object.
        /// </summary>
        public ulong Owner;
        /// <summary>
        /// Gets or sets the value contained in the ulong variable.
        /// </summary>
        public ulong Contained;
        /// <summary>
        /// Gets or sets the creator of the object.
        /// </summary>
        public ulong Creator;
        /// <summary>
        /// Represents the identifier of the gift creator.
        /// </summary>
        public ulong GiftCreator;
        /// <summary>
        /// Gets or sets the count of items in the stack.
        /// </summary>
        public int StackCount;
        /// <summary>
        /// Gets or sets the duration of the object.
        /// </summary>
        public int Duration;
        /// <summary>
        /// Gets or sets the spell charge level 1.
        /// </summary>
        public int SpellCharge1;
        /// <summary>
        /// Represents the level of charge for the second spell. 
        /// </summary>
        public int SpellCharge2;
        /// <summary>
        /// Represents the third level of spell charge.
        /// </summary>
        public int SpellCharge3;
        /// <summary>
        /// Represents the Spell Charge 4, which is an integer value.
        /// </summary>
        public int SpellCharge4;
        ///<summary>
        /// The variable representing the spell charge level 5.
        ///</summary>
        public int SpellCharge5;
        /// <summary>
        /// Gets or sets the flags associated with the integer value.
        /// </summary>
        public int Flags;
        /// <summary>
        /// The first item enchantment.
        /// </summary>
        public ItemEnchantment Enchantment1;
        /// <summary>
        /// The second item enchantment.
        /// </summary>
        public ItemEnchantment Enchantment2;
        /// <summary>
        /// Gets or sets the third item enchantment.
        /// </summary>
        public ItemEnchantment Enchantment3;
        /// <summary>
        /// Gets or sets the fourth item enchantment.
        /// </summary>
        public ItemEnchantment Enchantment4;
        /// <summary>
        /// Represents the fifth enchantment of an item.
        /// </summary>
        public ItemEnchantment Enchantment5;
        /// <summary>
        /// Gets or sets the sixth enchantment for the item.
        /// </summary>
        public ItemEnchantment Enchantment6;
        /// <summary>
        /// The enchantment for the seventh item.
        /// </summary>
        public ItemEnchantment Enchantment7;
        /// <summary>
        /// Represents the eighth enchantment for an item.
        /// </summary>
        public ItemEnchantment Enchantment8;
        /// <summary>
        /// The ninth enchantment for an item.
        /// </summary>
        public ItemEnchantment Enchantment9;
        /// <summary>
        /// Represents the 10th item enchantment.
        /// </summary>
        public ItemEnchantment Enchantment10;
        /// <summary>
        /// Represents the first enchantment associated with an item.
        /// </summary>
        public ItemEnchantment Enchantment11;
        /// <summary>
        /// Represents the item enchantment at position 12.
        /// </summary>
        public ItemEnchantment Enchantment12;
        /// <summary>
        /// Gets or sets the seed value for generating the property.
        /// </summary>
        public int PropertySeed;
        /// <summary>
        /// Gets or sets the ID for the random properties.
        /// </summary>
        public int RandomPropertiesId;
        /// <summary>
        /// Gets or sets the durability of an object.
        /// </summary>
        public int Durability;
        /// <summary>
        /// Gets or sets the maximum durability of an object.
        /// </summary>
        public int MaxDurability;
        /// <summary>
        /// Represents the amount of time a player has spent playing.
        /// </summary>
        public int CreatePlayedTime;
        /// <summary>
        /// Represents the WowItemPad field, which stores the integer value.
        /// </summary>
        public int WowItemPad;

        /// <summary>
        /// The end offset value.
        /// </summary>
        public static readonly int EndOffset = 232;
    }
}