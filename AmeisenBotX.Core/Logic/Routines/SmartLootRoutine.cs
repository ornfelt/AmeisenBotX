using AmeisenBotX.Logging;
using AmeisenBotX.Wow.Objects.Enums;

namespace AmeisenBotX.Core.Logic.Routines
{
    /// <summary>
    /// Smart looting routine that:
    /// - Always takes: Money, Quest Items, Valuable items
    /// - Clears bag space if needed for valuable items
    /// - Only takes gray/white "junk" if we have free bag space
    /// - Compatible with WotLK 3.3.5a (No GetLootSlotType)
    /// </summary>
    public static class SmartLootRoutine
    {
        // Minimum bag slots to keep free for emergencies
        private const int MinFreeSlotsToKeep = 1;

        // Quality threshold - items at or below this are considered "optional"
        private const int JunkQualityThreshold = (int)WowItemQuality.Common; // Gray + White

        /// <summary>
        /// Smart loot that prioritizes valuable items and manages bag space.
        /// </summary>
        public static void Run(AmeisenBotInterfaces bot, AmeisenBotConfig config)
        {
            int freeBagSlots = bot.Character.Inventory.FreeBagSlots;

            if (freeBagSlots < MinFreeSlotsToKeep)
            {
                // Very low space - only loot money, quest items, and high-value items
                LootSelectiveHighPriority(bot, config);
            }
            else if (freeBagSlots <= 3)
            {
                // Low space - loot everything valuable, skip pure junk
                LootSelectiveSkipJunk(bot, config);
            }
            else
            {
                // Plenty of space - loot everything
                bot.Wow.LootEverything();
            }
        }

        /// <summary>
        /// Loot only money, quest items, and high quality items.
        /// Will delete trash to make room for valuable items.
        /// </summary>
        private static void LootSelectiveHighPriority(AmeisenBotInterfaces bot, AmeisenBotConfig config)
        {
            AmeisenLogger.I.Log("SmartLoot", "Low bag space - looting high priority only");

            // First pass: Loot money (no bag space needed)
            bot.Wow.LuaDoString(@"
                for i = GetNumLootItems(), 1, -1 do
                    local link = GetLootSlotLink(i)
                    if not link then -- Money or Currency
                        LootSlot(i)
                    end
                end
            ");

            // Second pass: Try to make space FIRST if bags are full
            if (config.AutoDestroyTrash && bot.Character.Inventory.FreeBagSlots < 1)
            {
                // Delete up to 2 items to ensure we have space for quest items
                int deleted = TrashItemsRoutine.TryMakeBagSpace(bot, config, 2);
                if (deleted > 0)
                {
                    AmeisenLogger.I.Log("SmartLoot", $"Deleted {deleted} items to make space for loot");
                }
            }

            // Third pass: Loot quest items (AFTER making space!)
            // Uses 3.3.5a compatible GetLootSlotInfo signature
            bot.Wow.LuaDoString(@"
                for i = GetNumLootItems(), 1, -1 do
                    local _, _, _, _, locked = GetLootSlotInfo(i)
                    local link = GetLootSlotLink(i)
                    if not locked and link then
                        local _, _, _, _, _, type = GetItemInfo(link)
                        if type == 'Quest' then
                            LootSlot(i)
                            ConfirmLootSlot(i)
                        end
                    end
                end
            ");

            // Fourth pass: Loot green+ quality items (Quality >= 2)
            bot.Wow.LuaDoString(@"
                for i = GetNumLootItems(), 1, -1 do
                    local _, _, _, quality, locked = GetLootSlotInfo(i)
                    if not locked and quality and quality >= 2 then
                        LootSlot(i)
                        ConfirmLootSlot(i)
                    end
                end
            ");
        }

        /// <summary>
        /// Loot everything except pure junk (gray items).
        /// </summary>
        private static void LootSelectiveSkipJunk(AmeisenBotInterfaces bot, AmeisenBotConfig config)
        {
            AmeisenLogger.I.Log("SmartLoot", "Medium bag space - skipping junk");

            // Loot money first
            bot.Wow.LuaDoString(@"
                for i = GetNumLootItems(), 1, -1 do
                    local link = GetLootSlotLink(i)
                    if not link then
                        LootSlot(i)
                    end
                end
            ");

            // Loot quest items (explicit pass to be safe)
            bot.Wow.LuaDoString(@"
                for i = GetNumLootItems(), 1, -1 do
                    local _, _, _, _, locked = GetLootSlotInfo(i)
                    local link = GetLootSlotLink(i)
                    if not locked and link then
                        local _, _, _, _, _, type = GetItemInfo(link)
                        if type == 'Quest' then
                            LootSlot(i)
                            ConfirmLootSlot(i)
                        end
                    end
                end
            ");

            // Loot items with quality > Poor (skip gray junk)
            // Quality: 0=Gray, 1=White, 2=Green...
            bot.Wow.LuaDoString(@"
                for i = GetNumLootItems(), 1, -1 do
                    local _, _, _, quality, locked = GetLootSlotInfo(i)
                    local link = GetLootSlotLink(i)
                    -- Loot if quality > 0 (White/Common or better)
                    if not locked and link and quality and quality > 0 then
                        LootSlot(i)
                        ConfirmLootSlot(i)
                    end
                end
            ");

            // If we still have space, grab the gray items too
            if (bot.Character.Inventory.FreeBagSlots > MinFreeSlotsToKeep)
            {
                bot.Wow.LuaDoString(@"
                    for i = GetNumLootItems(), 1, -1 do
                        local _, _, _, quality, locked = GetLootSlotInfo(i)
                        local link = GetLootSlotLink(i)
                        if not locked and link and quality == 0 then
                            LootSlot(i)
                            ConfirmLootSlot(i)
                        end
                    end
                ");
            }
        }
    }
}
