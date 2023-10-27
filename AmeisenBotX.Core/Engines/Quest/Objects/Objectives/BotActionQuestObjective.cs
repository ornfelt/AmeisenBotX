using System;

namespace AmeisenBotX.Core.Engines.Quest.Objects.Objectives
{
    /// <summary>
    /// Represents a quest objective that requires a specific bot action to be completed.
    /// </summary>
    public class BotActionQuestObjective : IQuestObjective
    {
        /// <summary>
        /// Initializes a new instance of the BotActionQuestObjective class with the specified action.
        /// </summary>
        /// <param name="action">The action associated with the quest objective.</param>
        public BotActionQuestObjective(Action action)
        {
            Action = action;
        }

        /// <summary>
        /// Gets the Action property.
        /// </summary>
        public Action Action { get; }

        /// <summary>
        /// Gets a value indicating whether the task is finished.
        /// </summary>
        public bool Finished => Progress == 100.0;

        /// <summary>
        /// Gets or sets the progress as a double value.
        /// </summary>
        public double Progress { get; set; }

        /// <summary>
        /// Executes the code and updates the progress to 100.0.
        /// </summary>
        public void Execute()
        {
            if (Finished) { return; }

            Action();

            Progress = 100.0;
        }
    }
}