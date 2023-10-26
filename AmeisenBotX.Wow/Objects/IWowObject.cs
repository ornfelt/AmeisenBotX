using AmeisenBotX.Common.Math;
using AmeisenBotX.Wow.Objects.Enums;
using System;

namespace AmeisenBotX.Wow.Objects
{
    /// <summary>
    /// Represents an object in the game world.
    /// </summary>
    /// <typeparam name="IntPtr">The base address of the object.</typeparam>
    /// <typeparam name="IntPtr">The descriptor address of the object.</typeparam>
    /// <typeparam name="int">The entry ID of the object.</typeparam>
    /// <typeparam name="ulong">The unique identifier of the object.</typeparam>
    /// <typeparam name="Vector3">The position of the object.</typeparam>
    /// <typeparam name="float">The scale of the object.</typeparam>
    /// <typeparam name="WowObjectType">The type of the object.</typeparam>
    /// <remarks>
    /// This interface provides properties and methods to interact with an object in the game world.
    /// </remarks>
    public interface IWowObject
    {
        /// <summary>
        /// Gets the base address of the IntPtr object.
        /// </summary>
        IntPtr BaseAddress { get; }

        /// <summary>
        /// Gets the address of the descriptor.
        /// </summary>
        IntPtr DescriptorAddress { get; }

        /// <summary>
        /// Gets the entry ID.
        /// </summary>
        int EntryId { get; }

        /// <summary>
        /// Gets the GUID of the object as an unsigned long integer.
        /// </summary>
        ulong Guid { get; }

        /// <summary>
        /// Gets the position of the Vector3 object.
        /// </summary>
        Vector3 Position { get; }

        /// <summary>
        /// Gets the scale value.
        /// </summary>
        float Scale { get; }

        /// <summary>
        /// Gets the type of WowObject.
        /// </summary>
        public WowObjectType Type => WowObjectType.None;

        /// <summary>
        /// Calculates the distance between the current object's position and the position of another object.
        /// </summary>
        /// <param name="b">The other object to calculate the distance to.</param>
        /// <returns>The distance between the current object and the specified object.</returns>
        public float DistanceTo(IWowObject b)
        {
            return Position.GetDistance(b.Position);
        }

        /// <summary>
        /// Calculates the distance between the current vector and the specified vector.
        /// </summary>
        /// <param name="b">The vector to calculate the distance to.</param>
        /// <returns>The distance between the two vectors.</returns>
        public float DistanceTo(Vector3 b)
        {
            return Position.GetDistance(b);
        }

        /// <summary>
        /// Initializes the WowMemoryApi with the specified memory, base address, and descriptor address.
        /// </summary>
        void Init(WowMemoryApi memory, IntPtr baseAddress, IntPtr descriptorAddress);

        /// <summary>
        /// Checks if the object is a container.
        /// </summary>
        public bool IsContainer()
        {
            return Type == WowObjectType.Container;
        }

        /// <summary>
        /// Checks if the object is a corpse.
        /// </summary>
        public bool IsCorpse()
        {
            return Type == WowObjectType.Corpse;
        }

        /// <summary>
        /// Determines if the object is a Dynoject.
        /// </summary>
        public bool IsDynoject()
        {
            return Type == WowObjectType.DynamicObject;
        }

        ///<summary>
        ///Checks if the type of the object is a GameObject.
        ///</summary>
        public bool IsGameobject()
        {
            return Type == WowObjectType.GameObject;
        }

        /// <summary>
        /// Determines if the distance to the given IWowObject is within the specified range.
        /// </summary>
        /// <param name="b">The IWowObject to compare the distance with.</param>
        /// <param name="range">The range to check against the distance.</param>
        /// <returns><c>true</c> if the distance to the given IWowObject is less than the specified range, otherwise <c>false</c>.</returns>
        public bool IsInRange(IWowObject b, float range)
        {
            return DistanceTo(b) < range;
        }

        /// <summary>
        /// Determines if the given vector is within the specified range from the current vector.
        /// </summary>
        /// <param name="b">The vector to compare with.</param>
        /// <param name="range">The range within which the comparison is evaluated.</param>
        /// <returns>True if the given vector is within the specified range, otherwise false.</returns>
        public bool IsInRange(Vector3 b, float range)
        {
            return DistanceTo(b) < range;
        }

        /// <summary>
        /// Determines if the object is a player.
        /// </summary>
        public bool IsPlayer()
        {
            return Type == WowObjectType.Player;
        }

        /// <summary>
        /// Returns a boolean value indicating whether the object is a unit.
        /// </summary>
        public bool IsUnit()
        {
            return Type == WowObjectType.Unit;
        }

        /// <summary>
        /// Updates the current state of the system.
        /// </summary>
        void Update();
    }
}