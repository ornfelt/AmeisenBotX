using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;

namespace AmeisenBotX.Core.Engines.Combat.Helpers.Targets.Validation.Basic
{
    /// <summary>
    /// Represents a target validator that checks if a target is attackable.
    /// </summary>
    public class IsAttackableTargetValidator : ITargetValidator
    {
        /// <summary>
        /// Initializes a new instance of the IsAttackableTargetValidator class.
        /// </summary>
        /// <param name="bot">The bot instance to be used for validation.</param>
        public IsAttackableTargetValidator(AmeisenBotInterfaces bot)
        {
            Bot = bot;
        }

        /// <summary>
        /// Gets or sets the instance of the AmeisenBotInterfaces associated with the private Bot property.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Determines if the given unit is valid based on the reaction of the bot's player towards that unit.
        /// </summary>
        /// <param name="unit">The unit to check for validity.</param>
        /// <returns>True if the unit's reaction is hostile or neutral, otherwise false.</returns>
        public bool IsValid(IWowUnit unit)
        {
            return Bot.Db.GetReaction(Bot.Player, unit)
                is WowUnitReaction.Hostile
                or WowUnitReaction.Neutral;
        }
    }
}