using AmeisenBotX.BehaviorTree.Enums;
using AmeisenBotX.BehaviorTree.Objects;
using AmeisenBotX.Common.Math;
using AmeisenBotX.Common.Utils;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Logic.Leafs.Movement;

/// <summary>
/// Intelligent role-based combat movement with AoE avoidance and spread mechanics.
/// - Tank: Position between target and party, face target away from group
/// - Healer: Maintain 20-25yd range with LoS to party, avoid AoE
/// - Melee DPS: Stay behind target (rogues) or at optimal melee range, avoid AoE
/// - Ranged DPS: Maintain max casting range with LoS, spread from other ranged
/// </summary>
public class CombatMovementLeaf(AmeisenBotInterfaces bot) : INode
{
    public string Name { get; } = "Movement.Combat";

    private AmeisenBotInterfaces Bot { get; } = bot;

    // Throttled checks to avoid expensive operations - randomized intervals for humanization
    private TimegatedEvent LoSCheckEvent { get; } = new(TimeSpan.FromMilliseconds(900 + Random.Shared.Next(200)));
    private TimegatedEvent AoECheckEvent { get; } = new(TimeSpan.FromMilliseconds(400 + Random.Shared.Next(200)));
    private TimegatedEvent SpreadCheckEvent { get; } = new(TimeSpan.FromMilliseconds(1800 + Random.Shared.Next(400)));
    private TimegatedEvent PathValidationEvent { get; } = new(TimeSpan.FromMilliseconds(200 + Random.Shared.Next(100)));

    // Stuck Detection
    private Vector3 _lastStuckCheckPos;
    private DateTime _stuckCheckTime;
    private DateTime _movementBlockedUntil;

    private bool LastLoSResult { get; set; } = true;
    private bool IsInAoE { get; set; } = false;
    private Vector3 AoEEscapePosition { get; set; } = Vector3.Zero;

    // Base positioning constants (will be jittered at runtime)
    private const float HEALER_OPTIMAL_RANGE_BASE = 22f;
    private const float HEALER_MIN_RANGE_BASE = 12f;
    private const float RANGED_OPTIMAL_RANGE_BASE = 26f;
    private const float RANGED_MIN_RANGE_BASE = 18f;
    private const float BEHIND_TARGET_DISTANCE = 2f;
    private const float SPREAD_MIN_DISTANCE = 6f;
    private const float AOE_ESCAPE_DISTANCE = 8f;
    private const float JITTER_PERCENT = 0.12f; // ±12% variance

    // Cached jittered values (refreshed periodically for natural drift)
    private float _healerOptimalRange;
    private float _healerMinRange;
    private float _rangedOptimalRange;
    private float _rangedMinRange;
    private DateTime _nextJitterRefresh = DateTime.MinValue;

    /// <summary>
    /// Applies random ±12% variance to a base value to avoid perfect distances.
    /// </summary>
    private static float Jitter(float baseValue)
    {
        float variance = baseValue * JITTER_PERCENT;
        return baseValue + ((Random.Shared.NextSingle() - 0.5f) * 2f * variance);
    }

    private void RefreshJitteredRanges()
    {
        if (DateTime.UtcNow > _nextJitterRefresh)
        {
            _healerOptimalRange = Jitter(HEALER_OPTIMAL_RANGE_BASE);
            _healerMinRange = Jitter(HEALER_MIN_RANGE_BASE);
            _rangedOptimalRange = Jitter(RANGED_OPTIMAL_RANGE_BASE);
            _rangedMinRange = Jitter(RANGED_MIN_RANGE_BASE);
            // Refresh every 30-60 seconds for natural drift
            _nextJitterRefresh = DateTime.UtcNow.AddSeconds(30 + Random.Shared.Next(30));
        }
    }

