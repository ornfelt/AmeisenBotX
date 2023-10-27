using AmeisenBotX.Common.Math;
using AmeisenBotX.Wow;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow335a.Objects.Descriptors;
using System;

namespace AmeisenBotX.Wow335a.Objects
{
    /// <summary>
    /// Gets the unique identifier of the RawObject.
    /// </summary>
    [Serializable]
    public class WowObject335a : IWowObject
    {
        /// <summary>
        /// Gets or sets the base address of the object.
        /// </summary>
        public IntPtr BaseAddress { get; private set; }

        /// <summary>
        /// Gets or sets the memory address of the descriptor.
        /// </summary>
        public IntPtr DescriptorAddress { get; private set; }

        /// <summary>
        /// Gets the entry ID of the raw object.
        /// </summary>
        public int EntryId => RawObject.EntryId;

        /// <summary>
        /// Gets the unique identifier of the RawObject.
        /// </summary>
        public ulong Guid => RawObject.Guid;

        /// <summary>
        /// Gets or sets the position of the object in 3D space.
        /// </summary>
        public Vector3 Position { get; protected set; }

        /// <summary>
        /// Gets the scale of the RawObject.
        /// </summary>
        public float Scale => RawObject.Scale;

        /// <summary>
        /// Gets or sets the type of the Wow object.
        /// </summary>
        public WowObjectType Type { get; protected set; }

        /// <summary>
        /// Gets or sets the protected WowMemoryApi object for accessing memory.
        /// </summary>
        protected WowMemoryApi Memory { get; private set; }

        /// <summary>
        /// Gets or sets the raw object descriptor for WowObject335a.
        /// </summary>
        protected WowObjectDescriptor335a RawObject { get; private set; }

        /// <summary>
        /// Initializes the WowMemoryApi with the given memory, base address, and descriptor address.
        /// </summary>
        public virtual void Init(WowMemoryApi memory, IntPtr baseAddress, IntPtr descriptorAddress)
        {
            Memory = memory;
            BaseAddress = baseAddress;
            DescriptorAddress = descriptorAddress;

            Update();
        }

        /// <summary>
        /// Converts the object to its string representation.
        /// </summary>
        /// <returns>A string that represents the object, with the format "Object: <Guid>"</returns>
        public override string ToString()
        {
            return $"Object: {Guid}";
        }

        /// <summary>
        /// Updates the current object by reading the data from the memory at the specified descriptor address.
        /// </summary>
        public virtual void Update()
        {
            if (DescriptorAddress != IntPtr.Zero && Memory.Read(DescriptorAddress, out WowObjectDescriptor335a obj))
            {
                RawObject = obj;
            }
        }
    }
}