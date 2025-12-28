using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using AmeisenBotX.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Logic.Routines
{
    /// <summary>
    /// Automatically uses quest items that can be used (e.g., "Use this item" quests).
    /// Runs periodically to check for usable quest items in inventory.
    /// </summary>
    public class QuestItemUserRoutine
    {
        private readonly AmeisenBotInterfaces Bot;
        private readonly AmeisenBotConfig Config;
        private readonly TimegatedEvent UseItemEvent;

        // Items that should NOT be auto-used (important or consumable quest items)
        private static readonly HashSet<string> DoNotAutoUsePatterns =
        [
            "Hearthstone", "Ruhestein",
            "Potion", "Elixir", "Flask", "Trank",
            "Bandage", "Verband",
            "Food", "Water", "Drink",
            "Scroll", "Schriftrolle"
        ];

        public QuestItemUserRoutine(AmeisenBotInterfaces bot, AmeisenBotConfig config)
        {
            Bot = bot;
            Config = config;
            UseItemEvent = new(TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Check for and use any usable quest items.
        /// </summary>
        public void Update()
        {
            if (!UseItemEvent.Run())
            {
                return;
            }

            // Don't use items while in combat, dead, or casting
            if (Bot.Player == null || Bot.Player.IsDead || Bot.Player.IsInCombat || Bot.Player.IsCasting)
            {
                return;
            }

            TryUseQuestItems();
        }

        private void TryUseQuestItems()
        {
            // Find usable quest items in inventory
            // Quest items that are "usable" typically have a Use: effect in their tooltip
            // We detect this via the IsUsable/Lootable flags or by checking if it's a quest item with no equip slot

            foreach (IWowInventoryItem item in Bot.Character.Inventory.Items.ToList())
            {
                // Must be a quest item
                if (!ItemEvaluator.IsQuestItem(item))
                {
                    continue;
                }

                // Skip items that shouldn't be auto-used
                if (ShouldNotAutoUse(item))
                {
                    continue;
                }

                // Check if item is usable (has a Use: effect)
                // We can try using it via Lua and see if it works
                if (TryUseItem(item))
                {
                    AmeisenLogger.I.Log("QuestItemUser", $"Used quest item: {item.Name}");
                    // Only use one item per tick to avoid issues
                    return;
                }
            }
        }

        private bool ShouldNotAutoUse(IWowInventoryItem item)
        {
            string nameLower = item.Name?.ToLowerInvariant() ?? "";

            foreach (string pattern in DoNotAutoUsePatterns)
            {
                if (nameLower.Contains(pattern.ToLowerInvariant()))
                {
                    return true;
                }
            }

            return false;
        }

        private bool TryUseItem(IWowInventoryItem item)
        {
            // Use Lua to detect if item is usable while avoiding opening letters that do not start quests.
            // Readable items (letters) are only used if they explicitly say "Begins a Quest".
            // Non-readable items (soul gems etc) are used if IsUsableItem is true.
            string luaScript = $@"
                AmeisenBot_QuestItemResult = 0
                local bag, slot = {item.BagId}, {item.BagSlot}
                local link = GetContainerItemLink(bag, slot)

                if link then
                    local _, _, _, _, readable = GetContainerItemInfo(bag, slot)
                    
                    if not AmeisenBotScanTooltip then 
                        CreateFrame('GameTooltip', 'AmeisenBotScanTooltip', UIParent, 'GameTooltipTemplate') 
                    end
                    
                    AmeisenBotScanTooltip:SetOwner(UIParent, 'ANCHOR_NONE')
                    AmeisenBotScanTooltip:SetBagItem(bag, slot)

                    local startsQuest = false
                    for i = 1, AmeisenBotScanTooltip:NumLines() do
                        local line = _G['AmeisenBotScanTooltipTextLeft'..i]
                        if line and line:GetText() then
                            if string.find(line:GetText(), 'Begins a Quest') then
                                startsQuest = true
                                break
                            end
                        end
                    end

                    local shouldSkip = false
                    if readable and not startsQuest then shouldSkip = true end

                    if not shouldSkip then
                        local usable, noMana = IsUsableItem(link)
                        if usable and not noMana then
                            UseContainerItem(bag, slot)
                            AmeisenBot_QuestItemResult = 1
                        end
                    end
                end
            ";

            return Bot.Wow.ExecuteLuaAndRead((luaScript, "AmeisenBot_QuestItemResult"), out string resultStr) && resultStr == "1";
        }
    }
}
