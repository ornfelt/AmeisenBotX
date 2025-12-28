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

        public double WaypointCheckThreshold { get; set; } = 2.5;

        public double WaypointCheckThresholdMounted { get; set; } = 5.0;

        // ===== New realistic movement settings =====





        /// <summary>
        /// Velocity damping factor (0.0 to 1.0) applied each frame to reduce oscillation.
        /// Lower values = more damping = smoother but less responsive movement.
        /// 0.90-0.95 recommended for realistic WoW 3.3.5a movement.
        /// </summary>
        public float VelocityDamping { get; set; } = 0.92f;



        /// <summary>
        /// Minimum milliseconds between CTM updates.
        /// Prevents sending hundreds of commands per second.
        /// </summary>
        public int MinUpdateIntervalMs { get; set; } = 200;



        // ===== Unified Arrival Thresholds =====

        /// <summary>
        /// 3D distance at which BasicVehicle considers the target reached.
        /// Used for close-range arrival detection.
        /// </summary>
        public float ArrivalThreshold3D { get; set; } = 1.0f;

        /// <summary>
        /// 2D distance (ignoring height) at which BasicVehicle considers target reached,
        /// combined with HeightToleranceForArrival.
        /// </summary>
        public float ArrivalThreshold2D { get; set; } = 0.8f;

        /// <summary>
        /// Maximum height difference to consider arrived when using 2D distance check.
        /// </summary>
        public float HeightToleranceForArrival { get; set; } = 2.0f;



        /// <summary>
        /// Distance at which BasicVehicle starts slowing down (Arrive behavior activation).
        /// </summary>
        public float ArriveSlowdownRadius { get; set; } = 4.0f;

        /// <summary>
        /// Distance at which separation force is disabled to prevent jitter at destination.
        /// </summary>
        public float SeparationDisableDistance { get; set; } = 4.0f;
    }
}
