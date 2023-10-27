using AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Priority.Basic;
using AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Priority.Special;
using AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Validation.Basic;
using AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Validation.Special;
using AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Validation.Util;
using AmeisenBotX.Wow.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Logics.Dps
{
    /// <summary>
    /// Initializes a new instance of the SimpleDpsTargetSelectionLogic class with the specified bot.
    /// Sets up the target validators and prioritizers for selecting the target.
    /// </summary>
    /// <param name="bot">The bot used for validating and prioritizing targets.</param>
    public class SimpleDpsTargetSelectionLogic : BasicTargetSelectionLogic
    {
        /// <summary>
        /// Initializes a new instance of the SimpleDpsTargetSelectionLogic class with the specified bot.
        /// Sets up the target validators and prioritizers for selecting the target.
        /// </summary>
        /// <param name="bot">The bot used for validating and prioritizing targets.</param>
        public SimpleDpsTargetSelectionLogic(AmeisenBotInterfaces bot) : base(bot)
        {
            TargetValidator.Add(new IsAttackableTargetValidator(bot));
            TargetValidator.Add(new IsInCombatTargetValidator());
            TargetValidator.Add(new IsThreatTargetValidator(bot));
            TargetValidator.Add(new DungeonTargetValidator(bot));
            TargetValidator.Add(new CachedTargetValidator(new IsReachableTargetValidator(bot), TimeSpan.FromSeconds(4)));

            TargetPrioritizer.Add(new ListTargetPrioritizer());
            TargetPrioritizer.Add(new DungeonTargetPrioritizer(bot));
        }

        /// <summary>
        /// Selects a target for the bot by filtering and ordering a collection of possible targets based on various criteria.
        /// Returns whether a target was successfully selected.
        /// </summary>
        public override bool SelectTarget(out IEnumerable<IWowUnit> possibleTargets)
        {
            possibleTargets = Bot.Objects.All.OfType<IWowUnit>()
                .Where(e => TargetValidator.IsValid(e))
                .OrderByDescending(e => IsPriorityTarget(e))
                .ThenByDescending(e => e.Type)
                .ThenBy(e => e.DistanceTo(Bot.Player));

            return possibleTargets != null && possibleTargets.Any();
        }
    }
}