using AmeisenBotX.Common.Math;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Movement.Pathfinding
{
    /// <summary>
    /// Interface for a pathfinding handler.
    /// </summary>
    public interface IPathfindingHandler
    {
        /// <summary>
        /// Retrieves the path between the specified origin and target
        /// coordinates on the map with the provided mapId.
        /// </summary>
        /// <param name="mapId">The unique identifier of the map</param>
        /// <param name="origin">The starting position of the path</param>
        /// <param name="target">The destination position of the path</param>
        /// <returns>An IEnumerable collection of Vector3 containing the points 
        /// that make up the path from the origin to the target</returns>
        IEnumerable<Vector3> GetPath(int mapId, Vector3 origin, Vector3 target);

        /// <summary>
        /// Generates a random point within the specified map identified by mapId.
        /// </summary>
        /// <param name="mapId">The identifier of the map to generate the random point within.</param>
        /// <returns>A Vector3 representing the random point within the specified map identified by mapId.</returns>
        Vector3 GetRandomPoint(int mapId);

        /// <summary>
        /// Generates a random point around the specified origin within the given maximum radius.
        /// </summary>
        /// <param name="mapId">The identifier of the map where the point will be generated.</param>
        /// <param name="origin">The center point from which the random point will be generated.</param>
        /// <param name="maxRadius">The maximum distance from the origin that the generated point can be.</param>
        /// <returns>A Vector3 representing a random point within the specified radius around the given origin.</returns>
        Vector3 GetRandomPointAround(int mapId, Vector3 origin, float maxRadius);

        /// <summary>
        /// Moves the object along the surface defined by the given map ID from the specified origin point
        /// towards the specified target point and returns the resulting Vector3 position.
        /// </summary>
        /// <param name="mapId">The identifier of the map containing the surface.</param>
        /// <param name="origin">The starting position of the object.</param>
        /// <param name="target">The target position to move towards.</param>
        /// <returns>The resulting position after moving along the surface towards the target.</returns>
        Vector3 MoveAlongSurface(int mapId, Vector3 origin, Vector3 target);

        /// <summary>
        /// Stops the current process.
        /// </summary>
        void Stop();
    }
}