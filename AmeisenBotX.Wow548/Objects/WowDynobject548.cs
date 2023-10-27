using AmeisenBotX.Common.Math;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow548.Objects.Descriptors;

namespace AmeisenBotX.Wow548.Objects
{
    /// <summary>
    /// Represents a dynamic object in the Wow game that inherits from WowObject548 and implements the IWowDynobject interface.
    /// </summary>
    [Serializable]
    public unsafe class WowDynobject548 : WowObject548, IWowDynobject
    {
        /// <summary>
        /// Gets or sets the protected WowDynamicobjectDescriptor548 instance.
        /// </summary>
        protected WowDynamicobjectDescriptor548? DynamicobjectDescriptor;

        /// <summary>
        /// Gets the caster of the dynamic object descriptor.
        /// </summary>
        public ulong Caster => GetDynamicobjectDescriptor().Caster;

        /// <summary>
        /// Gets the position of the WoW dynamic object by reading the memory at the given base address offset, and returns the position if successful, otherwise returns a zero vector.
        /// </summary>
        public new Vector3 Position => Memory.Read(IntPtr.Add(BaseAddress, (int)Memory.Offsets.WowDynobjectPosition), out Vector3 position) ? position : Vector3.Zero;

        /// <summary>
        /// Gets the radius of the dynamic object descriptor.
        /// </summary>
        public float Radius => GetDynamicobjectDescriptor().Radius;

        /// <summary>
        /// Gets the SpellId from the DynamicobjectDescriptor.
        /// </summary>
        public int SpellId => GetDynamicobjectDescriptor().SpellId;

        /// <summary>
        /// Overrides the ToString() method to return a string representation of the DynamicObject instance.
        /// Includes the Guid, SpellId, Caster, and Radius.
        /// </summary>
        public override string ToString()
        {
            return $"DynamicObject: [{Guid}] SpellId: {SpellId} Caster: {Caster} Radius: {Radius}";
        }

        /// <summary>
        /// Calls the base Update method.
        /// </summary>
        public override void Update()
        {
            base.Update();
        }

        /// <summary>
        /// Retrieves the dynamic object descriptor associated with the specified WoW object.
        /// If the dynamic object descriptor is already assigned, it is returned.
        /// Otherwise, it is read from memory using the DescriptorAddress and sizeof(WowObjectDescriptor548) properties.
        /// If the read operation is successful, the retrieved descriptor is assigned to the DynamicobjectDescriptor property and returned.
        /// If the read operation fails, a new instance of WowDynamicobjectDescriptor548 is returned.
        /// </summary>
        /// <returns>
        /// The dynamic object descriptor associated with the WoW object.
        /// </returns>
        protected WowDynamicobjectDescriptor548 GetDynamicobjectDescriptor()
        {
            return DynamicobjectDescriptor ??= Memory.Read(DescriptorAddress + sizeof(WowObjectDescriptor548), out WowDynamicobjectDescriptor548 objPtr) ? objPtr : new();
        }
    }
}