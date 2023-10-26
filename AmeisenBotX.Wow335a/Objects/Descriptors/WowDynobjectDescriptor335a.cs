using System.Runtime.InteropServices;

namespace AmeisenBotX.Wow335a.Objects.Descriptors
{
    /// <summary>
    /// Represents a descriptor for a dynamic object in World of Warcraft (version 3.3.5a).
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct WowDynobjectDescriptor335a
    {
        /// <summary>
        /// Gets or sets the caster's value.
        /// </summary>
        public ulong Caster;
        /// <summary>
        /// Represents an array of 4 fixed bytes for the Dynobject.
        /// </summary>
        public fixed byte DynobjectBytes[4];
        ///<summary>
        /// Gets or sets the SpellId.
        ///</summary>
        public int SpellId;
        /// <summary>
        /// Represents the radius of a circle.
        /// </summary>
        public float Radius;
        /// <summary>
        /// Gets or sets the casting time of the spell.
        /// </summary>
        public int CastTime;

        /// <summary>
        /// Represents the end offset value, which is set to 24.
        /// </summary>
        public static readonly int EndOffset = 24;
    }
}