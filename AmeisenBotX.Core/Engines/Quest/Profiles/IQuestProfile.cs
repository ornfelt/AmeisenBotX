using AmeisenBotX.Core.Engines.Quest.Objects.Quests;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Quest.Profiles
{
    /// <summary>
    /// Represents a quest profile for a bot.
    /// </summary>
    public interface IQuestProfile
    {
        /// <summary>
        /// Gets or sets the queue of lists of IBotQuest objects.
        /// </summary>
        Queue<List<IBotQuest>> Quests { get; }
    }
}