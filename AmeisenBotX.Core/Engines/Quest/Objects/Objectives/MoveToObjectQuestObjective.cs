using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Wow.Objects;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Quest.Objects.Objectives
{
    /// <summary>
    /// Initializes a new instance of the MoveToObjectQuestObjective class.
    /// </summary>
    /// <param name="bot">The bot interface used for controlling the bot.</param>
    /// <param name="objectDisplayId">The display ID of the object to move towards.</param>
    /// <param name="distance">The maximum distance from the object that is considered as reached.</param>
    public class MoveToObjectQuestObjective : IQuestObjective
    {
        /// <summary>
        /// Initializes a new instance of the MoveToObjectQuestObjective class.
        /// </summary>
        /// <param name="bot">The bot interface used for controlling the bot.</param>
        /// <param name="objectDisplayId">The display ID of the object to move towards.</param>
        /// <param name="distance">The maximum distance from the object that is considered as reached.</param>
        public MoveToObjectQuestObjective(AmeisenBotInterfaces bot, int objectDisplayId, double distance)
        {
            Bot = bot;
            ObjectDisplayIds = new List<int>() { objectDisplayId };
            Distance = distance;
        }

        /// <summary>
        /// Initializes a new instance of the MoveToObjectQuestObjective class.
        /// </summary>
        /// <param name="bot">The bot to use for the quest objective.</param>
        /// <param name="objectDisplayIds">The list of object display IDs to move towards.</param>
        /// <param name="distance">The distance to move towards the object.</param>
        public MoveToObjectQuestObjective(AmeisenBotInterfaces bot, List<int> objectDisplayIds, double distance)
        {
            Bot = bot;
            ObjectDisplayIds = objectDisplayIds;
            Distance = distance;
        }

        /// <summary>
        /// Gets a boolean value indicating if the progress is finished, 
        /// which will be true if the progress is equal to 100.0.
        /// </summary>
        public bool Finished => Progress == 100.0;

        /// <summary>
        /// Calculates the progress based on the distance between the current player and the WoW game object.
        /// Progress is set to 100.0 if IWowGameobject is not null and the distance is less than the specified Distance; otherwise, progress is set to 0.0.
        /// </summary>
        public double Progress => IWowGameobject != null && IWowGameobject.Position.GetDistance(Bot.Player.Position) < Distance ? 100.0 : 0.0;

        /// <summary>
        /// Gets or sets the interface for the AmeisenBot class which represents a bot.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets the distance value.
        /// </summary>
        private double Distance { get; }

        /// <summary>
        /// Gets or sets the interface for a World of Warcraft game object.
        /// </summary>
        private IWowGameobject IWowGameobject { get; set; }

        /// <summary>
        /// Gets the list of int values representing the display IDs of objects.
        /// </summary>
        private List<int> ObjectDisplayIds { get; }

        /// <summary>
        /// Executes the code to interact with the closest game object, if it exists and is within a specified distance.
        /// If the execution is finished or the closest game object does not exist, it stops the movement and click-to-move actions of the bot.
        /// </summary>
        public void Execute()
        {
            if (Finished)
            {
                Bot.Movement.Reset();
                Bot.Wow.StopClickToMove();
                return;
            }

            IWowGameobject = Bot.GetClosestGameObjectByDisplayId(Bot.Player.Position, ObjectDisplayIds);

            if (IWowGameobject != null)
            {
                if (IWowGameobject.Position.GetDistance2D(Bot.Player.Position) > Distance)
                {
                    Bot.Movement.SetMovementAction(MovementAction.Move, IWowGameobject.Position);
                }
            }
        }
    }
}