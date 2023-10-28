using AmeisenBotX.Common.Utils;
using AmeisenBotX.Wow.Objects.Enums;
using System;

/// <summary>
/// Namespace containing classes and delegates related to quest objectives for the Runeforging engine.
/// </summary>
namespace AmeisenBotX.Core.Engines.Quest.Objects.Objectives
{
    /// <summary>
    /// Represents a delegate that checks if a specific condition for an enchant item quest objective has been met.
    /// </summary>
    public delegate bool EnchantItemQuestObjectiveCondition();

    public class RuneforgingQuestObjective : IQuestObjective
    {
        /// <summary>
        /// Initializes a new instance of the RuneforgingQuestObjective class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object.</param>
        /// <param name="condition">The EnchantItemQuestObjectiveCondition object.</param>
        public RuneforgingQuestObjective(AmeisenBotInterfaces bot, EnchantItemQuestObjectiveCondition condition)
        {
            Bot = bot;
            Condition = condition;

            EnchantEvent = new(TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Gets a value indicating whether the progress is finished, which is determined by checking if it equals 100.0.
        /// </summary>
        public bool Finished => Progress == 100.0;

        /// <summary>
        /// Gets the progress as a double value.
        /// </summary>
        public double Progress => Condition() ? 100.0 : 0.0;

        /// <summary>
        /// Gets or sets the Bot interface for the AmeisenBot.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets the condition for the Enchant Item quest objective.
        /// </summary>
        private EnchantItemQuestObjectiveCondition Condition { get; }

        /// <summary>
        /// Gets the private TimegatedEvent EnchantEvent.
        /// </summary>
        private TimegatedEvent EnchantEvent { get; }

        /// <summary>
        /// Executes the method to perform a series of actions for completing a trade skill.
        /// If the task is already finished or the player is currently casting a spell, the trade skill frame will be closed.
        /// If the enchant event is successfully run, the movement is reset, click to move is stopped, the "Runeforging" spell is casted,
        /// and the trade skill create button is clicked. The main hand inventory item is used and the static popup button 1 is clicked.
        /// </summary>
        public void Execute()
        {
            if (Finished || Bot.Player.IsCasting)
            {
                Bot.Wow.ClickUiElement("TradeSkillFrameCloseButton");
                return;
            }

            if (EnchantEvent.Run())
            {
                Bot.Movement.Reset();
                Bot.Wow.StopClickToMove();

                Bot.Wow.CastSpell("Runeforging");
                Bot.Wow.ClickUiElement("TradeSkillCreateButton");
                Bot.Wow.UseInventoryItem(WowEquipmentSlot.INVSLOT_MAINHAND);
                Bot.Wow.ClickUiElement("StaticPopup1Button1");
            }
        }
    }
}