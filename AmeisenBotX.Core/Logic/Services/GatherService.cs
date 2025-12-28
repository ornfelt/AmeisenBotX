using AmeisenBotX.BehaviorTree.Enums;
using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Core.Logic.Harvest;
using AmeisenBotX.Core.Logic.Routines;
using AmeisenBotX.Logging;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Logic.Services
{
    /// <summary>
    /// Service for gathering logic.
    /// Manages full harvest lifecycle via state machine.
    /// </summary>
    public class GatherService(AmeisenBotInterfaces bot, AmeisenBotConfig config)
    {
        private readonly AmeisenBotInterfaces Bot = bot;
        private readonly AmeisenBotConfig Config = config;
        private readonly HarvestManager HarvestManager = new(bot);

        // State Machine
        private enum GatherState
        {
            Scanning,
            Approaching,
            Interacting,
            WaitingForCast,
            WaitingForLoot
        }

        private GatherState CurrentState { get; set; } = GatherState.Scanning;

        // Current target
        public IWowGameobject CollectGobject { get; private set; }

        // Blacklists
        public Dictionary<ulong, DateTime> GobjectBlacklist { get; } = [];
        public Dictionary<ulong, DateTime> PathBlacklist { get; } = [];

        // Timeout tracking
        private ulong _currentGobjectGuid;
        private DateTime _gobjectStartTime;
        public int GobjectCollectTries { get; private set; }

        // Throttled scan
        private readonly TimegatedEvent GobjectCheckEvent = new(TimingConfig.GobjectCheck);

        // UI Visibility Throttling (reduce Lua spam)
        private readonly TimegatedEvent _lootCheckEvent = new(TimeSpan.FromMilliseconds(250));
        private bool _cachedLootFrameVisible;

        /// <summary>
        /// Main Execution Method for BT.
        /// Handles the full lifecycle: Scan -> Approach -> Interact -> Wait.
        /// </summary>
        public BtStatus Execute()
        {
            // Safety: Full bags?
            if (Bot.Character.Inventory.FreeBagSlots < 1)
            {
                if (Config.AutoDestroyTrash && TrashItemsRoutine.TryDeleteOneItem(Bot, Config))
                {
                    return BtStatus.Ongoing;
                }
                Reset();
                return BtStatus.Failed;
            }

            switch (CurrentState)
            {
                case GatherState.Scanning:
                    if (ScanForTarget())
                    {
                        CurrentState = GatherState.Approaching;
                        return BtStatus.Ongoing;
                    }
                    return BtStatus.Failed;

                case GatherState.Approaching:
                    return ExecuteApproaching();

                case GatherState.Interacting:
                    return ExecuteInteraction();

                case GatherState.WaitingForCast:
                    return ExecuteWaitingForCast();

                case GatherState.WaitingForLoot:
                    return ExecuteWaitingForLoot();

                default:
                    Reset();
                    return BtStatus.Failed;
            }
        }

        /// <summary>
        /// External check for BT Condition
        /// </summary>
        public bool HasValidTarget()
        {
            // If we have a target, we are busy. If we can find one, we will be busy.
            if (CollectGobject != null)
            {
                return true;
            }

            return ScanForTarget(); // Will set CollectGobject if found
        }

        private bool ScanForTarget()
        {
            // Throttle new searches if not already targeting
            if (CollectGobject == null && !GobjectCheckEvent.Run())
            {
                return false;
            }

            CleanupBlacklists();

            // Determine parameters
            Vector3 searchCenter = Bot.Player.Position;
            float maxSearchRadius = TimingConfig.SoloCollectRadius;

            if (Bot.Objects.Partymembers.Any())
            {
                searchCenter = Bot.Objects.CenterPartyPosition;
                maxSearchRadius = TimingConfig.GroupCollectRadius;
            }

            // Merge blacklists
            HashSet<ulong> mergedBlacklist = [.. GobjectBlacklist.Keys, .. PathBlacklist.Keys];

            IWowGameobject bestTarget = HarvestManager?.FindBestTarget(
                searchCenter,
                maxSearchRadius,
                mergedBlacklist
            );

            if (bestTarget == null)
            {
                Reset();
                return false;
            }

            // Validation checks
            if (Bot.Player.IsMounted && !bestTarget.IsSparkling)
            {
                return false;
            }

            if (AreDangerousMobsNear(bestTarget.Position) || IsAnotherPlayerCompeting(bestTarget))
            {
                GobjectBlacklist[bestTarget.Guid] = DateTime.UtcNow.AddSeconds(10); // Short blacklist
                return false;
            }

            if (PathBlacklist.ContainsKey(bestTarget.Guid))
            {
                return false;
            }

            // New target found?
            if (CollectGobject == null || CollectGobject.Guid != bestTarget.Guid)
            {
                CollectGobject = bestTarget;
                _currentGobjectGuid = bestTarget.Guid;
                _gobjectStartTime = DateTime.UtcNow;
                CurrentState = GatherState.Approaching;
            }

            return true;
        }

        private BtStatus ExecuteApproaching()
        {
            if (!ValidateTarget())
            {
                return BtStatus.Failed;
            }

            // Use larger tolerance for interact
            float interactDist = 4.5f; // Standard interact range is ~5y
            if (interactDist <= 0)
            {
                interactDist = 3.5f;
            }

            if (Bot.Player.DistanceTo(CollectGobject) > interactDist)
            {
                // Timeout check
                if (DateTime.UtcNow - _gobjectStartTime > TimingConfig.GobjectTimeout)
                {
                    BlacklistCurrent("Timeout reaching target");
                    return BtStatus.Failed;
                }

                Bot.Movement.SetMovementAction(MovementAction.Move, CollectGobject.Position);
                return BtStatus.Ongoing;
            }

            // Arrived
            Bot.Movement.SetMovementAction(MovementAction.None, Bot.Player.Position);
            CurrentState = GatherState.Interacting;
            return BtStatus.Ongoing;
        }

        private BtStatus ExecuteInteraction()
        {
            if (!ValidateTarget())
            {
                return BtStatus.Failed;
            }

            // Dismount if needed
            if (Bot.Player.IsMounted && !CollectGobject.IsSparkling)
            {
                Bot.Wow.LuaDoString("Dismount()");
                return BtStatus.Ongoing;
            }

            // Interact
            Bot.Wow.InteractWithObject(CollectGobject);

            // Transition to Waiting
            _gobjectStartTime = DateTime.UtcNow; // Reset timeout for cast/loot phase
            CurrentState = GatherState.WaitingForCast;
            return BtStatus.Ongoing;
        }

        private BtStatus ExecuteWaitingForCast()
        {
            // If casting, we are good. Wait.
            if (Bot.Player.IsCasting)
            {
                return BtStatus.Ongoing;
            }

            // If loot window open, switch to Loot phase
            if (_lootCheckEvent.Run())
            {
                _cachedLootFrameVisible = Bot.Wow.UiIsVisible("LootFrame");
            }

            if (_cachedLootFrameVisible)
            {
                CurrentState = GatherState.WaitingForLoot;
                return BtStatus.Ongoing;
            }

            // If cast finished/interrupted and no loot window yet...
            // Verify if object is still there and harvestable?
            // Or maybe we haven't started casting yet (latency).

            // Timeout for cast start
            if (DateTime.UtcNow - _gobjectStartTime > TimeSpan.FromSeconds(3))
            {
                // No cast started, no loot window after 3s interaction?
                // Retry interaction?
                GobjectCollectTries++;
                if (GobjectCollectTries < 2)
                {
                    CurrentState = GatherState.Interacting;
                    return BtStatus.Ongoing;
                }

                BlacklistCurrent("Interaction failed (No cast/loot)");
                return BtStatus.Failed;
            }

            return BtStatus.Ongoing;
        }

        private BtStatus ExecuteWaitingForLoot()
        {
            // Throttled LootFrame check
            if (_lootCheckEvent.Run())
            {
                _cachedLootFrameVisible = Bot.Wow.UiIsVisible("LootFrame");
            }

            // Loot window is visible
            if (_cachedLootFrameVisible)
            {
                Bot.Wow.LootEverything();
                return BtStatus.Ongoing;
            }

            // Loot window closed? Success!
            // Wait, did we get everything? Assuming yes for now.
            // Or maybe it auto-closed because object despawned.

            Reset(); // Done
            return BtStatus.Success;
        }

        private bool ValidateTarget()
        {
            if (CollectGobject == null)
            {
                return false;
            }

            IWowGameobject freshObj = Bot.GetWowObjectByGuid<IWowGameobject>(CollectGobject.Guid);
            if (freshObj == null)
            {
                Reset(); // Object gone (someone else took it?)
                return false;
            }
            CollectGobject = freshObj; // Update reference
            return true;
        }

        private void BlacklistCurrent(string reason)
        {
            if (CollectGobject != null)
            {
                AmeisenLogger.I.Log("GatherService", $"Blacklisting {CollectGobject.Name}: {reason}", Logging.Enums.LogLevel.Debug);
                GobjectBlacklist[CollectGobject.Guid] = DateTime.UtcNow.AddMinutes(5);
            }
            Reset();
        }

        public void Reset()
        {
            CollectGobject = null;
            _currentGobjectGuid = 0;
            GobjectCollectTries = 0;
            CurrentState = GatherState.Scanning;
        }

        // --- Helpers ---

        /// <summary>
        /// Mark a path to gobject as unreachable (called by stuck handler usually).
        /// </summary>
        public void BlacklistPath(ulong guid)
        {
            PathBlacklist[guid] = DateTime.UtcNow + TimingConfig.PathBlacklistDuration;
        }

        private void CleanupBlacklists()
        {
            DateTime now = DateTime.UtcNow;
            List<ulong> expiredG = GobjectBlacklist.Where(kv => now > kv.Value).Select(kv => kv.Key).ToList();
            foreach (ulong k in expiredG)
            {
                GobjectBlacklist.Remove(k);
            }

            List<ulong> expiredP = PathBlacklist.Where(kv => now > kv.Value).Select(kv => kv.Key).ToList();
            foreach (ulong k in expiredP)
            {
                PathBlacklist.Remove(k);
            }
        }

        private bool AreDangerousMobsNear(Vector3 position, float radius = 14.0f)
        {
            return Bot.Objects.All.OfType<IWowUnit>()
                .Any(e => !e.IsDead && e.Level > Bot.Player.Level && e.Position.GetDistance(position) < radius && Bot.Db.GetReaction(Bot.Player, e) == WowUnitReaction.Hostile);
        }

        private bool IsAnotherPlayerCompeting(IWowGameobject obj)
        {
            return Bot.Objects.All.OfType<IWowPlayer>()
                .Where(p => p.Guid != Bot.Player.Guid)
                .Any(p => p.Position.GetDistance(obj.Position) < 4f || IsPlayerMovingTowards(p, obj.Position, TimingConfig.CompetingRadius));
        }

        private bool IsPlayerMovingTowards(IWowPlayer player, Vector3 targetPosition, float maxDistance)
        {
            float dist = player.Position.GetDistance(targetPosition);
            return dist <= maxDistance && dist >= 2f && BotMath.IsFacing(player.Position, player.Rotation, targetPosition, 0.7f);
        }
    }
}
