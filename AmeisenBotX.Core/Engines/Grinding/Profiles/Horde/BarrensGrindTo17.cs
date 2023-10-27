using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Grinding.Objects;
using AmeisenBotX.Core.Objects;
using AmeisenBotX.Core.Objects.Enums;
using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Grinding.Profiles.Horde
{
    /// <summary>
    /// Represents a grinding profile for leveling up to level 17 in the Barrens.
    /// </summary>
    public class BarrensGrindTo17 : IGrindingProfile
    {
        /// <summary>
        /// Gets the list of non-player characters (NPCs) of interest.
        /// </summary>
        public List<Npc> NpcsOfInterest { get; } = new()
        {
            new Npc("Nargal Deatheye", 3479,
                WowMapId.Kalimdor, WowZoneId.TheCrossroads, new Vector3(-356, -2568, 95),
                NpcType.VendorRepair)
        };

        /// <summary>
        /// Gets a list of interactable objects of interest.
        /// </summary>
        public List<InteractableObject> ObjectsOfInterest { get; } = new()
        {
            new InteractableObject(143982,
                WowMapId.Kalimdor, WowZoneId.RazorHill, new Vector3(-443, -2649, 95),
                InteractableObjectType.Mailbox, MailboxFactionType.Horde)
        };

        /// <summary>
        /// Gets or sets the value indicating whether the spots are randomized.
        /// By default, the value is set to false.
        /// </summary>
        public bool RandomizeSpots => false;

        /// <summary>
        /// This property represents a list of GrindingSpot objects, which are locations in the game world where grinding can take place.
        /// Each GrindingSpot object is initialized with a Vector3 representing its position, a float representing its grinding range, and two integers representing its minimum and maximum levels.
        /// The list is initialized with six GrindingSpot objects, each with different position, range, and level values.
        /// </summary>
        public List<GrindingSpot> Spots { get; } = new()
        {
            new GrindingSpot(new Vector3(-314, -2712, 93), 50.0f, 11, 17),
            new GrindingSpot(new Vector3(-310, -2821, 92), 50.0f, 11, 17),
            new GrindingSpot(new Vector3(-181, -2905, 92), 50.0f, 11, 17),
            new GrindingSpot(new Vector3(-112, -2972, 91), 50.0f, 11, 17),
            new GrindingSpot(new Vector3(3, -3090, 91), 50.0f, 11, 17),
            new GrindingSpot(new Vector3(-9, -3201, 91), 50.0f, 11, 17)
        };

        /// <summary>
        /// Overrides the default ToString() method to return a string representation of the current object.
        /// </summary>
        /// <returns>A string representation of the object, specifically "[H][Durotar] 14 To 17 Grinding".</returns>
        public override string ToString()
        {
            return "[H][Durotar] 14 To 17 Grinding";
        }
    }
}