using System.Runtime.InteropServices;

namespace AmeisenBotX.WowWotlk.Objects.Descriptors
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct WowContainerDescriptor335a
    {
        public int SlotCount;
        public fixed byte WowContainerPad[4];
        public fixed long Slots[36];

        public static readonly int EndOffset = 296;
    }
}
