using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Core.Engines.Movement.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Movement
{
    public class MovementEngine : IMovementEngine
    {
        /// <summary>
        /// Initializes a new instance of the MovementEngine class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object representing the bot.</param>
        /// <param name="config">The AmeisenBotConfig object representing the bot's configuration.</param>
        public MovementEngine(AmeisenBotInterfaces bot, AmeisenBotConfig config)
        {
            Bot = bot;
            Config = config;

            FindPathEvent = new(TimeSpan.FromMilliseconds(500));
            RefreshPathEvent = new(TimeSpan.FromMilliseconds(500));
            DistanceMovedCheckEvent = new(TimeSpan.FromMilliseconds(500));

            PathQueue = new();
            PlacesToAvoidList = new();

            PlayerVehicle = new(bot);
        }

        /// <summary>
        /// Gets or sets the current speed.
        /// </summary>
        public float CurrentSpeed { get; private set; }

        /// <summary>
        /// Checks if the current time in UTC is greater than the value of 'MovementBlockedUntil'.
        /// </summary>
        public bool IsAllowedToMove => DateTime.UtcNow > MovementBlockedUntil;

        /// <summary>
        /// Gets or sets a value indicating whether the object is currently moving.
        /// </summary>
        public bool IsMoving { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the object is currently in the process of unstucking.
        /// </summary>
        public bool IsUnstucking { get; private set; }

        /// <summary>
        /// Gets the date and time of the last movement.
        /// </summary>
        public DateTime LastMovement { get; private set; }

        /// <summary>
        /// Gets the last position of the Vector3.
        /// </summary>
        public Vector3 LastPosition { get; private set; }

        /// <summary>
        /// Gets the path of Vector3 objects stored in the PathQueue property.
        /// </summary>
        public IEnumerable<Vector3> Path => PathQueue;

        ///<summary>
        /// Returns a collection of places to avoid, each consisting of a position and radius.
        ///</summary>
        public IEnumerable<(Vector3 position, float radius)> PlacesToAvoid => PlacesToAvoidList.Where(e => DateTime.UtcNow <= e.until).Select(e => (e.position, e.radius));

        /// <summary>
        /// Gets or sets the status of the Movement Action.
        /// </summary>
        public MovementAction Status { get; private set; }

        /// <summary>
        /// Gets or sets the target position for unstucking.
        /// </summary>
        public Vector3 UnstuckTarget { get; private set; }

        /// <summary>
        /// Gets or sets the interface for the AmeisenBot.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// Gets the instance of the AmeisenBotConfig class.
        /// </summary>
        private AmeisenBotConfig Config { get; }

        /// <summary>
        /// Gets the private TimegatedEvent DistanceMovedCheckEvent.
        /// </summary>
        private TimegatedEvent DistanceMovedCheckEvent { get; }

        /// <summary>
        /// Gets the private TimegatedEvent property named FindPathEvent.
        /// </summary>
        private TimegatedEvent FindPathEvent { get; }

        /// <summary>
        /// Gets or sets the last known target position in 3D space.
        /// </summary>
        private Vector3 LastTargetPosition { get; set; }

        /// <summary>
        /// Gets or sets the date and time until which movement is blocked.
        /// </summary>
        private DateTime MovementBlockedUntil { get; set; }

        /// <summary>
        /// Gets or sets the queue of Vector3 objects representing a path.
        /// </summary>
        private Queue<Vector3> PathQueue { get; set; }

        /// <summary>
        /// Gets or sets the list of places to avoid, each containing information about its position, radius, and until when it should be avoided.
        /// </summary>
        private List<(Vector3 position, float radius, DateTime until)> PlacesToAvoidList { get; set; }

        /// <summary>
        /// Gets or sets the player's basic vehicle.
        /// </summary>
        private BasicVehicle PlayerVehicle { get; set; }

        /// <summary>
        /// Gets or sets the PreventMovementType property.
        /// </summary>
        private PreventMovementType PreventMovementType { get; set; }

        /// <summary>
        /// Gets the TimegatedEvent for refreshing the path.
        /// </summary>
        private TimegatedEvent RefreshPathEvent { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has tried to mount up.
        /// </summary>
        private bool TriedToMountUp { get; set; }

        /// <summary>
        /// Adds a new place to the avoid list and removes places that have expired.
        /// </summary>
        /// <param name="position">The position of the place to avoid.</param>
        /// <param name="radius">The radius around the position to avoid.</param>
        /// <param name="timeSpan">The duration for which the place should be avoided.</param>
        public void AvoidPlace(Vector3 position, float radius, TimeSpan timeSpan)
        {
            DateTime now = DateTime.UtcNow;

            PlacesToAvoidList.Add((position, radius, now + timeSpan));
            PlacesToAvoidList.RemoveAll(e => now > e.until);
        }

        /// <summary>
        /// Moves the bot's character to the specified position using the MoveToPosition method with a speed of 20.9f and a tolerance of 0.5f.
        /// </summary>
        public void DirectMove(Vector3 position)
        {
            Bot.Character.MoveToPosition(position, 20.9f, 0.5f);
            // PlayerVehicle.Update((x) => Bot.Character.MoveToPosition(x, 20.9f, 0.5f),
            // MovementAction.Follow, position);
        }

        /// <summary>
        /// Executes the movement logic for the bot.
        /// If the bot is not allowed to move and the prevent movement check is valid, stop the movement.
        /// If the bot is unstucking and the distance to the unstuck target is less than 2.0, set the "IsUnstucking" flag to false.
        /// If there are nodes in the path queue, perform the necessary movement actions to reach the next node.
        /// If the bot is at a node, remove it from the path queue.
        /// If there are no nodes in the path queue, check for any AOEs to avoid and set the movement action accordingly.
        /// </summary>
        public void Execute()
        {
            if (!IsAllowedToMove && IsPreventMovementValid())
            {
                Bot.Movement.StopMovement();
                return;
            }

            if (IsUnstucking && UnstuckTarget.GetDistance(Bot.Player.Position) < 2.0f)
            {
                IsUnstucking = false;
            }

            if (PathQueue.Count > 0)
            {
                Vector3 currentNode = IsUnstucking ? UnstuckTarget : PathQueue.Peek();
                float distanceToNode = Bot.Player.Position.GetDistance2D(currentNode);

                if (distanceToNode > 1.0f)
                {
                    if (!TriedToMountUp)
                    {
                        float distance = Bot.Player.Position.GetDistance(PathQueue.Last());

                        // try to mount only once per path
                        if (distance > 40.0f
                            && !Bot.Player.IsInCombat
                            && !Bot.Player.IsGhost
                            && !Bot.Player.IsMounted
                            && Bot.Player.IsOutdoors
                            && Bot.Character.Mounts != null
                            && Bot.Character.Mounts.Any()
                            // wsg flags
                            && !Bot.Player.HasBuffById(Bot.Player.IsAlliance() ? 23333 : 23335))
                        {
                            MountUp();
                            TriedToMountUp = true;
                        }
                    }

                    // we need to move to the node
                    if (!Bot.Player.IsCasting)
                    {
                        PlayerVehicle.Update
                        (
                            MoveCharacter,
                            Status,
                            currentNode,
                            Bot.Player.Rotation,
                            Bot.Player.IsInCombat ? Config.MovementSettings.MaxSteeringCombat : Config.MovementSettings.MaxSteering,
                            Config.MovementSettings.MaxVelocity,
                            Config.MovementSettings.SeperationDistance
                        );
                    }
                }
                else
                {
                    // we are at the node
                    PathQueue.Dequeue();
                }
            }
            else
            {
                if (AvoidAoeStuff(Bot.Player.Position, out Vector3 newPosition))
                {
                    SetMovementAction(MovementAction.Move, newPosition);
                }
            }
        }

        /// <summary>
        /// Prevents movement for a specified duration of time.
        /// </summary>
        /// <param name="timeSpan">The duration of time to prevent movement for.</param>
        /// <param name="preventMovementType">The type of movement prevention to apply (default is Hard).</param>
        public void PreventMovement(TimeSpan timeSpan, PreventMovementType preventMovementType = PreventMovementType.Hard)
        {
            PreventMovementType = preventMovementType;
            StopMovement();
            MovementBlockedUntil = DateTime.UtcNow + timeSpan;
        }

        /// <summary>
        /// Resets the state of the object, clearing the path queue, setting the movement status to None,
        /// and resetting the action of trying to mount up.
        /// </summary>
        public void Reset()
        {
            PathQueue.Clear();
            Status = MovementAction.None;
            TriedToMountUp = false;
        }

        /// <summary>
        /// Sets the movement action for the bot's character.
        /// </summary>
        /// <param name="state">The movement action state to set.</param>
        /// <param name="position">The target position to move towards.</param>
        /// <param name="rotation">The rotation value (optional, defaults to 0.0f).</param>
        /// <returns>True if the movement action was set, false otherwise.</returns>
        public bool SetMovementAction(MovementAction state, Vector3 position, float rotation = 0.0f)
        {
            if (IsAllowedToMove && (PathQueue.Count == 0 || RefreshPathEvent.Ready))
            {
                if (state == MovementAction.DirectMove || Bot.Player.IsFlying || Bot.Player.IsUnderwater)
                {
                    Bot.Character.MoveToPosition(IsUnstucking ? UnstuckTarget : position);
                    Status = state;
                    DistanceMovedJumpCheck();
                }
                else if (FindPathEvent.Run() && TryGetPath(position, out IEnumerable<Vector3> path))
                {
                    // if its a new path, we can try to mount again
                    if (path.Last().GetDistance(LastTargetPosition) > 10.0f)
                    {
                        TriedToMountUp = false;
                    }

                    PathQueue.Clear();

                    foreach (Vector3 node in path)
                    {
                        PathQueue.Enqueue(node);
                    }

                    RefreshPathEvent.Run();
                    Status = state;
                    LastTargetPosition = path.Last();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Stops the movement of the bot by calling the Reset method and the StopClickToMove method in the Bot.Wow class.
        /// </summary>
        public void StopMovement()
        {
            Reset();
            Bot.Wow.StopClickToMove();
        }

        ///<summary>
        /// Tries to get a path from the current position to a target position.
        ///</summary>
        ///<param name="position">The target position to reach.</param>
        ///<param name="path">The resulting path as an IEnumerable of Vector3 points.</param>
        ///<param name="maxDistance">The maximum distance allowed to consider the path as valid (defaults to 5.0f).</param>
        ///<returns>
        /// Returns true if a valid path is found, false otherwise.
        ///</returns>
        public bool TryGetPath(Vector3 position, out IEnumerable<Vector3> path, float maxDistance = 5.0f)
        {
            // dont search a path into aoe effects
            if (AvoidAoeStuff(position, out Vector3 newPosition))
            {
                position = newPosition;
            }

            path = Bot.PathfindingHandler.GetPath((int)Bot.Objects.MapId, Bot.Player.Position, position);

            if (path != null && path.Any())
            {
                Vector3 lastNode = path.LastOrDefault();

                if (lastNode == default)
                {
                    return false;
                }

                // TODO: handle incomplete paths, disabled for now double distance =
                // lastNode.GetDistance(position); return distance < maxDistance;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Avoids area of effect (AOE) spells by moving the position away from them.
        /// </summary>
        /// <param name="position">The current position.</param>
        /// <param name="newPosition">The new position to move to, away from AOE spells.</param>
        /// <returns>True if AOE spells were detected and avoided, false otherwise.</returns>
        private bool AvoidAoeStuff(Vector3 position, out Vector3 newPosition)
        {
            List<(Vector3 position, float radius)> places = new(PlacesToAvoid);

            // TODO: avoid dodging player aoe spells in sactuaries, this may looks suspect
            if (Config.AoeDetectionAvoid)
            {
                // add all aoe spells
                IEnumerable<IWowDynobject> aoeEffects = Bot.GetAoeSpells(position, Config.AoeDetectionExtends)
                    .Where(e => (Config.AoeDetectionIncludePlayers || Bot.GetWowObjectByGuid<IWowUnit>(e.Caster)?.Type == WowObjectType.Unit)
                             && Bot.Db.GetReaction(Bot.Player, Bot.GetWowObjectByGuid<IWowUnit>(e.Caster)) is WowUnitReaction.Hostile or WowUnitReaction.Neutral);

                places.AddRange(aoeEffects.Select(e => (e.Position, e.Radius)));
            }

            if (places.Any())
            {
                // build mean position and move away x meters from it x is the biggest distance
                // we have to move
                Vector3 meanAoePos = BotMath.GetMeanPosition(places.Select(e => e.position));
                float distanceToMove = places.Max(e => e.radius) + Config.AoeDetectionExtends;

                // claculate the repell direction to move away from the aoe effects
                Vector3 repellDirection = position - meanAoePos;
                repellDirection.Normalize();

                // "repell" the position from the aoe spell
                newPosition = meanAoePos + (repellDirection * distanceToMove);
                return true;
            }

            newPosition = default;
            return false;
        }

        /// <summary>
        /// Checks if the distance moved by the bot meets the criteria for a jump.
        /// If the bot is stuck in a soft spot, it performs a jump to free itself.
        /// If the bot is unstucking, it checks if it has reached the target distance or if the target is not set, then stops unstucking.
        /// If the bot is not unstucking and its current speed is zero, it initiates the unstucking process by setting a random target location and initiating movement towards it.
        /// </summary>
        private void DistanceMovedJumpCheck()
        {
            if (DistanceMovedCheckEvent.Ready)
            {
                if (LastMovement != default && DateTime.UtcNow - LastMovement < TimeSpan.FromSeconds(1))
                {
                    CurrentSpeed = LastPosition.GetDistance2D(Bot.Player.Position) / (float)(DateTime.UtcNow - LastMovement).TotalSeconds;

                    if (CurrentSpeed > 0.0f && CurrentSpeed < 0.1f)
                    {
                        // soft stuck
                        Bot.Character.Jump();
                    }

                    if (IsUnstucking)
                    {
                        if ((CurrentSpeed > 1.0f && UnstuckTarget.GetDistance(Bot.Player.Position) <= Config.MovementSettings.WaypointCheckThreshold) || UnstuckTarget == Vector3.Zero)
                        {
                            IsUnstucking = false;
                            UnstuckTarget = Vector3.Zero;
                        }
                    }
                    else
                    {
                        if (CurrentSpeed == 0.0f)
                        {
                            IsUnstucking = true;
                            UnstuckTarget = Bot.PathfindingHandler.GetRandomPointAround((int)Bot.Objects.MapId, Bot.Player.Position, 6.0f);
                            SetMovementAction(MovementAction.Move, UnstuckTarget);
                        }
                    }
                }

                LastMovement = DateTime.UtcNow;
                LastPosition = Bot.Player.Position;
                DistanceMovedCheckEvent.Run();
            }
        }

        /// <summary>
        /// Determines if the prevent movement condition is valid.
        /// </summary>
        /// <returns>Returns true if the prevent movement condition is valid, false otherwise.</returns>
        private bool IsPreventMovementValid()
        {
            switch (PreventMovementType)
            {
                case PreventMovementType.SpellCast:
                    // cast maybe aborted, allow to move again
                    return Bot.Player.IsCasting;

                default:
                    break;
            }

            return false;
        }

        /// <summary>
        /// Mounts up the character based on the filtered mounts.
        /// If UseOnlySpecificMounts is true, filters the mounts based on the names provided in the Mounts configuration.
        /// Selects a random mount from the filtered mounts and mounts it.
        /// </summary>
        private void MountUp()
        {
            IEnumerable<WowMount> filteredMounts = Bot.Character.Mounts;

            if (Config.UseOnlySpecificMounts)
            {
                filteredMounts = filteredMounts.Where(e => Config.Mounts.Split(",", StringSplitOptions.RemoveEmptyEntries).Any(x => x.Equals(e.Name.Trim(), StringComparison.OrdinalIgnoreCase)));
            }

            if (filteredMounts != null && filteredMounts.Any())
            {
                WowMount mount = filteredMounts.ElementAt(new Random().Next(0, filteredMounts.Count()));
                PreventMovement(TimeSpan.FromSeconds(2));
                Bot.Wow.CallCompanion(mount.Index, "MOUNT");
            }
        }

        ///<summary>
        ///Moves the character to the specified position.
        ///</summary>
        private void MoveCharacter(Vector3 positionToGoTo)
        {
            Vector3 node = Bot.PathfindingHandler.MoveAlongSurface((int)Bot.Objects.MapId, Bot.Player.Position, positionToGoTo);

            if (node != Vector3.Zero)
            {
                Bot.Character.MoveToPosition(node, MathF.Tau, 0.25f);

                if (Config.MovementSettings.EnableDistanceMovedJumpCheck)
                {
                    DistanceMovedJumpCheck();
                }
            }
        }
    }
}