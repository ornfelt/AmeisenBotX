using AmeisenBotX.BehaviorTree.Interfaces;
using AmeisenBotX.Common.Math;
using AmeisenBotX.Wow.Objects;
using System;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Battleground.Jannis
{
    /// <summary>
    /// Represents a blackboard used for managing Capture The Flag (CTF) battleground information.
    /// </summary>
    public class CtfBlackboard : IBlackboard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CtfBlackboard"/> class with an update action.
        /// </summary>
        /// <param name="updateAction">The action to be executed when the blackboard is updated.</param>
        public CtfBlackboard(Action updateAction)
        {
            UpdateAction = updateAction;
        }

        /// <summary>
        /// Gets or sets the enemy team's flag carrier.
        /// </summary>
        public IWowUnit EnemyTeamFlagCarrier { get; set; }

        /// <summary>
        /// Gets or sets the position of the enemy team's flag.
        /// </summary>
        public Vector3 EnemyTeamFlagPos { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the enemy team has the flag.
        /// </summary>
        public bool EnemyTeamHasFlag { get; set; }

        /// <summary>
        /// Gets or sets the maximum score for the enemy team.
        /// </summary>
        public int EnemyTeamMaxScore { get; set; }

        /// <summary>
        /// Gets or sets the current score of the enemy team.
        /// </summary>
        public int EnemyTeamScore { get; set; }

        /// <summary>
        /// Gets or sets the flag carrier for the player's team.
        /// </summary>
        public IWowUnit MyTeamFlagCarrier { get; set; }

        /// <summary>
        /// Gets or sets the position of the player's team's flag.
        /// </summary>
        public Vector3 MyTeamFlagPos { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player's team has the flag.
        /// </summary>
        public bool MyTeamHasFlag { get; set; }

        /// <summary>
        /// Gets or sets the maximum score for the player's team.
        /// </summary>
        public int MyTeamMaxScore { get; set; }

        /// <summary>
        /// Gets or sets the current score of the player's team.
        /// </summary>
        public int MyTeamScore { get; set; }

        /// <summary>
        /// Gets or sets a collection of game objects representing flags near the player.
        /// </summary>
        public IEnumerable<IWowGameobject> NearFlags { get; set; }

        /// <summary>
        /// Private field for the update action.
        /// </summary>
        private Action UpdateAction { get; }

        /// <summary>
        /// Updates the blackboard by executing the update action.
        /// </summary>
        public void Update()
        {
            UpdateAction();
        }
    }
}