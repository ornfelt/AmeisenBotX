using AmeisenBotX.Common.Math;
using AmeisenBotX.Wow;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow548.Objects.Descriptors;
using System.Collections.Specialized;

namespace AmeisenBotX.Wow548.Objects
{
    /// <summary>
    /// Represents a WowObject548 that implements the IWowObject interface.
    /// </summary>
    [Serializable]
    public class WowObject548 : IWowObject
    {
        /// <summary>
        /// Gets or sets the WowObjectDescriptor548 object for this ProtectedObject.
        /// </summary>
        protected WowObjectDescriptor548? ObjectDescriptor;

        /// <summary>
        /// Gets or sets the base address of the object represented by an IntPtr.
        /// </summary>
        public IntPtr BaseAddress { get; private set; }

        /// <summary>
        /// Gets or sets the address of the descriptor.
        /// </summary>
        public IntPtr DescriptorAddress { get; private set; }

        /// <summary>
        /// Gets the entry ID of the object descriptor.
        /// </summary>
        public int EntryId => GetObjectDescriptor().EntryId;

        /// <summary>
        /// Gets the GUID of the object descriptor obtained from the GetObjectDescriptor method.
        /// </summary>
        public ulong Guid => GetObjectDescriptor().Guid;

        ///<summary>
        ///Gets or sets the position of the object in 3D space.
        ///</summary>
        public Vector3 Position { get; protected set; }

        /// <summary>
        /// Gets the scale of the object descriptor.
        /// </summary>
        public float Scale => GetObjectDescriptor().Scale;

        /// <summary>
        /// Gets the BitVector32 representing the dynamic flags of the unit.
        /// </summary>
        public BitVector32 UnitFlagsDynamic => GetObjectDescriptor().DynamicFlags;

        /// <summary>
        /// Gets or sets the protected WowMemoryApi object representing the memory.
        /// </summary>
        protected WowMemoryApi Memory { get; private set; }

        /// <summary>
        /// Initializes the object with the specified memory, base address, and descriptor address.
        /// </summary>
        /// <param name="memory">The WowMemoryApi object used for memory operations.</param>
        /// <param name="baseAddress">The base address of the object.</param>
        /// <param name="descriptorAddress">The address of the descriptor.</param>
        public virtual void Init(WowMemoryApi memory, IntPtr baseAddress, IntPtr descriptorAddress)
        {
            Memory = memory;
            BaseAddress = baseAddress;
            DescriptorAddress = descriptorAddress;

            Update();
        }

        /// <summary>
        /// The method performs an update operation.
        /// </summary>
        public virtual void Update()
        {
        }

        ///<summary>Returns the object descriptor for the specified object.</summary>
        protected WowObjectDescriptor548 GetObjectDescriptor()
        {
            return ObjectDescriptor ??= Memory.Read(DescriptorAddress, out WowObjectDescriptor548 objPtr) ? objPtr : new();
        }
    }
}