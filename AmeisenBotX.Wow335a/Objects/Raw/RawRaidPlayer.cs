using System.Runtime.InteropServices;

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