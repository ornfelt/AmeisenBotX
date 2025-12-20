using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.AI;
using AmeisenBotX.Core.Engines.Movement.AI;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Movement.Providers.Combat
{
    /// <summary>
    /// AI-driven combat movement that uses:
    /// - Threat awareness
    /// - Enemy cast detection
    /// - Tactical positioning (behind target, avoid cleaves)
    /// - Health-based kiting
    /// - Nearby unit awareness
    /// </summary>
    public class AiCombatMovementProvider : IMovementProvider, ICombatAi
    {
        private AmeisenBotInterfaces Bot { get; }

        private AmeisenBotConfig Config { get; }

        private AiSettings AiSettings { get; }

        private CombatStateAnalyzer StateAnalyzer { get; }

        private DateTime LastCastReaction { get; set; }

        private float LastWinProbability { get; set; } = 1.0f;

        private string LastAnalysisReason { get; set; } = string.Empty;

        private DateTime LastAnalysisTime { get; set; }

        private bool IsFleeing { get; set; }

        /// <summary>
        /// Exposed state analyzer for visualization.
        /// </summary>
        public CombatStateAnalyzer Analyzer => StateAnalyzer;

        /// <summary>
        /// Exposed action classifier for combat decision making.
        /// </summary>
        public CombatActionClassifier ActionClassifier { get; }

        /// <summary>
        /// Current combat strategy selected by AI.
        /// Respects the global AI master switch.
        /// </summary>
        public AiCombatStrategy CurrentStrategy
        {
            get
            {
                if (!AiSettings.Enabled || !AiSettings.CombatMovementEnabled)
                {
                    return AiCombatStrategy.Standard;
                }

                return StateAnalyzer?.CurrentStrategy ?? AiCombatStrategy.Standard;
            }
        }

        /// <summary>
        /// Current win probability calculated by AI (0.0-1.0).
        /// </summary>
        public float CurrentWinProbability => LastWinProbability;

        /// <summary>
        /// Reason for current AI decision.
        /// </summary>
        public string CurrentAnalysisReason => LastAnalysisReason;

        public AiCombatMovementProvider(AmeisenBotInterfaces bot, AmeisenBotConfig config)
        {
            Bot = bot;
            Config = config;
            AiSettings = Config.AiSettings;

            StateAnalyzer = new CombatStateAnalyzer(bot, config);
            ActionClassifier = new CombatActionClassifier(config);
        }

        public bool Get(out Vector3 position, out MovementAction type)
        {
            if (!AiSettings.Enabled || !AiSettings.CombatMovementEnabled)
            {
                AmeisenBotX.Logging.AmeisenLogger.I.Log("CombatAI", $"AI Skipped: Settings Disabled (Enabled={AiSettings.Enabled}, Combat={AiSettings.CombatMovementEnabled})", AmeisenBotX.Logging.Enums.LogLevel.Master);
                position = Vector3.Zero;
                type = MovementAction.None;
                return false;
            }

            // Always update probability to handle Idle state (-1.0f)
            UpdateWinProbability();

            if ((Bot.CombatClass != null && Bot.CombatClass.HandlesMovement)
                || !IWowUnit.IsValidAliveInCombat(Bot.Player)
                || Bot.Player.IsGhost)
            {
                // Combat class handles movement or player invalid
                if (Bot.CombatClass?.HandlesMovement == true)
                {
                    AmeisenBotX.Logging.AmeisenLogger.I.Log("CombatAI", "AI Skipped: CC Handles Movement", AmeisenBotX.Logging.Enums.LogLevel.Master);
                }

                position = Vector3.Zero;
                type = MovementAction.None;
                return false;
            }

            // Priority 0: Critical Score Override - Flee if probability is too low
            if (ShouldFleeBasedOnScore(out Vector3 fleeDestination))
            {
                position = fleeDestination;
                type = MovementAction.Flee;
                return true;
            }

            // Priority 1: Survival - flee if low health (Legacy check, kept for safety)
            if (ShouldFlee(out position))
            {
                type = MovementAction.Flee;
                return true;
            }

            // Priority 2: React to dangerous enemy casts
            if (AiSettings.ReactToEnemyCasts && ShouldDodgeCast(out position))
            {
                type = MovementAction.Move;
                return true;
            }

            // Priority 3: Tactical positioning based on role
            if (TryGetTacticalPosition(out position, out type, out float rotation))
            {
                return true;
            }

            position = Vector3.Zero;
            type = MovementAction.None;
            return false;
        }

        /// <summary>
        /// Check if we should flee based on health and threat.
        /// </summary>
        private bool ShouldFlee(out Vector3 fleePosition)
        {
            fleePosition = Vector3.Zero;

            // Check health threshold
            if (Bot.Player.HealthPercentage > AiSettings.FleeHealthThreshold)
                return false;

            // Non-tanks should flee when low
            if (GetRoleSafe() == WowRole.Tank)
                return false;

            // Find direction away from enemies
            Vector3 threatCenter = GetThreatCenter();
            if (threatCenter == Vector3.Zero)
                return false;

            Vector3 fleeDirection = Bot.Player.Position - threatCenter;
            fleeDirection.Normalize2D();

            fleePosition = Bot.Player.Position + fleeDirection * 10f;
            return true;
        }

        /// <summary>
        /// Check if an enemy is casting something dangerous and we should move.
        /// </summary>
        private bool ShouldDodgeCast(out Vector3 dodgePosition)
        {
            dodgePosition = Vector3.Zero;

            // Don't react too frequently
            if ((DateTime.UtcNow - LastCastReaction).TotalSeconds < 1.5f)
                return false;

            // Find enemies casting at us
            IEnumerable<IWowUnit> castingEnemies = Bot.Objects.All
                .OfType<IWowUnit>()
                .Where(u => !u.IsDead
                         && u.IsInCombat
                         && u.CurrentlyCastingSpellId != 0
                         && u.TargetGuid == Bot.Player.Guid
                         && Bot.Db.GetReaction(Bot.Player, u) == WowUnitReaction.Hostile
                         && u.Position.GetDistance(Bot.Player.Position) < 40);

            IWowUnit caster = castingEnemies.FirstOrDefault();
            if (caster == null)
                return false;

            // Move perpendicular to the caster to dodge
            Vector3 toCaster = caster.Position - Bot.Player.Position;
            Vector3 perpendicular = new() { X = -toCaster.Y, Y = toCaster.X, Z = 0 };
            perpendicular.Normalize2D();

            dodgePosition = Bot.Player.Position + perpendicular;
            LastCastReaction = DateTime.UtcNow;
            return true;
        }

        /// <summary>
        /// Get tactical position based on role and target.
        /// </summary>
        private bool TryGetTacticalPosition(out Vector3 position, out MovementAction type, out float rotation)
        {
            position = Vector3.Zero;
            type = MovementAction.None;
            rotation = 0f;

            if (!IWowUnit.IsValidAlive(Bot.Target))
                return false;

            float distance = Bot.Player.DistanceTo(Bot.Target);

            switch (GetRoleSafe())
            {
                case WowRole.Dps:
                    return GetDpsPosition(distance, out position, out type, out rotation);

                case WowRole.Heal:
                    return GetHealerPosition(distance, out position, out type, out rotation);

                case WowRole.Tank:
                    return GetTankPosition(distance, out position, out type, out rotation);
            }

            return false;
        }

        private bool GetDpsPosition(float distance, out Vector3 position, out MovementAction type, out float rotation)
        {
            position = Vector3.Zero;
            type = MovementAction.None;
            rotation = 0f;

            if (IsMeleeSafe())
            {
                float meleeRange = Bot.Player.MeleeRangeTo(Bot.Target);

                // AGGRO-AWARE POSITIONING: If enemy is not targeting us, position behind for better DPS/safety
                if (Bot.Target.TargetGuid != Bot.Player.Guid)
                {
                    Vector3 behindPosition = GetBehindTargetPosition();
                    float distanceToBehind = Bot.Player.DistanceTo(behindPosition);

                    // Only move if we're not already in a good position (1.5y tolerance)
                    if (distanceToBehind > 1.5f)
                    {
                        position = behindPosition;
                        type = MovementAction.Move;
                        return true;
                    }
                }
                // Enemy IS targeting us or we're already behind - use chase/pursuit to stick to them
                else if (distance > meleeRange)
                {
                    position = Bot.Target.Position;
                    type = MovementAction.Chase;
                    rotation = Bot.Target.Rotation;  // Enable pursuit prediction
                    return true;
                }
            }
            else // Ranged DPS
            {
                float maxRange = 28f;
                float minRange = 8f; // Stay out of melee

                if (distance > maxRange)
                {
                    position = Bot.Target.Position;
                    type = MovementAction.Chase;
                    rotation = Bot.Target.Rotation;  // Enable pursuit prediction
                    return true;
                }

                // Too close - back off (kiting)
                if (distance < minRange && AiSettings.KitingAggressiveness > 0)
                {
                    Vector3 awayFromTarget = Bot.Player.Position - Bot.Target.Position;
                    awayFromTarget.Normalize2D();
                    position = Bot.Player.Position + awayFromTarget * (minRange - distance + 3f);
                    type = MovementAction.Move;
                    return true;
                }
            }

            return false;
        }

        private bool GetHealerPosition(float distance, out Vector3 position, out MovementAction type, out float rotation)
        {
            position = Vector3.Zero;
            type = MovementAction.None;
            rotation = 0f;

            // Get party center if possible
            Vector3 partyCenter = GetPartyCenterPosition();
            float distanceToParty = Bot.Player.Position.GetDistance(partyCenter);

            // Stay near party but at safe distance from enemies
            if (distanceToParty > 15f)
            {
                position = partyCenter;
                type = MovementAction.Move;
                // Use target rotation if valid for pursuit prediction (helps stick to moving party)
                if (IWowUnit.IsValid(Bot.Target))
                {
                    rotation = Bot.Target.Rotation;
                }
                return true;
            }

            // Keep distance from target (likely an enemy near the group)
            if (IWowUnit.IsValid(Bot.Target) && distance < 10f)
            {
                Vector3 awayFromTarget = Bot.Player.Position - Bot.Target.Position;
                awayFromTarget.Normalize2D();
                position = Bot.Player.Position + awayFromTarget * 8f;
                type = MovementAction.Move;
                return true;
            }

            return false;
        }

        private bool GetTankPosition(float distance, out Vector3 position, out MovementAction type, out float rotation)
        {
            position = Vector3.Zero;
            type = MovementAction.None;
            rotation = 0f;

            float meleeRange = Bot.Player.MeleeRangeTo(Bot.Target);

            // Chase if not in melee
            if (distance > meleeRange)
            {
                position = Bot.Target.Position;
                type = MovementAction.Chase;
                rotation = Bot.Target.Rotation;  // Enable pursuit prediction
                return true;
            }

            // Face target away from party if possible
            if (AiSettings.AvoidFrontalCone)
            {
                Vector3 partyCenter = GetPartyCenterPosition();
                Vector3 idealPosition = GetPositionFacingAwayFromParty(partyCenter);

                if (Bot.Player.Position.GetDistance(idealPosition) > 2f)
                {
                    position = idealPosition;
                    type = MovementAction.Move;
                    return true;
                }
            }

            return false;
        }

        private Vector3 GetBehindTargetPosition()
        {
            // Calculate position behind target based on their rotation
            // Use 75% of melee range to ensure we're comfortably in backstab/behind range
            float distance = Bot.Player.MeleeRangeTo(Bot.Target) * 0.75f;
            return BotMath.CalculatePositionBehind(Bot.Target.Position, Bot.Target.Rotation, distance);
        }

        private bool IsBehindTarget()
        {
            Vector3 toPlayer = Bot.Player.Position - Bot.Target.Position;
            float angleToPlayer = MathF.Atan2(toPlayer.Y, toPlayer.X);
            float angleDiff = MathF.Abs(NormalizeAngle(angleToPlayer - Bot.Target.Rotation));

            // Behind = angle > 90 degrees from front
            return angleDiff > MathF.PI * 0.5f;
        }

        private void UpdateWinProbability()
        {
            if ((DateTime.UtcNow - LastAnalysisTime).TotalSeconds < 0.5) return;

            LastWinProbability = StateAnalyzer.CalculateWinProbability(out string reason);
            LastAnalysisReason = reason;
            LastAnalysisTime = DateTime.UtcNow;

            // Use STRATEGY for flee decision, not raw probability
            var currentStrategy = StateAnalyzer?.CurrentStrategy ?? AiCombatStrategy.Standard;

            // Hysteresis for flee state
            if (IsFleeing && currentStrategy != AiCombatStrategy.Flee && currentStrategy != AiCombatStrategy.Survival)
            {
                IsFleeing = false;
            }
            else if (!IsFleeing && currentStrategy == AiCombatStrategy.Flee)
            {
                IsFleeing = true;
                AmeisenBotX.Logging.AmeisenLogger.I.Log("CombatAI", $"Strategy: FLEE. Reason: {reason}. FLEEING.", AmeisenBotX.Logging.Enums.LogLevel.Warning);
            }
        }

        private bool ShouldFleeBasedOnScore(out Vector3 fleePosition)
        {
            fleePosition = Vector3.Zero;

            // Do not flee if we haven't analyzed yet (Idle/-1.0f)
            if (LastWinProbability < 0) return false;

            // Only flee if FLEE is the dominant strategy from the neural network
            var currentStrategy = StateAnalyzer?.CurrentStrategy ?? AiCombatStrategy.Standard;
            if (currentStrategy != AiCombatStrategy.Flee)
            {
                return false;
            }

            // Flee strategy is active - calculate flee position
            Vector3 threatCenter = GetThreatCenter();
            if (threatCenter == Vector3.Zero)
            {
                if (Bot.Target != null)
                    threatCenter = Bot.Target.Position;
                else
                    return false;
            }

            Vector3 fleeDirection = Bot.Player.Position - threatCenter;
            fleeDirection.Normalize2D();

            fleePosition = Bot.Player.Position + fleeDirection * 15f;
            return true;
        }

        private Vector3 GetThreatCenter()
        {
            IEnumerable<IWowUnit> threats = Bot.Objects.All
                .OfType<IWowUnit>()
                .Where(u => !u.IsDead
                         && u.IsInCombat
                         && Bot.Db.GetReaction(Bot.Player, u) == WowUnitReaction.Hostile
                         && u.Position.GetDistance(Bot.Player.Position) < 30);

            if (!threats.Any())
                return Vector3.Zero;

            float x = threats.Average(t => t.Position.X);
            float y = threats.Average(t => t.Position.Y);
            float z = threats.Average(t => t.Position.Z);

            return new Vector3 { X = x, Y = y, Z = z };
        }

        private Vector3 GetPartyCenterPosition()
        {
            IEnumerable<IWowUnit> party = Bot.Objects.Partymembers.Where(p => !p.IsDead);
            if (!party.Any())
                return Bot.Player.Position;

            float x = party.Average(p => p.Position.X);
            float y = party.Average(p => p.Position.Y);
            float z = party.Average(p => p.Position.Z);

            return new Vector3 { X = x, Y = y, Z = z };
        }

        private Vector3 GetPositionFacingAwayFromParty(Vector3 partyCenter)
        {
            // Tank should position so target faces away from party
            Vector3 targetToParty = partyCenter - Bot.Target.Position;
            targetToParty.Normalize2D();

            // Position on opposite side of target from party
            Vector3 idealPos = Bot.Target.Position - targetToParty * Bot.Player.MeleeRangeTo(Bot.Target) * 0.8f;
            return idealPos;
        }

        private WowRole GetRoleSafe()
        {
            if (Bot.CombatClass != null) return Bot.CombatClass.Role;

            // Default to DPS as requested (AutoAttack mode)
            return WowRole.Dps;
        }

        private bool IsMeleeSafe()
        {
            if (Bot.CombatClass != null) return Bot.CombatClass.IsMelee;

            // Default to Melee to ensure AutoAttacks work (match Logic behavior)
            return true;
        }

        private float NormalizeAngle(float angle)
        {
            while (angle > MathF.PI) angle -= 2 * MathF.PI;
            while (angle < -MathF.PI) angle += 2 * MathF.PI;
            return angle;
        }
    }
}
