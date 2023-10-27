using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow335a.Objects.Descriptors;
using System;

namespace AmeisenBotX.Wow335a.Objects
{
    [Serializable]
    public class WowContainer335a : WowObject335a, IWowContainer
    {
        /// <summary>
        /// Gets the number of slots in the WoW container.
        /// </summary>
        public int SlotCount => RawWowContainer.SlotCount;

        /// <summary>
        /// Gets or sets the raw WowContainerDescriptor335a for the protected WowContainer.
        /// </summary>
        protected WowContainerDescriptor335a RawWowContainer { get; private set; }

        /// <summary>
        /// Overrides the default ToString method to return a string representation of the object's properties.
        /// The returned string includes the container's GUID and the number of slots in the container.
        /// </summary>
        public override string ToString()
        {
            return $"Container: [{Guid}] SlotCount: {SlotCount}";
        }

        /// <summary>
        /// Updates the object by reading the descriptor address and setting the RawWowContainer field if successful.
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (Memory.Read(DescriptorAddress + WowObjectDescriptor335a.EndOffset, out WowContainerDescriptor335a obj))
            {
                RawWowContainer = obj;
            }
        }
    }
}