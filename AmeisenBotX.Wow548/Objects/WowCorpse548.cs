using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow548.Objects.Descriptors;

/// <summary>
/// Namespace containing objects related to corpses in World of Warcraft version 5.4.8.
/// </summary>
namespace AmeisenBotX.Wow548.Objects
{
    /// <summary>
    /// Represents a corpse in the game with a specific descriptor.
    /// </summary>
    [Serializable]
    public unsafe class WowCorpse548 : WowObject548, IWowCorpse
    {
        /// <summary>
        /// Gets or sets the WowCorpseDescriptor object representing the corpse descriptor for WowCorpseDescriptor548.
        /// </summary>
        protected WowCorpseDescriptor548? CorpseDescriptor;

        /// <summary>
        /// Retrieves the display ID of the corpse descriptor associated with this object and returns it as an integer.
        /// </summary>
        public int DisplayId => GetCorpseDescriptor().DisplayId;

        /// <summary>
        /// Gets the owner of the corpse descriptor.
        /// </summary>
        public ulong Owner => GetCorpseDescriptor().Owner;

        /// <summary>
        /// Gets the party value of the corpse descriptor.
        /// </summary>
        public ulong Party => GetCorpseDescriptor().PartyGuid;

        /// <summary>
        /// Returns a string representation of the Corpse object.
        /// The string includes the unique identifier of the corpse, the owner's name, the party it belongs to, and its display ID.
        /// </summary>
        /// <returns>A string representation of the Corpse object.</returns>
        public override string ToString()
        {
            return $"Corpse: [{Guid}] Owner: {Owner} Party: {Party} DisplayId: {DisplayId}";
        }

        /// <summary>
        /// Updates the object by calling the base Update method.
        /// </summary>
        public override void Update()
        {
            base.Update();
        }

        /// <summary>
        /// Returns the WowCorpseDescriptor548 object associated with the current instance. If the object is null, it will be assigned the value returned by Memory.Read method with the specified address. If the memory read operation fails, a new WowCorpseDescriptor548 object will be created and returned.
        /// </summary>
        protected WowCorpseDescriptor548 GetCorpseDescriptor()
        {
            return CorpseDescriptor ??= Memory.Read(DescriptorAddress + sizeof(WowObjectDescriptor548), out WowCorpseDescriptor548 objPtr) ? objPtr : new();
        }
    }
}