using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Battleground.KamelBG.Enums;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Wow.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Namespace containing the implementation of the Eye of the Storm battleground engine.
/// </summary>
namespace AmeisenBotX.Core.Engines.Battleground.KamelBG
{
    /// <summary>
    /// Represents an implementation of the <see cref="IBattlegroundEngine"/> interface for the Eye of the Storm battleground.
    /// </summary>
    internal class EyeOfTheStorm : IBattlegroundEngine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EyeOfTheStorm"/> class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces instance.</param>
        public EyeOfTheStorm(AmeisenBotInterfaces bot)
        {
            Bot = bot;

            CaptureFlagEvent = new(TimeSpan.FromSeconds(1));
            CombatEvent = new(TimeSpan.FromSeconds(2));
        }

        /// <inheritdoc />
        public string Author => "Lukas";

        /// <inheritdoc />
        public string Description => "Eye of the Storm";

        /// <inheritdoc />
        public string Name => "Eye of the Storm";

        /// <summary>
        /// Gets the base path waypoints for the Eye of the Storm battleground.
        /// </summary>
        public List<Vector3> PathBase { get; } = new List<Vector3>()
        {
            new Vector3(2284, 1731, 1189),//Mage Tower
            new Vector3(2286, 1402, 1197),//Draenei Ruins
            new Vector3(2048, 1393, 1194),//Blood Elf Tower
            new Vector3(2043, 1729, 1189)//Fel Reaver Ruins
        };

        /// <summary>
        /// Gets the flag path waypoints for the Eye of the Storm battleground.
        /// </summary>
        public List<Vector3> PathFlag { get; } = new List<Vector3>()
        {
            new Vector3(2176, 1570, 1159)//Flag
        };

        /// <summary>
        /// Gets the AmeisenBotInterfaces instance used by the Eye of the Storm engine.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets the time-gated event for capturing the flag.
        /// </summary>
        private TimegatedEvent CaptureFlagEvent { get; }

        /// <summary>
        /// Gets the time-gated event for combat actions.
        /// </summary>
        private TimegatedEvent CombatEvent { get; }

        /// <summary>
        /// Gets or sets the current node counter used for navigating the base path waypoints.
        /// </summary>
        private int CurrentNodeCounter { get; set; }

        /// <summary>
        /// Gets or sets the faction flag state (e.g., "Alliance Controlled" or "Horde Controlled").
        /// </summary>
        private string FactionFlagState { get; set; }

        /// <summary>
        /// Handles combat logic for the Eye of the Storm battleground.
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
            Faction();
        }

        /// <inheritdoc />
        public void Execute()
        {
            Combat();

            IWowGameobject FlagNode = Bot.Objects.All
                .OfType<IWowGameobject>()
                .Where(x => Enum.IsDefined(typeof(Flags), x.DisplayId)
                        && x.Position.GetDistance(Bot.Player.Position) < 15)
                .OrderBy(x => x.Position.GetDistance(Bot.Player.Position))
                .FirstOrDefault();

            if (FlagNode != null)
            {
                Bot.Movement.SetMovementAction(MovementAction.Move, FlagNode.Position);

                if (Bot.Player.Position.GetDistance(FlagNode.Position) <= 4)
                {
                    Bot.Movement.StopMovement();

                    if (CaptureFlagEvent.Run())
                    {
                        Bot.Wow.InteractWithObject(FlagNode);
                    }
                }
                else
                {
                    Bot.Movement.SetMovementAction(MovementAction.Move, FlagNode.Position);
                }
            }
            else
            {
                if (Bot.Wow.ExecuteLuaAndRead(BotUtils.ObfuscateLua("{v:0}=\"\" for i = 1, GetNumMapLandmarks(), 1 do base, status = GetMapLandmarkInfo(i) {v:0}= {v:0}..base..\":\"..status..\";\" end"), out string result))
                {
                    Vector3 currentNode = PathBase[CurrentNodeCounter];
                    string[] AllBaseList = result.Split(';');

                    if (Bot.Player.HasBuffById(34976))
                    {
                        if (AllBaseList[CurrentNodeCounter].Contains(FactionFlagState))
                        {
                            Bot.Movement.SetMovementAction(MovementAction.Move, currentNode);
                        }
                        else
                        {
                            ++CurrentNodeCounter;
                            if (CurrentNodeCounter >= PathBase.Count)
                            {
                                CurrentNodeCounter = 0;
                            }
                        }
                    }
                    else
                    {
                        if (AllBaseList[CurrentNodeCounter].Contains("Uncontrolled")
                            || AllBaseList[CurrentNodeCounter].Contains("In Conflict")
                            || AllBaseList[CurrentNodeCounter].Contains(FactionFlagState))
                        {
                            Bot.Movement.SetMovementAction(MovementAction.Move, currentNode);
                        }

                        if (Bot.Player.Position.GetDistance(currentNode) < 10.0f)
                        {
                            ++CurrentNodeCounter;

                            if (CurrentNodeCounter >= PathBase.Count)
                            {
                                CurrentNodeCounter = 0;
                            }
                        }
                        else if (FactionFlagState != null && AllBaseList[CurrentNodeCounter].Contains(FactionFlagState))
                        {
                            ++CurrentNodeCounter;
                            if (CurrentNodeCounter >= PathBase.Count)
                            {
                                CurrentNodeCounter = 0;
                            }
                        }
                        else if (FlagNode != null)
                        {
                            IEnumerable<IWowPlayer> enemiesNearFlag = Bot.GetNearEnemies<IWowPlayer>(FlagNode.Position, 40);
                            IEnumerable<IWowPlayer> friendsNearFlag = Bot.GetNearFriends<IWowPlayer>(FlagNode.Position, 40);
                            IEnumerable<IWowPlayer> friendsNearPlayer = Bot.GetNearFriends<IWowPlayer>(Bot.Player.Position, 20);

                            if (enemiesNearFlag != null)
                            {
                                if (enemiesNearFlag.Count() >= 2)
                                {
                                    if (friendsNearFlag != null && (friendsNearFlag.Any() || friendsNearPlayer.Any()))
                                    {
                                        Bot.Movement.SetMovementAction(MovementAction.Move, currentNode);
                                        return;
                                    }
                                }
                                else
                                {
                                    Bot.Movement.SetMovementAction(MovementAction.Move, currentNode);
                                    return;
                                }
                            }
                        }
                        else
                        {
                            Bot.Movement.SetMovementAction(MovementAction.Move, currentNode);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Determines the faction's flag state.
        /// </summary>
        public void Faction()
        {
            if (!Bot.Player.IsHorde())
            {
                FactionFlagState = "Alliance Controlled";
            }
            else
            {
                FactionFlagState = "Hord Controlled";
            }
        }

        /// <inheritdoc />
        public void Reset()
        {
        }
    }
}