using AmeisenBotX.Common.Math;
using System;

namespace AmeisenBotX.Core.Logic.Idle.Actions
{
    /// <summary>
    /// Initializes a new instance of the LookAtTargetIdleAction class.
    /// </summary>
    /// <param name="bot">The bot object.</param>
    public class LookAtTargetIdleAction : IIdleAction
    {
        /// <summary>
        /// Initializes a new instance of the LookAtTargetIdleAction class.
        /// </summary>
        /// <param name="bot">The bot object.</param>
        public LookAtTargetIdleAction(AmeisenBotInterfaces bot)
        {
            Bot = bot;
            Rnd = new Random();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the AutopilotOnly mode is enabled.
        /// </summary>
        /// <returns>
        ///   <c>false</c> always, as the AutopilotOnly mode is not enabled.
        /// </returns>
        public bool AutopilotOnly => false;

        /// <summary>
        /// Gets the instance of the AmeisenBotInterfaces.
        /// </summary>
        /// <returns>The instance of the AmeisenBotInterfaces.</returns>
        public AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets or sets the cooldown time.
        /// </summary>
        public DateTime Cooldown { get; set; }

        /// <summary>
        /// Gets the maximum cooldown value in milliseconds.
        /// </summary>
        public int MaxCooldown => 38 * 1000;

        /// <summary>
        /// Represents the maximum duration value.
        /// </summary>
        public int MaxDuration => 0;

        /// <summary>
        /// Gets the minimum cooldown in milliseconds.
        /// </summary>
        public int MinCooldown => 12 * 1000;

        /// <summary>
        /// Gets the minimum duration, which is always 0.
        /// </summary>
        public int MinDuration => 0;

        /// <summary>
        /// Gets the instance of Random used in the class.
        /// </summary>
        private Random Rnd { get; }

        /// <summary>
        /// Checks if the bot has a target and returns true if it does.
        /// </summary>
        public bool Enter()
        {
            return Bot.Target != null;
        }

        /// <summary>
        /// Executes the current action for the bot.
        /// If the target is not null, the bot faces a new position calculated using the current player's position and a random angle around the target's position.
        /// </summary>
        public void Execute()
        {
            if (Bot.Target != null)
            {
                Bot.Wow.FacePosition(Bot.Player.BaseAddress, Bot.Player.Position, BotMath.CalculatePositionAround(Bot.Target.Position, 0.0f, (float)Rnd.NextDouble() * (MathF.PI * 2), (float)Rnd.NextDouble()), true);
            }
        }

        /// <summary>
        /// Overrides the default ToString() method and returns a string representation of the object.
        /// If AutopilotOnly is true, adds "(🤖)" prefix to the string.
        /// The returned string is "Look at Target".
        /// </summary>
        public override string ToString()
        {
            return $"{(AutopilotOnly ? "(🤖) " : "")}Look at Target";
        }
    }
}