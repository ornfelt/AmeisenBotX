using AmeisenBotX.Common.Math;
using AmeisenBotX.Wow.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Logic.Idle.Actions
{
    /// <summary>
    /// Initializes a new instance of the LookAtGroupmemberIdleAction class.
    /// </summary>
    public class LookAtGroupmemberIdleAction : IIdleAction
    {
        /// <summary>
        /// Initializes a new instance of the LookAtGroupmemberIdleAction class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object used for the action.</param>
        public LookAtGroupmemberIdleAction(AmeisenBotInterfaces bot)
        {
            Bot = bot;
            Rnd = new Random();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the autopilot is enabled for the current operation mode.
        /// </summary>
        /// <value>
        ///   <c>true</c> if autopilot is enabled only; otherwise, <c>false</c>.
        /// </value>
        public bool AutopilotOnly => false;

        /// <summary>
        /// Gets or sets the AmeisenBotInterfaces object for the bot.
        /// </summary>
        public AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets or sets the cooldown of the DateTime.
        /// </summary>
        public DateTime Cooldown { get; set; }

        /// <summary>
        /// Gets the maximum cooldown value.
        /// </summary>
        public int MaxCooldown => 32 * 1000;

        /// <summary>
        /// Gets the maximum duration.
        /// </summary>
        public int MaxDuration => 0;

        /// <summary>
        /// Gets the minimum cooldown in milliseconds.
        /// </summary>
        public int MinCooldown => 3 * 1000;

        /// <summary>
        /// Gets the minimum duration value, which is always 0.
        /// </summary>
        public int MinDuration => 0;

        /// <summary>
        /// Gets or sets the collection of <see cref="IWowUnit"/> objects representing the party members near the current unit and facing it.
        /// </summary>
        private IEnumerable<IWowUnit> NearPartymembersFacingMe { get; set; }

        /// <summary>
        /// Gets the instance of the Random class used for generating random numbers.
        /// </summary>
        private Random Rnd { get; }

        /// <summary>
        /// Checks if there are any party members near the player and facing towards them.
        /// </summary>
        /// <returns>Returns true if there are party members near and facing towards the player. Otherwise, returns false.</returns>
        public bool Enter()
        {
            NearPartymembersFacingMe = Bot.Objects.Partymembers.Where(e => e.Guid != Bot.Wow.PlayerGuid && e.Position.GetDistance(Bot.Player.Position) < 12.0f && BotMath.IsFacing(e.Position, e.Rotation, Bot.Player.Position));
            return NearPartymembersFacingMe.Any();
        }

        /// <summary>
        /// Executes the code to face a random party member.
        /// </summary>
        public void Execute()
        {
            IWowUnit randomPartymember = NearPartymembersFacingMe.ElementAt(Rnd.Next(0, NearPartymembersFacingMe.Count()));

            if (randomPartymember != null)
            {
                Bot.Wow.FacePosition(Bot.Player.BaseAddress, Bot.Player.Position, BotMath.CalculatePositionAround(randomPartymember.Position, 0.0f, (float)Rnd.NextDouble() * (MathF.PI * 2), (float)Rnd.NextDouble()), true);
            }
        }

        /// <summary>
        /// Overrides the ToString() method to return a formatted string representing the object.
        /// </summary>
        /// <returns>
        /// A string representation of the object. If AutopilotOnly is true, it includes the 🤖 emoji; otherwise, only the "Look at Group" text is returned.
        /// </returns>
        public override string ToString()
        {
            return $"{(AutopilotOnly ? "(🤖) " : "")}Look at Group";
        }
    }
}