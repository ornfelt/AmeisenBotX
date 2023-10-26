using System.Runtime.InteropServices;

namespace AmeisenBotX.Wow335a.Objects.Descriptors
{
    /// <summary>
    /// Represents a descriptor for a container in the World of Warcraft game.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct WowContainerDescriptor335a
    {
        /// <summary>
        /// Represents the number of available slots.
        /// </summary>
        public int SlotCount;
        /// <summary>
        /// Represents a fixed-size byte array used for padding in the WowContainer class.
        /// </summary>
        public fixed byte WowContainerPad[4];
        /// <summary>
        /// Represents an array of 36 fixed-length slots.
        /// </summary>
        public fixed long Slots[36];

        /// <summary>
        /// Represents the end offset value, which is a constant integer.
        /// </summary>
        public static readonly int EndOffset = 296;
    }
}