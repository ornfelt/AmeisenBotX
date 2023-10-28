using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Movement.Enums;

/// <summary>
/// Contains classes related to quest objectives for moving vehicles to specific positions while maintaining a certain distance.
/// </summary>
namespace AmeisenBotX.Core.Engines.Quest.Objects.Objectives
{
    /// <summary>
    /// Represents a quest objective to move a vehicle to a specific position while maintaining a certain distance.
    /// </summary>
    public class MoveVehicleToPositionQuestObjective : IQuestObjective
    {
        /// <summary>
        /// Initializes a new instance of the MoveVehicleToPositionQuestObjective class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object.</param>
        /// <param name="position">The target position to move the vehicle to.</param>
        /// <param name="distance">The distance to maintain from the target position.</param>
        /// <param name="movementAction">The movement action to perform (default is Move).</param>
        public MoveVehicleToPositionQuestObjective(AmeisenBotInterfaces bot, Vector3 position, double distance, MovementAction movementAction = MovementAction.Move)
        {
            Bot = bot;
            WantedPosition = position;
            Distance = distance;
            MovementAction = movementAction;
        }

        /// <summary>
        /// Gets a value indicating whether the task is finished or not based on the progress being 100.0.
        /// </summary>
        public bool Finished => Progress == 100.0;

        /// <summary>
        /// Gets the progress of the vehicle.
        /// If the vehicle is not null and the distance between the wanted position and the vehicle's position is less than the given distance,
        /// the progress is 100.0, otherwise it is 0.0.
        /// </summary>
        public double Progress => Bot.Objects.Vehicle != null && WantedPosition.GetDistance(Bot.Objects.Vehicle.Position) < Distance ? 100.0 : 0.0;

        /// <summary>
        /// Gets or sets the Bot object that implements the AmeisenBotInterfaces.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets the value representing the distance.
        /// </summary>
        private double Distance { get; }

        /// <summary>
        /// Gets the movement action associated with this private property.
        /// </summary>
        private MovementAction MovementAction { get; }

        ///<summary>Gets the wanted position of the object in a Vector3 format.</summary>
        private Vector3 WantedPosition { get; }

        /// <summary>
        /// Executes the code for the given task.
        /// If the task is already finished, it resets the movement and stops the click-to-move action.
        /// Otherwise, it checks the distance between the current vehicle position and the wanted position.
        /// If the distance is greater than the specified distance, it sets the movement action with the wanted position.
        /// </summary>
        public void Execute()
        {
            if (Finished)
            {
                Bot.Movement.Reset();
                Bot.Wow.StopClickToMove();
                return;
            }

            if (WantedPosition.GetDistance2D(Bot.Objects.Vehicle.Position) > Distance)
            {
                Bot.Movement.SetMovementAction(MovementAction, WantedPosition, 0);
            }
        }
    }
}