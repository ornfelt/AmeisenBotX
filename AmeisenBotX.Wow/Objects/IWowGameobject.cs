using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Specialized;

namespace AmeisenBotX.Wow.Objects
{
    public interface IWowGameobject : IWowObject
    {
        byte Bytes0 { get; }

        ulong CreatedBy { get; }

        int DisplayId { get; }

        short DynamicFlags { get; }

        int Faction { get; }

        BitVector32 Flags { get; }

        WowGameObjectType GameObjectType { get; }

        bool IsSparkling { get; }

        int Level { get; }

        bool Locked { get; }

        string Name { get; }

        public new WowObjectType Type => WowObjectType.GameObject;
    }
}