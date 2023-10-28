using System.Collections.Specialized;
using System.Runtime.InteropServices;

/// <summary>
/// Represents a namespace for World of Warcraft object descriptors.
/// </summary>
namespace AmeisenBotX.Wow548.Objects.Descriptors
{
    /// <summary>
    /// Represents a descriptor for a World of Warcraft object.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct WowObjectDescriptor548
    {
        /// <summary>
        /// Represents a globally unique identifier (GUID).
        /// </summary>
        public ulong Guid;
        /// <summary>
        /// Represents a public ulong property named Data.
        /// </summary>
        public ulong Data;
        /// <summary>
        /// Gets or sets the type of the variable.
        /// </summary>
        public int Type;
        /// <summary>
        /// Gets or sets the entry ID.
        /// </summary>
        public int EntryId;
        /// <summary>
        /// Gets or sets a compact representation of a collection of boolean flags.
        /// </summary>
        public BitVector32 DynamicFlags;
        /// <summary>
        /// The scale of the object.
        /// </summary>
        public float Scale;
    }
}