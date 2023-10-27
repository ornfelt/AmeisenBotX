using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow548.Objects.Descriptors;

namespace AmeisenBotX.Wow548.Objects
{
    /// <summary>
    /// Returns a string representation of the Container object, including its Guid and SlotCount.
    /// </summary>
    [Serializable]
    public unsafe class WowContainer548 : WowObject548, IWowContainer
    {
        /// <summary>
        /// Gets or sets the protected instance of WowContainerDescriptor548.
        /// </summary>
        protected WowContainerDescriptor548? ContainerDescriptor;

        /// <summary>
        /// Returns the total count of slots in the container.
        /// </summary>
        public int SlotCount => GetContainerDescriptor().NumSlots;

        /// <summary>
        /// Returns a string representation of the Container object, including its Guid and SlotCount.
        /// </summary>
        public override string ToString()
        {
            return $"Container: [{Guid}] SlotCount: {SlotCount}";
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        public override void Update()
        {
            base.Update();
        }

        /// <summary>
        /// Gets the container descriptor for the WowContainerDescriptor548 object.
        /// If the ContainerDescriptor is not null, it returns the ContainerDescriptor.
        /// Otherwise, it reads the memory at DescriptorAddress + sizeof(WowObjectDescriptor548) using the Memory.Read method,
        /// and assigns the result to the ContainerDescriptor.
        /// If Memory.Read is successful and returns a valid reference to a WowContainerDescriptor548 object, that object is returned.
        /// Otherwise, a new instance of WowContainerDescriptor548 is created and returned.
        /// </summary>
        protected WowContainerDescriptor548 GetContainerDescriptor()
        {
            return ContainerDescriptor ??= Memory.Read(DescriptorAddress + sizeof(WowObjectDescriptor548), out WowContainerDescriptor548 objPtr) ? objPtr : new();
        }
    }
}