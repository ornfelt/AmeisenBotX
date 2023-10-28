using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Objects.Enums;
using AmeisenBotX.Wow.Objects.Enums;

/// <summary>
/// Contains core objects used in the AmeisenBotX project.
/// </summary>
namespace AmeisenBotX.Core.Objects
{
    /// <summary>
    /// Represents a non-playable character (NPC) in the game.
    /// </summary>
    public class Npc
    {
        /// <summary>
        /// Gets the entry ID.
        /// </summary>
        public readonly int EntryId;
        /// <summary>
        /// Gets the subtype of the NPC.
        /// </summary>
        public readonly NpcSubType SubType;
        /// <summary>
        /// Gets the type of the Non-Player Character (NPC).
        /// </summary>
        public readonly NpcType Type;
        /// <summary>
        /// The identifier for the WowMap.
        /// </summary>
        public WowMapId MapId;
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name;
        /// <summary>
        /// Represents the position of an object in 3D space.
        /// </summary>
        public Vector3 Position;
        /// <summary>
        /// The ID of the WowZone.
        /// </summary>
        public WowZoneId ZoneId;

        /// <summary>
        /// Initializes a new instance of the Npc class with the specified parameters.
        /// </summary>
        /// <param name="name">The name of the NPC.</param>
        /// <param name="entryId">The ID of the NPC's entry.</param>
        /// <param name="mapId">The map ID where the NPC is located.</param>
        /// <param name="zoneId">The zone ID where the NPC is located.</param>
        /// <param name="position">The position of the NPC.</param>
        /// <param name="type">The type of the NPC.</param>
        /// <param name="subType">The subtype of the NPC. (Optional)</param>
        public Npc(string name, int entryId, WowMapId mapId, WowZoneId zoneId, Vector3 position,
                    NpcType type, NpcSubType subType = NpcSubType.None)
        {
            Name = name;
            EntryId = entryId;
            MapId = mapId;
            ZoneId = zoneId;
            Position = position;
            Type = type;
            SubType = subType;
        }
    }
}