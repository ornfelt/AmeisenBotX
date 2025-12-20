using AmeisenBotX.Logging;
using AmeisenBotX.Logging.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AmeisenBotX.Core.Engines.Movement.AI
{
    /// <summary>
    /// Learning engine for combat outcomes.
    /// Remembers past fights and predicts future outcomes based on similarity (KNN).
    /// </summary>
    public class CombatLearner : IDisposable
    {
        private readonly List<CombatSnapshot> memory = [];
        private readonly string memoryFile;
        private readonly string brainFile;
        private const int MaxMemorySize = 1000;
        private readonly Lock _memoryLock = new(); // Protection for memory list
        private CancellationTokenSource autoSaveCts;

        public MultiHeadNeuralNetwork Brain => brain; // Exposed for Visualization
        public float LastWinProbability { get; private set; } // Exposed for Visualization

        private MultiHeadNeuralNetwork brain;
        private bool isTrained = false;

        public CombatLearner(AmeisenBotConfig config)
        {
            try
            {
                if (string.IsNullOrEmpty(config.Path))
                {
                    throw new Exception("Bot configuration path is missing.");
                }

                string profileFolder = Path.GetDirectoryName(config.Path);
                string folder = Path.Combine(profileFolder, "brain");
                Directory.CreateDirectory(folder);

                memoryFile = Path.Combine(folder, "combat_memory.json");
                brainFile = Path.Combine(folder, "multihead_net.json");
                AmeisenLogger.I.Log("CombatLearner", $"Brain file path: {brainFile}", LogLevel.Master);

                LoadMemory();

                // Initialize Brain (MULTI-HEAD NETWORK V11 - ChatGPT Spec)
                // Backbone: 25→64→BN→Drop→64→BN→Drop→32
                // Strategy Head: 32→5 (Softmax)
                // WinProb Head: 16→1 (Sigmoid)
                brain = MultiHeadNeuralNetwork.Load(brainFile);

                if (brain == null)
                {
                    brain = new MultiHeadNeuralNetwork();
                    AmeisenLogger.I.Log("CombatLearner", "Initialized new MULTI-HEAD Neural Network (V11 - ChatGPT Spec).", LogLevel.Master);

                    // SYNTHETIC PRE-TRAINING: Start optimistic, learn from deaths
                    GenerateSyntheticTraining();
                }
                else
                {
                    AmeisenLogger.I.Log("CombatLearner", "Loaded existing Multi-Head Neural Network.", LogLevel.Master);
                    isTrained = true;
                }

                // Initial Training on boot (real memory)
                if (memory.Count > 0)
                {
                    TrainOnMemory(epochs: 50);
                }

                // Start Auto-Save Loop with cancellation
                autoSaveCts = new CancellationTokenSource();
                _ = Task.Run(async () =>
                {
                    while (!autoSaveCts.Token.IsCancellationRequested)
                    {
                        try
                        {
                            await Task.Delay(60000 * 5, autoSaveCts.Token);
                            SaveBrain();
                        }
                        catch (OperationCanceledException) { break; }
                    }
                });
            }
            catch (Exception ex)
            {
                AmeisenLogger.I.Log("CombatLearner", $"Init Failed: {ex.Message}", LogLevel.Error);
            }
        }

        public void Dispose()
        {
            autoSaveCts?.Cancel();
            SaveBrain(); // Final save
            autoSaveCts?.Dispose();
            GC.SuppressFinalize(this);
        }

        public bool HasSufficientData
        {
            get { lock (_memoryLock) { return isTrained || memory.Count > 10; } }
        }

        // --- PREDICTION ---
        public (AiCombatStrategy Strategy, float Confidence, float ExplicitWinProb) PredictStrategy(CombatSnapshot s)
        {
            // HEURISTIC FALLBACK (If Brain is untrained/initializing)
            if (brain == null || !isTrained)
            {
                // Simple Rule-Based Logic
                double myHp = (s.MyMaxHealth > 0) ? (s.MyHealth / s.MyMaxHealth) : 0;
                double targetHp = (s.TargetMaxHealth > 0) ? (s.TargetHealth / s.TargetMaxHealth) : 1;

                // Winning?
                if (targetHp < 0.35 && myHp > 0.4)
                {
                    return (AiCombatStrategy.Farm, 0.95f, 0.99f);
                }

                // Losing?
                if (myHp < 0.35)
                {
                    return (AiCombatStrategy.Survival, 0.25f, 0.10f);
                }

                // Standard Combat
                return (AiCombatStrategy.Standard, 0.75f, 0.60f);
            }

            double[] inputs = ExtractFeatures(s);

            try
            {
                // MultiHead returns (Strategy[], WinProb)
                (double[] strategyOutputs, double winProb) = brain.Forward(inputs);

                // Find max strategy (Softmax output already sums to 1)
                int maxIndex = 0;
                double maxValue = strategyOutputs[0];
                for (int i = 1; i < strategyOutputs.Length; i++)
                {
                    if (strategyOutputs[i] > maxValue)
                    {
                        maxValue = strategyOutputs[i];
                        maxIndex = i;
                    }
                }

                // Map Index 0-5 to Enum
                AiCombatStrategy strategy = maxIndex switch
                {
                    0 => AiCombatStrategy.Flee,
                    1 => AiCombatStrategy.Survival,
                    2 => AiCombatStrategy.Burst,
                    3 => AiCombatStrategy.Standard,
                    4 => AiCombatStrategy.Farm,
                    5 => AiCombatStrategy.Interrupt,
                    _ => AiCombatStrategy.Standard
                };

                return (strategy, (float)maxValue, (float)winProb);
            }
            catch (Exception ex)
            {
                AmeisenLogger.I.Log("CombatLearner", $"Prediction failed: {ex.Message}", LogLevel.Warning);
                return (AiCombatStrategy.Standard, 0.0f, 0.5f);
            }
        }

        public void Learn(CombatSnapshot startState, bool won, float finalHealthPct, double durationSeconds)
        {
            if (startState == null || brain == null)
            {
                return;
            }

            // 1. Determine GROUND TRUTH Strategy (The "Label") based on Outcome
            int targetIndex = 3; // Default Standard

            // PRIORITY 1: If target was casting, we should have interrupted
            if (startState.TargetIsCasting)
            {
                targetIndex = 5; // Interrupt
            }
            else if (!won)
            {
                targetIndex = 0; // Flee (Loss = Should have Fled)
            }
            else
            {
                // We Won. How hard was it?
                if (finalHealthPct < 0.25f)
                {
                    targetIndex = 1; // Survival (Barely survived)
                }
                else if (finalHealthPct < 0.60f || durationSeconds > 30)
                {
                    targetIndex = 2; // Burst (Hard fight)
                }
                else if (finalHealthPct > 0.85f && durationSeconds < 15)
                {
                    targetIndex = 4; // Farm (Easy)
                }
                else
                {
                    targetIndex = 3; // Standard
                }
            }

            // One-Hot Encoding for Strategy (6 classes including Interrupt)
            double[] strategyTargets = new double[6];
            for (int i = 0; i < 6; i++)
            {
                strategyTargets[i] = (i == targetIndex) ? 1.0 : 0.0;
            }

            // Win Probability (Continuous based on fight quality)
            double winQuality;
            if (!won)
            {
                winQuality = 0.0; // Loss = 0
            }
            else
            {
                double healthFactor = Math.Clamp(finalHealthPct, 0.0, 1.0);
                double timeFactor = Math.Clamp(1.0 - (durationSeconds / 60.0), 0.3, 1.0);
                winQuality = healthFactor * timeFactor;
            }

            startState.Won = won;
            startState.WinQuality = winQuality;
            startState.ResultStrategy = targetIndex;

            lock (_memoryLock)
            {
                memory.Add(startState);
                if (memory.Count > MaxMemorySize)
                {
                    memory.RemoveAt(0);
                }
            }

            // Online Train with new API
            double[] inputs = ExtractFeatures(startState);
            brain.Train(inputs, strategyTargets, winQuality);
            isTrained = true;
            SaveMemory();
            SaveBrain(); // Save after every combat
        }

        private void SaveBrain()
        {
            if (brain == null || string.IsNullOrEmpty(brainFile))
            {
                return;
            }

            try
            {
                brain.Save(brainFile);
                AmeisenLogger.I.Log("CombatLearner", $"Brain saved to: {brainFile}", LogLevel.Debug);
            }
            catch (Exception ex)
            {
                AmeisenLogger.I.Log("CombatLearner", $"Failed to save brain: {ex.Message}", LogLevel.Error);
            }
        }

        private void TrainOnMemory(int epochs)
        {
            if (brain == null)
            {
                return;
            }

            // Experience Replay: Take random shuffled samples for decorrelated training
            List<CombatSnapshot> trainingSet;
            lock (_memoryLock)
            {
                Random rand = new();
                trainingSet = memory.OrderBy(_ => rand.Next()).Take(200).ToList();
            }

            for (int e = 0; e < epochs; e++)
            {
                // Shuffle each epoch for better generalization
                List<CombatSnapshot> shuffled = trainingSet.OrderBy(_ => Guid.NewGuid()).ToList();

                foreach (CombatSnapshot s in shuffled)
                {
                    double[] inputs = ExtractFeatures(s);
                    double[] strategyTargets = new double[6];
                    int idx = s.ResultStrategy;
                    for (int i = 0; i < 6; i++)
                    {
                        strategyTargets[i] = (i == idx) ? 1.0 : 0.0;
                    }

                    brain.Train(inputs, strategyTargets, s.WinQuality);
                }
            }
            isTrained = true;
        }

        private double[] ExtractFeatures(CombatSnapshot s)
        {
            // === OPTIMIZED FEATURE SET (17 inputs) ===

            // 1. HP Advantage (my HP% - target HP%) → range [-1, 1]
            double myHpPct = (s.MyMaxHealth > 0) ? (s.MyHealth / s.MyMaxHealth) : 0;
            double targetHpPct = (s.TargetMaxHealth > 0) ? (s.TargetHealth / s.TargetMaxHealth) : 1;
            double f1 = myHpPct - targetHpPct; // Positive = winning, Negative = losing

            // 2. My Power% (mana/rage/energy)
            double f2 = (s.MyMaxPower > 0) ? (s.MyPower / s.MyMaxPower) : 1;

            // 3. Target Power% (OOM target = weaker)
            double f3 = (s.TargetMaxPower > 0) ? (s.TargetPower / s.TargetMaxPower) : 0.5;

            // 4. Level Threat Score (more granular, uses TARGET level delta)
            // WoW mechanics: negative = I'm higher level, positive = enemy higher
            // Note: Using TargetLevelDelta not AvgLevelDelta for single-target accuracy
            double levelDelta = s.TargetLevelDelta; // positive = enemy higher level
            double f4;
            if (levelDelta <= -10)
            {
                f4 = 0.0;       // Gray mob - trivial
            }
            else if (levelDelta <= -5)
            {
                f4 = 0.15;  // Green mob - easy
            }
            else if (levelDelta <= -3)
            {
                f4 = 0.25; // Light green - manageable
            }
            else if (levelDelta <= -2)
            {
                f4 = 0.33;  // Slightly lower - advantage
            }
            else if (levelDelta <= -1)
            {
                f4 = 0.4;  // Slightly lower - advantage
            }
            else if (levelDelta == 0)
            {
                f4 = 0.5;   // Same level - fair
            }
            else if (levelDelta <= 2)
            {
                f4 = 0.6;   // Slightly higher - slight disadvantage
            }
            else if (levelDelta <= 5)
            {
                f4 = 0.75;  // Orange - challenging
            }
            else if (levelDelta <= 8)
            {
                f4 = 0.9;   // Red - dangerous
            }
            else
            {
                f4 = 1.0;                         // Skull (10+) - nearly impossible
            }

            // 5. Enemy Count (normalized)
            double f5 = Math.Min(s.EnemyCount / 5.0, 1.0);

            // 6. Party Count (normalized)
            double f6 = Math.Min(s.PartyCount / 5.0, 1.0);

            // 7. Enemy Threat (total HP relative to me)
            double hpRatio = (s.MyMaxHealth > 0) ? (s.TotalEnemyCurrentHealth / s.MyMaxHealth) : 10.0;
            double f7 = Math.Tanh(hpRatio / 3.0);

            // 8. Target Contribution (how much of total threat is target)
            double f8 = (s.TotalEnemyCurrentHealth > 0) ? (s.TargetHealth / s.TotalEnemyCurrentHealth) : 1;

            // 9. Incoming Damage (DTPS normalized)
            double pressureIn = (s.MyMaxHealth > 0) ? (s.IncomingDTPS / s.MyMaxHealth) : 0;
            double f9 = Math.Tanh(pressureIn * 5.0);

            // 10. Outgoing Damage (DPS normalized)
            double pressureOut = (s.TotalEnemyMaxHealth > 0) ? (s.OutgoingDPS / s.TotalEnemyMaxHealth) : 0;
            double f10 = Math.Tanh(pressureOut * 5.0);

            // 11. Elite present?
            double f11 = s.EliteCount > 0 ? 1.0 : 0.0;

            // 12. Instance context
            double f12 = s.IsInstance ? 1.0 : 0.0;

            // 13. PVP context
            double f13 = s.IsPvp ? 1.0 : 0.0;

            // 14. Enemy healer present
            double f14 = Math.Min(s.EnemyHealerCount / 2.0, 1.0);

            // 15. Target is Player (dangerous!)
            double f15 = s.TargetIsPlayer ? 1.0 : 0.0;

            // 16. Critical HP warning (panic state)
            double f16 = (myHpPct < 0.35) ? 1.0 : 0.0;

            // 17. Target HP% (absolute - for tracking kill progress)
            double f17 = targetHpPct;

            // === NEW FEATURES ===
            // 18. Target Is Casting (interrupt opportunity)
            double f18 = s.TargetIsCasting ? 1.0 : 0.0;

            // 19. Distance to Target (normalized: 40yd = max range)
            double f19 = Math.Min(s.DistanceToTarget / 40.0, 1.0);

            // 20. Combat Duration (capped at 60s - longer = we're stuck)
            double f20 = Math.Min(s.CombatDurationSeconds / 60.0, 1.0);

            return [
                f1, f2, f3, f4, f5, f6, f7, f8, f9, f10,
                f11, f12, f13, f14, f15, f16, f17, f18, f19, f20
            ];
        }

        // Persistence logic
        private void LoadMemory()
        {
            if (!File.Exists(memoryFile))
            {
                return;
            }

            try
            {
                string json = File.ReadAllText(memoryFile);
                List<CombatSnapshot> loaded = JsonSerializer.Deserialize<List<CombatSnapshot>>(json);

                if (loaded != null)
                {
                    lock (_memoryLock)
                    {
                        memory.AddRange(loaded);
                    }
                }
            }
            catch { }
        }

        private void SaveMemory()
        {
            try
            {
                string json;
                lock (_memoryLock)
                {
                    json = JsonSerializer.Serialize(memory, new JsonSerializerOptions { WriteIndented = false });
                }
                File.WriteAllText(memoryFile, json);
            }
            catch (Exception ex)
            {
                AmeisenLogger.I.Log("CombatLearner", "Save failed: " + ex.Message, LogLevel.Error);
            }
        }

        /// <summary>
        /// Generate synthetic training data with OPTIMISTIC bias.
        /// Philosophy: Overestimate → Die → Learn (better than underestimate → never fight)
        /// </summary>
        private void GenerateSyntheticTraining()
        {
            AmeisenLogger.I.Log("CombatLearner", "Generating synthetic pre-training (optimistic bias)...", LogLevel.Master);

            Random rand = new();
            int scenarios = 500;

            for (int i = 0; i < scenarios; i++)
            {
                // Generate random but realistic combat scenarios
                double myHp = 0.7 + (rand.NextDouble() * 0.3); // 70-100% HP at start
                double myPower = 0.5 + (rand.NextDouble() * 0.5); // 50-100% power
                double targetPower = 0.3 + (rand.NextDouble() * 0.7); // 30-100% power
                int enemyCount = rand.Next(1, 4); // 1-3 enemies
                int levelDelta = rand.Next(-3, 4); // -3 to +3 levels
                bool isElite = rand.NextDouble() < 0.15; // 15% elite
                double targetHp = 0.8 + (rand.NextDouble() * 0.2); // 80-100% target HP
                bool isPlayer = rand.NextDouble() < 0.1; // 10% player target
                bool isCasting = rand.NextDouble() < 0.2; // 20% casting target
                double distance = 5 + (rand.NextDouble() * 30); // 5-35 yards
                double duration = rand.NextDouble() * 30; // 0-30 seconds

                // Build synthetic feature vector (20 features)
                double[] inputs = new double[20];
                inputs[0] = myHp - targetHp;                    // HP Advantage (relative)
                inputs[1] = myPower;                            // My Power%
                inputs[2] = targetPower;                        // Target Power%
                inputs[3] = (levelDelta + 10) / 20.0;           // Level threat
                inputs[4] = enemyCount / 5.0;                   // Enemy count
                inputs[5] = 0.0;                                // Party count (solo)
                inputs[6] = 0.3 + (rand.NextDouble() * 0.4);      // Enemy threat
                inputs[7] = 1.0 / enemyCount;                   // Target contribution
                inputs[8] = rand.NextDouble() * 0.3;            // Inc DTPS
                inputs[9] = rand.NextDouble() * 0.5;            // Out DPS
                inputs[10] = isElite ? 1.0 : 0.0;               // Elite?
                inputs[11] = 0.0;                               // Instance
                inputs[12] = isPlayer ? 1.0 : 0.0;              // PVP
                inputs[13] = 0.0;                               // Enemy healer
                inputs[14] = isPlayer ? 1.0 : 0.0;              // Target is player
                inputs[15] = myHp < 0.35 ? 1.0 : 0.0;           // Critical HP
                inputs[16] = targetHp;                          // Target HP%
                inputs[17] = isCasting ? 1.0 : 0.0;             // Target Casting
                inputs[18] = distance / 40.0;                   // Distance (normalized)
                inputs[19] = duration / 60.0;                   // Duration (normalized)

                // Calculate OPTIMISTIC win probability
                double winProb = 0.85;
                winProb += (myHp - targetHp) * 0.3;
                winProb += levelDelta * -0.05;
                winProb += (enemyCount - 1) * -0.1;
                winProb += isElite ? -0.25 : 0.0;
                winProb += isPlayer ? -0.15 : 0.0;
                winProb = Math.Clamp(winProb, 0.3, 0.98);

                // Determine strategy (now 6 including Interrupt)
                int strategyIdx;
                if (isCasting && rand.NextDouble() < 0.7)
                {
                    strategyIdx = 5; // Interrupt (high priority if casting)
                }
                else if (winProb < 0.4)
                {
                    strategyIdx = 0;        // Flee
                }
                else if (winProb < 0.55)
                {
                    strategyIdx = 1;       // Survival
                }
                else if (winProb < 0.75 || isElite)
                {
                    strategyIdx = 2; // Burst
                }
                else if (winProb > 0.90)
                {
                    strategyIdx = 4;       // Farm
                }
                else
                {
                    strategyIdx = 3;                           // Standard
                }

                double[] strategyTargets = new double[6];
                for (int t = 0; t < 6; t++)
                {
                    strategyTargets[t] = (t == strategyIdx) ? 1.0 : 0.0;
                }

                brain.Train(inputs, strategyTargets, winProb);
            }

            isTrained = true;
            SaveBrain();
            AmeisenLogger.I.Log("CombatLearner", $"Synthetic pre-training complete ({scenarios} scenarios).", LogLevel.Master);
        }
    }
}
