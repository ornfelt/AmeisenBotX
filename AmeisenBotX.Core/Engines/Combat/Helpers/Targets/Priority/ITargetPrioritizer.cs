using AmeisenBotX.Wow.Objects;

/// <summary>
/// Determines if the specified World of Warcraft unit has priority.
/// </summary>
namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Priority
{
    /// <summary>
    /// Determines if the specified unit has priority.
    /// </summary>
    public interface ITargetPrioritizer
    {
        /// <summary>
        /// Determines if the specified World of Warcraft unit has priority.
        /// </summary>
        /// <param name="unit">The World of Warcraft unit to evaluate.</param>
        /// <returns>True if the unit has priority, false otherwise.</returns>
        bool HasPriority(IWowUnit unit);
    }
}