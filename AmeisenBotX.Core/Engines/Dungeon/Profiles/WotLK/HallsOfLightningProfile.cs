﻿using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Dungeon.Enums;
using AmeisenBotX.Core.Engines.Dungeon.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;

/// <summary>
/// Contains dungeon profiles for the Wrath of the Lich King expansion in the AmeisenBotX project.
/// </summary>
namespace AmeisenBotX.Core.Engines.Dungeon.Profiles.WotLK
{
    /// <summary>
    /// Represents a Dungeon profile for the Halls of Lightning Dungeon near Ulduar, designed for players at Level 75 to 80.
    /// </summary>
    public class HallsOfLightningProfile : IDungeonProfile
    {
        /// <summary>
        /// Gets the author of the code.
        /// </summary>
        public string Author { get; } = "Jannis";

        /// <summary>
        /// Gets the description of the Dungeon profile for the Dungeon near Ulduar, made for Level 75 to 80.
        /// </summary>
        public string Description { get; } = "Profile for the Dungeon near Ulduar, made for Level 75 to 80.";

        /// <summary>
        /// Gets the position of the dungeon exit.
        /// </summary>
        public Vector3 DungeonExit { get; } = new(1331, 275, 53);

        /// <summary>
        /// Gets the faction type of the dungeon.
        /// </summary>
        public DungeonFactionType FactionType { get; } = DungeonFactionType.Neutral;

        /// <summary>
        /// Gets the size of the group.
        /// </summary>
        public int GroupSize { get; } = 5;

        /// <summary>
        /// Gets or sets the World of Warcraft map ID for Halls of Lightning.
        /// </summary>
        public WowMapId MapId { get; } = WowMapId.HallsOfLighting;

        /// <summary>
        /// Gets the maximum level value, which is set as 80.
        /// </summary>
        public int MaxLevel { get; } = 80;

        /// <summary>
        /// Gets the name of the location, which is set as "[75-80] Utgarde Keep".
        /// </summary>
        public string Name { get; } = "[75-80] Utgarde Keep";

