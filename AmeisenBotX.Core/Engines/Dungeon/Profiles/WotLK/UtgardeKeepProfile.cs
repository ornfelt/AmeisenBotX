﻿using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Dungeon.Enums;
using AmeisenBotX.Core.Engines.Dungeon.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Dungeon.Profiles.WotLK
{
    public class UtgardeKeepProfile : IDungeonProfile
    {
        /// <summary>
        /// Gets the name of the author.
        /// </summary>
        public string Author { get; } = "Jannis";

        /// <summary>
        /// Gets the description for the Dungeon in the Howling Fjord.
        /// This profile is designed for Level 68 to 80.
        /// </summary>
        public string Description { get; } = "Profile for the Dungeon in the Howling Fjord, made for Level 68 to 80.";

        /// <summary>
        /// Gets the dungeon exit position as a Vector3.
        /// </summary>
        public Vector3 DungeonExit { get; } = new(148, -87, 13);

        /// <summary>
        /// Gets or sets the faction type of the dungeon.
        /// </summary>
        public DungeonFactionType FactionType { get; } = DungeonFactionType.Neutral;

        /// <summary>
        /// Gets the size of the group.
        /// </summary>
        public int GroupSize { get; } = 5;

        /// <summary>
        /// Gets or sets the WowMapId of the Utgarde Keep map.
        /// </summary>
        public WowMapId MapId { get; } = WowMapId.UtgardeKeep;

        /// <summary>
        /// Gets the maximum level value which is set to 80.
        /// </summary>
        public int MaxLevel { get; } = 80;

        /// <summary>
        /// Gets the name of the instance as "[68-80] Utgarde Keep".
        /// </summary>
        public string Name { get; } = "[68-80] Utgarde Keep";

        /// <summary>
        /// The list of DungeonNodes in the current dungeon.
        /// </summary>
        /// <value>A list of DungeonNodes.</value>
        public List<DungeonNode> Nodes { get; private set; } = new()
        {
            new(new(157, -85, 13)),
            new(new(165, -83, 13)),
            new(new(173, -80, 13)),
            new(new(181, -78, 14)),
            new(new(188, -76, 17)),
            new(new(195, -73, 19)),
            new(new(202, -71, 21)),
            new(new(209, -68, 25)),
            new(new(217, -66, 25)),
            new(new(224, -63, 25)),
            new(new(230, -58, 25)),
            new(new(237, -53, 25)),
            new(new(244, -48, 25)),
            new(new(251, -44, 25)),
            new(new(257, -49, 25)),
            new(new(261, -56, 25)),
            new(new(268, -59, 25)),
            new(new(274, -54, 25)),
            new(new(280, -49, 25)),
            new(new(286, -44, 25)),
            new(new(292, -39, 25)),
            new(new(299, -36, 25)),
            new(new(307, -35, 25)),
            new(new(315, -36, 23)),
            new(new(322, -40, 23)),
            new(new(330, -41, 23)),
            new(new(338, -43, 23)),
            new(new(346, -45, 23)),
            new(new(354, -47, 23)),
            new(new(361, -50, 23)),
            new(new(369, -51, 23)),
            new(new(377, -48, 23)),
            new(new(385, -45, 23)),
            new(new(392, -41, 23)),
            new(new(397, -35, 23)),
            new(new(401, -28, 23)),
            new(new(398, -21, 23)),
            new(new(392, -15, 23)),
            new(new(390, -7, 23)),
            new(new(388, 1, 23)),
            new(new(383, 8, 23)),
            new(new(376, 13, 23)),
            new(new(372, 20, 23)),
            new(new(373, 28, 23)),
            new(new(374, 36, 25)),
            new(new(374, 44, 25)),
            new(new(375, 52, 25)),
            new(new(376, 60, 25)),
            new(new(377, 68, 28)),
            new(new(378, 75, 31)),
            new(new(379, 83, 31)),
            new(new(378, 91, 31)),
            new(new(376, 99, 31)),
            new(new(374, 107, 31)),
            new(new(371, 114, 31)),
            new(new(368, 121, 31)),
            new(new(365, 129, 31)),
            new(new(362, 136, 31)),
            new(new(357, 143, 31)),
            new(new(353, 150, 31)),
            new(new(350, 157, 31)),
            new(new(350, 165, 31)),
            new(new(350, 173, 31)),
            new(new(349, 181, 31)),
            new(new(348, 189, 31)),
            new(new(345, 197, 31)),
            new(new(342, 205, 31)),
            new(new(340, 213, 31)),
            new(new(337, 220, 31)),
            new(new(335, 228, 31)),
            new(new(332, 236, 31)),
            new(new(331, 244, 31)),
            new(new(329, 252, 31)),
            new(new(322, 255, 31)),
            new(new(315, 252, 31)),
            new(new(308, 249, 31)),
            new(new(300, 246, 32)),
            new(new(293, 244, 35)),
            new(new(286, 241, 37)),
            new(new(279, 238, 40)),
            new(new(272, 236, 43)),
            new(new(265, 233, 43)),
            new(new(258, 230, 43)),
            new(new(250, 227, 43)),
            new(new(243, 225, 41)),
            new(new(235, 226, 41)),
            new(new(227, 228, 41)),
            new(new(219, 229, 41)),
            new(new(211, 230, 41)),
            new(new(203, 228, 41)),
            new(new(197, 222, 41)),
            new(new(193, 215, 41)),
            new(new(191, 207, 41)),
            new(new(191, 199, 41)),
            new(new(184, 202, 41)),
            new(new(182, 210, 41)),
            new(new(179, 217, 41)),
            new(new(176, 224, 41)),
            new(new(174, 232, 43)),
            new(new(171, 239, 43)),
            new(new(166, 246, 43)),
            new(new(161, 253, 43)),
            new(new(157, 260, 43)),
            new(new(152, 266, 43)),
            new(new(147, 272, 43)),
            new(new(140, 276, 43)),
            new(new(132, 273, 43)),
            new(new(126, 268, 43)),
            new(new(121, 262, 43)),
            new(new(115, 257, 43)),
            new(new(109, 252, 43)),
            new(new(103, 246, 43)),
            new(new(98, 240, 43)),
            new(new(94, 233, 46)),
            new(new(91, 226, 49)),
            new(new(89, 218, 49)),
            new(new(87, 210, 49)),
            new(new(85, 202, 49)),
            new(new(85, 194, 49)),
            new(new(84, 186, 49)),
            new(new(84, 178, 49)),
            new(new(84, 170, 50)),
            new(new(84, 163, 53)),
            new(new(85, 156, 56)),
            new(new(87, 149, 59)),
            new(new(90, 142, 63)),
            new(new(92, 135, 66)),
            new(new(95, 128, 66)),
            new(new(97, 120, 65)),
            new(new(100, 113, 65)),
            new(new(102, 105, 65)),
            new(new(105, 98, 65)),
            new(new(110, 92, 66)),
            new(new(117, 88, 66)),
            new(new(124, 84, 66)),
            new(new(130, 79, 66)),
            new(new(134, 72, 66)),
            new(new(133, 64, 66)),
            new(new(130, 57, 66)),
            new(new(124, 51, 66)),
            new(new(116, 50, 66)),
            new(new(108, 50, 66)),
            new(new(102, 56, 66)),
            new(new(100, 64, 66)),
            new(new(98, 71, 68)),
            new(new(97, 78, 73)),
            new(new(95, 84, 78)),
            new(new(92, 91, 82)),
            new(new(91, 97, 87)),
            new(new(88, 104, 87)),
            new(new(87, 112, 87)),
            new(new(93, 117, 87)),
            new(new(101, 119, 87)),
            new(new(109, 121, 87)),
            new(new(115, 116, 87)),
            new(new(116, 108, 88)),
            new(new(118, 102, 93)),
            new(new(120, 95, 97)),
            new(new(121, 88, 102)),
            new(new(124, 82, 107)),
            new(new(126, 75, 109)),
            new(new(129, 68, 109)),
            new(new(126, 61, 109)),
            new(new(119, 58, 109)),
            new(new(112, 55, 109)),
            new(new(105, 52, 109)),
            new(new(97, 50, 109)),
            new(new(90, 47, 109)),
            new(new(83, 45, 111)),
            new(new(76, 43, 114)),
            new(new(69, 40, 115)),
            new(new(61, 38, 115)),
            new(new(54, 35, 115)),
            new(new(47, 32, 115)),
            new(new(40, 29, 115)),
            new(new(34, 23, 115)),
            new(new(34, 15, 115)),
            new(new(37, 8, 115)),
            new(new(40, 1, 116)),
            new(new(44, -6, 119)),
            new(new(47, -13, 119)),
            new(new(51, -20, 119)),
            new(new(56, -26, 119)),
            new(new(62, -31, 119)),
            new(new(69, -34, 119)),
            new(new(77, -36, 119)),
            new(new(85, -37, 119)),
            new(new(93, -37, 119)),
            new(new(101, -38, 119)),
            new(new(109, -38, 119)),
            new(new(117, -37, 119)),
            new(new(124, -35, 121)),
            new(new(131, -33, 124)),
            new(new(138, -31, 127)),
            new(new(144, -28, 131)),
            new(new(151, -26, 134)),
            new(new(159, -23, 135)),
            new(new(167, -20, 135)),
            new(new(175, -17, 135)),
            new(new(183, -14, 135)),
            new(new(190, -11, 135)),
            new(new(196, -6, 135)),
            new(new(202, -1, 135)),
            new(new(208, 5, 135)),
            new(new(214, 10, 135)),
            new(new(221, 14, 135)),
            new(new(228, 17, 135)),
            new(new(236, 17, 135)),
            new(new(242, 12, 135)),
            new(new(242, 4, 135)),
            new(new(239, -4, 135)),
            new(new(234, -10, 135)),
            new(new(227, -13, 135)),
            new(new(220, -16, 136)),
            new(new(213, -18, 140)),
            new(new(207, -21, 145)),
            new(new(201, -24, 149)),
            new(new(194, -25, 153)),
            new(new(187, -25, 157)),
            new(new(179, -25, 157)),
            new(new(172, -22, 157)),
            new(new(170, -14, 157)),
            new(new(173, -7, 157)),
            new(new(180, -4, 157)),
            new(new(186, -1, 161)),
            new(new(193, 1, 166)),
            new(new(200, 3, 170)),
            new(new(206, 5, 175)),
            new(new(213, 6, 178)),
            new(new(221, 9, 179)),
            new(new(229, 10, 179)),
            new(new(233, 3, 179)),
            new(new(233, -5, 179)),
            new(new(236, -12, 179)),
            new(new(240, -19, 179)),
            new(new(243, -27, 179)),
            new(new(245, -34, 181)),
            new(new(247, -41, 183)),
            new(new(249, -48, 185)),
            new(new(252, -55, 187)),
            new(new(254, -62, 191)),
            new(new(257, -69, 191)),
            new(new(260, -77, 190)),
            new(new(262, -85, 190)),
            new(new(265, -92, 190)),
            new(new(268, -99, 190)),
            new(new(271, -106, 190)),
            new(new(273, -114, 190)),
            new(new(275, -122, 190)),
            new(new(275, -130, 190)),
            new(new(274, -138, 190)),
            new(new(270, -145, 190)),
            new(new(264, -150, 190)),
            new(new(258, -155, 190)),
            new(new(251, -158, 190)),
            new(new(243, -161, 190)),
            new(new(235, -161, 190)),
            new(new(227, -160, 190)),
            new(new(220, -157, 190)),
            new(new(212, -155, 190)),
            new(new(204, -154, 190)),
            new(new(197, -157, 190)),
            new(new(190, -161, 187)),
            new(new(183, -164, 185)),
            new(new(176, -167, 182)),
            new(new(169, -170, 181)),
            new(new(162, -173, 181)),
            new(new(157, -179, 181)),
            new(new(161, -186, 180)),
            new(new(165, -193, 180)),
            new(new(169, -200, 180)),
            new(new(173, -207, 180)),
            new(new(177, -214, 181)),
            new(new(181, -221, 180)),
            new(new(184, -228, 181)),
            new(new(188, -235, 181)),
            new(new(192, -242, 180)),
            new(new(196, -249, 181)),
            new(new(200, -256, 181)),
            new(new(204, -263, 180)),
            new(new(208, -270, 180)),
            new(new(212, -277, 180)),
            new(new(216, -284, 180)),
            new(new(219, -291, 180)),
            new(new(223, -298, 180)),
            new(new(227, -305, 180)),
            new(new(231, -312, 180)),
            new(new(234, -319, 180)),
            new(new(238, -326, 180)),
            new(new(242, -333, 180)),
            new(new(241, -325, 180)),
        };

        /// <summary>
        /// Gets the list of priority units.
        /// </summary>
        public List<int> PriorityUnits { get; } = new();

        /// <summary>
        /// Gets the required item level.
        /// </summary>
        public int RequiredItemLevel { get; } = 100;

        /// <summary>
        /// Gets the required level for accessing this property.
        /// </summary>
        public int RequiredLevel { get; } = 68;

        /// <summary>
        /// Gets the world entry point as a Vector3.
        /// </summary>
        public Vector3 WorldEntry { get; } = new(1238, -4860, 41);

        /// <summary>
        /// Gets the World Entry Map ID for the WowMapId property which is set to WowMapId.Northrend.
        /// </summary>
        public WowMapId WorldEntryMapId { get; } = WowMapId.Northrend;
    }
}