using System.Runtime.InteropServices;

/// <summary>
/// Represents a namespace for raw objects related to the World of Warcraft 3.3.5a version.
/// </summary>
namespace AmeisenBotX.Wow335a.Objects.Raw
{
    /// <summary>
    /// Represents a raw RAID player.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct RawRaidPlayer
    {
        /// <summary>
        /// Gets or sets the GUID value.
        /// </summary>
        public ulong Guid { get; set; }

        /// <summary>
        /// Represents a fixed array of 9 unsigned long integers used for padding.
        /// </summary>
        public fixed ulong Padding[9];
    }
}