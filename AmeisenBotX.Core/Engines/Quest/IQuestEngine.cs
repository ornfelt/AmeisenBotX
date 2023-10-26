using AmeisenBotX.Core.Engines.Quest.Profiles;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Quest
{
    /// <summary>
    /// Represents a quest engine interface.
    /// </summary>
    public interface IQuestEngine
    {
        /// <summary>
        /// Gets the list of completed quests.
        /// </summary>
        List<int> CompletedQuests { get; }

        /// <summary>
        /// Gets or sets the IQuestProfile object representing the profile.
        /// </summary>
        IQuestProfile Profile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the completed quests have been updated.
        /// </summary>
        bool UpdatedCompletedQuests { get; set; }

        /// <summary>
        /// Method to enter.
        /// </summary>
        void Enter();

        /// <summary>
        /// Executes a specific action or set of instructions.
        /// </summary>
        void Execute();
    }
}