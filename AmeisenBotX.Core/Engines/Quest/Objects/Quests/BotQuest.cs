using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Core.Engines.Quest.Objects.Objectives;
using AmeisenBotX.Core.Managers.Character.Inventory;
using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using AmeisenBotX.Wow.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AmeisenBotX.Core.Engines.Quest.Objects.Quests
{
    /// <summary>
    /// Represents a delegate that retrieves the position of a bot for a quest.
    /// </summary>
    /// <returns>
    /// The position of the bot for the quest as a tuple containing an <see cref="IWowObject"/> 
    /// representing the bot and a <see cref="Vector3"/> representing the position.
    /// </returns>
    public delegate (IWowObject, Vector3) BotQuestGetPosition();

    public class BotQuest : IBotQuest
    {
        /// <summary>
        /// Initializes a new instance of the BotQuest class.
        /// </summary>
        /// <param name="bot">The BotInterfaces object.</param>
        /// <param name="id">The quest id.</param>
        /// <param name="name">The quest name.</param>
        /// <param name="level">The quest level.</param>
        /// <param name="gossipId">The gossip id.</param>
        /// <param name="start">The starting position.</param>
        /// <param name="end">The ending position.</param>
        /// <param name="objectives">The list of quest objectives.</param>
        public BotQuest(AmeisenBotInterfaces bot, int id, string name, int level, int gossipId, BotQuestGetPosition start, BotQuestGetPosition end, List<IQuestObjective> objectives)
        {
            Bot = bot;

            Id = id;
            Name = name;
            Level = level;
            GossipId = gossipId;
            GetStartObject = start;
            GetEndObject = end;
            Objectives = objectives;

            ActionEvent = new(TimeSpan.FromMilliseconds(250));
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is accepted.
        /// </summary>
        public bool Accepted { get; set; }

        /// <summary>
        /// Gets the TimegatedEvent for the action event.
        /// </summary>
        public TimegatedEvent ActionEvent { get; }

        /// <summary>
        /// Gets or sets the value indicating whether the ActionToggle is enabled or disabled.
        /// </summary>
        public bool ActionToggle { get; set; }

        /// <summary>
        /// Determines if the task is finished by checking if all objectives are finished or
        /// if the progress is at 100%.
        /// </summary>
        public bool Finished => (Objectives != null && Objectives.All(e => e.Finished)) || Progress == 100.0;

        /// <summary>
        /// Gets or sets the position of the end object in the BotQuest.
        /// </summary>
        public BotQuestGetPosition GetEndObject { get; set; }

        /// <summary>
        /// Gets or sets the start object used in the BotQuestGetPosition.
        /// </summary>
        public BotQuestGetPosition GetStartObject { get; set; }

        /// <summary>
        /// Gets or sets the Gossip ID.
        /// </summary>
        public int GossipId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the object.
        /// </summary>
        public int Id { get; set; }

        ///<summary>
        ///Gets or sets the level.
        ///</summary>
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets the name property.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the list of quest objectives.
        /// </summary>
        public List<IQuestObjective> Objectives { get; set; }

        /// <summary>
        /// Gets the overall progress of the objectives.
        /// If there are no objectives, the progress is already complete (100%).
        /// Calculates the average progress by summing up the progress of each objective and dividing it by the total number of objectives.
        /// </summary>
        /// <returns>The overall progress as a double value.</returns>
        public double Progress
        {
            get
            {
                if (Objectives == null || Objectives.Count == 0) { return 100.0; }

                double totalProgress = Objectives.Sum(questObjective => questObjective.Progress);

                return Math.Round(totalProgress / Objectives.Count, 1);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether an item has been returned.
        /// </summary>
        public bool Returned { get; set; }

        /// <summary>
        /// Gets the Bot object for the AmeisenBotInterfaces.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the check has been performed to determine if it is accepted.
        /// </summary>
        private bool CheckedIfAccepted { get; set; } = false;

        /// <summary>
        /// Method to accept a quest. 
        /// Checks if the quest has already been accepted, if not, checks if the bot's WoW object has the quest in its quest log. 
        /// If the quest is found in the quest log, sets the 'Accepted' flag to true. 
        /// If the quest has been accepted, returns without taking any further action. 
        /// Gets the start object's position and checks if it is within a certain distance from the bot's position. 
        /// If the distance is greater than 5.0, sets the movement action to move towards the start object's position. 
        /// If the distance is within 5.0 and the ActionEvent.Run() method returns true, performs the action toggle check. 
        /// If ActionToggle is false, right-clicks the quest giver. 
        /// If ActionToggle is true, selects the quest via its name or gossip ID, sleeps for 1 second, accepts the quest, sleeps for 250 milliseconds, and checks if the quest is in the quest log again to set the 'Accepted' flag. 
        /// Finally, toggles the ActionToggle flag. 
        /// If the start object is not found in the WoW object, checks if the object's position is defined. 
        /// If defined, checks if the distance between the object and the bot is greater than 5.0 and sets the movement action accordingly. 
        /// </summary>
        public void AcceptQuest()
        {
            if (!CheckedIfAccepted)
            {
                if (Bot.Wow.GetQuestLogIdByTitle(Name, out _))
                {
                    Accepted = true;
                }

                CheckedIfAccepted = true;
            }

            if (Accepted)
            {
                return;
            }

            (IWowObject, Vector3) objectPositionCombo = GetStartObject();

            if (objectPositionCombo.Item1 != null)
            {
                if (Bot.Player.Position.GetDistance(objectPositionCombo.Item1.Position) > 5.0)
                {
                    Bot.Movement.SetMovementAction(MovementAction.Move, objectPositionCombo.Item1.Position);
                }
                else if (ActionEvent.Run())
                {
                    if (!ActionToggle)
                    {
                        RightClickQuestgiver(objectPositionCombo.Item1);
                    }
                    else
                    {
                        Bot.Wow.SelectQuestByNameOrGossipId(Name, GossipId, true);
                        Thread.Sleep(1000);
                        Bot.Wow.AcceptQuest();
                        Thread.Sleep(250);

                        if (Bot.Wow.GetQuestLogIdByTitle(Name, out _))
                        {
                            Accepted = true;
                        }
                    }

                    ActionToggle = !ActionToggle;
                }
            }
            else if (objectPositionCombo.Item2 != default)
            {
                // move to position
                if (Bot.Player.Position.GetDistance(objectPositionCombo.Item2) > 5.0)
                {
                    Bot.Movement.SetMovementAction(MovementAction.Move, objectPositionCombo.Item2);
                }
            }
        }

        /// <summary>
        /// Method to complete a quest. Returns true if the quest is successfully completed, otherwise false.
        /// </summary>
        public bool CompleteQuest()
        {
            if (Returned)
            {
                return true;
            }

            (IWowObject, Vector3) objectPositionCombo = GetEndObject();

            if (objectPositionCombo.Item1 != null)
            {
                // move to unit / object
                if (Bot.Player.Position.GetDistance(objectPositionCombo.Item1.Position) > 5.0)
                {
                    Bot.Movement.SetMovementAction(MovementAction.Move, objectPositionCombo.Item1.Position);
                }
                else
                {
                    // interact with it
                    if (!ActionToggle)
                    {
                        RightClickQuestgiver(objectPositionCombo.Item1);
                    }
                    else if (ActionEvent.Run())
                    {
                        Bot.Wow.SelectQuestByNameOrGossipId(Name, GossipId, false);
                        Thread.Sleep(1000);
                        Bot.Wow.CompleteQuest();
                        Thread.Sleep(1000);

                        bool selectedReward = false;
                        // TODO: This only works for the english locale!
                        if (Bot.Wow.GetQuestLogIdByTitle(Name, out int questLogId))
                        {
                            Bot.Wow.SelectQuestLogEntry(questLogId);

                            if (Bot.Wow.GetNumQuestLogChoices(out int numChoices))
                            {
                                for (int i = 1; i <= numChoices; ++i)
                                {
                                    if (Bot.Wow.GetQuestLogChoiceItemLink(i, out string itemLink))
                                    {
                                        string itemJson = Bot.Wow.GetItemByNameOrLink(itemLink);
                                        WowBasicItem item = ItemFactory.BuildSpecificItem(ItemFactory.ParseItem(itemJson));

                                        if (item == null)
                                        {
                                            break;
                                        }

                                        if (item.Name == "0" || item.ItemLink == "0")
                                        {
                                            // get the item id and try again
                                            itemJson = Bot.Wow.GetItemByNameOrLink
                                            (
                                                itemLink.Split(new string[] { "Hitem:" }, StringSplitOptions.RemoveEmptyEntries)[1]
                                                        .Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[0]
                                            );

                                            item = ItemFactory.BuildSpecificItem(ItemFactory.ParseItem(itemJson));
                                        }

                                        if (Bot.Character.IsItemAnImprovement(item, out _))
                                        {
                                            Bot.Wow.SelectQuestReward(i);
                                            Bot.Wow.SelectQuestReward(i);
                                            Bot.Wow.SelectQuestReward(i);
                                            selectedReward = true;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                        if (!selectedReward)
                        {
                            Bot.Wow.SelectQuestReward(1);
                        }

                        Thread.Sleep(250);
                        Returned = true;
                        return true;
                    }

                    ActionToggle = !ActionToggle;
                }
            }
            else if (objectPositionCombo.Item2 != default)
            {
                // move to position
                if (Bot.Player.Position.GetDistance(objectPositionCombo.Item2) > 5.0)
                {
                    Bot.Movement.SetMovementAction(MovementAction.Move, objectPositionCombo.Item2);
                }
            }

            return false;
        }

        /// <summary>
        /// Executes the first unfinished objective in the list of objectives.
        /// </summary>
        public void Execute()
        {
            Objectives.FirstOrDefault(e => !e.Finished)?.Execute();
        }

        /// <summary>
        /// Method to handle right clicking on a quest giver object. It checks the type of the object and interacts with it accordingly.
        /// </summary>
        /// <param name="obj">The object to be right clicked on.</param>
        private void RightClickQuestgiver(IWowObject obj)
        {
            if (obj.GetType() == typeof(IWowGameobject))
            {
                Bot.Wow.InteractWithObject(obj);
            }
            else if (obj.GetType() == typeof(IWowUnit))
            {
                Bot.Wow.InteractWithUnit((IWowUnit)obj);
            }
        }
    }
}