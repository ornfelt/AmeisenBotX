using AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Modules;
using AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Modules.Core;
using AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Modules.Group;
using AmeisenBotX.Logging;
using AmeisenBotX.Wow.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets
{
    /// <summary>
    /// Manages modular target selection, loading context-appropriate modules.
    /// Only active modules are evaluated for performance optimization.
    /// </summary>
    public class TargetSelectionManager
    {
        private readonly List<ITargetSelectionModule> AllModules;
        private readonly AmeisenBotInterfaces Bot;

        public TargetSelectionManager(AmeisenBotInterfaces bot)
        {
            Bot = bot ?? throw new ArgumentNullException(nameof(bot));
            AllModules = [];
            InitializeModules();
        }

        /// <summary>
        /// Gets count of currently active modules for diagnostics.
        /// </summary>
        public int ActiveModuleCount => AllModules.Count(m => m.IsActive(Bot));

        private void InitializeModules()
        {
            // Core modules (always available)
            AllModules.Add(new SelfDefenseModule());      // Priority: 100
            AllModules.Add(new ExecuteModule());          // Priority: 80
            AllModules.Add(new CCProtectionModule());     // Priority: -1000
            AllModules.Add(new ProximityModule());        // Priority: 20

            // Group modules (activate when in party/raid)
            AllModules.Add(new TankAssistModule());       // Priority: 90
            AllModules.Add(new HealerProtectionModule()); // Priority: 75
            AllModules.Add(new FocusFireModule());        // Priority: 15 * count
            AllModules.Add(new QuestPriorityModule());    // Priority: 50 (when active)
        }

        /// <summary>
        /// Select the best target from candidates using active modules.
        /// Returns null if no suitable target found.
        /// </summary>
        public IWowUnit SelectBestTarget(IEnumerable<IWowUnit> candidates)
        {
            if (candidates == null || !candidates.Any())
            {
                return null;
            }

            // Get currently active modules (context-dependent)
            List<ITargetSelectionModule> activeModules = AllModules.Where(m => m.IsActive(Bot)).ToList();

            if (activeModules.Count == 0)
            {
                AmeisenLogger.I.Log("TargetSelectionManager", "No active modules - fallback to first candidate", Logging.Enums.LogLevel.Warning);
                return candidates.FirstOrDefault();
            }

            try
            {
                // Score each candidate using active modules
                var scored = candidates
                    .Select(target => new
                    {
                        Target = target,
                        Score = CalculateTotalScore(target, activeModules)
                    })
                    .Where(x => x.Score > -999f) // Filter out forbidden targets (CC'd = -1000)
                    .OrderByDescending(x => x.Score)
                    .ToList();

                return scored.FirstOrDefault()?.Target;
            }
            catch (Exception ex)
            {
                AmeisenLogger.I.Log("TargetSelectionManager", $"Error selecting target: {ex.Message}", Logging.Enums.LogLevel.Error);
                return null;
            }
        }

        /// <summary>
        /// Calculate total priority score for a target by summing all active module bonuses.
        /// </summary>
        private float CalculateTotalScore(IWowUnit target, List<ITargetSelectionModule> modules)
        {
            float totalScore = 0f;

            foreach (ITargetSelectionModule module in modules)
            {
                try
                {
                    float bonus = module.GetPriorityBonus(target, Bot);
                    totalScore += bonus;
                }
                catch (Exception ex)
                {
                    AmeisenLogger.I.Log("TargetSelectionManager",
                        $"Error in module {module.Name}: {ex.Message}",
                        Logging.Enums.LogLevel.Warning);
                }
            }

            return totalScore;
        }
    }
}
