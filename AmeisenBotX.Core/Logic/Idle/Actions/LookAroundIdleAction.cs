using System;

namespace AmeisenBotX.Core.Logic.Idle.Actions
{
    /// <summary>
    /// Represents a class for performing a "look around" idle action.
    /// </summary>
    public class LookAroundIdleAction : IIdleAction
    {
        /// <summary>
        /// Initializes a new instance of the LookAroundIdleAction class.
        /// </summary>
        /// <param name="bot">The bot to perform the action.</param>
        public LookAroundIdleAction(AmeisenBotInterfaces bot)
        {
            Bot = bot;
            Rnd = new Random();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Autopilot is set to only.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the Autopilot is set to only; otherwise, <c>false</c>.
        /// </value>
        public bool AutopilotOnly => false;

        /// <summary>
        /// Gets the Bot property.
        /// </summary>
        public AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets or sets the cooldown time for an action.
        /// </summary>
        public DateTime Cooldown { get; set; }

        /// <summary>
        /// Gets the maximum cooldown value in milliseconds.
        /// </summary>
        public int MaxCooldown => 49 * 1000;

        /// <summary>
        /// Gets or sets the maximum duration.
        /// </summary>
        public int MaxDuration => 0;

        /// <summary>
        /// Gets the minimum cooldown in milliseconds.
        /// </summary>
        public int MinCooldown => 2 * 1000;

        /// <summary>
        /// Gets the minimum duration, which is always 0.
        /// </summary>
        public int MinDuration => 0;

        ///<summary>
        ///Gets the instance of the Random class used for generating random numbers.
        ///</summary>
        private Random Rnd { get; }

        /// <summary>
        /// Method to simulate entering an action or condition.
        /// </summary>
        /// <returns>Boolean value indicating successful entry.</returns>
        public bool Enter()
        {
            return true;
        }

        /// <summary>
        /// Executes the code to modify the player's facing direction in the game.
        /// </summary>
        public void Execute()
        {
            float modificationFactor = ((float)Rnd.NextDouble() - 0.5f) / ((float)Rnd.NextDouble() * 1.2f);
            Bot.Wow.SetFacing(Bot.Player.BaseAddress, Bot.Player.Rotation + modificationFactor, true);
        }

        /// <summary>
        /// Overrides the ToString method to return the string representation of the object.
        /// If AutopilotOnly is true, it returns a string appended with "(🤖)" symbol.
        /// </summary>
        /// <returns>
        /// A string representing the object. If AutopilotOnly is true, it appends "(🤖)" to the string.
        /// </returns>
        public override string ToString()
        {
            return $"{(AutopilotOnly ? "(🤖) " : "")}Look Around";
        }
    }
}