/// <summary>
/// Represents a quest objective that requires casting a vehicle spell.
/// </summary>
namespace AmeisenBotX.Core.Engines.Quest.Objects.Objectives
{
    /// <summary>
    /// Represents a delegate that defines the condition for casting a vehicle spell in a quest objective.
    /// </summary>
    public delegate bool CastVehicleSpellQuestObjectiveCondition();

    /// <summary>
    /// Represents a quest objective that requires casting a specific vehicle spell.
    /// </summary>
    public class CastVehicleSpellQuestObjective : IQuestObjective
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CastVehicleSpellQuestObjective"/> class.
        /// </summary>
        /// <param name="bot">The bot instance.</param>
        /// <param name="spellId">The ID of the spell.</param>
        /// <param name="condition">The condition for casting the vehicle spell.</param>
        public CastVehicleSpellQuestObjective(AmeisenBotInterfaces bot, int spellId, CastVehicleSpellQuestObjectiveCondition condition)
        {
            Bot = bot;
            SpellId = spellId;
            Condition = condition;
        }

        /// <summary>
        /// Gets a value indicating whether the progress is finished.
        /// </summary>
        public bool Finished => Progress == 100.0;

        /// <summary>
        /// Gets the progress value based on the condition.
        /// Returns 100.0 if the condition is true, otherwise returns 0.0.
        /// </summary>
        public double Progress => Condition() ? 100.0 : 0.0;

        /// <summary>
        /// Gets the instance of the Bot class that implements the AmeisenBotInterfaces.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets the condition for the CastVehicleSpellQuestObjective.
        /// </summary>
        private CastVehicleSpellQuestObjectiveCondition Condition { get; }

        /// <summary>
        /// Gets the SpellId property.
        /// </summary>
        private int SpellId { get; }

        /// <summary>
        /// Executes the code if the condition is met.
        /// Stops execution if the condition is not met or if the bot is currently casting a spell.
        /// Resets the bot's movement and stops the bot from click-to-move.
        /// Casts a spell specified by the given SpellId.
        /// </summary>
        public void Execute()
        {
            if (Finished || Bot.Objects.Vehicle.IsCasting) { return; }

            Bot.Movement.Reset();
            Bot.Wow.StopClickToMove();
            Bot.Wow.CastSpellById(SpellId);
        }
    }
}