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

        public bool CanHarvest(IWowGameobject gobject)
        {
            // Bulletproof null checks
            if (gobject == null || Bot?.Character?.Inventory == null)
            {
                return false;
            }

            // CRITICAL: Must be a chest type
            if (gobject.GameObjectType != WowGameObjectType.Chest)
            {
                return false;
            }

            // CRITICAL: Need sufficient bag space
            // Chests often contain multiple items (2-4 items typical)
            // Require 3+ free slots to prevent inventory spam
            int freeBagSlots = Bot.Character.Inventory.FreeBagSlots;
            return freeBagSlots >= MIN_BAG_SLOTS_FOR_CHEST;
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
