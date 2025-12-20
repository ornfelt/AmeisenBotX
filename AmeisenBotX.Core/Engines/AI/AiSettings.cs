namespace AmeisenBotX.Core.Engines.AI
{
    /// <summary>
    /// Consolidated AI settings for all bot AI features.
    /// ALL features are disabled by default enable manually in config for testing.
    /// </summary>
    public class AiSettings
    {
        // ========== MASTER SWITCH ==========

        /// <summary>
        /// Master switch for all AI features.
        /// Must be true for any AI feature to work.
        /// </summary>
        public bool Enabled { get; set; } = false;

        // ========== COMBAT AI ==========

        /// <summary>
        /// Enable AI-driven combat movement.
        /// Uses threat awareness, enemy casting, and tactical positioning.
        /// </summary>
        public bool CombatMovementEnabled { get; set; } = false;

        /// <summary>
        /// How aggressively to kite when low on health (0-1).
        /// Higher = starts kiting earlier.
        /// </summary>
        public float KitingAggressiveness { get; set; } = 0.3f;

        /// <summary>
        /// Health percentage below which to consider fleeing.
        /// </summary>
        public float FleeHealthThreshold { get; set; } = 20f;

        /// <summary>
        /// Prefer positioning behind targets (backstab, etc).
        /// </summary>
        public bool PreferBehindTarget { get; set; } = true;

        /// <summary>
        /// Avoid standing in front of enemies (cleave attacks).
        /// </summary>
        public bool AvoidFrontalCone { get; set; } = true;

        /// <summary>
        /// React to enemy spell casts by moving.
        /// </summary>
        public bool ReactToEnemyCasts { get; set; } = true;

        /// <summary>
        /// Win Probability Score below which the bot will switch to defensive/flee mode.
        /// Scale 0.0 - 1.0. Default 0.3 (30%).
        /// </summary>
        public float FleeScoreThreshold { get; set; } = 0.3f;

        /// <summary>
        /// Win Probability Score above which the bot will resume aggressive combat.
        /// Hysteresis to prevent flipping. Default 0.5 (50%).
        /// </summary>
        public float ReengageScoreThreshold { get; set; } = 0.5f;

        // ========== FOLLOW AI (on-the-fly learning) ==========

        /// <summary>
        /// Enable on-the-fly imitation learning for following.
        /// Bots learn from master's movements in real-time.
        /// </summary>
        public bool FollowLearningEnabled { get; set; } = false;

        /// <summary>
        /// Minimum observations required before AI starts making predictions.
        /// Default: 50 (about 10 seconds of movement data).
        /// </summary>
        public int MinObservationsToLearn { get; set; } = 50;

        /// <summary>
        /// How much to anticipate where the leader is going (0-1).
        /// </summary>
        public float LeaderAnticipationFactor { get; set; } = 0.3f;

        /// <summary>
        /// Minimum distance bots keep from each other.
        /// </summary>
        public float MinPlayerSeparation { get; set; } = 2.5f;

        // ========== DATA RECORDING ==========

        /// <summary>
        /// Enable movement recording for offline training.
        /// </summary>
        public bool RecordingEnabled { get; set; } = false;

        /// <summary>
        /// Path to save recorded movement data.
        /// </summary>
        public string RecordingPath { get; set; } = string.Empty;
    }
}
