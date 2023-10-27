using AmeisenBotX.Wow.Objects;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Logics.Heal
{
    public class SimpleHealTargetSelectionLogic : BasicTargetSelectionLogic
    {
        /// <summary>
        /// Initializes a new instance of the SimpleHealTargetSelectionLogic class.
        /// </summary>
        /// <param name="bot">The bot instance.</param>
        public SimpleHealTargetSelectionLogic(AmeisenBotInterfaces bot) : base(bot)
        {
        }

        /// <summary>
        /// Selects a target for healing and assigns it to the out parameter <paramref name="possibleTargets"/>.
        /// </summary>
        /// <param name="possibleTargets">The collection of healable units that are potential targets for healing.</param>
        /// <returns>True if a target is selected, false otherwise.</returns>
        public override bool SelectTarget(out IEnumerable<IWowUnit> possibleTargets)
        {
            List<IWowUnit> healableUnits = new(Bot.Objects.Partymembers)
            {
                Bot.Player
            };

            // healableUnits.AddRange(Bot.ObjectManager.PartyPets);

            possibleTargets = healableUnits
                .Where(e => TargetValidator.IsValid(e) && e.Health > 1 && e.Health < e.MaxHealth)
                .OrderByDescending(e => e.Type)
                .ThenByDescending(e => e.MaxHealth - e.Health);

            return possibleTargets.Any();
        }
    }
}