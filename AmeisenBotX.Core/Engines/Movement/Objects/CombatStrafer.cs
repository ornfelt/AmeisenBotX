using AmeisenBotX.Common.Keyboard.Enums;
using AmeisenBotX.Common.Math;
using AmeisenBotX.Wow.Objects;
using System;
using System.Threading.Tasks;

namespace AmeisenBotX.Core.Engines.Movement.Objects
{
    /// <summary>
    /// Provides combat strafing functionality using keyboard input for more human-like movement.
    /// This module allows circle-strafing around targets using A/D keys instead of ClickToMove.
    /// Includes safety checks using MoveAlongSurface to prevent falling off cliffs.
    /// </summary>
    public partial class CombatStrafer
    {
        private const uint WM_KEYDOWN = 0x100;
        private const uint WM_KEYUP = 0x101;

        private readonly AmeisenBotInterfaces bot;
        private readonly Random random = new();

        private bool isStrafeKeyDown;
        private KeyCode currentStrafeKey;
        private DateTime strafeStartTime;
        private DateTime lastDirectionChange;
        private DateTime lastSafetyCheck;

        /// <summary>
        /// Whether strafing is currently active.
        /// </summary>
        public bool IsStrafing { get; private set; }

        /// <summary>
        /// Current strafe direction: true = clockwise (D key), false = counter-clockwise (A key).
        /// </summary>
        public bool IsClockwise { get; private set; }

        /// <summary>
        /// Minimum duration of a strafe in milliseconds.
        /// </summary>
        public int MinStrafeDurationMs { get; set; } = 300;

        /// <summary>
        /// Maximum duration of a strafe in milliseconds.
        /// </summary>
        public int MaxStrafeDurationMs { get; set; } = 800;

        /// <summary>
        /// Minimum time between direction changes in milliseconds.
        /// </summary>
        public int MinDirectionChangeIntervalMs { get; set; } = 1500;

        /// <summary>
        /// Chance (0.0 to 1.0) to change strafe direction on each update.
        /// </summary>
        public float DirectionChangeChance { get; set; } = 0.3f;

        /// <summary>
        /// How far ahead to check for safety (in yards).
        /// </summary>
        public float SafetyCheckDistance { get; set; } = 3.0f;

        /// <summary>
        /// How often to perform safety checks while strafing (in milliseconds).
        /// </summary>
        public int SafetyCheckIntervalMs { get; set; } = 200;

        /// <summary>
        /// Maximum allowed height difference for safe strafing (in yards).
        /// If the predicted position differs in Z by more than this, strafing is unsafe.
        /// </summary>
        public float MaxSafeHeightDifference { get; set; } = 2.0f;

        /// <summary>
        /// Gets the player's current strafe speed. Uses the actual movement speed from memory.
        /// Defaults to 7.0 yards/sec if not available.
        /// </summary>
        private float StrafeSpeed => bot.Player?.RunSpeed > 0 ? bot.Player.RunSpeed : 7.0f;

        public CombatStrafer(AmeisenBotInterfaces bot)
        {
            this.bot = bot;
        }

        /// <summary>
        /// Start strafing around the current target in a random direction.
        /// Performs safety check before starting.
        /// </summary>
        public void StartStrafing()
        {
            if (IsStrafing || bot.Target == null)
            {
                return;
            }

            // Pick random direction, but prefer one that's safe
            bool preferClockwise = random.Next(2) == 0;

            // Try preferred direction first
            if (IsStrafeDirectionSafe(preferClockwise))
            {
                IsClockwise = preferClockwise;
            }
            // Try opposite direction
            else if (IsStrafeDirectionSafe(!preferClockwise))
            {
                IsClockwise = !preferClockwise;
            }
            // Neither direction is safe, don't strafe
            else
            {
                return;
            }

            // Stop any ClickToMove first
            bot.Wow.StopClickToMove();

            currentStrafeKey = IsClockwise ? KeyCode.D : KeyCode.A;

            // Send key down
            SendKeyDown(currentStrafeKey);
            isStrafeKeyDown = true;
            strafeStartTime = DateTime.UtcNow;
            lastDirectionChange = DateTime.UtcNow;
            lastSafetyCheck = DateTime.UtcNow;
            IsStrafing = true;
        }

        /// <summary>
        /// Stop strafing and release any held keys.
        /// </summary>
        public void StopStrafing()
        {
            if (!IsStrafing)
            {
                return;
            }

            if (isStrafeKeyDown)
            {
                SendKeyUp(currentStrafeKey);
                isStrafeKeyDown = false;
            }

            IsStrafing = false;
        }

