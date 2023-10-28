﻿using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Dungeon.Enums;
using AmeisenBotX.Core.Engines.Dungeon.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;

/// <summary>
/// Contains dungeon profiles for The Burning Crusade expansion in the AmeisenBotX project.
/// </summary>
namespace AmeisenBotX.Core.Engines.Dungeon.Profiles.TBC
{
    /// <summary>
    /// Represents a dungeon profile for The Underbog, designed for players at level 61 to 65.
    /// </summary>
    public class TheUnderbogProfile : IDungeonProfile
    {
        /// <summary>
        /// Gets the name of the author.
        /// </summary>
        public string Author { get; } = "Jannis";

        /// <summary>
        /// Gets the description of the Dungeon profile for the Dungeon in Outland, made for Levels 61 to 65.
        /// </summary>
        public string Description { get; } = "Profile for the Dungeon in Outland, made for Level 61 to 65.";

        /// <summary>
        /// Gets the position of the dungeon exit in the form of a Vector3 object.
        /// </summary>
        public Vector3 DungeonExit { get; } = new(5, -14, -3);

        /// <summary>
        /// Gets the faction type of the dungeon.
        /// </summary>
        public DungeonFactionType FactionType { get; } = DungeonFactionType.Neutral;

        ///<summary>
        /// Gets the size of the group.
        ///</summary>
        public int GroupSize { get; } = 5;

        /// <summary>
        /// Gets the map ID for The Underbog.
        /// </summary>
        public WowMapId MapId { get; } = WowMapId.TheUnderbog;

        /// <summary>
        /// Gets the maximum level allowed, which is 65.
        /// </summary>
        public int MaxLevel { get; } = 65;

        ///<summary> 
        ///Gets the name of the area, which is "[61-65] The Underbog"
        ///</summary>
        public string Name { get; } = "[61-65] The Underbog";

