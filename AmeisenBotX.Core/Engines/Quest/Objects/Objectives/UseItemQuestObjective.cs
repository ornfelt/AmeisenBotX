using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using System.Linq;

/// <summary>
/// Contains classes and delegates related to the objectives of quests in the AmeisenBotX engine.
/// </summary>
namespace AmeisenBotX.Core.Engines.Quest.Objects.Objectives
{
    /// <summary>
    /// Delegate representing a boolean condition to determine if an objective of a UseItem quest has been completed.
    /// </summary>
    public delegate bool UseItemQuestObjectiveCondition();

    /// <summary>
    /// Represents a quest objective that requires using a specific item to complete.
    /// </summary>
    public class UseItemQuestObjective : IQuestObjective
    {
        /// <summary>
        /// Initializes a new instance of the UseItemQuestObjective class.
        /// </summary>
        /// <param name="bot">The bot instance to use for completing the objective.</param>
        /// <param name="itemId">The ID of the item to use for completing the objective.</param>
        /// <param name="condition">The condition for completing the objective.</param>
        public UseItemQuestObjective(AmeisenBotInterfaces bot, int itemId, UseItemQuestObjectiveCondition condition)
        {
            Bot = bot;
            ItemId = itemId;
            Condition = condition;
        }

        /// <summary>
        /// Gets a value indicating whether the task is finished.
        /// </summary>
        public bool Finished => Progress == 100.0;

        /// <summary>
        /// Returns the progress as a percentage.
        /// If the condition is true, it returns 100.0; otherwise, it returns 0.0.
        /// </summary>
        public double Progress => Condition() ? 100.0 : 0.0;

        /// <summary>
        /// Gets or sets the reference to the AmeisenBotInterfaces instance.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets the condition for the UseItemQuestObjective.
        /// </summary>
        private UseItemQuestObjectiveCondition Condition { get; }

        /// <summary>
        /// Gets the item ID.
        /// </summary>
        private int ItemId { get; }

        /// <summary>
        /// Executes the action if it meets the specified conditions.
        /// </summary>
        public void Execute()
        {
            if (Finished || Bot.Player.IsCasting) { return; }

            IWowInventoryItem item = Bot.Character.Inventory.Items.FirstOrDefault(e => e.Id == ItemId);

            if (item != null)
            {
                Bot.Movement.Reset();
                Bot.Wow.StopClickToMove();
                Bot.Wow.UseContainerItem(item.BagId, item.BagSlot);
            }
        }
    }
}