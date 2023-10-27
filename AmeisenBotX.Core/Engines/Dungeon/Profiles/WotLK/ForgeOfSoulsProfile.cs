﻿using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Dungeon.Enums;
using AmeisenBotX.Core.Engines.Dungeon.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Dungeon.Profiles.WotLK
{
    /// <summary>
    /// Gets the level range of the Dungeon in Icecrown.
    /// </summary>
    /// <returns>
    /// The level range of the Dungeon in Icecrown.
    /// </returns>
    public class ForgeOfSoulsProfile : IDungeonProfile
    {
        /// <summary>
        /// Gets or sets the author of the code.
        /// </summary>
        public string Author { get; } = "Jannis & Kamel";

        /// <summary>
        /// Gets the description of the Dungeon in Icecrown.
        /// </summary>
        /// <remarks>
        /// This profile is specifically made for Level 80 characters.
        /// </remarks>
        /// <returns>
        /// The description of the Dungeon in Icecrown.
        /// </returns>
        public string Description { get; } = "Profile for the Dungeon in Icecrown, made for Level 80.";

        /// <summary>
        /// Gets the position of the dungeon exit as a Vector3.
        /// </summary>
        public Vector3 DungeonExit { get; } = new(4926, 2170, 639);

        /// <summary>
        /// Gets the faction type of the dungeon, which is always neutral.
        /// </summary>
        public DungeonFactionType FactionType { get; } = DungeonFactionType.Neutral;

        /// <summary>
        /// Gets the size of the group.
        /// </summary>
        public int GroupSize { get; } = 5;

        /// <summary>
        /// Gets the map ID for the instance "The Forge of Souls".
        /// </summary>
        public WowMapId MapId { get; } = WowMapId.TheForgeOfSouls;

        /// <summary>
        /// Gets the maximum level allowed.
        /// </summary>
        public int MaxLevel { get; } = 80;

        /// <summary>
        /// Gets the name of the forge of souls.
        /// </summary>
        public string Name { get; } = "[80+] Forge of Souls";

        /// <summary>
        /// Gets the list of DungeonNodes.
        /// </summary>
        /// <returns>A list of DungeonNode objects.</returns>
        public List<DungeonNode> Nodes { get; } = new()
        {
            new(new(4921, 2177, 639)),
            new(new(4918, 2184, 639)),
            new(new(4914, 2191, 639)),
            new(new(4911, 2198, 639)),
            new(new(4911, 2206, 639)),
            new(new(4916, 2212, 639)),
            new(new(4923, 2216, 639)),
            new(new(4930, 2220, 639)),
            new(new(4937, 2224, 639)),
            new(new(4944, 2228, 639)),
            new(new(4951, 2231, 639)),
            new(new(4958, 2235, 639)),
            new(new(4965, 2239, 639)),
            new(new(4973, 2237, 639)),
            new(new(4978, 2231, 639)),
            new(new(4982, 2224, 639)),
            new(new(4985, 2217, 639)),
            new(new(4989, 2210, 639)),
            new(new(4992, 2203, 639)),
            new(new(4995, 2195, 639)),
            new(new(4998, 2188, 639)),
            new(new(5001, 2181, 639)),
            new(new(5005, 2174, 639)),
            new(new(5006, 2166, 639)),
            new(new(5009, 2159, 639)),
            new(new(5012, 2152, 640)),
            new(new(5018, 2147, 642)),
            new(new(5025, 2143, 644)),
            new(new(5032, 2139, 646)),
            new(new(5039, 2136, 648)),
            new(new(5046, 2133, 650)),
            new(new(5053, 2131, 652)),
            new(new(5060, 2128, 652)),
            new(new(5068, 2126, 652)),
            new(new(5076, 2124, 652)),
            new(new(5084, 2122, 652)),
            new(new(5092, 2120, 652)),
            new(new(5100, 2119, 652)),
            new(new(5108, 2118, 652)),
            new(new(5116, 2117, 652)),
            new(new(5124, 2118, 652)),
            new(new(5132, 2120, 652)),
            new(new(5140, 2121, 652)),
            new(new(5148, 2123, 652)),
            new(new(5156, 2125, 652)),
            new(new(5163, 2128, 652)),
            new(new(5170, 2131, 652)),
            new(new(5177, 2135, 652)),
            new(new(5184, 2138, 652)),
            new(new(5190, 2143, 652)),
            new(new(5196, 2148, 652)),
            new(new(5202, 2153, 652)),
            new(new(5208, 2159, 652)),
            new(new(5213, 2165, 653)),
            new(new(5214, 2173, 652)),
            new(new(5214, 2181, 652)),
            new(new(5213, 2189, 653)),
            new(new(5212, 2197, 655)),
            new(new(5210, 2205, 656)),
            new(new(5209, 2213, 657)),
            new(new(5208, 2221, 658)),
            new(new(5207, 2229, 660)),
            new(new(5205, 2237, 661)),
            new(new(5204, 2245, 662)),
            new(new(5202, 2253, 663)),
            new(new(5200, 2261, 665)),
            new(new(5199, 2269, 665)),
            new(new(5194, 2275, 665)),
            new(new(5186, 2276, 665)),
            new(new(5178, 2275, 665)),
            new(new(5170, 2275, 665)),
            new(new(5162, 2276, 665)),
            new(new(5154, 2278, 665)),
            new(new(5147, 2281, 665)),
            new(new(5143, 2288, 665)),
            new(new(5145, 2296, 665)),
            new(new(5146, 2304, 665)),
            new(new(5141, 2310, 666)),
            new(new(5136, 2316, 667)),
            new(new(5130, 2322, 668)),
            new(new(5125, 2328, 668)),
            new(new(5121, 2335, 668)),
            new(new(5119, 2343, 668)),
            new(new(5126, 2346, 668)),
            new(new(5134, 2344, 668)),
            new(new(5142, 2345, 668)),
            new(new(5150, 2346, 668)),
            new(new(5158, 2347, 668)),
            new(new(5166, 2348, 668)),
            new(new(5174, 2349, 668)),
            new(new(5182, 2350, 668)),
            new(new(5190, 2351, 668)),
            new(new(5198, 2352, 668)),
            new(new(5206, 2353, 668)),
            new(new(5214, 2354, 668)),
            new(new(5221, 2358, 668)),
            new(new(5221, 2366, 668)),
            new(new(5218, 2374, 668)),
            new(new(5215, 2382, 668)),
            new(new(5211, 2389, 668)),
            new(new(5212, 2397, 668)),
            new(new(5216, 2404, 670)),
            new(new(5221, 2410, 672)),
            new(new(5226, 2417, 672)),
            new(new(5231, 2423, 672)),
            new(new(5236, 2430, 672)),
            new(new(5241, 2436, 672)),
            new(new(5246, 2443, 672)),
            new(new(5251, 2450, 674)),
            new(new(5256, 2456, 677)),
            new(new(5260, 2463, 678)),
            new(new(5258, 2471, 678)),
            new(new(5253, 2477, 678)),
            new(new(5249, 2484, 678)),
            new(new(5246, 2491, 678)),
            new(new(5244, 2499, 678)),
            new(new(5244, 2507, 678)),
            new(new(5251, 2504, 678)),
            new(new(5259, 2503, 680)),
            new(new(5266, 2503, 683)),
            new(new(5274, 2504, 686)),
            new(new(5282, 2505, 686)),
            new(new(5290, 2506, 686)),
            new(new(5298, 2507, 686)),
            new(new(5306, 2507, 686)),
            new(new(5314, 2508, 686)),
            new(new(5322, 2510, 686)),
            new(new(5329, 2511, 682)),
            new(new(5336, 2512, 679)),
            new(new(5344, 2513, 678)),
            new(new(5345, 2505, 678)),
            new(new(5346, 2497, 678)),
            new(new(5344, 2489, 679)),
            new(new(5342, 2482, 682)),
            new(new(5341, 2474, 685)),
            new(new(5339, 2466, 686)),
            new(new(5339, 2458, 686)),
            new(new(5342, 2451, 686)),
            new(new(5349, 2446, 687)),
            new(new(5356, 2443, 688)),
            new(new(5363, 2440, 690)),
            new(new(5371, 2438, 691)),
            new(new(5379, 2437, 692)),
            new(new(5387, 2437, 694)),
            new(new(5395, 2439, 695)),
            new(new(5403, 2441, 697)),
            new(new(5411, 2443, 698)),
            new(new(5418, 2447, 700)),
            new(new(5425, 2451, 702)),
            new(new(5431, 2456, 704)),
            new(new(5437, 2461, 706)),
            new(new(5443, 2466, 706)),
            new(new(5448, 2472, 706)),
            new(new(5454, 2477, 706)),
            new(new(5460, 2482, 706)),
            new(new(5465, 2489, 706)),
            new(new(5467, 2497, 706)),
            new(new(5471, 2504, 706)),
            new(new(5479, 2505, 706)),
            new(new(5486, 2500, 706)),
            new(new(5492, 2495, 706)),
            new(new(5498, 2490, 706)),
            new(new(5503, 2484, 706)),
            new(new(5509, 2479, 706)),
            new(new(5515, 2474, 706)),
            new(new(5521, 2469, 706)),
            new(new(5527, 2464, 706)),
            new(new(5533, 2459, 706)),
            new(new(5540, 2454, 706)),
            new(new(5547, 2449, 706)),
            new(new(5553, 2444, 706)),
            new(new(5560, 2440, 706)),
            new(new(5566, 2435, 706)),
            new(new(5572, 2430, 706)),
            new(new(5578, 2425, 706)),
            new(new(5585, 2420, 706)),
            new(new(5593, 2423, 706)),
            new(new(5598, 2429, 706)),
            new(new(5603, 2435, 706)),
            new(new(5609, 2440, 706)),
            new(new(5614, 2446, 706)),
            new(new(5619, 2453, 706)),
            new(new(5624, 2459, 706)),
            new(new(5629, 2466, 708)),
            new(new(5634, 2472, 709)),
            new(new(5639, 2479, 709)),
            new(new(5644, 2486, 709)),
            new(new(5649, 2492, 709)),
            new(new(5654, 2498, 709)),
            new(new(5659, 2504, 709)),
            new(new(5664, 2510, 709)),
        };

        /// <summary>
        /// Gets the list of priority units.
        /// </summary>
        public List<int> PriorityUnits { get; } = new() { 30270 };

        /// <summary>
        /// Gets the required item level.
        /// </summary>
        public int RequiredItemLevel { get; } = 200;

        /// <summary>
        /// Gets the required level for this code, which is set to 80.
        /// </summary>
        public int RequiredLevel { get; } = 80;

        ///<summary>
        ///Gets the world entry point as a Vector3.
        ///The entry point coordinates are set to (5675, 1997, 798).
        ///</summary>
        public Vector3 WorldEntry { get; } = new(5675, 1997, 798);

        /// <summary>
        /// Gets the WorldEntryMapId property value.
        /// </summary>
        public WowMapId WorldEntryMapId { get; } = WowMapId.Northrend;
    }
}