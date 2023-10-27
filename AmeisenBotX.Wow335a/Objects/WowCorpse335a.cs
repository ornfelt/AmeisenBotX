using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow335a.Objects.Descriptors;
using System;

namespace AmeisenBotX.Wow335a.Objects
{
    /// <summary>
    /// Represents a corpse in the World of Warcraft game. Implements the IWowCorpse interface.
    /// </summary>
    [Serializable]
    public class WowCorpse335a : WowObject335a, IWowCorpse
    {
        /// <summary>
        /// Gets the DisplayId of the RawWowCorpse.
        /// </summary>
        public int DisplayId => RawWowCorpse.DisplayId;

        /// <summary>
        /// Gets the owner of the raw wow corpse as an unsigned long value.
        /// </summary>
        public ulong Owner => RawWowCorpse.Owner;

        /// <summary>
        /// Gets the party associated with the RawWowCorpse.
        /// </summary>
        public ulong Party => RawWowCorpse.Party;

        /// <summary>
        /// Gets or sets the raw WowCorpseDescriptor335a object for the protected WowCorpseDescriptor335a property.
        /// </summary>
        protected WowCorpseDescriptor335a RawWowCorpse { get; private set; }

        /// <summary>
        /// Returns a string representation of the Corpse object.
        /// The string includes the Corpse's Guid, Owner, Party, and DisplayId.
        /// </summary>
        public override string ToString()
        {
            return $"Corpse: [{Guid}] Owner: {Owner} Party: {Party} DisplayId: {DisplayId}";
        }

        /// <summary>
        /// Overrides the base Update method. Reads the WowCorpseDescriptor335a object from memory at the specified descriptor address and assigns it to the RawWowCorpse property.
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (Memory.Read(DescriptorAddress + WowObjectDescriptor335a.EndOffset, out WowCorpseDescriptor335a obj))
            {
                RawWowCorpse = obj;
            }
        }
    }
}