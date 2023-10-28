using AmeisenBotX.Common.Math;

/// <summary>
/// Represents a node in an area.
/// </summary>
namespace AmeisenBotX.Core.Engines.Quest.Objects
{
    /// <summary>
    /// Represents a node in an area.
    /// </summary>
    public class AreaNode
    {
        /// <summary>
        /// Initializes a new instance of the AreaNode class with the specified position and radius.
        /// </summary>
        /// <param name="position">The position of the AreaNode.</param>
        /// <param name="radius">The radius of the AreaNode.</param>
        public AreaNode(Vector3 position, double radius)
        {
            Position = position;
            Radius = radius;
        }

        /// <summary>
        /// Gets or sets the position of the Vector3 object.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the radius of the circle.
        /// </summary>
        public double Radius { get; set; }
    }
}