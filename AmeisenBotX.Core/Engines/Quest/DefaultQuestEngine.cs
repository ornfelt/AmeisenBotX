using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Quest.Objects.Quests;
using AmeisenBotX.Core.Engines.Quest.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Quest
{
    /// <summary>
    /// Gets or sets the event that is triggered when querying for completed quests.
    /// </summary>
    public class DefaultQuestEngine : IQuestEngine
    {
        /// <summary>
        /// Initializes a new instance of the DefaultQuestEngine class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object that represents the bot.</param>
        public DefaultQuestEngine(AmeisenBotInterfaces bot)
        {
            Bot = bot;

            CompletedQuests = new();
            QueryCompletedQuestsEvent = new(TimeSpan.FromSeconds(2));
        }

        /// <summary>
        /// Gets or sets the list of completed quests.
        /// </summary>
        public List<int> CompletedQuests { get; private set; }

        /// <summary>
        /// Gets or sets the quest profile.
        /// </summary>
        public IQuestProfile Profile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the completed quests have been updated.
        /// </summary>
        public bool UpdatedCompletedQuests { get; set; }

        /// <summary>
        /// Gets the Bot interface.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets or sets the last time a quest was abandoned, represented in UTC.
        /// </summary>
        private DateTime LastAbandonQuestTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets the private timegated event for querying completed quests.
        /// </summary>
        private TimegatedEvent QueryCompletedQuestsEvent { get; }

        ///<summary>
        ///This method is used to perform an action when entering a specific state.
        ///</summary>
        public void Enter()
        {
        }

        /// <summary>
        /// Executes a series of actions related to completing quests based on the current profile.
        /// </summary>
        public void Execute()
        {
            if (Profile == null)
            {
                return;
            }

            if (!UpdatedCompletedQuests)
            {
                if (Bot.Wow.Events.Events.All(e => e.Key != "QUEST_QUERY_COMPLETE"))
                {
                    Bot.Wow.Events.Subscribe("QUEST_QUERY_COMPLETE", OnGetQuestsCompleted);
                }

                if (QueryCompletedQuestsEvent.Run())
                {
                    Bot.Wow.QueryQuestsCompleted();
                }

                return;
            }

            if (Profile.Quests.Count > 0)
            {
                IEnumerable<IBotQuest> selectedQuests = Profile.Quests.Peek().Where(e => !e.Returned && !CompletedQuests.Contains(e.Id));

                // drop all quest that are not selected
                if (Bot.Player.QuestlogEntries.Count() == 25 && DateTime.UtcNow.Subtract(LastAbandonQuestTime).TotalSeconds > 30)
                {
                    Bot.Wow.AbandonQuestsNotIn(selectedQuests.Select(q => q.Name));
                    LastAbandonQuestTime = DateTime.UtcNow;
                }

                if (selectedQuests.Any())
                {
                    IBotQuest notAcceptedQuest = selectedQuests.FirstOrDefault(e => !e.Accepted);

                    // make sure we got all quests
                    if (notAcceptedQuest != null)
                    {
                        if (!notAcceptedQuest.Accepted)
                        {
                            notAcceptedQuest.AcceptQuest();
                            return;
                        }
                    }
                    else
                    {
                        // do the quests if not all of them are finished
                        if (selectedQuests.Any(e => !e.Finished))
                        {
                            IBotQuest activeQuest = selectedQuests.FirstOrDefault(e => !e.Finished);
                            activeQuest?.Execute();
                        }
                        else
                        {
                            // make sure we return all quests
                            IBotQuest notReturnedQuest = selectedQuests.FirstOrDefault(e => !e.Returned);

                            if (notReturnedQuest != null)
                            {
                                if (notReturnedQuest.CompleteQuest())
                                {
                                    CompletedQuests.Add(notReturnedQuest.Id);
                                }

                                return;
                            }
                        }
                    }
                }
                else
                {
                    CompletedQuests.AddRange(Profile.Quests.Dequeue().Select(e => e.Id));
                    return;
                }
            }

            // filter duplicates
            CompletedQuests = CompletedQuests.Distinct().ToList();
        }

        /// <summary>
        /// Clears the list of completed quests and updates it with the completed quests obtained from the World of Warcraft bot.
        /// Sets a flag indicating that the completed quests have been updated.
        /// </summary>
        private void OnGetQuestsCompleted(long timestamp, List<string> args)
        {
            Bot.Quest.CompletedQuests.Clear();
            Bot.Quest.CompletedQuests.AddRange(Bot.Wow.GetCompletedQuests());

            Bot.Quest.UpdatedCompletedQuests = true;
        }
    }
}