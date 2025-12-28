using AmeisenBotX.Logging;

namespace AmeisenBotX.Core.Logic.Routines
{
    /// <summary>
    /// Proactive bag space monitoring system.
    /// Checks bag space before looting and triggers vendor/delete states when needed.
    /// </summary>
    public class BagSpaceMonitor
    {
        private readonly AmeisenBotInterfaces Bot;
        private readonly AmeisenBotConfig Config;

        // Minimum free slots required before attempting to loot
        private const int MinimumFreeSlotsForLooting = 3;

        // Critical threshold - triggers vendor/delete state
        private const int CriticalBagSpaceThreshold = 1;

        // Emergency threshold - forces trash deletion
        private const int EmergencyBagSpaceThreshold = 0;

        public BagSpaceMonitor(AmeisenBotInterfaces bot, AmeisenBotConfig config)
        {
            Bot = bot;
            Config = config;
        }

        /// <summary>
        /// Check if we have enough bag space to loot safely.
        /// </summary>
        public bool CanLootSafely()
        {
            return Bot.Character.Inventory.FreeBagSlots >= MinimumFreeSlotsForLooting;
        }

        /// <summary>
        /// Check if bag space is critically low (should trigger vendor state).
        /// </summary>
        public bool IsBagSpaceCritical()
        {
            return Bot.Character.Inventory.FreeBagSlots <= CriticalBagSpaceThreshold;
        }

        /// <summary>
        /// Check if bags are completely full (emergency).
        /// </summary>
        public bool IsBagSpaceEmergency()
        {
            return Bot.Character.Inventory.FreeBagSlots <= EmergencyBagSpaceThreshold;
        }

        /// <summary>
        /// Attempt to make emergency bag space by deleting trash items.
        /// Returns true if successful in creating enough space.
        /// </summary>
        public bool TryMakeEmergencySpace()
        {
            if (CanLootSafely())
            {
                return true; // Already have enough space
            }

            AmeisenLogger.I.Log("BagSpaceMonitor",
                $"Low bag space detected ({Bot.Character.Inventory.FreeBagSlots} free), attempting emergency cleanup...");

            // Try to delete trash to make space
            int deleted = TrashItemsRoutine.TryMakeBagSpace(
                Bot,
                Config,
                MinimumFreeSlotsForLooting
            );

            if (deleted > 0)
            {
                AmeisenLogger.I.Log("BagSpaceMonitor",
                    $"Emergency cleanup: deleted {deleted} items, now have {Bot.Character.Inventory.FreeBagSlots} free slots");
                return CanLootSafely();
            }

            AmeisenLogger.I.Log("BagSpaceMonitor",
                "Emergency cleanup failed - no suitable trash items found");
            return false;
        }

        /// <summary>
        /// Get a summary of current bag space status.
        /// </summary>
        public string GetStatusSummary()
        {
            int freeSlots = Bot.Character.Inventory.FreeBagSlots;
            string status = freeSlots switch
            {
                <= EmergencyBagSpaceThreshold => "EMERGENCY",
                <= CriticalBagSpaceThreshold => "CRITICAL",
                < MinimumFreeSlotsForLooting => "LOW",
                _ => "OK"
            };

            return $"{status} ({freeSlots} free slots)";
        }
    }
}
