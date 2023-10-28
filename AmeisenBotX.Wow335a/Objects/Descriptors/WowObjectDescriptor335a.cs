using System.Runtime.InteropServices;

/// <summary>
/// Represents a namespace for object descriptors in the World of Warcraft 3.3.5a version.
/// </summary>
namespace AmeisenBotX.Wow335a.Objects.Descriptors
{
    /// <summary>
    /// Represents a descriptor for a World of Warcraft object in the 3.3.5a version.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WowObjectDescriptor335a
    {
        /// <summary>
        /// Represents a globally unique identifier (GUID).
        /// </summary>
        public ulong Guid;
        /// <summary>
        /// Represents the type of the object.
        /// </summary>
        public int Type;
        /// <summary>
        /// The unique identifier for an entry.
        /// </summary>
        public int EntryId;
        /// <summary>
        /// Gets or sets the scale factor to be applied.
        /// </summary>
        public float Scale;
        /// <summary>
        /// Represents the padding value for the WowObject.
        /// </summary>
        public int WowObjectPad;

        /// <summary>
        /// Represents the end offset value.
        /// </summary>
        public static readonly int EndOffset = 24;
    }
}