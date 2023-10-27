using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Grinding.Objects;
using AmeisenBotX.Core.Objects;
using AmeisenBotX.Core.Objects.Enums;
using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Grinding.Profiles.Horde
{
    /// <summary>
    /// Represents a grinding profile for the zone Durotar, used to level up to level 14.
    /// </summary>
    public class DurotarGrindTo14 : IGrindingProfile
    {
        /// <summary>
        /// List of NPCs that are of interest.
        /// </summary>
        public List<Npc> NpcsOfInterest { get; } = new()
        {
            new Npc("Wuark", 3167,
                WowMapId.Kalimdor, WowZoneId.RazorHill, new Vector3(358, -4706, 14),
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

            new Npc("Rawrk", 5943,
                WowMapId.Kalimdor, WowZoneId.RazorHill, new Vector3(330, -4826, 10),
                NpcType.ProfessionTrainer, NpcSubType.FirstAidTrainer)
        };

        /// <summary>
        /// A list of InteractableObjects that represent objects of interest.
        /// </summary>
        public List<InteractableObject> ObjectsOfInterest { get; } = new()
        {
            new InteractableObject(143981,
                WowMapId.Kalimdor, WowZoneId.RazorHill, new Vector3(322, -4706, 14),
                InteractableObjectType.Mailbox, MailboxFactionType.Horde),

            new InteractableObject(106318,
                WowMapId.Kalimdor, WowZoneId.RazorHill, new Vector3(421, -4252, 26),
                InteractableObjectType.Container),
            new InteractableObject(106318,
                WowMapId.Kalimdor, WowZoneId.RazorHill, new Vector3(426, -4280, 29),
                InteractableObjectType.Container),
            new InteractableObject(106318,
                WowMapId.Kalimdor, WowZoneId.RazorHill, new Vector3(440, -4214, 25),
                InteractableObjectType.Container)
        };

        /// <summary>
        /// Gets or sets a value indicating whether the spots should be randomized.
        /// </summary>
        /// <value>
        ///   <c>false</c> if the spots should not be randomized; otherwise, <c>true</c>.
        /// </value>
        public bool RandomizeSpots => false;

        /// <summary>
        /// This list stores the grinding spots for razormane/scorpids/reptiles.
        /// Each grinding spot is represented by a GrindingSpot object that contains its position, maximum distance, and minimum and maximum level range.
        /// </summary>
        public List<GrindingSpot> Spots { get; } = new()
        {
            // razormane/scorpids/reptiles
            new GrindingSpot(new Vector3(393, -4312, 25), 50.0f, 6, 14),
            new GrindingSpot(new Vector3(377, -4236, 24), 50.0f, 6, 14),
            new GrindingSpot(new Vector3(445, -4143, 27), 50.0f, 6, 14),
            new GrindingSpot(new Vector3(533, -4188, 17), 50.0f, 6, 14),
            new GrindingSpot(new Vector3(504, -4300, 21), 50.0f, 6, 14)
        };

        /// <summary>
        /// Converts the object to its string representation.
        /// Returns the string "[H][Durotar] 10 To 14 Grinding".
        /// </summary>
        public override string ToString()
        {
            return "[H][Durotar] 10 To 14 Grinding";
        }
    }
}