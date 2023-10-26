using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Specialized;

namespace AmeisenBotX.Wow.Objects
{
    /// <summary>
    /// Represents a World of Warcraft game object.
    /// </summary>
    public interface IWowGameobject : IWowObject
    {
        /// <summary>
        /// Gets or sets the value of the Bytes0 property.
        /// </summary>
        byte Bytes0 { get; }

        /// <summary>
        /// Gets the ID of the user who created the object.
        /// </summary>
        ulong CreatedBy { get; }

        /// <summary>
        /// Gets the value of the DisplayId property.
        /// </summary>
        int DisplayId { get; }

        /// <summary>
        /// Gets the faction of the object.
        /// </summary>
        int Faction { get; }

        /// <summary>
        /// Gets or sets the flags stored in this BitVector32.
        /// </summary>
        BitVector32 Flags { get; }

        /// <summary>
        /// Gets the type of the game object.
        /// </summary>
        WowGameObjectType GameObjectType { get; }

        /// <summary>
        /// Gets the level value.
        /// </summary>
        int Level { get; }

        /// <summary>
        /// Gets or sets the type of the WoW object, which is a GameObject.
        /// </summary>
        public new WowObjectType Type => WowObjectType.GameObject;
    }
}