using AmeisenBotX.Wow.Objects.Enums;

namespace AmeisenBotX.Wow.Objects
{
    /// <summary>
    /// Represents a World of Warcraft corpse object.
    /// </summary>
    public interface IWowCorpse : IWowObject
    {
        /// <summary>
        /// Gets the display ID.
        /// </summary>
        int DisplayId { get; }

        /// <summary>
        /// Gets the unique identifier of the owner.
        /// </summary>
        ulong Owner { get; }

        /// <summary>
        /// Gets or sets the party identifier, represented as an unsigned long.
        /// </summary>
        ulong Party { get; }

        /// <summary>
        /// Gets the type of the WowObjectType, which is set to Corpse.
        /// </summary>
        public new WowObjectType Type => WowObjectType.Corpse;
    }
}