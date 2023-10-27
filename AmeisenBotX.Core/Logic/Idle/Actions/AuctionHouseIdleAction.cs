using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Wow.Cache.Enums;
using AmeisenBotX.Wow.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Logic.Idle.Actions
{
    public class AuctionHouseIdleAction : IIdleAction
    {
        /// <summary>
        /// Creates a new instance of the AuctionHouseIdleAction class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object representing the bot.</param>
        public AuctionHouseIdleAction(AmeisenBotInterfaces bot)
        {
            Bot = bot;
            Rnd = new Random();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Autopilot is the only mode available.
        /// </summary>
        public bool AutopilotOnly => true;

        /// <summary>
        /// Gets or sets the cooldown time.
        /// </summary>
        public DateTime Cooldown { get; set; }

        /// <summary>
        /// Gets the maximum cooldown in milliseconds.
        /// </summary>
        public int MaxCooldown => 5 * 60 * 1000;

        /// <summary>
        /// Gets the maximum duration in milliseconds.
        /// </summary>
        public int MaxDuration => 3 * 60 * 1000;

        /// <summary>
        /// Gets the minimum cooldown duration in milliseconds.
        /// </summary>
        public int MinCooldown => 4 * 60 * 1000;

        /// <summary>
        /// Gets the minimum duration in milliseconds.
        /// </summary>
        public int MinDuration => 2 * 60 * 1000;

        /// <summary>
        /// Gets or sets the time when the auctioneer talks.
        /// </summary>
        private DateTime AuctioneerTalkTime { get; set; }

        /// <summary>
        /// Gets or sets the instance of the AmeisenBotInterfaces class representing the bot.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets or sets the current auctioneer's Vector3 position.
        /// </summary>
        private Vector3 CurrentAuctioneer { get; set; }

        /// <summary>
        /// Gets or sets the origin position as a Vector3.
        /// </summary>
        private Vector3 OriginPosition { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the object has returned to its original state.
        /// </summary>
        private bool ReturnedToOrigin { get; set; }

        /// <summary>
        /// Gets the instance of the Random class used for generating random numbers.
        /// </summary>
        private Random Rnd { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the player has talked to the auctioneer.
        /// </summary>
        private bool TalkedToAuctioneer { get; set; }

        ///<summary>
        ///Enters the auction.
        ///</summary>
        public bool Enter()
        {
            TalkedToAuctioneer = false;
            AuctioneerTalkTime = default;
            OriginPosition = Bot.Player.Position;

            if (Bot.Db.TryGetPointsOfInterest(Bot.Objects.MapId, PoiType.Auctioneer, Bot.Player.Position, 256.0f, out IEnumerable<Vector3> auctioneers))
            {
                CurrentAuctioneer = Bot.PathfindingHandler.GetRandomPointAround((int)Bot.Objects.MapId, auctioneers.OrderBy(e => e.GetDistance(Bot.Player.Position)).First(), 2.5f);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Executes the action of interacting with an auctioneer.
        /// If the player has not talked to the auctioneer yet, it checks if the player's distance from the current auctioneer is greater than 3.2f.
        /// If it is, the bot sets the movement action to move towards the current auctioneer.
        /// If not, the bot stops its movement, finds the closest auctioneer within a distance of 1.0f, faces towards the auctioneer, and interacts with it.
        /// Sets TalkedToAuctioneer to true and sets AuctioneerTalkTime to a random time between 2 to 3 minutes from the current time.
        /// If the player has already talked to the auctioneer and it is time to return to the origin position, it checks if the player's distance from the origin position is greater than 8.0f.
        /// If it is, the bot sets the movement action to move towards the origin position.
        /// If not, the bot stops its movement and sets ReturnedToOrigin to true.
        /// </summary>
        public void Execute()
        {
            if (!TalkedToAuctioneer)
            {
                if (CurrentAuctioneer.GetDistance(Bot.Player.Position) > 3.2f)
                {
                    Bot.Movement.SetMovementAction(MovementAction.Move, CurrentAuctioneer);
                }
                else
                {
                    Bot.Movement.StopMovement();

                    IWowUnit auctioneer = Bot.Objects.All.OfType<IWowUnit>()
                        .FirstOrDefault(e => e.IsAuctioneer && e.Position.GetDistance(CurrentAuctioneer) < 1.0f);

                    if (auctioneer != null)
                    {
                        Bot.Wow.FacePosition(Bot.Player.BaseAddress, Bot.Player.Position, auctioneer.Position);
                        Bot.Wow.InteractWithUnit(auctioneer);
                    }

                    TalkedToAuctioneer = true;
                    AuctioneerTalkTime = DateTime.UtcNow + TimeSpan.FromSeconds(Rnd.Next(120, 180));
                }
            }
            else if (!ReturnedToOrigin && AuctioneerTalkTime < DateTime.UtcNow)
            {
                if (CurrentAuctioneer.GetDistance(OriginPosition) > 8.0f)
                {
                    Bot.Movement.SetMovementAction(MovementAction.Move, OriginPosition);
                }
                else
                {
                    Bot.Movement.StopMovement();
                    ReturnedToOrigin = true;
                }
            }
        }

        /// <summary>
        /// Returns a string representation of the object. If the AutopilotOnly property is set to true, 
        /// it adds a prefix of "(🤖)" to the string before indicating the action of going to the Auctionhouse.
        /// </summary>
        public override string ToString()
        {
            return $"{(AutopilotOnly ? "(🤖) " : "")}Go to Auctionhouse";
        }
    }
}