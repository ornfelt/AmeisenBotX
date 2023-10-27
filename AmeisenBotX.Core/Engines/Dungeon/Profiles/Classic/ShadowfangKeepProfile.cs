﻿using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Dungeon.Enums;
using AmeisenBotX.Core.Engines.Dungeon.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Dungeon.Profiles.Classic
{
    /// <summary>
    /// Gets the exit coordinates of the Shadowfang Keep dungeon.
    /// </summary>
    public class ShadowfangKeepProfile : IDungeonProfile
    {
        /// <summary>
        /// Gets the author of the code.
        /// </summary>
        public string Author { get; } = "Jannis";

        /// <summary>
        /// Gets or sets the description of the profile for the Dungeon in The Silverpine Forest, made for Level 22 to 30.
        /// </summary>
        public string Description { get; } = "Profile for the Dungeon in The Silverpine Forest, made for Level 22 to 30.";

        /// <summary>
        /// Gets the exit coordinates of the dungeon.
        /// </summary>
        public Vector3 DungeonExit { get; } = new(-231, 2106, 77);

        /// <summary>
        /// Gets or sets the faction type for the dungeon.
        /// </summary>
        public DungeonFactionType FactionType { get; } = DungeonFactionType.Neutral;

        /// <summary>
        /// Gets the size of the group.
        /// </summary>
        public int GroupSize { get; } = 5;

        /// <summary>
        /// Gets the map ID for the Shadowfang Keep in World of Warcraft.
        /// </summary>
        public WowMapId MapId { get; } = WowMapId.ShadowfangKeep;

        /// <summary>
        /// Gets the maximum level value, which is set to 30.
        /// </summary>
        public int MaxLevel { get; } = 30;

        /// <summary>
        /// Gets the name of the location as "[22-30] Shadowfang Keep".
        /// </summary>
        public string Name { get; } = "[22-30] Shadowfang Keep";

        /// <summary>
        /// A list of DungeonNodes representing the nodes in the dungeon.
        /// Each node contains a position (x, y, z).
        /// </summary>
        public List<DungeonNode> Nodes { get; } = new()
        {
            new(new(-229, 2109, 77)),
            new(new(-221, 2110, 77)),
            new(new(-214, 2107, 77)),
            new(new(-209, 2101, 77)),
            new(new(-201, 2099, 77)),
            new(new(-200, 2107, 79)),
            new(new(-207, 2110, 81)),
            new(new(-204, 2117, 81)),
            new(new(-198, 2122, 82)),
            new(new(-193, 2129, 82)),
            new(new(-188, 2136, 82)),
            new(new(-191, 2143, 84)),
            new(new(-198, 2146, 86)),
            new(new(-205, 2147, 89)),
            new(new(-211, 2141, 91)),
            new(new(-219, 2144, 91)),
            new(new(-227, 2147, 91)),
            new(new(-235, 2148, 90)),
            new(new(-239, 2141, 87)),
            new(new(-247, 2144, 87)),
            new(new(-252, 2138, 84)),
            new(new(-254, 2131, 81)),
            new(new(-257, 2123, 81)),
            new(new(-254, 2130, 81)),
            new(new(-252, 2137, 84)),
            new(new(-250, 2144, 87)),
            new(new(-243, 2141, 87)),
            new(new(-236, 2144, 88)),
            new(new(-237, 2151, 91)),
            new(new(-241, 2158, 91)),
            new(new(-238, 2166, 89)),
            new(new(-235, 2173, 85)),
            new(new(-232, 2180, 81)),
            new(new(-229, 2187, 80)),
            new(new(-226, 2194, 80)),
            new(new(-223, 2201, 80)),
            new(new(-220, 2208, 80)),
            new(new(-216, 2215, 80)),
            new(new(-208, 2217, 80)),
            new(new(-200, 2217, 80)),
            new(new(-192, 2217, 80)),
            new(new(-184, 2217, 80)),
            new(new(-176, 2217, 80)),
            new(new(-177, 2225, 79)),
            new(new(-179, 2232, 76)),
            new(new(-180, 2240, 76)),
            new(new(-183, 2247, 76)),
            new(new(-189, 2252, 76)),
            new(new(-196, 2256, 76)),
            new(new(-203, 2260, 76)),
            new(new(-210, 2263, 76)),
            new(new(-218, 2266, 75)),
            new(new(-225, 2270, 75)),
            new(new(-232, 2274, 75)),
            new(new(-239, 2277, 75)),
            new(new(-246, 2280, 75)),
            new(new(-253, 2283, 75)),
            new(new(-260, 2287, 75)),
            new(new(-262, 2295, 75)),
            new(new(-268, 2300, 76)),
            new(new(-275, 2297, 76)),
            new(new(-270, 2291, 76)),
            new(new(-262, 2289, 75)),
            new(new(-269, 2286, 77)),
            new(new(-276, 2289, 81)),
            new(new(-283, 2292, 83)),
            new(new(-288, 2296, 88)),
            new(new(-290, 2303, 91)),
            new(new(-297, 2299, 91)),
            new(new(-301, 2293, 95)),
            new(new(-303, 2285, 96)),
            new(new(-298, 2279, 96)),
            new(new(-290, 2277, 96)),
            new(new(-282, 2275, 96)),
            new(new(-274, 2272, 96)),
            new(new(-266, 2270, 96)),
            new(new(-259, 2268, 100)),
            new(new(-252, 2264, 101)),
            new(new(-247, 2258, 101)),
            new(new(-239, 2258, 101)),
            new(new(-232, 2261, 102)),
            new(new(-224, 2263, 103)),
            new(new(-221, 2256, 103)),
            new(new(-228, 2252, 103)),
            new(new(-236, 2253, 101)),
            new(new(-244, 2254, 101)),
            new(new(-249, 2248, 101)),
            new(new(-252, 2241, 101)),
            new(new(-249, 2233, 100)),
            new(new(-243, 2230, 96)),
            new(new(-236, 2232, 94)),
            new(new(-232, 2225, 94)),
            new(new(-232, 2218, 97)),
            new(new(-235, 2211, 97)),
            new(new(-236, 2203, 97)),
            new(new(-238, 2195, 97)),
            new(new(-241, 2188, 97)),
            new(new(-244, 2181, 94)),
            new(new(-249, 2174, 94)),
            new(new(-252, 2167, 94)),
            new(new(-252, 2159, 94)),
            new(new(-256, 2153, 91)),
            new(new(-261, 2148, 94)),
            new(new(-268, 2150, 96)),
            new(new(-271, 2143, 96)),
            new(new(-264, 2140, 98)),
            new(new(-257, 2138, 100)),
            new(new(-249, 2140, 100)),
            new(new(-242, 2144, 100)),
            new(new(-242, 2136, 100)),
            new(new(-245, 2129, 100)),
            new(new(-253, 2128, 100)),
            new(new(-258, 2122, 100)),
            new(new(-258, 2114, 100)),
            new(new(-250, 2112, 100)),
            new(new(-243, 2109, 99)),
            new(new(-236, 2107, 97)),
            new(new(-229, 2104, 97)),
            new(new(-222, 2101, 97)),
            new(new(-215, 2098, 97)),
            new(new(-207, 2099, 97)),
            new(new(-204, 2106, 97)),
            new(new(-201, 2113, 97)),
            new(new(-199, 2121, 97)),
            new(new(-196, 2128, 97)),
            new(new(-193, 2135, 97)),
            new(new(-191, 2143, 97)),
            new(new(-188, 2150, 97)),
            new(new(-185, 2157, 97)),
            new(new(-183, 2165, 97)),
            new(new(-181, 2173, 97)),
            new(new(-182, 2181, 98)),
            new(new(-175, 2185, 97)),
            new(new(-170, 2179, 95)),
            new(new(-169, 2171, 94)),
            new(new(-162, 2168, 94)),
            new(new(-154, 2166, 94)),
            new(new(-147, 2163, 94)),
            new(new(-139, 2160, 94)),
            new(new(-131, 2162, 94)),
            new(new(-134, 2169, 94)),
            new(new(-140, 2173, 97)),
            new(new(-147, 2174, 100)),
            new(new(-155, 2175, 101)),
            new(new(-156, 2183, 104)),
            new(new(-150, 2188, 106)),
            new(new(-143, 2184, 109)),
            new(new(-136, 2182, 113)),
            new(new(-129, 2179, 113)),
            new(new(-122, 2176, 113)),
            new(new(-115, 2173, 109)),
            new(new(-108, 2170, 107)),
            new(new(-104, 2164, 104)),
            new(new(-106, 2157, 102)),
            new(new(-114, 2155, 102)),
            new(new(-122, 2155, 102)),
            new(new(-130, 2157, 102)),
            new(new(-137, 2161, 105)),
            new(new(-144, 2163, 107)),
            new(new(-151, 2166, 109)),
            new(new(-159, 2169, 109)),
            new(new(-166, 2172, 109)),
            new(new(-174, 2175, 109)),
            new(new(-172, 2183, 110)),
            new(new(-175, 2190, 112)),
            new(new(-183, 2190, 114)),
            new(new(-187, 2183, 115)),
            new(new(-183, 2176, 117)),
            new(new(-175, 2174, 119)),
            new(new(-172, 2181, 120)),
            new(new(-176, 2188, 122)),
            new(new(-184, 2188, 124)),
            new(new(-184, 2180, 126)),
            new(new(-177, 2176, 128)),
            new(new(-171, 2181, 129)),
            new(new(-163, 2180, 129)),
            new(new(-156, 2177, 129)),
            new(new(-149, 2174, 128)),
            new(new(-142, 2171, 128)),
            new(new(-135, 2168, 129)),
            new(new(-128, 2165, 129)),
            new(new(-120, 2163, 129)),
            new(new(-119, 2171, 130)),
            new(new(-118, 2179, 131)),
            new(new(-119, 2187, 132)),
            new(new(-124, 2194, 133)),
            new(new(-130, 2199, 134)),
            new(new(-137, 2202, 135)),
            new(new(-145, 2204, 135)),
            new(new(-153, 2203, 136)),
            new(new(-160, 2200, 137)),
            new(new(-166, 2194, 138)),
            new(new(-171, 2187, 139)),
            new(new(-166, 2181, 139)),
            new(new(-162, 2174, 139)),
            new(new(-161, 2166, 139)),
            new(new(-156, 2160, 139)),
            new(new(-149, 2157, 139)),
            new(new(-141, 2158, 139)),
            new(new(-134, 2161, 139)),
            new(new(-129, 2167, 139)),
            new(new(-121, 2165, 139)),
            new(new(-118, 2172, 140)),
            new(new(-118, 2180, 141)),
            new(new(-120, 2188, 143)),
            new(new(-125, 2194, 144)),
            new(new(-131, 2200, 145)),
            new(new(-138, 2203, 147)),
            new(new(-146, 2204, 148)),
            new(new(-154, 2202, 149)),
            new(new(-161, 2198, 151)),
            new(new(-167, 2192, 152)),
            new(new(-171, 2185, 152)),
            new(new(-165, 2180, 152)),
            new(new(-157, 2177, 153)),
            new(new(-150, 2175, 156)),
            new(new(-143, 2172, 156)),
            new(new(-136, 2169, 156)),
            new(new(-129, 2166, 156)),
            new(new(-122, 2163, 156)),
            new(new(-115, 2160, 156)),
            new(new(-110, 2154, 156)),
            new(new(-113, 2148, 151)),
            new(new(-116, 2142, 147)),
            new(new(-117, 2134, 145)),
            new(new(-109, 2133, 145)),
            new(new(-103, 2138, 145)),
            new(new(-99, 2145, 145)),
            new(new(-91, 2145, 145)),
            new(new(-84, 2141, 147)),
            new(new(-78, 2145, 152)),
            new(new(-77, 2152, 156)),
            new(new(-82, 2158, 156)),
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
        public int RequiredLevel { get; } = 22;

        /// <summary>
        /// Gets the world entry vector in 3D space.
        /// </summary>
        public Vector3 WorldEntry { get; } = new(-232, -1569, 77);

        /// <summary>
        /// Gets the World Entry Map ID for the WoW Map.
        /// </summary>
        public WowMapId WorldEntryMapId { get; } = WowMapId.EasternKingdoms;
    }
}