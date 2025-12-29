using AmeisenBotX.Common.Math;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.WowWotlk.Objects.Descriptors;
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;

namespace AmeisenBotX.WowWotlk.Objects
{
    [Serializable]
    public class WowGameobject335a : WowObject335a, IWowGameobject
    {
        private const short GO_DYNFLAG_SPARKLE = 0x08;

        public byte Bytes0 { get; set; }

        public ulong CreatedBy { get; set; }

        public int DisplayId { get; set; }

        public short DynamicFlags { get; set; }

        public int Faction { get; set; }

        public BitVector32 Flags { get; set; }

        public WowGameObjectType GameObjectType { get; set; }

        public bool IsSparkling => (DynamicFlags & GO_DYNFLAG_SPARKLE) != 0;

        // GameObject state flags
        private const int GO_FLAG_IN_USE = 0x01;         // Object is currently being used
        private const int GO_FLAG_NOT_SELECTABLE = 0x10; // Cannot be selected/interacted

        /// <summary>
        /// Whether the object can be interacted with (not in-use, not non-selectable).
        /// </summary>
        public bool IsUsable => !Flags[GO_FLAG_IN_USE] && !Flags[GO_FLAG_NOT_SELECTABLE];

        public int Level { get; set; }

        public bool Locked => Flags[2]; // 0x2 -> locked

        public string Name { get; private set; }

        public override string ToString()
        {
            return $"GameObject: [{EntryId}] ({(Enum.IsDefined(typeof(WowGameObjectDisplayId), DisplayId) ? ((WowGameObjectDisplayId)DisplayId).ToString() : DisplayId.ToString(CultureInfo.InvariantCulture))}:{DisplayId}) Sparkle={IsSparkling}";
        }

        public override void Update()
        {
            base.Update();

            if (Memory.Read(DescriptorAddress + WowObjectDescriptor335a.EndOffset, out WowGameobjectDescriptor335a objPtr)
                && Memory.Read(nint.Add(BaseAddress, (int)Memory.Offsets.WowGameobjectPosition), out Vector3 position))
            {
                GameObjectType = (WowGameObjectType)objPtr.GameobjectBytes1;
                CreatedBy = objPtr.CreatedBy;
                Bytes0 = objPtr.GameobjectBytes0;
                DisplayId = objPtr.DisplayId;
                DynamicFlags = objPtr.DynamicFlagsLow;
                Faction = objPtr.Faction;
                Flags = new(objPtr.Flags);
                Level = objPtr.Level;
                Position = position;

                Name = Memory.Read(nint.Add(BaseAddress, 0x1A4), out nint cachePtr)
                    && Memory.Read(nint.Add(cachePtr, 0x90), out nint namePtr)
                    && Memory.ReadString(namePtr, Encoding.UTF8, out string n)
                    ? n
                    : "Unknown";
            }
        }
    }
}
