using System.Runtime.InteropServices;

namespace AmeisenBotX.MPQ.Dbc.Records
{
    [StructLayout(LayoutKind.Explicit)]
    public struct CreatureRecord
    {
        [FieldOffset(0)]
        public uint Id;

        [FieldOffset(4)]
        public uint NameOffset;
    }
}
