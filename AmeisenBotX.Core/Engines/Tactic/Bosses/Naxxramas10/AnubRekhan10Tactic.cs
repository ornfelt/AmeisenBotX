using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Storage;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using AmeisenBotX.Wow335a.Constants.Raids.Wotlk.Naxxramas;
using System;
using System.Collections.Generic;

/// <summary>
/// Contains tactics for bosses in the 10-man version of the Naxxramas raid in AmeisenBotX.
/// </summary>
namespace AmeisenBotX.Core.Engines.Tactic.Bosses.Naxxramas10
{
    /// <summary>
    /// Represents a tactic for defeating Anub'Rekhan in the 10-man raid version.
    /// </summary>
    public class AnubRekhan10Tactic : SimpleConfigurable, ITactic
    {
        /// <summary>
        /// Initializes a new instance of the AnubRekhan10Tactic class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object.</param>
        public AnubRekhan10Tactic(AmeisenBotInterfaces bot)
        {
            Bot = bot;
            TankingPathQueue = new();

            Configurables.TryAdd("isOffTank", false);
        }

        /// <summary>
        /// Represents the area defined by a three-dimensional vector.
        /// The default value is set to (3273, -3476, 287).
        /// </summary>
        public Vector3 Area { get; } = new(3273, -3476, 287);

        /// <summary>
        /// Gets the area radius.
        /// </summary>
        public float AreaRadius { get; } = 120.0f;

        /// <summary>
        /// Gets or sets the date and time when the locust swarm was activated.
        /// </summary>
        public DateTime LocustSwarmActivated { get; private set; }

        /// <summary>
        /// Gets or sets the unique identifier for the Naxxramas map in the World of Warcraft.
        /// </summary>
        public WowMapId MapId { get; } = WowMapId.Naxxramas;

        /// <summary>
        /// Gets or sets the list of display ids that will be used for addition.
        /// </summary>
        /// <value>
        /// The list of display ids.
        /// </value>
        private static List<int> AddsDisplayIds { get; } = new() { 14698, 27943 };

        /// <summary>
        /// Gets the display IDs for Anub'Rekhan.
        /// </summary>
        private static List<int> AnubRekhanDisplayId { get; } = new() { 15931 };

        /// <summary>
        /// Gets or sets the Bot instance.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets or sets the position for impale dodge in 3D space.
        /// </summary>
        private Vector3 ImpaleDodgePos { get; set; }

        ///<summary>
        ///Checks if the Locust Swarm is currently active by comparing the activation time
        ///with the current time plus 20 seconds.
        ///</summary>
        private bool LocustSwarmActive => (LocustSwarmActivated + TimeSpan.FromSeconds(20)) > DateTime.UtcNow;

        /// <summary>
        /// Gets or sets a value indicating whether the melee DPS is moving towards the middle.
        /// </summary>
        private bool MeleeDpsIsMovingToMid { get; set; }

        /// <summary>
        /// Route used for tanking and kiting. Consists of a list of Vector3 coordinates.
        /// </summary>
        private List<Vector3> TankingKitingRouteA { get; } = new()
        {
            new Vector3(3323, -3497, 287),
            new Vector3(3312, -3514, 287),
            new Vector3(3294, -3526, 287),
            new Vector3(3273, -3530, 287),
            new Vector3(3252, -3526, 287),
            new Vector3(3235, -3514, 287),
            new Vector3(3223, -3497, 287),
            new Vector3(3220, -3484, 287),
        };

        /// <summary>
        /// TankingKitingRouteB is a list of Vector3 coordinates that represent a route for tanking and kiting.
        /// </summary>
        private List<Vector3> TankingKitingRouteB { get; } = new()
        {
            new Vector3(3223, -3455, 287),
            new Vector3(3235, -3437, 287),
            new Vector3(3252, -3425, 287),
            new Vector3(3274, -3422, 287),
            new Vector3(3294, -3425, 287),
            new Vector3(3312, -3437, 287),
            new Vector3(3324, -3456, 287),
            new Vector3(3326, -3465, 287),
        };

