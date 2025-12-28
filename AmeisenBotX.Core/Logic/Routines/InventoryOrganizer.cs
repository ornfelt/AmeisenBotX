using AmeisenBotX.Common.Utils;
using AmeisenBotX.Logging;
using AmeisenBotX.Wow.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Logic.Routines
{
    /// <summary>
    /// Inventory organizer that:
    /// - Sorts bags by item importance using our ItemEvaluator logic
    /// - Groups similar items together
    /// - Auto-sells at nearby vendors (once per vendor visit)
    /// - Runs on inventory changes, throttled to max every 10 seconds
    /// </summary>
    public class InventoryOrganizer
    {
        private readonly AmeisenBotInterfaces Bot;
        private readonly AmeisenBotConfig Config;
        private readonly TimegatedEvent OrganizeEvent;
        private readonly TimegatedEvent VendorCheckEvent;
        private readonly BagSpaceMonitor BagMonitor;

        // Track inventory state to detect changes
        private int LastItemCount;
        private int LastFreeBagSlots;

        // Track last vendor we sold to - only sell once per proximity
        private ulong LastVendorSoldTo;
        private bool IsNearVendor;

        // Distance threshold for "near vendor"
        private const float VendorInteractDistance = 6.0f;

        // === PUBLIC API FOR STATE MACHINE INTEGRATION ===

        /// <summary>
        /// Indicates if bot should go to vendor due to low bag space.
        /// State machine should check this to trigger vendor/sell state.
        /// </summary>
        public bool ShouldVendor => BagMonitor.IsBagSpaceCritical();

        /// <summary>
        /// Check if bags have enough space to loot safely.
        /// Looting logic should call this before attempting to loot.
        /// </summary>
        public bool CanLootSafely() => BagMonitor.CanLootSafely();

        /// <summary>
        /// Attempt to make emergency bag space by deleting trash.
        /// Returns true if successful in creating enough space.
        /// </summary>
        public bool TryMakeEmergencySpace() => BagMonitor.TryMakeEmergencySpace();

        /// <summary>
        /// Get current bag space status summary.
        /// </summary>
        public string GetBagSpaceStatus() => BagMonitor.GetStatusSummary();

        public InventoryOrganizer(AmeisenBotInterfaces bot, AmeisenBotConfig config)
        {
            Bot = bot;
            Config = config;
            BagMonitor = new BagSpaceMonitor(bot, config);
            OrganizeEvent = new(TimeSpan.FromSeconds(10));
            VendorCheckEvent = new(TimeSpan.FromSeconds(2));
            LastItemCount = -1;
            LastFreeBagSlots = -1;
            LastVendorSoldTo = 0;
            IsNearVendor = false;
        }

        /// <summary>
        /// Check if inventory changed and organize if needed.
        /// Also checks for nearby vendors to auto-sell.
        /// </summary>
        public void Update()
        {
            // Check if inventory changed
            int currentItemCount = Bot.Character.Inventory.Items.Count;
            int currentFreeBagSlots = Bot.Character.Inventory.FreeBagSlots;

            bool inventoryChanged = currentItemCount != LastItemCount
                || currentFreeBagSlots != LastFreeBagSlots;

            LastItemCount = currentItemCount;
            LastFreeBagSlots = currentFreeBagSlots;

            // If changed and not on cooldown, organize
            if (inventoryChanged && OrganizeEvent.Run())
            {
                OrganizeInventory();
            }

            // Check for nearby vendors to auto-sell
            if (Config.AutoSell && VendorCheckEvent.Run())
            {
                TryAutoSellAtNearbyVendor();
            }
        }

        /// <summary>
        /// Force an inventory organization.
        /// </summary>
        public void ForceOrganize()
        {
            OrganizeInventory();
        }

        /// <summary>
        /// Get evaluated inventory for UI display.
        /// </summary>
        public List<ItemScore> GetEvaluatedInventory()
        {
            return ItemEvaluator.EvaluateInventory(Bot, Config);
        }

        private void TryAutoSellAtNearbyVendor()
        {
            // Don't sell while in combat or dead
            if (Bot.Player == null || Bot.Player.IsDead || Bot.Player.IsInCombat)
            {
                return;
            }

            // Find nearest vendor
            IWowUnit nearestVendor = Bot.Objects.All
                .OfType<IWowUnit>()
                .Where(u => u.IsVendor
                    && !u.IsDead
                    && u.Position.GetDistance(Bot.Player.Position) <= VendorInteractDistance)
                .OrderBy(u => u.Position.GetDistance(Bot.Player.Position))
                .FirstOrDefault();

            bool wasNearVendor = IsNearVendor;
            IsNearVendor = nearestVendor != null;

            if (!IsNearVendor)
            {
                // Left vendor area - reset so we can sell again when we return
                if (wasNearVendor)
                {
                    LastVendorSoldTo = 0;
                }
                return;
            }

            // Already sold to this vendor while nearby
            if (nearestVendor.Guid == LastVendorSoldTo)
            {
                return;
            }

            // New vendor or returned to same vendor! Interact and sell
            LastVendorSoldTo = nearestVendor.Guid;

            string vendorName = Bot.Db.GetUnitName(nearestVendor, out string name) ? name : "Unknown";
            AmeisenLogger.I.Log("InventoryOrganizer",
                $"Auto-selling at nearby vendor: {vendorName}");

            // Target and interact with vendor
            Bot.Wow.ChangeTarget(nearestVendor.Guid);
            Bot.Wow.InteractWithUnit(nearestVendor);

            // The actual sell will happen in OnMerchantShow event handler
        }

        private void OrganizeInventory()
        {
            if (Bot.Character.Inventory.Items.Count < 2)
            {
                return;
            }

            AmeisenLogger.I.Log("InventoryOrganizer", "Organizing inventory using ItemEvaluator...");

            // Evaluate all items using our advanced ItemEvaluator
            List<ItemScore> scoredItems = ItemEvaluator.EvaluateInventory(Bot, Config);

            // Group items by protected status
            List<ItemScore> protectedItems = scoredItems.Where(s => s.Score >= 1000).ToList();
            List<ItemScore> disposableItems = scoredItems.Where(s => s.Score < 1000).OrderBy(s => s.Score).ToList();

            // Stack similar items together to save space
            StackSimilarItems();

            // Defragment (Compact) items to the left
            CompactInventory();

            // Log the current organization
            AmeisenLogger.I.Log("InventoryOrganizer",
                $"Inventory analyzed: Protected={protectedItems.Count}, Disposable={disposableItems.Count}");

            if (disposableItems.Count > 0)
            {
                ItemScore worstItem = disposableItems.First();
                AmeisenLogger.I.Log("InventoryOrganizer",
                    $"Lowest value item: {worstItem.Item.Name} (Score: {worstItem.Score:F1})");
            }
        }

        private void CompactInventory()
        {
            Bot.Wow.LuaDoString(@"
                local function GetBagSize(bag)
                    return GetContainerNumSlots(bag)
                end

                local function MoveItem(fromBag, fromSlot, toBag, toSlot)
                    ClearCursor()
                    PickupContainerItem(fromBag, fromSlot)
                    PickupContainerItem(toBag, toSlot)
                    ClearCursor()
                end

                -- Simple deformeter: Move items from high slots to low empty slots
                for toBag = 0, 4 do
                    for toSlot = 1, GetBagSize(toBag) do
                        local itemId = GetContainerItemID(toBag, toSlot)
                        if not itemId then
                            -- Found empty slot, look for item to fill it from END of bags
                            local found = false
                            for fromBag = 4, toBag, -1 do
                                local startSlot = GetBagSize(fromBag)
                                local endSlot = 1
                                if fromBag == toBag then startSlot = GetBagSize(toBag) end
                                
                                for fromSlot = startSlot, endSlot, -1 do
                                    -- Ensure we only look 'after' the current empty slot
                                    if (fromBag > toBag) or (fromBag == toBag and fromSlot > toSlot) then
                                        if GetContainerItemID(fromBag, fromSlot) then
                                            MoveItem(fromBag, fromSlot, toBag, toSlot)
                                            found = true
                                            break
                                        end
                                    end
                                end
                                if found then break end
                            end
                        end
                    end
                end
            ");
        }

        private void StackSimilarItems()
        {
            // Stack incomplete stacks together using Lua
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

        /// <summary>
        /// Get a summary of inventory organization.
        /// </summary>
        public InventorySummary GetSummary()
        {
            List<ItemScore> scores = ItemEvaluator.EvaluateInventory(Bot, Config);

            return new InventorySummary
            {
                TotalItems = scores.Count,
                ProtectedItems = scores.Count(s => s.Score >= 1000),
                DisposableItems = scores.Count(s => s.Score < 1000),
                JunkItems = scores.Count(s => s.Score < 20),
                FreeBagSlots = Bot.Character.Inventory.FreeBagSlots,
                WorstItem = scores.OrderBy(s => s.Score).FirstOrDefault(),
                BestItem = scores.OrderByDescending(s => s.Score).FirstOrDefault()
            };
        }
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
