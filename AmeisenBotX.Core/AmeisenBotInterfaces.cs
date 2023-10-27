using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Storage;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Battleground;
using AmeisenBotX.Core.Engines.Combat.Classes;
using AmeisenBotX.Core.Engines.Dungeon;
using AmeisenBotX.Core.Engines.Grinding;
using AmeisenBotX.Core.Engines.Jobs;
using AmeisenBotX.Core.Engines.Movement;
using AmeisenBotX.Core.Engines.Movement.Pathfinding;
using AmeisenBotX.Core.Engines.PvP;
using AmeisenBotX.Core.Engines.Quest;
using AmeisenBotX.Core.Engines.Tactic;
using AmeisenBotX.Core.Engines.Test;
using AmeisenBotX.Core.Logic.Idle;
using AmeisenBotX.Core.Managers.Character;
using AmeisenBotX.Core.Managers.Chat;
using AmeisenBotX.Core.Managers.Threat;
using AmeisenBotX.RconClient;
using AmeisenBotX.Wow;
using AmeisenBotX.Wow.Cache;
using AmeisenBotX.Wow.Combatlog;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AmeisenBotX.Core
{
    /// <summary>
    /// Gets or sets the combat class for the character.
    /// </summary>
    public class AmeisenBotInterfaces
    {
        /// <summary>
        /// Gets or sets the interface for the Battleground Engine.
        /// </summary>
        public IBattlegroundEngine Battleground { get; set; }

        /// <summary>
        /// Gets or sets the Character of type ICharacterManager.
        /// </summary>
        public ICharacterManager Character { get; set; }

        /// <summary>
        /// Gets or sets the chat manager.
        /// </summary>
        public IChatManager Chat { get; set; }

        /// <summary>
        /// Gets or sets the combat class for the character.
        /// </summary>
        public ICombatClass CombatClass { get; set; }

        /// <summary>
        /// Gets or sets the combat log parser.
        /// </summary>
        public ICombatlogParser CombatLog { get; set; }

        /// <summary>
        /// Gets or sets the instance of the IAmeisenBotDb interface used by the AmeisenBot.
        /// </summary>
        public IAmeisenBotDb Db { get; set; }

        /// <summary>
        /// Public property representing the dungeon engine.
        /// </summary>
        public IDungeonEngine Dungeon { get; set; }

        /// <summary>
        /// Gets or sets the grinding engine used for grinding.
        /// </summary>
        public IGrindingEngine Grinding { get; set; }

        /// <summary>
        /// Gets or sets the IdleActionManager property.
        /// </summary>
        public IdleActionManager IdleActions { get; set; }

        /// <summary>
        /// Gets or sets the job engine used in the application.
        /// </summary>
        public IJobEngine Jobs { get; set; }

        /// <summary>
        /// Gets or sets the last target of the World of Warcraft unit.
        /// </summary>
        public IWowUnit LastTarget => Objects.LastTarget;

        /// <summary>
        /// Gets the WowMemoryApi instance associated with the Wow object.
        /// </summary>
        public WowMemoryApi Memory => Wow.Memory;

        /// <summary>
        /// Gets or sets the movement engine for the object.
        /// </summary>
        public IMovementEngine Movement { get; set; }

        /// <summary>Gets the object provider.</summary>
        public IObjectProvider Objects => Wow.ObjectProvider;

        /// <summary>
        /// Gets or sets the pathfinding handler.
        /// </summary>
        public IPathfindingHandler PathfindingHandler { get; set; }

        /// <summary>
        /// Gets the WoW unit representing the pet object.
        /// </summary>
        public IWowUnit Pet => Objects.Pet;

        /// <summary>
        /// Gets or sets the instance of the IWowPlayer interface representing the player object.
        /// </summary>
        public IWowPlayer Player => Objects.Player;

        /// <summary>
        /// Gets or sets the PvP engine for the game.
        /// </summary>
        public IPvpEngine Pvp { get; set; }

        /// <summary>
        /// Gets or sets the IQuestEngine for the public Quest property.
        /// </summary>
        public IQuestEngine Quest { get; set; }

        /// <summary>
        /// Gets or sets the Ameisen bot RCON client.
        /// </summary>
        public AmeisenBotRconClient Rcon { get; set; }

        /// <summary>
        /// Gets or sets the storage manager.
        /// </summary>
        public StorageManager Storage { get; set; }

        /// <summary>
        /// Gets or sets the Tactic engine for the current object.
        /// </summary>
        public ITacticEngine Tactic { get; set; }

        /// <summary>
        /// Gets the target of the IWowUnit.
        /// </summary>
        public IWowUnit Target => Objects.Target;

        /// <summary>
        /// Gets or sets the test engine.
        /// </summary>
        public ITestEngine Test { get; set; }

        /// Gets or sets the ThreatManager object used to manage threats.
        public ThreatManager Threat { get; set; }

        /// <summary>
        /// Gets or sets the instance of the IWowInterface interface.
        /// </summary>
        public IWowInterface Wow { get; set; }

        /// <summary>
        /// Retrieves all area-of-effect (AoE) spells within a specified distance from a given position.
        /// </summary>
        /// <param name="position">The position from which to measure the distance.</param>
        /// <param name="extends">The additional distance to expand the search radius.</param>
        /// <returns>An enumerable collection of WowDynobject instances representing the AoE spells.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<IWowDynobject> GetAoeSpells(Vector3 position, float extends = 2.0f)
        {
            return Objects.All.OfType<IWowDynobject>()
                .Where(e => e.Position.GetDistance(position) < e.Radius + extends);
        }

        /// <summary>
        /// Returns the closest game object by display ID based on the specified position and display IDs.
        /// </summary>
        /// <param name="position">The position to calculate the distance from.</param>
        /// <param name="displayIds">The collection of display IDs to filter the game objects.</param>
        /// <returns>The closest game object that matches the display IDs, or null if no matching game object is found.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IWowGameobject GetClosestGameObjectByDisplayId(Vector3 position, IEnumerable<int> displayIds)
        {
            return Objects.All.OfType<IWowGameobject>()
                .Where(e => displayIds.Contains(e.DisplayId))
                .OrderBy(e => e.Position.GetDistance(position))
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns the closest quest giver by display id based on the given position and display ids.
        /// </summary>
        /// <param name="position">The position to use as reference.</param>
        /// <param name="displayIds">The list of display ids to search for.</param>
        /// <param name="onlyQuestGivers">Indicates whether to only consider quest givers.</param>
        /// <returns>The closest quest giver matching the criteria.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IWowUnit GetClosestQuestGiverByDisplayId(Vector3 position, IEnumerable<int> displayIds, bool onlyQuestGivers = true)
        {
            return Objects.All.OfType<IWowUnit>()
                .Where(e => !e.IsDead && (!onlyQuestGivers || e.IsQuestgiver) && displayIds.Contains(e.DisplayId))
                .OrderBy(e => e.Position.GetDistance(position))
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns the closest quest giver by NPC ID based on the specified position, list of NPC IDs, and a flag to determine whether to include only quest givers.
        /// </summary>
        /// <param name="position">The position to calculate the distance from.</param>
        /// <param name="npcIds">The list of NPC IDs to search for.</param>
        /// <param name="onlyQuestGivers">Determines whether to include only quest givers in the search.</param>
        /// <returns>The closest quest giver as an IWowUnit object.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IWowUnit GetClosestQuestGiverByNpcId(Vector3 position, IEnumerable<int> npcIds, bool onlyQuestGivers = true)
        {
            return Objects.All.OfType<IWowUnit>()
                .Where(e => !e.IsDead && (!onlyQuestGivers || e.IsQuestgiver) && npcIds.Contains(BotUtils.GuidToNpcId(e.Guid)))
                .OrderBy(e => e.Position.GetDistance(position))
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns the closest trainer by the specified entry ID.
        /// </summary>
        /// <param name="entryId">The entry ID of the trainer.</param>
        /// <returns>The closest trainer that matches the specified entry ID.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IWowUnit GetClosestTrainerByEntryId(int entryId)
        {
            return Objects.All.OfType<IWowUnit>()
                .Where(e => !e.IsDead && e.IsTrainer && Db.GetReaction(Player, e) != WowUnitReaction.Hostile && e.EntryId == entryId)
                .OrderBy(e => e.Position.GetDistance(Player.Position))
                .FirstOrDefault();
        }

        /// <summary>
        /// Returns the closest vendor by entry ID.
        /// </summary>
        /// <param name="entryId">The entry ID of the vendor.</param>
        /// <returns>The closest vendor as an IWowUnit object, or null if not found.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IWowUnit GetClosestVendorByEntryId(int entryId)
        {
            return Objects.All.OfType<IWowUnit>()
                .Where(e => !e.IsDead && e.IsVendor && Db.GetReaction(Player, e) != WowUnitReaction.Hostile && e.EntryId == entryId)
                .OrderBy(e => e.Position.GetDistance(Player.Position))
                .FirstOrDefault();
        }

        /// <summary>
        /// Retrieves a collection of enemies that are in combat with the player and within a specified distance from a given position.
        /// </summary>
        /// <typeparam name="T">The type of enemy units to retrieve.</typeparam>
        /// <param name="position">The position from which to search for enemies.</param>
        /// <param name="distance">The maximum distance within which to consider enemies.</param>
        /// <returns>A collection of enemy units of type T that are in combat with the player and within the specified distance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> GetEnemiesInCombatWithMe<T>(Vector3 position, float distance) where T : IWowUnit
        {
            return GetNearEnemies<T>(position, distance)  // is hostile
                .Where(e => e.IsInCombat                  // needs to be in combat
                         && e.TargetGuid == Player.Guid); // targets us
        }

        /// <summary>
        /// Returns a collection of enemies in combat with the party within a specified distance from a given position.
        /// </summary>
        /// <typeparam name="T">The type of enemy to retrieve.</typeparam>
        /// <param name="position">The position from which to search for enemies.</param>
        /// <param name="distance">The maximum distance from the position to consider enemies.</param>
        /// <returns>A collection of enemies that meet the specified criteria.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> GetEnemiesInCombatWithParty<T>(Vector3 position, float distance) where T : IWowUnit
        {
            return GetNearEnemies<T>(position, distance)                                // is hostile
                .Where(e => e.IsInCombat && (e.IsTaggedByMe || !e.IsTaggedByOther)      // needs to be in combat and tagged by us or no one else
                         && (e.TargetGuid == Player.Guid                                // targets us
                            || Objects.Partymembers.Any(x => x.Guid == e.TargetGuid))); // targets a party member
        }

        /// <summary>
        /// Returns a sequence of enemies within a specified distance along a given path.
        /// </summary>
        /// <typeparam name="T">The type of enemies to retrieve.</typeparam>
        /// <param name="path">The path to search for enemies along.</param>
        /// <param name="distance">The maximum distance for an enemy to be considered within range.</param>
        /// <returns>A sequence of enemies within range along the path, or an empty sequence if no enemies are found.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> GetEnemiesInPath<T>(IEnumerable<Vector3> path, float distance) where T : IWowUnit
        {
            foreach (Vector3 pathPosition in path)
            {
                IEnumerable<T> nearEnemies = GetNearEnemies<T>(pathPosition, distance);

                if (nearEnemies.Any())
                {
                    return nearEnemies;
                }
            }

            return Array.Empty<T>();
        }

        /// <summary>
        /// Retrieves the enemies or neutrals within a specified distance from a given position that are in combat and targeting the player.
        /// </summary>
        /// <typeparam name="T">The type of units to retrieve.</typeparam>
        /// <param name="position">The position to check for enemies or neutrals.</param>
        /// <param name="distance">The maximum distance to consider for unit retrieval.</param>
        /// <returns>An IEnumerable of units of type T that are enemies or neutrals in combat with the player.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> GetEnemiesOrNeutralsInCombatWithMe<T>(Vector3 position, float distance) where T : IWowUnit
        {
            return GetNearEnemiesOrNeutrals<T>(position, distance) // is hostile/neutral
                .Where(e => e.IsInCombat                           // needs to be in combat
                         && e.TargetGuid == Player.Guid);          // targets us
        }

        /// <summary>
        /// Retrieves a collection of enemies or neutrals within a specified distance of the given position that are targeting the player.
        /// </summary>
        /// <typeparam name="T">The type of units to retrieve, must implement the IWowUnit interface.</typeparam>
        /// <param name="position">The position from which to search for enemies or neutrals.</param>
        /// <param name="distance">The maximum distance within which to search for enemies or neutrals.</param>
        /// <returns>A collection of enemies or neutrals that are targeting the player.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> GetEnemiesOrNeutralsTargetingMe<T>(Vector3 position, float distance) where T : IWowUnit
        {
            return GetNearEnemiesOrNeutrals<T>(position, distance)  // is hostile/neutral
                .Where(e => e.TargetGuid == Player.Guid); // targets us
        }

        /// <summary>
        /// Returns a collection of enemies targeting the player within a specified distance from a given position.
        /// </summary>
        /// <typeparam name="T">The type of enemy unit to retrieve.</typeparam>
        /// <param name="position">The position from which to search for enemies.</param>
        /// <param name="distance">The maximum distance within which enemies should be considered.</param>
        /// <returns>A filtered collection of enemies targeting the player.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> GetEnemiesTargetingMe<T>(Vector3 position, float distance) where T : IWowUnit
        {
            return GetNearEnemies<T>(position, distance)  // is hostile
                .Where(e => e.TargetGuid == Player.Guid); // targets us
        }

        /// <summary>
        /// Returns a collection of enemies that are targeting party members or pets within a specified distance from the given position.
        /// </summary>
        /// <typeparam name="T">The type of enemy unit to retrieve.</typeparam>
        /// <param name="position">The position to search for enemies.</param>
        /// <param name="distance">The maximum distance from the position to consider enemies.</param>
        /// <returns>A collection of enemies targeting party members or pets.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> GetEnemiesTargetingPartyMembers<T>(Vector3 position, float distance) where T : IWowUnit
        {
            return GetNearEnemies<T>(position, distance)                           // is hostile
                .Where(e => e.IsInCombat                                           // is in combat
                         && (Objects.Partymembers.Any(x => x.Guid == e.TargetGuid) // is targeting a partymember
                         || Objects.PartyPets.Any(x => x.Guid == e.TargetGuid)));  // is targeting a pet in party
        }

        /// <summary>
        /// Retrieves all alive and attackable enemy units of type T within a specified distance from the given position.
        /// </summary>
        /// <typeparam name="T">The type of enemy unit.</typeparam>
        /// <param name="position">The position from which to calculate the distance.</param>
        /// <param name="distance">The maximum distance from the position to include enemy units.</param>
        /// <returns>An IEnumerable collection of enemy units of type T that meet the specified criteria.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> GetNearEnemies<T>(Vector3 position, float distance) where T : IWowUnit
        {
            return Objects.All.OfType<T>()
                .Where(e => !e.IsDead && !e.IsNotAttackable                      // is alive and attackable
                         && Db.GetReaction(Player, e) == WowUnitReaction.Hostile // is hostile
                         && e.Position.GetDistance(position) < distance);        // is in range
        }

        /// <summary>
        /// Retrieves a collection of near enemies or neutrals of type T, within a specified distance from a given position.
        /// </summary>
        /// <typeparam name="T">The type of objects to retrieve.</typeparam>
        /// <param name="position">The position to check for near enemies or neutrals.</param>
        /// <param name="distance">The maximum distance within which to consider objects as near.</param>
        /// <returns>A collection of near enemies or neutrals of type T.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> GetNearEnemiesOrNeutrals<T>(Vector3 position, float distance) where T : IWowUnit
        {
            return Objects.All.OfType<T>()
                .Where(e => !e.IsDead && !e.IsNotAttackable                       // is alive and attackable
                         && Db.GetReaction(Player, e) != WowUnitReaction.Friendly // is hostile/neutral
                         && e.Position.GetDistance(position) < distance);         // is in range
        }

        /// <summary>
        /// Returns a collection of near friends of type T within a specified distance from the given position.
        /// </summary>
        /// <typeparam name="T">The type of friends to be returned.</typeparam>
        /// <param name="position">The position to search near friends from.</param>
        /// <param name="distance">The maximum distance for a friend to be considered near.</param>
        /// <returns>A collection of near friends of type T within the specified distance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> GetNearFriends<T>(Vector3 position, float distance) where T : IWowUnit
        {
            return Objects.All.OfType<T>()
                .Where(e => !e.IsDead && !e.IsNotAttackable                       // is alive and attackable
                         && Db.GetReaction(Player, e) == WowUnitReaction.Friendly // is hostile
                         && e.Position.GetDistance(position) < distance);         // is in range
        }

        /// <summary>
        /// Retrieves a collection of party members near a specified position within a given distance.
        /// </summary>
        /// <typeparam name="T">The type of party member to retrieve.</typeparam>
        /// <param name="position">The position to check for party members.</param>
        /// <param name="distance">The maximum distance within which party members should be considered.</param>
        /// <returns>A collection of party members that meet the specified criteria.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> GetNearPartyMembers<T>(Vector3 position, float distance) where T : IWowUnit
        {
            return Objects.Partymembers.OfType<T>()
                .Where(e => !e.IsDead && !e.IsNotAttackable               // is alive and attackable
                         && e.Position.GetDistance(position) < distance); // is in range
        }

        /// <summary>
        /// Retrieves an object of type T from all objects and their descendants, matching the provided guid.
        /// </summary>
        /// <typeparam name="T">The type of object to retrieve.</typeparam>
        /// <param name="guid">The guid to search for.</param>
        /// <returns>The first object of type T, or default if no match is found.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetWowObjectByGuid<T>(ulong guid) where T : IWowObject
        {
            return Objects.All.OfType<T>().FirstOrDefault(e => e.Guid == guid);
        }

        /// <summary>
        /// Tries to get a WoW object by its GUID.
        /// </summary>
        /// <typeparam name="T">The type of the WoW object.</typeparam>
        /// <param name="guid">The GUID of the WoW object.</param>
        /// <param name="obj">The retrieved WoW object.</param>
        /// <returns>True if the WoW object was successfully retrieved, otherwise false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetWowObjectByGuid<T>(ulong guid, out T obj) where T : IWowObject
        {
            obj = guid == 0 ? default : GetWowObjectByGuid<T>(guid);
            return obj != null;
        }
    }
}