        /// <summary>
        /// Gets or sets the private queue of Vector3 objects representing the tanking path.
        /// </summary>
        private Queue<Vector3> TankingPathQueue { get; }

        /// <summary>
        /// The coordinates of Tanking Spot A.
        /// </summary>
        private Vector3 TankingSpotA { get; } = new(3325, -3486, 287);

        /// <summary>
        /// The coordinates of the designated tanking spot for the tank object.
        /// The tanking spot is located at x = 3222, y = -3464, and z = 287.
        /// </summary>
        private Vector3 TankingSpotB { get; } = new(3222, -3464, 287);

        /// <summary>
        /// Gets or sets a value indicating whether the tank is currently kiting.
        /// </summary>
        private bool TankIsKiting { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tank is using A.
        /// </summary>
        private bool TankIsUsingA { get; set; } = true;

        /// <summary>
        /// Executes a tactic based on the specified role in World of Warcraft.
        /// </summary>
        /// <param name="role">The role of the player character.</param>
        /// <param name="isMelee">Specifies if the player character is a melee role.</param>
        /// <param name="handlesMovement">Output parameter indicating if the tactic handles movement.</param>
        /// <param name="allowAttacking">Output parameter indicating if attacking is allowed in the tactic.</param>
        /// <returns>A boolean value indicating the success of the tactic execution.</returns>
        public bool ExecuteTactic(WowRole role, bool isMelee, out bool handlesMovement, out bool allowAttacking)
        {
            return role switch
            {
                WowRole.Tank => DoTank(out handlesMovement, out allowAttacking),
                WowRole.Heal => DoDpsHeal(false, out handlesMovement, out allowAttacking),
                WowRole.Dps => DoDpsHeal(isMelee, out handlesMovement, out allowAttacking),
            };
        }

        /// <summary>
        /// Performs a DPS heal strategy for Anub'Rekhan encounter.
        /// </summary>
        /// <param name="isMelee">Specifies if the player is a melee DPS.</param>
        /// <param name="handlesMovement">Outputs a value indicating whether the function handles movement.</param>
        /// <param name="allowAttacking">Outputs a value indicating whether attacking is allowed.</param>
        /// <returns>True if a specific action is taken, otherwise false.</returns>
        private bool DoDpsHeal(bool isMelee, out bool handlesMovement, out bool allowAttacking)
        {
            IWowUnit anubrekhan = Bot.GetClosestQuestGiverByDisplayId(Bot.Player.Position, AnubRekhanDisplayId, false);

            if (anubrekhan != null)
            {
                handlesMovement = true;
                allowAttacking = true;

                // Locust Swarm
                if (anubrekhan.CurrentlyCastingSpellId == AnubRekhan335a.LocustSwarmSpellId)
                {
                    LocustSwarmActivated = DateTime.UtcNow;
                    Bot.CombatClass.BlacklistedTargetDisplayIds = AnubRekhanDisplayId;

                    MeleeDpsIsMovingToMid = true;
                }

                if (!LocustSwarmActive)
                {
                    Bot.CombatClass.BlacklistedTargetDisplayIds = null;
                }

                if (!isMelee)
                {
                    if (anubrekhan.CurrentlyCastingSpellId == AnubRekhan335a.ImpaleSpellId)
                    {
                        if (ImpaleDodgePos == Vector3.Zero)
                        {
                            float angle = new Random().NextDouble() > 0.5 ? MathF.PI + (MathF.PI / 2.0f) : MathF.PI - (MathF.PI / 2.0f);
                            ImpaleDodgePos = BotMath.CalculatePositionAround(Bot.Player.Position, Bot.Player.Rotation, angle, 2.0f);
                        }

                        Bot.Movement.SetMovementAction(MovementAction.DirectMove, ImpaleDodgePos);
                        return true;
                    }
                    else
                    {
                        ImpaleDodgePos = Vector3.Zero;
                    }

                    Vector3 targetPosition = BotUtils.MoveAhead(Area, anubrekhan.Position, -30.0f);

                    if (!LocustSwarmActive && Bot.Player.Position.GetDistance(Area) > 6.0f)
                    {
                        Bot.Movement.SetMovementAction(MovementAction.Move, targetPosition);
                        return true;
                    }
                }
                else
                {
                    if (MeleeDpsIsMovingToMid)
                    {
                        if (Bot.Player.Position.GetDistance(Area) > 24.0f)
                        {
                            Bot.Movement.SetMovementAction(MovementAction.Move, Area);
                            return true;
                        }
                        else
                        {
                            MeleeDpsIsMovingToMid = false;
                        }
                    }

                    handlesMovement = false;
                    allowAttacking = true;
                    return false;
                }
            }

            handlesMovement = false;
            allowAttacking = true;
            return false;
        }

        /// <summary>
        /// Performs tank actions in combat, including movement and attacking.
        /// </summary>
        /// <param name="handlesMovement">Indicates if the tank is responsible for movement.</param>
        /// <param name="allowAttacking">Indicates if the tank is allowed to attack.</param>
        /// <returns>Returns true if tank actions are successfully performed, otherwise false.</returns>
        private bool DoTank(out bool handlesMovement, out bool allowAttacking)
        {
            handlesMovement = false;
            allowAttacking = true;

            IWowUnit anubrekhan = Bot.GetClosestQuestGiverByDisplayId(Bot.Player.Position, AnubRekhanDisplayId, false);

            if (anubrekhan != null)
            {
                if (Configurables["isOffTank"] == true)
                {
                    // offtank should only focus adds
                    Bot.CombatClass.BlacklistedTargetDisplayIds = AnubRekhanDisplayId;
                }
                else
                {
                    Bot.CombatClass.BlacklistedTargetDisplayIds = AddsDisplayIds;

                    // Locust Swarm
                    if (anubrekhan.CurrentlyCastingSpellId == AnubRekhan335a.LocustSwarmSpellId)
                    {
                        TankIsKiting = true;
                    }

                    if (!TankIsKiting)
                    {
                        if (anubrekhan.TargetGuid == Bot.Wow.PlayerGuid)
                        {
                            Vector3 tankingSpot = TankIsUsingA ? TankingSpotA : TankingSpotB;

                            if (Bot.Player.DistanceTo(tankingSpot) > 10.0f)
                            {
                                Bot.Movement.SetMovementAction(MovementAction.DirectMove, tankingSpot);
                                handlesMovement = true;
                            }
                            else
                            {
                                if (anubrekhan.CurrentlyCastingSpellId == AnubRekhan335a.ImpaleSpellId)
                                {
                                    if (ImpaleDodgePos == Vector3.Zero)
                                    {
                                        float angle = MathF.PI + new Random().NextDouble() > 0.5 ? BotMath.HALF_PI : -BotMath.HALF_PI;
                                        ImpaleDodgePos = BotMath.CalculatePositionAround(Bot.Player.Position, Bot.Player.Rotation, angle, 5.0f);
                                    }

                                    Bot.Movement.SetMovementAction(MovementAction.DirectMove, ImpaleDodgePos);
                                    return true;
                                }
                                else
                                {
                                    ImpaleDodgePos = Vector3.Zero;
                                }
                            }
                        }
                    }
                    else
                    {
                        allowAttacking = false;

                        if (TankingPathQueue.Count == 0)
                        {
                            foreach (Vector3 v in TankIsUsingA ? TankingKitingRouteA : TankingKitingRouteB)
                            {
                                TankingPathQueue.Enqueue(v);
                            }
                        }
                        else
                        {
                            Vector3 targetPosition = TankingPathQueue.Peek();

                            if (targetPosition.GetDistance2D(Bot.Player.Position) > 2.0f)
                            {
                                Bot.Movement.SetMovementAction(MovementAction.DirectMove, targetPosition);
                                handlesMovement = true;
                            }
                            else
                            {
                                TankingPathQueue.Dequeue();

                                if (TankingPathQueue.Count == 0)
                                {
                                    TankIsKiting = false;
                                    TankIsUsingA = !TankIsUsingA;
                                }
                            }
                        }
                    }

                    return true;
                }
            }

            return false;
        }
    }
}