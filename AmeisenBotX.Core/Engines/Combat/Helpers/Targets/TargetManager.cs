using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Logics;
using AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Logics.Dps;
using AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Logics.Heal;
using AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Logics.Tank;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Namespace containing helper classes for managing targets in combat.
/// </summary>
namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets
{
    /// <summary>
    /// Initializes a new instance of the TargetManager class with the specified parameters.
    /// </summary>
    public class TargetManager : ITargetProvider
    {
        /// <summary>
        /// Initializes a new instance of the TargetManager class with the specified parameters.
        /// </summary>
        public TargetManager(AmeisenBotInterfaces bot, WowRole role, TimeSpan minTargetSwitchTime)
        {
            TargetSelectionLogic = role switch
            {
                WowRole.Dps => new SimpleDpsTargetSelectionLogic(bot),
                WowRole.Tank => new SimpleTankTargetSelectionLogic(bot),
                WowRole.Heal => new SimpleHealTargetSelectionLogic(bot),
                _ => throw new NotImplementedException(),
            };

            TargetSwitchEvent = new(minTargetSwitchTime);
        }

        /// <summary>
        /// Initializes a new instance of the TargetManager class.
        /// </summary>
        /// <param name="targetSelectionLogic">The target selection logic.</param>
        /// <param name="minTargetSwitchTime">The minimum target switch time.</param>
        public TargetManager(BasicTargetSelectionLogic targetSelectionLogic, TimeSpan minTargetSwitchTime)
        {
            TargetSelectionLogic = targetSelectionLogic;
            TargetSwitchEvent = new(minTargetSwitchTime);
        }

        /// <summary>
        /// Gets or sets the list of blacklisted targets.
        /// </summary>
        /// <returns>An IEnumerable of integers representing the blacklisted targets.</returns>
        public IEnumerable<int> BlacklistedTargets
        {
            get => TargetSelectionLogic.TargetValidator.BlacklistTargetValidator.Blacklist;
            set => TargetSelectionLogic.TargetValidator.BlacklistTargetValidator.Blacklist = value;
        }

        /// <summary>
        /// Gets or sets the list of priority targets.
        /// </summary>
        public IEnumerable<int> PriorityTargets
        {
            get => TargetSelectionLogic.TargetPrioritizer.ListTargetPrioritizer.PriorityDisplayIds;
            set => TargetSelectionLogic.TargetPrioritizer.ListTargetPrioritizer.PriorityDisplayIds = value;
        }

        /// <summary>
        /// Gets or sets the possible targets for the WowUnit.
        /// </summary>
        /// <returns>An enumerable collection of WowUnit targets.</returns>
        private IEnumerable<IWowUnit> PossibleTargets { get; set; }

        /// <summary>
        /// Gets or sets the basic target selection logic used by the code.
        /// </summary>
        private BasicTargetSelectionLogic TargetSelectionLogic { get; }

        /// <summary>
        /// Gets or sets the TimegatedEvent for when the target switches.
        /// </summary>
        private TimegatedEvent TargetSwitchEvent { get; set; }

        /// <summary>
        /// Retrieves the possible targets for the current operation. 
        /// If the target switch event is triggered and a new target is selected,
        /// the list of possible targets is updated accordingly. If no target
        /// is selected, the possible targets are set to null.
        /// </summary>
        /// <param name="possibleTargets">The list of possible targets, if any.</param>
        /// <returns>True if there are possible targets, otherwise false.</returns>
        public bool Get(out IEnumerable<IWowUnit> possibleTargets)
        {
            if (TargetSwitchEvent.Run())
            {
                if (TargetSelectionLogic.SelectTarget(out IEnumerable<IWowUnit> newPossibleTargets))
                {
                    PossibleTargets = newPossibleTargets;
                }
                else
                {
                    PossibleTargets = null;
                }
            }

            possibleTargets = PossibleTargets;
            return PossibleTargets != null && PossibleTargets.Any();
        }
    }
}