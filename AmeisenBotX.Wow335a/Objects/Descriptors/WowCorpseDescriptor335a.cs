using System.Runtime.InteropServices;

namespace AmeisenBotX.Wow335a.Objects.Descriptors
{
    /// <summary>
    /// Represents the descriptor of a corpse in World of Warcraft.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct WowCorpseDescriptor335a
    {
        /// <summary>
        /// Represents the owner of the object as a 64-bit unsigned integer value.
        /// </summary>
        public ulong Owner;
        /// <summary>
        /// Represents the party's identifier as an unsigned long integer.
        /// </summary>
        public ulong Party;
        /// <summary>
        /// Represents the unique identifier used to display the object.
        /// </summary>
        public int DisplayId;
        /// <summary>
        /// Represents an array of fixed-size items with 19 elements.
        /// </summary>
        public fixed int Items[19];
        /// <summary>
        /// Represents the first four bytes of a corpse in fixed memory.
        /// </summary>
        public fixed byte CorpseBytes0[4];
        /// <summary>
        /// Represents a fixed-size array of 4 bytes used for storing corpse data.
        /// </summary>
        public fixed byte CorpseBytes1[4];
        /// <summary>
        /// Represents the guild identifier.
        /// </summary>
        public int Guild;
        /// <summary>
        /// Represents a public integer variable called Flags.
        /// </summary>
        public int Flags;
        /// <summary>
        /// Gets or sets the dynamic flags value.
        /// </summary>
        public int DynamicFlags;
        /// <summary>
        /// Represents the number of Wow Corpse Pad instances.
        /// </summary>
        public int WowCorpsePad;

        /// <summary>
        /// The end offset.
        /// </summary>
        public static readonly int EndOffset = 120;
    }
}