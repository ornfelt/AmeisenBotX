using System.Runtime.InteropServices;

/// <summary>
/// Represents a structure that describes a WoW game object.
/// </summary>
namespace AmeisenBotX.Wow335a.Objects.Descriptors
{
    /// <summary>
    /// Represents a structure that describes a WoW game object.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct WowGameobjectDescriptor335a
    {
        /// <summary>
        /// Gets or sets the ID of the user who created the object.
        /// </summary>
        public ulong CreatedBy;
        /// <summary>
        /// Gets or sets the display id.
        /// </summary>
        public int DisplayId;
        /// <summary>
        /// Represents a set of flags used for various purposes.
        /// </summary>
        public int Flags;
        /// <summary>
        /// The array of floating-point values representing the parent rotations.
        /// </summary>
        public fixed float ParentRotations[4];
        /// <summary>
        /// Gets or sets an array of fixed-size shorts representing the dynamics values.
        /// </summary>
        public fixed short Dynamics[2];
        /// <summary>
        /// Represents the faction associated with an object.
        /// </summary>
        public int Faction;
        /// <summary>
        /// Gets or sets the level value.
        /// </summary>
        public int Level;
        /// <summary>
        /// Public field representing the byte value for GameobjectBytes0.
        /// </summary>
        public byte GameobjectBytes0;
        /// <summary>
        /// The byte representation of the GameobjectBytes1.
        /// </summary>
        public byte GameobjectBytes1;
        /// <summary>
        /// Represents the byte array for the GameObjectBytes2 property.
        /// </summary>
        public byte GameobjectBytes2;
        /// <summary>
        /// The game object's bytes in a byte array format.
        /// </summary>
        public byte GameobjectBytes3;

        /// <summary>
        /// Represents the end offset value.
        /// </summary>
        public static readonly int EndOffset = 48;
    }
}