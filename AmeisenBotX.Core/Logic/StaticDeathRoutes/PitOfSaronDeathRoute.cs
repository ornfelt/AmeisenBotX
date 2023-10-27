﻿using AmeisenBotX.Common.Math;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Logic.StaticDeathRoutes
{
    /// <summary>
    /// Gets or sets the death point for the character.
    /// </summary>
    /// <value>
    /// The death point represented by a Vector3 object.
    /// </value>
    public class PitOfSaronDeathRoute : StaticPathDeathRoute
    {
        /// <summary>
        /// Gets or sets the death point for the character.
        /// </summary>
        /// <value>
        /// The death point represented by a Vector3 object.
        /// </value>
        protected override Vector3 DeathPoint { get; } = new(5592, 2010, 514);

        /// <summary>
        /// The path for the object, represented by a list of Vector3 coordinates.
        /// </summary>
        protected override List<Vector3> Path { get; } = new()
        {
            new(6447, 2061, 564),
            new(6436, 2070, 563),
            new(6426, 2077, 564),
            new(6415, 2085, 566),
            new(6406, 2091, 570),
            new(6396, 2095, 575),
            new(6387, 2099, 581),
            new(6375, 2103, 588),
            new(6366, 2107, 594),
            new(6356, 2110, 600),
            new(6344, 2114, 608),
            new(6333, 2117, 615),
            new(6323, 2120, 621),
            new(6311, 2124, 629),
            new(6301, 2127, 634),
            new(6288, 2130, 640),
            new(6277, 2132, 644),
            new(6264, 2135, 648),
            new(6250, 2138, 651),
            new(6239, 2140, 654),
            new(6225, 2143, 657),
            new(6214, 2145, 660),
            new(6203, 2148, 663),
            new(6189, 2151, 666),
            new(6178, 2153, 669),
            new(6164, 2156, 672),
            new(6153, 2158, 675),
            new(6142, 2161, 677),
            new(6131, 2163, 680),
            new(6117, 2165, 683),
            new(6106, 2167, 686),
            new(6094, 2168, 689),
            new(6083, 2170, 692),
            new(6072, 2172, 695),
            new(6058, 2174, 698),
            new(6044, 2176, 702),
            new(6030, 2178, 705),
            new(6016, 2180, 709),
            new(6002, 2182, 713),
            new(5988, 2184, 716),
            new(5974, 2186, 720),
            new(5961, 2188, 723),
            new(5949, 2189, 726),
            new(5936, 2190, 730),
            new(5922, 2189, 734),
            new(5911, 2188, 738),
            new(5899, 2187, 742),
            new(5886, 2186, 746),
            new(5875, 2185, 750),
            new(5864, 2184, 754),
            new(5853, 2183, 758),
            new(5842, 2182, 762),
            new(5828, 2181, 766),
            new(5817, 2180, 770),
            new(5804, 2179, 774),
            new(5792, 2178, 778),
            new(5779, 2177, 783),
            new(5765, 2176, 787),
            new(5754, 2176, 791),
            new(5743, 2175, 795),
            new(5732, 2173, 798),
            new(5721, 2172, 802),
            new(5707, 2170, 804),
            new(5693, 2165, 805),
            new(5688, 2152, 805),
            new(5688, 2137, 803),
            new(5689, 2123, 803),
            new(5689, 2109, 804),
            new(5683, 2099, 805),
            new(5672, 2095, 805),
            new(5658, 2097, 804),
            new(5644, 2100, 804),
            new(5632, 2093, 805),
            new(5634, 2079, 805),
            new(5636, 2064, 805),
            new(5633, 2050, 804),
            new(5625, 2039, 802),
            new(5616, 2031, 799),
            new(5605, 2022, 798),
            new(5596, 2014, 798),
            new(5587, 2005, 798),
        };
    }
}