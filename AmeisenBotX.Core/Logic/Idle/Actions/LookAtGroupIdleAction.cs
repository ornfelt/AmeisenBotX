using AmeisenBotX.Common.Math;
using AmeisenBotX.Wow.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Logic.Idle.Actions
{
    public class LookAtGroupIdleAction : IIdleAction
    {
        /// <summary>
        /// Initializes a new instance of the LookAtGroupIdleAction class.
        /// </summary>
        /// <param name="bot">The bot object implementing the AmeisenBotInterfaces.</param>
        public LookAtGroupIdleAction(AmeisenBotInterfaces bot)
        {
            Bot = bot;
            Rnd = new Random();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the autopilot is the only option.
        /// </summary>
        /// <value>
        /// Returns false indicating that the autopilot is not the only option.
        /// </value>
        public bool AutopilotOnly => false;

        /// <summary>
        /// Gets the Bot property of the AmeisenBotInterfaces class.
        /// </summary>
        public AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets or sets the cooldown time.
        /// </summary>
        public DateTime Cooldown { get; set; }

        /// <summary>
        /// Gets the maximum cooldown value.
        /// </summary>
        public int MaxCooldown => 29 * 1000;

        /// <summary>
        /// Gets the maximum duration, which is always 0.
        /// </summary>
        public int MaxDuration => 0;

        /// <summary>
        /// Gets the minimum cooldown time in milliseconds.
        /// </summary>
        public int MinCooldown => 2 * 1000;

        /// <summary>
        /// Gets the minimum duration, which is always 0.
        /// </summary>
        public int MinDuration => 0;

        /// <summary>
        /// Gets or sets the collection of WoW units that are near the party members.
        /// </summary>
        private IEnumerable<IWowUnit> NearPartymembers { get; set; }

        /// <summary>
        /// Gets the random number generator.
        /// </summary>
        private Random Rnd { get; }

        /// <summary>
        /// Checks if the bot can enter a party.
        /// Returns true if the bot's current center party position is not at Vector3.Zero
        /// and if there is at least one party member nearby (within a distance of 12.0f) that is not the bot itself.
        /// </summary>
        public bool Enter()
        {
            return Bot.Objects.CenterPartyPosition != Vector3.Zero
                && Bot.Objects.Partymembers.Any(e => e.Guid != Bot.Wow.PlayerGuid && e.Position.GetDistance(Bot.Player.Position) < 12.0f);
        }

        /// <summary>
        /// Executes a command to make the bot's character face a random position around the center of the party.
        /// </summary>
        public void Execute()
        {
            Bot.Wow.FacePosition(Bot.Player.BaseAddress, Bot.Player.Position, BotMath.CalculatePositionAround(Bot.Objects.CenterPartyPosition, 0.0f, (float)Rnd.NextDouble() * (MathF.PI * 2), (float)Rnd.NextDouble()), true);
        }

        /// <summary>
        /// Overrides the default ToString() method.
        /// Returns a string representation of the object, appending "(🤖)" if AutopilotOnly is true.
        /// </summary>
        public override string ToString()
        {
            return $"{(AutopilotOnly ? "(🤖) " : "")}Look at Group";
        }
    }
}