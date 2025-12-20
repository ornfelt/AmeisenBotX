using System.Runtime.InteropServices;

namespace AmeisenBotX.MPQ.Dbc.Records
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ItemDisplayInfoRecord
    {
        [FieldOffset(0)]
        public uint Id;

        [FieldOffset(20)]
        public uint InventoryIconOffset;
    }
}
