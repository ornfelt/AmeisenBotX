﻿using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Dungeon.Enums;
using AmeisenBotX.Core.Engines.Dungeon.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Dungeon.Profiles.TBC
{
    /// <summary>
    /// Represents a profile for the Blood Furnace dungeon, implementing the IDungeonProfile interface.
    /// </summary>
    public class TheBloodFurnaceProfile : IDungeonProfile
    {
        /// <summary>
        /// Gets the value of the Author property, which is set to "Jannis".
        /// </summary>
        public string Author { get; } = "Jannis";

        /// <summary>
        /// Gets the description of the profile for the Dungeon in Outland, made for Level 59 to 63.
        /// </summary>
        public string Description { get; } = "Profile for the Dungeon in Outland, made for Level 59 to 63.";

        /// <summary>
        /// Gets the position of the dungeon exit in 3D space.
        /// The position is set to (0, 24, -45) by default.
        /// </summary>
        public Vector3 DungeonExit { get; } = new(0, 24, -45);

        /// <summary>
        /// Gets the faction type of the dungeon,
        /// which is neutral by default.
        /// </summary>
        public DungeonFactionType FactionType { get; } = DungeonFactionType.Neutral;

        /// <summary>
        /// Represents the size of the group.
        /// </summary>
        public int GroupSize { get; } = 5;

        /// <summary>
        /// Gets the map ID for The Blood Furnace.
        /// </summary>
        public WowMapId MapId { get; } = WowMapId.TheBloodFurnace;

        /// <summary>
        /// Gets the maximum level.
        /// </summary>
        public int MaxLevel { get; } = 63;

        /// <summary>
        /// Gets the name of the Blood Furnace dungeon.
        /// </summary>
        public string Name { get; } = "[59-63] The Blood Furnace";

        /// <summary>
        /// Gets or sets a list of DungeonNodes.
        /// </summary>
        /// <remarks>
        /// The list is initially populated with a series of DungeonNode objects, with each object representing a specific location in the dungeon.
        /// The DungeonNodes are arranged in a specific pattern throughout the dungeon.
        /// </remarks>
        public List<DungeonNode> Nodes { get; private set; } = new()
        {
            new(new(-4, 15, -45)),
            new(new(-2, 7, -45)),
            new(new(-1, -1, -44)),
            new(new(1, -9, -43)),
            new(new(3, -17, -43)),
            new(new(4, -25, -42)),
            new(new(6, -33, -42)),
            new(new(7, -41, -41)),
            new(new(8, -49, -41)),
            new(new(3, -56, -41)),
            new(new(-4, -60, -41)),
            new(new(-11, -64, -41)),
            new(new(-17, -69, -41)),
            new(new(-18, -77, -41)),
            new(new(-14, -84, -41)),
            new(new(-7, -87, -41)),
            new(new(1, -86, -41)),
            new(new(9, -85, -41)),
            new(new(17, -85, -41)),
            new(new(25, -85, -41)),
            new(new(33, -86, -41)),
            new(new(40, -89, -40)),
            new(new(48, -91, -40)),
            new(new(56, -91, -37)),
            new(new(64, -91, -34)),
            new(new(71, -90, -31)),
            new(new(78, -90, -28)),
            new(new(85, -89, -24)),
            new(new(92, -89, -21)),
            new(new(99, -89, -18)),
            new(new(106, -88, -15)),
            new(new(113, -88, -12)),
            new(new(120, -88, -8)),
            new(new(127, -88, -5)),
            new(new(134, -88, -2)),
            new(new(141, -88, 1)),
            new(new(148, -88, 4)),
            new(new(155, -88, 7)),
            new(new(162, -86, 10)),
            new(new(170, -84, 10)),
            new(new(178, -84, 10)),
            new(new(186, -84, 10)),
            new(new(194, -84, 10)),
            new(new(202, -85, 10)),
            new(new(210, -87, 10)),
            new(new(216, -92, 10)),
            new(new(223, -96, 10)),
            new(new(230, -93, 10)),
            new(new(234, -86, 10)),
            new(new(231, -78, 10)),
            new(new(228, -71, 10)),
            new(new(234, -66, 10)),
            new(new(241, -63, 10)),
            new(new(244, -56, 10)),
            new(new(246, -48, 10)),
            new(new(249, -40, 9)),
            new(new(252, -33, 7)),
            new(new(255, -26, 7)),
            new(new(261, -20, 7)),
            new(new(266, -14, 7)),
            new(new(272, -8, 7)),
            new(new(277, -2, 8)),
            new(new(285, -3, 9)),
            new(new(293, -3, 10)),
            new(new(300, 1, 10)),
            new(new(307, 5, 10)),
            new(new(313, 0, 10)),
            new(new(318, -6, 10)),
            new(new(326, -4, 10)),
            new(new(333, -1, 10)),
            new(new(339, 4, 10)),
            new(new(337, 12, 10)),
            new(new(333, 19, 10)),
            new(new(330, 26, 10)),
            new(new(327, 33, 10)),
            new(new(327, 41, 10)),
            new(new(327, 49, 10)),
            new(new(328, 57, 10)),
            new(new(330, 65, 10)),
            new(new(323, 69, 10)),
            new(new(316, 66, 10)),
            new(new(308, 64, 10)),
            new(new(305, 71, 10)),
            new(new(308, 78, 10)),
            new(new(310, 86, 10)),
            new(new(309, 94, 10)),
            new(new(312, 101, 10)),
            new(new(320, 99, 10)),
            new(new(328, 99, 10)),
            new(new(336, 100, 10)),
            new(new(344, 100, 10)),
            new(new(348, 93, 10)),
            new(new(355, 96, 10)),
            new(new(355, 104, 10)),
            new(new(347, 106, 10)),
            new(new(339, 105, 10)),
            new(new(332, 108, 10)),
            new(new(329, 115, 10)),
            new(new(329, 123, 10)),
            new(new(328, 131, 10)),
            new(new(328, 139, 10)),
            new(new(328, 147, 10)),
            new(new(327, 155, 10)),
            new(new(325, 163, 10)),
            new(new(322, 170, 10)),
            new(new(319, 178, 10)),
            new(new(317, 186, 10)),
            new(new(325, 184, 10)),
            new(new(333, 186, 10)),
            new(new(340, 189, 10)),
            new(new(348, 191, 10)),
            new(new(356, 192, 10)),
            new(new(364, 192, 10)),
            new(new(372, 192, 10)),
            new(new(380, 192, 10)),
            new(new(388, 191, 10)),
            new(new(396, 191, 10)),
            new(new(404, 191, 10)),
            new(new(412, 190, 10)),
            new(new(420, 190, 10)),
            new(new(428, 191, 10)),
            new(new(436, 191, 10)),
            new(new(444, 190, 10)),
            new(new(452, 188, 10)),
            new(new(459, 184, 10)),
            new(new(460, 176, 10)),
            new(new(459, 168, 10)),
            new(new(457, 160, 10)),
            new(new(456, 152, 10)),
            new(new(455, 144, 10)),
            new(new(456, 136, 10)),
            new(new(462, 131, 10)),
            new(new(463, 123, 10)),
            new(new(456, 120, 10)),
            new(new(463, 116, 10)),
            new(new(471, 114, 10)),
            new(new(477, 109, 10)),
            new(new(480, 102, 10)),
            new(new(478, 94, 10)),
            new(new(470, 94, 10)),
            new(new(462, 94, 10)),
            new(new(454, 94, 10)),
            new(new(448, 88, 10)),
            new(new(441, 85, 10)),
            new(new(438, 78, 10)),
            new(new(438, 70, 10)),
            new(new(441, 63, 10)),
            new(new(449, 63, 10)),
            new(new(456, 66, 10)),
            new(new(464, 66, 10)),
            new(new(472, 65, 10)),
            new(new(469, 58, 10)),
            new(new(457, 54, 10), DungeonNodeType.Use, "Cell Door Lever"),
            new(new(457, 51, 10), DungeonNodeType.Use, "Cell Door Lever"),
            new(new(456, 43, 10)),
            new(new(456, 35, 10)),
            new(new(456, 27, 10)),
            new(new(456, 19, 10)),
            new(new(456, 11, 10)),
            new(new(456, 3, 10)),
            new(new(464, 3, 10)),
            new(new(471, 7, 10)),
            new(new(478, 10, 10)),
            new(new(486, 12, 10)),
            new(new(491, 6, 10)),
            new(new(492, -2, 10)),
            new(new(488, -9, 10)),
            new(new(483, -15, 10)),
            new(new(478, -22, 10)),
            new(new(474, -29, 10)),
            new(new(471, -36, 10)),
            new(new(468, -43, 10)),
            new(new(466, -51, 10)),
            new(new(466, -59, 10)),
            new(new(466, -67, 10)),
            new(new(464, -75, 10)),
            new(new(461, -82, 10)),
            new(new(453, -84, 10)),
            new(new(445, -84, 10)),
            new(new(437, -84, 10)),
            new(new(429, -84, 10)),
            new(new(421, -85, 10)),
            new(new(417, -92, 10)),
            new(new(415, -100, 10)),
            new(new(413, -108, 10)),
            new(new(411, -116, 9)),
            new(new(409, -123, 7)),
            new(new(407, -130, 4)),
            new(new(404, -136, 0)),
            new(new(401, -142, -4)),
            new(new(396, -147, -8)),
            new(new(390, -152, -10)),
            new(new(385, -157, -13)),
            new(new(378, -162, -16)),
            new(new(371, -165, -19)),
            new(new(364, -166, -22)),
            new(new(357, -165, -26)),
            new(new(354, -172, -26)),
            new(new(349, -178, -26)),
            new(new(344, -185, -26)),
            new(new(339, -191, -26)),
            new(new(331, -193, -26)),
            new(new(323, -190, -26)),
            new(new(317, -185, -26)),
            new(new(313, -178, -26)),
            new(new(319, -173, -26)),
            new(new(326, -169, -26)),
            new(new(328, -161, -26)),
            new(new(328, -153, -26)),
            new(new(328, -145, -25)),
            new(new(328, -137, -25)),
            new(new(328, -129, -25)),
            new(new(325, -122, -25)),
            new(new(321, -115, -25)),
            new(new(316, -109, -25)),
            new(new(311, -103, -25)),
            new(new(307, -96, -25)),
            new(new(302, -90, -25)),
            new(new(305, -83, -25)),
            new(new(308, -76, -25)),
            new(new(311, -68, -25)),
            new(new(318, -64, -25)),
            new(new(326, -64, -25)),
            new(new(334, -64, -25)),
            new(new(341, -67, -25)),
            new(new(347, -73, -25)),
            new(new(349, -81, -25)),
            new(new(350, -89, -25)),
            new(new(348, -97, -25)),
            new(new(345, -104, -25)),
            new(new(338, -109, -25)),
            new(new(330, -107, -25)),
            new(new(328, -99, -25)),
            new(new(327, -91, -25)),
            new(new(327, -83, -25)),
            new(new(323, -90, -25)),
        };

        /// <summary>
        /// Gets the list of priority units.
        /// </summary>
        public List<int> PriorityUnits { get; } = new();

        /// <summary>
        /// Gets the required item level.
        /// </summary>
        public int RequiredItemLevel { get; } = 62;

        /// <summary>
        /// Gets the required level for this code.
        /// </summary>
        public int RequiredLevel { get; } = 59;

        /// <summary>
        /// Gets or sets the world entry position.
        /// </summary>
        public Vector3 WorldEntry { get; } = new(-305, 3167, 31);

        /// <summary>
        /// Gets the World Entry Map Id for the Outland.
        /// </summary>
        public WowMapId WorldEntryMapId { get; } = WowMapId.Outland;
    }
}