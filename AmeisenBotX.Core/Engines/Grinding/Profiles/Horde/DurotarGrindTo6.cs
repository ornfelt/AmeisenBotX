using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Grinding.Objects;
using AmeisenBotX.Core.Objects;
using AmeisenBotX.Core.Objects.Enums;
using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Grinding.Profiles.Horde
{
    public class DurotarGrindTo6 : IGrindingProfile
    {
        /// <summary>
        /// A list of NPCs of interest in the Valley of Trials area of Kalimdor.
        /// </summary>
        /// <remarks>
        /// This list includes NPCs that serve as vendors and buy/sell items to players, as well as class trainers
        /// for various classes such as Priest, Shaman, Warrior, and Mage.
        /// </remarks>
        public List<Npc> NpcsOfInterest { get; } = new()
        {
            new Npc("Duokna", 3158,
                WowMapId.Kalimdor, WowZoneId.ValleyofTrials, new Vector3(-565, -4214, 41),
                NpcType.VendorSellBuy),

            new Npc("Ken'jai", 3707,
                WowMapId.Kalimdor, WowZoneId.ValleyofTrials, new Vector3(-617, -4202, 38),
                NpcType.ClassTrainer, NpcSubType.PriestTrainer),
            new Npc("Shikrik", 3157,
                WowMapId.Kalimdor, WowZoneId.ValleyofTrials, new Vector3(-623, -4203, 38),
                NpcType.ClassTrainer, NpcSubType.ShamanTrainer),
            new Npc("Frang", 3153,
                WowMapId.Kalimdor, WowZoneId.ValleyofTrials, new Vector3(-639, -4230, 38),
                NpcType.ClassTrainer, NpcSubType.WarriorTrainer),
            new Npc("Mai'ah", 5884,
                WowMapId.Kalimdor, WowZoneId.ValleyofTrials, new Vector3(-625, -4210, 38),
                NpcType.ClassTrainer, NpcSubType.MageTrainer)
        };

        /// <summary>
        /// Gets a list of interactable objects of interest.
        /// </summary>
        public List<InteractableObject> ObjectsOfInterest { get; } = new()
        {
            new InteractableObject(3084,
                WowMapId.Kalimdor, WowZoneId.ValleyofTrials, new Vector3(-602, -4250, 37),
                InteractableObjectType.Fire)
        };

        /// <summary>
        /// Gets or sets a value indicating whether the spots should be randomized.
        /// </summary>
        public bool RandomizeSpots => true;

        /// <summary>
        /// Represents a list of GrindingSpots.
        /// </summary>
        /// <remarks>
        /// The list includes GrindingSpots for pigs and scorpids. 
        /// The GrindingSpots for pigs have a Vector3 position of (-546, -4308, 38), (-450, -4258, 48) with a distance of 45.0f,
        /// and a minimumLevel of 1 and maximumLevel of 3.
        /// The GrindingSpots for scorpids have a Vector3 position of (-435, -4154, 52), (-379, -4096, 49), (-399, -4116, 50),
        /// (-284, -4179, 51) with a distance of 55.0f, and a minimumLevel of 2 and maximumLevel of 7.
        /// </remarks>
        public List<GrindingSpot> Spots { get; } = new()
        {
            // pigs
            new GrindingSpot(new Vector3(-546, -4308, 38), 45.0f, 1, 3),
            new GrindingSpot(new Vector3(-450, -4258, 48), 45.0f, 1, 3),
            // scorpids
            new GrindingSpot(new Vector3(-435, -4154, 52), 55.0f, 2, 7),
            new GrindingSpot(new Vector3(-379, -4096, 49), 55.0f, 2, 7),
            new GrindingSpot(new Vector3(-399, -4116, 50), 55.0f, 2, 7),
            new GrindingSpot(new Vector3(-284, -4179, 51), 55.0f, 2, 7),
        };

        /// <summary>
        /// Overrides the ToString method to return the specified string representation.
        /// The returned string is "[H][Durotar] 1 To 6 Grinding".
        /// </summary>
        public override string ToString()
        {
            return "[H][Durotar] 1 To 6 Grinding";
        }
    }
}