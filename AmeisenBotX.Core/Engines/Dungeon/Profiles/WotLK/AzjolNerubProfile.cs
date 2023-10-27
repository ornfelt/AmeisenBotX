﻿using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Dungeon.Enums;
using AmeisenBotX.Core.Engines.Dungeon.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Dungeon.Profiles.WotLK
{
    /// <summary>
    /// Represents a profile for the Azjol Nerub Dungeon.
    /// </summary>
    public class AzjolNerubProfile : IDungeonProfile
    {
        /// <summary>
        /// Gets the author of the code.
        /// </summary>
        public string Author { get; } = "Jannis";

        /// <summary>
        /// Gets the description of the Dungeon profile.
        /// </summary>
        /// <value>
        /// Profile description.
        /// </value>
        public string Description { get; } = "Profile for the Dungeon in Dragonblight, made for Level 70 to 80.";

        /// <summary>
        /// Gets the position of the dungeon exit.
        /// </summary>
        public Vector3 DungeonExit { get; } = new(406, 800, 832);

        /// <summary>
        /// Gets the faction type of the dungeon.
        /// </summary>
        public DungeonFactionType FactionType { get; } = DungeonFactionType.Neutral;

        /// <summary>
        /// Gets or sets the size of the group. The default value is 5.
        /// </summary>
        public int GroupSize { get; } = 5;

        /// <summary>
        /// Gets the MapId of WowMapId.AzjolNerub.
        /// </summary>
        public WowMapId MapId { get; } = WowMapId.AzjolNerub;

        /// <summary>
        /// Gets the maximum level which is set to 80.
        /// </summary>
        public int MaxLevel { get; } = 80;

        /// <summary>
        /// Gets the name of the location, which is "[70-80] Azjol Nerub".
        /// </summary>
        public string Name { get; } = "[70-80] Azjol Nerub";

        /// <summary>
        /// Gets or sets the list of DungeonNodes in the code.
        /// </summary>
        public List<DungeonNode> Nodes { get; private set; } = new()
        {
            new(new(413, 796, 831)),
            new(new(418, 790, 830)),
            new(new(423, 784, 828)),
            new(new(429, 779, 827)),
            new(new(435, 774, 827)),
            new(new(442, 770, 827)),
            new(new(449, 766, 827)),
            new(new(456, 763, 825)),
            new(new(464, 760, 824)),
            new(new(472, 759, 823)),
            new(new(479, 757, 821)),
            new(new(486, 755, 818)),
            new(new(494, 754, 815)),
            new(new(501, 753, 811)),
            new(new(508, 752, 808)),
            new(new(515, 751, 804)),
            new(new(522, 749, 800)),
            new(new(528, 747, 795)),
            new(new(535, 745, 790)),
            new(new(541, 742, 786)),
            new(new(547, 739, 782)),
            new(new(551, 734, 778)),
            new(new(552, 726, 777)),
            new(new(549, 719, 777)),
            new(new(546, 712, 777)),
            new(new(542, 705, 777)),
            new(new(539, 698, 776)),
            new(new(536, 691, 775)),
            new(new(533, 684, 775)),
            new(new(531, 676, 775)),
            new(new(529, 668, 776)),
            new(new(530, 660, 776)),
            new(new(530, 652, 777)),
            new(new(530, 644, 777)),
            new(new(530, 636, 777)),
            new(new(530, 628, 778)),
            new(new(531, 620, 778)),
            new(new(531, 612, 778)),
            new(new(531, 604, 777)),
            new(new(531, 596, 777)),
            new(new(531, 588, 777)),
            new(new(531, 580, 777)),
            new(new(531, 572, 776)),
            new(new(531, 564, 775)),
            new(new(531, 557, 772)),
            new(new(530, 550, 769)),
            new(new(531, 542, 768)),
            new(new(531, 534, 766)),
            new(new(530, 527, 761)),
            new(new(530, 520, 757)),
            new(new(530, 513, 754)),
            new(new(531, 505, 754)),
            new(new(536, 499, 755)),
            new(new(543, 495, 758)),
            new(new(548, 500, 753)),
            new(new(549, 505, 747)),
            new(new(548, 512, 744)),
            new(new(546, 519, 742)),
            new(new(542, 526, 739)),
            new(new(538, 532, 736)),
            new(new(534, 539, 734)),
            new(new(529, 545, 732)),
            new(new(528, 553, 732)),
            new(new(534, 558, 733)),
            new(new(541, 561, 732)),
            new(new(548, 564, 731)),
            new(new(555, 568, 729)),
            new(new(562, 571, 728)),
            new(new(569, 574, 727)),
            new(new(577, 576, 727)),
            new(new(585, 577, 726)),
            new(new(593, 576, 725)),
            new(new(601, 575, 723)),
            new(new(607, 570, 721)),
            new(new(612, 564, 719)),
            new(new(615, 557, 716)),
            new(new(618, 550, 712)),
            new(new(620, 543, 708)),
            new(new(621, 536, 704)),
            new(new(620, 529, 699)),
            new(new(616, 523, 695)),
            new(new(611, 517, 695)),
            new(new(605, 512, 695)),
            new(new(597, 512, 695)),
            new(new(589, 511, 695)),
            new(new(582, 512, 698)),
            new(new(574, 512, 698)),
            new(new(566, 512, 699)),
            new(new(559, 513, 695)),
            new(new(551, 514, 695)),
            new(new(546, 519, 692)),
            new(new(539, 524, 689)),
            new(new(534, 528, 685)),
            new(new(529, 532, 681)),
            new(new(525, 537, 677)),
            new(new(520, 543, 675)),
            new(new(522, 550, 672)),
            new(new(523, 558, 670)),
            new(new(518, 562, 666)),
            new(new(518, 561, 658)),
            new(new(520, 557, 651)),
            new(new(522, 550, 647)),
            new(new(527, 544, 646)),
            new(new(532, 549, 643)),
            new(new(536, 514, 290)),
            new(new(537, 506, 290)),
            new(new(539, 498, 290)),
            new(new(541, 490, 289)),
            new(new(543, 482, 289)),
            new(new(544, 474, 289)),
            new(new(546, 466, 289)),
            new(new(547, 458, 288)),
            new(new(549, 450, 287)),
            new(new(549, 442, 285)),
            new(new(549, 434, 285)),
            new(new(549, 426, 285)),
            new(new(549, 418, 284)),
            new(new(548, 410, 283)),
            new(new(548, 402, 280)),
            new(new(549, 396, 275)),
            new(new(549, 391, 269)),
            new(new(549, 385, 263)),
            new(new(549, 380, 257)),
            new(new(549, 374, 252)),
            new(new(549, 369, 246)),
            new(new(550, 363, 241)),
            new(new(550, 355, 241)),
            new(new(550, 347, 241)),
            new(new(550, 339, 241)),
            new(new(550, 331, 241)),
            new(new(550, 324, 238)),
            new(new(550, 317, 235)),
            new(new(550, 310, 232)),
            new(new(550, 302, 230)),
            new(new(551, 294, 228)),
            new(new(551, 286, 226)),
            new(new(551, 278, 224)),
            new(new(551, 270, 223)),
            new(new(551, 262, 224)),
            new(new(551, 254, 224)),
            new(new(550, 246, 223)),
            new(new(557, 242, 223)),
            new(new(563, 247, 223)),
            new(new(556, 250, 224)),
            new(new(550, 245, 223)),
            new(new(550, 237, 224)),
            new(new(550, 229, 224)),
            new(new(550, 222, 220)),
            new(new(550, 215, 216)),
            new(new(549, 208, 213)),
            new(new(550, 201, 209)),
            new(new(550, 194, 206)),
            new(new(549, 187, 202)),
            new(new(549, 180, 199)),
            new(new(549, 173, 196)),
            new(new(549, 165, 196)),
            new(new(549, 157, 196)),
            new(new(549, 149, 197)),
            new(new(548, 141, 198)),
            new(new(548, 133, 198)),
            new(new(548, 125, 198)),
            new(new(548, 117, 197)),
            new(new(548, 109, 195)),
            new(new(548, 101, 195)),
            new(new(545, 95, 199)),
            new(new(539, 91, 202)),
            new(new(533, 89, 207)),
            new(new(526, 89, 212)),
            new(new(519, 88, 216)),
            new(new(512, 88, 220)),
            new(new(505, 87, 224)),
            new(new(498, 87, 228)),
            new(new(491, 86, 232)),
            new(new(484, 86, 235)),
            new(new(477, 84, 238)),
            new(new(469, 83, 240)),
            new(new(462, 81, 242)),
            new(new(455, 78, 243)),
            new(new(448, 75, 244)),
            new(new(441, 72, 245)),
            new(new(434, 68, 246)),
            new(new(427, 65, 246)),
            new(new(420, 61, 246)),
            new(new(413, 58, 249)),
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
        /// Gets the required level for a certain action or attribute.
        /// </summary>
        public int RequiredLevel { get; } = 70;

        /// <summary>
        /// Gets the world entry point as a Vector3.
        /// The default value is set to (3672, 2170, 36).
        /// </summary>
        public Vector3 WorldEntry { get; } = new(3672, 2170, 36);

        ///<summary>
        ///Gets or sets the World Entry Map ID for the current WowMapId.
        ///</summary>
        public WowMapId WorldEntryMapId { get; } = WowMapId.Northrend;
    }
}