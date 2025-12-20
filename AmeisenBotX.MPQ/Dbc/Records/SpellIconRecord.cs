using System.Runtime.InteropServices;

namespace AmeisenBotX.MPQ.Dbc.Records
{
    [StructLayout(LayoutKind.Explicit)]
    public struct SpellIconRecord
    {
        [FieldOffset(0)]
        public uint Id;

        [FieldOffset(4)]
        public uint PathOffset;
    }
}