        /// <summary>
        /// Update the strafing behavior. Call this each tick while combat strafing is desired.
        /// Maintains facing toward target, handles strafe timing/direction changes, and performs safety checks.
        /// </summary>
        /// <returns>True if strafing is active, false otherwise.</returns>
        public bool Update()
        {
            if (!IsStrafing)
            {
                return false;
            }

            IWowUnit target = bot.Target;

            // Stop if no target or target is dead
            if (target == null || target.IsDead)
            {
                StopStrafing();
                return false;
            }

            // Periodic safety check - stop if we're about to fall off a cliff
            TimeSpan timeSinceSafetyCheck = DateTime.UtcNow - lastSafetyCheck;
            if (timeSinceSafetyCheck.TotalMilliseconds > SafetyCheckIntervalMs)
            {
                if (!IsStrafeDirectionSafe(IsClockwise))
                {
                    // Try to change to opposite direction
                    if (IsStrafeDirectionSafe(!IsClockwise))
                    {
                        ChangeDirection();
                    }
                    else
                    {
                        // Neither direction is safe, stop strafing
                        StopStrafing();
                        return false;
                    }
                }
                lastSafetyCheck = DateTime.UtcNow;
            }

            // Keep facing the target while strafing
            bot.Wow.FacePosition(bot.Player.BaseAddress, bot.Player.Position, target.Position);

            // Check if we should change direction (randomly)
            TimeSpan timeSinceDirectionChange = DateTime.UtcNow - lastDirectionChange;
            if (timeSinceDirectionChange.TotalMilliseconds > MinDirectionChangeIntervalMs
                && random.NextDouble() < DirectionChangeChance)
            {
                // Only change if the new direction is safe
                if (IsStrafeDirectionSafe(!IsClockwise))
                {
                    ChangeDirection();
                }
            }

            // Check if current strafe duration exceeded
            TimeSpan strafeDuration = DateTime.UtcNow - strafeStartTime;
            int targetDuration = random.Next(MinStrafeDurationMs, MaxStrafeDurationMs);

            if (strafeDuration.TotalMilliseconds > targetDuration)
            {
                // Briefly pause then continue
                StopStrafing();

                // Small delay before next strafe
                Task.Delay(random.Next(50, 150)).ContinueWith(_ =>
                {
                    if (bot.Target != null && !bot.Target.IsDead && bot.Player.IsInCombat)
                    {
                        StartStrafing();
                    }
                });
            }

            return true;
        }

        /// <summary>
        /// Change strafe direction mid-strafe (with safety check).
        /// </summary>
        public void ChangeDirection()
        {
            if (!IsStrafing)
            {
                return;
            }

            // Release current key
            if (isStrafeKeyDown)
            {
                SendKeyUp(currentStrafeKey);
            }

            // Switch direction
            IsClockwise = !IsClockwise;
            currentStrafeKey = IsClockwise ? KeyCode.D : KeyCode.A;

            // Press new key
            SendKeyDown(currentStrafeKey);
            isStrafeKeyDown = true;
            lastDirectionChange = DateTime.UtcNow;
        }

        /// <summary>
        /// Check if combat strafing should be used based on current combat situation.
        /// </summary>
        /// <param name="maxDistance">Maximum distance to target for strafing.</param>
        /// <returns>True if strafing is appropriate.</returns>
        public bool ShouldStrafe(float maxDistance = 5.0f)
        {
            if (bot.Target == null || !bot.Player.IsInCombat)
            {
                return false;
            }

            float distance = bot.Player.Position.GetDistance(bot.Target.Position);

            // Only strafe when in melee range
            return distance <= maxDistance
                && !bot.Target.IsDead
                && !bot.Player.IsCasting;
        }

        /// <summary>
        /// Check if strafing in the given direction is safe (won't fall off a cliff).
        /// Uses MoveAlongSurface to validate the predicted strafe position.
        /// </summary>
        /// <param name="clockwise">True for clockwise (right/D), false for counter-clockwise (left/A).</param>
        /// <returns>True if the direction is safe to strafe.</returns>
        private bool IsStrafeDirectionSafe(bool clockwise)
        {
            Vector3 currentPos = bot.Player.Position;
            float rotation = bot.Player.Rotation;

            // Calculate strafe direction: perpendicular to facing direction
            // Clockwise (right strafe) = rotation - 90 degrees
            // Counter-clockwise (left strafe) = rotation + 90 degrees
            float strafeAngle = clockwise
                ? rotation - MathF.PI / 2.0f
                : rotation + MathF.PI / 2.0f;

            // Calculate predicted position after strafing
            Vector3 predictedPos = new()
            {
                X = currentPos.X + MathF.Cos(strafeAngle) * SafetyCheckDistance,
                Y = currentPos.Y + MathF.Sin(strafeAngle) * SafetyCheckDistance,
                Z = currentPos.Z
            };

            // Use pathfinding to check if we can actually reach that position
            Vector3 validatedPos = bot.PathfindingHandler.MoveAlongSurface(
                (int)bot.Objects.MapId,
                currentPos,
                predictedPos
            );

            // If MoveAlongSurface returns zero, the path is invalid
            if (validatedPos == Vector3.Zero)
            {
                return false;
            }

            // Check if the validated position is close to our predicted position
            // If they differ significantly, there's an obstacle or cliff
            float horizontalDistance = new Vector3(
                predictedPos.X - validatedPos.X,
                predictedPos.Y - validatedPos.Y,
                0
            ).GetMagnitude2D();

            float heightDifference = MathF.Abs(predictedPos.Z - validatedPos.Z);

            // Safe if:
            // 1. The horizontal distance is roughly what we expected (within 0.5 yards tolerance)
            // 2. The height difference isn't too large (cliff detection)
            bool isSafe = horizontalDistance < SafetyCheckDistance * 0.5f
                       && heightDifference < MaxSafeHeightDifference;

            return isSafe;
        }

        private void SendKeyDown(KeyCode key)
        {
            nint windowHandle = bot.Memory.Process.MainWindowHandle;
            SendMessage(windowHandle, WM_KEYDOWN, new nint((int)key), nint.Zero);
        }

        private void SendKeyUp(KeyCode key)
        {
            nint windowHandle = bot.Memory.Process.MainWindowHandle;
            SendMessage(windowHandle, WM_KEYUP, new nint((int)key), nint.Zero);
        }

        [System.Runtime.InteropServices.LibraryImport("user32")]
        private static partial nint SendMessage(nint windowHandle, uint msg, nint param, nint parameter);
    }
}
