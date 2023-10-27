using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Dungeon.Enums;
using AmeisenBotX.Core.Engines.Dungeon.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Dungeon.Profiles.Classic
{
    /// <summary>
    /// Profile for the Deadmines Dungeon in Westfall, made for Level 15 to 18.
    /// </summary>
    public class DeadminesProfile : IDungeonProfile
    {
        /// <summary>
        /// The name of the Defias Gunpowder.
        /// </summary>
        private const string defiasGunPowderName = "Defias Gunpowder";

        /// <summary>
        /// Gets the author of the code.
        /// </summary>
        public string Author { get; } = "Jannis";

        /// <summary>
        /// Gets or sets the description of the Dungeon profile.
        /// </summary>
        /// <value>
        /// Profile for the Dungeon in Westfall, made for Level 15 to 18.
        /// </value>
        public string Description { get; } = "Profile for the Dungeon in Westfall, made for Level 15 to 18.";

        /// <summary>
        /// Gets the coordinates of the dungeon exit in a Vector3 format.
        /// </summary>
        public Vector3 DungeonExit { get; } = new(-15, -392, 63);

        /// <summary>
        /// Gets or sets the faction type of the dungeon as Neutral.
        /// </summary>
        public DungeonFactionType FactionType { get; } = DungeonFactionType.Neutral;

        /// <summary>
        /// Gets the size of the group, which is always 5.
        /// </summary>
        public int GroupSize { get; } = 5;

        /// <summary>
        /// Gets the map ID of the Deadmines.
        /// </summary>
        public WowMapId MapId { get; } = WowMapId.Deadmines;

        /// <summary>
        /// Gets or sets the maximum level allowed.
        /// </summary>
        public int MaxLevel { get; } = 18;

        /// <summary>
        /// Gets the name of the location, which is "[15-18] Deadmines".
        /// </summary>
        public string Name { get; } = "[15-18] Deadmines";

        ///<summary>
        ///A list of DungeonNodes representing the nodes in the dungeon. Each DungeonNode 
        ///contains the coordinates of the node and an optional DungeonNodeType, 
        ///indicating whether it is a boss node, a door node, a collect node, or a use node.
        ///</summary>
        public List<DungeonNode> Nodes { get; private set; } = new()
        {
            new(new(-16, -383, 62)),
            new(new(-34, -375, 59)),
            new(new(-52, -372, 55)),
            new(new(-74, -372, 55)),
            new(new(-97, -378, 58)),
            new(new(-120, -384, 59)),
            new(new(-135, -402, 58)),
            new(new(-162, -398, 56)),
            new(new(-182, -418, 54)),
            new(new(-193, -444, 54), DungeonNodeType.Boss),
            new(new(-191, -457, 54), DungeonNodeType.Door),
            new(new(-188, -485, 54), DungeonNodeType.Door),
            new(new(-193, -505, 53)),
            new(new(-218, -497, 49)),
            new(new(-243, -482, 49)),
            new(new(-260, -482, 49), DungeonNodeType.Door),
            new(new(-278, -484, 49), DungeonNodeType.Door),
            new(new(-284, -497, 49)),
            new(new(-290, -520, 50), DungeonNodeType.Boss),
            new(new(-290, -534, 49), DungeonNodeType.Door),
            new(new(-290, -558, 49), DungeonNodeType.Door),
            new(new(-304, -597, 48)),
            new(new(-286, -602, 49)),
            new(new(-269, -578, 50)),
            new(new(-245, -579, 51), DungeonNodeType.Door),
            new(new(-230, -579, 51), DungeonNodeType.Door),
            new(new(-217, -556, 51)),
            new(new(-192, -555, 51)),
            new(new(-178, -578, 47)),
            new(new(-187, -598, 38)),
            new(new(-202, -606, 31)),
            new(new(-229, -597, 20)),
            new(new(-237, -581, 19)),
            new(new(-218, -580, 21)),
            new(new(-204, -592, 21)),
            new(new(-222, -572, 21)),
            new(new(-226, -557, 19)),
            new(new(-209, -550, 19)),
            new(new(-192, -553, 19)),
            new(new(-179, -573, 19)),
            new(new(-170, -580, 19), DungeonNodeType.Door),
            new(new(-154, -580, 19), DungeonNodeType.Door),
            new(new(-135, -582, 18)),
            new(new(-132, -596, 17)),
            new(new(-132, -614, 13)),
            new(new(-107, -617, 14), DungeonNodeType.Collect, defiasGunPowderName),
            new(new(-132, -621, 13)),
            new(new(-124, -637, 13)),
            new(new(-106, -648, 7)),
            new(new(-105, -658, 7), DungeonNodeType.Use, defiasGunPowderName),
            new(new(-98, -678, 7), DungeonNodeType.Use, defiasGunPowderName),
            new(new(-97, -691, 8)),
            new(new(-97, -704, 8)),
            new(new(-97, -717, 9)),
            new(new(-90, -726, 9)),
            new(new(-65, -731, 8)),
            new(new(-53, -729, 9)),
            new(new(-35, -727, 9)),
            new(new(-21, -731, 8)),
            new(new(-6, -742, 9)),
            new(new(3, -755, 9)),
            new(new(-1, -777, 11), DungeonNodeType.Boss),
            new(new(-20, -796, 19)),
            new(new(-29, -802, 19)),
            new(new(-37, -790, 19)),
            new(new(-54, -782, 18)),
            new(new(-76, -783, 17)),
            new(new(-101, -791, 17)),
            new(new(-116, -796, 17)),
            new(new(-124, -791, 17)),
            new(new(-101, -780, 22)),
            new(new(-84, -776, 27)),
            new(new(-81, -787, 26)),
            new(new(-104, -790, 28)),
            new(new(-102, -801, 30)),
            new(new(-97, -805, 30)),
            new(new(-79, -793, 39)),
            new(new(-63, -789, 40)),
            new(new(-41, -794, 40)),
            new(new(-45, -807, 42)),
            new(new(-59, -812, 42), DungeonNodeType.Boss),
            new(new(-80, -820, 40), DungeonNodeType.Boss),
            new(new(-64, -821, 41))
        };

        /// <summary>
        /// Gets the list of priority units.
        /// </summary>
        public List<int> PriorityUnits { get; } = new();

        /// <summary>
        /// Gets the required item level.
        /// </summary>
        public int RequiredItemLevel { get; } = 10;

        /// <summary>
        /// Gets the required level.
        /// </summary>
        public int RequiredLevel { get; } = 15;

        /// <summary>
        /// Gets the world entry coordinates in a three-dimensional vector.
        /// The coordinates are set to (-11208, 1680, 24) by default.
        /// </summary>
        public Vector3 WorldEntry { get; } = new(-11208, 1680, 24);

        /// <summary>
        /// Gets the map ID of the World Entry in Eastern Kingdoms.
        /// </summary>
        public WowMapId WorldEntryMapId { get; } = WowMapId.EasternKingdoms;
    }
}