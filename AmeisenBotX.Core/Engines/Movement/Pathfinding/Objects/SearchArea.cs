using AmeisenBotX.Common.Math;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Represents a specific area to be searched.
/// </summary>
namespace AmeisenBotX.Core.Engines.Movement.Pathfinding.Objects
{
    /// <summary>
    /// Represents a specific area to be searched.
    /// </summary>
    internal class SearchArea
    {
        /// <summary>
        /// Initializes a new instance of the SearchArea class with the given search area.
        /// </summary>
        /// <param name="searchArea">The list of Vector3 representing the search area.</param>
        public SearchArea(List<Vector3> searchArea)
        {
            Area = searchArea;
            CurrentSearchPathIndex = 0;
            CalculateSearchPath();
        }

        /// <summary>
        /// Gets or sets the list of Vector3 objects representing the area.
        /// </summary>
        private List<Vector3> Area { get; }

        /// <summary>
        /// Gets or sets the index of the current search path.
        /// </summary>
        private int CurrentSearchPathIndex { get; set; }

        /// <summary>
        /// Gets or sets the list of Vector3 elements representing the searched path.
        /// </summary>
        private List<Vector3> SearchPath { get; set; }

        /// <summary>
        /// Gets the visibility radius of an object.
        /// </summary>
        private float VisibilityRadius { get; } = 30.0f;

