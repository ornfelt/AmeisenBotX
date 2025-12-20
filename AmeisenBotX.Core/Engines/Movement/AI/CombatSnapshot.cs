using System;

namespace AmeisenBotX.Core.Engines.Movement.AI
{
    /// <summary>
    /// A snapshot of the combat situation used for learning win probabilities.
    /// Factors strictly into "Self" vs "Group" aspects.
    /// </summary>
    public class CombatSnapshot
    {
        public DateTime Timestamp { get; set; }
        public bool IsInstance { get; set; }
        public bool IsPvp { get; set; }

        // --- GROUP FACTORS ---
        public int PartyCount { get; set; }
        public double TotalPartyCurrentHealth { get; set; }
        public double TotalPartyMaxHealth { get; set; }
        public int DeadToAliveRatio { get; set; }
        public int TankCount { get; set; }
        public int HealerCount { get; set; }

        // --- ENEMY FACTORS ---
        public int EnemyCount { get; set; }
        public int EnemyHealerCount { get; set; } // NEW
        public int EnemyTankCount { get; set; }   // NEW (High armor/HP?)
        public double TotalEnemyCurrentHealth { get; set; }
        public double TotalEnemyMaxHealth { get; set; }
        public float AvgLevelDelta { get; set; }
        public int EliteCount { get; set; }

        // --- TARGET FACTORS (Focus Focus) ---
        public double TargetHealth { get; set; }
        public double TargetMaxHealth { get; set; }
        public double TargetPower { get; set; }
        public double TargetMaxPower { get; set; }
        public int TargetLevelDelta { get; set; }
        public bool TargetIsPlayer { get; set; }
        public int TargetCreatureType { get; set; } // Cast from WowCreatureType
        public bool TargetIsCasting { get; set; }   // For interrupt detection
        public float DistanceToTarget { get; set; } // Range in yards
        public double CombatDurationSeconds { get; set; } // How long we've been fighting

        public bool Won { get; set; }
        public double WinQuality { get; set; } // Continuous 0.0-1.0 based on fight difficulty
        public int ResultStrategy { get; set; } // For Training (0=Flee, 1=Survival, 2=Burst, 3=Standard, 4=Farm)

        // --- SELF FACTORS ---
        public double MyHealth { get; set; }
        public double MyMaxHealth { get; set; }
        public double MyPower { get; set; }
        public double MyMaxPower { get; set; }
        public int EnemiesTargetingMe { get; set; }
        public float IncomingDTPS { get; set; }
        public float OutgoingDPS { get; set; }

        // --- OUTCOME ---
        public bool Win { get; set; }
    }
}
