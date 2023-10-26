using AmeisenBotX.Wow.Objects;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Quest.Objects.Objectives
{
    /// <summary>
    /// Represents a delegate that is used to determine whether a unit meets the conditions for a quest objective.
    /// </summary>
    public delegate bool UseUnitQuestObjectiveCondition();

    public class UseUnitQuestObjective : IQuestObjective
    {
        /// <summary>
        /// Initializes a new instance of the UseUnitQuestObjective class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces used for the objective.</param>
        /// <param name="objectDisplayId">The display ID of the object to use.</param>
        /// <param name="questgiversOnly">Specifies whether only quest givers should be targeted.</param>
        /// <param name="condition">The condition for the objective.</param>
        public UseUnitQuestObjective(AmeisenBotInterfaces bot, int objectDisplayId, bool questgiversOnly, UseUnitQuestObjectiveCondition condition)
        {
            Bot = bot;
            ObjectDisplayIds = new List<int>() { objectDisplayId };
            Condition = condition;
            QuestgiversOnly = questgiversOnly;
        }

        /// <summary>
        /// Initializes a new instance of the UseUnitQuestObjective class.
        /// </summary>
        /// <param name="bot">The bot interface.</param>
        /// <param name="objectDisplayIds">The list of object display IDs.</param>
        /// <param name="questgiversOnly">Determines if only quest givers are considered.</param>
        /// <param name="condition">The condition for the use unit quest objective.</param>
        public UseUnitQuestObjective(AmeisenBotInterfaces bot, List<int> objectDisplayIds, bool questgiversOnly, UseUnitQuestObjectiveCondition condition)
        {
            Bot = bot;
            ObjectDisplayIds = objectDisplayIds;
            Condition = condition;
            QuestgiversOnly = questgiversOnly;
        }

        /// <summary>
        /// Gets a value indicating whether the task is finished or not.
        /// </summary>
        public bool Finished => Progress == 100.0;

        /// <summary>
        /// Gets the progress value, either 100.0 or 0.0, based on the condition.
        /// </summary>
        public double Progress => Condition() ? 100.0 : 0.0;

        /// <summary>
        /// Gets or sets the reference to the AmeisenBotInterfaces class. This property represents the
        /// AmeisenBot instance.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets the condition that determines whether the UseUnitQuestObjective
        /// has been met.
        /// </summary>
        private UseUnitQuestObjectiveCondition Condition { get; }

        /// <summary>
        /// Gets or sets the list of integer object display IDs.
        /// </summary>
        private List<int> ObjectDisplayIds { get; }

        /// <summary>
        /// Gets a value indicating whether the property is restricted to quest givers only.
        /// </summary>
        private bool QuestgiversOnly { get; }

        /// <summary>
        /// Gets or sets the WowUnit object representing the unit in the game.
        /// </summary>
        private IWowUnit Unit { get; set; }

        /// <summary>
        /// Executes the specified logic for interacting with a quest giver.
        /// If the execution is finished or the player is currently casting a spell, the method returns.
        /// The method retrieves the closest quest giver using the provided object display IDs and quests givers flag.
        /// If a quest giver is found, the method checks if the player is within 3.0 units distance from the quest giver's position.
        /// If the player is close enough, it stops click-to-move and resets the movement.
        /// Finally, it interacts with the quest giver using the Wow API.
        /// </summary>
        public void Execute()
        {
            if (Finished || Bot.Player.IsCasting) { return; }

            Unit = Bot.GetClosestQuestGiverByDisplayId(Bot.Player.Position, ObjectDisplayIds, QuestgiversOnly);

            if (Unit != null)
            {
                if (Unit.Position.GetDistance(Bot.Player.Position) < 3.0)
                {
                    Bot.Wow.StopClickToMove();
                    Bot.Movement.Reset();
                }

                Bot.Wow.InteractWithUnit(Unit);
            }
        }
    }
}