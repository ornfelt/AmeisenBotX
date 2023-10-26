using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace AmeisenBotX.Wow548.Objects.Descriptors
{
    /// <summary>
    /// Represents a WowItemDescriptor548, which describes the properties and attributes of a World of Warcraft item.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct WowItemDescriptor548
    {
        /// <summary>
        /// The owner of the specified property.
        /// </summary>
        public ulong Owner;
        /// <summary>
        /// Gets or sets the value indicating whether this ulong is contained in a certain structure or object.
        /// </summary>
        public ulong ContainedIn;
        /// <summary>
        /// Gets or sets the creator of the object.
        /// </summary>
        public ulong Creator;
        /// <summary>
        /// Represents the identification number of the gift creator.
        /// </summary>
        public ulong GiftCreator;
        /// <summary>
        /// Gets or sets the value representing the count of an element in the stack.
        /// </summary>
        public int StackCount;
        /// <summary>
        /// Gets or sets the expiration value.
        /// </summary>
        public int Expiration;
        /// <summary>
        /// Gets or sets the array of spell charges, where each index represents the charges for a specific spell.
        /// </summary>
        public fixed int SpellCharges[5];
        /// <summary>
        /// Represents a compact representation of 32 Boolean flags that can be efficiently set or cleared.
        /// </summary>
        public BitVector32 DynamicFlags;
        /// <summary>
        /// Represents an array of integers that represents enchantments.
        /// The array has a fixed size of 39 elements.
        /// </summary>
        public fixed int Enchantment[39];
        ///<summary>
        /// Gets or sets the seed value for the property.
        ///</summary>
        public int PropertySeed;
        /// <summary>
        /// Gets or sets the ID of the random properties.
        /// </summary>
        public int RandomPropertiesId;
        /// <summary>
        /// Gets or sets the durability of the object.
        /// </summary>
        public int Durability;
        /// <summary>
        /// Gets or sets the maximum durability value.
        /// </summary>
        public int MaxDurability;
        /// <summary>
        /// This property represents the amount of time a player has spent playing a game.
        /// </summary>
        public int CreatePlayedTime;
        /// <summary>
        /// Represents the mask used to determine the modifiers applied to an object. 
        /// </summary>
        public int ModifiersMask;
    }
}