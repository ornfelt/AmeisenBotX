﻿using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Dungeon.Enums;
using AmeisenBotX.Core.Engines.Dungeon.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Dungeon.Profiles.Classic
{
    /// <summary>
    /// Represents a profile for the Ragefire Chasm Dungeon in Orgrimmar, designed for players between level 13 to 18.
    /// </summary>
    public class RagefireChasmProfile : IDungeonProfile
    {
        /// <summary>
        /// Gets the author of the code.
        /// </summary>
        public string Author { get; } = "Jannis";

        /// <summary>
        /// Gets the description of the Dungeon Profile.
        /// </summary>
        /// <value>
        /// A string representing the description of the Dungeon Profile.
        /// </value>
        public string Description { get; } = "Profile for the Dungeon in Orgrimmar, made for Level 13 to 18.";

        /// <summary>
        /// Gets the vector representing the position of the dungeon exit.
        /// </summary>
        public Vector3 DungeonExit { get; } = new(3, -4, -16);

        /// <summary>
        /// Gets the faction type of the dungeon.
        /// </summary>
        public DungeonFactionType FactionType { get; } = DungeonFactionType.Horde;

        /// <summary>
        /// Gets the size of the group.
        /// </summary>
        public int GroupSize { get; } = 5;

        /// <summary>
        /// Gets the Map ID for the Ragefire Chasm.
        /// </summary>
        public WowMapId MapId { get; } = WowMapId.RagefireChasm;

        /// <summary>
        /// Gets the maximum level.
        /// </summary>
        public int MaxLevel { get; } = 18;

        /// <summary>
        /// Gets the name of the instance called "[13-18] Ragefire Chasm".
        /// </summary>
        public string Name { get; } = "[13-18] Ragefire Chasm";

        /// <summary>
        /// A list of DungeonNode objects representing various nodes in a dungeon.
        /// Each DungeonNode object is initialized with a set of coordinates.
        /// The list is pre-populated with a set of DungeonNode objects.
        /// </summary>
        public List<DungeonNode> Nodes { get; } = new()
        {
            new(new(4, -15, -18)),
            new(new(1, -23, -20)),
            new(new(-1, -30, -22)),
            new(new(-4, -37, -22)),
            new(new(-7, -44, -22)),
            new(new(-16, -46, -22)),
            new(new(-24, -47, -22)),
            new(new(-32, -45, -22)),
            new(new(-41, -43, -22)),
            new(new(-49, -40, -22)),
            new(new(-57, -38, -20)),
            new(new(-64, -36, -18)),
            new(new(-72, -35, -19)),
            new(new(-81, -35, -20)),
            new(new(-88, -36, -24)),
            new(new(-95, -36, -28)),
            new(new(-104, -35, -30)),
            new(new(-113, -35, -32)),
            new(new(-122, -34, -33)),
            new(new(-130, -34, -33)),
            new(new(-139, -34, -33)),
            new(new(-148, -33, -35)),
            new(new(-156, -32, -38)),
            new(new(-165, -31, -41)),
            new(new(-174, -30, -43)),
            new(new(-183, -31, -45)),
            new(new(-192, -33, -46)),
            new(new(-200, -34, -48)),
            new(new(-207, -35, -51)),
            new(new(-215, -35, -53)),
            new(new(-224, -35, -56)),
            new(new(-233, -36, -57)),
            new(new(-241, -36, -58)),
            new(new(-248, -38, -60)),
            new(new(-256, -40, -61)),
            new(new(-263, -43, -61)),
            new(new(-270, -48, -61)),
            new(new(-277, -52, -61)),
            new(new(-285, -54, -61)),
            new(new(-292, -50, -61)),
            new(new(-298, -45, -61)),
            new(new(-301, -36, -61)),
            new(new(-304, -28, -60)),
            new(new(-306, -20, -58)),
            new(new(-306, -13, -55)),
            new(new(-305, -5, -52)),
            new(new(-302, 2, -49)),
            new(new(-295, 8, -47)),
            new(new(-286, 8, -46)),
            new(new(-279, 8, -49)),
            new(new(-271, 8, -50)),
            new(new(-263, 8, -50)),
            new(new(-254, 8, -50)),
            new(new(-245, 8, -48)),
            new(new(-237, 8, -45)),
            new(new(-228, 8, -44)),
            new(new(-219, 8, -43)),
            new(new(-211, 7, -40)),
            new(new(-204, 8, -37)),
            new(new(-197, 10, -34)),
            new(new(-189, 11, -33)),
            new(new(-181, 13, -32)),
            new(new(-172, 14, -31)),
            new(new(-165, 14, -28)),
            new(new(-157, 14, -26)),
            new(new(-149, 13, -23)),
            new(new(-142, 11, -21)),
            new(new(-135, 8, -21)),
            new(new(-127, 9, -20)),
            new(new(-120, 12, -19)),
            new(new(-114, 17, -19)),
            new(new(-111, 24, -19)),
            new(new(-108, 31, -18)),
            new(new(-106, 40, -18)),
            new(new(-106, 49, -18)),
            new(new(-109, 57, -19)),
            new(new(-114, 63, -20)),
            new(new(-119, 69, -21)),
            new(new(-127, 74, -22)),
            new(new(-134, 77, -22)),
            new(new(-143, 78, -21)),
            new(new(-152, 77, -21)),
            new(new(-161, 75, -21)),
            new(new(-170, 76, -21)),
            new(new(-179, 77, -22)),
            new(new(-187, 80, -23)),
            new(new(-192, 86, -24)),
            new(new(-195, 95, -25)),
            new(new(-204, 96, -25)),
            new(new(-212, 93, -25)),
            new(new(-220, 93, -25)),
            new(new(-229, 93, -23)),
            new(new(-237, 93, -22)),
            new(new(-246, 93, -23)),
            new(new(-254, 93, -25)),
            new(new(-261, 97, -25)),
            new(new(-264, 104, -25)),
            new(new(-261, 111, -25)),
            new(new(-258, 118, -22)),
            new(new(-253, 126, -20)),
            new(new(-250, 134, -19)),
            new(new(-247, 141, -19)),
            new(new(-245, 149, -19)),
            new(new(-242, 157, -19)),
            new(new(-240, 165, -19)),
            new(new(-238, 174, -19)),
            new(new(-236, 181, -21)),
            new(new(-234, 189, -24)),
            new(new(-232, 197, -25)),
            new(new(-234, 205, -25)),
            new(new(-241, 208, -25)),
            new(new(-249, 210, -23)),
            new(new(-256, 210, -20)),
            new(new(-264, 211, -22)),
            new(new(-271, 212, -25)),
            new(new(-279, 213, -25)),
            new(new(-287, 214, -25)),
            new(new(-296, 216, -25)),
            new(new(-305, 218, -25)),
            new(new(-314, 219, -22)),
            new(new(-322, 219, -21)),
            new(new(-330, 217, -20)),
            new(new(-338, 215, -21)),
            new(new(-345, 211, -21)),
            new(new(-351, 205, -22)),
            new(new(-357, 200, -22)),
            new(new(-360, 193, -22)),
            new(new(-363, 186, -22)),
            new(new(-371, 184, -22)),
            new(new(-374, 193, -22)),
            new(new(-374, 202, -22)),
            new(new(-374, 210, -22)),
            new(new(-366, 213, -22)),
            new(new(-357, 215, -22)),
            new(new(-348, 217, -21)),
            new(new(-340, 219, -21)),
            new(new(-332, 220, -20)),
            new(new(-324, 220, -21)),
            new(new(-315, 220, -22)),
            new(new(-307, 219, -25)),
            new(new(-298, 218, -26)),
            new(new(-289, 215, -25)),
            new(new(-281, 213, -25)),
            new(new(-273, 211, -25)),
            new(new(-264, 209, -22)),
            new(new(-255, 207, -21)),
            new(new(-247, 207, -24)),
            new(new(-239, 210, -25)),
            new(new(-233, 215, -25)),
            new(new(-232, 224, -25)),
            new(new(-237, 231, -24)),
            new(new(-243, 236, -23)),
            new(new(-249, 241, -21)),
            new(new(-257, 246, -20)),
            new(new(-264, 249, -18)),
            new(new(-272, 252, -17)),
            new(new(-280, 253, -17)),
            new(new(-288, 253, -16)),
            new(new(-297, 253, -14)),
            new(new(-306, 252, -13)),
            new(new(-314, 252, -12)),
            new(new(-323, 253, -11)),
            new(new(-332, 256, -10)),
            new(new(-341, 258, -8)),
            new(new(-350, 259, -7)),
            new(new(-359, 260, -6)),
            new(new(-368, 259, -5)),
            new(new(-375, 256, -5)),
            new(new(-382, 251, -5)),
            new(new(-388, 245, -5)),
            new(new(-392, 238, -5)),
            new(new(-396, 231, -3)),
            new(new(-400, 224, -2)),
            new(new(-404, 217, -1)),
            new(new(-407, 210, 1)),
            new(new(-409, 203, 3)),
            new(new(-410, 195, 4)),
            new(new(-410, 186, 6)),
            new(new(-409, 177, 7)),
            new(new(-406, 170, 7)),
            new(new(-403, 163, 8)),
            new(new(-398, 157, 8)),
            new(new(-392, 152, 8)),
            new(new(-386, 147, 8)),
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
        public int RequiredLevel { get; } = 13;

        /// <summary>
        /// Gets the world entry position in a Vector3 format.
        /// </summary>
        public Vector3 WorldEntry { get; } = new(1816, -4422, -19);

        /// <summary>
        /// Gets the WorldEntryMapId of the public WowMapId property.
        /// </summary>
        public WowMapId WorldEntryMapId { get; } = WowMapId.Kalimdor;
    }
}