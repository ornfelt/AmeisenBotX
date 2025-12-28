using AmeisenBotX.Common.Math;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.WowMop.Objects.Descriptors;
using System.Collections.Specialized;
using System.Globalization;

namespace AmeisenBotX.WowMop.Objects
{
    [Serializable]
    public unsafe class WowGameobject548 : WowObject548, IWowGameobject
    {
        private const short GO_DYNFLAG_SPARKLE = 0x08;

        protected WowGameobjectDescriptor548? GameobjectDescriptor;

        public byte Bytes0 { get; set; }

        public ulong CreatedBy => GetGameobjectDescriptor().CreatedBy;

        public int DisplayId => GetGameobjectDescriptor().DisplayId;

        public short DynamicFlags => GetGameobjectDescriptor().DynamicFlagsLow;

        public int Faction => GetGameobjectDescriptor().FactionTemplate;

        public BitVector32 Flags => GetGameobjectDescriptor().Flags;

        public WowGameObjectType GameObjectType { get; set; }

        public bool IsSparkling => (DynamicFlags & GO_DYNFLAG_SPARKLE) != 0;

        public bool Locked => Flags[1];

        public int Level => GetGameobjectDescriptor().Level;

        public new Vector3 Position => Memory.Read(nint.Add(BaseAddress, (int)Memory.Offsets.WowGameobjectPosition), out Vector3 position) ? position : Vector3.Zero;

        public string Name => throw new NotImplementedException();

        public override string ToString()
        {
            return $"GameObject: [{EntryId}] ({(Enum.IsDefined(typeof(WowGameObjectDisplayId), DisplayId) ? ((WowGameObjectDisplayId)DisplayId).ToString() : DisplayId.ToString(CultureInfo.InvariantCulture))}:{DisplayId}) Sparkle={IsSparkling}";
        }

        public override void Update()
        {
            base.Update();

            // GameObjectType = (WowGameObjectType)objPtr.GameobjectBytes1; Bytes0 = objPtr.GameobjectBytes0;
        }

        protected WowGameobjectDescriptor548 GetGameobjectDescriptor()
        {
            return GameobjectDescriptor ??= Memory.Read(DescriptorAddress + sizeof(WowObjectDescriptor548), out WowGameobjectDescriptor548 objPtr) ? objPtr : new();
        }
    }
}
