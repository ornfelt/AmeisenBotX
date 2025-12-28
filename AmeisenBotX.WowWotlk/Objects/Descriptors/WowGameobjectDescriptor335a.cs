using System.Runtime.InteropServices;

namespace AmeisenBotX.WowWotlk.Objects.Descriptors
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct WowGameobjectDescriptor335a
    {
        public ulong CreatedBy;
        public int DisplayId;
        public int Flags;
        public fixed float ParentRotations[4];
        public short DynamicFlagsLow;   // Contains sparkle flag (0x08)
        public short DynamicFlagsHigh;
        public int Faction;
        public int Level;
        public byte GameobjectBytes0;
        public byte GameobjectBytes1;
        public byte GameobjectBytes2;
        public byte GameobjectBytes3;

        public static readonly int EndOffset = 48;
    }
}
