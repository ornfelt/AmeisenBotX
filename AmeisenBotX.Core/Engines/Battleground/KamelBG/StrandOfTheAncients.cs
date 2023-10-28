using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Battleground.KamelBG.Enums;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Wow.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Namespace for the KamelBG engine in the AmeisenBotX.Core.Engines.Battleground namespace.
/// </summary>
namespace AmeisenBotX.Core.Engines.Battleground.KamelBG
{
    /// <summary>
    /// Represents an implementation of the <see cref="IBattlegroundEngine"/> interface for the Strand of the Ancients battleground.
    /// </summary>
    internal class StrandOfTheAncients : IBattlegroundEngine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StrandOfTheAncients"/> class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces instance.</param>
        public StrandOfTheAncients(AmeisenBotInterfaces bot)
        {
            Bot = bot;

            CombatEvent = new(TimeSpan.FromSeconds(2));
        }

        /// <inheritdoc />
        public string Author => "Lukas";

        /// <summary>
        /// Gets the AmeisenBotInterfaces instance.
        /// </summary>
        public AmeisenBotInterfaces Bot { get; }

        /// <inheritdoc />
        public string Description => "Strand of the Ancients";

        /// <inheritdoc />
        public string Name => "Strand of the Ancients";

        /// <summary>
        /// Gets the right path waypoints for the Strand of the Ancients battleground.
        /// </summary>
        public List<Vector3> PathRight { get; } = new()
        {
            new(1403, 69, 30)
        };

        /// <summary>
        /// Represents a time-gated event for combat actions.
        /// </summary>
        private TimegatedEvent CombatEvent { get; }

        /// <summary>
        /// Handles combat logic for the Strand of the Ancients battleground.
        /// </summary>
        public void Combat()
        {
            IWowPlayer weakestPlayer = Bot.GetNearEnemies<IWowPlayer>(Bot.Player.Position, 30.0f).OrderBy(e => e.Health).FirstOrDefault();

            if (weakestPlayer != null)
            {
                double distance = weakestPlayer.Position.GetDistance(Bot.Player.Position);
                double threshold = Bot.CombatClass.IsMelee ? 3.0 : 28.0;

                if (distance > threshold)
                {
                    Bot.Movement.SetMovementAction(MovementAction.Move, weakestPlayer.Position);
                }
                else if (CombatEvent.Run())
                {
                    // StateMachine.Get<StateCombat>().Mode = CombatMode.Force;
                    Bot.Wow.ChangeTarget(weakestPlayer.Guid);
                }
            }
            else
            {
            }
        }

        /// <inheritdoc />
        public void Enter()
        {
        }

        /// <inheritdoc />
        public void Execute()
        {
            Combat();

            if (Bot.Objects.Vehicle == null)
            {
                IWowGameobject VehicleNode = Bot.Objects.All
                    .OfType<IWowGameobject>()
                    .Where(x => Enum.IsDefined(typeof(Vehicle), x.DisplayId)
                            && x.Position.GetDistance(Bot.Player.Position) < 20)
                    .OrderBy(x => x.Position.GetDistance(Bot.Player.Position))
                    .FirstOrDefault();

                if (VehicleNode != null)
                {
                    Bot.Movement.SetMovementAction(MovementAction.Move, VehicleNode.Position);

                    if (Bot.Player.Position.GetDistance(VehicleNode.Position) <= 4)
                    {
                        Bot.Movement.StopMovement();

                        Bot.Wow.InteractWithObject(VehicleNode);
                    }
                }
            }
            else
            {
                Vector3 currentNode = PathRight[0];
                Bot.Movement.SetMovementAction(MovementAction.Move, currentNode);
            }
        }

        /// <inheritdoc />
        public void Reset()
        {
        }
    }
}