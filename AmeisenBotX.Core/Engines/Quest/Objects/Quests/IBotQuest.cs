using AmeisenBotX.Core.Engines.Quest.Objects.Objectives;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Quest.Objects.Quests
{
    /// <summary>
    /// Represents a bot quest.
    /// </summary>
    /// <remarks>
    /// This interface defines the properties and methods that should be implemented
    /// by a bot quest. A bot quest has information such as whether it is accepted,
    /// finished, or returned, as well as an ID and a name. It also has a list of
    /// objectives that need to be completed. The AcceptQuest() method is used to
    /// accept the quest, the CompleteQuest() method is used to complete the quest,
    /// and the Execute() method is used to execute the quest. 
    /// </remarks>
    public interface IBotQuest
    {
        /// <summary>
        /// Gets the value indicating whether the operation is accepted or not.
        /// </summary>
        bool Accepted { get; }

        /// <summary>
        /// Gets or sets a value indicating if the process is finished.
        /// </summary>
        bool Finished { get; }

        /// <summary>
        /// Gets the value of the Id property.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the list of quest objectives.
        /// </summary>
        List<IQuestObjective> Objectives { get; }

        /// <summary>
        /// Gets a value indicating whether the function has returned or not.
        /// </summary>
        bool Returned { get; }

        /// <summary>
        /// Accepts a quest.
        /// </summary>
        void AcceptQuest();

        /// <summary>
        /// Represents a method that determines if a quest has been completed.
        /// </summary>
        /// <returns>True if the quest has been completed, otherwise false.</returns>
        bool CompleteQuest();

        /// <summary>
        /// Executes a specific action.
        /// </summary>
        void Execute();
    }
}