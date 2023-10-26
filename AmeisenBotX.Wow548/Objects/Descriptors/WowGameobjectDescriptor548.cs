using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace AmeisenBotX.Wow548.Objects.Descriptors
{
    /// <summary>
    /// Represents a descriptor for a WoW game object with specific properties.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct WowGameobjectDescriptor548
    {
        /// <summary>
        /// Gets or sets the user ID of the individual who created the object.
        /// </summary>
        public ulong CreatedBy;
        /// <summary>
        /// Represents the ID used for displaying a certain value.
        /// </summary>
        public int DisplayId;
        /// <summary>
        /// Gets or sets a BitVector32 that represents a collection of flags.
        /// </summary>
        public BitVector32 Flags;
        /// <summary>
        /// Gets or sets the rotation of the parent object in a fixed array format.
        /// The array contains four floats representing the rotation in a quaternion format.
        /// </summary>
        public fixed float ParentRotation[4];
        /// <summary>
        /// Represents the id of a faction template.
        /// </summary>
        public int FactionTemplate;
        /// <summary>
        /// Represents the level of the object.
        /// </summary>
        public int Level;
        /// <summary>
        /// Represents the percentage of health. 
        /// </summary>
        public int PercentHealth;
        /// <summary>
        /// Gets or sets the spell visual ID for the state.
        /// </summary>
        public int StateSpellVisualId;
    }
}