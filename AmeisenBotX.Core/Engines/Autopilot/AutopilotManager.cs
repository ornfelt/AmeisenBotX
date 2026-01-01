using AmeisenBotX.BehaviorTree;
using AmeisenBotX.BehaviorTree.Enums;
using AmeisenBotX.BehaviorTree.Objects;
using AmeisenBotX.Core.Engines.Autopilot.Objects;
using AmeisenBotX.Core.Engines.Autopilot.Quest;
using AmeisenBotX.Core.Engines.Autopilot.Services;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Flags;
using AmeisenBotX.Common.Math;
using System.Linq;
using System.Collections.Generic;
using AmeisenBotX.Logging.Enums;
using AmeisenBotX.Logging;
using System;

namespace AmeisenBotX.Core.Engines.Autopilot
{
    public class AutopilotManager
    {
        private readonly AmeisenBotInterfaces Bot;
        private readonly AmeisenBotConfig Config;
        private readonly QuestPulseEngine QuestPulse;
        private readonly ProgressionState State;
        private readonly QuestDiscoveryService Discovery;
        private readonly QuestInteractionService Interactor;
        private readonly QuestBatcher Batcher;
        private readonly Tree MainTree;

        // Cooldown Tracking
        private DateTime _lastInteraction = DateTime.MinValue;
        private readonly TimeSpan _interactionCooldown = TimeSpan.FromSeconds(1.5);
        private ulong _lastInteractedGuid;
        
        // Combat Target Stickiness - prevents target thrashing during kill objectives
        private ulong _engagedQuestTargetGuid;
        private string _engagedQuestTargetName;
        
        // Opportunistic Pickup Stickiness
        private ulong _opportunisticTargetGuid;
        private DateTime _targetDeathTime = DateTime.MinValue;
        private DateTime _targetMissingTime = DateTime.MinValue;
        
        // Cache for Item -> Valid Unit Names (from Tooltip)
        // Item Name -> Set of Unit Names
        private readonly Dictionary<string, HashSet<string>> _questItemDroppers = [];
        private DateTime _lastTooltipScan = DateTime.MinValue;

        // Ignore list for unproductive quest givers
        private readonly Dictionary<ulong, DateTime> _ignoredQuestGivers = [];
        private readonly TimeSpan _ignoreDuration = TimeSpan.FromMinutes(5);

        // Separate ignore list for Turn-In attempts (NPCs who only have Available quests, not Active)
        private readonly Dictionary<ulong, DateTime> _ignoredForTurnIn = [];
        private readonly TimeSpan _turnInIgnoreDuration = TimeSpan.FromSeconds(60);

        private bool CanInteract(ulong guid) => 
            DateTime.Now - _lastInteraction > _interactionCooldown || _lastInteractedGuid != guid;

        private bool IsIgnored(ulong guid)
        {
            if (_ignoredQuestGivers.TryGetValue(guid, out DateTime expiry))
            {
                if (DateTime.Now < expiry) return true;
                _ignoredQuestGivers.Remove(guid); // Expired
            }
            return false;
        }

        private void IgnoreQuestGiver(ulong guid)
        {
            _ignoredQuestGivers[guid] = DateTime.Now + _ignoreDuration;
            AmeisenLogger.I.Log("Autopilot", $"Ignoring unproductive quest giver {guid} for 5 minutes", LogLevel.Debug);
        }

        private bool IsIgnoredForTurnIn(ulong guid)
        {
            if (_ignoredForTurnIn.TryGetValue(guid, out DateTime expiry))
            {
                if (DateTime.Now < expiry) return true;
                _ignoredForTurnIn.Remove(guid);
            }
            return false;
        }

        private void IgnoreForTurnIn(ulong guid)
        {
            _ignoredForTurnIn[guid] = DateTime.Now + _turnInIgnoreDuration;
            AmeisenLogger.I.Log("Autopilot", $"Ignoring NPC {guid} for turn-in attempts for 60 seconds", LogLevel.Debug);
        }
        
        private void MarkInteraction(ulong guid)
        {
            _lastInteraction = DateTime.Now;
            _lastInteractedGuid = guid;
        }

        public AutopilotManager(AmeisenBotInterfaces bot, AmeisenBotConfig config, QuestPulseEngine questPulse)
        {
            Bot = bot;
            Config = config;
            QuestPulse = questPulse;
            State = new ProgressionState(bot);
            Discovery = new QuestDiscoveryService(bot, questPulse);
            Interactor = new QuestInteractionService(bot, config);
            Batcher = new QuestBatcher();

            MainTree = new Tree(BuildAutopilotTree(), true);
        }

