﻿using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Dungeon.Enums;
using AmeisenBotX.Core.Engines.Dungeon.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Dungeon.Profiles.WotLK
{
    /// <summary>
    /// Gets the coordinates of the dungeon exit.
    /// </summary>
    public class PitOfSaronProfile : IDungeonProfile
    {
        /// <summary>
        /// Gets the author of the code.
        /// </summary>
        public string Author { get; } = "Jannis & Kamel";

        /// <summary>
        /// Gets the description of the Dungeon in Icecrown, made for Level 80.
        /// </summary>
        public string Description { get; } = "Profile for the Dungeon in Icecrown, made for Level 80.";

        /// <summary>
        /// Gets the coordinates of the dungeon exit.
        /// </summary>
        public Vector3 DungeonExit { get; } = new(425, 212, 529);

        /// <summary>
        /// Gets the faction type of the dungeon.
        /// </summary>
        public DungeonFactionType FactionType { get; } = DungeonFactionType.Neutral;

        /// <summary>
        /// Gets the size of the group.
        /// </summary>
        public int GroupSize { get; } = 5;

        /// <summary>
        /// Gets the map ID for the Forge of Souls.
        /// </summary>
        public WowMapId MapId { get; } = WowMapId.TheForgeOfSouls;

        /// <summary>
        /// Gets the maximum level, which is set as 80.
        /// </summary>
        public int MaxLevel { get; } = 80;

        /// <summary>
        /// Gets or sets the name of the location, which is set to "[80+] Pit Of Saron".
        /// </summary>
        public string Name { get; } = "[80+] Pit Of Saron";

        /// <summary>
        /// Represents a list of DungeonNodes.
        /// </summary>
        public List<DungeonNode> Nodes { get; } = new()
        {
            new(new(433, 212, 529)),
            new(new(441, 212, 529)),
            new(new(449, 211, 529)),
            new(new(457, 210, 529)),
            new(new(465, 207, 529)),
            new(new(473, 205, 529)),
            new(new(481, 203, 529)),
            new(new(488, 200, 529)),
            new(new(495, 196, 529)),
            new(new(501, 191, 529)),
            new(new(507, 186, 529)),
            new(new(513, 181, 529)),
            new(new(519, 175, 529)),
            new(new(526, 170, 528)),
            new(new(532, 165, 527)),
            new(new(538, 160, 526)),
            new(new(544, 156, 523)),
            new(new(550, 151, 521)),
            new(new(556, 147, 518)),
            new(new(561, 141, 515)),
            new(new(563, 133, 514)),
            new(new(564, 126, 511)),
            new(new(567, 119, 510)),
            new(new(574, 115, 509)),
            new(new(582, 118, 508)),
            new(new(590, 119, 508)),
            new(new(597, 115, 507)),
            new(new(602, 109, 508)),
            new(new(607, 103, 509)),
            new(new(613, 98, 510)),
            new(new(617, 91, 512)),
            new(new(617, 83, 512)),
            new(new(618, 75, 512)),
            new(new(622, 68, 512)),
            new(new(624, 60, 512)),
            new(new(624, 52, 512)),
            new(new(623, 44, 512)),
            new(new(622, 36, 512)),
            new(new(622, 28, 513)),
            new(new(625, 21, 513)),
            new(new(629, 14, 514)),
            new(new(633, 7, 514)),
            new(new(639, 1, 514)),
            new(new(640, -7, 514)),
            new(new(640, -15, 514)),
            new(new(641, -23, 514)),
            new(new(643, -31, 514)),
            new(new(645, -39, 514)),
            new(new(648, -46, 514)),
            new(new(651, -53, 514)),
            new(new(655, -60, 514)),
            new(new(659, -67, 514)),
            new(new(663, -74, 514)),
            new(new(667, -81, 514)),
            new(new(671, -88, 514)),
            new(new(676, -95, 514)),
            new(new(680, -102, 514)),
            new(new(685, -109, 514)),
            new(new(689, -116, 514)),
            new(new(691, -124, 515)),
            new(new(692, -131, 519)),
            new(new(692, -138, 522)),
            new(new(692, -145, 526)),
            new(new(692, -153, 528)),
            new(new(693, -161, 528)),
            new(new(695, -169, 527)),
            new(new(697, -177, 527)),
            new(new(699, -185, 527)),
            new(new(698, -193, 527)),
            new(new(694, -200, 527)),
            new(new(689, -206, 527)),
            new(new(681, -205, 527)),
            new(new(673, -202, 527)),
            new(new(665, -199, 527)),
            new(new(666, -191, 527)),
            new(new(670, -184, 527)),
            new(new(675, -178, 527)),
            new(new(679, -171, 527)),
            new(new(683, -164, 527)),
            new(new(686, -157, 528)),
            new(new(689, -150, 528)),
            new(new(692, -143, 525)),
            new(new(695, -136, 521)),
            new(new(698, -129, 518)),
            new(new(701, -122, 515)),
            new(new(705, -115, 514)),
            new(new(708, -108, 514)),
            new(new(714, -103, 514)),
            new(new(722, -101, 514)),
            new(new(730, -100, 514)),
            new(new(737, -97, 513)),
            new(new(742, -91, 513)),
            new(new(745, -84, 513)),
            new(new(746, -76, 512)),
            new(new(748, -68, 511)),
            new(new(750, -60, 510)),
            new(new(754, -53, 509)),
            new(new(760, -48, 508)),
            new(new(766, -43, 508)),
            new(new(772, -37, 508)),
            new(new(779, -33, 508)),
            new(new(786, -28, 508)),
            new(new(793, -23, 509)),
            new(new(800, -18, 509)),
            new(new(806, -13, 509)),
            new(new(811, -7, 510)),
            new(new(816, -1, 510)),
            new(new(818, 7, 510)),
            new(new(820, 15, 510)),
            new(new(822, 23, 510)),
            new(new(824, 31, 510)),
            new(new(826, 39, 510)),
            new(new(825, 47, 510)),
            new(new(822, 54, 510)),
            new(new(819, 61, 510)),
            new(new(816, 68, 510)),
            new(new(813, 75, 510)),
            new(new(809, 82, 509)),
            new(new(804, 88, 509)),
            new(new(798, 94, 509)),
            new(new(793, 100, 510)),
            new(new(787, 105, 510)),
            new(new(782, 111, 510)),
            new(new(780, 119, 510)),
            new(new(780, 127, 510)),
            new(new(785, 134, 510)),
            new(new(793, 136, 510)),
            new(new(799, 131, 509)),
            new(new(804, 125, 509)),
            new(new(809, 119, 509)),
            new(new(812, 112, 509)),
            new(new(815, 105, 509)),
            new(new(821, 100, 509)),
            new(new(829, 103, 510)),
            new(new(835, 108, 510)),
            new(new(840, 114, 510)),
            new(new(846, 120, 510)),
            new(new(853, 124, 510)),
            new(new(854, 116, 510)),
            new(new(849, 110, 510)),
            new(new(842, 106, 510)),
            new(new(836, 101, 510)),
            new(new(830, 96, 510)),
            new(new(824, 91, 510)),
            new(new(818, 86, 510)),
            new(new(818, 78, 510)),
            new(new(822, 71, 510)),
            new(new(826, 64, 510)),
            new(new(830, 57, 510)),
            new(new(834, 50, 510)),
            new(new(838, 43, 511)),
            new(new(846, 41, 511)),
            new(new(854, 43, 513)),
            new(new(861, 45, 516)),
            new(new(868, 47, 519)),
            new(new(875, 50, 523)),
            new(new(881, 53, 527)),
            new(new(886, 57, 531)),
            new(new(891, 61, 535)),
            new(new(896, 66, 539)),
            new(new(901, 70, 543)),
            new(new(906, 75, 548)),
            new(new(913, 78, 553)),
            new(new(920, 79, 557)),
            new(new(927, 80, 561)),
            new(new(934, 79, 564)),
            new(new(942, 78, 565)),
            new(new(948, 73, 566)),
            new(new(950, 65, 566)),
            new(new(950, 57, 567)),
            new(new(949, 49, 568)),
            new(new(949, 41, 570)),
            new(new(944, 34, 572)),
            new(new(941, 27, 573)),
            new(new(937, 20, 575)),
            new(new(934, 12, 576)),
            new(new(932, 5, 578)),
            new(new(930, -3, 581)),
            new(new(929, -11, 583)),
            new(new(928, -19, 585)),
            new(new(927, -27, 588)),
            new(new(926, -35, 589)),
            new(new(926, -43, 591)),
            new(new(928, -51, 592)),
            new(new(930, -59, 592)),
            new(new(932, -67, 592)),
            new(new(935, -74, 592)),
            new(new(938, -81, 593)),
            new(new(941, -88, 595)),
            new(new(946, -95, 596)),
            new(new(951, -102, 595)),
            new(new(955, -109, 595)),
            new(new(961, -115, 596)),
            new(new(968, -118, 598)),
            new(new(975, -122, 600)),
            new(new(981, -124, 605)),
            new(new(986, -128, 609)),
            new(new(992, -132, 612)),
            new(new(999, -133, 616)),
            new(new(1006, -134, 619)),
            new(new(1013, -132, 622)),
            new(new(1020, -129, 624)),
            new(new(1027, -125, 626)),
            new(new(1034, -122, 627)),
            new(new(1040, -116, 629)),
            new(new(1045, -110, 630)),
            new(new(1049, -103, 631)),
            new(new(1053, -96, 633)),
            new(new(1055, -88, 633)),
            new(new(1058, -81, 633)),
            new(new(1060, -73, 634)),
            new(new(1061, -65, 634)),
            new(new(1063, -57, 634)),
            new(new(1063, -49, 634)),
            new(new(1065, -41, 634)),
            new(new(1067, -33, 634)),
            new(new(1069, -25, 633)),
            new(new(1072, -18, 633)),
            new(new(1073, -10, 634)),
            new(new(1074, -2, 634)),
            new(new(1075, 6, 635)),
            new(new(1077, 14, 634)),
            new(new(1078, 22, 632)),
            new(new(1076, 29, 630)),
            new(new(1075, 37, 630)),
            new(new(1073, 45, 630)),
            new(new(1070, 53, 631)),
            new(new(1069, 61, 632)),
            new(new(1068, 69, 631)),
            new(new(1067, 77, 631)),
            new(new(1066, 85, 631)),
            new(new(1064, 93, 631)),
            new(new(1062, 101, 631)),
            new(new(1058, 108, 629)),
            new(new(1053, 115, 628)),
            new(new(1048, 122, 628)),
            new(new(1041, 126, 628)),
            new(new(1035, 131, 628)),
            new(new(1032, 138, 628)),
            new(new(1029, 145, 628)),
            new(new(1024, 152, 628)),
            new(new(1019, 158, 628)),
            new(new(1025, 163, 628)),
        };

        /// <summary>
        /// Gets the list of priority units.
        /// </summary>
        public List<int> PriorityUnits { get; } = new();

        /// <summary>
        /// Gets the required item level.
        /// </summary>
        public int RequiredItemLevel { get; } = 200;

        /// <summary>
        /// Gets the required level for a certain task.
        /// </summary>
        public int RequiredLevel { get; } = 80;

        /// <summary>
        /// Gets the world entry vector in 3D space.
        /// The default value is (5587, 2005, 798).
        /// </summary>
        public Vector3 WorldEntry { get; } = new(5587, 2005, 798);

        /// <summary>
        /// The WorldEntryMapId property represents the map ID for the Northrend zone.
        /// </summary>
        public WowMapId WorldEntryMapId { get; } = WowMapId.Northrend;
    }
}