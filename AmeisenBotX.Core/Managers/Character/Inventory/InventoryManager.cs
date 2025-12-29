using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Logic.Routines;
using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using AmeisenBotX.Logging;
using AmeisenBotX.Logging.Enums;
using AmeisenBotX.Wow;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AmeisenBotX.Core.Managers.Character.Inventory
{
    /// <summary>
    /// Centralized inventory management.
    /// Handles item data parsing, storage, organization (stacking/sorting), and maintenance (selling/deleting).
    /// Replaces CharacterInventory and InventoryOrganizer.
    /// </summary>
    public class InventoryManager
    {
        private readonly AmeisenBotInterfaces Bot;
        private readonly AmeisenBotConfig Config;
        private readonly Lock queryLock = new();

        private List<IWowInventoryItem> items;

        // Organization Logic
        private readonly TimegatedEvent OrganizeEvent;
        private readonly TimegatedEvent VendorCheckEvent;
        private readonly TimegatedEvent SortEvent;
        private readonly BagSpaceMonitor BagMonitor;

        // State Tracking
        private int LastItemCount = -1;
        private int LastFreeBagSlots = -1;
        private ulong LastVendorSoldTo = 0;
        private bool IsNearVendor = false;
        private bool ConfirmDelete = false;
        private DateTime ConfirmDeleteTime;
        private bool IsSorting = false;

        // Constants
        private const float VendorInteractDistance = 6.0f;

        public InventoryManager(AmeisenBotInterfaces bot, AmeisenBotConfig config)
        {
            Bot = bot ?? throw new ArgumentNullException(nameof(bot));
            Config = config ?? throw new ArgumentNullException(nameof(config));
            Items = [];
            
            // Initialize Organizers
            BagMonitor = new BagSpaceMonitor(bot, config);
            OrganizeEvent = new(TimeSpan.FromSeconds(10));
            VendorCheckEvent = new(TimeSpan.FromSeconds(2));
            SortEvent = new(TimeSpan.FromMilliseconds(250));
        }

        #region Public Data Access

        public int FreeBagSlots { get; private set; }

        public List<IWowInventoryItem> Items
        {
            get
            {
                using (queryLock.EnterScope())
                {
                    return items;
                }
            }
            private init
            {
                using (queryLock.EnterScope())
                {
                    items = value;
                }
            }
        }

        public IWowInventoryItem GetItem(int id) => Items.FirstOrDefault(e => e.Id == id);

        public IWowInventoryItem GetItem(string name) => Items.FirstOrDefault(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        public int GetItemCount(int id)
        {
            return Items.Where(e => e.Id == id).Sum(e => e.Count);
        }

        public bool HasItemByName(string name, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            return Items.Any(e => e.Name.Equals(name, stringComparison));
        }

        public bool HasItemTypeInBag<T>(bool needsToBeUseable = false)
        {
            return Items.Any(e => Enum.IsDefined(typeof(T), e.Id));
        }

        #endregion

        #region Status Queries

        /// <summary>
        /// Indicates if bot should go to vendor due to low bag space.
        /// </summary>
        public bool ShouldVendor => BagMonitor.IsBagSpaceCritical();

        public bool CanLootSafely() => BagMonitor.CanLootSafely();

        public string GetBagSpaceStatus() => BagMonitor.GetStatusSummary();

        /// <summary>
        /// Get evaluated inventory summary for UI or Log.
        /// </summary>
        public InventorySummary GetSummary()
        {
            List<ItemScore> scores = ItemEvaluator.EvaluateInventory(Bot, Config);

            return new InventorySummary
            {
                TotalItems = scores.Count,
                ProtectedItems = scores.Count(s => s.Score >= 10_000),
                DisposableItems = scores.Count(s => s.Score < 10_000),
                JunkItems = scores.Count(s => s.Score < 100), // Below Common (Green is 2k, White is 1k, Grey is 0)
                FreeBagSlots = FreeBagSlots,
                WorstItem = scores.OrderBy(s => s.Score).FirstOrDefault(),
                BestItem = scores.OrderByDescending(s => s.Score).FirstOrDefault()
            };
        }

        #endregion

        #region Main Update Loop

        public void Update()
        {
            // 1. Refresh Data
            UpdateData();

            // 2. Organization Check
            int currentItemCount = Items.Count;
            int currentFreeBagSlots = FreeBagSlots;
            bool inventoryChanged = currentItemCount != LastItemCount || currentFreeBagSlots != LastFreeBagSlots;

            LastItemCount = currentItemCount;
            LastFreeBagSlots = currentFreeBagSlots;

            // If changed and not on cooldown, trigger organize
            if (!IsSorting && inventoryChanged && OrganizeEvent.Run())
            {
                if (Config.AutoDestroyTrash)
                {
                    OrganizeInventory();
                }
            }

            // 3. Iterative Sorting (One swap per tick)
            if (IsSorting && SortEvent.Run())
            {
                IsSorting = ExecuteNextSortSwap();
                if (!IsSorting)
                {
                    AmeisenLogger.I.Log("InventoryManager", $"Sorting Complete. {GetSummary()}");
                }
            }

            // 4. Vendor Check (Auto-Sell)
            if (Config.AutoSell && VendorCheckEvent.Run())
            {
                TryAutoSellAtNearbyVendor();
            }
        }

        private void UpdateData()
        {
            FreeBagSlots = Bot.Wow.GetFreeBagSlotCount();
            string resultJson = Bot.Wow.GetInventoryItems();

            try
            {
                List<WowBasicItem> basicItems = ItemFactory.ParseItemList(resultJson);

                if (basicItems is not { Count: > 0 })
                {
                    return;
                }

                using (queryLock.EnterScope())
                {
                    Items.Clear();
                    foreach (WowBasicItem basicItem in basicItems)
                    {
                        Items.Add(ItemFactory.BuildSpecificItem(basicItem));
                    }
                }
            }
            catch (Exception e)
            {
                AmeisenLogger.I.Log("InventoryManager", $"Failed to parse Inventory JSON:\n{resultJson}\n{e}", LogLevel.Error);
            }
        }

        #endregion

        #region Maintenance Actions (Organize/Sell/Delete)

        public void ForceOrganize() => OrganizeInventory();

        public bool TryMakeEmergencySpace() => BagMonitor.TryMakeEmergencySpace();

        public void DestroyItemByName(string name, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            if (HasItemByName(name, stringComparison))
            {
                Bot.Wow.DeleteItemByName(name);
            }
        }

        public void TryDestroyTrash(WowItemQuality maxQuality = WowItemQuality.Poor)
        {
            if (DateTime.UtcNow - ConfirmDeleteTime > TimeSpan.FromSeconds(10))
            {
                ConfirmDelete = false;
            }
            else if (ConfirmDelete)
            {
                return;
            }

            foreach (IWowInventoryItem item in Items.Where(e => e.Price > 0 && e.ItemQuality == (int)maxQuality).OrderBy(e => e.Price))
            {
                if (!Config.ItemSellBlacklist.Any(e => e.Equals(item.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    AmeisenLogger.I.Log("Inventory", $"Deleting Trash: {item.Name}");
                    Bot.Wow.DeleteItemByName(item.Name);
                    ConfirmDelete = true;
                    ConfirmDeleteTime = DateTime.UtcNow;
                    break;
                }
            }
        }

        public void OnStaticPopupDeleteItem(int id)
        {
            ConfirmDelete = false;
            Bot.Wow.ClickUiElement($"StaticPopup{id}Button1");
            AmeisenLogger.I.Log("Inventory", $"Confirmed Deleting");
        }

        private void OrganizeInventory()
        {
            if (Items.Count < 2) return;

            // Stack similar first
            StackSimilarItems();

            // Trigger Sorting Mode
            IsSorting = true;
            AmeisenLogger.I.Log("InventoryManager", "Started Inventory Sort...");
        }

        private bool ExecuteNextSortSwap()
        {
            // 1. Calculate Scores & Sort Target List
            List<IWowInventoryItem> targetList;
            using (queryLock.EnterScope())
            {
                targetList = items
                    .Select(item => new { Item = item, Score = ItemEvaluator.CalculateSortScore(Bot, Config, item) })
                    .OrderByDescending(x => x.Score)
                    .Select(x => x.Item)
                    .ToList();
            }

            // 2. Iterate through slots linearly to find first mismatch
            int currentItemIndex = 0;
            
            for (int bag = 0; bag <= 4; bag++) // Backpack (0) to Bag 4
            {
                int numSlots = Bot.Wow.GetContainerNumSlots(bag);
                for (int slot = 1; slot <= numSlots; slot++)
                {
                    // If we have placed all items, the rest should be empty.
                    // We don't explicitly clear empty slots here to save time, 
                    // as compacting happens naturally by filling top slots.
                    if (currentItemIndex >= targetList.Count)
                    {
                        return false; // All items placed correctly
                    }

                    IWowInventoryItem targetItem = targetList[currentItemIndex];

                    // Check if the item at this slot is ALREADY the target item
                    // We compare BagId/BagSlot because 'targetItem' is a snapshot of current state
                    if (targetItem.BagId == bag && targetItem.BagSlot == slot)
                    {
                        // Match! Move to next item.
                        currentItemIndex++;
                        continue;
                    }

                    // MISMATCH: The item that SHOULD be here (targetItem) is currently at (targetItem.BagId, targetItem.BagSlot).
                    // We verify that the target position is actually different (it is, per condition above).
                    
                    // SWAP: Move item FROM current location TO desired location
                    AmeisenLogger.I.Log("Sort", $"Swapping {targetItem.Name} from {targetItem.BagId}:{targetItem.BagSlot} to {bag}:{slot}");
                    
                    Bot.Wow.LuaDoString($"ClearCursor(); PickupContainerItem({targetItem.BagId}, {targetItem.BagSlot}); PickupContainerItem({bag}, {slot}); ClearCursor();");
                    
                    return true; // Performed one swap, return to wait for next tick
                }
            }

            return false; // Loop finished without swaps
        }

        private void StackSimilarItems()
        {
            Bot.Wow.LuaDoString(@"
                local processed = {}
                for bag1 = 0, 4 do
                    for slot1 = 1, GetContainerNumSlots(bag1) do
                        local itemId1 = GetContainerItemID(bag1, slot1)
                        if itemId1 and not processed[bag1..':'..slot1] then
                            local _, count1, _, _, _, _, link1, _, _, itemId = GetContainerItemInfo(bag1, slot1)
                            local _, _, _, _, _, _, _, maxStack = GetItemInfo(link1 or '')
                            if maxStack and count1 < maxStack then
                                for bag2 = 0, 4 do
                                    for slot2 = 1, GetContainerNumSlots(bag2) do
                                        if bag2 ~= bag1 or slot2 ~= slot1 then
                                            local itemId2 = GetContainerItemID(bag2, slot2)
                                            if itemId2 == itemId1 then
                                                local _, count2 = GetContainerItemInfo(bag2, slot2)
                                                if count2 and count2 < maxStack then
                                                    PickupContainerItem(bag2, slot2)
                                                    PickupContainerItem(bag1, slot1)
                                                    processed[bag2..':'..slot2] = true
                                                end
                                            end
                                        end
                                    end
                                end
                            end
                            processed[bag1..':'..slot1] = true
                        end
                    end
                end
            ");
        }

        private void TryAutoSellAtNearbyVendor()
        {
            if (Bot.Player == null || Bot.Player.IsDead || Bot.Player.IsInCombat) return;

            IWowUnit nearestVendor = Bot.Objects.All
                .OfType<IWowUnit>()
                .Where(u => u.IsVendor && !u.IsDead && u.Position.GetDistance(Bot.Player.Position) <= VendorInteractDistance)
                .OrderBy(u => u.Position.GetDistance(Bot.Player.Position))
                .FirstOrDefault();

            bool wasNearVendor = IsNearVendor;
            IsNearVendor = nearestVendor != null;

            if (!IsNearVendor)
            {
                if (wasNearVendor) LastVendorSoldTo = 0;
                return;
            }

            if (nearestVendor.Guid == LastVendorSoldTo) return;

            LastVendorSoldTo = nearestVendor.Guid;
            string vendorName = Bot.Db.GetUnitName(nearestVendor, out string name) ? name : "Unknown";
            AmeisenLogger.I.Log("InventoryManager", $"Auto-selling at nearby vendor: {vendorName}");

            Bot.Wow.ChangeTarget(nearestVendor.Guid);
            Bot.Wow.InteractWithUnit(nearestVendor);
        }

        #endregion
    }

    /// <summary>
    /// Summary of inventory state for UI display.
    /// </summary>
    public class InventorySummary
    {
        public int TotalItems { get; set; }
        public int ProtectedItems { get; set; }
        public int DisposableItems { get; set; }
        public int JunkItems { get; set; }
        public int FreeBagSlots { get; set; }
        public ItemScore WorstItem { get; set; }
        public ItemScore BestItem { get; set; }

        public override string ToString()
        {
            string worst = WorstItem != null ? $"{WorstItem.Item.Name} ({WorstItem.Score:F0})" : "N/A";
            string best = BestItem != null ? $"{BestItem.Item.Name} ({BestItem.Score:F0})" : "N/A";
            return $"Items: {TotalItems} | Protected: {ProtectedItems} | Junk: {JunkItems} | Free: {FreeBagSlots}\n" +
                   $"Worst: {worst} | Best: {best}";
        }
    }
}
