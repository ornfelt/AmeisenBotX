using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Core.Engines.Movement.Objects;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Movement;

/// <summary>
/// Multi-stage unstuck recovery escalation
/// </summary>
public enum UnstuckStage
{
    None,
    Jump,      // Try jumping over obstacle
    Strafe,    // Strafe left/right
    Reverse,   // Move backward
    PathReset  // Clear path and recalculate
}

public class MovementEngine(AmeisenBotInterfaces bot, AmeisenBotConfig config) : IMovementEngine
{
    private const string MountCommand = "MOUNT";
    private const float RunSpeed = 20.9f;

    public float CurrentSpeed { get; private set; }

    public bool IsAllowedToMove => DateTime.UtcNow > MovementBlockedUntil;

    public bool IsMoving { get; private set; }

    public bool IsUnstucking { get; private set; }

    public DateTime LastMovement { get; private set; }

    public Vector3 LastPosition { get; private set; }

    public IEnumerable<Vector3> Path => PathQueue;

    public IEnumerable<(Vector3 position, float radius)> PlacesToAvoid => PlacesToAvoidList.Where(e => DateTime.UtcNow <= e.until).Select(e => (e.position, e.radius));

    public MovementAction Status { get; private set; }

    public Vector3 UnstuckTarget { get; private set; }

    private AmeisenBotInterfaces Bot { get; } = bot;

    private AmeisenBotConfig Config { get; } = config;

    private TimegatedEvent DistanceMovedCheckEvent { get; } = new(TimeSpan.FromMilliseconds(500));

    private TimegatedEvent FindPathEvent { get; } = new(TimeSpan.FromMilliseconds(500));

    private Vector3 LastTargetPosition { get; set; }

    private DateTime MovementBlockedUntil { get; set; }

    private Queue<Vector3> PathQueue { get; set; } = new();

    private List<(Vector3 position, float radius, DateTime until)> PlacesToAvoidList { get; set; } = [];

    private BasicVehicle PlayerVehicle { get; set; } = new(bot);

    private PreventMovementType PreventMovementType { get; set; }

    private TimegatedEvent RefreshPathEvent { get; } = new(TimeSpan.FromMilliseconds(500));

    private bool TriedToMountUp { get; set; }

    // CTM Throttling
    private DateTime _lastCtmUpdate = DateTime.MinValue;
    private Vector3 _lastCtmTarget = Vector3.Zero;
    private Vector3 _smoothedTarget = Vector3.Zero;

    // Enhanced Stuck Detection
    private UnstuckStage _currentUnstuckStage = UnstuckStage.None;
    private DateTime _unstuckStageStartTime = DateTime.MinValue;
    private int _stuckCounter = 0;
    private float _lastRotation = 0f;
    private const int StuckThreshold = 3; // Consecutive stuck checks before escalation
    private const float StrafeDistance = 4.0f;
    private const float ReverseDistance = 5.0f;

    // Random Humanization
    private DateTime _nextRandomJump = DateTime.UtcNow;
    public void AvoidPlace(Vector3 position, float radius, TimeSpan timeSpan)
    {
        DateTime now = DateTime.UtcNow;

        PlacesToAvoidList.Add((position, radius, now + timeSpan));
        PlacesToAvoidList.RemoveAll(e => now > e.until);
    }

    public void DirectMove(Vector3 position)
    {
        // Use MoveAlongSurface even for direct moves to stay on navmesh
        Vector3 safePosition = Bot.PathfindingHandler.MoveAlongSurface((int)Bot.Objects.MapId, Bot.Player.Position, position);
        Bot.Character.MoveToPosition(safePosition != Vector3.Zero ? safePosition : position, RunSpeed, 0.5f);
    }

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
            float distanceToNode = Bot.Player.Position.GetDistance(currentNode);
            double threshold = Bot.Player.IsMounted ? Config.MovementSettings.WaypointCheckThresholdMounted : Config.MovementSettings.WaypointCheckThreshold;

