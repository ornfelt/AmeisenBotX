using AmeisenBotX.Core.Engines.Quest.Objects.Objectives;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Quest.Objects.Quests
{
    /// <summary>
    /// Gets or sets the value indicating whether the acceptance is true.
    /// </summary>
    internal class GrindingBotQuest : IBotQuest
    {
        /// <summary>
        /// Initializes a new instance of the GrindingBotQuest class with the specified name and objectives.
        /// </summary>
        /// <param name="name">The name of the quest.</param>
        /// <param name="objectives">The objectives of the quest.</param>
        public GrindingBotQuest(string name, List<IQuestObjective> objectives)
        {
            Name = name;
            Objectives = objectives;
        }

        /// <summary>
        /// Gets or sets the value indicating whether the acceptance is true.
        /// </summary>
        public bool Accepted => true;

        /// <summary>
        /// Gets a value indicating whether all objectives are finished.
        /// </summary>
        public bool Finished => Objectives != null && Objectives.All(e => e.Finished);

        /// <summary>
        /// Gets or sets the Id property.
        /// </summary>
        public int Id => -1;

        /// <summary>
        /// Gets the name of the object.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the list of quest objectives.
        /// </summary>
        /// <returns>A list of <see cref="IQuestObjective"/>.</returns>
        public List<IQuestObjective> Objectives { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the action has been finished and returned.
        /// </summary>
        public bool Returned => Finished;

        /// <summary>
        /// This method is used to indicate that the user has accepted a quest.
        /// </summary>
        public void AcceptQuest()
        {
        }

        /// <summary>
        /// Completes the quest.
        /// </summary>
        /// <returns>Returns false.</returns>
        public bool CompleteQuest()
        {
            return false;
        }

        /// <summary>
        /// Executes the first unfinished objective in the list of objectives.
        /// </summary>
        public void Execute()
        {
            Objectives.FirstOrDefault(e => !e.Finished)?.Execute();
        }
    }
}