        /// <summary>
        /// Gets or sets the list of DungeonNodes in the current Dungeon.
        /// </summary>
        public List<DungeonNode> Nodes { get; private set; } = new()
        {
            new(new(10, -16, -3)),
            new(new(16, -21, -3)),
            new(new(23, -25, -3)),
            new(new(30, -29, -3)),
            new(new(37, -34, -3)),
            new(new(44, -38, -3)),
            new(new(51, -42, -3)),
            new(new(58, -47, -3)),
            new(new(64, -52, -3)),
            new(new(68, -59, -3)),
            new(new(70, -67, -3)),
            new(new(70, -75, -3)),
            new(new(67, -82, -3)),
            new(new(63, -89, -3)),
            new(new(59, -96, -3)),
            new(new(55, -103, -3)),
            new(new(52, -110, -3)),
            new(new(53, -118, -3)),
            new(new(54, -126, -3)),
            new(new(56, -134, -3)),
            new(new(57, -142, -3)),
            new(new(58, -150, -3)),
            new(new(58, -158, -3)),
            new(new(57, -166, -3)),
            new(new(53, -173, -3)),
            new(new(48, -179, -4)),
            new(new(43, -185, -4)),
            new(new(36, -190, -4)),
            new(new(29, -195, -4)),
            new(new(23, -200, -4)),
            new(new(16, -204, -5)),
            new(new(8, -207, -5)),
            new(new(1, -210, -5)),
            new(new(-6, -214, -5)),
            new(new(-13, -218, -5)),
            new(new(-20, -221, -5)),
            new(new(-27, -225, -5)),
            new(new(-33, -230, -5)),
            new(new(-40, -235, -5)),
            new(new(-45, -241, -5)),
            new(new(-50, -248, -5)),
            new(new(-53, -255, -5)),
            new(new(-58, -261, -5)),
            new(new(-62, -268, -5)),
            new(new(-69, -273, -3)),
            new(new(-76, -277, -2)),
            new(new(-83, -281, 0)),
            new(new(-90, -284, 1)),
            new(new(-98, -286, 2)),
            new(new(-106, -286, 3)),
            new(new(-113, -284, 5)),
            new(new(-119, -279, 7)),
            new(new(-122, -272, 9)),
            new(new(-116, -267, 13)),
            new(new(-112, -262, 17)),
            new(new(-108, -258, 22)),
            new(new(-101, -255, 24)),
            new(new(-97, -262, 24)),
            new(new(-97, -270, 25)),
            new(new(-98, -278, 26)),
            new(new(-100, -286, 27)),
            new(new(-103, -294, 28)),
            new(new(-106, -302, 29)),
            new(new(-108, -310, 30)),
            new(new(-111, -317, 31)),
            new(new(-113, -325, 32)),
            new(new(-116, -332, 33)),
            new(new(-115, -340, 33)),
            new(new(-117, -348, 34)),
            new(new(-119, -356, 34)),
            new(new(-119, -364, 35)),
            new(new(-117, -372, 36)),
            new(new(-111, -377, 37)),
            new(new(-104, -380, 37)),
            new(new(-97, -384, 37)),
            new(new(-90, -387, 36)),
            new(new(-82, -388, 35)),
            new(new(-74, -388, 33)),
            new(new(-66, -388, 31)),
            new(new(-58, -388, 31)),
            new(new(-50, -388, 31)),
            new(new(-42, -388, 31)),
            new(new(-34, -386, 32)),
            new(new(-28, -380, 32)),
            new(new(-25, -373, 31)),
            new(new(-22, -366, 31)),
            new(new(-19, -359, 30)),
            new(new(-16, -352, 30)),
            new(new(-13, -345, 30)),
            new(new(-10, -338, 30)),
            new(new(-7, -331, 31)),
            new(new(-4, -324, 31)),
            new(new(2, -319, 31)),
            new(new(9, -315, 31)),
            new(new(16, -312, 32)),
            new(new(23, -309, 32)),
            new(new(30, -305, 32)),
            new(new(37, -302, 32)),
            new(new(44, -299, 33)),
            new(new(51, -295, 33)),
            new(new(58, -292, 33)),
            new(new(65, -288, 32)),
            new(new(73, -285, 32)),
            new(new(81, -286, 32)),
            new(new(86, -292, 32)),
            new(new(90, -299, 32)),
            new(new(93, -307, 32)),
            new(new(94, -315, 33)),
            new(new(95, -323, 33)),
            new(new(95, -331, 33)),
            new(new(93, -339, 33)),
            new(new(91, -347, 33)),
            new(new(89, -355, 33)),
            new(new(86, -362, 33)),
            new(new(83, -369, 33)),
            new(new(78, -376, 33)),
            new(new(73, -383, 33)),
            new(new(71, -391, 33)),
            new(new(79, -393, 34)),
            new(new(87, -393, 34)),
            new(new(95, -396, 35)),
            new(new(102, -398, 38)),
            new(new(109, -401, 40)),
            new(new(116, -404, 43)),
            new(new(123, -408, 45)),
            new(new(130, -411, 48)),
            new(new(137, -415, 49)),
            new(new(144, -418, 49)),
            new(new(151, -422, 49)),
            new(new(158, -425, 48)),
            new(new(165, -422, 48)),
            new(new(170, -416, 48)),
            new(new(174, -409, 48)),
            new(new(179, -403, 48)),
            new(new(184, -397, 48)),
            new(new(189, -390, 48)),
            new(new(194, -384, 48)),
            new(new(199, -378, 48)),
            new(new(205, -373, 48)),
            new(new(212, -370, 51)),
            new(new(218, -368, 56)),
            new(new(225, -366, 61)),
            new(new(232, -364, 65)),
            new(new(238, -363, 70)),
            new(new(246, -364, 72)),
            new(new(248, -372, 72)),
            new(new(242, -377, 73)),
            new(new(234, -379, 73)),
            new(new(227, -382, 73)),
            new(new(219, -384, 73)),
            new(new(212, -387, 72)),
            new(new(205, -390, 72)),
            new(new(198, -395, 72)),
            new(new(191, -400, 72)),
            new(new(185, -405, 72)),
            new(new(180, -411, 72)),
            new(new(175, -417, 72)),
            new(new(171, -424, 72)),
            new(new(168, -431, 72)),
            new(new(165, -438, 72)),
            new(new(162, -445, 72)),
            new(new(158, -452, 72)),
            new(new(157, -460, 73)),
            new(new(162, -466, 75)),
            new(new(170, -468, 76)),
            new(new(178, -469, 76)),
            new(new(186, -470, 77)),
            new(new(194, -470, 78)),
            new(new(202, -471, 79)),
            new(new(208, -476, 80)),
            new(new(215, -479, 81)),
            new(new(222, -476, 81)),
            new(new(229, -473, 81)),
            new(new(236, -469, 81)),
            new(new(243, -466, 81)),
            new(new(250, -462, 81)),
            new(new(246, -455, 81)),
            new(new(238, -457, 81)),
            new(new(246, -459, 81)),
            new(new(254, -461, 81)),
            new(new(261, -464, 81)),
            new(new(269, -463, 81)),
            new(new(277, -464, 81)),
            new(new(279, -465, 81)),
            new(new(292, -468, 49)),
            new(new(300, -469, 49)),
            new(new(308, -470, 49)),
            new(new(316, -472, 49)),
            new(new(324, -473, 49)),
            new(new(332, -475, 52)),
            new(new(336, -475, 52)),
            new(new(348, -474, 24)),
            new(new(354, -469, 24)),
            new(new(356, -461, 26)),
            new(new(357, -454, 29)),
            new(new(358, -447, 32)),
            new(new(359, -440, 35)),
            new(new(359, -433, 38)),
            new(new(357, -426, 42)),
            new(new(353, -420, 45)),
            new(new(348, -413, 46)),
            new(new(343, -407, 47)),
            new(new(338, -401, 45)),
            new(new(333, -394, 45)),
            new(new(335, -386, 44)),
            new(new(337, -379, 41)),
            new(new(339, -372, 38)),
            new(new(340, -365, 35)),
            new(new(341, -357, 32)),
            new(new(340, -349, 29)),
            new(new(337, -342, 26)),
            new(new(333, -335, 23)),
            new(new(329, -328, 21)),
            new(new(323, -322, 19)),
            new(new(317, -317, 19)),
            new(new(310, -313, 19)),
            new(new(303, -310, 19)),
            new(new(296, -306, 19)),
            new(new(289, -303, 19)),
            new(new(282, -299, 19)),
            new(new(277, -293, 21)),
            new(new(273, -286, 23)),
            new(new(270, -279, 24)),
            new(new(268, -271, 25)),
            new(new(268, -263, 26)),
            new(new(269, -255, 27)),
            new(new(269, -247, 27)),
            new(new(269, -239, 28)),
            new(new(269, -231, 28)),
            new(new(269, -223, 29)),
            new(new(269, -215, 29)),
            new(new(268, -207, 29)),
            new(new(266, -199, 29)),
            new(new(263, -192, 28)),
            new(new(260, -185, 29)),
            new(new(257, -178, 29)),
            new(new(254, -170, 29)),
            new(new(253, -162, 29)),
            new(new(254, -154, 29)),
            new(new(259, -147, 29)),
            new(new(265, -142, 30)),
            new(new(272, -138, 30)),
            new(new(279, -134, 30)),
            new(new(286, -130, 30)),
            new(new(290, -123, 30)),
            new(new(283, -119, 30)),
            new(new(275, -121, 30)),
            new(new(267, -123, 30)),
            new(new(260, -126, 29)),
            new(new(252, -127, 29)),
            new(new(244, -128, 28)),
            new(new(236, -128, 26)),
            new(new(228, -129, 26)),
            new(new(220, -129, 26)),
            new(new(212, -129, 27)),
            new(new(204, -129, 28)),
            new(new(196, -128, 28)),
            new(new(188, -128, 28)),
            new(new(180, -126, 27)),
            new(new(172, -123, 26)),
            new(new(165, -119, 25)),
            new(new(160, -113, 25)),
            new(new(160, -105, 26)),
            new(new(162, -97, 26)),
            new(new(165, -90, 27)),
            new(new(167, -82, 27)),
            new(new(170, -75, 27)),
            new(new(174, -68, 27)),
            new(new(180, -62, 26)),
            new(new(186, -57, 26)),
            new(new(193, -53, 27)),
            new(new(199, -48, 27)),
            new(new(205, -43, 27)),
            new(new(211, -38, 28)),
            new(new(216, -32, 28)),
            new(new(219, -25, 28)),
            new(new(218, -17, 28)),
            new(new(215, -10, 28)),
            new(new(212, -3, 28)),
            new(new(208, 4, 28)),
            new(new(202, 9, 28)),
            new(new(194, 11, 28)),
            new(new(186, 12, 27)),
            new(new(178, 14, 27)),
            new(new(170, 15, 27)),
            new(new(162, 16, 27)),
            new(new(154, 14, 27)),
            new(new(147, 10, 27)),
            new(new(140, 6, 27)),
            new(new(133, 3, 27)),
            new(new(126, -1, 27)),
            new(new(119, -4, 28)),
            new(new(111, -6, 27)),
            new(new(104, -8, 24)),
            new(new(97, -10, 21)),
            new(new(89, -11, 20)),
            new(new(81, -13, 18)),
            new(new(73, -15, 17)),
            new(new(65, -16, 19)),
            new(new(57, -17, 21)),
            new(new(49, -19, 20)),
            new(new(42, -22, 19)),
            new(new(40, -23, 19)),
            new(new(30, -28, -3)),
            new(new(22, -29, -3)),
        };

        /// <summary>
        /// Gets or sets the list of priority units.
        /// </summary>
        public List<int> PriorityUnits { get; } = new();

        /// <summary>
        /// Gets the required item level.
        /// </summary>
        public int RequiredItemLevel { get; } = 65;

        /// <summary>
        /// Gets the required level for the code. It is initialized to 61.
        /// </summary>
        public int RequiredLevel { get; } = 61;

        /// <summary>
        /// Gets the world entry position in vector3 format.
        /// The position is set to a specific set of coordinates (782, 6746, -73).
        /// </summary>
        public Vector3 WorldEntry { get; } = new(782, 6746, -73);

        /// <summary>
        /// Gets the World Entry Map ID for the Outland.
        /// </summary>
        public WowMapId WorldEntryMapId { get; } = WowMapId.Outland;
    }
}