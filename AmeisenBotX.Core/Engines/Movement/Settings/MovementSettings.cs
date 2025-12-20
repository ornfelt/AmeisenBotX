namespace AmeisenBotX.Core.Engines.Movement.Settings
{
    public class MovementSettings
    {
        public bool EnableDistanceMovedJumpCheck { get; set; } = true;

        /// <summary>
        /// Enable random jumps while moving to appear more human-like.
        /// </summary>
        public bool EnableRandomJumps { get; set; } = true;

        /// <summary>
        /// Chance (0.0 to 1.0) to jump during each movement update.
        /// Default 0.005 = 0.5% chance per update, roughly 1 jump every ~30 seconds of movement.
        /// </summary>
        public float RandomJumpChance { get; set; } = 0.005f;

        /// <summary>
        /// Enable slight randomization of turn speed and arrival distance.
        /// </summary>
        public bool EnableMovementVariation { get; set; } = true;

        /// <summary>
        /// Base turn speed for ClickToMove (default 20.9).
        /// </summary>
        public float BaseTurnSpeed { get; set; } = 20.9f;

        /// <summary>
        /// Maximum variation (+/-) applied to turn speed.
        /// </summary>
        public float TurnSpeedVariation { get; set; } = 2.0f;

        /// <summary>
        /// Base arrival distance for ClickToMove.
        /// </summary>
        public float BaseArrivalDistance { get; set; } = 0.25f;

        /// <summary>
        /// Maximum variation (+) applied to arrival distance.
        /// </summary>
        public float ArrivalDistanceVariation { get; set; } = 0.15f;

        public float MaxSteering { get; set; } = 3.0f;

        public float MaxSteeringCombat { get; set; } = 10.0f;

        public float MaxVelocity { get; set; } = 5.0f;

        public float SeperationDistance { get; set; } = 2.0f;

        public double WaypointCheckThreshold { get; set; } = 1.7;

        public double WaypointCheckThresholdMounted { get; set; } = 3.5;

        // ===== New realistic movement settings =====

        /// <summary>
        /// Distance (in yards) at which to switch from steering behaviors to direct ClickToMove.
        /// Prevents spiral/oscillation when approaching waypoints.
        /// </summary>
        public float DirectMoveThreshold { get; set; } = 8.0f;

        /// <summary>
        /// Slowdown radius for the Seek behavior. When within this distance of target,
        /// movement velocity is gradually reduced to prevent overshooting.
        /// </summary>
        public float SlowdownRadius { get; set; } = 6.0f;

        /// <summary>
        /// Velocity damping factor (0.0 to 1.0) applied each frame to reduce oscillation.
        /// Lower values = more damping = smoother but less responsive movement.
        /// 0.90-0.95 recommended for realistic WoW 3.3.5a movement.
        /// NOTE: Currently unused in new CTM-based movement system.
        /// </summary>
        public float VelocityDamping { get; set; } = 0.92f;

        /// <summary>
        /// Minimum distance to destination before we send a CTM update.
        /// Prevents micro-movements that confuse WoW's movement system.
        /// Higher values mean nodes are considered "passed" sooner.
        /// </summary>
        public float MinUpdateDistance { get; set; } = 2.5f;

        /// <summary>
        /// Minimum change in destination position to trigger a CTM update.
        /// Prevents spam-clicking when target barely moves.
        /// Lower values = more responsive to waypoint changes.
        /// </summary>
        public float SignificantChange { get; set; } = 1.5f;

        /// <summary>
        /// Minimum milliseconds between CTM updates.
        /// Prevents sending hundreds of commands per second.
        /// </summary>
        public int MinUpdateIntervalMs { get; set; } = 200;

        /// <summary>
        /// Minimum directional change (in degrees) to trigger update.
        /// Allows updates when direction changes even if distance hasn't.
        /// </summary>
        public float DirectionalChangeThreshold { get; set; } = 30.0f;

        /// <summary>
        /// How far ahead (in seconds) to predict target movement for Pursuit/Evade.
        /// </summary>
        public float PredictionTime { get; set; } = 1.0f;

        /// <summary>
        /// Estimated running speed of targets for prediction (yards/second).
        /// WoW running speed is approximately 7 yards/sec.
        /// </summary>
        public float TargetVelocityEstimate { get; set; } = 7.0f;

        /// <summary>
        /// Whether to use direct movement when close to waypoint (prevents spiral behavior).
        /// </summary>
        public bool UseDirectMoveWhenClose { get; set; } = true;
    }
}