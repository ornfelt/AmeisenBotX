using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Wow.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// This namespace contains classes and delegates related to the objectives of quests in the AmeisenBotX engine.
/// </summary>
namespace AmeisenBotX.Core.Engines.Quest.Objects.Objectives
{
    /// <summary>
    /// Represents a delegate that defines a condition for a quest objective that uses an object.
    /// </summary>
    /// <returns>
    /// Returns a boolean value indicating whether the condition is met.
    /// </returns>
    public delegate bool UseObjectQuestObjectiveCondition();

    /// <summary>
    /// Represents a quest objective that requires using a specific object with the given display ID.
    /// </summary>
    public class UseObjectQuestObjective : IQuestObjective
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UseObjectQuestObjective"/> class.
        /// </summary>
        /// <param name="bot">The bot.</param>
        /// <param name="objectDisplayId">The object display identifier.</param>
        /// <param name="condition">The condition.</param>
        public UseObjectQuestObjective(AmeisenBotInterfaces bot, int objectDisplayId, UseObjectQuestObjectiveCondition condition)
        {
            Bot = bot;
            ObjectDisplayIds = new List<int>() { objectDisplayId };
            Condition = condition;

            UseEvent = new(TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Initializes a new instance of the UseObjectQuestObjective class.
        /// </summary>
        /// <param name="bot">The bot instance.</param>
        /// <param name="objectDisplayIds">The list of object display IDs.</param>
        /// <param name="condition">The condition for the objective.</param>
        public UseObjectQuestObjective(AmeisenBotInterfaces bot, List<int> objectDisplayIds, UseObjectQuestObjectiveCondition condition)
        {
            Bot = bot;
            ObjectDisplayIds = objectDisplayIds;
            Condition = condition;

            UseEvent = new(TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Gets a value indicating whether the task is finished or not.
        /// </summary>
        public bool Finished => Progress == 100.0;

        /// <summary>
        /// Gets the progress value based on the condition.
        /// If the condition is true, returns 100.0,
        /// otherwise returns 0.0.
        /// </summary>
        public double Progress => Condition() ? 100.0 : 0.0;

        /// <summary>
        /// Gets or sets the instance of the AmeisenBotInterfaces that represents the bot.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets or sets the condition for the UseObjectQuestObjective.
        /// </summary>
        private UseObjectQuestObjectiveCondition Condition { get; }

        /// <summary>
        /// Gets or sets the instance of the IWowGameobject interface.
        /// </summary>
        private IWowGameobject IWowGameobject { get; set; }

        /// <summary>
        /// Gets or sets the list of integer values representing the display IDs of the objects.
        /// </summary>
        private List<int> ObjectDisplayIds { get; }

        /// <summary>
        /// Gets or sets the private TimegatedEvent UseEvent.
        /// </summary>
        private TimegatedEvent UseEvent { get; }

        ///<summary>
        ///Executes the code to interact with a specific game object in the game.
        ///</summary>
        public void Execute()
        {
            if (Finished || Bot.Player.IsCasting) { return; }

            IWowGameobject = Bot.Objects.All
                .OfType<IWowGameobject>()
                .Where(e => ObjectDisplayIds.Contains(e.DisplayId))
                .OrderBy(e => e.Position.GetDistance(Bot.Player.Position))
                .FirstOrDefault();

            if (IWowGameobject != null)
            {
                if (IWowGameobject.Position.GetDistance(Bot.Player.Position) < 3.0)
                {
                    if (UseEvent.Run())
                    {
                        Bot.Wow.StopClickToMove();
                        Bot.Movement.Reset();

                        Bot.Wow.InteractWithObject(IWowGameobject);
                    }
                }
                else
                {
                    Bot.Movement.SetMovementAction(MovementAction.Move, IWowGameobject.Position);
                }
            }
        }
    }
}