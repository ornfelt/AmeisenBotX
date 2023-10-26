using System.Runtime.InteropServices;

namespace AmeisenBotX.Wow548.Objects.Descriptors
{
    /// <summary>
    /// Represents a Wow Dynamicobject Descriptor 548.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct WowDynamicobjectDescriptor548
    {
        /// <summary>
        /// Gets or sets the caster value.
        /// </summary>
        public ulong Caster;
        /// <summary>
        /// Represents the visual ID associated with the type.
        /// </summary>
        public int TypeAndVisualId;
        /// <summary>
        /// Gets or sets the SpellId.
        /// </summary>
        public int SpellId;
        /// <summary>
        /// Gets or sets the radius of a circle.
        /// </summary>
        public float Radius;
        /// <summary>
        /// Gets or sets the cast time for an action.
        /// </summary>
        public int CastTime;
    }
}