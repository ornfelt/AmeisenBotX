using AmeisenBotX.Core.Engines.Movement.AI;
using AmeisenBotX.Logging;
using AmeisenBotX.Logging.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace AmeisenBotX.Core.Engines.AI
{
    /// <summary>
    /// Snapshot of immediate tactical state for micro-decisions.
    /// Used to decide WHICH spell category to use right now.
    /// </summary>
    public class TacticalSnapshot
    {
        // Resource state
        public double HealthPct { get; set; }
        public double PowerPct { get; set; }

        // Target state  
        public double TargetHealthPct { get; set; }
        public double TargetDistance { get; set; }
        public bool TargetIsCasting { get; set; }
        public bool TargetIsInterruptible { get; set; }

        // Self state
        public bool HasDefensiveBuff { get; set; }
        public bool HasOffensiveBuff { get; set; }
        public int ActiveDotsOnTarget { get; set; }
        public int ActiveHotsOnSelf { get; set; }

        // Cooldown availability (normalized 0-1, 0 = ready)
        public double BurstCdRatio { get; set; }
        public double DefensiveCdRatio { get; set; }
        public double InterruptCdRatio { get; set; }

        // Context flags
        public bool IsMoving { get; set; }
        public bool EnemyInMelee { get; set; }
        public int EnemyCount { get; set; }
        public int FriendlyCount { get; set; }

        // Ground truth for training
        public int ActionTaken { get; set; } // AiSpellCategory index
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Tactical Brain.
    /// Decides WHICH spell category to cast based on the immediate micro-situation.
    /// Inputs: Cooldowns, Ranges, Buffs, Resource, EnemyState.
    /// Output: AiSpellCategory (One-Hot Encoded).
    /// </summary>
    public class CombatActionClassifier
    {
        private const int InputSize = 20;
        private const int OutputSize = 12; // AiSpellCategory enum size

        private SimpleNeuralNetwork brain;
        private readonly string brainFile;
        private readonly string memoryFile;
        private readonly List<TacticalSnapshot> memory = new();
        private readonly Lock _lock = new();
        private bool isTrained = false;

        public CombatActionClassifier(AmeisenBotConfig config)
        {
            string profileFolder = Path.GetDirectoryName(config.Path);
            string folder = Path.Combine(profileFolder, "brain");
            Directory.CreateDirectory(folder);
            brainFile = Path.Combine(folder, "tactical_net.json");
            memoryFile = Path.Combine(folder, "tactical_memory.json");

            // Load existing brain
            brain = SimpleNeuralNetwork.Load(brainFile);

            if (brain == null || brain.InputSize != InputSize || brain.OutputSize != OutputSize)
            {
                brain = new SimpleNeuralNetwork(InputSize, 64, 48, OutputSize);
                AmeisenLogger.I.Log("CombatActionClassifier", "Initialized new TACTICAL Neural Network.", LogLevel.Master);
            }
            else
            {
                isTrained = true;
            }

            // Load memory
            LoadMemory();
        }

        /// <summary>
        /// Predict which spell category to use based on current tactical state.
        /// </summary>
        public AiSpellCategory PredictAction(TacticalSnapshot state)
        {
            if (brain == null || state == null)
                return AiSpellCategory.Unknown;

            // Heuristic fallback if untrained
            if (!isTrained)
            {
                return GetHeuristicAction(state);
            }

            var inputs = ExtractFeatures(state);
            var outputs = brain.FeedForward(inputs);

            // Find max output (argmax)
            int maxIdx = 0;
            double maxVal = outputs[0];
            for (int i = 1; i < outputs.Length; i++)
            {
                if (outputs[i] > maxVal)
                {
                    maxVal = outputs[i];
                    maxIdx = i;
                }
            }

            // Only use prediction if confidence is reasonable
            if (maxVal < 0.3)
                return GetHeuristicAction(state);

            return (AiSpellCategory)maxIdx;
        }

        /// <summary>
        /// Train on a single observation.
        /// </summary>
        public void Train(TacticalSnapshot state, AiSpellCategory actionTaken)
        {
            if (brain == null || state == null) return;

            state.ActionTaken = (int)actionTaken;

            lock (_lock)
            {
                memory.Add(state);
                if (memory.Count > 500) memory.RemoveAt(0);
            }

            // Online training
            var inputs = ExtractFeatures(state);
            var targets = new double[OutputSize];
            targets[(int)actionTaken] = 1.0;

            brain.Train(inputs, targets);
            isTrained = true;

            // Periodic save
            if (memory.Count % 20 == 0)
            {
                SaveBrain();
                SaveMemory();
            }
        }

        private double[] ExtractFeatures(TacticalSnapshot s)
        {
            return new double[]
            {
                // Resource (0-2)
                s.HealthPct,
                s.PowerPct,
                s.HealthPct < 0.35 ? 1.0 : 0.0, // Critical flag
                
                // Target (3-7)
                s.TargetHealthPct,
                Math.Min(s.TargetDistance / 40.0, 1.0), // Normalized to 40 yards
                s.TargetIsCasting ? 1.0 : 0.0,
                s.TargetIsInterruptible ? 1.0 : 0.0,
                s.TargetHealthPct < 0.20 ? 1.0 : 0.0, // Execute range
                
                // Buffs (8-11)
                s.HasDefensiveBuff ? 1.0 : 0.0,
                s.HasOffensiveBuff ? 1.0 : 0.0,
                Math.Min(s.ActiveDotsOnTarget / 3.0, 1.0),
                Math.Min(s.ActiveHotsOnSelf / 3.0, 1.0),
                
                // Cooldowns (12-14)
                s.BurstCdRatio,
                s.DefensiveCdRatio,
                s.InterruptCdRatio,
                
                // Context (15-19)
                s.IsMoving ? 1.0 : 0.0,
                s.EnemyInMelee ? 1.0 : 0.0,
                Math.Min(s.EnemyCount / 5.0, 1.0),
                Math.Min(s.FriendlyCount / 5.0, 1.0),
                s.EnemyCount > 3 ? 1.0 : 0.0 // AoE situation
            };
        }

        private AiSpellCategory GetHeuristicAction(TacticalSnapshot s)
        {
            // Emergency healing
            if (s.HealthPct < 0.30)
                return AiSpellCategory.HealSelf;

            // Defensive cooldown
            if (s.HealthPct < 0.40 && s.DefensiveCdRatio < 0.1)
                return AiSpellCategory.DefensiveCooldown;

            // Interrupt
            if (s.TargetIsCasting && s.TargetIsInterruptible && s.InterruptCdRatio < 0.1)
                return AiSpellCategory.CrowdControl;

            // Execute phase
            if (s.TargetHealthPct < 0.20 && s.BurstCdRatio < 0.1)
                return AiSpellCategory.BurstCooldown;

            // Need DoTs
            if (s.ActiveDotsOnTarget == 0)
                return AiSpellCategory.DamageDot;

            // Default damage
            return AiSpellCategory.Damage;
        }

        private void SaveBrain()
        {
            try { brain?.Save(brainFile); }
            catch { }
        }

        private void LoadMemory()
        {
            if (!File.Exists(memoryFile)) return;
            try
            {
                var loaded = JsonSerializer.Deserialize<List<TacticalSnapshot>>(File.ReadAllText(memoryFile));
                if (loaded != null)
                {
                    lock (_lock) memory.AddRange(loaded);
                }
            }
            catch { }
        }

        private void SaveMemory()
        {
            try
            {
                string json;
                lock (_lock)
                {
                    json = JsonSerializer.Serialize(memory, new JsonSerializerOptions { WriteIndented = false });
                }
                File.WriteAllText(memoryFile, json);
            }
            catch { }
        }
    }
}
