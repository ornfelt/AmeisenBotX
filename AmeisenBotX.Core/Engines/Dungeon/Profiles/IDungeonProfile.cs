using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Dungeon.Enums;
using AmeisenBotX.Core.Engines.Dungeon.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Dungeon.Profiles
{
    /// <summary>
    /// Interface representing a dungeon profile.
    /// </summary>
    public interface IDungeonProfile
    {
        /// <summary>
        /// Gets the author of the code.
        /// </summary>
        string Author { get; }

        /// <summary>
        /// Gets the description of the string object.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the position of the dungeon exit as a <see cref="Vector3"/>.
        /// </summary>
        Vector3 DungeonExit { get; }

        /// <summary>
        /// Gets the faction type of the Dungeon.
        /// </summary>
        DungeonFactionType FactionType { get; }

        /// <summary>
        /// Gets the size of the group.
        /// </summary>
        int GroupSize { get; }

        /// <summary>
        /// Gets the map ID of the WowMapId.
        /// </summary>
        WowMapId MapId { get; }

        /// <summary>
        /// Gets the maximum level.
        /// </summary>
        int MaxLevel { get; }

        /// <summary>
        /// Gets the value of the Name property.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets or sets the list of DungeonNode objects.
        /// </summary>
        List<DungeonNode> Nodes { get; }

        /// <summary>
        /// Gets the list of priority units.
        /// </summary>
        List<int> PriorityUnits { get; }

        /// <summary>
        /// Gets the required item level.
        /// </summary>
        int RequiredItemLevel { get; }

        /// <summary>
        /// Gets the required level.
        /// </summary>
        int RequiredLevel { get; }

        /// <summary>
        /// Gets the world entry vector in 3D space.
        /// </summary>
        Vector3 WorldEntry { get; }

        /// <summary>
        /// Gets the map ID of the world entry.
        /// </summary>
        WowMapId WorldEntryMapId { get; }
    }
}