        public string CurrentTaskName => MainTree?.LastExecutedNode?.Name ?? "Idle";
        public string DetailedStatus { get; private set; } = "Initializing...";

        public void UpdateDecision()
        {
            // Placeholder for future logic overrides
        }

        public void ExecuteCurrentTask()
        {
            MainTree.Tick();
        }

        private INode BuildAutopilotTree()
        {
            return new Waterfall(
                // Fallback (Idle)
                new Leaf(() => { 
                    AmeisenLogger.I.Log("Autopilot", $"IDLE - Quests: {QuestPulse.ActiveQuests.Count}, Objectives: {QuestPulse.ActiveObjectives.Count}", LogLevel.Debug);
                    DetailedStatus = "Idle / No Tasks";
                    return BtStatus.Success; 
                }, "Idle"),

                // Priority 1: Maintenance
                (() => false, new Leaf(() => BtStatus.Failed, "Placeholder Maintenance")),

                // Priority 1: Combat Focus (Restored & Simplified)
                // If we have a target or are in combat, ensure we are moving to engage.
                // The Combat Engine (SimpleDpsTargetSelectionLogic) will ensure we STICK to the target.
                (() => _engagedQuestTargetGuid != 0 || Bot.Player.IsInCombat,
                    new Leaf(() => {
                        // 1. In Combat? Let Combat Engine handle it.
                        if (Bot.Player.IsInCombat)
                        {
                            DetailedStatus = "Combat";
                            // Ensure we don't just stand still if target is far, but CR usually handles this.
                            // If we want to be safe, we can Chase.
                            var target = Bot.Objects.All.OfType<IWowUnit>().FirstOrDefault(u => u.Guid == Bot.Player.TargetGuid);
                            if (target != null && target.DistanceTo(Bot.Player) > GetOptimalCombatRange())
                            {
                                Bot.Movement.SetMovementAction(AmeisenBotX.Core.Engines.Movement.Enums.MovementAction.Move, target.Position);
                            }
                            return BtStatus.Success;
                        }

                            // 2. Not in combat, but Locked? Approach.
                        if (_engagedQuestTargetGuid != 0)
                        {
                            var target = Bot.Objects.All.OfType<IWowUnit>().FirstOrDefault(u => u.Guid == _engagedQuestTargetGuid);
                            if (target == null)
                            {
                                // Target vanished? Release lock and fail (go back to questing)
                                _engagedQuestTargetGuid = 0;
                                return BtStatus.Failed;
                            }

                            if (target.IsDead)
                            {
                                // Dead? If Looting logic isn't here, we might get stuck.
                                // We removed the Looting Node too (it was part of Lifecycle).
                                // We need to restore basic Looting or add it here.
                                // Let's add simple loot logic here to be safe.
                                if (target.IsLootable)
                                {
                                     DetailedStatus = "Looting";
                                     if (target.DistanceTo(Bot.Player) > 4f)
                                     {
                                         Bot.Movement.SetMovementAction(AmeisenBotX.Core.Engines.Movement.Enums.MovementAction.Move, target.Position);
                                         return BtStatus.Success;
                                     }
                                     Bot.Movement.StopMovement();
                                     Bot.Wow.InteractWithUnit(target);
                                     MarkInteraction(target.Guid);
                                     return BtStatus.Success;
                                }
                                
                                // Not lootable yet? Wait or release?
                                // If we just killed it, we should wait. 
                                // But relying on simple logic: Release lock. 
                                // If it becomes lootable, `LootManager` (if exists) or Quest Logic should handle it?
                                // Actually, `Looting` used to be Priority 2. I should probably restore a Looting Node.
                                _engagedQuestTargetGuid = 0;
                                return BtStatus.Failed; 
                            }

                            // Alive: Approach
                            DetailedStatus = $"Approaching {target.ReadName()}";
                            Bot.Wow.ChangeTarget(target.Guid); // Ensure game target matches
                            if (target.DistanceTo(Bot.Player) > GetOptimalCombatRange())
                            {
                                Bot.Movement.SetMovementAction(AmeisenBotX.Core.Engines.Movement.Enums.MovementAction.Move, target.Position);
                            }
                            else
                            {
                                Bot.Movement.StopMovement();
                            }
                            return BtStatus.Success;
                        }

                        return BtStatus.Failed;
                    }, "Combat Focus")),

                // Priority 3: Resting
                (() => !Bot.Player.IsInCombat && (Bot.Player.HealthPercentage < 50 || (Bot.Player.MaxMana > 0 && Bot.Player.ManaPercentage < 30)),
                    new Leaf(() => {
                        DetailedStatus = $"Resting (HP: {Bot.Player.HealthPercentage:F0}% Mana: {Bot.Player.ManaPercentage:F0}%)";
                        Bot.Movement.StopMovement();
                        return BtStatus.Success;
                    }, "Resting")),

                // Priority 4: Leveling
                (() => State.Level <= 80, 
                     new Waterfall(
                        // Internal Fallback used to be Failed. Changed to Idle to show status correctly.
                        new Leaf(() => {
                            AmeisenLogger.I.Log("Autopilot", $"Leveling Idle - Quests: {QuestPulse.ActiveQuests.Count}, Complete: {QuestPulse.ActiveQuests.Count(q => q.IsComplete)}, NearbyNPCs: {Discovery.GetNearbyQuestGivers().Count()}", LogLevel.Debug);
                            DetailedStatus = "Idle (Leveling)";
                            return BtStatus.Success;
                        }, "Leveling Idle Fallback"),
                        
                        // 4.1: Turn-in completed quests
                        (() => QuestPulse.ActiveQuests.Any(q => q.IsComplete), 
                            new Leaf(() => {
                                // Filter to NPCs not already ignored for turn-in
                                var nearbyNPCs = Discovery.GetNearbyQuestGivers()
                                    .Where(u => !IsIgnoredForTurnIn(u.Guid))
                                    .ToList();
                                AmeisenLogger.I.Log("Autopilot", $"Turn-In: Found {nearbyNPCs.Count} nearby quest givers (not ignored)", LogLevel.Debug);
                                
                                var turnInNpc = nearbyNPCs.FirstOrDefault(); 
                                if (turnInNpc == null)
                                {
                                    DetailedStatus = "Looking for turn-in NPC...";
                                    return BtStatus.Failed;
                                }

                                if (turnInNpc.Position.GetDistance(Bot.Player.Position) > 4f)
                                {
                                    DetailedStatus = $"Moving to {turnInNpc.ReadName()} for turn-in";
                                    Bot.Movement.SetMovementAction(AmeisenBotX.Core.Engines.Movement.Enums.MovementAction.Move, turnInNpc.Position);
                                    return BtStatus.Success;
                                }

                                if (!CanInteract(turnInNpc.Guid)) return BtStatus.Success; 
                                
                                Bot.Movement.StopMovement();
                                Bot.Wow.InteractWithUnit(turnInNpc);
                                MarkInteraction(turnInNpc.Guid);
                                
                                var result = Interactor.ProcessDialogs();
                                
                                // If the NPC only had Available quests (not Active turn-ins), ignore it for turn-in
                                if (result == InteractionResult.HandlingAvailable || result == InteractionResult.None)
                                {
                                    IgnoreForTurnIn(turnInNpc.Guid);
                                    AmeisenLogger.I.Log("Autopilot", $"NPC {turnInNpc.ReadName()} has no turn-in, ignoring for 60s", LogLevel.Debug);
                                }
                                
                                return BtStatus.Success;
                            }, "Turn-In Quests")),

                        // 4.2: Opportunistic Quest Pickup
                        (() => !Bot.Player.IsInCombat && QuestPulse.ActiveQuests.Count < 20 &&
                               Discovery.GetNearbyQuestGivers().Any(u => !IsIgnored(u.Guid) && u.Position.GetDistance(Bot.Player.Position) <= 40f),
                            new Leaf(() => {
                                IWowUnit nearbyQuestGiver = null;
                                if (_opportunisticTargetGuid != 0)
                                {
                                    nearbyQuestGiver = Discovery.GetNearbyQuestGivers().FirstOrDefault(u => u.Guid == _opportunisticTargetGuid);
                                    if (nearbyQuestGiver == null || IsIgnored(nearbyQuestGiver.Guid) || nearbyQuestGiver.Position.GetDistance(Bot.Player.Position) > 45f)
                                    {
                                        _opportunisticTargetGuid = 0;
                                        nearbyQuestGiver = null;
                                    }
                                }

                                if (nearbyQuestGiver == null)
                                {
                                    var candidates = Discovery.GetNearbyQuestGivers()
                                        .Where(u => !IsIgnored(u.Guid) && u.Position.GetDistance(Bot.Player.Position) <= 40f)
                                        .ToList();

                                    nearbyQuestGiver = candidates
                                        .Select(u => new { Unit = u, PathDist = GetPathDistance(u.Position) })
                                        .Where(x => x.PathDist < 1000f)
                                        .OrderBy(x => x.PathDist)
                                        .FirstOrDefault()?.Unit;
                                    
                                    if (nearbyQuestGiver != null) _opportunisticTargetGuid = nearbyQuestGiver.Guid;
                                }
                                    
                                if (nearbyQuestGiver == null) return BtStatus.Failed;
                                
                                if (nearbyQuestGiver.Position.GetDistance(Bot.Player.Position) > 4f)
                                {
                                    DetailedStatus = $"Moving to opportunistic quest giver: {nearbyQuestGiver.ReadName()}";
                                    Bot.Movement.SetMovementAction(AmeisenBotX.Core.Engines.Movement.Enums.MovementAction.Move, nearbyQuestGiver.Position);
                                    return BtStatus.Success;
                                }

                                if (!CanInteract(nearbyQuestGiver.Guid)) return BtStatus.Success;
                                
                                Bot.Movement.StopMovement();
                                Bot.Wow.InteractWithUnit(nearbyQuestGiver);
                                MarkInteraction(nearbyQuestGiver.Guid);
                                
                                var result = Interactor.ProcessDialogs();
                                
                                if (result == InteractionResult.None || result == InteractionResult.HandlingActive)
                                {
                                    IgnoreQuestGiver(nearbyQuestGiver.Guid);
                                    _opportunisticTargetGuid = 0;
                                }
                                else if (result == InteractionResult.HandlingAvailable)
                                {
                                     _opportunisticTargetGuid = 0;
                                }
                                
                                return BtStatus.Success;
                            }, "Opportunistic Quest Pickup")),

                        // 4.3: Execute current batch
                        (() => QuestPulse.ActiveObjectives.Any(),
                             new Leaf(() => {
                                 var batches = Batcher.BatchObjectives(QuestPulse.ActiveObjectives);
                                 Batcher.CalculateBatchScores(batches, Bot.Player, QuestPulse.ActiveQuests);
                                 var batch = batches.OrderByDescending(b => b.Score).FirstOrDefault();

                                 if (batch == null) return BtStatus.Failed;

                                 var nearestObj = batch.Objectives.OrderBy(o => o.Location.GetDistance(Bot.Player.Position)).FirstOrDefault();
                                 if (nearestObj == null) return BtStatus.Failed;

                                 var targetPos = GetNavigablePosition(nearestObj.Location);

                                 if (targetPos != AmeisenBotX.Common.Math.Vector3.Zero && targetPos.GetDistance2D(Bot.Player.Position) > 30f)
                                 {
                                     DetailedStatus = $"Moving to Quest Area: {nearestObj.QuestTitle}";
                                     Bot.Movement.SetMovementAction(AmeisenBotX.Core.Engines.Movement.Enums.MovementAction.Move, targetPos);
                                     return BtStatus.Success;
                                 }

                                 switch (nearestObj.Type)
                                 {
                                     case QuestObjectiveType.Kill:
                                     case QuestObjectiveType.Collect:
                                         IWowUnit currentTarget = null;
                                         if (_engagedQuestTargetGuid != 0)
                                         {
                                             currentTarget = Bot.Objects.All.OfType<IWowUnit>()
                                                 .FirstOrDefault(u => u.Guid == _engagedQuestTargetGuid && !u.IsDead);
                                         }

                                         if (currentTarget == null && nearestObj.Type == QuestObjectiveType.Collect)
                                         {
                                             currentTarget = IdentifyQuestTarget(nearestObj.TargetName);
                                             
                                             if (currentTarget != null)
                                             {
                                                 _engagedQuestTargetGuid = currentTarget.Guid;
                                                 _engagedQuestTargetName = nearestObj.TargetName;
                                             }
                                         }
                                         
                                         if (currentTarget == null)
                                         {
                                             currentTarget = Bot.Objects.All.OfType<IWowUnit>()
                                                 .Where(u => !u.IsDead && u.ReadName().Contains(nearestObj.TargetName, StringComparison.OrdinalIgnoreCase))
                                                 .OrderBy(u => u.Position.GetDistance(Bot.Player.Position))
                                                 .FirstOrDefault();
                                             
                                             if (currentTarget != null)
                                             {
                                                 _engagedQuestTargetGuid = currentTarget.Guid;
                                                 _engagedQuestTargetName = nearestObj.TargetName;
                                             }
                                         }

                                         if (currentTarget == null && nearestObj.Type == QuestObjectiveType.Collect)
                                         {
                                             var collectGo = Bot.Objects.All.OfType<IWowGameobject>()
                                                 .Where(g => g.Name.Contains(nearestObj.TargetName, StringComparison.OrdinalIgnoreCase))
                                                 .OrderBy(g => g.Position.GetDistance(Bot.Player.Position))
                                                 .FirstOrDefault();
                                                 
                                             if (collectGo != null)
                                             {
                                                 DetailedStatus = $"Collecting: {collectGo.Name}";
                                                 if (collectGo.Position.GetDistance(Bot.Player.Position) > 5f)
                                                 {
                                                     Bot.Movement.SetMovementAction(AmeisenBotX.Core.Engines.Movement.Enums.MovementAction.Move, collectGo.Position);
                                                 }
                                                 else if (CanInteract(collectGo.Guid))
                                                 {
                                                     Bot.Movement.StopMovement();
                                                     Bot.Wow.InteractWithObject(collectGo);
                                                     MarkInteraction(collectGo.Guid);
                                                 }
                                                 return BtStatus.Success;
                                             }
                                         }

                                          if (currentTarget != null)
                                          {
                                              // Hand off to Engagement Lifecycle
                                              _engagedQuestTargetGuid = currentTarget.Guid;
                                              _engagedQuestTargetName = nearestObj.TargetName;
                                              DetailedStatus = $"Target Selected: {currentTarget.ReadName()}";
                                              return BtStatus.Success;
                                          }
                                          else
                                          {
                                              _engagedQuestTargetGuid = 0;
                                              _engagedQuestTargetName = null;
                                              DetailedStatus = "Searching for targets...";
                                          }
                                         break;

                                     case QuestObjectiveType.Interact:
                                         var gameObject = Bot.Objects.All.OfType<IWowGameobject>()
                                             .Where(g => g.Name.Contains(nearestObj.TargetName, StringComparison.OrdinalIgnoreCase))
                                             .OrderBy(g => g.Position.GetDistance(Bot.Player.Position))
                                             .FirstOrDefault();

                                         if (gameObject != null)
                                         {
                                             DetailedStatus = $"Interacting w/: {gameObject.Name}";
                                             if (gameObject.Position.GetDistance(Bot.Player.Position) > 5f)
                                             {
                                                 Bot.Movement.SetMovementAction(AmeisenBotX.Core.Engines.Movement.Enums.MovementAction.Move, gameObject.Position);
                                             }
                                             else if (CanInteract(gameObject.Guid))
                                             {
                                                 Bot.Movement.StopMovement();
                                                 Bot.Wow.InteractWithObject(gameObject);
                                                 MarkInteraction(gameObject.Guid);
                                             }
                                             return BtStatus.Success;
                                         }
                                         break;

                                     case QuestObjectiveType.TalkTo:
                                     case QuestObjectiveType.Event:
                                         var npc = Bot.Objects.All.OfType<IWowUnit>()
                                             .Where(u => !u.IsDead && u.ReadName().Contains(nearestObj.TargetName, StringComparison.OrdinalIgnoreCase))
                                             .OrderBy(u => u.Position.GetDistance(Bot.Player.Position))
                                             .FirstOrDefault();

                                         if (npc == null)
                                         {
                                             npc = Bot.Objects.All.OfType<IWowUnit>()
                                                 .Where(u => !u.IsDead && u.Position.GetDistance(Bot.Player.Position) <= 10f && u.Type != WowObjectType.Player)
                                                 .OrderBy(u => u.Position.GetDistance(Bot.Player.Position))
                                                 .FirstOrDefault();
                                         }

                                         if (npc != null)
                                         {
                                             float dist = npc.Position.GetDistance(Bot.Player.Position);
                                             DetailedStatus = $"Talking to: {npc.ReadName()}";
                                             
                                             if (dist > 4f)
                                             {
                                                 Bot.Movement.SetMovementAction(AmeisenBotX.Core.Engines.Movement.Enums.MovementAction.Move, npc.Position);
                                             }
                                             else if (CanInteract(npc.Guid))
                                             {
                                                 Bot.Movement.StopMovement();
                                                 Bot.Wow.InteractWithUnit(npc);
                                                 MarkInteraction(npc.Guid);
                                                 Interactor.ProcessDialogs();
                                             }
                                             return BtStatus.Success;
                                         }
                                         break;
                                 }

                                 if (nearestObj.Location != AmeisenBotX.Common.Math.Vector3.Zero)
                                 {
                                     var fallbackTarget = GetNavigablePosition(nearestObj.Location);
                                     DetailedStatus = $"Moving to POI (No targets found)";
                                     Bot.Movement.SetMovementAction(AmeisenBotX.Core.Engines.Movement.Enums.MovementAction.Move, fallbackTarget);
                                 }
                                 return BtStatus.Success;
                             }, "Execute Quest Batch")),

                        // 4.4: Pickup available quests
                        (() => QuestPulse.ActiveQuests.Count < 20 && Discovery.GetNearbyQuestGivers().Any(),
                            new Leaf(() => {
                                var questGiver = Discovery.GetNearbyQuestGivers().OrderBy(u => u.Position.GetDistance(Bot.Player.Position)).FirstOrDefault();
                                if (questGiver == null) return BtStatus.Failed;

                                if (questGiver.Position.GetDistance(Bot.Player.Position) > 4f)
                                {
                                    Bot.Movement.SetMovementAction(AmeisenBotX.Core.Engines.Movement.Enums.MovementAction.Move, questGiver.Position);
                                    return BtStatus.Success;
                                }

                                if (!CanInteract(questGiver.Guid)) return BtStatus.Success; 
                                
                                Bot.Movement.StopMovement();
                                Bot.Wow.InteractWithUnit(questGiver);
                                MarkInteraction(questGiver.Guid);
                                Interactor.ProcessDialogs();
                                return BtStatus.Success;
                            }, "Pickup Quests"))
                     )
                )
            );
        }

