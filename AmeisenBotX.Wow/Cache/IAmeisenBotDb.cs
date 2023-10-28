using AmeisenBotX.Common.Math;
using AmeisenBotX.Wow.Cache.Enums;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;

/// <summary>
/// Interface for a database that stores various data related to a bot's interactions with the game.
/// </summary>
namespace AmeisenBotX.Wow.Cache
{
    /// <summary>
    /// Interface for a database that stores various data related to a bot's interactions with the game.
    /// </summary>
    public interface IAmeisenBotDb
    {
        /// <summary>
        /// Retrieves all the blacklisted nodes stored in a read-only dictionary.
        /// The keys of the dictionary represent unique integer values,
        /// while the associated values are lists of Vector3 objects.
        /// </summary>
        /// <returns>A read-only dictionary containing the blacklisted nodes.</returns>
        IReadOnlyDictionary<int, List<Vector3>> AllBlacklistNodes();

        /// <summary>
        /// Retrieves all the herb nodes in the game world, organized by map ID, herb ID, and position.
        /// Returns an immutable dictionary where each map ID is mapped to a dictionary of herb IDs, which is then mapped to a list of positions.
        /// </summary>
        IReadOnlyDictionary<WowMapId, Dictionary<WowHerbId, List<Vector3>>> AllHerbNodes();

        /// <summary>
        /// Returns an immutable dictionary that contains all the names
        /// associated with their corresponding ulong values.
        /// </summary>
        IReadOnlyDictionary<ulong, string> AllNames();

        /// <summary>
        /// Returns a read-only dictionary containing all the ore nodes in the world map.
        /// The dictionary is indexed by WowMapId and each map id is associated with a dictionary of WowOreId as key and a list of Vector3 as value.
        /// </summary>
        IReadOnlyDictionary<WowMapId, Dictionary<WowOreId, List<Vector3>>> AllOreNodes();

        /// <summary>
        /// Gets all the points of interest in the form of a read-only dictionary, where the keys are WowMapIds and the values are dictionaries. 
        /// Each inner dictionary contains PoiTypes as keys and lists of Vector3 positions as values.
        /// </summary>
        IReadOnlyDictionary<WowMapId, Dictionary<PoiType, List<Vector3>>> AllPointsOfInterest();

        /// <summary>
        /// Returns an immutable dictionary that contains all the reactions of the WoW units.
        /// The dictionary key is an integer representing the ID of the WoW unit.
        /// The dictionary value is a nested dictionary that maps another integer to an instance of 
        /// the WowUnitReaction class, representing the reaction of the WoW unit with another unit.
        /// </summary>
        IReadOnlyDictionary<int, Dictionary<int, WowUnitReaction>> AllReactions();

        /// <summary>
        /// Retrieves all spell names as an read-only dictionary.
        /// The keys are integers that represent the spell's ID.
        /// The values are strings that represent the spell names.
        /// </summary>
        IReadOnlyDictionary<int, string> AllSpellNames();

        /// <summary>
        /// Caches a specific herb at the given position on the WoW map.
        /// </summary>
        void CacheHerb(WowMapId mapId, WowHerbId displayId, Vector3 position);

        /// <summary>
        /// Caches the specified ore at the given position on the specified map.
        /// </summary>
        void CacheOre(WowMapId mapId, WowOreId displayId, Vector3 position);

        /// <summary>
        /// Caches the Point of Interest (POI) of a specified map at a given position.
        /// </summary>
        /// <param name="mapId">The ID of the WoW map.</param>
        /// <param name="poiType">The type of the POI.</param>
        /// <param name="position">The position where the POI is located.</param>
        void CachePoi(WowMapId mapId, PoiType poiType, Vector3 position);

        /// <summary>
        /// Clears all data and resets the state of the program.
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets the reaction between two World of Warcraft units.
        /// </summary>
        WowUnitReaction GetReaction(IWowUnit a, IWowUnit b);

        /// <summary>
        /// Retrieves the name of a spell based on its unique identifier.
        /// </summary>
        /// <param name="spellId">The unique identifier of the spell.</param>
        /// <returns>The name of the spell as a string.</returns>
        string GetSpellName(int spellId);

        /// <summary>
        /// Retrieves the name of the given World of Warcraft unit.
        /// </summary>
        /// <param name="unit">The unit object for which the name needs to be retrieved.</param>
        /// <param name="name">The output parameter which will store the name of the unit.</param>
        /// <returns>Returns true if the unit name is successfully retrieved, false otherwise.</returns>
        bool GetUnitName(IWowUnit unit, out string name);

        /// <summary>
        /// Saves the specified database file.
        /// </summary>
        /// <param name="dbFile">The path of the database file to save.</param>
        void Save(string dbFile);

        /// <summary>
        /// Tries to retrieve the blacklist position for a given map ID, position, and maximum radius.
        /// </summary>
        /// <param name="mapId">The ID of the map to query.</param>
        /// <param name="position">The position to check for blacklist nodes.</param>
        /// <param name="maxRadius">The maximum radius of the area to search for blacklist nodes.</param>
        /// <param name="nodes">An out parameter that returns the collection of blacklist nodes found within the specified area.</param>
        /// <returns>A boolean value indicating whether any blacklist nodes were found within the specified area.</returns>
        bool TryGetBlacklistPosition(int mapId, Vector3 position, float maxRadius, out IEnumerable<Vector3> nodes);

        /// <summary>
        /// Tries to get the points of interest within a specified radius around a given position on a given map.
        /// </summary>
        /// <param name="mapId">The ID of the map.</param>
        /// <param name="poiType">The type of points of interest.</param>
        /// <param name="position">The position in the map to search around.</param>
        /// <param name="maxRadius">The maximum radius in which to search for points of interest.</param>
        /// <param name="nodes">The IEnumerable container that will hold the found points of interest.</param>
        /// <returns>A boolean value indicating if the operation was successful.</returns>
        bool TryGetPointsOfInterest(WowMapId mapId, PoiType poiType, Vector3 position, float maxRadius, out IEnumerable<Vector3> nodes);
    }
}