        /// <summary>
        /// Gets or sets the list of DungeonNodes.
        /// </summary>
        /// <value>The list of DungeonNodes.</value>
        public List<DungeonNode> Nodes { get; private set; } = new()
        {
            new(new(1331, 237, 53)),
            new(new(1331, 229, 53)),
            new(new(1331, 221, 53)),
            new(new(1331, 213, 53)),
            new(new(1331, 205, 53)),
            new(new(1330, 197, 53)),
            new(new(1330, 189, 54)),
            new(new(1330, 181, 54)),
            new(new(1330, 173, 54)),
            new(new(1330, 165, 54)),
            new(new(1330, 157, 54)),
            new(new(1330, 149, 52)),
            new(new(1331, 142, 49)),
            new(new(1331, 135, 46)),
            new(new(1331, 128, 43)),
            new(new(1331, 121, 40)),
            new(new(1330, 113, 40)),
            new(new(1327, 105, 40)),
            new(new(1321, 100, 40)),
            new(new(1313, 99, 40)),
            new(new(1306, 96, 39)),
            new(new(1299, 94, 37)),
            new(new(1292, 97, 36)),
            new(new(1285, 102, 34)),
            new(new(1278, 105, 34)),
            new(new(1271, 108, 34)),
            new(new(1263, 110, 34)),
            new(new(1259, 103, 34)),
            new(new(1260, 95, 34)),
            new(new(1263, 88, 34)),
            new(new(1265, 80, 34)),
            new(new(1267, 72, 34)),
            new(new(1266, 64, 34)),
            new(new(1263, 57, 33)),
            new(new(1260, 49, 33)),
            new(new(1260, 41, 34)),
            new(new(1262, 33, 33)),
            new(new(1264, 25, 33)),
            new(new(1266, 17, 34)),
            new(new(1266, 9, 34)),
            new(new(1262, 2, 33)),
            new(new(1259, -5, 34)),
            new(new(1257, -13, 34)),
            new(new(1257, -21, 34)),
            new(new(1262, -27, 34)),
            new(new(1270, -28, 34)),
            new(new(1278, -27, 34)),
            new(new(1286, -27, 34)),
            new(new(1294, -27, 36)),
            new(new(1302, -27, 38)),
            new(new(1310, -28, 40)),
            new(new(1318, -30, 40)),
            new(new(1325, -33, 40)),
            new(new(1331, -38, 40)),
            new(new(1333, -46, 40)),
            new(new(1333, -54, 38)),
            new(new(1333, -61, 35)),
            new(new(1332, -69, 32)),
            new(new(1332, -77, 29)),
            new(new(1332, -85, 27)),
            new(new(1332, -92, 24)),
            new(new(1332, -100, 23)),
            new(new(1332, -108, 23)),
            new(new(1332, -116, 23)),
            new(new(1332, -124, 23)),
            new(new(1331, -132, 23)),
            new(new(1328, -140, 23)),
            new(new(1324, -147, 23)),
            new(new(1320, -154, 23)),
            new(new(1317, -161, 23)),
            new(new(1318, -169, 23)),
            new(new(1321, -176, 23)),
            new(new(1324, -183, 23)),
            new(new(1327, -191, 23)),
            new(new(1328, -199, 23)),
            new(new(1329, -206, 26)),
            new(new(1329, -213, 30)),
            new(new(1328, -220, 34)),
            new(new(1327, -227, 38)),
            new(new(1323, -234, 38)),
            new(new(1315, -236, 38)),
            new(new(1310, -230, 38)),
            new(new(1311, -223, 41)),
            new(new(1310, -216, 44)),
            new(new(1310, -209, 48)),
            new(new(1308, -202, 52)),
            new(new(1305, -195, 52)),
            new(new(1297, -193, 52)),
            new(new(1294, -186, 52)),
            new(new(1294, -178, 52)),
            new(new(1294, -170, 52)),
            new(new(1294, -162, 52)),
            new(new(1297, -155, 52)),
            new(new(1303, -150, 52)),
            new(new(1309, -144, 52)),
            new(new(1315, -139, 52)),
            new(new(1321, -133, 55)),
            new(new(1326, -127, 57)),
            new(new(1329, -120, 57)),
            new(new(1332, -112, 57)),
            new(new(1333, -104, 57)),
            new(new(1327, -110, 57)),
            new(new(1326, -118, 57)),
            new(new(1323, -125, 57)),
            new(new(1319, -132, 55)),
            new(new(1314, -138, 53)),
            new(new(1307, -143, 52)),
            new(new(1300, -146, 52)),
            new(new(1295, -152, 52)),
            new(new(1289, -157, 52)),
            new(new(1282, -162, 52)),
            new(new(1275, -165, 52)),
            new(new(1267, -165, 52)),
            new(new(1259, -165, 52)),
            new(new(1253, -160, 52)),
            new(new(1251, -152, 52)),
            new(new(1243, -152, 52)),
            new(new(1235, -154, 53)),
            new(new(1227, -153, 53)),
            new(new(1219, -151, 52)),
            new(new(1211, -152, 52)),
            new(new(1203, -153, 53)),
            new(new(1195, -153, 53)),
            new(new(1187, -152, 52)),
            new(new(1179, -150, 53)),
            new(new(1171, -152, 52)),
            new(new(1166, -158, 52)),
            new(new(1166, -166, 52)),
            new(new(1168, -174, 52)),
            new(new(1169, -182, 53)),
            new(new(1169, -190, 52)),
            new(new(1167, -198, 52)),
            new(new(1168, -206, 52)),
            new(new(1169, -214, 52)),
            new(new(1171, -222, 53)),
            new(new(1172, -230, 52)),
            new(new(1173, -238, 52)),
            new(new(1175, -246, 52)),
            new(new(1176, -254, 52)),
            new(new(1171, -260, 52)),
            new(new(1163, -261, 52)),
            new(new(1155, -261, 52)),
            new(new(1147, -259, 52)),
            new(new(1140, -255, 53)),
            new(new(1133, -251, 55)),
            new(new(1126, -248, 57)),
            new(new(1119, -251, 57)),
            new(new(1112, -254, 57)),
            new(new(1104, -256, 57)),
            new(new(1096, -257, 57)),
            new(new(1089, -259, 60)),
            new(new(1082, -254, 60)),
            new(new(1078, -247, 61)),
            new(new(1076, -239, 61)),
            new(new(1073, -232, 61)),
            new(new(1073, -224, 61)),
            new(new(1074, -216, 61)),
            new(new(1080, -210, 61)),
            new(new(1084, -203, 61)),
            new(new(1088, -196, 61)),
            new(new(1090, -189, 58)),
            new(new(1091, -181, 57)),
            new(new(1091, -173, 57)),
            new(new(1093, -165, 58)),
            new(new(1097, -159, 61)),
            new(new(1100, -152, 61)),
            new(new(1100, -144, 61)),
            new(new(1099, -136, 61)),
            new(new(1099, -128, 61)),
            new(new(1102, -121, 61)),
            new(new(1107, -115, 61)),
            new(new(1105, -107, 61)),
            new(new(1103, -99, 61)),
            new(new(1100, -92, 60)),
            new(new(1096, -85, 59)),
            new(new(1093, -78, 59)),
            new(new(1088, -71, 60)),
            new(new(1082, -65, 61)),
            new(new(1077, -59, 61)),
            new(new(1075, -51, 61)),
            new(new(1076, -43, 61)),
            new(new(1076, -35, 61)),
            new(new(1076, -27, 61)),
            new(new(1073, -20, 61)),
            new(new(1067, -15, 61)),
            new(new(1060, -10, 61)),
            new(new(1054, -5, 61)),
            new(new(1048, 0, 61)),
            new(new(1042, 6, 61)),
            new(new(1037, 12, 61)),
            new(new(1034, 19, 59)),
            new(new(1033, 27, 58)),
            new(new(1039, 32, 58)),
            new(new(1046, 36, 58)),
            new(new(1052, 41, 57)),
            new(new(1058, 46, 54)),
            new(new(1064, 51, 53)),
            new(new(1070, 56, 53)),
            new(new(1076, 62, 53)),
            new(new(1083, 65, 53)),
            new(new(1091, 63, 53)),
            new(new(1097, 58, 53)),
            new(new(1103, 53, 53)),
            new(new(1109, 47, 53)),
            new(new(1115, 42, 55)),
            new(new(1122, 39, 57)),
            new(new(1130, 36, 58)),
            new(new(1138, 36, 58)),
            new(new(1146, 36, 59)),
            new(new(1154, 36, 61)),
            new(new(1162, 36, 61)),
            new(new(1170, 36, 61)),
            new(new(1178, 35, 61)),
            new(new(1180, 27, 61)),
            new(new(1183, 19, 61)),
            new(new(1191, 20, 61)),
            new(new(1198, 24, 61)),
            new(new(1205, 27, 61)),
            new(new(1212, 30, 61)),
            new(new(1219, 33, 61)),
            new(new(1227, 34, 61)),
            new(new(1235, 34, 58)),
            new(new(1243, 34, 58)),
        };

        /// <summary>
        /// Gets the list of priority units.
        /// </summary>
        public List<int> PriorityUnits { get; } = new();

        /// <summary>
        /// Gets the required item level.
        /// </summary>
        public int RequiredItemLevel { get; } = 120;

        /// <summary>
        /// Gets the required level for the property.
        /// </summary>
        public int RequiredLevel { get; } = 75;

        /// <summary>
        /// Gets the world entry position as a Vector3.
        /// The default entry position is set to (9186, -1387, 1110).
        /// </summary>
        public Vector3 WorldEntry { get; } = new(9186, -1387, 1110);

        /// <summary>
        /// Gets or sets the unique identifier of the world entry map in Northrend.
        /// </summary>
        public WowMapId WorldEntryMapId { get; } = WowMapId.Northrend;
    }
}