            // LOOKAHEAD SMOOTHING: When close to current node, blend towards next node
            // This prevents instant 90° turns at corners
            if (PathQueue.Count > 1 && distanceToNode < 4.0f && !IsUnstucking)
            {
                Vector3 nextNode = PathQueue.ElementAtOrDefault(1);
                if (nextNode != default)
                {
                    // Blend factor: 0 = current node, 1 = next node
                    float blendFactor = 1.0f - (distanceToNode / 4.0f);
                    blendFactor = MathF.Max(0f, MathF.Min(1f, blendFactor)) * 0.5f; // Max 50% blend

                    currentNode = new Vector3(
                        currentNode.X + ((nextNode.X - currentNode.X) * blendFactor),
                        currentNode.Y + ((nextNode.Y - currentNode.Y) * blendFactor),
                        currentNode.Z + ((nextNode.Z - currentNode.Z) * blendFactor)
                    );
                }
            }

            if (distanceToNode > threshold)
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

                // Only disable separation if this is the KEY destination (last point in path), 
                // otherwise keep separation active to avoid clumps during travel.
                float sepDisableParams = (PathQueue.Count <= 1) ? Config.MovementSettings.SeparationDisableDistance : 0.0f;

                // we need to move to the node
                if (!Bot.Player.IsCasting)
                {
                    PlayerVehicle.Update
                    (
                        MoveCharacter,
                        Bot.Character.Jump,
                        Status,
                        currentNode,
                        Bot.Player.Rotation,
                        Bot.Player.IsInCombat ? Config.MovementSettings.MaxSteeringCombat : Config.MovementSettings.MaxSteering,
                        Config.MovementSettings.MaxVelocity,
                        Config.MovementSettings.SeperationDistance,
                        Config.MovementSettings.ArrivalThreshold3D,
                        Config.MovementSettings.ArrivalThreshold2D,
                        Config.MovementSettings.HeightToleranceForArrival,
                        Config.MovementSettings.ArriveSlowdownRadius,
                        sepDisableParams, // dynamically set dependent on if it is final dest
                        Config.MovementSettings.VelocityDamping
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

    public void PreventMovement(TimeSpan timeSpan, PreventMovementType preventMovementType = PreventMovementType.Hard)
    {
        PreventMovementType = preventMovementType;
        StopMovement();
        MovementBlockedUntil = DateTime.UtcNow + timeSpan;
    }

    public void Reset()
    {
        PathQueue.Clear();
        Status = MovementAction.None;
        TriedToMountUp = false;
        _smoothedTarget = Vector3.Zero;
        _lastCtmTarget = Vector3.Zero;
    }

    public bool SetMovementAction(MovementAction state, Vector3 position, float rotation = 0.0f)
    {
        if (IsAllowedToMove && (PathQueue.Count == 0 || RefreshPathEvent.Ready))
        {
            if (state == MovementAction.DirectMove || Bot.Player.IsFlying || Bot.Player.IsUnderwater)
            {
                Vector3 targetPos = IsUnstucking ? UnstuckTarget : position;
                // Still use MoveAlongSurface for safety, fallback to direct if it fails
                Vector3 safePos = Bot.PathfindingHandler.MoveAlongSurface((int)Bot.Objects.MapId, Bot.Player.Position, targetPos);
                Bot.Character.MoveToPosition(safePos != Vector3.Zero ? safePos : targetPos);
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

    public void StopMovement()
    {
        Reset();
        _smoothedTarget = Vector3.Zero;
        Bot.Wow.StopClickToMove();
    }

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

    private bool AvoidAoeStuff(Vector3 position, out Vector3 newPosition)
    {
        List<(Vector3 position, float radius)> places = [.. PlacesToAvoid];

        // TODO: avoid dodging player aoe spells in sactuaries, this may looks suspect
        if (Config.AoeDetectionAvoid)
        {
            // add all aoe spells
            IEnumerable<IWowDynobject> aoeEffects = Bot.GetAoeSpells(position, Config.AoeDetectionExtends)
                .Where(e => (Config.AoeDetectionIncludePlayers || Bot.GetWowObjectByGuid<IWowUnit>(e.Caster)?.Type == WowObjectType.Unit)
                         && Bot.Db.GetReaction(Bot.Player, Bot.GetWowObjectByGuid<IWowUnit>(e.Caster)) is WowUnitReaction.Hostile or WowUnitReaction.Neutral);

            places.AddRange(aoeEffects.Select(e => (e.Position, e.Radius)));
        }

        if (places.Count != 0)
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

    private void DistanceMovedJumpCheck()
    {
        if (!DistanceMovedCheckEvent.Ready)
        {
            return;
        }

        DateTime now = DateTime.UtcNow;

        if (LastMovement != default && now - LastMovement < TimeSpan.FromSeconds(1))
        {
            CurrentSpeed = LastPosition.GetDistance2D(Bot.Player.Position) / (float)(now - LastMovement).TotalSeconds;
            float rotationDelta = MathF.Abs(Bot.Player.Rotation - _lastRotation);

            // Random humanization jump (1% chance per check, ~ every 50 seconds of movement)
            if (CurrentSpeed > 2.0f && now > _nextRandomJump && !Bot.Player.IsInCombat)
            {
                Bot.Character.Jump();
                _nextRandomJump = now.AddSeconds(30 + Random.Shared.Next(0, 60));
            }

            // Check if we're stuck
            bool isStuck = CurrentSpeed < 0.1f && PathQueue.Count > 0;
            bool isSpinning = rotationDelta > 0.5f && CurrentSpeed < 0.5f; // Spinning but not moving

            if (isStuck || isSpinning)
            {
                _stuckCounter++;

                if (_stuckCounter >= StuckThreshold)
                {
                    HandleMultiStageUnstuck();
                }
                else if (CurrentSpeed is > 0.0f and < 0.1f)
                {
                    // Soft stuck - just jump
                    Bot.Character.Jump();
                }
            }
            else
            {
                // Moving fine - reset stuck state
                _stuckCounter = 0;
                if (_currentUnstuckStage != UnstuckStage.None && CurrentSpeed > 1.0f)
                {
                    _currentUnstuckStage = UnstuckStage.None;
                    IsUnstucking = false;
                }
            }

            // Check if unstuck target reached
            if (IsUnstucking && UnstuckTarget != Vector3.Zero)
            {
                if (UnstuckTarget.GetDistance(Bot.Player.Position) <= 2.0f || CurrentSpeed > 2.0f)
                {
                    IsUnstucking = false;
                    UnstuckTarget = Vector3.Zero;
                    _currentUnstuckStage = UnstuckStage.None;
                    _stuckCounter = 0;
                }
            }
        }

        _lastRotation = Bot.Player.Rotation;
        LastMovement = now;
        LastPosition = Bot.Player.Position;
        DistanceMovedCheckEvent.Run();
    }

    /// <summary>
    /// Multi-stage unstuck recovery: Jump ? Strafe ? Reverse ? PathReset
    /// </summary>
    private void HandleMultiStageUnstuck()
    {
        DateTime now = DateTime.UtcNow;
        TimeSpan stageDuration = now - _unstuckStageStartTime;

        // Escalate stage if current stage hasn't worked
        if (_currentUnstuckStage == UnstuckStage.None || stageDuration > TimeSpan.FromSeconds(3))
        {
            _currentUnstuckStage = _currentUnstuckStage switch
            {
                UnstuckStage.None => UnstuckStage.Jump,
                UnstuckStage.Jump => UnstuckStage.Strafe,
                UnstuckStage.Strafe => UnstuckStage.Reverse,
                UnstuckStage.Reverse => UnstuckStage.PathReset,
                _ => UnstuckStage.Jump
            };
            _unstuckStageStartTime = now;
        }

        IsUnstucking = true;
        Vector3 playerPos = Bot.Player.Position;
        Vector3 forward = Bot.Player.RotationVector;

        switch (_currentUnstuckStage)
        {
            case UnstuckStage.Jump:
                // Jump and try to continue forward
                Bot.Character.Jump();
                UnstuckTarget = playerPos + (forward * 4.0f);
                break;

            case UnstuckStage.Strafe:
                // Strafe left or right randomly
                Vector3 right = new(forward.Y, -forward.X, 0);
                float strafeDir = Random.Shared.Next(2) == 0 ? 1.0f : -1.0f;
                UnstuckTarget = playerPos + (right * strafeDir * StrafeDistance);
                // Validate with navmesh
                Vector3 validated = Bot.PathfindingHandler.MoveAlongSurface(
                    (int)Bot.Objects.MapId, playerPos, UnstuckTarget);
                if (validated != Vector3.Zero)
                {
                    UnstuckTarget = validated;
                }

                break;

            case UnstuckStage.Reverse:
                // Move backward
                UnstuckTarget = playerPos - (forward * ReverseDistance);
                validated = Bot.PathfindingHandler.MoveAlongSurface(
                    (int)Bot.Objects.MapId, playerPos, UnstuckTarget);
                if (validated != Vector3.Zero)
                {
                    UnstuckTarget = validated;
                }

                break;

            case UnstuckStage.PathReset:
                // Clear path and try to find a new random nearby point
                PathQueue.Clear();
                UnstuckTarget = Bot.PathfindingHandler.GetRandomPointAround(
                    (int)Bot.Objects.MapId, playerPos, 10.0f);
                if (UnstuckTarget != Vector3.Zero)
                {
                    SetMovementAction(MovementAction.Move, UnstuckTarget);
                }
                _currentUnstuckStage = UnstuckStage.None; // Reset for next cycle
                _stuckCounter = 0;
                break;
        }
    }

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

    private void MountUp()
    {
        IEnumerable<WowMount> filteredMounts = Bot.Character.Mounts;

        if (Config.UseOnlySpecificMounts)
        {
            filteredMounts = filteredMounts.Where(e => Config.Mounts.Split(",", StringSplitOptions.RemoveEmptyEntries).Any(x => x.Equals(e.Name.Trim(), StringComparison.OrdinalIgnoreCase)));
        }

        if (filteredMounts != null && filteredMounts.Any())
        {
            WowMount mount = filteredMounts.ElementAt(Random.Shared.Next(0, filteredMounts.Count()));
            PreventMovement(TimeSpan.FromSeconds(2));
            Bot.Wow.CallCompanion(mount.Index, MountCommand);
        }
    }

    private void MoveCharacter(Vector3 positionToGoTo)
    {
        // CTM Throttling: Only update if significant change or enough time passed
        float distFromLast = positionToGoTo.GetDistance(_lastCtmTarget);
        double msSinceUpdate = (DateTime.UtcNow - _lastCtmUpdate).TotalMilliseconds;

        // Smoother threshold: allow updates if direction changed significantly
        int throttleMs = Config.MovementSettings.MinUpdateIntervalMs > 0 ? Config.MovementSettings.MinUpdateIntervalMs : 50;
        if (distFromLast < 0.5f && msSinceUpdate < throttleMs)
        {
            return; // Skip this update to prevent jitter
        }

        // SMOOTH TARGET: Lerp towards new position to avoid sudden jumps
        if (_smoothedTarget == Vector3.Zero)
        {
            _smoothedTarget = positionToGoTo;
        }
        else
        {
            // Smooth factor: 0.7 = fast tracking, 0.3 = slow smooth
            float smoothFactor = 0.6f;
            _smoothedTarget = new Vector3(
                _smoothedTarget.X + ((positionToGoTo.X - _smoothedTarget.X) * smoothFactor),
                _smoothedTarget.Y + ((positionToGoTo.Y - _smoothedTarget.Y) * smoothFactor),
                _smoothedTarget.Z + ((positionToGoTo.Z - _smoothedTarget.Z) * smoothFactor)
            );
        }

        Vector3 node = Bot.PathfindingHandler.MoveAlongSurface((int)Bot.Objects.MapId, Bot.Player.Position, _smoothedTarget);

        // Adaptive Turn Speed: Slower when mounted, faster in combat
        float baseTurnSpeed = Bot.Player.IsMounted ? 12.0f : (Bot.Player.IsInCombat ? 25.0f : 18.0f);
        float turnSpeedVariance = (float)(Random.Shared.NextDouble() - 0.5) * 2.0f; // ±1 (reduced variance)
        float turnSpeed = baseTurnSpeed + turnSpeedVariance;

        if (node != Vector3.Zero)
        {
            Bot.Character.MoveToPosition(node, turnSpeed, 0.25f);
            _lastCtmUpdate = DateTime.UtcNow;
            _lastCtmTarget = node;

            if (Config.MovementSettings.EnableDistanceMovedJumpCheck)
            {
                DistanceMovedJumpCheck();
            }
        }
        else
        {
            // Fallback: If MoveAlongSurface fails, try direct CTM
            Bot.Character.MoveToPosition(_smoothedTarget, turnSpeed, 0.25f);
            _lastCtmUpdate = DateTime.UtcNow;
            _lastCtmTarget = positionToGoTo;
        }
    }
}