        private float GetOptimalCombatRange()
        {
            return Bot.Player.Class switch
            {
                WowClass.Hunter or WowClass.Mage or WowClass.Warlock or WowClass.Priest or WowClass.Druid or WowClass.Shaman => 25f,
                _ => 4f
            };
        }

        private Vector3 GetNavigablePosition(Vector3 poi)
        {
            if (poi == Vector3.Zero) return Vector3.Zero;
            
            if (Math.Abs(poi.Z) < 0.1f)
            {
                return new Vector3(poi.X, poi.Y, Bot.Player.Position.Z);
            }
            
            return poi;
        }

        private float GetPathDistance(Vector3 target)
        {
            if (!Bot.Movement.TryGetPath(target, out var path)) return float.MaxValue;
            
            float distance = 0f;
            Vector3 previous = Bot.Player.Position;
            foreach (var point in path)
            {
                distance += previous.GetDistance(point);
                previous = point;
            }
            return distance;
        }

        private IWowUnit IdentifyQuestTarget(string itemName)
        {
            if (_questItemDroppers.TryGetValue(itemName, out var validNames))
            {
                var cachedUnit = Bot.Objects.All.OfType<IWowUnit>()
                    .Where(u => !u.IsDead && validNames.Contains(u.ReadName()))
                    .OrderBy(u => u.Position.GetDistance(Bot.Player.Position))
                    .FirstOrDefault();
                
                if (cachedUnit != null) return cachedUnit;
            }

            if (DateTime.Now - _lastTooltipScan > TimeSpan.FromSeconds(2))
            {
                _lastTooltipScan = DateTime.Now;
                var nearbyUnits = Bot.Objects.All.OfType<IWowUnit>()
                    .Where(u => !u.IsDead && u.Position.GetDistance(Bot.Player.Position) < 40f)
                    .Take(15)
                    .ToList();

                foreach (var unit in nearbyUnits)
                {
                    string cleanName = itemName.Replace("Meat", "").Replace("Skin", "").Replace("Tooth", "").Replace("Claw", "").Replace("Fur", "").Replace("Pelt", "").Trim();
                    if (cleanName.Length > 3 && unit.ReadName().Contains(cleanName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (!_questItemDroppers.ContainsKey(itemName)) _questItemDroppers[itemName] = new HashSet<string>();
                        _questItemDroppers[itemName].Add(unit.ReadName());
                        return unit;
                    }
                }
            }
            
             string searchName = itemName.Split(' ')[0];
             if (searchName.Length > 3)
             {
                 return Bot.Objects.All.OfType<IWowUnit>()
                     .Where(u => !u.IsDead && u.ReadName().Contains(searchName, StringComparison.OrdinalIgnoreCase))
                     .OrderBy(u => u.Position.GetDistance(Bot.Player.Position))
                     .FirstOrDefault();
             }
             
             return null;
        }
    }
}
