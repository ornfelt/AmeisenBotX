using AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Priority;
using AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Validation;
using AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Validation.Basic;
using AmeisenBotX.Wow.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Logics
{
    /// <summary>
    /// Represents a basic logic for selecting targets.
    /// </summary>
    public abstract class BasicTargetSelectionLogic
    {
        /// <summary>
        /// Initializes a new instance of the BasicTargetSelectionLogic class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object.</param>
        public BasicTargetSelectionLogic(AmeisenBotInterfaces bot)
        {
            Bot = bot;
            TargetValidator = new(new IsValidAliveTargetValidator());
            TargetPrioritizer = new();
        }

        /// <summary>
        /// Gets or sets the collection of blacklisted targets.
        /// </summary>
        /// <returns>An IEnumerable collection of integers representing the blacklisted targets.</returns>
        public IEnumerable<int> BlacklistedTargets { get; set; }

        /// <summary>
        /// Gets or sets the instance of the AmeisenBotInterfaces that represents the bot.
        /// </summary>
        public AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets or sets the collection of priority targets that are integers.
        /// </summary>
        public IEnumerable<int> PriorityTargets { get; set; }

        /// <summary>
        /// Gets or sets the TargetPriorityManager, responsible for prioritizing targets.
        /// </summary>
        public TargetPriorityManager TargetPrioritizer { get; set; }

        /// <summary>
        /// Gets or sets the target validation manager.
        /// </summary>
        public TargetValidationManager TargetValidator { get; set; }

        /// <summary>
        /// Selects a target and retrieves a collection of objects that implement the IWowUnit interface.
        /// </summary>
        /// <param name="wowUnit">An enumerable collection of IWowUnit objects that will be populated with the selected target.</param>
        /// <returns>Returns true if a target is successfully selected; otherwise, false.</returns>
        public abstract bool SelectTarget(out IEnumerable<IWowUnit> wowUnit);

        /// <summary>
        /// Determines if the given WoW unit is a priority target by checking if the PriorityTargets collection is not null and contains the display ID of the WoW unit.
        /// </summary>
        protected bool IsPriorityTarget(IWowUnit wowUnit)
        {
            return PriorityTargets != null && PriorityTargets.Contains(wowUnit.DisplayId);
        }
    }
}