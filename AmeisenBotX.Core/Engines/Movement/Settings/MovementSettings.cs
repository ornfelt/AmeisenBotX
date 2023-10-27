namespace AmeisenBotX.Core.Engines.Movement.Settings
{
    /// <summary>
    /// Defines the configuration options for movement behavior.
    /// </summary>
    public class MovementSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether the distance moved jump check is enabled.
        /// </summary>
        public bool EnableDistanceMovedJumpCheck { get; set; } = true;

        /// <summary>
        /// Gets or sets the maximum steering value.
        /// </summary>
        public float MaxSteering { get; set; } = 3.0f;

        /// <summary>
        /// Gets or sets the maximum steering value in combat situations.
        /// </summary>
        public float MaxSteeringCombat { get; set; } = 10.0f;

        /// <summary>
        /// Gets or sets the maximum velocity.
        /// </summary>
        public float MaxVelocity { get; set; } = 5.0f;

        /// <summary>
        /// The distance at which objects should separate from each other.
        /// </summary>
        public float SeperationDistance { get; set; } = 2.0f;

        /// <summary>
        /// Gets or sets the threshold value used for waypoint checks.
        /// </summary>
        /// <value>
        /// The threshold value used for waypoint checks. The default value is 1.7.
        /// </value>
        public double WaypointCheckThreshold { get; set; } = 1.7;

        /// <summary>
        /// Gets or sets the waypoint check threshold mounted.
        /// </summary>
        /// <value>The waypoint check threshold mounted.</value>
        public double WaypointCheckThresholdMounted { get; set; } = 3.5;
    }
}