using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Logic.Harvest.Modules;
using AmeisenBotX.Logging;
using AmeisenBotX.Wow.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Logic.Harvest
{
    /// <summary>
    /// Manages harvest modules and coordinates GameObject targeting.
    /// Only loads modules applicable to the current character's professions.
    /// </summary>
    public class HarvestManager
    {
        private readonly AmeisenBotInterfaces Bot;
        private readonly List<IHarvestModule> LoadedModules;

        public HarvestManager(AmeisenBotInterfaces bot)
        {
            Bot = bot ?? throw new ArgumentNullException(nameof(bot));
            LoadedModules = [];
            InitializeModules();
        }

        /// <summary>
        /// Gets count of loaded modules for diagnostics.
        /// </summary>
        public int LoadedModuleCount => LoadedModules.Count;

        /// <summary>
        /// Initialize and load applicable harvest modules.
        /// </summary>
        private void InitializeModules()
        {
            // Available module types
            IHarvestModule[] availableModules =
            [
                new MiningHarvestModule(Bot),
                new HerbalismHarvestModule(Bot),
                new QuestObjectHarvestModule(Bot),  // Prioritize Quest Objects (Sparkling)
                new ChestHarvestModule(Bot)         // Then Loot Chests (Non-Sparkling)
            ];

            // Load only modules that apply to this character
            foreach (IHarvestModule module in availableModules)
            {
                try
                {
                    if (module.ShouldLoad(Bot))
                    {
                        LoadedModules.Add(module);
                        AmeisenLogger.I.Log("HarvestManager", $"Loaded module: {module.Name}");
                    }
                }
                catch (Exception ex)
                {
                    AmeisenLogger.I.Log("HarvestManager", $"Failed to load module {module.Name}: {ex.Message}", Logging.Enums.LogLevel.Error);
                }
            }

            if (LoadedModules.Count == 0)
            {
                AmeisenLogger.I.Log("HarvestManager", "No harvest modules loaded - harvesting disabled");
            }
        }

        /// <summary>
        /// Find the best GameObject to harvest using optimized 3-stage pipeline:
        /// Stage 1: Global pre-filter (IsUsable, blacklist, Euclidean range) - CHEAPEST
        /// Stage 2: Module evaluation (Matches + CanHarvest + Priority) - MEDIUM
        /// Stage 3: Pathfinding validation (top N candidates) - EXPENSIVE
        /// </summary>
        /// <param name="searchCenter">Center point for search radius.</param>
        /// <param name="maxRadius">Maximum search distance (default 30 yards).</param>
        /// <param name="blacklist">GUIDs to exclude from search.</param>
        /// <returns>Best harvest target, or null if none found.</returns>
        public IWowGameobject FindBestTarget(Vector3 searchCenter, float maxRadius, HashSet<ulong> blacklist)
        {
            // Fast exit if no modules loaded
            if (LoadedModules.Count == 0 || Bot?.Objects == null || Bot?.Player == null)
            {
                return null;
            }

            blacklist ??= [];
            Vector3 playerPos = Bot.Player.Position;

            try
            {
                // ═══════════════════════════════════════════════════════════════
                // STAGE 1: Global Pre-Filter (CHEAPEST - no module calls)
                // ═══════════════════════════════════════════════════════════════
                var stage1Candidates = Bot.Objects.All.OfType<IWowGameobject>()
                    .Where(g => g != null)
                    .Where(g => g.IsUsable)                                    // Universal interactability
                    .Where(g => !blacklist.Contains(g.Guid))                   // Blacklist check  
                    .Where(g => g.Position.GetDistance(searchCenter) <= maxRadius);  // Range check

                // ═══════════════════════════════════════════════════════════════
                // STAGE 2: Module Evaluation (MEDIUM - type + skill checks)
                // ═══════════════════════════════════════════════════════════════
                var stage2Candidates = stage1Candidates
                    .Select(g => new { Obj = g, Module = GetMatchingModule(g) })
                    .Where(x => x.Module != null)
                    .Select(x => new
                    {
                        x.Obj,
                        Priority = x.Module.GetPriority(x.Obj),
                        EucDist = x.Obj.Position.GetDistance(playerPos)
                    })
                    .Where(x => x.Priority > 0)
                    .OrderByDescending(x => x.Priority)
                    .ThenBy(x => x.EucDist)
                    .ToList();

                if (stage2Candidates.Count == 0)
                {
                    return null;
                }

                // ═══════════════════════════════════════════════════════════════
                // STAGE 3: Pathfinding Validation (EXPENSIVE - top N only)
                // ═══════════════════════════════════════════════════════════════
                const int MAX_CANDIDATES_TO_CHECK = 5;
                var topCandidates = stage2Candidates.Take(MAX_CANDIDATES_TO_CHECK);

                List<(IWowGameobject Obj, int Priority, float PathDist)> validCandidates = [];

                foreach (var candidate in topCandidates)
                {
                    IEnumerable<Vector3> path = Bot.PathfindingHandler.GetPath(
                        (int)Bot.Objects.MapId,
                        playerPos,
                        candidate.Obj.Position);

                    if (path != null && path.Any())
                    {
                        // Calculate real distance along path
                        float pathDist = 0f;
                        Vector3 prev = playerPos;
                        foreach (Vector3 point in path)
                        {
                            pathDist += prev.GetDistance(point);
                            prev = point;
                        }

                        validCandidates.Add((candidate.Obj, candidate.Priority, pathDist));
                    }
                    // Unreachable candidates are implicitly filtered out
                }

                if (validCandidates.Count > 0)
                {
                    return validCandidates
                        .OrderByDescending(x => x.Priority)
                        .ThenBy(x => x.PathDist)
                        .First().Obj;
                }

                return null;
            }
            catch (Exception ex)
            {
                AmeisenLogger.I.Log("HarvestManager", $"Error in FindBestTarget: {ex.Message}", Logging.Enums.LogLevel.Error);
                return null;
            }
        }

        /// <summary>
        /// Find the first module that matches AND can harvest this object.
        /// Returns null if no module can handle it.
        /// </summary>
        private IHarvestModule GetMatchingModule(IWowGameobject gobject)
        {
            foreach (IHarvestModule module in LoadedModules)
            {
                try
                {
                    if (module.Matches(gobject) && module.CanHarvest(gobject))
                    {
                        AmeisenLogger.I.Log("HarvestManager", $"Module '{module.Name}' matched object '{gobject.Name}' (DisplayId={gobject.DisplayId})", Logging.Enums.LogLevel.Debug);
                        return module;
                    }
                }
                catch
                {
                    // Ignore module errors
                }
            }
            return null;
        }
    }
}
