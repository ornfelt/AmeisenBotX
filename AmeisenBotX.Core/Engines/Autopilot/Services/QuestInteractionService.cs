using AmeisenBotX.Core.Logic.Routines;
using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using AmeisenBotX.Logging;
using AmeisenBotX.Logging.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AmeisenBotX.Core.Engines.Autopilot.Services
{
    public enum InteractionResult
    {
        None,
        HandlingAvailable,
        HandlingActive,
        Gossip
    }

    public class QuestInteractionService
    {
        private readonly AmeisenBotInterfaces Bot;
        private readonly AmeisenBotConfig Config;

        public QuestInteractionService(AmeisenBotInterfaces bot, AmeisenBotConfig config)
        {
            Bot = bot;
            Config = config;
        }

        public string HandleGossip()
        {
            // Check for available quests first, then active quests
            string script = "local n = GetNumGossipAvailableQuests() or 0; if n > 0 then SelectGossipAvailableQuest(1) AB_GOSSIP='available' else n = GetNumGossipActiveQuests() or 0; if n > 0 then SelectGossipActiveQuest(1) AB_GOSSIP='active' else AB_GOSSIP='none' end end";
            
            if (Bot.Wow.ExecuteLuaAndRead((script, "AB_GOSSIP"), out string result))
            {
                AmeisenLogger.I.Log("Autopilot", $"Gossip handled: {result}", LogLevel.Debug);
                return result;
            }
            return "none";
        }

        public void HandleQuestAccept()
        {
            Bot.Wow.LuaDoString("AcceptQuest()");
            AmeisenLogger.I.Log("Autopilot", "Accepted quest", LogLevel.Debug);
        }

        public void HandleQuestComplete()
        {
            int numChoices = 0;
            if (!Bot.Wow.ExecuteLuaAndRead(("AB_NUMCHOICES=GetNumQuestChoices() or 0", "AB_NUMCHOICES"), out string nStr) || !int.TryParse(nStr, out numChoices) || numChoices == 0)
            {
                Bot.Wow.LuaDoString("GetQuestReward(1)");
                AmeisenLogger.I.Log("Autopilot", "Completed quest (no choices)", LogLevel.Debug);
                return;
            }

            int bestIndex = 1;
            double bestScore = -1;

            for (int i = 1; i <= numChoices; i++)
            {
                // API: name, texture, numItems, quality, isUsable = GetQuestItemInfo(type, index)
                string script = $"local n, t, c, q, u = GetQuestItemInfo('choice', {i}); " +
                               $"local link = GetQuestItemLink('choice', {i}); " +
                               $"AB_REWARD = tostring(link) .. '^' .. tostring(q or 0) .. '^' .. tostring(n or '') .. '^' .. tostring(u or false)";
                
                if (Bot.Wow.ExecuteLuaAndRead((script, "AB_REWARD"), out string result))
                {
                    var parts = result.Split('^');
                    if (parts.Length >= 4)
                    {
                        string link = parts[0];
                        int quality = int.TryParse(parts[1], out int q) ? q : 0;
                        string name = parts[2];
                        bool isUsable = parts[3].ToLower() == "true" || parts[3] == "1";

                        var mockItem = CreateMockItem(name, link, quality);

                        // Optional: Get full item info from Lua if link is valid
                        if (!string.IsNullOrEmpty(link) && link != "nil")
                        {
                            string escapedLink = link.Replace("'", "\\'");
                            string fullInfoScript = $"local n, _, q, il, rl, t, st = GetItemInfo('{escapedLink}'); " +
                                                   $"AB_INFO = tostring(il or 0) .. '^' .. tostring(t or '') .. '^' .. tostring(st or '')";
                            
                            if (Bot.Wow.ExecuteLuaAndRead((fullInfoScript, "AB_INFO"), out string infoResult))
                            {
                                var infoParts = infoResult.Split('^');
                                if (infoParts.Length >= 3)
                                {
                                    mockItem.ItemLevel = int.TryParse(infoParts[0], out int il) ? il : 0;
                                    mockItem.Type = infoParts[1];
                                    mockItem.Subtype = infoParts[2];
                                }
                            }
                        }
                        
                        double score = ItemEvaluator.CalculateEquipScore(Bot, Config, mockItem);
                        if (!isUsable) score -= 1000;

                        AmeisenLogger.I.Log("Autopilot", $"Reward {i}: {name} - Score: {score:F2} (Usable: {isUsable})", LogLevel.Debug);

                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestIndex = i;
                        }
                    }
                }
            }

            Bot.Wow.LuaDoString($"GetQuestReward({bestIndex})");
            AmeisenLogger.I.Log("Autopilot", $"Completed quest with reward choice {bestIndex} (Score: {bestScore:F2})", LogLevel.Debug);
        }

        private WowBasicItem CreateMockItem(string name, string link, int quality)
        {
            return new WowBasicItem
            {
                Name = name,
                ItemLink = link,
                ItemQuality = quality,
                Id = ParseItemIdFromLink(link)
            };
        }

        public int ParseItemIdFromLink(string link)
        {
            if (string.IsNullOrEmpty(link)) return 0;
            var match = Regex.Match(link, @"item:(\d+)");
            return match.Success && int.TryParse(match.Groups[1].Value, out int id) ? id : 0;
        }

        public InteractionResult ProcessDialogs()
        {
            // Use a function wrapper to properly return the dialog type
            string script = "AB_DIALOG=(function() " +
                           "if GossipFrame and GossipFrame:IsVisible() then return 'gossip' end " +
                           "if QuestFrameGreetingPanel and QuestFrameGreetingPanel:IsVisible() then return 'greeting' end " +
                           "if QuestFrameDetailPanel and QuestFrameDetailPanel:IsVisible() then return 'detail' end " +
                           "if QuestFrameProgressPanel and QuestFrameProgressPanel:IsVisible() then return 'progress' end " +
                           "if QuestFrameRewardPanel and QuestFrameRewardPanel:IsVisible() then return 'reward' end " +
                           "if ClassTrainerFrame and ClassTrainerFrame:IsVisible() then return 'trainer' end " +
                           "return 'none' end)()";
            
            if (Bot.Wow.ExecuteLuaAndRead((script, "AB_DIALOG"), out string frame))
            {
                switch (frame)
                {
                    case "gossip": 
                        var status = HandleGossip();
                        return status == "available" ? InteractionResult.HandlingAvailable :
                               status == "active" ? InteractionResult.HandlingActive : InteractionResult.None;
                    
                    case "detail": 
                        HandleQuestAccept(); 
                        return InteractionResult.HandlingAvailable;

                    case "progress": 
                        Bot.Wow.LuaDoString("CompleteQuest()"); 
                        return InteractionResult.HandlingActive;

                    case "reward": 
                        HandleQuestComplete(); 
                        return InteractionResult.HandlingActive;

                    case "greeting": 
                        string gScript = "local n = GetNumAvailableQuests() or 0; if n > 0 then SelectAvailableQuest(1) AB_GREET='available' else n = GetNumActiveQuests() or 0; if n > 0 then SelectActiveQuest(1) AB_GREET='active' else AB_GREET='none' end end";
                        if (Bot.Wow.ExecuteLuaAndRead((gScript, "AB_GREET"), out string gResult))
                        {
                             return gResult == "available" ? InteractionResult.HandlingAvailable :
                                    gResult == "active" ? InteractionResult.HandlingActive : InteractionResult.None;
                        }
                        return InteractionResult.None;

                    case "trainer":
                        Bot.Wow.LuaDoString("CloseTrainer()");
                        return InteractionResult.None; // Triggers blacklist
                }
            }
            return InteractionResult.None;
        }
    }
}