    public BtStatus Execute()
    {
        // Combat class handles its own movement
        if (Bot.CombatClass != null && Bot.CombatClass.HandlesMovement)
        {
            return BtStatus.Success;
        }

        // Refresh jittered ranges periodically for natural movement variance
        RefreshJitteredRanges();

        // Validate player and target
        if (!IWowUnit.IsValidAliveInCombat(Bot.Player) || !IWowUnit.IsValidAlive(Bot.Target) || Bot.Player.IsGhost)
        {
            return BtStatus.Success;
        }

        // --- FALLBACK STUCK DETECTION START ---
        if (DateTime.UtcNow < _movementBlockedUntil)
        {
            return BtStatus.Success; // Suppress movement while "stuck-blocked"
        }
        // --- FALLBACK STUCK DETECTION END ---

        // ===== PRIORITY 1: AoE AVOIDANCE =====
        // This takes priority over everything else - survival first!
        if (CheckAndAvoidAoE())
        {
            return BtStatus.Ongoing;
        }

        // Prevent movement during casts (but allow AoE escape above)
        if (Bot.Player.IsCasting)
        {
            return BtStatus.Success;
        }

        // Execute role-based positioning
        return Bot.CombatClass?.Role switch
        {
            WowRole.Tank => ExecuteTankPositioning(),
            WowRole.Heal => ExecuteHealerPositioning(),
            WowRole.Dps => Bot.CombatClass.IsMelee ? ExecuteMeleePositioning() : ExecuteRangedPositioning(),
            _ => ExecuteDefaultPositioning()
        };
    }

