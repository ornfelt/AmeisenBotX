using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Wow.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Namespace for quest objectives related to collecting specific items in a game.
/// </summary>
namespace AmeisenBotX.Core.Engines.Quest.Objects.Objectives
{
    /// <summary>
    /// Represents a deprecated quest objective that requires collecting a specific item in a game.
    /// </summary>
    public class CollectQuestObjectiveDEPRECATED : IQuestObjective
    {
        /// <summary>
        /// Initializes a new instance of the CollectQuestObjectiveDEPRECATED class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object.</param>
        /// <param name="itemId">The ID of the item.</param>
        /// <param name="itemAmount">The amount of the item to collect.</param>
        /// <param name="objectDisplayId">The ID of the object display.</param>
        /// <param name="area">The list of AreaNode objects.</param>
        public CollectQuestObjectiveDEPRECATED(AmeisenBotInterfaces bot, int itemId, int itemAmount, int objectDisplayId, List<AreaNode> area)
        {
            Bot = bot;
            ItemId = itemId;
            WantedItemAmount = itemAmount;
            ObjectDisplayId = objectDisplayId;
            Area = area;

            RightClickEvent = new(TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Gets or sets the list of AreaNodes.
        /// </summary>
        public List<AreaNode> Area { get; set; }

        /// <summary>
        /// Gets a value indicating whether the task is finished.
        /// </summary>
        public bool Finished => Progress == 100.0;


        /// <summary>
        /// Calculates the progress as a percentage based on the current item amount and the wanted item amount.
        /// </summary>
        /// <returns>The progress as a percentage.</returns>
        public double Progress => Math.Round(CurrentItemAmount / (double)WantedItemAmount * 100.0, 1);

        /// <summary>
        /// Gets or sets the instance of the AmeisenBotInterfaces that represents the bot.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets the current amount of items in the character's inventory with the specified item ID.
        /// </summary>
        private int CurrentItemAmount => Bot.Character.Inventory.Items.Count(e => e.Id == ItemId);

        /// <summary>
        /// Gets the ItemId.
        /// </summary>
        private int ItemId { get; }

        /// <summary>
        /// Gets the display ID of the object.
        /// </summary>
        private int ObjectDisplayId { get; }

        /// <summary>
        /// The RightClickEvent property represents the timegated event that occurs when a right click action is performed.
        /// </summary>
        private TimegatedEvent RightClickEvent { get; }

        /// <summary>
        /// Gets the amount of the wanted item.
        /// </summary>
        private int WantedItemAmount { get; }

        ///<summary>
        ///This method is used to execute a series of actions based on certain conditions. 
        ///If the object being interacted with is a lootable object, the bot will move towards it if it is not within 3.0 units of distance. 
        ///If the object is within range, the bot will perform a right-click event, reset its movement, stop click-to-move, and interact with the object.
        ///If there is no lootable object nearby, the bot will select an area to move towards based on the closest proximity to the player's position.
        ///</summary>
        public void Execute()
        {
            if (Finished) { return; }

            IWowGameobject lootableObject = Bot.Objects.All.OfType<IWowGameobject>()
                .Where(e => e.DisplayId == ObjectDisplayId)
                .OrderBy(e => e.Position.GetDistance(Bot.Player.Position))
                .FirstOrDefault();

            if (lootableObject != null)
            {
                if (lootableObject.Position.GetDistance(Bot.Player.Position) > 3.0)
                {
                    Bot.Movement.SetMovementAction(MovementAction.Move, lootableObject.Position);
                }
                else
                {
                    if (RightClickEvent.Run())
                    {
                        Bot.Movement.Reset();
                        Bot.Wow.StopClickToMove();
                        Bot.Wow.InteractWithObject(lootableObject);
                    }
                }
            }
            else
            {
                AreaNode selectedArea = Area
                    .OrderBy(e => e.Position.GetDistance(Bot.Player.Position))
                    .FirstOrDefault(e => e.Position.GetDistance(Bot.Player.Position) < e.Radius);

                if (selectedArea != null)
                {
                    Bot.Movement.SetMovementAction(MovementAction.Move, selectedArea.Position);
                }
            }
        }
    }
}