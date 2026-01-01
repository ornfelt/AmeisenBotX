using AmeisenBotX.Logging;
using System;
using AmeisenBotX.Logging.Enums;
using System.Text.RegularExpressions;
using AmeisenBotX.Core.Engines.Autopilot.Services;
using AmeisenBotX.MPQ;
using AmeisenBotX.MPQ.Dbc;
using AmeisenBotX.Common.Math;
using System.Collections.Generic;
using System.Linq;
using AmeisenBotX.Wow.Objects;

namespace AmeisenBotX.Core.Engines.Autopilot.Quest
{
    public class QuestPulseEngine
    {
        private readonly AmeisenBotInterfaces Bot;
        
        // Regex patterns for parsing quest objectives
        private readonly Regex KillPattern = new(@"Slay (\d+) (.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex CollectPattern = new(@"Collect (\d+) (.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly QuestParser Parser;
        private readonly MapCoordinateService MapService;
        
        // Throttling to prevent hook pressure
        private DateTime _lastUpdate = DateTime.MinValue;
        private readonly TimeSpan _updateCooldown = TimeSpan.FromSeconds(2);
        private bool _forceUpdate;
        
        // Cache for quest target weights to improve performance
        private readonly Dictionary<ulong, float> _questTargetCache = [];
        
        public List<ParsedQuest> ActiveQuests { get; private set; } = [];
        public List<ParsedQuestObjective> ActiveObjectives => ActiveQuests.SelectMany(q => q.Objectives).ToList();

        public event Action OnQuestsUpdated;

        public QuestPulseEngine(AmeisenBotInterfaces bot, DbcBridge dbc)
        {
            Bot = bot;
            Parser = new QuestParser(bot);
            MapService = new MapCoordinateService(dbc);
            
            // Subscribe to WoW Events for real-time updates
            if (Bot.Wow.Events != null)
            {
                Bot.Wow.Events.Subscribe("QUEST_LOG_UPDATE", HandleQuestLogUpdate);
                Bot.Wow.Events.Subscribe("QUEST_WATCH_UPDATE", HandleQuestLogUpdate);
                // QUEST_ACCEPTED might be redundant if LOG_UPDATE fires, but good for safety
                Bot.Wow.Events.Subscribe("QUEST_ACCEPTED", HandleQuestLogUpdate); 
                Bot.Wow.Events.Subscribe("PLAYER_ENTERING_WORLD", HandleQuestLogUpdate); // Ensure update on login/zone change
            }
            
            AmeisenLogger.I.Log("QuestPulse", "Initialized QuestPulseEngine with POI support and Event Listeners", LogLevel.Verbose);
            
            // Initial Parse
            _forceUpdate = true;
            Update();
        }

        private void HandleQuestLogUpdate(long timestamp, List<string> args)
        {
            // Mark for update on next tick (don't update directly in event handler)
            _forceUpdate = true;
        }

        public void Update()
        {
            if (Bot.Player == null) return;
            
            // Throttle updates to prevent hook pressure
            if (!_forceUpdate && DateTime.Now - _lastUpdate < _updateCooldown) 
                return;
            
            _forceUpdate = false;
            _lastUpdate = DateTime.Now;

            ActiveQuests = Parser.ParseActiveQuests();
            UpdatePoiForObjectives();
            
            // Invalidate cache on update
            _questTargetCache.Clear();
            
            // Notify listeners (UI)
            OnQuestsUpdated?.Invoke();
        }

        private void UpdatePoiForObjectives()
        {
             int currentMapId = (int)Bot.Objects.MapId;
             int currentZoneId = Bot.Objects.ZoneId;

             foreach (var quest in ActiveQuests)
             {
                 foreach (var obj in quest.Objectives)
                 {
                     // Reset location first or keep it if it's already a valid world pos?
                     // Let's try to get a more specific POI for the objective first
                     string varName = "AB_QP_POI";
                     string script = $"local x, y = GetQuestPOILeaderBoard({quest.LogIndex}, {obj.ObjectiveIndex}); {varName} = tostring(x) .. '^' .. tostring(y)";
                     
                     if (Bot.Wow.ExecuteLuaAndRead((script, varName), out string result))
                     {
                        var parts = result.Split('^');
                        if (parts.Length >= 2 && float.TryParse(parts[0], out float x) && float.TryParse(parts[1], out float y) && (x != 0 || y != 0))
                        {
                            var worldPos = MapService.GetWorldPos(currentMapId, currentZoneId, x, y);
                            if (worldPos.HasValue)
                            {
                                obj.Location = worldPos.Value;
                                AmeisenLogger.I.Log("QuestPulse", $"POI for {quest.Title}/{obj.OriginalText}: Map({x:F3},{y:F3}) -> World({worldPos.Value})", LogLevel.Debug);
                                continue;
                            }
                        }
                     }

                     // Fallback to quest-level POI if objective-specific POI failed
                     if (quest.PoiX != 0 || quest.PoiY != 0)
                     {
                         var worldPos = MapService.GetWorldPos(currentMapId, currentZoneId, quest.PoiX, quest.PoiY);
                         if (worldPos.HasValue)
                         {
                             obj.Location = worldPos.Value;
                             AmeisenLogger.I.Log("QuestPulse", $"Fallback POI for {quest.Title}/{obj.OriginalText}: Map({quest.PoiX:F3},{quest.PoiY:F3}) -> World({worldPos.Value})", LogLevel.Debug);
                         }
                     }
                 }
             }
        }

        public IWowUnit GetBestQuestTarget()
        {
            if (ActiveObjectives.Count == 0) return null;

            var killObjectives = ActiveObjectives.Where(o => o.Type == QuestObjectiveType.Kill).ToList();
            if (killObjectives.Count == 0) return null;

            // Find nearest alive unit matching any objective
            // Using minimal distance check
            return Bot.Objects.All.OfType<IWowUnit>()
                .Where(u => !u.IsDead && u.Health > 0)
                .Where(u => IsQuestTarget(u)) // Use optimized check
                .OrderByDescending(u => GetQuestTargetWeight(u)) // Prioritize by weight (completion/multi-match)
                .ThenBy(u => u.Position.GetDistance(Bot.Player.Position)) // Then by distance
                .FirstOrDefault();
        }

        public bool IsQuestTarget(IWowUnit unit)
        {
            return GetQuestTargetWeight(unit) > 0;
        }

        public float GetQuestTargetWeight(IWowUnit unit)
        {
            if (unit == null) return 0f;
            
            if (_questTargetCache.TryGetValue(unit.Guid, out float cachedWeight))
            {
                return cachedWeight;
            }

            string unitName = unit.ReadName();
            if (string.IsNullOrEmpty(unitName))
            {
                _questTargetCache[unit.Guid] = 0f;
                return 0f;
            }

            // Find all objectives that this unit satisfies
            var matches = ActiveObjectives.Where(o => 
            {
                if (o.Type == QuestObjectiveType.Kill)
                {
                     return unitName.Contains(o.TargetName, StringComparison.OrdinalIgnoreCase);
                }
                else if (o.Type == QuestObjectiveType.Collect)
                {
                    return unitName.Contains(o.TargetName, StringComparison.OrdinalIgnoreCase) || 
                           o.TargetName.Contains(unitName, StringComparison.OrdinalIgnoreCase);
                }
                return false;
            }).ToList();

            float weight = 0f;

            if (matches.Any())
            {
                weight = 1.0f; // Base weight for being a target
                
                // Add bonus for multiple objectives (0.5 per extra objective)
                weight += (matches.Count - 1) * 0.5f;

                // Add bonus for near completion (finish up quests!)
                foreach (var match in matches)
                {
                    if (match.RequiredCount > 0)
                    {
                        float progress = (float)match.CurrentCount / match.RequiredCount;
                        if (progress >= 0.8f) weight += 0.5f; // Bonus for >80% done
                        else if (progress >= 0.5f) weight += 0.2f; // Small bonus for >50% done
                    }
                }
            }
            
            _questTargetCache[unit.Guid] = weight;
            return weight;
        }
    }
}