    /// <summary>
    /// Check for AoE effects and escape if standing in one.
    /// Priority 1 - survival takes precedence over positioning!
    /// </summary>
    private bool CheckAndAvoidAoE()
    {
        if (!AoECheckEvent.Run())
        {
            // Still escaping from previous AoE detection
            if (IsInAoE && AoEEscapePosition != Vector3.Zero)
            {
                float distanceToEscape = Bot.Player.DistanceTo(AoEEscapePosition);
                if (distanceToEscape > 2f)
                {
                    SafeMoveTo(AoEEscapePosition);
                    return true;
                }
                else
                {
                    IsInAoE = false;
                    AoEEscapePosition = Vector3.Zero;
                }
            }
            return false;
        }

        // Check for hostile AoE effects near player
        IEnumerable<IWowDynobject> hostileAoE = Bot.Objects.All
            .OfType<IWowDynobject>()
            .Where(e =>
            {
                IWowUnit caster = Bot.GetWowObjectByGuid<IWowUnit>(e.Caster);
                return caster != null
                    && Bot.Db.GetReaction(Bot.Player, caster) is WowUnitReaction.Hostile or WowUnitReaction.Neutral
                    && e.Position.GetDistance(Bot.Player.Position) < e.Radius + 3f;
            });

        if (hostileAoE.Any())
        {
            IsInAoE = true;

            // Calculate escape direction: away from the mean AoE position
            Vector3 meanAoEPos = BotMath.GetMeanPosition(hostileAoE.Select(e => e.Position));
            Vector3 escapeDirection = Bot.Player.Position - meanAoEPos;
            escapeDirection.Normalize();

            // Escape position: move away from AoE center
            AoEEscapePosition = Bot.Player.Position + (escapeDirection * AOE_ESCAPE_DISTANCE);

            Bot.Movement.SetMovementAction(MovementAction.Move, AoEEscapePosition);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Tank positioning: Stay between target and party, face target away from group.
    /// </summary>
    private BtStatus ExecuteTankPositioning()
    {
        float distance = Bot.Player.DistanceTo(Bot.Target);

        // If we have aggro, position between target and party
        if (Bot.Target.TargetGuid == Bot.Wow.PlayerGuid)
        {
            Vector3 partyCenter = Bot.Objects.CenterPartyPosition;

            // Only reposition if party exists
            if (partyCenter != Vector3.Zero && Bot.Objects.Partymembers.Any())
            {
                // Calculate ideal tank position: Between target and party, at melee range
                Vector3 directionToParty = partyCenter - Bot.Target.Position;
                directionToParty.Normalize();

                // Position slightly past the target, away from party
                Vector3 idealTankSpot = Bot.Target.Position - (directionToParty * 2f);

                float distanceToIdealSpot = Bot.Player.DistanceTo(idealTankSpot);

                // Move to ideal spot if too far, but stay in melee range of target
                if (distanceToIdealSpot > 3f && distance <= Bot.Player.MeleeRangeTo(Bot.Target) + 2f)
                {
                    SafeMoveTo(idealTankSpot);
                    return BtStatus.Ongoing;
                }
            }
        }

        // Basic tank logic: Stay in melee range
        if (distance > Bot.Player.MeleeRangeTo(Bot.Target))
        {
            Bot.Movement.SetMovementAction(MovementAction.Chase, Bot.Target.Position);
            return BtStatus.Ongoing;
        }

        // Face target (tank should always face target)
        if (!BotMath.IsFacing(Bot.Player.Position, Bot.Player.Rotation, Bot.Target.Position, 0.5f))
        {
            Bot.Wow.FacePosition(Bot.Player.BaseAddress, Bot.Player.Position, Bot.Target.Position);
        }

        return BtStatus.Success;
    }

    /// <summary>
    /// Healer positioning: Maintain range to lowest health ally, stay in LoS, spread from other healers.
    /// </summary>
    private BtStatus ExecuteHealerPositioning()
    {
        // Find the unit we should be near (lowest health party member or tank)
        IWowUnit healFocusTarget = Bot.Objects.Partymembers
            .Where(e => e != null && !e.IsDead)
            .OrderBy(e => e.HealthPercentage)
            .FirstOrDefault() ?? Bot.Target;

        if (healFocusTarget == null)
        {
            return BtStatus.Success;
        }

        float distance = Bot.Player.DistanceTo(healFocusTarget);
        bool hasLoS = CheckLineOfSight(healFocusTarget.Position);

        // Too close - back up (avoid cleaves/melee)
        if (distance < _healerMinRange)
        {
            Vector3 backupSpot = GetBackupPosition(healFocusTarget.Position, 5f);
            SafeMoveTo(backupSpot);
            return BtStatus.Ongoing;
        }

        // Too far or no LoS - move closer
        if (distance > _healerOptimalRange || !hasLoS)
        {
            Vector3 optimalSpot = GetOptimalRangePosition(healFocusTarget.Position, _healerOptimalRange);
            SafeMoveTo(optimalSpot);
            return BtStatus.Ongoing;
        }

        // Spread check for healers (avoid stacking for AoE)
        return SpreadCheckEvent.Run() && CheckAndSpreadFromAllies(WowRole.Heal) ? BtStatus.Ongoing : BtStatus.Success;
    }

    /// <summary>
    /// Melee DPS positioning: Behind target for backstab classes, otherwise optimal melee.
    /// </summary>
    private BtStatus ExecuteMeleePositioning()
    {
        float distance = Bot.Player.DistanceTo(Bot.Target);

        // Need to get into melee range first
        if (distance > Bot.Player.MeleeRangeTo(Bot.Target))
        {
            Bot.Movement.SetMovementAction(MovementAction.Chase, Bot.Target.Position);
            return BtStatus.Ongoing;
        }

        // If WalkBehindEnemy is set, position behind target (Rogues, Feral Druids)
        if (Bot.CombatClass != null && Bot.CombatClass.WalkBehindEnemy)
        {
            Vector3 behindPosition = GetBehindPosition(Bot.Target, BEHIND_TARGET_DISTANCE);
            float distanceToBehind = Bot.Player.DistanceTo(behindPosition);

            // Only move if we're not already behind
            if (distanceToBehind > 1.5f)
            {
                SafeMoveTo(behindPosition);
                return BtStatus.Ongoing;
            }
        }
        else
        {
            // For non-backstab melee, avoid standing directly in front (cleaves)
            if (IsInFrontOfTarget(Bot.Target))
            {
                Vector3 sidePosition = GetSidePosition(Bot.Target, BEHIND_TARGET_DISTANCE);
                SafeMoveTo(sidePosition);
                return BtStatus.Ongoing;
            }
        }

        return BtStatus.Success;
    }

    /// <summary>
    /// Ranged DPS positioning: Max casting range with LoS, spread from other ranged.
    /// </summary>
    private BtStatus ExecuteRangedPositioning()
    {
        float distance = Bot.Player.DistanceTo(Bot.Target);
        bool hasLoS = CheckLineOfSight(Bot.Target.Position);

        // Too far or no LoS - move closer
        if (distance > _rangedOptimalRange + Bot.Target.CombatReach || !hasLoS)
        {
            Vector3 optimalSpot = GetOptimalRangePosition(Bot.Target.Position, _rangedOptimalRange);
            SafeMoveTo(optimalSpot);
            return BtStatus.Ongoing;
        }

        // Too close - back up to optimal range
        if (distance < _rangedMinRange)
        {
            Vector3 backupSpot = GetBackupPosition(Bot.Target.Position, 5f);
            SafeMoveTo(backupSpot);
            return BtStatus.Ongoing;
        }

        // Spread check for ranged DPS (avoid stacking for AoE)
        return SpreadCheckEvent.Run() && CheckAndSpreadFromAllies(WowRole.Dps) ? BtStatus.Ongoing : BtStatus.Success;
    }

    /// <summary>
    /// Default positioning for unknown roles.
    /// </summary>
    private BtStatus ExecuteDefaultPositioning()
    {
        float distance = Bot.Player.DistanceTo(Bot.Target);

        if (distance > Bot.Player.MeleeRangeTo(Bot.Target))
        {
            Bot.Movement.SetMovementAction(MovementAction.Chase, Bot.Target.Position);
            return BtStatus.Ongoing;
        }

        return BtStatus.Success;
    }

    /// <summary>
    /// Check if we're too close to allies of the same role and spread out.
    /// </summary>
    private bool CheckAndSpreadFromAllies(WowRole role)
    {
        // Find nearby party members with the same role
        IEnumerable<IWowUnit> nearbyAllies = Bot.Objects.Partymembers
            .Where(e => e != null
                && !e.IsDead
                && e.Guid != Bot.Player.Guid
                && e.Position.GetDistance(Bot.Player.Position) < SPREAD_MIN_DISTANCE);

        if (nearbyAllies.Any())
        {
            // Calculate spread direction: away from the mean ally position
            Vector3 meanAllyPos = BotMath.GetMeanPosition(nearbyAllies.Select(e => e.Position));
            Vector3 spreadDirection = Bot.Player.Position - meanAllyPos;
            spreadDirection.Normalize();

            // Move away but stay in range of target
            Vector3 spreadPosition = Bot.Player.Position + (spreadDirection * 4f);

            // Validate we're still in range after spreading
            float newDistance = spreadPosition.GetDistance(Bot.Target.Position);

            if (newDistance < (role == WowRole.Heal ? _healerOptimalRange : _rangedOptimalRange) + 5f)
            {
                SafeMoveTo(spreadPosition);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Get position behind target based on target's rotation.
    /// </summary>
    private Vector3 GetBehindPosition(IWowUnit target, float distance)
    {
        float behindAngle = target.Rotation + MathF.PI;

        return new Vector3(
            target.Position.X + (MathF.Cos(behindAngle) * distance),
            target.Position.Y + (MathF.Sin(behindAngle) * distance),
            target.Position.Z
        );
    }

    /// <summary>
    /// Get position to the side of target (avoid frontal cleaves).
    /// </summary>
    private Vector3 GetSidePosition(IWowUnit target, float distance)
    {
        float sideAngle = target.Rotation + (MathF.PI / 2f);

        return new Vector3(
            target.Position.X + (MathF.Cos(sideAngle) * distance),
            target.Position.Y + (MathF.Sin(sideAngle) * distance),
            target.Position.Z
        );
    }

    /// <summary>
    /// Get optimal range position at specified distance from target.
    /// </summary>
    private Vector3 GetOptimalRangePosition(Vector3 targetPosition, float optimalRange)
    {
        Vector3 directionToTarget = targetPosition - Bot.Player.Position;
        directionToTarget.Normalize();
        return targetPosition - (directionToTarget * optimalRange);
    }

    /// <summary>
    /// Get backup position away from target.
    /// </summary>
    private Vector3 GetBackupPosition(Vector3 targetPosition, float backupDistance)
    {
        Vector3 backupDirection = Bot.Player.Position - targetPosition;
        backupDirection.Normalize();
        return Bot.Player.Position + (backupDirection * backupDistance);
    }

    /// <summary>
    /// Check if player is standing in front of target (dangerous for cleaves).
    /// </summary>
    private bool IsInFrontOfTarget(IWowUnit target)
    {
        Vector3 directionToPlayer = Bot.Player.Position - target.Position;
        directionToPlayer.Normalize();

        float targetFacingX = MathF.Cos(target.Rotation);
        float targetFacingY = MathF.Sin(target.Rotation);

        float dot = (directionToPlayer.X * targetFacingX) + (directionToPlayer.Y * targetFacingY);
        return dot > 0.3f;
    }

    /// <summary>
    /// Throttled LoS check using game's built-in check.
    /// </summary>
    private bool CheckLineOfSight(Vector3 targetPosition)
    {
        if (LoSCheckEvent.Run())
        {
            LastLoSResult = Bot.Objects.IsTargetInLineOfSight;
        }
        return LastLoSResult;
    }

    public INode GetNodeToExecute()
    {
        return this;
    }

    /// <summary>
    /// Combines Smart Positioning (Path Validation) and Reactive Stuck Detection.
    /// </summary>
    private void SafeMoveTo(Vector3 targetPos)
    {
        // 1. Reactive Stuck Detection
        if (DateTime.UtcNow - _stuckCheckTime > TimeSpan.FromSeconds(2))
        {
            if (_lastStuckCheckPos != Vector3.Zero && Bot.Player.Position.GetDistance(_lastStuckCheckPos) < 1.0f)
            {
                // We tried to move for 2s but didn't go anywhere -> We are stuck or blocked.
                // Stop moving for 5 seconds and let the bot fight from here.
                _movementBlockedUntil = DateTime.UtcNow + TimeSpan.FromSeconds(5);
                Bot.Movement.StopMovement();
                _stuckCheckTime = DateTime.UtcNow;
                _lastStuckCheckPos = Bot.Player.Position;
                return;
            }

            _lastStuckCheckPos = Bot.Player.Position;
            _stuckCheckTime = DateTime.UtcNow;
        }

        // 2. Smart Positioning (Throttled or Direct)
        float distanceToTarget = Bot.Player.Position.DistanceTo(targetPos);

        if (PathValidationEvent.Run())
        {
            if (Bot.Movement.TryGetPath(targetPos, out IEnumerable<Vector3> path))
            {
                // Valid path found - snap target to the reachable mesh
                Vector3 reachableTarget = path.Last();
                Bot.Movement.SetMovementAction(MovementAction.Move, reachableTarget);
            }
            else
            {
                // Path not found - destination is unreachable (e.g. in a wall). Do nothing (stop moving).
                Bot.Movement.StopMovement();
            }
        }
        else
        {
            // In between checks, continue moving to the PREVIOUSLY validated position (or just re-issue move to target for CTM continuity)
            // However, since we don't store the validated target across frames efficiently here without extra state,
            // and SetMovementAction(Move) is stateful in the engine, we might not need to call it every tick if it's already moving.
            // BUT, to be safe and responsive:
            Bot.Movement.SetMovementAction(MovementAction.Move, targetPos);
        }
    }

}
