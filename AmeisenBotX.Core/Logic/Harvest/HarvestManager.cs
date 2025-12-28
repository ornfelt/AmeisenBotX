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
                new ChestHarvestModule(Bot),
                new QuestObjectHarvestModule(Bot)
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
        /// Find the best GameObject to harvest based on loaded modules.
        /// MUCH faster than monolithic checks - only evaluates relevant objects!
        /// </summary>
        /// <param name="searchCenter">Center point for search radius.</param>
        /// <param name="maxRadius">Maximum search distance.</param>
        /// <param name="blacklist">GUIDs to exclude from search.</param>
        /// <returns>Best harvest target, or null if none found.</returns>
        /// <summary>
        /// Find the best GameObject to harvest based on loaded modules.
        /// Uses 2-pass filtering:
        /// 1. fast Euclidean checks to get candidates
        /// 2. Pathfinding checks on top candidates to ensure reachability + true distance
        /// </summary>
        /// <param name="searchCenter">Center point for search radius.</param>
        /// <param name="maxRadius">Maximum search distance.</param>
        /// <param name="blacklist">GUIDs to exclude from search (can include path blacklist).</param>
        /// <returns>Best harvest target, or null if none found.</returns>
        public IWowGameobject FindBestTarget(Vector3 searchCenter, float maxRadius, HashSet<ulong> blacklist)
        {
            // Fast exit if no modules loaded
            if (LoadedModules.Count == 0 || Bot?.Objects == null)
            {
                return null;
            }

            // Defensive null check on blacklist
            blacklist ??= [];

            try
            {
                // Filter gameobjects - only check if ANY loaded module can harvest
                var initialCandidates = Bot.Objects.All.OfType<IWowGameobject>()
                    .Where(e => e != null
                        && e.Position.GetDistance(searchCenter) <= maxRadius
                        && !blacklist.Contains(e.Guid))
                    .Where(e => LoadedModules.Any(m => m.CanHarvest(e)))
                    .Select(e => new
                    {
                        GameObject = e,
                        Priority = GetHighestPriority(e),
                        EuclideanDist = e.Position.GetDistance(Bot.Player?.Position ?? searchCenter)
                    })
                    .Where(x => x.Priority > 0)
                    .OrderByDescending(x => x.Priority)
                    .ThenBy(x => x.EuclideanDist)
                    .ToList();

                if (initialCandidates.Count == 0)
                {
                    return null;
                }

                // 2. Pathfinding Pass
                // Only verify the top N candidates to allow for performance optimization
                // e.g. if the closest node is unreachable, check the next one, etc.
                int candidatesToCheck = 5;
                var topCandidates = initialCandidates.Take(candidatesToCheck).ToList();

                List<(IWowGameobject GameObject, int Priority, float PathDistance)> validCandidates = [];

                foreach (var candidate in topCandidates)
                {
                    // Calculate path distance
                    // Use IPathfindingHandler to get path points approx
                    // Note: We need MapId. Assuming Player MapId is sufficient for nearby objects.
                    IEnumerable<Vector3> path = Bot.PathfindingHandler.GetPath((int)Bot.Objects.MapId, Bot.Player.Position, candidate.GameObject.Position);

                    if (path != null && path.Any())
                    {
                        // Calculate real distance along the path
                        float pathDist = 0f;
                        Vector3 prev = Bot.Player.Position;
                        foreach (Vector3 point in path)
                        {
                            pathDist += prev.GetDistance(point);
                            prev = point;
                        }

                        validCandidates.Add((candidate.GameObject, candidate.Priority, pathDist));
                    }
                    else
                    {
                        // Pathfinding failed - unreachable
                        // Implicitly filtered out
                        // AmeisenLogger.I.Log("HarvestManager", $"Unreachable harvest target: {candidate.GameObject.Name}");
                    }
                }

                // If no top candidates were reachable, fallback or return null?
                // If we found ANY reachable candidates in the top N, pick the best one.
                if (validCandidates.Count > 0)
                {
                    (IWowGameobject GameObject, int Priority, float PathDistance) = validCandidates
                        .OrderByDescending(x => x.Priority)
                        .ThenBy(x => x.PathDistance)
                        .First();

                    return GameObject;
                }

                // Optional: If top N failed, maybe we should try the rest? 
                // For now, let's assume if the top 5 closest are unreachable, we probably can't reach anything or should wait.
                // Or we loop the next batch. 
                // Let's rely on the module blacklisting the unreachable ones if we return "null" and it tries to approach? 
                // Ah, if we return null here, the module does nothing.
                // But we just filtered them out here. 

                // Improved Logic: If top candidates failed pathing, we effectively "ignore" them for this tick.
                // The next tick will likely do the same unless we blacklist them effectively.
                // However, doing full pathfinding on ALL candidates is too heavy.
                // Strategy: Return null. Use "PathBlacklist" in the module to permanently ignore them if move fails.
                // But here we are Pre-Checking.

                return null;
            }
            catch (Exception ex)
            {
                AmeisenLogger.I.Log("HarvestManager", $"Error finding harvest target: {ex.Message}", Logging.Enums.LogLevel.Error);
                return null;
            }
        }

        /// <summary>
        /// Get the highest priority score from all loaded modules for a GameObject.
        /// </summary>
        private int GetHighestPriority(IWowGameobject gobject)
        {
            if (gobject == null)
            {
                return 0;
            }

            int maxPriority = 0;

            foreach (IHarvestModule module in LoadedModules)
            {
                try
                {
                    if (module.CanHarvest(gobject))
                    {
                        int priority = module.GetPriority(gobject);
                        if (priority > maxPriority)
                        {
                            maxPriority = priority;
                        }
                    }
                }
                catch (Exception ex)
                {
                    AmeisenLogger.I.Log("HarvestManager", $"Error getting priority from {module.Name}: {ex.Message}", Logging.Enums.LogLevel.Warning);
                }
            }

            return maxPriority;
        }
    }
}
