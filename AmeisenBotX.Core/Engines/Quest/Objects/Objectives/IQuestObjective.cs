namespace AmeisenBotX.Core.Engines.Quest.Objects.Objectives
{
    /// <summary>
    /// Represents an interface for a quest objective.
    /// </summary>
    /// <remarks>
    /// This interface defines the properties and methods required for a quest objective.
    /// An objective is considered finished when the Finished property is true.
    /// The Progress property reflects the completion progress of the objective, represented as a double value between 0 and 1.
    /// The Execute method is responsible for executing the necessary actions to progress the objective.
    /// </remarks>
    public interface IQuestObjective
    {
        /// <summary>
        /// Gets or sets a value indicating whether the task is finished.
        /// </summary>
        bool Finished { get; }

        /// <summary>
        /// Gets the progress value.
        /// </summary>
        double Progress { get; }

        /// <summary>
        /// Executes the specified action.
        /// </summary>
        void Execute();
    }
}