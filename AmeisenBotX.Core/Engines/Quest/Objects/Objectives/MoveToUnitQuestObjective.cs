using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Wow.Objects;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Quest.Objects.Objectives
{
    /// <summary>
    /// Initializes a new instance of the MoveToUnitQuestObjective class.
    /// </summary>
    public class MoveToUnitQuestObjective : IQuestObjective
    {
        /// <summary>
        /// Initializes a new instance of the MoveToUnitQuestObjective class with the specified bot, unit display ID, and distance.
        /// </summary>
        public MoveToUnitQuestObjective(AmeisenBotInterfaces bot, int unitDisplayId, double distance)
        {
            Bot = bot;
            UnitDisplayIds = new List<int>() { unitDisplayId };
            Distance = distance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveToUnitQuestObjective"/> class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object.</param>
        /// <param name="unitDisplayIds">The list of unit display ids.</param>
        /// <param name="distance">The distance to move to the unit.</param>
        public MoveToUnitQuestObjective(AmeisenBotInterfaces bot, List<int> unitDisplayIds, double distance)
        {
            Bot = bot;
            UnitDisplayIds = unitDisplayIds;
            Distance = distance;
        }

        /// <summary>
        /// Gets a value indicating whether the task is finished.
        /// </summary>
        public bool Finished => Progress == 100.0;

        /// <summary>
        /// Gets the progress value based on the distance between the player and the WoW unit.
        /// Returns 100 if the WoW unit exists and its position is closer to the player than the specified distance,
        /// otherwise returns 0.
        /// </summary>
        public double Progress => IWowUnit != null && IWowUnit.Position.GetDistance(Bot.Player.Position) < Distance ? 100.0 : 0.0;

        /// <summary>
        /// Gets the instance of the AmeisenBotInterfaces Bot.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets or sets the distance.
        /// </summary>
        private double Distance { get; }

        /// <summary>
        /// Gets or sets the instance of the IWowUnit interface.
        /// </summary>
        private IWowUnit IWowUnit { get; set; }

        /// <summary>
        /// Gets or sets the list of unit display IDs.
        /// </summary>
        private List<int> UnitDisplayIds { get; }

        /// <summary>
        /// Executes a movement to the closest quest giver if the character is not finished.
        /// </summary>
        public void Execute()
        {
            if (Finished)
            {
                Bot.Movement.Reset();
                Bot.Wow.StopClickToMove();
                return;
            }

            IWowUnit = Bot.GetClosestQuestGiverByDisplayId(Bot.Player.Position, UnitDisplayIds);

            if (IWowUnit != null)
            {
                if (IWowUnit.Position.GetDistance2D(Bot.Player.Position) > Distance)
                {
                    Bot.Movement.SetMovementAction(MovementAction.Move, IWowUnit.Position);
                }
            }
        }
    }
}