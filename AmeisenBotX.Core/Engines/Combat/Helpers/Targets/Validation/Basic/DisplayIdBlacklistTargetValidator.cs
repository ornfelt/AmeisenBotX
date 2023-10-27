using AmeisenBotX.Wow.Objects;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Validation.Basic
{
    /// <summary>
    /// Gets or sets the collection of blacklisted integers.
    /// </summary>
    public class DisplayIdBlacklistTargetValidator : ITargetValidator
    {
        /// <summary>
        /// Initializes a new instance of the DisplayIdBlacklistTargetValidator class.
        /// </summary>
        /// <param name="blacklistedGuids">An optional collection of integers representing the blacklisted GUIDs.</param>
        public DisplayIdBlacklistTargetValidator(IEnumerable<int> blacklistedGuids = null)
        {
            Blacklist = blacklistedGuids ?? new List<int>();
        }

        ///<summary>
        /// Gets or sets the collection of blacklisted integers.
        ///</summary>
        public IEnumerable<int> Blacklist { get; set; }

        /// <summary>
        /// Checks if the given unit is valid by verifying if its display ID is not present in the blacklist.
        /// </summary>
        /// <param name="unit">The unit to be checked.</param>
        /// <returns>True if the unit is valid, false otherwise.</returns>
        public bool IsValid(IWowUnit unit)
        {
            return !Blacklist.Any(e => e == unit.DisplayId);
        }
    }
}