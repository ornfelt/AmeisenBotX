using AmeisenBotX.Common.Math;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow335a.Objects.Descriptors;
using System;

namespace AmeisenBotX.Wow335a.Objects
{
    /// <summary>
    /// Represents a dynamic object in the World of Warcraft game, with properties for the caster, radius, and spell ID.
    /// </summary>
    [Serializable]
    public class WowDynobject335a : WowObject335a, IWowDynobject
    {
        /// <summary>
        /// Gets or sets the Caster property as an unsigned long value.
        /// </summary>
        public ulong Caster { get; set; }

        /// <summary>
        /// Gets or sets the radius of an object.
        /// </summary>
        public float Radius { get; set; }

        /// <summary>
        /// Gets or sets the spell ID.
        /// </summary>
        public int SpellId { get; set; }

        /// <summary>
        /// Returns a string representation of the DynamicObject, including its Guid, SpellId, Caster, 
        /// and Radius properties.
        /// </summary>
        public override string ToString()
        {
            return $"DynamicObject: [{Guid}] SpellId: {SpellId} Caster: {Caster} Radius: {Radius}";
        }

        /// <summary>
        /// Updates the object's properties by reading from memory.
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (Memory.Read(DescriptorAddress + WowObjectDescriptor335a.EndOffset, out WowDynobjectDescriptor335a objPtr)
                && Memory.Read(IntPtr.Add(BaseAddress, (int)Memory.Offsets.WowDynobjectPosition), out Vector3 position))
            {
                Caster = objPtr.Caster;
                Radius = objPtr.Radius;
                SpellId = objPtr.SpellId;
                Position = position;
            }
        }
    }
}