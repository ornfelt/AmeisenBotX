using AmeisenBotX.Common.Math;
using AmeisenBotX.Wow.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Logic.Idle.Actions
{
    /// <summary>
    /// A class that represents an idle action where the character looks at nearby NPCs.
    /// </summary>
    public class LookAtNpcsIdleAction : IIdleAction
    {
        /// <summary>
        /// Initializes a new instance of the LookAtNpcsIdleAction class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object to use.</param>
        public LookAtNpcsIdleAction(AmeisenBotInterfaces bot)
        {
            Bot = bot;
            Rnd = new Random();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the autopilot mode is exclusive.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the autopilot mode is exclusive; otherwise, <c>false</c>.
        /// </value>
        public bool AutopilotOnly => false;

        /// <summary>
        /// Gets the AmeisenBotInterfaces object for this instance.
        /// </summary>
        public AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets or sets the cooldown time.
        /// </summary>
        public DateTime Cooldown { get; set; }

        /// <summary>
        /// Gets the maximum cooldown in milliseconds.
        /// </summary>
        public int MaxCooldown => 28 * 1000;

        ///<summary>Returns the maximum duration.</summary>
        public int MaxDuration => 0;

        /// <summary>
        /// Gets the minimum cooldown in milliseconds.
        /// </summary>
        public int MinCooldown => 9 * 1000;

        /// <summary>
        /// Gets the minimum duration which is always 0.
        /// </summary>
        public int MinDuration => 0;

        /// <summary>
        /// Gets or sets the collection of non-player character (NPC) units near the current object.
        /// </summary>
        /// <remarks>
        /// The collection represents an enumeration of objects implementing the <see cref="IWowUnit"/> interface.
        /// </remarks>
        private IEnumerable<IWowUnit> NpcsNearMe { get; set; }

        /// <summary>
        /// Gets the random number generator instance.
        /// </summary>
        private Random Rnd { get; }

        /// <summary>
        /// Method to check if there are any nearby NPCs within a 12.0f distance from the player's position.
        /// </summary>
        /// <returns>True if there are nearby NPCs, otherwise false.</returns>
        public bool Enter()
        {
            NpcsNearMe = Bot.Objects.All.OfType<IWowUnit>().Where(e => e.Position.GetDistance(Bot.Player.Position) < 12.0f);
            return NpcsNearMe.Any();
        }

        /// <summary>
        /// Executes the code to make the bot character face a random party member.
        /// </summary>
        public void Execute()
        {
            IWowUnit randomPartymember = NpcsNearMe.ElementAt(Rnd.Next(0, NpcsNearMe.Count()));

            if (randomPartymember != null)
            {
                Bot.Wow.FacePosition(Bot.Player.BaseAddress, Bot.Player.Position, BotMath.CalculatePositionAround(randomPartymember.Position, 0.0f, (float)Rnd.NextDouble() * (MathF.PI * 2), (float)Rnd.NextDouble()), true);
            }
        }

        /// <summary>
        /// Overrides the ToString method and returns a string that includes a flag emoji if AutopilotOnly is true,
        /// indicating that only NPCs are being looked at.
        /// </summary>
        public override string ToString()
        {
            return $"{(AutopilotOnly ? "(🤖) " : "")}Look at NPCs";
        }
    }
}