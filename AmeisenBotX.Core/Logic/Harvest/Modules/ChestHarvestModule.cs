using AmeisenBotX.Wow;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;

namespace AmeisenBotX.Core.Logic.Harvest.Modules
{
    /// <summary>
    /// PERFECTED Chest harvest module - always loads (everyone can loot chests).
    /// Smart bag space validation prevents clogging inventory.
    /// </summary>
    public class ChestHarvestModule : IHarvestModule
    {
        private readonly AmeisenBotInterfaces Bot;
        private const int MIN_BAG_SLOTS_FOR_CHEST = 3; // Chests often have 2-4 items

        public string Name => "Chests";

        public ChestHarvestModule(AmeisenBotInterfaces bot)
        {
            Bot = bot ?? throw new ArgumentNullException(nameof(bot));
        }

        public bool ShouldLoad(AmeisenBotInterfaces bot)
        {
            // Everyone can loot chests - always load
            return bot != null;
        }

        /// <summary>
        /// Fast type check - is this a chest?
        /// </summary>
        public bool Matches(IWowGameobject gobject)
        {
            if (gobject == null || gobject.GameObjectType != WowGameObjectType.Chest)
            {
                return false;
            }

            // If it's sparkling, it's definitely interesting (Quest or special Chest).
            // (QuestObjectHarvestModule usually picks these up first with higher priority).
            if (gobject.IsSparkling)
            {
                return true;
            }

            // If it's NOT sparkling, it might be a standard loot chest OR a useless quest object.
            // We only match standard loot chests based on name to avoid garbage.
            return IsStandardLootChest(gobject.Name);
        }

        private readonly string[] KnownLootChestNames = 
        [
            "Chest", "Trunk", "Footlocker", "Lockbox", "Strongbox"
        ];

        private bool IsStandardLootChest(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;

            foreach (string safeName in KnownLootChestNames)
            {
                if (name.Contains(safeName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Inventory check - do we have enough bag space?
        /// NOTE: IsUsable checked globally, Matches() already passed.
        /// </summary>
        public bool CanHarvest(IWowGameobject gobject)
        {
            if (gobject == null || Bot?.Character?.Inventory == null)
            {
                return false;
            }

            // Safety: Ensure we don't accidentally loot profession nodes that might be flagged as chests
            // This prevents bots without skills from trying to open ore/herb nodes
            if (WowHarvestHelper.IsOre(gobject.DisplayId) || WowHarvestHelper.IsHerb(gobject.DisplayId, gobject.EntryId))
            {
                return false;
            }

            // Need sufficient bag space (chests often have 2-4 items)
            return Bot.Character.Inventory.FreeBagSlots >= MIN_BAG_SLOTS_FOR_CHEST;
        }

        public int GetPriority(IWowGameobject gobject)
        {
            if (gobject == null)
            {
                return 0;
            }

            // High base priority - chests are free loot!
            const int BASE_PRIORITY = 100;

            // Could be enhanced with chest-type detection if needed:
            // - Rare spawn chests = higher priority
            // - Dungeon chests = highest priority
            // - Quest chests = moderate priority
            // But for now, all chests are equally valuable

            return BASE_PRIORITY;
        }
    }
}
