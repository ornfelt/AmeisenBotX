using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Quest.Objects.Objectives
{
    /// <summary>
    /// Initializes a new instance of the QuestObjectiveChain class with the provided list of quest objectives.
    /// </summary>
    public class QuestObjectiveChain : IQuestObjective
    {
        /// <summary>
        /// Initializes a new instance of the QuestObjectiveChain class with the provided list of quest objectives.
        /// </summary>
        /// <param name="questObjectives">The list of quest objectives to be set for the QuestObjectiveChain.</param>
        public QuestObjectiveChain(List<IQuestObjective> questObjectives)
        {
            QuestObjectives = questObjectives;
        }

        /// <summary>
        /// Gets a value indicating whether the task is finished by checking if the progress is equal to 100.0.
        /// </summary>
        public bool Finished => Progress == 100.0;

        /// <summary>
        /// Calculates and returns the current progress of the quest as a percentage.
        /// </summary>
        public double Progress => QuestObjectives.Count(e => QuestObjectives.IndexOf(e) <= AlreadyCompletedIndex || e.Finished) / (double)QuestObjectives.Count * 100.0;

        ///<summary>
        ///Gets or sets the list of quest objectives.
        ///</summary>
        public List<IQuestObjective> QuestObjectives { get; }

        /// <summary>
        /// Returns the index of the last completed quest objective in the list of quest objectives.
        /// </summary>
        private int AlreadyCompletedIndex
        {
            get
            {
                IQuestObjective questObjective = QuestObjectives.LastOrDefault(e => e.Finished);
                return QuestObjectives.IndexOf(questObjective);
            }
        }

        /// <summary>
        /// Executes the first unfinished quest objective, starting from the first one after the AlreadyCompletedIndex.
        /// </summary>
        public void Execute()
        {
            QuestObjectives.FirstOrDefault(e => QuestObjectives.IndexOf(e) > AlreadyCompletedIndex && !e.Finished)?.Execute();
        }
    }
}