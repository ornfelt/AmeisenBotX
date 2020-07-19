﻿using AmeisenBotX.BehaviorTree;
using AmeisenBotX.BehaviorTree.Enums;
using AmeisenBotX.BehaviorTree.Objects;
using AmeisenBotX.Core.Character.Objects;
using AmeisenBotX.Core.Common;
using AmeisenBotX.Core.Movement.Enums;
using AmeisenBotX.Core.Movement.Objects;
using AmeisenBotX.Core.Movement.Pathfinding.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace AmeisenBotX.Core.Movement.SMovementEngine
{
    public class SickMovementEngine : IMovementEngine
    {
        public SickMovementEngine(WowInterface wowInterface, AmeisenBotConfig config)
        {
            WowInterface = wowInterface;
            Nodes = new Queue<Vector3>();
            PlayerVehicle = new BasicVehicle(wowInterface);

            MovementWatchdog = new Timer(1000);
            MovementWatchdog.Elapsed += MovementWatchdog_Elapsed;

            if (WowInterface.MovementSettings.EnableDistanceMovedJumpCheck)
            {
                MovementWatchdog.Start();
            }

            PathRefreshEvent = new TimegatedEvent(TimeSpan.FromMilliseconds(500));
            JumpCheckEvent = new TimegatedEvent(TimeSpan.FromSeconds(1));
            MountCheck = new TimegatedEvent(TimeSpan.FromSeconds(3));
            PathDecayEvent = new TimegatedEvent(TimeSpan.FromSeconds(4));

            Blackboard = new MovementBlackboard(UpdateBlackboard);
            BehaviorTree = new AmeisenBotBehaviorTree<MovementBlackboard>
            (
                "MovementTree",
                new Selector<MovementBlackboard>
                (
                    "DoINeedToMove",
                    (b) => WowInterface.ObjectManager.Player.Position.GetDistance(TargetPosition) > MinDistanceToMove,
                    new Selector<MovementBlackboard>
                    (
                        "NeedToUnstuck",
                        (b) => ShouldBeMoving && StuckCounter > WowInterface.MovementSettings.StuckCounterUnstuck,
                        new Leaf<MovementBlackboard>
                        (
                            (b) => DoUnstuck()
                        ),
                        new Selector<MovementBlackboard>
                        (
                            "NeedToJump",
                            (b) => JumpOnNextMove,
                            new Leaf<MovementBlackboard>((b) =>
                            {
                                WowInterface.CharacterManager.Jump();
                                JumpOnNextMove = false;
                                return BehaviorTreeStatus.Success;
                            }),
                            new Selector<MovementBlackboard>
                            (
                                "IsDirectMovingState",
                                (b) => IsDirectMovingState(),
                                new Leaf<MovementBlackboard>((b) =>
                                {
                                    if (Nodes.Count > 0)
                                    {
                                        Nodes.Clear();
                                    }

                                    ShouldBeMoving = true;

                                    //Vector3 tPos = WowInterface.PathfindingHandler.MoveAlongSurface((int)WowInterface.ObjectManager.MapId, WowInterface.ObjectManager.Player.Position, TargetPosition);
                                    PlayerVehicle.Update((p) => WowInterface.CharacterManager.MoveToPosition(p), MovementAction, TargetPosition, TargetRotation);

                                    return WowInterface.ObjectManager.Player.Position.GetDistance(TargetPosition) < WowInterface.MovementSettings.WaypointCheckThreshold
                                           ? BehaviorTreeStatus.Success : BehaviorTreeStatus.Ongoing;
                                }),
                                new Selector<MovementBlackboard>
                                (
                                    "DoINeedToFindAPath",
                                    (b) => DoINeedToFindAPath() && (Nodes == null || Nodes.Count == 0 || PathRefreshEvent.Run()),
                                    new Leaf<MovementBlackboard>
                                    (
                                        "FindPathToTargetPosition",
                                        FindPathToTargetPosition
                                    ),
                                    new Selector<MovementBlackboard>
                                    (
                                        "NeedToCheckANode",
                                        (b) => Nodes.Peek().GetDistance2D(WowInterface.ObjectManager.Player.Position) < WowInterface.MovementSettings.WaypointCheckThreshold,
                                        new Leaf<MovementBlackboard>("CheckWaypoint", (b) =>
                                        {
                                            Nodes.Dequeue();

                                            if (Nodes.Count == 0)
                                            {
                                                MovementAction = MovementAction.None;
                                            }

                                            return BehaviorTreeStatus.Success;
                                        }),
                                        new Leaf<MovementBlackboard>("Move", (b) =>
                                        {
                                            if (config.UseMounts)
                                            {
                                                if (!WowInterface.ObjectManager.Player.HasBuffByName("Warsong Flag")
                                                    && !WowInterface.ObjectManager.Player.HasBuffByName("Silverwing Flag")
                                                    && MountCheck.Run()
                                                    && wowInterface.CharacterManager.Mounts.Count > 0
                                                    && TargetPosition.GetDistance2D(WowInterface.ObjectManager.Player.Position) > 20.0
                                                    && !WowInterface.ObjectManager.Player.IsMounted
                                                    && wowInterface.HookManager.IsOutdoors())
                                                {
                                                    List<WowMount> filteredMounts;

                                                    if (config.UseOnlySpecificMounts)
                                                    {
                                                        filteredMounts = WowInterface.CharacterManager.Mounts.Where(e => config.Mounts.Split(",", StringSplitOptions.RemoveEmptyEntries).Contains(e.Name)).ToList();
                                                    }
                                                    else
                                                    {
                                                        filteredMounts = WowInterface.CharacterManager.Mounts;
                                                    }

                                                    if (filteredMounts != null && filteredMounts.Count >= 0)
                                                    {
                                                        WowMount mount = filteredMounts[new Random().Next(0, filteredMounts.Count)];
                                                        WowInterface.MovementEngine.StopMovement();
                                                        WowInterface.HookManager.Mount(mount.Index);
                                                    }

                                                    IsCastingMount = true;
                                                    return BehaviorTreeStatus.Ongoing;
                                                }

                                                if (IsCastingMount)
                                                {
                                                    if (wowInterface.ObjectManager.Player.IsCasting)
                                                    {
                                                        return BehaviorTreeStatus.Ongoing;
                                                    }
                                                    else
                                                    {
                                                        IsCastingMount = false;
                                                    }
                                                }
                                            }

                                            ShouldBeMoving = true;
                                            PlayerVehicle.Update((p) => WowInterface.CharacterManager.MoveToPosition(p), MovementAction, Nodes.Peek(), TargetRotation);
                                            return BehaviorTreeStatus.Ongoing;
                                        })
                                    )
                                )
                            )
                        )
                    ),
                    new Leaf<MovementBlackboard>((b) => { return BehaviorTreeStatus.Success; })
                ),
                Blackboard
            );
        }

        public AmeisenBotBehaviorTree<MovementBlackboard> BehaviorTree { get; }

        public MovementBlackboard Blackboard { get; }

        public bool IsAtTargetPosition => TargetPosition != default && TargetPosition.GetDistance(WowInterface.ObjectManager.Player.Position) < WowInterface.MovementSettings.WaypointCheckThreshold;

        public bool IsCastingMount { get; set; }

        public bool JumpOnNextMove { get; private set; }

        public Vector3 LastPlayerPosition { get; private set; }

        public double MinDistanceToMove { get; private set; }

        public TimegatedEvent MountCheck { get; }

        public double MovedDistance { get; private set; }

        public MovementAction MovementAction { get; private set; }

        public Queue<Vector3> Nodes { get; private set; }

        public List<Vector3> Path => Nodes.ToList();

        public TimegatedEvent PathDecayEvent { get; private set; }

        public TimegatedEvent PathRefreshEvent { get; }

        public BasicVehicle PlayerVehicle { get; }

        public bool ShouldBeMoving { get; private set; }

        public int StuckCounter { get; private set; }

        public Vector3 StuckPosition { get; private set; }

        public float StuckRotation { get; private set; }

        public Vector3 TargetPosition { get; private set; }

        public Vector3 TargetPositionLastPathfinding { get; private set; }

        public float TargetRotation { get; private set; }

        public Vector3 UnstuckTargetPosition { get; private set; }

        private TimegatedEvent JumpCheckEvent { get; }

        private Timer MovementWatchdog { get; }

        private WowInterface WowInterface { get; }

        public void Execute()
        {
            if (MovementAction != MovementAction.None)
            {
                // check for obstacles in our way
                if (WowInterface.MovementSettings.EnableTracelineJumpCheck
                    && !JumpOnNextMove
                    && JumpCheckEvent.Run())
                {
                    Vector3 pos = BotUtils.MoveAhead(WowInterface.ObjectManager.Player.Position, WowInterface.ObjectManager.Player.Rotation, WowInterface.MovementSettings.JumpCheckDistance);

                    if (!WowInterface.HookManager.IsInLineOfSight
                    (
                        WowInterface.ObjectManager.Player.Position,
                        pos,
                        WowInterface.MovementSettings.JumpCheckHeight
                    ))
                    {
                        JumpOnNextMove = true;
                    }
                }

                BehaviorTree.Tick();
            }
        }

        public void Reset()
        {
            MovementAction = MovementAction.None;
            TargetPosition = Vector3.Zero;
            TargetRotation = 0f;

            StuckCounter = 0;
            StuckPosition = default;
            ShouldBeMoving = false;

            if (Nodes.Count > 0)
            {
                Nodes.Clear();
            }
        }

        public void SetMovementAction(MovementAction movementAction, Vector3 positionToGoTo, float targetRotation = 0f, double minDistanceToMove = 1.5)
        {
            MovementAction = movementAction;
            TargetPosition = positionToGoTo;
            TargetRotation = targetRotation;
            MinDistanceToMove = minDistanceToMove;
        }

        public void StopMovement()
        {
            WowInterface.HookManager.StopClickToMoveIfActive();
            Reset();
        }

        private bool DoINeedToFindAPath()
        {
            return Path == null
                || Path.Count == 0
                || PathDecayEvent.Run()
                || TargetPositionLastPathfinding.GetDistance(TargetPosition) > 1.0;
        }

        private BehaviorTreeStatus DoUnstuck()
        {
            if (StuckPosition == default)
            {
                StuckPosition = WowInterface.ObjectManager.Player.Position;
                StuckRotation = WowInterface.ObjectManager.Player.Rotation;

                // position behind us + random rotation 0 to PI - half PI (will result in -1.5 to 1.5)
                double angle = Math.PI + ((new Random().NextDouble() * Math.PI) - (Math.PI / 2.0));
                UnstuckTargetPosition = BotMath.CalculatePositionAround(WowInterface.ObjectManager.Player.Position, StuckRotation, (float)angle, WowInterface.MovementSettings.UnstuckDistance);
            }
            else
            {
                if (StuckPosition.GetDistance(WowInterface.ObjectManager.Player.Position) > 0.0
                    && StuckPosition.GetDistance(WowInterface.ObjectManager.Player.Position) < WowInterface.MovementSettings.MinUnstuckDistance)
                {
                    PlayerVehicle.Update((p) => WowInterface.CharacterManager.MoveToPosition(p), MovementAction.Moving, UnstuckTargetPosition);
                    WowInterface.CharacterManager.Jump();
                }
                else
                {
                    Reset();
                    return BehaviorTreeStatus.Success;
                }
            }

            return BehaviorTreeStatus.Ongoing;
        }

        private BehaviorTreeStatus FindPathToTargetPosition(MovementBlackboard blackboard)
        {
            List<Vector3> path = WowInterface.PathfindingHandler.GetPath((int)WowInterface.ObjectManager.MapId, WowInterface.ObjectManager.Player.Position, TargetPosition);

            if (path != null && path.Count > 0)
            {
                Queue<Vector3> newPath = new Queue<Vector3>();

                for (int i = 0; i < path.Count; ++i)
                {
                    newPath.Enqueue(path[i]);
                }

                Nodes = newPath;

                TargetPositionLastPathfinding = TargetPosition;
                return BehaviorTreeStatus.Success;
            }
            else
            {
                return BehaviorTreeStatus.Failed;
            }
        }

        private bool IsDirectMovingState()
        {
            return MovementAction != MovementAction.Moving
                && MovementAction != MovementAction.Following;
        }

        private void MovementWatchdog_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (MovementAction == MovementAction.None)
            {
                ShouldBeMoving = false;
                return;
            }

            // check wether we should be moving or not
            if (ShouldBeMoving)
            {
                // if we already need to jump, dont check it again
                if (!JumpOnNextMove)
                {
                    MovedDistance = LastPlayerPosition.GetDistance(WowInterface.ObjectManager.Player.Position);
                    LastPlayerPosition = WowInterface.ObjectManager.Player.Position;

                    if (MovedDistance > WowInterface.MovementSettings.MinDistanceMovedJumpUnstuck
                        && MovedDistance < WowInterface.MovementSettings.MaxDistanceMovedJumpUnstuck)
                    {
                        ++StuckCounter;
                        JumpOnNextMove = true;
                    }
                    else
                    {
                        StuckCounter = 0;
                    }
                }
            }
            else
            {
                StuckCounter = 0;
            }
        }

        private void UpdateBlackboard()
        {
        }
    }
}