using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Contains classes and delegates related to quest objectives in the AmeisenBotX engine.
/// </summary>
namespace AmeisenBotX.Core.Engines.Quest.Objects.Objectives
{
    /// <summary>
    /// Represents a condition to determine if a unit should be killed for a quest objective.
    /// </summary>
    /// <returns>True if the unit should be killed for the quest objective; otherwise, false.</returns>
    public delegate bool KillUnitQuestObjectiveCondition();

    public class KillUnitQuestObjective : IQuestObjective
    {
        /// <summary>
        /// Initializes a new instance of the KillUnitQuestObjective class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces instance.</param>
        /// <param name="objectDisplayId">The object display ID.</param>
        /// <param name="condition">The KillUnitQuestObjectiveCondition.</param>
        public KillUnitQuestObjective(AmeisenBotInterfaces bot, int objectDisplayId, KillUnitQuestObjectiveCondition condition)
        {
            Bot = bot;
            ObjectDisplayIds = new Dictionary<int, int>() { { 0, objectDisplayId } };
            Condition = condition;
        }

        /// <summary>
        /// Initializes a new instance of the KillUnitQuestObjective class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object.</param>
        /// <param name="objectDisplayIds">The dictionary containing object display IDs.</param>
        /// <param name="condition">The KillUnitQuestObjectiveCondition object.</param>
        public KillUnitQuestObjective(AmeisenBotInterfaces bot, Dictionary<int, int> objectDisplayIds, KillUnitQuestObjectiveCondition condition)
        {
            Bot = bot;
            ObjectDisplayIds = objectDisplayIds;
            Condition = condition;
        }

        /// <summary>
        /// Determines if the task is finished.
        /// </summary>
        public bool Finished => Progress == 100.0;

        /// <summary>
        /// Gets the progress as a percentage.
        /// Returns 100.0 if the condition is true, otherwise returns 0.0.
        /// </summary>
        public double Progress => Condition() ? 100.0 : 0.0;

        /// <summary>
        /// Gets or sets the AmeisenBotInterfaces instance used by the bot.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets the private KillUnitQuestObjectiveCondition condition.
        /// </summary>
        private KillUnitQuestObjectiveCondition Condition { get; }

        /// <summary>
        /// Gets the dictionary of object display ids.
        /// </summary>
        /// <returns>The dictionary of object display ids.</returns>
        private Dictionary<int, int> ObjectDisplayIds { get; }

        /// <summary>
        /// Gets or sets the private property for the WowUnit.
        /// </summary>
        private IWowUnit Unit { get; set; }

        /// <summary>
        /// Executes the code to interact with a target unit.
        /// </summary>
        public void Execute()
        {
            if (Finished || Bot.Player.IsCasting) { return; }

            if (Bot.Target != null
                && !Bot.Target.IsDead
                && !Bot.Target.IsNotAttackable
                && Bot.Db.GetReaction(Bot.Player, Bot.Target) != WowUnitReaction.Friendly)
            {
                Unit = Bot.Target;
            }
            else
            {
                Bot.Wow.ClearTarget();

                Unit = Bot.Objects.All
                    .OfType<IWowUnit>()
                    .Where(e => !e.IsDead && ObjectDisplayIds.Values.Contains(e.DisplayId))
                    .OrderBy(e => e.Position.GetDistance(Bot.Player.Position))
                    .OrderBy(e => ObjectDisplayIds.First(x => x.Value == e.DisplayId).Key)
                    .FirstOrDefault();
            }

            if (Unit != null)
            {
                if (Unit.Position.GetDistance(Bot.Player.Position) < 3.0)
                {
                    Bot.Wow.StopClickToMove();
                    Bot.Movement.Reset();
                    Bot.Wow.InteractWithUnit(Unit);
                }
                else
                {
                    Bot.Movement.SetMovementAction(MovementAction.Move, Unit.Position);
                }
            }
        }
    }
}