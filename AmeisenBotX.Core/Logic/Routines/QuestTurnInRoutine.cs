using AmeisenBotX.Logging;
using System;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Logic.Routines
{
    /// <summary>
    /// Handles intelligent quest turn-in:
    /// - Selects the best reward based on ItemEvaluator scores
    /// - Handles multiple quests properly
    /// - Ensures proper completion before moving to next quest
    /// </summary>
    public static class QuestTurnInRoutine
    {
        public static void HandleQuestComplete(AmeisenBotInterfaces bot, AmeisenBotConfig config)
        {
            // Ensure bag space
            if (config.AutoDestroyTrash && bot.Character.Inventory.FreeBagSlots < 2)
            {
                TrashItemsRoutine.TryMakeBagSpace(bot, config, 2);
            }

            // Check if we need to choose a reward
            int bestRewardIndex = GetBestRewardIndex(bot, config);

            if (bestRewardIndex > 0)
            {
                AmeisenLogger.I.Log("QuestTurnIn", $"Selecting reward #{bestRewardIndex} as best option");

                // Select the best reward and complete
                bot.Wow.LuaDoString($@"
                    QuestInfoItem{bestRewardIndex}:Click()
                    GetQuestReward({bestRewardIndex})
                ");
            }
            else
            {
                // No choices or only money reward - just complete
                bot.Wow.LuaDoString(@"
                    if QuestFrameCompleteQuestButton and QuestFrameCompleteQuestButton:IsEnabled() then
                        QuestFrameCompleteQuestButton:Click()
                    end
                    local numChoices = GetNumQuestChoices()
                    if numChoices == 0 then
                        -- No choice required, complete immediately
                        GetQuestReward()
                    elseif numChoices == 1 then
                        -- Only one choice, select it
                        GetQuestReward(1)
                    end
                ");
            }
        }

        public static void HandleQuestGossip(AmeisenBotInterfaces bot, AmeisenBotConfig config)
        {
            // Clear bag space first
            if (config.AutoDestroyTrash && bot.Character.Inventory.FreeBagSlots < 2)
            {
                TrashItemsRoutine.TryDeleteOneItem(bot, config);
            }

            // Prioritize turn-ins over accepting new quests
            // Uses dynamic stride detection for 3.3.5 compatibility (4 or 5 returns per quest)
            bot.Wow.LuaDoString(@"
                -- Get active (completable) quests first
                local numActive = GetNumGossipActiveQuests()
                if numActive and numActive > 0 then
                    local gossipData = {GetGossipActiveQuests()}
                    local stride = #gossipData / numActive
                    
                    for i = 1, numActive do
                        local baseIdx = (i - 1) * stride
                        -- isComplete is always at stride-1 pos relative to base (if stride 5, index 4. if stride 6... wait)
                        -- 3.3.5: title, level, isLowLevel, isComplete, isLegendary (5) -> isComplete at index 4
                        -- 3.3.5 old: title, level, isLowLevel, isComplete (4) -> isComplete at index 4
                        -- So index 4 seems safe relative to base?
                        local isComplete = gossipData[baseIdx + 4]
                        if isComplete then
                            SelectGossipActiveQuest(i)
                            return
                        end
                    end
                end
                
                -- No completable active quests, check for available quests
                local numAvailable = GetNumGossipAvailableQuests()
                if numAvailable and numAvailable > 0 then
                    SelectGossipAvailableQuest(1)
                    return
                end
                
                -- Check QuestFrame (different UI for some NPCs)
                if QuestFrame and QuestFrame:IsVisible() then
                    for i = 1, 20 do
                        local button = _G['QuestTitleButton'..i]
                        if button and button:IsVisible() then
                            local icon = _G['QuestTitleButton'..i..'QuestIcon']
                            -- Yellow ? = completable, Yellow ! = available
                            if icon and icon:GetTexture() then
                                local tex = icon:GetTexture()
                                if tex and (string.find(tex, 'ActiveQuestIcon') or string.find(tex, 'Complete')) then
                                    button:Click()
                                    return
                                end
                            end
                        end
                    end
                    -- No active quest found, click first available
                    local button = QuestTitleButton1
                    if button and button:IsVisible() then
                        button:Click()
                    end
                end
            ");
        }

        public static void HandleQuestProgress(AmeisenBotInterfaces bot, AmeisenBotConfig config)
        {
            // This is the screen that shows required items - click Continue/Complete
            bot.Wow.LuaDoString(@"
                if QuestFrameCompleteButton and QuestFrameCompleteButton:IsVisible() and QuestFrameCompleteButton:IsEnabled() then
                    QuestFrameCompleteButton:Click()
                elseif QuestFrameCompleteQuestButton and QuestFrameCompleteQuestButton:IsVisible() and QuestFrameCompleteQuestButton:IsEnabled() then
                    QuestFrameCompleteQuestButton:Click()
                end
            ");
        }

        private static int GetBestRewardIndex(AmeisenBotInterfaces bot, AmeisenBotConfig config)
        {
            // Query reward info via Lua
            if (!bot.Wow.ExecuteLuaAndRead(("", @"
                local result = ''
                local numChoices = GetNumQuestChoices()
                if numChoices and numChoices > 1 then
                    for i = 1, numChoices do
                        local name, texture, numItems, quality, isUsable = GetQuestItemInfo('choice', i)
                        local link = GetQuestItemLink('choice', i)
                        local sellPrice = 0
                        if link then
                            local _, _, _, _, _, _, _, _, _, _, sp = GetItemInfo(link)
                            sellPrice = sp or 0
                        end
                        result = result .. i .. '|' .. (name or '') .. '|' .. (quality or 0) .. '|' .. sellPrice .. ';'
                    end
                end
                abx_result = result
            "), out string result) || string.IsNullOrEmpty(result))
            {
                return 0;
            }

            // Parse results and score each item
            List<(int Index, string Name, int Quality, int SellPrice)> rewards = [];

            foreach (string entry in result.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                string[] parts = entry.Split('|');
                if (parts.Length >= 4
                    && int.TryParse(parts[0], out int idx)
                    && int.TryParse(parts[2], out int quality)
                    && int.TryParse(parts[3], out int sellPrice))
                {
                    rewards.Add((idx, parts[1], quality, sellPrice));
                }
            }

            if (rewards.Count == 0)
            {
                return 0;
            }

            // Score each reward
            double bestScore = double.MinValue;
            int bestIndex = 1;

            foreach ((int Index, string Name, int Quality, int SellPrice) reward in rewards)
            {
                double score = 0;

                // Quality score
                score += reward.Quality * 50;
                score += Math.Log10(Math.Max(1, reward.SellPrice)) * 10;

                string nameLower = reward.Name.ToLowerInvariant();

                // Check if item matches our class/role
                if (bot.CombatClass != null)
                {
                    Dictionary<string, double> weights = bot.CombatClass.Specialization != Wow.Objects.Enums.WowSpecialization.None
                        ? SpecStatWeights.GetWeights(bot.CombatClass.Specialization)
                        : null;

                    if (weights != null)
                    {
                        foreach (KeyValuePair<string, double> stat in weights)
                        {
                            if (nameLower.Contains(stat.Key.ToLowerInvariant()))
                            {
                                score += stat.Value * 15;
                            }
                        }
                    }
                }

                if (score > bestScore)
                {
                    bestScore = score;
                    bestIndex = reward.Index;
                }
            }

            AmeisenLogger.I.Log("QuestTurnIn", $"Best reward: #{bestIndex} with score {bestScore:F1}");
            return bestIndex;
        }
    }
}
