using AmeisenBotX.Wow.Objects.Enums;

namespace AmeisenBotX.Wow.Objects
{
    /// <summary>
    /// Represents a container in the World of Warcraft game.
    /// </summary>
    public interface IWowContainer : IWowObject
    {
        /// <summary>
        /// Gets the total count of slots.
        /// </summary>
        int SlotCount { get; }

        /// <summary>
        /// Gets the type of object, which is a container. 
        /// </summary>
        public new WowObjectType Type => WowObjectType.Container;
    }
}