using System.Runtime.InteropServices;

namespace AmeisenBotX.MPQ.Dbc.Records
{
    [StructLayout(LayoutKind.Explicit)]
    public struct GameObjectRecord
    {
        [FieldOffset(0)]
        public uint Id;

        [FieldOffset(8)]
        public uint DisplayId;

        [FieldOffset(12)]
        public uint NameOffset;
    }
}
