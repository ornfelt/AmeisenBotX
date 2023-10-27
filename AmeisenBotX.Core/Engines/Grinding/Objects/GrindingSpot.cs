using AmeisenBotX.Common.Math;

namespace AmeisenBotX.Core.Engines.Grinding.Objects
{
    /// <summary>
    /// Initializes a new instance of the GrindingSpot class with the specified position, radius, minimum level, and maximum level.
    /// </summary>
    public class GrindingSpot
    {
        /// <summary>
        /// Initializes a new instance of the GrindingSpot class.
        /// </summary>
        public GrindingSpot()
        {
        }

        /// <summary>
        /// Initializes a new instance of the GrindingSpot class with the specified position, radius, minimum level, and maximum level.
        /// </summary>
        /// <param name="position">The position of the grinding spot.</param>
        /// <param name="radius">The radius of the grinding spot.</param>
        /// <param name="minLevel">The minimum level of the grinding spot.</param>
        /// <param name="maxLevel">The maximum level of the grinding spot.</param>
        public GrindingSpot(Vector3 position, float radius, int minLevel, int maxLevel)
        {
            Position = position;
            Radius = radius;
            MinLevel = minLevel;
            MaxLevel = maxLevel;
        }

        /// <summary>
        /// Gets or sets the maximum level value.
        /// </summary>
        public int MaxLevel { get; set; }

        /// <summary>
        /// Gets or sets the minimum level.
        /// </summary>
        public int MinLevel { get; set; }

        /// <summary>
        /// Gets or sets the position of the object in 3D space.
        /// </summary>
        public Vector3 Position { get; set; }

        ///<summary>
        /// Gets or sets the radius of the object.
        ///</summary>
        public float Radius { get; set; }
    }
}