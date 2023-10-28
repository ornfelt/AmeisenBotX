using System.Collections.Specialized;
using System.Runtime.InteropServices;

/// <summary>
/// Represents a descriptor for a World of Warcraft corpse with specific attributes.
/// </summary>
namespace AmeisenBotX.Wow548.Objects.Descriptors
{
    /// <summary>
    /// Represents a descriptor for a World of Warcraft corpse with specific attributes.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct WowCorpseDescriptor548
    {
        /// <summary>
        /// Gets or sets the owner of the object.
        /// </summary>
        public ulong Owner;
        /// <summary>
        /// Gets or sets the unique identifier for a party.
        /// </summary>
        public ulong PartyGuid;
        /// <summary>
        /// The id of the display.
        /// </summary>
        public int DisplayId;
        /// <summary>
        /// Array of fixed size to store 19 items of type int.
        /// </summary>
        public fixed int Items[19];
        /// <summary>
        /// Gets or sets the Skin Id.
        /// </summary>
        public int SkinId;
        /// <summary>
        /// Represents the identifier of the facial hair style.
        /// </summary>
        public int FacialHairStyleId;
        /// <summary>
        /// Represents a bit vector that can store 32 Boolean values.
        /// </summary>
        public BitVector32 Flags;
        /// <summary>
        /// Represents a vector of 32 Boolean flags that can be dynamically modified.
        /// </summary>
        public BitVector32 DynamicFlags;
    }
}