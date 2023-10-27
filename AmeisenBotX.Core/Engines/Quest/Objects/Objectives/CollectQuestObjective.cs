using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Core.Managers.Character.Inventory.Objects;
using AmeisenBotX.Wow.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Quest.Objects.Objectives
{
    /// <summary>
    /// Represents a quest objective to collect a certain amount of a specific item.
    /// </summary>
    public class CollectQuestObjective : IQuestObjective
    {
        /// <summary>
        /// Initializes a new instance of the CollectQuestObjective class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object.</param>
        /// <param name="itemId">The ID of the item to collect.</param>
        /// <param name="itemAmount">The amount of the item to collect.</param>
        /// <param name="gameObjectIds">The list of game object IDs to interact with.</param>
        /// <param name="positions">The list of positions to move to within the area.</param>
        public CollectQuestObjective(AmeisenBotInterfaces bot, int itemId, int itemAmount, List<int> gameObjectIds, List<Vector3> positions)
        {
            Bot = bot;
            ItemId = itemId;
            WantedItemAmount = itemAmount;
            GameObjectIds = gameObjectIds;
            Area = positions.Select(pos => new AreaNode(pos, 10.0)).ToList();
            RightClickEvent = new(TimeSpan.FromMilliseconds(1500));
        }

        /// <summary>
        /// Gets or sets a List of AreaNodes.
        /// </summary>
        public List<AreaNode> Area { get; set; }

        /// <summary>
        /// Indicates whether the progress of the operation has finished.
        /// </summary>
        public bool Finished => Math.Abs(Progress - 100.0) < 0.0001;

        /// <summary>
        /// Gets the progress of the current item.
        /// </summary>
        /// <returns>
        /// The progress as a percentage. If the wanted item amount is 0, returns 100.0.
        /// If the inventory item is found, returns the progress as a percentage based on the count of the item and the wanted item amount.
        /// If the inventory item is not found, returns 0.0.
        /// </returns>
        public double Progress
        {
            get
            {
                if (WantedItemAmount == 0)
                {
                    return 100.0;
                }

                IWowInventoryItem inventoryItem = Bot.Character.Inventory.Items.Find(item => item.Id == ItemId);
                return inventoryItem != null ? Math.Min(100.0 * ((float)inventoryItem.Count) / ((float)WantedItemAmount), 100.0) : 0.0;
            }
        }

        ///<summary>
        /// Gets or sets the instance of the AmeisenBotInterfaces associated with the Bot property.
        ///</summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets the list of game object IDs.
        /// </summary>
        private List<int> GameObjectIds { get; }

        /// <summary>
        /// Gets or sets the identifier of the item.
        /// </summary>
        private int ItemId { get; }

        /// <summary>
        /// Gets or sets the private TimegatedEvent for right-click events.
        /// </summary>
        private TimegatedEvent RightClickEvent { get; }

        /// <summary>
        /// Gets the amount of the wanted item.
        /// </summary>
        private int WantedItemAmount { get; }

        /// <summary>
        /// Executes the specified code block. 
        /// This method is responsible for interacting with a lootable object or moving to a selected area.
        /// It first checks if the object to be looted exists. 
        /// If it does, it checks if the distance between the object and the player is greater than 5.0. 
        /// If so, it sets the movement action to move towards the object's position.
        /// If the distance is less than or equal to 5.0, it performs a right-click event to interact with the object.
        /// Once the interaction is complete, it resets the movement, stops click-to-move, and interacts with the object using the Bot.Wow.InteractWithObject() method.
        /// If the lootableObject is null, it selects an area from a list of areas ordered by distance to the player's position.
        /// If an area is selected, it sets the movement action to move towards the selected area's position.
        /// </summary>
        public void Execute()
        {
            if (Finished) { return; }

            IWowGameobject lootableObject = Bot.Objects.All.OfType<IWowGameobject>()
                .Where(e => GameObjectIds.Contains(e.EntryId))
                .OrderBy(e => e.Position.GetDistance(Bot.Player.Position))
                .FirstOrDefault();

            if (lootableObject != null)
            {
                if (lootableObject.Position.GetDistance(Bot.Player.Position) > 5.0)
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