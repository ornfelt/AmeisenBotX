using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Logging;
using AmeisenBotX.Logging.Enums;
using AmeisenBotX.Wow.Cache.Enums;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AmeisenBotX.Wow.Cache
{
    /// <summary>
    /// Gets or sets a concurrent dictionary that contains blacklisted nodes with their associated information.
    /// </summary>
    public class LocalAmeisenBotDb : IAmeisenBotDb
    {
        /// <summary>
        /// Constructor for the "FromJson" method
        /// </summary>
        public LocalAmeisenBotDb()
        {
            Clear();
        }

        /// <summary>
        /// Initializes a new instance of the LocalAmeisenBotDb class with the specified WowInterface.
        /// </summary>
        public LocalAmeisenBotDb(IWowInterface wowInterface)
        {
            Wow = wowInterface;

            Clear();
        }

        /// <summary>
        /// Gets or sets a concurrent dictionary that contains blacklisted nodes with their associated lists of Vector3 positions.
        /// </summary>
        public ConcurrentDictionary<int, List<Vector3>> BlacklistNodes { get; set; }

        /// <summary>
        /// Gets or sets the collection of herb nodes, organized by map and herb id, with corresponding positions.
        /// </summary>
        public ConcurrentDictionary<WowMapId, Dictionary<WowHerbId, List<Vector3>>> HerbNodes { get; set; }

        /// <summary>
        /// Gets or sets a thread-safe dictionary that maps ulong keys to string values representing names.
        /// </summary>
        public ConcurrentDictionary<ulong, string> Names { get; set; }

        /// <summary>
        /// Gets or sets the dictionary of ore nodes, grouped by map id and ore id, with each node represented by a list of Vector3 coordinates.
        /// </summary>
        public ConcurrentDictionary<WowMapId, Dictionary<WowOreId, List<Vector3>>> OreNodes { get; set; }

        /// <summary>
        /// Gets or sets the points of interest, organized by map ID and point of interest type,
        /// each containing a list of vectors representing the positions.
        /// </summary>
        public ConcurrentDictionary<WowMapId, Dictionary<PoiType, List<Vector3>>> PointsOfInterest { get; set; }

        /// <summary>
        /// Gets or sets the concurrent dictionary of reactions.
        /// The keys of the dictionary are integers, representing certain identifiers.
        /// The values of the dictionary are dictionaries, where the keys are integers and the values are WowUnitReaction objects.
        /// WowUnitReaction represents the reaction of a WowUnit entity.
        /// </summary>
        public ConcurrentDictionary<int, Dictionary<int, WowUnitReaction>> Reactions { get; set; }

        /// <summary>
        /// Gets or sets the concurrent dictionary of spell names.
        /// </summary>
        public ConcurrentDictionary<int, string> SpellNames { get; set; }

        /// <summary>
        /// Gets or sets the WoW interface.
        /// </summary>
        private IWowInterface Wow { get; set; }

        /// <summary>
        /// Converts a JSON file containing data for a LocalAmeisenBotDb into an instance of the LocalAmeisenBotDb class.
        /// If the directory of the JSON file does not exist, it creates the necessary directory.
        /// If the JSON file exists, it attempts to deserialize the file into an instance of LocalAmeisenBotDb 
        /// using the JsonSerializer class. If successful, it sets the Wow property of the instance to the provided wowInterface
        /// and returns the deserialized instance. 
        /// If there is an error during deserialization, it logs the error and returns a new instance of LocalAmeisenBotDb 
        /// with the provided wowInterface.
        /// </summary>
        /// <param name="dbFile">The path to the JSON file.</param>
        /// <param name="wowInterface">The IWowInterface implementation to assign to the Wow property.</param>
        /// <returns>An instance of LocalAmeisenBotDb.</returns>
        public static LocalAmeisenBotDb FromJson(string dbFile, IWowInterface wowInterface)
        {
            if (!Directory.Exists(Path.GetDirectoryName(dbFile)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(dbFile));
            }
            else if (File.Exists(dbFile))
            {
                AmeisenLogger.I.Log("AmeisenBot", $"Loading LocalAmeisenBotDb from: {dbFile}", LogLevel.Debug);

                try
                {
                    LocalAmeisenBotDb db = JsonSerializer.Deserialize<LocalAmeisenBotDb>(File.ReadAllText(dbFile), new JsonSerializerOptions() { AllowTrailingCommas = true, NumberHandling = JsonNumberHandling.AllowReadingFromString });
                    db.Wow = wowInterface;
                    return db;
                }
                catch (Exception ex)
                {
                    AmeisenLogger.I.Log("Cache", $"Error while loading db:\n{ex}", LogLevel.Debug);
                }
            }

            return new(wowInterface);
        }

        /// <summary>
        /// Gets all the nodes in the blacklist.
        /// </summary>
        /// <returns>A read-only dictionary with integers as keys and lists of Vector3 as values.</returns>
        public IReadOnlyDictionary<int, List<Vector3>> AllBlacklistNodes()
        {
            return BlacklistNodes;
        }

        /// <summary>
        /// Returns a read-only dictionary containing all the herb nodes in the game, organized by map and herb ID.
        /// </summary>
        public IReadOnlyDictionary<WowMapId, Dictionary<WowHerbId, List<Vector3>>> AllHerbNodes()
        {
            return HerbNodes;
        }

        /// <summary>
        /// Returns a read-only dictionary of all names.
        /// </summary>
        public IReadOnlyDictionary<ulong, string> AllNames()
        {
            return Names;
        }

        /// <summary>
        /// Returns all ore nodes in the game, categorized by map and ore type.
        /// </summary>
        /// <returns>A dictionary containing the ore nodes, with WowMapId as the key, and a dictionary of WowOreId and corresponding Vector3 list as the value.</returns>
        public IReadOnlyDictionary<WowMapId, Dictionary<WowOreId, List<Vector3>>> AllOreNodes()
        {
            return OreNodes;
        }

        /// <summary>
        /// Retrieves a read-only dictionary of all points of interest. 
        /// The keys in the dictionary are WowMapIds, and the values are dictionaries of PoiType and corresponding lists of Vector3 coordinates.
        /// </summary>
        /// <returns>A read-only dictionary containing all points of interest.</returns>
        public IReadOnlyDictionary<WowMapId, Dictionary<PoiType, List<Vector3>>> AllPointsOfInterest()
        {
            return PointsOfInterest;
        }

        /// <summary>
        /// Retrieves all reactions stored in the Reactions dictionary.
        /// </summary>
        /// <returns>
        /// An IReadOnlyDictionary containing all reactions. The key is an integer,
        /// representing the ID of the initial character, and the value is a Dictionary
        /// that maps the ID of the target character to their WowUnitReaction.
        /// </returns>
        public IReadOnlyDictionary<int, Dictionary<int, WowUnitReaction>> AllReactions()
        {
            return Reactions;
        }

        /// <summary>
        /// Returns a read-only dictionary containing all spell names.
        /// </summary>
        public IReadOnlyDictionary<int, string> AllSpellNames()
        {
            return SpellNames;
        }

        /// <summary>
        /// Caches the herb node information for a given map ID, display ID, and position.
        /// If the herb node dictionary does not contain the map ID, a new entry is created.
        /// If the display ID does not exist for the map ID, a new entry is created.
        /// If the position does not exist for the map ID and display ID, it is added to the list.
        /// </summary>
        public void CacheHerb(WowMapId mapId, WowHerbId displayId, Vector3 position)
        {
            if (!HerbNodes.ContainsKey(mapId))
            {
                HerbNodes.TryAdd(mapId, new Dictionary<WowHerbId, List<Vector3>>() { { displayId, new List<Vector3>() { position } } });
            }
            else if (!HerbNodes[mapId].ContainsKey(displayId))
            {
                HerbNodes[mapId].Add(displayId, new List<Vector3>() { position });
            }
            else if (!HerbNodes[mapId][displayId].Any(e => e == position))
            {
                HerbNodes[mapId][displayId].Add(position);
            }
        }

        /// <summary>
        /// Caches the ore information for a specific map, display, and position.
        /// </summary>
        /// <param name="mapId">The identifier of the map.</param>
        /// <param name="displayId">The identifier of the ore display.</param>
        /// <param name="position">The position of the ore node.</param>
        public void CacheOre(WowMapId mapId, WowOreId displayId, Vector3 position)
        {
            if (!OreNodes.ContainsKey(mapId))
            {
                OreNodes.TryAdd(mapId, new Dictionary<WowOreId, List<Vector3>>() { { displayId, new List<Vector3>() { position } } });
            }
            else if (!OreNodes[mapId].ContainsKey(displayId))
            {
                OreNodes[mapId].Add(displayId, new List<Vector3>() { position });
            }
            else if (!OreNodes[mapId][displayId].Any(e => e == position))
            {
                OreNodes[mapId][displayId].Add(position);
            }
        }

        /// <summary>
        /// Caches a point of interest (POI) identified by the map ID, POI type, and position.
        /// If the map ID is not already present in the cache, a new entry is added with the POI type and position.
        /// If the map ID is present but the POI type is not, a new entry is added with the POI type and position.
        /// If the map ID and POI type are present but the position is not, the position is added to the existing entry.
        /// </summary>
        public void CachePoi(WowMapId mapId, PoiType poiType, Vector3 position)
        {
            if (!PointsOfInterest.ContainsKey(mapId))
            {
                PointsOfInterest.TryAdd(mapId, new Dictionary<PoiType, List<Vector3>>() { { poiType, new List<Vector3>() { position } } });
            }
            else if (!PointsOfInterest[mapId].ContainsKey(poiType))
            {
                PointsOfInterest[mapId].Add(poiType, new List<Vector3>() { position });
            }
            else if (!PointsOfInterest[mapId][poiType].Any(e => e == position))
            {
                PointsOfInterest[mapId][poiType].Add(position);
            }
        }

        /// <summary>
        /// Caches the reaction between two integers and a WowUnitReaction.
        /// If the first integer is not found in the cache, it adds a new entry with the first integer as the key
        /// and a dictionary containing the second integer as the key and the WowUnitReaction as the value.
        /// If the first integer is found but the second integer is not found in the nested dictionary, it adds a new entry with the second integer as the key and the WowUnitReaction as the value.
        /// If both integers are found, it updates the WowUnitReaction value in the nested dictionary.
        /// </summary>
        public void CacheReaction(int a, int b, WowUnitReaction reaction)
        {
            if (!Reactions.ContainsKey(a))
            {
                Reactions.TryAdd(a, new Dictionary<int, WowUnitReaction>() { { b, reaction } });
            }
            else if (!Reactions[a].ContainsKey(b))
            {
                Reactions[a].Add(b, reaction);
            }
            else
            {
                Reactions[a][b] = reaction;
            }
        }

        /// <summary>
        /// Clears all the lists and collections in the current object.
        /// </summary>
        public void Clear()
        {
            Names = new();
            Reactions = new();
            SpellNames = new();
            BlacklistNodes = new();
            PointsOfInterest = new();
            OreNodes = new();
            HerbNodes = new();
        }

        /// <summary>
        /// Retrieves the reaction between two World of Warcraft units.
        /// If the reaction between the two units is already cached, the cached value is returned.
        /// If not, the reaction is obtained by calling the Wow.GetReaction method and then cached for future use.
        /// </summary>
        /// <param name="a">The first World of Warcraft unit.</param>
        /// <param name="b">The second World of Warcraft unit.</param>
        /// <returns>The reaction between the two World of Warcraft units.</returns>
        public WowUnitReaction GetReaction(IWowUnit a, IWowUnit b)
        {
            if (Reactions.ContainsKey(a.FactionTemplate) && Reactions[a.FactionTemplate].ContainsKey(b.FactionTemplate))
            {
                return Reactions[a.FactionTemplate][b.FactionTemplate];
            }
            else
            {
                WowUnitReaction reaction = Wow.GetReaction(a.BaseAddress, b.BaseAddress);
                CacheReaction(a.FactionTemplate, b.FactionTemplate, reaction);
                return reaction;
            }
        }

        /// <summary>
        /// Gets the name of a spell based on its spell ID.
        /// </summary>
        /// <param name="spellId">The spell ID of the desired spell.</param>
        /// <returns>The name of the spell associated with the specified spell ID. Returns an empty string if the spell ID is less than or equal to 0.</returns>
        public string GetSpellName(int spellId)
        {
            if (spellId <= 0) { return string.Empty; }

            if (SpellNames.ContainsKey(spellId))
            {
                return SpellNames[spellId];
            }
            else
            {
                string name = Wow.GetSpellNameById(spellId);

                if (!string.IsNullOrWhiteSpace(name))
                {
                    SpellNames.TryAdd(spellId, name);
                    return name;
                }

                SpellNames.TryAdd(spellId, "unk");
                return "unk";
            }
        }

        /// <summary>
        /// Gets the name of the specified unit.
        /// </summary>
        /// <param name="unit">The unit to get the name for.</param>
        /// <param name="name">The name of the unit.</param>
        /// <returns>Returns true if the name was successfully retrieved. Otherwise, returns false.</returns>
        public bool GetUnitName(IWowUnit unit, out string name)
        {
            if (Names.ContainsKey(unit.Guid))
            {
                name = Names[unit.Guid];
                return true;
            }
            else
            {
                name = unit.ReadName();

                if (!string.IsNullOrWhiteSpace(name))
                {
                    Names.TryAdd(unit.Guid, name);
                    return true;
                }

                name = "unk";
                return false;
            }
        }

        /// <summary>
        /// Saves the current object as an XML file to the specified file path.
        /// If the directory doesn't exist, it creates the directory.
        /// If an exception occurs while saving, it deletes the file.
        /// </summary>
        /// <param name="dbFile">The file path to save the object.</param>
        public void Save(string dbFile)
        {
            try
            {
                IOUtils.CreateDirectoryIfNotExists(Path.GetDirectoryName(dbFile));
                File.WriteAllText(dbFile, JsonSerializer.Serialize(this));
            }
            catch
            {
                File.Delete(dbFile);
            }
        }

        /// <summary>
        /// Tries to get the blacklist position for a given map ID, position, and maximum radius.
        /// </summary>
        /// <param name="mapId">The ID of the map.</param>
        /// <param name="position">The position to check.</param>
        /// <param name="maxRadius">The maximum radius to consider.</param>
        /// <param name="nodes">The collection of nodes within the blacklist position.</param>
        /// <returns>True if the blacklist position exists and there are nodes within the specified radius, otherwise false.</returns>
        public bool TryGetBlacklistPosition(int mapId, Vector3 position, float maxRadius, out IEnumerable<Vector3> nodes)
        {
            if (BlacklistNodes.ContainsKey(mapId))
            {
                nodes = BlacklistNodes[mapId].Where(e => e.GetDistance(position) < maxRadius);
                return nodes.Any();
            }

            nodes = null;
            return false;
        }

        /// <summary>
        /// Tries to get the points of interest for the specified map ID, point of interest type, position, and maximum radius.
        /// </summary>
        /// <param name="mapId">The ID of the map.</param>
        /// <param name="poiType">The type of point of interest.</param>
        /// <param name="position">The position to check for proximity to points of interest.</param>
        /// <param name="maxRadius">The maximum radius for points of interest to be considered as within range.</param>
        /// <param name="nodes">The collection of nodes representing the points of interest within the specified parameters, if found.</param>
        /// <returns>True if points of interest are found within the specified parameters, otherwise false.</returns>
        public bool TryGetPointsOfInterest(WowMapId mapId, PoiType poiType, Vector3 position, float maxRadius, out IEnumerable<Vector3> nodes)
        {
            KeyValuePair<WowMapId, PoiType> KeyValuePair = new(mapId, poiType);

            if (PointsOfInterest.ContainsKey(mapId)
                && PointsOfInterest[mapId].ContainsKey(poiType))
            {
                nodes = PointsOfInterest[mapId][poiType].Where(e => e.GetDistance(position) < maxRadius);
                return nodes.Any();
            }

            nodes = null;
            return false;
        }
    }
}