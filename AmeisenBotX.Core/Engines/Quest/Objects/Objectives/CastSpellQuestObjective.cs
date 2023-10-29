/// <summary>
/// This namespace contains classes and delegates related to the objectives of quests in the AmeisenBotX engine.
/// </summary>
namespace AmeisenBotX.Core.Engines.Quest.Objects.Objectives
{
    /// <summary>
    /// Delegate representing the condition for a cast spell quest objective.
    /// </summary>
    public delegate bool CastSpellQuestObjectiveCondition();

    /// <summary>
    /// Represents a quest objective that requires casting a specific spell.
    /// </summary>
    public class CastSpellQuestObjective : IQuestObjective
    {
        ///<summary>
        ///Constructor for CastSpellQuestObjective class.
        ///Initializes the object with the provided parameters: bot, spellId, condition.
        ///</summary>
        public CastSpellQuestObjective(AmeisenBotInterfaces bot, int spellId, CastSpellQuestObjectiveCondition condition)
        {
            Bot = bot;
            SpellId = spellId;
            Condition = condition;
        }

        /// <summary>
        /// Gets a value indicating whether the progress has reached 100%.
        /// </summary>
        public bool Finished => Progress == 100.0;

        /// <summary>
        /// Gets the progress as a percentage.
        /// </summary>
        public double Progress => Condition() ? 100.0 : 0.0;

        /// <summary>
        /// Gets or sets the <see cref="AmeisenBotInterfaces"/> object representing the bot.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        ///<summary>
        /// Gets the condition for the CastSpellQuestObjective.
        ///</summary>
        private CastSpellQuestObjectiveCondition Condition { get; }

        /// <summary>
        /// Gets the unique identifier for the spell.
        /// </summary>
        private int SpellId { get; }

        /// <summary>
        /// Executes the action.
        /// </summary>
        public void Execute()
        {
            if (Finished || Bot.Player.IsCasting) { return; }

            Bot.Movement.Reset();
            Bot.Wow.StopClickToMove();
            Bot.Wow.CastSpellById(SpellId);
        }
    }
}