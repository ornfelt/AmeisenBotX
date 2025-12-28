using AmeisenBotX.BehaviorTree.Enums;
using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Core.Logic.Routines;
using AmeisenBotX.Logging;
using AmeisenBotX.Wow.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Logic.Services
{
    /// <summary>
    /// Service for loot logic.
    /// Provides methods to scan for loot and execute the looting behavior.
    /// </summary>
    public class LootService(AmeisenBotInterfaces bot, AmeisenBotConfig config)
    {
        private readonly AmeisenBotInterfaces Bot = bot;
        private readonly AmeisenBotConfig Config = config;

        // Loot queue
        public Queue<ulong> UnitsToLoot { get; } = new();

        public List<ulong> UnitsLooted { get; } = [];

        // Timeout tracking
        private ulong _currentLootGuid;
        private DateTime _lootStartTime;
        private DateTime _lootWindowOpenTime = DateTime.MinValue;
        private int _lootTryCount;

        // Throttled events
        private readonly TimegatedEvent LootCheckEvent = new(TimingConfig.LootCheck);
        private readonly TimegatedEvent LootTryEvent = new(TimingConfig.LootTryInterval);
        private readonly TimegatedEvent UnitsLootedCleanupEvent = new(TimeSpan.FromMilliseconds(800 + Random.Shared.Next(400)));

        /// <summary>
        /// Scans for lootable units and updates the queue.
        /// Returns True if new loot was found or units are in queue.
        /// </summary>
        public bool ScanForLoot()
        {
            // Bags full check
            if (Bot.Character.Inventory.FreeBagSlots < 1 && !Config.AutoDestroyTrash)
            {
                return false;
            }

            // Cleanup already-looted units that are no longer dead
            if (UnitsLootedCleanupEvent.Run())
            {
                UnitsLooted.RemoveAll(guid =>
                {
                    IWowUnit unit = Bot.GetWowObjectByGuid<IWowUnit>(guid);
                    return unit != null && !unit.IsDead;
                });
            }

            // Throttled scan for new lootable units
            if (LootCheckEvent.Run())
            {
                // Scan for lootable units
                foreach (IWowUnit unit in GetLootableUnits())
                {
                    if (!UnitsLooted.Contains(unit.Guid) && !UnitsToLoot.Contains(unit.Guid))
                    {
                        UnitsToLoot.Enqueue(unit.Guid);
                    }
                }
            }

            return UnitsToLoot.Count > 0;
        }

        /// <summary>
        /// Execute the looting behavior. Returns BtStatus for behavior tree integration.
        /// </summary>
        public BtStatus ExecuteLoot()
        {
            if (UnitsToLoot.Count == 0)
            {
                return BtStatus.Failed;
            }

            // Inventory check before attempting loot
            if (Bot.Character.Inventory.FreeBagSlots < 1)
            {
                if (Config.AutoDestroyTrash && TrashItemsRoutine.TryDeleteOneItem(Bot, Config))
                {
                    return BtStatus.Ongoing; // Try again next tick
                }

                // Bags full, can't loot
                UnitsToLoot.Clear();
                Bot.IsLooting = false;
                return BtStatus.Failed;
            }

            ulong? unitGuid = UnitsToLoot.Peek();
            IWowUnit unit = Bot.GetWowObjectByGuid<IWowUnit>(unitGuid.Value);
            int skinningSkill = Bot.Character.Skills.TryGetValue("Skinning", out (int val, int max) s) ? s.val : 0;

            // Invalid unit or too many tries
            if (unit == null || (!unit.IsLootable && !(skinningSkill > 0 && unit.IsSkinnable)) || _lootTryCount > 2)
            {
                FinishLootingUnit();
                // Return Failed so we can either pick next in queue (if BT loops) or exit
                return BtStatus.Failed;
            }

            // Unit too far away
            if (unit.Position != Vector3.Zero && Bot.Player.DistanceTo(unit) > Config.LootUnitsRadius * 2.0f)
            {
                FinishLootingUnit();
                return BtStatus.Failed;
            }

            Bot.IsLooting = true;

            // Move to unit if not in range
            if (unit.Position != Vector3.Zero && Bot.Player.DistanceTo(unit) > TimingConfig.LootRange)
            {
                Bot.Movement.SetMovementAction(MovementAction.Move, unit.Position);
                return BtStatus.Ongoing;
            }

            // Timeout detection
            if (_currentLootGuid != unit.Guid)
            {
                _currentLootGuid = unit.Guid;
                _lootStartTime = DateTime.UtcNow;
            }
            else if (DateTime.UtcNow - _lootStartTime > TimingConfig.LootTimeout)
            {
                AmeisenLogger.I.Log("LootService", $"Loot timeout for unit {unit.Guid}");
                FinishLootingUnit();
                return BtStatus.Failed;
            }

            // Attempt loot interaction
            if (LootTryEvent.Run())
            {
                if (Bot.Player.IsCasting)
                {
                    return BtStatus.Ongoing;
                }

                if (Bot.Memory.Read(Bot.Memory.Offsets.LootWindowOpen, out byte lootOpen) && lootOpen > 0)
                {
                    // Track window open time for stuck detection
                    if (_lootWindowOpenTime == DateTime.MinValue)
                    {
                        _lootWindowOpenTime = DateTime.UtcNow;
                    }

                    // Force close if window stuck
                    if (DateTime.UtcNow - _lootWindowOpenTime > TimingConfig.LootWindowStuckTimeout)
                    {
                        Bot.Wow.ClickUiElement("LootCloseButton");
                        FinishLootingUnit();
                        return BtStatus.Failed;
                    }

                    // Loot everything and close
                    Bot.Wow.LootEverything();
                    Bot.Wow.ClickUiElement("LootCloseButton");
                    FinishLootingUnit();
                    return BtStatus.Success;
                }
                else
                {
                    _lootWindowOpenTime = DateTime.MinValue;
                    Bot.Wow.StopClickToMove();
                    Bot.Wow.InteractWithUnit(unit);
                    _lootTryCount++;
                }
            }

            return BtStatus.Ongoing;
        }

        public bool HasLootInQueue()
        {
            return UnitsToLoot.Count > 0;
        }

        private void FinishLootingUnit()
        {
            if (UnitsToLoot.Count > 0)
            {
                UnitsLooted.Add(UnitsToLoot.Dequeue());
            }
            _lootTryCount = 0;
            _currentLootGuid = 0;
            _lootWindowOpenTime = DateTime.MinValue;
            Bot.IsLooting = false;
        }

        public void Reset()
        {
            UnitsToLoot.Clear();
            UnitsLooted.Clear();
            _currentLootGuid = 0;
            _lootTryCount = 0;
            _lootWindowOpenTime = DateTime.MinValue;
            Bot.IsLooting = false;
        }

        private IEnumerable<IWowUnit> GetLootableUnits()
        {
            int skinningSkill = Bot.Character.Skills.TryGetValue("Skinning", out (int val, int max) s) ? s.val : 0;

            return Bot.Objects.All.OfType<IWowUnit>()
                .Where(e => (e.IsLootable || (skinningSkill >= 1 && e.IsSkinnable))
                    && e.IsDead
                    && (e.IsTaggedByMe || !e.IsTaggedByOther)
                    && e.Position.GetDistance(Bot.Player.Position) < 30.0f);
        }
    }
}
