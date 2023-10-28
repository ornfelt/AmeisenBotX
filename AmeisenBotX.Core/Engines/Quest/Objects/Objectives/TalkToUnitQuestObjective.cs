using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Wow.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Contains classes and delegates related to the objectives of quests in the AmeisenBotX engine.
/// </summary>
namespace AmeisenBotX.Core.Engines.Quest.Objects.Objectives
{
    /// <summary>
    /// Represents a delegate that checks the condition of a TalkToUnit quest objective.
    /// </summary>
    /// <returns>True if the condition is met, False otherwise.</returns>
    public delegate bool TalkToUnitQuestObjectiveCondition();

    public class TalkToUnitQuestObjective : IQuestObjective
    {
        /// <summary>
        /// Initializes a new instance of the TalkToUnitQuestObjective class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object.</param>
        /// <param name="displayId">The display ID.</param>
        /// <param name="gossipIds">The list of gossip IDs.</param>
        /// <param name="condition">The TalkToUnitQuestObjectiveCondition object.</param>
        public TalkToUnitQuestObjective(AmeisenBotInterfaces bot, int displayId, List<int> gossipIds, TalkToUnitQuestObjectiveCondition condition)
        {
            Bot = bot;
            DisplayIds = new List<int>() { displayId };
            GossipIds = gossipIds;
            Condition = condition;

            TalkEvent = new(TimeSpan.FromMilliseconds(500));
        }

        /// <summary>
        /// Initializes a new instance of the TalkToUnitQuestObjective class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object representing the bot.</param>
        /// <param name="displayIds">The list of display IDs.</param>
        /// <param name="gossipIds">The list of gossip IDs.</param>
        /// <param name="condition">The TalkToUnitQuestObjectiveCondition object representing the condition.</param>
        public TalkToUnitQuestObjective(AmeisenBotInterfaces bot, List<int> displayIds, List<int> gossipIds, TalkToUnitQuestObjectiveCondition condition)
        {
            Bot = bot;
            DisplayIds = displayIds;
            GossipIds = gossipIds;
            Condition = condition;

            TalkEvent = new(TimeSpan.FromMilliseconds(500));
        }

        /// <summary>
        /// Gets a value indicating whether the progress is finished, which is determined by comparing the progress value to 100.0.
        /// </summary>
        public bool Finished => Progress == 100.0;

        /// <summary>
        /// Gets the progress value.
        /// If the condition is true, returns 100.0.
        /// If the condition is false, returns 0.0.
        /// </summary>
        public double Progress => Condition() ? 100.0 : 0.0;

        /// <summary>
        /// Gets or sets the instance of AmeisenBotInterfaces used by the private field Bot.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets the condition for the TalkToUnitQuestObjective.
        /// </summary>
        private TalkToUnitQuestObjectiveCondition Condition { get; }

        /// <summary>
        /// Gets or sets the Counter property.
        /// </summary>
        private int Counter { get; set; }

        /// <summary>
        /// Gets the list of integers representing the display ids.
        /// </summary>
        private List<int> DisplayIds { get; }

        /// <summary>
        /// Gets the list of GossipIds.
        /// </summary>
        private List<int> GossipIds { get; }

        /// <summary>
        /// Gets or sets the private TimegatedEvent TalkEvent property.
        /// </summary>
        private TimegatedEvent TalkEvent { get; }

        /// <summary>
        /// Gets or sets the wow unit.
        /// </summary>
        private IWowUnit Unit { get; set; }

        /// <summary>
        /// Executes the action.
        /// If the action is finished or the player is currently casting, does nothing.
        /// Finds a suitable unit to interact with based on specific criteria.
        /// If the unit is within a certain distance, initiates interaction and selects a gossip option.
        /// Otherwise, moves towards the unit.
        /// </summary>
        public void Execute()
        {
            if (Finished || Bot.Player.IsCasting) { return; }

            Unit = Bot.Objects.All
                .OfType<IWowUnit>()
                .Where(e => e.IsGossip && !e.IsDead && DisplayIds.Contains(e.DisplayId))
                .OrderBy(e => e.Position.GetDistance(Bot.Player.Position))
                .FirstOrDefault();

            if (Unit != null)
            {
                if (Unit.Position.GetDistance(Bot.Player.Position) < 3.0)
                {
                    if (TalkEvent.Run())
                    {
                        Bot.Wow.StopClickToMove();
                        Bot.Movement.Reset();

                        Bot.Wow.InteractWithUnit(Unit);

                        ++Counter;
                        if (Counter > GossipIds.Count)
                        {
                            Counter = 1;
                        }

                        Bot.Wow.SelectGossipOption(GossipIds[Counter - 1]);
                    }
                }
                else
                {
                    Bot.Movement.SetMovementAction(MovementAction.Move, Unit.Position);
                }
            }
        }
    }
}