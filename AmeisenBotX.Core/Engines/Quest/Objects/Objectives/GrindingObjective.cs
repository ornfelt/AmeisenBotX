using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Core.Engines.Movement.Pathfinding.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Represents a namespace for grinding objectives in a quest.
/// </summary>
namespace AmeisenBotX.Core.Engines.Quest.Objects.Objectives
{
    /// <summary>
    /// Represents a grinding objective in a quest.
    /// </summary>
    internal class GrindingObjective : IQuestObjective
    {
        /// <summary>
        /// Initializes a new instance of the GrindingObjective class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object used for grinding.</param>
        /// <param name="targetLevel">The target level for grinding.</param>
        /// <param name="grindingAreas">The list of grinding areas.</param>
        /// <param name="vendorsLocation">The list of vendor locations. (Optional)</param>
        public GrindingObjective(AmeisenBotInterfaces bot, int targetLevel,
                    List<List<Vector3>> grindingAreas, List<Vector3> vendorsLocation = null)
        {
            Bot = bot;
            WantedLevel = targetLevel;
            SearchAreas = new SearchAreaEnsamble(grindingAreas);
            VendorsLocation = vendorsLocation;
        }

        /// <summary>
        /// Checks if the player's level is greater than or equal to the wanted level of the bot.
        /// Returns true if the player's level is greater than or equal to the wanted level; otherwise, false.
        /// </summary>
        public bool Finished => Bot.Player.Level >= WantedLevel;

        /// <summary>
        /// Calculates the progress percentage towards the wanted level based on the level and XP percentage of the player.
        /// </summary>
        public double Progress => 100.0 * (Bot.Player.Level + Bot.Player.XpPercentage / 100.0) / WantedLevel;

        /// <summary>
        /// Gets the list of vendors' locations.
        /// </summary>
        public List<Vector3> VendorsLocation { get; }

        /// <summary>
        /// Gets the AmeisenBotInterfaces instance representing the bot.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Private property representing the current node as a Vector3.
        /// </summary>
        private Vector3 CurrentNode { get; set; }

        /// <summary>
        /// Gets or sets the instance of IWowUnit.
        /// </summary>
        private IWowUnit IWowUnit { get; set; }

        /// <summary>
        /// Gets the SearchAreaEnsemble property.
        /// </summary>
        private SearchAreaEnsamble SearchAreas { get; }

        /// <summary>
        /// Gets the current wanted level.
        /// </summary>
        private int WantedLevel { get; }

        /// <summary>
        /// Executes the logic for the bot's behavior.
        /// The method checks if the bot is already finished or the player is currently casting, if so, it returns.
        /// It then checks if the player is not near a search area and does not have a target.
        /// In that case, it clears the target and sets the IWowUnit variable to null.
        /// If the bot is not in combat and also does not have a target, it tries to find a suitable target.
        /// The criteria for a suitable target are that it should not be dead, not be unattackable, not be a friendly unit,
        /// have more than 10 health, and the slope gradient angle between the player's position and the target's position should be less than or equal to 39.0f.
        /// The method then orders the list of suitable targets based on the distance from the player's position and selects the first one.
        /// If a suitable target is found, it changes the target to the selected target.
        /// If the IWowUnit variable is not null, it notifies the search areas about a detour and initiates the attack on the target.
        /// If the IWowUnit variable is null, it checks if the player's position is within a certain distance from the current node or if the search areas have aborted the path.
        /// If either condition is true, it selects the next position from the search areas and sets the movement action to move towards that position.
        /// </summary>
        public void Execute()
        {
            if (Finished || Bot.Player.IsCasting) { return; }

            if (!SearchAreas.IsPlayerNearSearchArea(Bot) && Bot.Target == null) // if i have target, go nearby don't clear it
            {
                Bot.Wow.ClearTarget();
                IWowUnit = null;
            }

            if (!Bot.Player.IsInCombat
                && Bot.Target == null) // if pulling with ranged we have target and yet not in combat
            {
                IWowUnit = Bot.Objects.All
                    .OfType<IWowUnit>()
                    .Where(e => !e.IsDead && !e.IsNotAttackable
                                    && Bot.Db.GetReaction(Bot.Player, e) != WowUnitReaction.Friendly
                                    && e.Health > 10 // workaround to filter some critters, would be nice e.CreatureType != WoWCreatureType.Critter
                                    && BotMath.SlopeGradientAngle(Bot.Player.Position, e.Position) <= 39.0f) // check if not too steep
                    .OrderBy(e => e.Position.GetDistance(Bot.Player.Position))
                    .FirstOrDefault();

                if (IWowUnit != null)
                {
                    Bot.Wow.ChangeTarget(IWowUnit.Guid);
                }
            }

            if (IWowUnit != null)
            {
                SearchAreas.NotifyDetour();
                Bot.CombatClass.AttackTarget();
            }
            else if (Bot.Player.Position.GetDistance(CurrentNode) < 3.5f || SearchAreas.HasAbortedPath())
            {
                CurrentNode = SearchAreas.GetNextPosition(Bot);
                Bot.Movement.SetMovementAction(MovementAction.Move, CurrentNode);
            }
        }
    }
}