using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Movement.Enums;

namespace AmeisenBotX.Core.Engines.Quest.Objects.Objectives
{
    /// <summary>
    /// Represents a quest objective that requires the player to move to a specific position.
    /// </summary>
    public class MoveToPositionQuestObjective : IQuestObjective
    {
        /// <summary>
        /// Initializes a new instance of the MoveToPositionQuestObjective class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object representing the bot.</param>
        /// <param name="position">The Vector3 object representing the wanted position.</param>
        /// <param name="distance">The distance value.</param>
        /// <param name="movementAction">The MovementAction enum representing the movement action (defaulted to MovementAction.Move).</param>
        public MoveToPositionQuestObjective(AmeisenBotInterfaces bot, Vector3 position, double distance, MovementAction movementAction = MovementAction.Move)
        {
            Bot = bot;
            WantedPosition = position;
            Distance = distance;
            MovementAction = movementAction;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the task is finished or not.
        /// </summary>
        public bool Finished { get; set; }

        /// <summary>
        /// Calculates the progress based on the distance between the wanted position and the player's current position.
        /// If the distance is less than the specified distance, the progress is set to 100.0, otherwise it is set to 0.0.
        /// </summary>
        public double Progress => WantedPosition.GetDistance(Bot.Player.Position) < Distance ? 100.0 : 0.0;

        /// <summary>
        /// Gets or sets the Bot instance for AmeisenBotInterfaces.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets the distance.
        /// </summary>
        private double Distance { get; }

        /// <summary>
        /// Gets the MovementAction property.
        /// </summary>
        private MovementAction MovementAction { get; }

        /// <summary>
        /// Gets the wanted position of the object.
        /// </summary>
        private Vector3 WantedPosition { get; }

        /// <summary>
        /// Executes the code to determine the movement action for the bot.
        /// If the bot is finished or the progress is already at 100%, the execution is terminated.
        /// Otherwise, if the distance between the current player position and the wanted position is greater than the specified distance,
        /// the movement action for the bot is set based on the provided MovementAction and WantedPosition.
        /// </summary>
        public void Execute()
        {
            if (Finished || Progress == 100.0)
            {
                Finished = true;
                Bot.Movement.Reset();
                Bot.Wow.StopClickToMove();
                return;
            }

            if (WantedPosition.GetDistance2D(Bot.Player.Position) > Distance)
            {
                Bot.Movement.SetMovementAction(MovementAction, WantedPosition);
            }
        }
    }
}