namespace AmeisenBotX.Core.Engines.Movement.Pathfinding.Enums
{
    /// <summary>
    /// Enumeration of message types.
    /// </summary>
    public enum EMessageType
    {
        /// <summary>
        /// Represents the path of a file or folder.
        /// </summary>
        PATH,
        /// <summary>
        /// Moves the object along the surface.
        /// </summary>
        MOVE_ALONG_SURFACE,
        /// <summary>
        /// Represents a randomly generated point in a two-dimensional space.
        /// </summary>
        RANDOM_POINT,
        /// <summary>
        /// Generates a random point around a given coordinate.
        /// </summary>
        RANDOM_POINT_AROUND,
    }
}