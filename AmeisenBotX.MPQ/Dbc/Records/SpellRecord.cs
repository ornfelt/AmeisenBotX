using System.Runtime.InteropServices;

namespace AmeisenBotX.MPQ.Dbc.Records
{
    [StructLayout(LayoutKind.Explicit)]
    public struct SpellRecord
    {
        [FieldOffset(0)]
        public uint Id;

        [FieldOffset(532)]
        public uint SpellIconID;

        [FieldOffset(544)]
        public uint NameOffset;
    }
}
