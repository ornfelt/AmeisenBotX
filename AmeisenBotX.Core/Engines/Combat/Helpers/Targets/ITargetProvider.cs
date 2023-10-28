using AmeisenBotX.Wow.Objects;
using System.Collections.Generic;

/// <summary>
/// Represents a provider for target information.
/// </summary>
namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets
{
    /// <summary>
    /// Represents a provider for target information.
    /// </summary>
    public interface ITargetProvider
    {
        /// <summary>
        /// Gets or sets the collection of blacklisted targets.
        /// </summary>
        IEnumerable<int> BlacklistedTargets { get; set; }

        /// <summary>
        /// Gets or sets the collection of priority targets represented by integers.
        /// </summary>
        IEnumerable<int> PriorityTargets { get; set; }

        /// <summary>
        /// Retrieves a collection of possible targets that implement the <see cref="IWowUnit"/> interface.
        /// </summary>
        /// <returns>A collection of possible targets.</returns>
        bool Get(out IEnumerable<IWowUnit> possibleTargets);
    }
}