using AmeisenBotX.Common.Utils;
using AmeisenBotX.Logging;
using AmeisenBotX.Logging.Enums;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AmeisenBotX.Core.Engines.Autopilot.Quest
{
    public class QuestParser
    {
        private readonly AmeisenBotInterfaces Bot;
        
        // Regex Patterns
        private static readonly Regex KillPattern = new(@"Slay (\d+) (.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex CollectPattern = new(@"Collect (\d+) (.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex ProgressPattern = new(@"(\d+)\s*/\s*(\d+)", RegexOptions.Compiled);

        public QuestParser(AmeisenBotInterfaces bot)
        {
            Bot = bot;
        }

        private List<string> GetLuaList(string code)
        {
            string varName = "AB_QP_Res";
            // Lua script: Capture return values into a table, convert all to string, concat with separator
            string script = $"local t = {{ {code} }}; for i=1, #t do t[i] = tostring(t[i]) end; {varName} = table.concat(t, '^')";
            
            if (Bot.Wow.ExecuteLuaAndRead((script, varName), out string result))
            {
                if (string.IsNullOrEmpty(result)) return new List<string>();
                return new List<string>(result.Split('^'));
            }
            return null;
        }

        public List<ParsedQuest> ParseActiveQuests()
        {
            var quests = new List<ParsedQuest>();

            // Optimized Lua script with obfuscation
            // {v:0} = result JSON, {v:1} = escape func, {v:2} = poi func, {v:3} = parse func
            // {v:4} = table, {v:5} = numEntries
            // POI: QuestPOIGetIconInfo(questId) returns completed, posX, posY, objective
            string script = "{v:0}=\"[\" " +
                "local function {v:1}(s) if not s then return\"\"end;s=s:gsub(\"\\\\\",\"\\\\\\\\\"):gsub(\"\\\"\",\"\\\\\\\"\"):gsub(\"\\n\",\"\\\\n\"):gsub(\"\\r\",\"\")return s end " +
                "local function {v:2}(q) if not q then return 0,0 end SetMapToCurrentZone()QuestPOIUpdateIcons()if QuestPOIGetIconInfo then local _,x,y=QuestPOIGetIconInfo(q)if x and y then return x,y end end return 0,0 end " +
                "local function {v:3}(t) if not t then return 0,0 end local c,r=t:match(\"(%d+)%s*/%s*(%d+)\")return tonumber(c)or 0,tonumber(r)or 0 end " +
                "local {v:4}={}local {v:5},_=GetNumQuestLogEntries()" +
                "for i=1,{v:5} do " +
                    "local t,l,g,sg,h,_,ic,d,q=GetQuestLogTitle(i)" +
                    "if not h and t then " +
                        "SelectQuestLogEntry(i)" +
                        "local ds,ot=GetQuestLogQuestText()" +
                        "local px,py={v:2}(q)" +
                        "local m=GetQuestLogRewardMoney()or 0 " +
                        "local obj={}local no=GetNumQuestLeaderBoards(i)or 0 " +
                        "for o=1,no do local tx,ty,f=GetQuestLogLeaderBoard(o,i)if tx then local c,r={v:3}(tx)table.insert(obj,'{\"Text\":\"'..{v:1}(tx)..'\",\"Type\":\"'..{v:1}(ty or\"unknown\")..'\",\"Current\":'..c..',\"Required\":'..r..',\"Finished\":'..(f and\"true\"or\"false\")..'}')end end " +
                        "local cr={}local nc=GetNumQuestLogChoices()or 0 " +
                        "for c=1,nc do local lk=GetQuestLogItemLink('choice',c)local n,tx,ni,qu,u=GetQuestLogChoiceInfo(c)if n then local il,rl,it,st,el=0,0,\"\",\"\",\"\"if lk then local _,_,_,a,b,c,d,_,e=GetItemInfo(lk)il=a or 0 rl=b or 0 it=c or\"\"st=d or\"\"el=e or\"\"end table.insert(cr,'{\"Index\":'..c..',\"Name\":\"'..{v:1}(n)..'\",\"Link\":\"'..{v:1}(lk)..'\",\"Texture\":\"'..{v:1}(tx)..'\",\"Count\":'..(ni or 1)..',\"Quality\":'..(qu or 0)..',\"Usable\":'..(u and\"true\"or\"false\")..',\"ItemLevel\":'..il..',\"ReqLevel\":'..rl..',\"ItemType\":\"'..{v:1}(it)..'\",\"SubType\":\"'..{v:1}(st)..'\",\"EquipLoc\":\"'..{v:1}(el)..'\"}')end end " +
                        "local fr={}local nf=GetNumQuestLogRewards()or 0 " +
                        "for f=1,nf do local n,tx,ni,qu,u=GetQuestLogRewardInfo(f)local lk=GetQuestLogItemLink('reward',f)if n then local il,rl,it,st,el=0,0,\"\",\"\",\"\"if lk then local _,_,_,a,b,c,d,_,e=GetItemInfo(lk)il=a or 0 rl=b or 0 it=c or\"\"st=d or\"\"el=e or\"\"end table.insert(fr,'{\"Index\":'..f..',\"Name\":\"'..{v:1}(n)..'\",\"Link\":\"'..{v:1}(lk)..'\",\"Texture\":\"'..{v:1}(tx)..'\",\"Count\":'..(ni or 1)..',\"Quality\":'..(qu or 0)..',\"Usable\":'..(u and\"true\"or\"false\")..',\"ItemLevel\":'..il..',\"ReqLevel\":'..rl..',\"ItemType\":\"'..{v:1}(it)..'\",\"SubType\":\"'..{v:1}(st)..'\",\"EquipLoc\":\"'..{v:1}(el)..'\"}')end end " +
                        "local icb=(ic==1 or ic==true)and\"true\"or\"false\" " +
                        "table.insert({v:4},'{\"Title\":\"'..{v:1}(t)..'\",\"Id\":'..(q or 0)..',\"Level\":'..(l or 0)..',\"LogIndex\":'..i..',\"IsComplete\":'..icb..',\"SuggestedGroup\":'..(sg or 0)..',\"IsDaily\":'..(d and\"true\"or\"false\")..',\"Tag\":\"'..{v:1}(g or\"\")..'\",\"Description\":\"'..{v:1}(ds)..'\",\"ObjectiveText\":\"'..{v:1}(ot)..'\",\"PoiX\":'..(px or 0)..',\"PoiY\":'..(py or 0)..',\"MoneyReward\":'..m..',\"Objectives\":['..table.concat(obj,\",\")..'],\"ChoiceRewards\":['..table.concat(cr,\",\")..'],\"FixedRewards\":['..table.concat(fr,\",\")..']}') " +
                    "end " +
                "end " +
                "{v:0}={v:0}..table.concat({v:4},\",\")..\"]\"";

            try 
            {
                if (Bot.Wow.ExecuteLuaAndRead(BotUtils.ObfuscateLua(script), out string jsonResult))
                {
                    if (string.IsNullOrWhiteSpace(jsonResult) || jsonResult == "[]" || jsonResult == "nil")
                        return quests;

                    var rawQuests = System.Text.Json.JsonSerializer.Deserialize<List<RawQuestData>>(jsonResult);

                    if (rawQuests != null)
                    {
                        foreach (var raw in rawQuests)
                        {
                            var quest = new ParsedQuest
                            {
                                Title = raw.Title,
                                Id = raw.Id,
                                Level = raw.Level,
                                LogIndex = raw.LogIndex,
                                IsComplete = raw.IsComplete,
                                PoiX = raw.PoiX,
                                PoiY = raw.PoiY
                            };

                            // Objectives - now use pre-parsed progress from Lua
                            if (raw.Objectives != null && raw.Objectives.Count > 0)
                            {
                                int oIndex = 1;
                                foreach (var rawObj in raw.Objectives)
                                {
                                    if (rawObj.Finished) continue;

                                    var obj = new ParsedQuestObjective
                                    {
                                        QuestTitle = quest.Title,
                                        QuestId = quest.Id,
                                        LogIndex = quest.LogIndex,
                                        ObjectiveIndex = oIndex++,
                                        OriginalText = rawObj.Text,
                                        CurrentCount = rawObj.Current,
                                        RequiredCount = rawObj.Required,
                                        Type = MapObjectiveType(rawObj.Type)
                                    };
                                    
                                    // Extract target name from text
                                    obj.TargetName = ExtractTargetName(rawObj.Text, obj.CurrentCount, obj.RequiredCount);
                                    
                                    quest.Objectives.Add(obj);
                                }
                            }

                            // Fallback for 0-objective quests (Talk-To)
                            if (quest.Objectives.Count == 0 && !string.IsNullOrWhiteSpace(raw.ObjectiveText))
                            {
                                var fallback = new ParsedQuestObjective
                                {
                                    QuestTitle = quest.Title,
                                    QuestId = quest.Id,
                                    LogIndex = quest.LogIndex,
                                    ObjectiveIndex = 0,
                                    OriginalText = raw.ObjectiveText,
                                    TargetName = raw.ObjectiveText?.TrimEnd('.', '!', '?', ',', ';', ':'),
                                    Type = QuestObjectiveType.TalkTo,
                                    CurrentCount = 0,
                                    RequiredCount = 1
                                };
                                
                                var match = Regex.Match(raw.ObjectiveText, @"(?:Speak|Talk) (?:to|with) (.+?)(?: at | in |$)", RegexOptions.IgnoreCase);
                                if (match.Success)
                                {
                                    fallback.TargetName = match.Groups[1].Value.Trim().TrimEnd('.', '!', '?', ',', ';', ':');
                                }
                                
                                quest.Objectives.Add(fallback);
                            }

                            // Process Choice Rewards
                            ProcessRewards(raw.ChoiceRewards, quest.Rewards, isChoice: true);
                            
                            // Process Fixed Rewards (always given)
                            ProcessRewards(raw.FixedRewards, quest.Rewards, isChoice: false);
                            
                            // Sort all rewards by score
                            quest.Rewards.Sort((a, b) => b.Score.CompareTo(a.Score));

                            quests.Add(quest);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                 AmeisenLogger.I.Log("QuestParser", $"JSON Parse Error: {ex.Message}", LogLevel.Error);
            }

            return quests;
        }

        private QuestObjectiveType MapObjectiveType(string luaType)
        {
            if (string.IsNullOrEmpty(luaType)) return QuestObjectiveType.Unknown;
            
            return luaType.ToLowerInvariant() switch
            {
                "monster" => QuestObjectiveType.Kill,
                "item" => QuestObjectiveType.Collect,
                "object" => QuestObjectiveType.Interact,
                "event" => QuestObjectiveType.Event,
                "player" => QuestObjectiveType.Kill,
                _ => QuestObjectiveType.Unknown
            };
        }

        private string ExtractTargetName(string text, int current, int required)
        {
            if (string.IsNullOrEmpty(text)) return "";
            
            // Remove progress pattern like "3/5" or ": 3/5"
            string pattern = $@":?\s*{current}\s*/\s*{required}";
            string cleanName = Regex.Replace(text, pattern, "").Trim();
            cleanName = cleanName.Trim(':', ' ');
            
            // Also handle "slain" suffix
            cleanName = Regex.Replace(cleanName, @"\s+slain$", "", RegexOptions.IgnoreCase);
            
            // Remove trailing punctuation (common in quest objectives like "Talk to Grommash.")
            cleanName = cleanName.TrimEnd('.', '!', '?', ',', ';', ':');
            
            return cleanName;
        }

        private void ProcessRewards(List<RawReward> rawRewards, List<ParsedQuestReward> targetList, bool isChoice)
        {
            if (rawRewards == null) return;
            
            foreach (var r in rawRewards)
            {
                var reward = new ParsedQuestReward
                {
                    ChoiceIndex = isChoice ? r.Index : 0, // 0 for fixed rewards
                    ItemLink = r.Link,
                    Name = r.Name,
                    Texture = r.Texture,
                    IsUsable = r.Usable,
                    ItemLevel = r.ItemLevel,
                    RequiredLevel = r.ReqLevel,
                    Type = r.ItemType,
                    SubType = r.SubType,
                    EquipLoc = r.EquipLoc
                };
                
                var dummyItem = new AmeisenBotX.Core.Managers.Character.Inventory.Objects.WowBasicItem
                {
                    Name = reward.Name,
                    ItemLink = reward.ItemLink,
                    ItemLevel = reward.ItemLevel,
                    ItemQuality = r.Quality,
                    RequiredLevel = reward.RequiredLevel,
                    Type = reward.Type,
                    Subtype = reward.SubType,
                    EquipLocation = reward.EquipLoc
                };
                 
                if (Bot.Player != null)
                {
                    reward.Score = (float)AmeisenBotX.Core.Logic.Routines.ItemEvaluator.CalculateEquipScore(Bot, null, dummyItem);
                }
                
                targetList.Add(reward);
            }
        }

        // Helper Classes for JSON Deserialization
        private class RawQuestData
        {
            public string Title { get; set; }
            public int Id { get; set; }
            public int Level { get; set; }
            public int LogIndex { get; set; }
            public bool IsComplete { get; set; }
            public int SuggestedGroup { get; set; }
            public bool IsDaily { get; set; }
            public string Tag { get; set; } // "Group", "Dungeon", "PvP", etc.
            public string Description { get; set; }
            public string ObjectiveText { get; set; }
            public float PoiX { get; set; }
            public float PoiY { get; set; }
            public int MoneyReward { get; set; } // Copper
            public List<RawObjective> Objectives { get; set; }
            public List<RawReward> ChoiceRewards { get; set; }
            public List<RawReward> FixedRewards { get; set; }
        }

        private class RawObjective
        {
            public string Text { get; set; }
            public string Type { get; set; } // "monster", "item", "event", "object", etc.
            public int Current { get; set; }
            public int Required { get; set; }
            public bool Finished { get; set; }
        }

        private class RawReward
        {
            public int Index { get; set; }
            public string Name { get; set; }
            public string Link { get; set; }
            public string Texture { get; set; }
            public int Count { get; set; }
            public int Quality { get; set; } // 0=Poor, 1=Common, 2=Uncommon, 3=Rare, 4=Epic, 5=Legendary
            public bool Usable { get; set; }
            public int ItemLevel { get; set; }
            public int ReqLevel { get; set; }
            public string ItemType { get; set; }
            public string SubType { get; set; }
            public string EquipLoc { get; set; }
        }
    }

    public class ParsedQuestObjective
    {
        public string QuestTitle { get; set; }
        public int QuestId { get; set; }
        public int LogIndex { get; set; }
        public int ObjectiveIndex { get; set; }
        public string TargetName { get; set; }
        public int CurrentCount { get; set; }
        public int RequiredCount { get; set; }
        public QuestObjectiveType Type { get; set; }
        public string OriginalText { get; set; }
        public AmeisenBotX.Common.Math.Vector3 Location { get; set; }
    }

    public enum QuestObjectiveType
    {
        Unknown,
        Kill,       // Slay mobs
        Collect,    // Loot items
        Interact,   // Click gameobjects
        Event,      // Speak/Explore
        TalkTo      // Specific Talk-To/Deliver quests
    }
}
