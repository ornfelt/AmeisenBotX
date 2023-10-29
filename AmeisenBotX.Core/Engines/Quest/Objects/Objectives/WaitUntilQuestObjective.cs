/// <summary>
/// Represents a quest objective that waits until a specific condition is fulfilled before considering it completed.
/// </summary>
namespace AmeisenBotX.Core.Engines.Quest.Objects.Objectives
{
    /// <summary>
    /// Delegate representing a condition that needs to be fulfilled before the quest objective is considered completed.
    /// The delegate returns a boolean value indicating whether the condition is currently met or not.
    /// </summary>
    public delegate bool WaitUntilQuestObjectiveCondition();

    /// <summary>
    /// Represents a quest objective that requires waiting until a specified condition is met.
    /// </summary>
    public class WaitUntilQuestObjective : IQuestObjective
    {
        /// <summary>
        /// Initializes a new instance of the WaitUntilQuestObjective class.
        /// </summary>
        /// <param name="condition">The condition to wait for.</param>
        public WaitUntilQuestObjective(WaitUntilQuestObjectiveCondition condition)
        {
            Condition = condition;
        }

        /// <summary>
        /// Gets the wait until quest objective condition.
        /// </summary>
        public WaitUntilQuestObjectiveCondition Condition { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the task is finished or not.
        /// </summary>
        public bool Finished { get; set; }

        /// <summary>
        /// Gets the progress as a double value.
        /// If the condition is true, the progress is 100.0.
        /// If the condition is false, the progress is 0.0.
        /// </summary>
        public double Progress => Condition() ? 100.0 : 0.0;

        /// <summary>
        /// Executes the command, setting the Finished flag to true if the command is already finished or the progress is 100.0.
        /// </summary>
        public void Execute()
        {
            if (Finished || Progress == 100.0)
            {
                Finished = true;
                return;
            }
        }
    }
}