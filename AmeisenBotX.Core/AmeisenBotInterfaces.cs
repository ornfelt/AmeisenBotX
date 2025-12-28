using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Storage;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.AI;
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
using AmeisenBotX.Core.Managers.Party;
using AmeisenBotX.Core.Managers.Threat;
using AmeisenBotX.RconClient;
using AmeisenBotX.Wow;
using AmeisenBotX.Wow.Cache;
using AmeisenBotX.Wow.Combatlog;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AmeisenBotX.Core
{
    /// <summary>
    /// Central access point for all bot subsystems.
    /// Provides convenience properties and helper methods used throughout the bot logic.
    /// </summary>
    public class AmeisenBotInterfaces
    {
        #region Engine References

        public IBattlegroundEngine Battleground { get; set; }
        public IDungeonEngine Dungeon { get; set; }
        public IGrindingEngine Grinding { get; set; }
        public IJobEngine Jobs { get; set; }
        public IMovementEngine Movement { get; set; }
        public IPvpEngine Pvp { get; set; }
        public IQuestEngine Quest { get; set; }
        public ITacticEngine Tactic { get; set; }
        public ITestEngine Test { get; set; }

        #endregion

        #region Combat

        public ICombatClass CombatClass { get; set; }
        public ICombatAi CombatAi { get; set; }
        public ICombatlogParser CombatLog { get; set; }
        public ThreatManager Threat { get; set; }

        #endregion

        #region Managers

        public ICharacterManager Character { get; set; }
        public IChatManager Chat { get; set; }
        public IPartyManager Party { get; set; }
        public IdleActionManager IdleActions { get; set; }
        public IPathfindingHandler PathfindingHandler { get; set; }

        #endregion

        #region Core Accessors

        /// <summary>WoW interface for memory reading, Lua execution, DBC access.</summary>
        public IWowInterface Wow { get; set; }

        /// <summary>Memory API for low-level process manipulation.</summary>
        public WowMemoryApi Memory => Wow.Memory;

        /// <summary>Object manager for game objects.</summary>
        public IObjectProvider Objects => Wow.ObjectProvider;

        /// <summary>Database cache for reactions, spell names, etc.</summary>
        public IAmeisenBotDb Db { get; set; }

        /// <summary>Persistent storage manager.</summary>
        public StorageManager Storage { get; set; }

        /// <summary>Remote console client.</summary>
        public AmeisenBotRconClient Rcon { get; set; }

        /// <summary>Bot configuration.</summary>
        public AmeisenBotConfig Config { get; set; }

        /// <summary>Reference to the behavior tree for debugging.</summary>
        public BehaviorTree.Tree BehaviorTree { get; set; }

        #endregion

        #region Quick Accessors

        /// <summary>Current player character.</summary>
        public IWowPlayer Player => Objects.Player;

        /// <summary>Current target.</summary>
        public IWowUnit Target => Objects.Target;

        /// <summary>Last target.</summary>
        public IWowUnit LastTarget => Objects.LastTarget;

        /// <summary>Player's pet.</summary>
        public IWowUnit Pet => Objects.Pet;

        #endregion

        #region State Flags

        /// <summary>
        /// Indicates if the bot is currently looting.
        /// Movement providers should respect this state.
        /// </summary>
        public bool IsLooting { get; set; }

        #endregion

        #region Query Methods - AoE

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<IWowDynobject> GetAoeSpells(Vector3 position, float extends = 2.0f)
        {
            return Objects.All.OfType<IWowDynobject>()
                .Where(e => e.Position.GetDistance(position) < e.Radius + extends);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IWowGameobject GetClosestGameObjectByDisplayId(Vector3 position, IEnumerable<int> displayIds)
        {
            return Objects.All.OfType<IWowGameobject>()
                .Where(e => displayIds.Contains(e.DisplayId))
                .OrderBy(e => e.Position.GetDistance(position))
                .FirstOrDefault();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IWowUnit GetClosestQuestGiverByDisplayId(Vector3 position, IEnumerable<int> displayIds, bool onlyQuestGivers = true)
        {
            return Objects.All.OfType<IWowUnit>()
                .Where(e => !e.IsDead && (!onlyQuestGivers || e.IsQuestgiver) && displayIds.Contains(e.DisplayId))
                .OrderBy(e => e.Position.GetDistance(position))
                .FirstOrDefault();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IWowUnit GetClosestQuestGiverByNpcId(Vector3 position, IEnumerable<int> npcIds, bool onlyQuestGivers = true)
        {
            return Objects.All.OfType<IWowUnit>()
                .Where(e => !e.IsDead && (!onlyQuestGivers || e.IsQuestgiver) && npcIds.Contains(BotUtils.GuidToNpcId(e.Guid)))
                .OrderBy(e => e.Position.GetDistance(position))
                .FirstOrDefault();
        }

        #endregion

        #region Query Methods - NPC Finding

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IWowUnit GetClosestTrainerByEntryId(int entryId)
        {
            return Player == null
                ? null
                : Objects.All.OfType<IWowUnit>()
                .Where(e => !e.IsDead && e.IsTrainer && Db.GetReaction(Player, e) != WowUnitReaction.Hostile && e.EntryId == entryId)
                .OrderBy(e => e.Position.GetDistance(Player.Position))
                .FirstOrDefault();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IWowUnit GetClosestVendorByEntryId(int entryId)
        {
            return Player == null
                ? null
                : Objects.All.OfType<IWowUnit>()
                .Where(e => !e.IsDead && e.IsVendor && Db.GetReaction(Player, e) != WowUnitReaction.Hostile && e.EntryId == entryId)
                .OrderBy(e => e.Position.GetDistance(Player.Position))
                .FirstOrDefault();
        }

        #endregion

        #region Query Methods - Combat

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> GetEnemiesInCombatWithMe<T>(Vector3 position, float distance) where T : IWowUnit
        {
            if (Player == null)
            {
                return [];
            }

            return GetNearEnemies<T>(position, distance)  // is hostile
                .Where(e => e.IsInCombat                  // needs to be in combat
                         && e.TargetGuid == Player.Guid); // targets us
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> GetEnemiesInCombatWithParty<T>(Vector3 position, float distance) where T : IWowUnit
        {
            if (Player == null)
            {
                return [];
            }

            return GetNearEnemies<T>(position, distance)                                // is hostile
                .Where(e => e.IsInCombat && (e.IsTaggedByMe || !e.IsTaggedByOther)      // needs to be in combat and tagged by us or no one else
                         && (e.TargetGuid == Player.Guid                                // targets us
                            || Objects.Partymembers.Any(x => x.Guid == e.TargetGuid))); // targets a party member
        }

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

            return [];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> GetEnemiesOrNeutralsInCombatWithMe<T>(Vector3 position, float distance) where T : IWowUnit
        {
            if (Player == null)
            {
                return [];
            }

            return GetNearEnemiesOrNeutrals<T>(position, distance) // is hostile/neutral
                .Where(e => e.IsInCombat                           // needs to be in combat
                         && e.TargetGuid == Player.Guid);          // targets us
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> GetEnemiesOrNeutralsInCombatWithParty<T>(Vector3 position, float distance) where T : IWowUnit
        {
            if (Player == null)
            {
                return [];
            }

            return GetNearEnemiesOrNeutrals<T>(position, distance)                      // is hostile
                .Where(e => e.IsInCombat && (e.IsTaggedByMe || !e.IsTaggedByOther)      // needs to be in combat and tagged by us or no one else
                         && (e.TargetGuid == Player.Guid                                // targets us
                            || Objects.Partymembers.Any(x => x.Guid == e.TargetGuid))); // targets a party member
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> GetEnemiesOrNeutralsTargetingMe<T>(Vector3 position, float distance) where T : IWowUnit
        {
            if (Player == null)
            {
                return [];
            }

            return GetNearEnemiesOrNeutrals<T>(position, distance)  // is hostile/neutral
                .Where(e => e.TargetGuid == Player.Guid); // targets us
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> GetEnemiesTargetingMe<T>(Vector3 position, float distance) where T : IWowUnit
        {
            if (Player == null)
            {
                return [];
            }

            return GetNearEnemies<T>(position, distance)  // is hostile
                .Where(e => e.TargetGuid == Player.Guid); // targets us
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> GetEnemiesTargetingPartyMembers<T>(Vector3 position, float distance) where T : IWowUnit
        {
            return GetNearEnemies<T>(position, distance)                           // is hostile
                .Where(e => e.IsInCombat                                           // is in combat
                         && (Objects.Partymembers.Any(x => x.Guid == e.TargetGuid) // is targeting a partymember
                         || Objects.PartyPets.Any(x => x.Guid == e.TargetGuid)));  // is targeting a pet in party
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsHostileReaction(IWowUnit unit)
        {
            if (Player == null)
            {
                return false;
            }

            WowUnitReaction reaction = Db.GetReaction(Player, unit);
            return reaction is WowUnitReaction.Hostile or WowUnitReaction.Hated or WowUnitReaction.Unfriendly or WowUnitReaction.Neutral;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> GetNearEnemies<T>(Vector3 position, float distance) where T : IWowUnit
        {
            return Objects.All.OfType<T>()
                .Where(e => !e.IsDead && !e.IsNotAttackable    // is alive and attackable
                         && IsHostileReaction(e)               // is hostile
                         && e.Position.GetDistance(position) < distance);  // is in range
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> GetNearEnemiesOrNeutrals<T>(Vector3 position, float distance) where T : IWowUnit
        {
            if (Player == null)
            {
                return [];
            }

            return Objects.All.OfType<T>()
                .Where(e => !e.IsDead && !e.IsNotAttackable                       // is alive and attackable
                         && Db.GetReaction(Player, e) != WowUnitReaction.Friendly // is hostile/neutral
                         && e.Position.GetDistance(position) < distance);         // is in range
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> GetNearFriends<T>(Vector3 position, float distance) where T : IWowUnit
        {
            if (Player == null)
            {
                return [];
            }

            return Objects.All.OfType<T>()
                .Where(e => !e.IsDead && !e.IsNotAttackable                       // is alive and attackable
                         && Db.GetReaction(Player, e) == WowUnitReaction.Friendly // is friendly
                         && e.Position.GetDistance(position) < distance);         // is in range
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> GetNearPartyMembers<T>(Vector3 position, float distance) where T : IWowUnit
        {
            return Objects.Partymembers.OfType<T>()
                .Where(e => !e.IsDead && !e.IsNotAttackable               // is alive and attackable
                         && e.Position.GetDistance(position) < distance); // is in range
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetWowObjectByGuid<T>(ulong guid) where T : IWowObject
        {
            return Objects.All.OfType<T>().FirstOrDefault(e => e.Guid == guid);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetWowObjectByGuid<T>(ulong guid, out T obj) where T : IWowObject
        {
            obj = guid == 0 ? default : GetWowObjectByGuid<T>(guid);
            return obj != null;
        }

        #endregion

        #region Query Methods - Icons

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bitmap GetIconBySpellname(string spellname)
        {
            return Wow.Mpq.GetIcon(Wow.Dbc.GetSpellIconPath(Wow.Dbc.GetSpellIdByName(spellname)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bitmap GetIconBySpellId(int spellId)
        {
            return Wow.Mpq.GetIcon(Wow.Dbc.GetSpellIconPath(spellId));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Bitmap GetIconByItemId(int itemId)
        {
            return Wow.Mpq.GetIcon(Wow.Dbc.GetItemIconPath(itemId));
        }

        #endregion
    }
}
