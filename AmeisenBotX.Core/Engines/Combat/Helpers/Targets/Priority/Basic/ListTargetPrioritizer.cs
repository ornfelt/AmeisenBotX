using AmeisenBotX.Wow.Objects;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Priority.Basic
{
    /// <summary>
    /// Represents a target prioritizer that uses a List to prioritize items.
    /// </summary>
    public class ListTargetPrioritizer : ITargetPrioritizer
    {
        /// <summary>
        /// Initializes a new instance of the ListTargetPrioritizer class.
        /// </summary>
        /// <param name="priorityDisplayIds">An optional collection of integer priority display IDs.</param>
        public ListTargetPrioritizer(IEnumerable<int> priorityDisplayIds = null)
        {
            PriorityDisplayIds = priorityDisplayIds ?? new List<int>();
        }

        /// <summary>
        /// Gets or sets the collection of priority display IDs.
        /// </summary>
        /// <returns>An enumerable collection of integer values representing the priority display IDs.</returns>
        public IEnumerable<int> PriorityDisplayIds { get; set; }

        /// <summary>
        /// Checks if the provided WoW unit has priority based on its display ID.
        /// </summary>
        /// <param name="unit">The WoW unit to check for priority.</param>
        /// <returns>True if the unit has priority, false otherwise.</returns>
        public bool HasPriority(IWowUnit unit)
        {
            return PriorityDisplayIds.Any(e => e == unit.DisplayId);
        }
    }
}