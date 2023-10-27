using AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Priority.Basic;
using AmeisenBotX.Wow.Objects;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Priority
{
    public class TargetPriorityManager : ITargetPrioritizer
    {
        /// <summary>
        /// Initializes a new instance of the TargetPriorityManager class.
        /// </summary>
        public TargetPriorityManager()
        {
            Prioritizers = new();
            ListTargetPrioritizer = new();
        }

        /// <summary>
        /// Initializes a new instance of the TargetPriorityManager class.
        /// </summary>
        /// <param name="validator">The target prioritizer used for validation.</param>
        public TargetPriorityManager(ITargetPrioritizer validator)
        {
            Prioritizers = new() { validator };
            ListTargetPrioritizer = new();
        }

        /// <summary>
        /// Initializes a new instance of the TargetPriorityManager class with the specified validators.
        /// </summary>
        public TargetPriorityManager(IEnumerable<ITargetPrioritizer> validators)
        {
            Prioritizers = new(validators);
            ListTargetPrioritizer = new();
        }

        /// <summary>
        /// Gets or sets the ListTargetPrioritizer property.
        /// </summary>
        public ListTargetPrioritizer ListTargetPrioritizer { get; }

        /// <summary>
        /// Gets or sets the list of prioritizers for targeting.
        /// </summary>
        public List<ITargetPrioritizer> Prioritizers { get; }

        /// <summary>
        /// Adds an ITargetPrioritizer to the list of prioritizers.
        /// </summary>
        public void Add(ITargetPrioritizer prioritizer)
        {
            Prioritizers.Add(prioritizer);
        }

        /// <summary>
        /// Checks if the specified World of Warcraft unit has priority based on prioritizers.
        /// </summary>
        /// <param name="unit">The World of Warcraft unit to check.</param>
        /// <returns>True if the unit has priority, otherwise false.</returns>
        public bool HasPriority(IWowUnit unit)
        {
            return Prioritizers.Any(e => e.HasPriority(unit));
        }
    }
}