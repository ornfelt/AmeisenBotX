using AmeisenBotX.Core.Engines.Grinding.Objects;
using AmeisenBotX.Core.Objects;
using System.Collections.Generic;

/// <summary>
/// Represents a namespace for grinding profiles, which contain a list of NPCs, objects, and spots of interest for grinding.
/// </summary>
namespace AmeisenBotX.Core.Engines.Grinding.Profiles
{
    /// <summary>
    /// Represents a grinding profile, which contains a list of NPCs, objects, and spots of interest for grinding.
    /// </summary>
    public interface IGrindingProfile
    {
        /// <summary>
        /// Gets or sets the list of Non-Player Characters (NPCs) of interest.
        /// </summary>
        List<Npc> NpcsOfInterest { get; }

        /// <summary>
        /// Gets the list of InteractableObject objects that are of interest.
        /// </summary>
        List<InteractableObject> ObjectsOfInterest { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the spots should be randomly generated.
        /// </summary>
        bool RandomizeSpots { get; }

        /// <summary>
        /// Gets the list of grinding spots.
        /// </summary>
        List<GrindingSpot> Spots { get; }
    }
}