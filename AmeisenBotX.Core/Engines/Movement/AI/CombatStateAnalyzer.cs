#nullable enable
using AmeisenBotX.Logging;
using AmeisenBotX.Logging.Enums;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AmeisenBotX.Core.Engines.Movement.AI
{
    /// <summary>
    /// Analyzes the current combat situation to determine a "Win Probability Score".
    /// Integrates both Heuristic Force Analysis and ML-based History Learning.
    /// </summary>
    public class CombatStateAnalyzer : IDisposable
    {
        public CombatLearner Learner => learner; // Exposed for Visualization

        public float CurrentWinProbability => lastScore; // Point to local score
        public string CurrentAnalysisReason => lastAnalysisReason;
        public event Action<float, string, AiCombatStrategy>? OnAnalysisUpdated;

        private string lastAnalysisReason = "Initializing...";
        private readonly AmeisenBotInterfaces bot;
        private readonly AmeisenBotConfig config;
        private readonly CombatLearner learner;
        private bool disposed = false;

        // State Tracking
        private bool wasInCombat = false;
        private CombatSnapshot? currentFightStartSnapshot;
        private DateTime combatStartTime;

        // DPS / DTPS Tracking (Sliding Window)
        private const int DpsWindowSeconds = 5;
        private readonly List<(DateTime time, int amount)> incomingDamageQueue = [];
        private readonly List<(DateTime time, int amount)> outgoingDamageQueue = [];
        private readonly Lock damageLock = new();

        public CombatStateAnalyzer(AmeisenBotInterfaces bot, AmeisenBotConfig config)
        {
            this.bot = bot;
            this.config = config;
            this.learner = new CombatLearner(config);

            // Wire up Combat Log
            if (bot.CombatLog != null)
            {
                bot.CombatLog.OnDamage += OnDamageHandler;
                AmeisenLogger.I.Log("CombatAnalyzer", "Wired up CombatLog.OnDamage.", LogLevel.Debug);
            }
            else
            {
                AmeisenLogger.I.Log("CombatAnalyzer", "Warning: bot.CombatLog is null. DPS tracking disabled.", LogLevel.Warning);
            }
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;

            // Unsubscribe from events
            if (bot?.CombatLog != null)
            {
                bot.CombatLog.OnDamage -= OnDamageHandler;
            }

            // Cancel CombatLearner's auto-save
            learner?.Dispose();

            GC.SuppressFinalize(this);
        }


        private void OnDamageHandler(ulong source, ulong dest, int spellId, int amount, int over)
        {
            lock (damageLock)
            {
                DateTime now = DateTime.UtcNow;

                // Outgoing: Source is Me or Pet or Party
                // Simplification: Just Me for now to measure MY performance? 
                // Or Party? User said "we".
                // Let's count ME and PET.
                if (source == bot.Player.Guid || (bot.Pet != null && source == bot.Pet.Guid))
                {
                    outgoingDamageQueue.Add((now, amount));
                }

                // Incoming: Dest is Me
                if (dest == bot.Player.Guid)
                {
                    incomingDamageQueue.Add((now, amount));
                }
            }
        }

        private float CalculateDps(List<(DateTime time, int amount)> queue, DateTime now)
        {
            lock (damageLock)
            {
                // Prune old
                DateTime cutoff = now.AddSeconds(-DpsWindowSeconds);
                queue.RemoveAll(x => x.time < cutoff);

                if (queue.Count == 0)
                {
                    return 0f;
                }

                long total = queue.Sum(x => (long)x.amount);
                // Divide by WindowSeconds (average over window) 
                // OR divide by (Last - First)?
                // Standard DPS meter uses TimeActive.
                // For "Current DPS" sliding window, Total / Window is a safe smoothed metric.
                return (float)total / DpsWindowSeconds;
            }
        }

        public void Tick()
        {
            // Force an analysis update
            CalculateWinProbability(out _);
        }

        public float CalculateWinProbability(out string reason)
        {
            // Safety check
            if (bot.Player == null) { reason = "Init"; return 1.0f; }

            // 1. Maintain Combat State / Learning Loop
            ManageCombatSession();

            // 2. Generate Snapshot (Always, to visualize brain state)
            CombatSnapshot current = CreateSnapshot();

            // 3. Neural Prediction (STRATEGY + PROBABILITY)
            (AiCombatStrategy Strategy, float Confidence, float ExplicitWinProb) = learner.PredictStrategy(current);
            CurrentStrategy = Strategy;

            // 4. Use Explicit Win Probability from Brain (Hybrid Output)
            float score = ExplicitWinProb; // The direct regression output

            reason = $"AI: {CurrentStrategy} ({Confidence:P0}) [Prob: {score:F2}]";
            lastAnalysisReason = reason; // Update backing field
            lastScore = score; // Update backing field

            OnAnalysisUpdated?.Invoke(score, reason, CurrentStrategy);

            // Only return valid score if in combat, otherwise -1 (Idle) for Bot Logic, 
            // but we still updated the Brain state for Visualization!
            return !bot.Player.IsInCombat ? -1.0f : score;
        }

        public AiCombatStrategy CurrentStrategy { get; private set; } = AiCombatStrategy.Unknown;

        // Backing fields for visualization
        private float lastScore = 0f;

        // Removed Hardcoded DetermineStrategy method (Logic is now inside Neural Net Training Labels)

        private void ManageCombatSession()
        {
            // Death Check
            if (bot.Player.IsDead || bot.Player.Health <= 0)
            {
                if (wasInCombat && currentFightStartSnapshot != null)
                {
                    double duration = (DateTime.UtcNow - combatStartTime).TotalSeconds;
                    AmeisenLogger.I.Log("CombatAnalyzer", "Player Died! Recording LOSS/FLEE.", LogLevel.Master);
                    // Loss = Flee Strategy Target
                    learner.Learn(currentFightStartSnapshot, false, 0, duration);
                    currentFightStartSnapshot = null;
                    wasInCombat = false;
                    lock (damageLock) { incomingDamageQueue.Clear(); outgoingDamageQueue.Clear(); }
                }
                return;
            }

            bool inCombat = bot.Player.IsInCombat;

            // ... (Middle parts same) ... -> Need to replace block or be careful with context.
            // I'll assume context matches until "End" block


            // COMBAT START
            if (inCombat && !wasInCombat)
            {
                currentFightStartSnapshot = CreateSnapshot();
                combatStartTime = DateTime.UtcNow;

                // Clear DPS queues on new fight
                lock (damageLock)
                {
                    incomingDamageQueue.Clear();
                    outgoingDamageQueue.Clear();
                }

                AmeisenLogger.I.Log("CombatAnalyzer", $"Combat Started against {currentFightStartSnapshot.EnemyCount} enemies (Instance={currentFightStartSnapshot.IsInstance}). Snapped.", LogLevel.Verbose);
            }
            // COMBAT UPDATE (Escalation & Worst-Case Tracking)
            else if (inCombat && currentFightStartSnapshot != null)
            {
                CombatSnapshot current = CreateSnapshot();

                // 1. Update Escalation (Max Danger)
                if (current.EnemyCount > currentFightStartSnapshot.EnemyCount)
                {
                    currentFightStartSnapshot.EnemyCount = current.EnemyCount;
                    currentFightStartSnapshot.TotalEnemyMaxHealth = Math.Max(currentFightStartSnapshot.TotalEnemyMaxHealth, current.TotalEnemyMaxHealth);
                    currentFightStartSnapshot.EliteCount = Math.Max(currentFightStartSnapshot.EliteCount, current.EliteCount);
                    currentFightStartSnapshot.EnemyHealerCount = Math.Max(currentFightStartSnapshot.EnemyHealerCount, current.EnemyHealerCount);
                    AmeisenLogger.I.Log("CombatAnalyzer", $"Combat Escalated! Enemies: {current.EnemyCount}", LogLevel.Verbose);
                }

                // 2. Update Worst-Case Self State (Min HP/Power)
                // This ensures we train on the "Crisis Point" of the fight, not just the healthy start.
                // If we dip to 20% HP and win, we want the AI to learn that "20% HP -> Survival Strategy".
                if (current.MyHealth < currentFightStartSnapshot.MyHealth)
                {
                    currentFightStartSnapshot.MyHealth = current.MyHealth;
                    currentFightStartSnapshot.MyPower = current.MyPower;
                }
            }
            // COMBAT END
            else if (!inCombat && wasInCombat)
            {
                if (currentFightStartSnapshot != null)
                {
                    bool amAlive = !bot.Player.IsDead && bot.Player.Health > 0;
                    bool won = amAlive && bot.Player.HealthPercentage > 10;

                    double duration = (DateTime.UtcNow - combatStartTime).TotalSeconds;
                    float finalHp = (float)(bot.Player.HealthPercentage / 100.0);

                    AmeisenLogger.I.Log("CombatAnalyzer", $"Combat Ended. Result: {(won ? "WIN" : "LOSS")} (Alive={amAlive}, HP={bot.Player.HealthPercentage:F0}%, Dur={duration:F0}s). Learning Strategy...", LogLevel.Master);
                    learner.Learn(currentFightStartSnapshot, won, finalHp, duration);
                }
                currentFightStartSnapshot = null;
            }

            wasInCombat = inCombat;
        }

        private float CalculateHeuristicScore(CombatSnapshot s, out string reason)
        {
            // PROVEN LOGIC: Absolute Power / TTL

            // 1. My Power: Sum of Party Health (Absolute)
            // Use Snapshot values directly now!
            double myPower = s.TotalPartyCurrentHealth;

            // Mana factor
            if (s.MyMaxPower > 0)
            {
                float powerPct = (float)(s.MyPower / s.MyMaxPower);
                if (powerPct < 0.2f)
                {
                    myPower *= 0.8f;
                }
            }

            // 2. Enemy Power
            double enemyPower = s.TotalEnemyCurrentHealth;

            // If start of fight, snapshot might have 0 if CreateSnapshot ran before targets targeted me?
            // (GetCombatEnemies logic handles it, but just in case)
            if (enemyPower == 0 && s.EnemyCount > 0)
            {
                // Fallback to Max?
                enemyPower = s.TotalEnemyMaxHealth > 0 ? s.TotalEnemyMaxHealth : 1000;
            }

            // TTL Logic
            float score = 0.5f;

            if (s.IncomingDTPS > 0 && s.OutgoingDPS > 0)
            {
                double ttlMe = myPower / s.IncomingDTPS;
                double ttlEnemy = enemyPower / s.OutgoingDPS;

                double ttlRatio = ttlMe / (ttlMe + ttlEnemy);
                score = (float)ttlRatio;
                reason = $"TTL {ttlMe:F0}s vs {ttlEnemy:F0}s";
            }
            else
            {
                // Fallback to HP Ratio
                if (myPower + enemyPower > 0)
                {
                    score = (float)(myPower / (myPower + enemyPower));
                }

                reason = (score > 0.5) ? "HP Adv" : "HP Disadv";
            }

            // Level / Elite Penalties (Secondary)
            if (s.AvgLevelDelta > 2)
            {
                score -= 0.1f;
            }

            if (s.EliteCount > 0)
            {
                score -= 0.1f;
            }

            return Math.Clamp(score, 0.0f, 1.0f);
        }

        private List<IWowUnit> GetCombatEnemies()
        {
            HashSet<ulong> uniqueGuids = [];
            List<IWowUnit> result = [];

            void AddUnits(IEnumerable<IWowUnit> units)
            {
                if (units == null)
                {
                    return;
                }

                foreach (IWowUnit u in units)
                {
                    if (u != null && !u.IsDead && uniqueGuids.Add(u.Guid))
                    {
                        result.Add(u);
                    }
                }
            }

            // 1. Helpers (Check both Party and Me explicitly)
            AddUnits(bot.GetEnemiesOrNeutralsInCombatWithParty<IWowUnit>(bot.Player.Position, 60f));
            AddUnits(bot.GetEnemiesOrNeutralsInCombatWithMe<IWowUnit>(bot.Player.Position, 60f));

            // 2. Explicit Target Check (Always include current target if hostile)
            IWowUnit target = bot.Objects.Target;
            // IsValid is static on IWowUnit
            if (target != null && IWowUnit.IsValid(target) && !target.IsDead && bot.Db.GetReaction(bot.Player, target) == WowUnitReaction.Hostile)
            {
                if (uniqueGuids.Add(target.Guid))
                {
                    result.Add(target);
                }
            }

            // 3. Fallback: If 0 found but we are in combat, scan surrounding units targeting us
            if (result.Count == 0 && bot.Player.IsInCombat)
            {
                IEnumerable<IWowUnit> extra = bot.Objects.All.OfType<IWowUnit>()
                    .Where(u => !u.IsDead
                             && u.IsInCombat
                             && u.TargetGuid == bot.Player.Guid
                             && u.Position.GetDistance(bot.Player.Position) < 40);
                AddUnits(extra);
            }

            return result;
        }

        private CombatSnapshot CreateSnapshot()
        {
            CombatSnapshot s = new()
            {
                Timestamp = DateTime.UtcNow,

                IsInstance = bot.Objects.MapId.IsDungeonMap()
            };
            try { s.IsPvp = bot.Objects.MapId.IsBattlegroundMap(); } catch { s.IsPvp = false; }

            // --- SELF ---
            s.MyHealth = bot.Player.Health;
            s.MyMaxHealth = bot.Player.MaxHealth;

            (double current, double max) = GetUnitPower(bot.Player);
            s.MyPower = current;
            s.MyMaxPower = max;

            // --- PARTY (Includes Self) ---
            List<IWowUnit> members = bot.Objects.Partymembers.Where(p => !p.IsDead).ToList();
            s.PartyCount = members.Count; // Party members list usually doesn't include self

            // Add self to party total
            s.TotalPartyCurrentHealth = s.MyHealth;
            s.TotalPartyMaxHealth = s.MyMaxHealth;

            if (bot.Pet != null)
            {
                s.TotalPartyCurrentHealth += bot.Pet.Health;
                s.TotalPartyMaxHealth += bot.Pet.MaxHealth;
            }

            foreach (IWowUnit? m in members)
            {
                s.TotalPartyCurrentHealth += m.Health;
                s.TotalPartyMaxHealth += m.MaxHealth;
            }
            s.PartyCount++; // Add self count

            s.HealerCount = members.Count(m => m.Class is WowClass.Priest or WowClass.Druid or WowClass.Shaman or WowClass.Paladin);
            s.TankCount = members.Count(m => m.Class is WowClass.Warrior or WowClass.Paladin or WowClass.Deathknight or WowClass.Druid);

            // --- ENEMIES ---
            List<IWowUnit> enemies = GetCombatEnemies();

            s.EnemyCount = enemies.Count;

            // OPTIMIZATION: Single Pass Loop (Loop Fusion)
            int healerCount = 0;
            int tankCount = 0; // Keeping placeholder logic if we restore it
            int eliteCount = 0;
            double totalEnemyCur = 0;
            double totalEnemyMax = 0;
            float levelSum = 0;

            foreach (IWowUnit e in enemies)
            {
                // Health Sums
                totalEnemyCur += e.Health;
                totalEnemyMax += e.MaxHealth;
                levelSum += e.Level;

                // Role Logic (Healer)
                if (e.Class == WowClass.Priest || e.Class == WowClass.Shaman || (e.Class == WowClass.Druid && e.MaxMana > 0) || (e.Class == WowClass.Paladin && e.MaxMana > 0))
                {
                    healerCount++;
                }

                // Elite Logic
                if (e.MaxHealth > bot.Player.MaxHealth * 3)
                {
                    eliteCount++;
                }
            }

            s.EnemyHealerCount = healerCount;
            s.EnemyTankCount = tankCount;
            s.EliteCount = eliteCount;

            if (s.EnemyCount > 0)
            {
                s.AvgLevelDelta = (levelSum / s.EnemyCount) - bot.Player.Level;
            }
            s.TotalEnemyCurrentHealth = totalEnemyCur;
            // Fallback for MaxHealth if 0 (sometimes happens with API lag)
            s.TotalEnemyMaxHealth = totalEnemyMax > 0 ? totalEnemyMax : totalEnemyCur;

            // --- TARGET (Focus) ---
            IWowUnit target = bot.Objects.Target;
            if (target != null && !target.IsDead)
            {
                s.TargetHealth = target.Health;
                s.TargetMaxHealth = target.MaxHealth > 0 ? target.MaxHealth : target.Health;
                s.TargetPower = target.Mana; // Use Mana as generic power for now
                s.TargetMaxPower = target.MaxMana;
                s.TargetLevelDelta = target.Level - bot.Player.Level;
                s.TargetIsPlayer = target.IsPlayer();
                s.TargetCreatureType = (int)target.ReadType();

                // New combat features
                s.TargetIsCasting = target.IsCasting;
                s.DistanceToTarget = bot.Player.DistanceTo(target);
            }
            else
            {
                s.TargetHealth = 0;
                s.TargetMaxHealth = 0;
                s.TargetPower = 0;
                s.TargetMaxPower = 0;
                s.TargetLevelDelta = 0;
                s.TargetIsPlayer = false;
                s.TargetCreatureType = 0;
                s.TargetIsCasting = false;
                s.DistanceToTarget = 0;
            }

            // --- Combat Duration ---
            if (combatStartTime == DateTime.MinValue && bot.Player.IsInCombat)
            {
                combatStartTime = s.Timestamp;
            }
            else if (!bot.Player.IsInCombat)
            {
                combatStartTime = DateTime.MinValue;
            }
            s.CombatDurationSeconds = combatStartTime != DateTime.MinValue
                ? (s.Timestamp - combatStartTime).TotalSeconds
                : 0;

            // --- DPS / DTPS (Real Combat Log) ---
            s.IncomingDTPS = CalculateDps(incomingDamageQueue, s.Timestamp);
            s.OutgoingDPS = CalculateDps(outgoingDamageQueue, s.Timestamp);

            return s;
        }

        private (double current, double max) GetUnitPower(IWowUnit unit)
        {
            return unit.Class switch
            {
                WowClass.Warrior => ((double current, double max))(unit.Rage, unit.MaxRage),
                WowClass.Rogue => ((double current, double max))(unit.Energy, unit.MaxEnergy),
                WowClass.Deathknight => ((double current, double max))(unit.RunicPower, unit.MaxRunicPower),
                _ => ((double current, double max))(unit.Mana, unit.MaxMana),
            };
        }
    }
}
