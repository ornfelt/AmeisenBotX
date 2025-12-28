using System.Runtime.InteropServices;

namespace AmeisenBotX.WowMop.Objects.Descriptors
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct WowDynamicobjectDescriptor548
    {
        public ulong Caster;
        public int TypeAndVisualId;
        public int SpellId;
        public float Radius;
        public int CastTime;
    }
}
