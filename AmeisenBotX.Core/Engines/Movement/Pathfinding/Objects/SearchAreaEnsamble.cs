using AmeisenBotX.Common.Math;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Movement.Pathfinding.Objects
{
    /// <summary>
    /// Initializes a new instance of the SearchAreaEnsemble class.
    /// </summary>
    internal class SearchAreaEnsamble
    {
        /// <summary>
        /// Initializes a new instance of the SearchAreaEnsemble class.
        /// </summary>
        /// <param name="searchAreas">A list of search areas represented by a list of Vector3 positions.</param>
        public SearchAreaEnsamble(List<List<Vector3>> searchAreas)
        {
            CurrentSearchArea = 0;
            foreach (List<Vector3> area in searchAreas)
            {
                Areas.Add(new(area));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the path has been aborted.
        /// </summary>
        private bool AbortedPath { get; set; } = true;

        /// <summary>
        /// Gets or sets the list of search areas.
        /// </summary>
        private List<SearchArea> Areas { get; } = new();

        /// <summary>
        /// Gets or sets the current search area.
        /// </summary>
        private int CurrentSearchArea { get; set; }

        /// <summary>
        /// Gets or sets the last search position in three-dimensional space.
        /// </summary>
        private Vector3 LastSearchPosition { get; set; } = Vector3.Zero;

        /// <summary>
        /// Returns the next position for the AmeisenBot to move to based on the provided bot object.
        /// If the path has not been aborted or the last searched position is Vector3.Zero,
        /// the position is retrieved from the internal method GetNextPositionInternal().
        /// The aborted path flag is set to false and the last searched position is returned.
        /// </summary>
        public Vector3 GetNextPosition(AmeisenBotInterfaces bot)
        {
            if (!AbortedPath || LastSearchPosition == Vector3.Zero)
            {
                LastSearchPosition = GetNextPositionInternal(bot);
            }

            AbortedPath = false;
            return LastSearchPosition;
        }

        /// <summary>
        /// Returns whether or not the path has been aborted.
        /// </summary>
        public bool HasAbortedPath()
        {
            return AbortedPath;
        }

        /// <summary>
        /// Checks whether the player is near the search area.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object.</param>
        /// <returns>True if the player is near the search area, otherwise false.</returns>
        public bool IsPlayerNearSearchArea(AmeisenBotInterfaces bot)
        {
            return Areas[CurrentSearchArea].ContainsPosition(bot.Objects.Player.Position)
                   || Areas[CurrentSearchArea].GetClosestVertexDistance(bot.Objects.Player.Position) <= 20.0;
        }

        /// <summary>
        /// Notifies that a detour has occurred.
        /// </summary>
        public void NotifyDetour()
        {
            AbortedPath = true;
        }

        /// <summary>
        /// Returns the next position for the AmeisenBot based on the current search area.
        /// If the current position is within the current search area, it retrieves the next search position.
        /// If the current search area is at its beginning, it updates the current search area to the next one in the list.
        /// Returns the closest entry position within the current search area if the current position is not within the current search area.
        /// </summary>
        private Vector3 GetNextPositionInternal(AmeisenBotInterfaces bot)
        {
            Vector3 currentPosition = bot.Objects.Player.Position;

            if (Areas[CurrentSearchArea].ContainsPosition(currentPosition))
            {
                Vector3 position = Areas[CurrentSearchArea].GetNextSearchPosition();

                if (Areas[CurrentSearchArea].IsAtTheBeginning())
                {
                    CurrentSearchArea = (CurrentSearchArea + 1) % Areas.Count;
                }

                return position;
            }

            return Areas[CurrentSearchArea].GetClosestEntry(bot);
        }
    }
}