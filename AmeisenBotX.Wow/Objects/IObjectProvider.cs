using AmeisenBotX.Common.Math;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow.Objects.Raw;
using System;
using System.Collections.Generic;

namespace AmeisenBotX.Wow.Objects
{
    public interface IObjectProvider
    {
        /// <summary>
        /// Gets fired when we updated all objects successfully.
        /// </summary>
        event Action<IEnumerable<IWowObject>> OnObjectUpdateComplete;

        /// <summary>
        /// Gets or sets an enumerable collection of IWowObject.
        /// </summary>
        IEnumerable<IWowObject> All { get; }

        /// <summary>
        /// Returns wow's camera information.
        /// </summary>
        RawCameraInfo Camera { get; }

        /// <summary>
        /// Gets the center position of the party in 3D space.
        /// </summary>
        Vector3 CenterPartyPosition { get; }

        /// <summary>
        /// Contains the state of the game while in GlueXML start screen.
        /// </summary>
        string GameState { get; }

        /// <summary>
        /// Gets a value indicating whether the target is in the line of sight.
        /// </summary>
        bool IsTargetInLineOfSight { get; }

        /// <summary>
        /// Gets a value indicating whether the world is loaded.
        /// </summary>
        bool IsWorldLoaded { get; }

        /// <summary>
        /// Gets the last target of the WowUnit.
        /// </summary>
        IWowUnit LastTarget { get; }

        /// <summary>
        /// Gets the WowMapId of the current instance.
        /// </summary>
        WowMapId MapId { get; }

        /// <summary>
        /// Gets the count of objects.
        /// </summary>
        int ObjectCount { get; }

        /// <summary>
        /// Gets or sets the WoW unit representing the party leader.
        /// </summary>
        IWowUnit Partyleader { get; }

        /// <summary>
        /// Gets the collection of party member GUIDs.
        /// </summary>
        IEnumerable<ulong> PartymemberGuids { get; }

        /// <summary>
        /// Gets the collection of party members.
        /// </summary>
        /// <returns>
        /// An IEnumerable containing the party members.
        /// </returns>
        IEnumerable<IWowUnit> Partymembers { get; }

        /// <summary>
        /// Gets the collection of unique identifiers for the party's pet entities.
        /// </summary>
        /// <returns>An enumerable collection of type <see cref="ulong"/> representing the party pet GUIDs.</returns>
        IEnumerable<ulong> PartyPetGuids { get; }

        /// <summary>
        /// Gets an enumerable collection of party pets that implement the IWowUnit interface.
        /// </summary>
        IEnumerable<IWowUnit> PartyPets { get; }

        /// <summary>
        /// Gets the WowUnit Pet.
        /// </summary>
        IWowUnit Pet { get; }

        /// <summary>
        /// Gets or sets the interface for a World of Warcraft player.
        /// </summary>
        IWowPlayer Player { get; }

        /// <summary>
        /// Gets the base address of the player.
        /// </summary>
        IntPtr PlayerBase { get; }

        /// <summary>
        /// Gets the target of the Wow unit.
        /// </summary>
        IWowUnit Target { get; }

        /// <summary>
        /// Represents a WoW unit that is a vehicle.
        /// </summary>
        /// <remarks>
        /// Use this property to access a WoW unit that is a vehicle.
        /// </remarks>
        IWowUnit Vehicle { get; }

        /// <summary>
        /// Gets the ZoneId property.
        /// </summary>
        int ZoneId { get; }

        /// <summary>
        /// Gets the name of the zone.
        /// </summary>
        string ZoneName { get; }

        /// <summary>
        /// Gets the subname of the zone.
        /// </summary>
        string ZoneSubName { get; }
    }
}