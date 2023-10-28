using AmeisenBotX.Common.Math;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow548.Objects.Descriptors;
using System.Collections.Specialized;
using System.Globalization;

/// <summary>
/// This namespace contains classes and interfaces related to WoW game objects for version 548.
/// </summary>
namespace AmeisenBotX.Wow548.Objects
{
    /// <summary>
    /// The WowGameobject548 class is a serialized representation of a WoW game object with identifier 548 and contains a descriptor for the object.
    /// </summary>
    [Serializable]
    public unsafe class WowGameobject548 : WowObject548, IWowGameobject
    {
        /// <summary>
        /// The descriptor for the WowGameobject548 object.
        /// </summary>
        /// <remarks>
        /// This descriptor provides information about the WowGameobject object with identifier 548.
        /// </remarks>
        protected WowGameobjectDescriptor548? GameobjectDescriptor;

        /// <summary>
        /// Gets or sets the value of the first byte.
        /// </summary>
        public byte Bytes0 { get; set; }

        /// <summary>
        /// Gets the ID of the user who created the GameObject Descriptor.
        /// </summary>
        public ulong CreatedBy => GetGameobjectDescriptor().CreatedBy;

        /// <summary>
        /// Gets the display ID of the game object descriptor.
        /// </summary>
        public int DisplayId => GetGameobjectDescriptor().DisplayId;

        /// <summary>
        /// Gets the faction of the game object.
        /// </summary>
        public int Faction => GetGameobjectDescriptor().FactionTemplate;

        /// <summary>
        /// Gets the flags of the GameObject Descriptor using a BitVector32.
        /// </summary>
        public BitVector32 Flags => GetGameobjectDescriptor().Flags;

        /// <summary>
        /// Gets or sets the type of the game object for the wow game.
        /// </summary>
        public WowGameObjectType GameObjectType { get; set; }

        /// <summary>
        /// Gets the level of the game object descriptor.
        /// </summary>
        public int Level => GetGameobjectDescriptor().Level;

        /// <summary>
        /// Gets the position of the WoW game object.
        /// If the position is successfully read from memory, returns the position.
        /// Otherwise, returns Vector3.Zero.
        /// </summary>
        public new Vector3 Position => Memory.Read(IntPtr.Add(BaseAddress, (int)Memory.Offsets.WowGameobjectPosition), out Vector3 position) ? position : Vector3.Zero;

        /// <summary>
        /// Returns a string representation of the GameObject, including its entry ID and display ID. If the display ID is a valid value from the WowGameObjectDisplayId enum, the enum value will be included in the string. Otherwise, the display ID will be represented as a numeric value.
        /// </summary>
        public override string ToString()
        {
            return $"GameObject: [{EntryId}] ({(Enum.IsDefined(typeof(WowGameObjectDisplayId), DisplayId) ? ((WowGameObjectDisplayId)DisplayId).ToString() : DisplayId.ToString(CultureInfo.InvariantCulture))}:{DisplayId})";
        }

        /// <summary>
        /// Overrides the Update method from the base class. 
        /// Updates the GameObjectType and Bytes0 based on the values of objPtr.
        /// </summary>
        public override void Update()
        {
            base.Update();

            // GameObjectType = (WowGameObjectType)objPtr.GameobjectBytes1; Bytes0 = objPtr.GameobjectBytes0;
        }

        /// <summary>
        /// Returns the game object descriptor associated with this WowGameobjectDescriptor548 object.
        /// If the GameobjectDescriptor property is null, it tries to read the descriptor from memory using the Memory.Read method.
        /// If successful, it returns the read WowGameobjectDescriptor548 object, otherwise it creates a new WowGameobjectDescriptor548 object.
        /// </summary>
        protected WowGameobjectDescriptor548 GetGameobjectDescriptor()
        {
            return GameobjectDescriptor ??= Memory.Read(DescriptorAddress + sizeof(WowObjectDescriptor548), out WowGameobjectDescriptor548 objPtr) ? objPtr : new();
        }
    }
}