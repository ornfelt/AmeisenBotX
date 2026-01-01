using System.Runtime.InteropServices;

namespace AmeisenBotX.MPQ.Dbc.Records
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WorldMapAreaRecord
    {
        public uint Id;
        public uint MapId;
        public uint AreaId;
        public uint AreaNameOffset;
        public float LocLeft;
        public float LocRight;
        public float LocTop;
        public float LocBottom;
        public int DisplayMapId;
        public int DefaultDungeonFloor;
        public int ParentWorldMapId;
    }
}
