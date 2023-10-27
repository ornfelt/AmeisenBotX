﻿using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Grinding.Objects;
using AmeisenBotX.Core.Objects;
using AmeisenBotX.Core.Objects.Enums;
using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Grinding.Profiles.Horde
{
    /// <summary>
    /// A list of non-player characters (NPCs) that are of interest.
    /// </summary>
    /// <value>
    /// The list of NPCs of interest.
    /// </value>
    public class DurotarGrindTo11 : IGrindingProfile
    {
        ///<summary>
        ///A list of non-player characters (NPCs) that are of interest.
        ///</summary>
        ///<value>
        ///The list of NPCs of interest.
        ///</value>
        public List<Npc> NpcsOfInterest { get; } = new()
        {
            new Npc("Trayexir", 10369,
                WowMapId.Kalimdor, WowZoneId.SenjinVillage, new Vector3(-769, -4948, 22),
                NpcType.VendorRepair),

            new Npc("Un'Thuwa", 5880,
                WowMapId.Kalimdor, WowZoneId.SenjinVillage, new Vector3(-839, -4939, 20),
                NpcType.ClassTrainer, NpcSubType.MageTrainer),
            new Npc("Tai'jin", 3706,
                WowMapId.Kalimdor, WowZoneId.RazorHill, new Vector3(294, -4831, 10),
                NpcType.ClassTrainer, NpcSubType.PriestTrainer),
            new Npc("Tarshaw Jaggedscar", 3169,
                WowMapId.Kalimdor, WowZoneId.RazorHill, new Vector3(311, -4827, 9),
                NpcType.ClassTrainer, NpcSubType.WarriorTrainer),
            new Npc("Swart", 3173,
                WowMapId.Kalimdor, WowZoneId.RazorHill, new Vector3(307, -4839, 10),
                NpcType.ClassTrainer, NpcSubType.ShamanTrainer),

            new Npc("Lau'Tiki", 5941,
                WowMapId.Kalimdor, WowZoneId.SenjinVillage, new Vector3(-1067, -4778, 17),
                NpcType.ProfessionTrainer, NpcSubType.FishingTrainer)
        };

        /// <summary>
        /// Gets or sets the list of objects of interest that can be interacted with.
        /// </summary>
        public List<InteractableObject> ObjectsOfInterest { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the spots should be randomized.
        /// </summary>
        /// <value>
        ///   <c>false</c> to keep the spots in their current order; otherwise, <c>true</c> to randomize them.
        /// </value>
        public bool RandomizeSpots => false;

        /// <summary>
        /// Generates a list of GrindingSpots.
        /// </summary>
        public List<GrindingSpot> Spots { get; } = new()
        {
            // scorpids/boars
            new GrindingSpot(new Vector3(-678, -4649, 37), 55.0f, 5, 11),
            new GrindingSpot(new Vector3(-756, -4645, 41), 55.0f, 5, 11),
            new GrindingSpot(new Vector3(-762, -4732, 32), 55.0f, 5, 11),
            // centaurs
            new GrindingSpot(new Vector3(-960, -4765, 14), 55.0f, 5, 11),
            new GrindingSpot(new Vector3(-968, -4703, 21), 55.0f, 5, 11)
        };

        /// <summary>
        /// Converts the object to a string representation.
        /// </summary>
        /// <returns>A string representation of the object.</returns>
        public override string ToString()
        {
            return "[H][Durotar] 5 To 11 Grinding";
        }
    }
}