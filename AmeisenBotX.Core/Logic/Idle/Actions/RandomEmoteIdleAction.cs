using AmeisenBotX.Wow.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Logic.Idle.Actions
{
    /// <summary>
    /// Represents an idle action that randomly performs emotes.
    /// </summary>
    public class RandomEmoteIdleAction : IIdleAction
    {
        /// <summary>
        /// Initializes a new instance of the RandomEmoteIdleAction class with the specified AmeisenBotInterfaces object.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object to assign to the Bot property.</param>
        public RandomEmoteIdleAction(AmeisenBotInterfaces bot)
        {
            Bot = bot;

            Emotes = new List<string>()
            {
                "flex",
                "train",
                "joke",
                "laugh",
                "dance",
                "sit",
                "sleep",
            };

            EmotesWithInteraction = new List<string>()
            {
                "hi",
                "wink",
                "salute",
                "fart",
                "flex",
                "laugh",
                "rude",
                "roar",
                "applaud",
                "shy",
            };

            Rnd = new Random();
        }

        /// <summary>
        /// Gets or sets whether the autopilot mode is enabled or not.
        /// </summary>
        /// <value>
        ///  Returns false, indicating that the autopilot mode is not the only mode available.
        /// </value>
        public bool AutopilotOnly => false;

        /// <summary>
        /// Gets or sets the AmeisenBotInterfaces Bot.
        /// </summary>
        public AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets or sets the cooldown time.
        /// </summary>
        public DateTime Cooldown { get; set; }

        /// <summary>
        /// Gets or sets the list of emotes.
        /// </summary>
        public List<string> Emotes { get; }

        /// <summary>
        /// Gets the list of emotes with interaction.
        /// </summary>
        public List<string> EmotesWithInteraction { get; }

        /// <summary>
        /// Gets the maximum cooldown value in milliseconds.
        /// </summary>
        public int MaxCooldown => 168 * 1000;

        /// <summary>
        /// Gets the maximum duration, which is 0.
        /// </summary>
        public int MaxDuration => 0;

        /// <summary>
        /// Gets the minimum cooldown in milliseconds.
        /// </summary>
        public int MinCooldown => 42 * 1000;

        /// <summary>
        /// Gets the minimum duration, which is always 0.
        /// </summary>
        public int MinDuration => 0;

        /// <summary>
        /// Gets the instance of the Random class.
        /// </summary>
        private Random Rnd { get; }

        /// <summary>
        /// Enters a specific location.
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        public bool Enter()
        {
            return true;
        }

        /// <summary>
        /// Executes a series of actions based on the current state of the bot and its surrounding friends.
        /// If there are friends within a certain radius and a randomly generated number is greater than 0.5,
        /// the bot will select a random friend and perform the following actions:
        /// - Change the target to the selected friend.
        /// - Face the direction from the bot's current position to the selected friend's position.
        /// - Send a chat message using a randomly selected emote with interaction.
        /// If there are no friends around or the randomly generated number is not greater than 0.5,
        /// the bot will send a chat message using a randomly selected emote.
        /// </summary>
        public void Execute()
        {
            IEnumerable<IWowPlayer> friendsAroundMe = Bot.GetNearFriends<IWowPlayer>(Bot.Player.Position, 24.0f)
                .Where(e => e.Guid != Bot.Wow.PlayerGuid && Bot.Objects.PartymemberGuids.Contains(e.Guid));

            if (friendsAroundMe.Any() && Rnd.NextDouble() > 0.5)
            {
                IWowPlayer player = friendsAroundMe.ElementAt(Rnd.Next(0, friendsAroundMe.Count()));

                if (Bot.Wow.TargetGuid != player.Guid)
                {
                    Bot.Wow.ChangeTarget(player.Guid);
                    Bot.Wow.FacePosition(Bot.Player.BaseAddress, Bot.Player.Position, player.Position, true);
                }

                Bot.Wow.SendChatMessage($"/{EmotesWithInteraction[Rnd.Next(0, EmotesWithInteraction.Count)]}");
            }
            else
            {
                Bot.Wow.SendChatMessage($"/{Emotes[Rnd.Next(0, Emotes.Count)]}");
            }
        }

        /// <summary>
        /// Overrides the default ToString() method to return a string representation of the object.
        /// Returns a random emote, with the option to include an autopilot indicator.
        /// </summary>
        public override string ToString()
        {
            return $"{(AutopilotOnly ? "(🤖) " : "")}Random Emote";
        }
    }
}