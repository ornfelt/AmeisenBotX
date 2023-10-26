using System.Runtime.InteropServices;

namespace AmeisenBotX.Wow548.Objects.Descriptors
{
    /// <summary>
    /// Represents a container descriptor for the "WowContainerDescriptor548" struct.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct WowContainerDescriptor548
    {
        /// <summary>
        /// Gets or sets the array of 72 fixed slots.
        /// </summary>
        public fixed int Slots[72];
        /// <summary>
        /// Represents the number of available slots.
        /// </summary>
        public int NumSlots;
    }
}