        /// <summary>
        /// Checks if the given position is inside the polygon defined by the Area points using the Ray Casting algorithm.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <returns>True if the position is inside the polygon, false otherwise.</returns>
        public bool ContainsPosition(Vector3 position)
        {
            if (Area.Count <= 1)
            {
                return false;
            }

            // Ray Casting algorithm
            float x = position.X;
            float y = position.Y;
            bool inside = false;

            for (int i = 0, j = Area.Count - 1; i < Area.Count; j = i++)
            {
                float xi = Area[i].X;
                float yi = Area[i].Y;
                float xj = Area[j].X;
                float yj = Area[j].Y;

                if (((yi > y) != (yj > y)) && (x < (xj - xi) * (y - yi) / (yj - yi) + xi))
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        /// <summary>
        /// Gets the closest entry point within the specified area for the given bot.
        /// </summary>
        /// <param name="bot">The bot for which to find the closest entry point.</param>
        /// <returns>
        /// The closest entry point within the specified area.
        /// </returns>
        public Vector3 GetClosestEntry(AmeisenBotInterfaces bot)
        {
            if (Area.Count == 1)
            {
                return Area[0];
            }

            Vector3 currentPosition = bot.Objects.Player.Position;

            // This is not optimal but fairly simple We ask for the path for every vertex. It could
            // be optimized by following the edges up or down and stop once the distance increased
            // in both directions. We dont ask for the Distance2D because we want to know the
            // movement path length

            List<double> distances = new();

            foreach (Vector3 vertex in Area)
            {
                double totalDistance = 0.0;
                IEnumerable<Vector3> path = bot.PathfindingHandler.GetPath((int)bot.Objects.MapId, currentPosition, vertex);

                if (path != null)
                {
                    Vector3 lastPosition = currentPosition;

                    foreach (Vector3 pathPosition in path)
                    {
                        totalDistance += pathPosition.GetDistance(lastPosition);
                        lastPosition = pathPosition;
                    }
                }

                distances.Add(totalDistance);
            }

            int minimumIndex = distances.IndexOf(distances.Min());
            Vector3 entryPosition = Area[minimumIndex];

            // The ContainsPoint function is sensible towards edges, therefore we will wiggle us
            // into the polygon
            if (!ContainsPosition(entryPosition))
            {
                {
                    Vector3 newEntryPosition = new(entryPosition);
                    newEntryPosition.X += VisibilityRadius / 2;
                    newEntryPosition.Y += VisibilityRadius / 2;

                    if (ContainsPosition(newEntryPosition))
                    {
                        return newEntryPosition;
                    }
                }

                {
                    Vector3 newEntryPosition = new(entryPosition);
                    newEntryPosition.X -= VisibilityRadius / 2;
                    newEntryPosition.Y -= VisibilityRadius / 2;

                    if (ContainsPosition(newEntryPosition))
                    {
                        return newEntryPosition;
                    }
                }

                {
                    Vector3 newEntryPosition = new(entryPosition);
                    newEntryPosition.X += VisibilityRadius / 2;
                    newEntryPosition.Y -= VisibilityRadius / 2;

                    if (ContainsPosition(newEntryPosition))
                    {
                        return newEntryPosition;
                    }
                }

                {
                    Vector3 newEntryPosition = new(entryPosition);
                    newEntryPosition.X -= VisibilityRadius / 2;
                    newEntryPosition.Y += VisibilityRadius / 2;

                    if (ContainsPosition(newEntryPosition))
                    {
                        return newEntryPosition;
                    }
                }
            }

            return entryPosition;
        }

        /// <summary>
        /// Calculates the closest distance between the given position and all vertices in the Area.
        /// </summary>
        /// <param name="position">The position to compare distances with.</param>
        /// <returns>The minimum distance between the given position and all vertices in the Area.</returns>
        public float GetClosestVertexDistance(Vector3 position)
        {
            return Area.Select(pos => pos.GetDistance(position)).Min();
        }

        /// <summary>
        /// Returns the next position to search in the search path.
        /// If there is only one position in the area or the search path is empty,
        /// the first position in the area is returned.
        /// Otherwise, the next position in the search path is returned and
        /// the current search path index is updated to the next index in a circular manner.
        /// </summary>
        public Vector3 GetNextSearchPosition()
        {
            if (Area.Count == 1 || SearchPath.Count == 0)
            {
                return Area[0];
            }

            Vector3 position = SearchPath[CurrentSearchPathIndex];
            CurrentSearchPathIndex = (CurrentSearchPathIndex + 1) % SearchPath.Count;
            return position;
        }

        /// <summary>
        /// Checks if the current search path index is at the beginning.
        /// </summary>
        public bool IsAtTheBeginning()
        {
            return CurrentSearchPathIndex == 0;
        }

        /// <summary>
        /// Calculates the search path for the given area.
        /// </summary>
        private void CalculateSearchPath()
        {
            if (Area.Count <= 1)
            {
                SearchPath = Area;
                return;
            }

            // This is not optimal but should be fast We raster the polygon with points apart 2x
            // VisibilityRadius We then remove all points that are not in the polygon Finally we
            // move to one point after another

            // First find top, right, left, right
            float top = Area[0].Y;
            float right = Area[0].X;
            float left = Area[0].X;
            float bottom = Area[0].Y;
            float maxZ = Area[0].Y;

            foreach (Vector3 vertex in Area)
            {
                top = MathF.Max(vertex.Y, top);
                right = MathF.Max(vertex.X, right);
                left = MathF.Min(vertex.X, left);
                bottom = MathF.Min(vertex.Y, bottom);
                maxZ = MathF.Max(vertex.Z, maxZ);
            }

            // Raster the rectangle and add fitting points
            int stepsTopToBottom = (int)MathF.Ceiling(MathF.Abs(top - bottom) / VisibilityRadius);
            int stepsLeftToRight = (int)MathF.Ceiling(MathF.Abs(left - right) / VisibilityRadius);

            float leftStart = left - VisibilityRadius / 2;
            float topStart = top + VisibilityRadius / 2;

            bool directionToggle = false;
            List<Vector3> newSearchPath = new();

            for (int y = 0; y < stepsTopToBottom - 1; ++y)
            {
                topStart += VisibilityRadius;

                for (int x = 0; x < stepsLeftToRight - 1; ++x)
                {
                    if (directionToggle)
                    {
                        leftStart -= VisibilityRadius;
                    }
                    else
                    {
                        leftStart += VisibilityRadius;
                    }

                    Vector3 newVertex = new(leftStart, topStart, maxZ);

                    if (ContainsPosition(newVertex))
                    {
                        newSearchPath.Add(newVertex);
                    }
                }

                directionToggle = !directionToggle;
            }

            SearchPath = newSearchPath;
        }
    }
}