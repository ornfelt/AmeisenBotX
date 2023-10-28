using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Objects.Enums;
using AmeisenBotX.Wow.Objects.Enums;

/// <summary>
/// Represents an interactable object in the game.
/// </summary>
namespace AmeisenBotX.Core.Objects
{
    /// <summary>
    /// Represents an interactable object in the game.
    /// </summary>
    public class InteractableObject
    {
        /// <summary>
        /// Gets the faction type of the mailbox.
        /// </summary>
        public readonly MailboxFactionType FactionType;
        /// <summary>
        /// Gets the type of the interactable object.
        /// </summary>
        public readonly InteractableObjectType ObjectType;
        /// <summary>
        /// Gets or sets the entry ID.
        /// </summary>
        public int EntryId;
        /// <summary>
        /// A public property representing the WowMapId for the current map.
        /// </summary>
        public WowMapId MapId;
        /// <summary>
        /// The position of the object in 3D space.
        /// </summary>
        public Vector3 Position;
        /// <summary>
        /// Represents the ZoneId of the WowZone.
        /// </summary>
        public WowZoneId ZoneId;

        /// <summary>
        /// Constructor for an InteractableObject object.
        /// </summary>
        /// <param name="entryId">The unique identifier for the object.</param>
        /// <param name="mapId">The map identifier for the object.</param>
        /// <param name="zoneId">The zone identifier for the object.</param>
        /// <param name="position">The position of the object in the game world.</param>
        /// <param name="objectType">The type of the object.</param>
        /// <param name="factionType">The faction type of the object (optional, defaults to None).</param>
        public InteractableObject(int entryId, WowMapId mapId, WowZoneId zoneId, Vector3 position,
                    InteractableObjectType objectType, MailboxFactionType factionType = MailboxFactionType.None)
        {
            EntryId = entryId;
            MapId = mapId;
            ZoneId = zoneId;
            Position = position;
            ObjectType = objectType;
            FactionType = factionType;
        }
    }
}