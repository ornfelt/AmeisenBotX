using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Wow.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Movement.Providers.Basic
{
    public class FollowMovementProvider : IMovementProvider
    {
        /// <summary>
        /// Initializes a new instance of the FollowMovementProvider class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces representing the bot.</param>
        /// <param name="config">The AmeisenBotConfig representing the bot configuration.</param>
        public FollowMovementProvider(AmeisenBotInterfaces bot, AmeisenBotConfig config)
        {
            Bot = bot;
            Config = config;

            Random = new();
            OffsetCheckEvent = new(TimeSpan.FromMilliseconds(30000));
        }

        /// <summary>
        /// Gets or sets the instance of the AmeisenBotInterfaces.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets the configuration settings for the AmeisenBot.
        /// </summary>
        private AmeisenBotConfig Config { get; }

        /// <summary>
        /// Gets or sets the offset for following an object in a Vector3 format.
        /// </summary>
        private Vector3 FollowOffset { get; set; }

        /// <summary>
        /// Gets the TimegatedEvent used for offset checking.
        /// </summary>
        private TimegatedEvent OffsetCheckEvent { get; }

        /// <summary>Gets the instance of the Random class used for generating random numbers.</summary>
        private Random Random { get; }

        /// <summary>
        /// Checks if the bot can get the position and movement action for following a unit.
        /// </summary>
        /// <param name="position">The position of the unit to follow.</param>
        /// <param name="type">The movement action to be performed.</param>
        /// <returns>True if the position and movement action are obtained successfully, false otherwise.</returns>
        public bool Get(out Vector3 position, out MovementAction type)
        {
            if (!Bot.Player.IsDead
                && !Bot.Player.IsInCombat
                && !Config.Autopilot
                && !Bot.Player.IsGhost)
            {
                if (IsUnitToFollowThere(out IWowUnit player))
                {
                    Vector3 pos = Config.FollowPositionDynamic ? player.Position + FollowOffset : player.Position;
                    float distance = Bot.Player.DistanceTo(pos);

                    if (distance > Config.MinFollowDistance && distance <= Config.MaxFollowDistance)
                    {
                        if (Config.FollowPositionDynamic && OffsetCheckEvent.Run())
                        {
                            float factor = Bot.Player.IsOutdoors ? 2.0f : 1.0f;

                            FollowOffset = new()
                            {
                                X = ((float)Random.NextDouble() * ((float)Config.MinFollowDistance * factor) - ((float)Config.MinFollowDistance * (0.5f * factor))) * 0.7071f,
                                Y = ((float)Random.NextDouble() * ((float)Config.MinFollowDistance * factor) - ((float)Config.MinFollowDistance * (0.5f * factor))) * 0.7071f,
                                Z = 0.0f
                            };
                        }

                        type = MovementAction.Move;
                        position = pos;
                        return true;
                    }
                }
            }

            type = MovementAction.None;
            position = Vector3.Zero;
            return false;
        }

        /// <summary>
        /// Determines if there is a unit to follow and assigns it to the out parameter playerToFollow.
        /// </summary>
        /// <param name="playerToFollow">The unit that will be followed.</param>
        /// <param name="ignoreRange">Optional. Flag to ignore range when determining if a player should be followed, default is false.</param>
        /// <returns>Returns true if there is a unit to follow, false otherwise.</returns>
        private bool IsUnitToFollowThere(out IWowUnit playerToFollow, bool ignoreRange = false)
        {
            IEnumerable<IWowPlayer> wowPlayers = Bot.Objects.All.OfType<IWowPlayer>().Where(e => !e.IsDead);

            if (wowPlayers.Any())
            {
                IWowUnit[] playersToTry =
                {
                    Config.FollowSpecificCharacter ? wowPlayers.FirstOrDefault(p => Bot.Db.GetUnitName(p, out string name) && name.Equals(Config.SpecificCharacterToFollow, StringComparison.OrdinalIgnoreCase)) : null,
                    Config.FollowGroupLeader ? Bot.Objects.Partyleader : null,
                    Config.FollowGroupMembers ? Bot.Objects.Partymembers.FirstOrDefault() : null
                };

                foreach (IWowUnit unit in playersToTry)
                {
                    if (unit == null || (!ignoreRange && !ShouldIFollowPlayer(unit)))
                    {
                        continue;
                    }

                    playerToFollow = unit;
                    return true;
                }
            }

            playerToFollow = null;
            return false;
        }

        /// <summary>
        /// Determines whether the bot should follow a player.
        /// </summary>
        /// <param name="playerToFollow">The player to follow.</param>
        /// <returns>True if the bot should follow the player, otherwise false.</returns>
        private bool ShouldIFollowPlayer(IWowUnit playerToFollow)
        {
            if (playerToFollow == null)
            {
                return false;
            }

            Vector3 pos = Config.FollowPositionDynamic ? playerToFollow.Position + FollowOffset : playerToFollow.Position;
            double distance = Bot.Player.DistanceTo(pos);

            return distance > Config.MinFollowDistance && distance < Config.MaxFollowDistance;
        }
    }
}