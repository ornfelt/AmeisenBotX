using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Wow.Objects;
using AmeisenBotX.Wow.Objects.Flags;
using System;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Movement.Objects
{
    public class BasicVehicle(AmeisenBotInterfaces bot)
    {
        public delegate void MoveCharacter(Vector3 positionToGoTo);
        public delegate void JumpCharacter();

        public DateTime LastUpdate { get; private set; } = DateTime.UtcNow;
        public Vector3 Velocity { get; private set; }

        private AmeisenBotInterfaces Bot { get; } = bot;

        // CTM State
        private DateTime _lastMovePacket = DateTime.MinValue;
        private Vector3 _lastSentDirection = Vector3.Zero;
        private Vector3 _lastSentPosition = Vector3.Zero;

        // Solver Vars
        private Vector3 _targetRandomizer = Vector3.Zero;

        // Wall avoidance throttling (TraceLine is expensive)
        private DateTime _lastWallCheck = DateTime.MinValue;
        private Vector3 _cachedWallForce = Vector3.Zero;
        private const int WallCheckIntervalMs = 200;

        public void Update(
            MoveCharacter moveCharacter,
            JumpCharacter jumpCharacter,
            MovementAction movementAction,
            Vector3 targetPosition,
            float rotation,
            float maxSteering,
            float maxVelocity,
            float seperationDistance,
            float arrivalThreshold3D = 0.6f,
            float arrivalThreshold2D = 0.5f,
            float heightTolerance = 2.0f,
            float arriveSlowdownRadius = 4.0f,
            float separationDisableDistance = 4.0f,
            float velocityDamping = 0.92f)
        {
            // --------------------------------------------------------------------------------
            // 1. DIRECT MOVE / FALLBACK (Shortcut forced by Engine)
            // --------------------------------------------------------------------------------
            if (movementAction == MovementAction.DirectMove)
            {
                if (IsSafeShortcut(targetPosition))
                {
                    float dist2D = Bot.Player.Position.DistanceTo2D(targetPosition);
                    float zDiff = Bot.Player.Position.Z - targetPosition.Z;

                    // Edge Jump: Wenn wir auf eine Kante zulaufen (> 1.5m tief)
                    if (dist2D < 6.0f && zDiff > 1.5f && Velocity.Length() > 4.0f)
                    {
                        jumpCharacter?.Invoke();
                    }

                    moveCharacter?.Invoke(targetPosition);

                    // Fake Velocity Update damit Physics nicht einschlafen
                    Velocity = (targetPosition - Bot.Player.Position).Normalized() * maxVelocity;
                    return;
                }

                // Fallback: Wenn DirectMove unsicher ist, springen wir unten in die normale Physik,
                // damit Avoidance greift und wir nicht gegen die Wand laufen.
            }

            // --------------------------------------------------------------------------------
            // 2. AUTO-SHORTCUT (Smart Drop Detection)
            // --------------------------------------------------------------------------------
            bool isMoveOrFollow = movementAction is MovementAction.Move or MovementAction.Follow or MovementAction.Chase;
            float distToTarget3D = Bot.Player.Position.DistanceTo(targetPosition);

            if (isMoveOrFollow)
            {
                float zDiff = Bot.Player.Position.Z - targetPosition.Z;
                bool isDrop = zDiff > 1.6f; // Nur bei deutlichem Drop (> 1.6m)

                // Wenn wir nah an einer Klippe sind (< 20m) und runter können -> Shortcut!
                if (isDrop && distToTarget3D < 20.0f && IsSafeShortcut(targetPosition))
                {
                    if (Velocity.Length() > 4.0f && distToTarget3D < 5.0f)
                    {
                        jumpCharacter?.Invoke();
                    }

                    moveCharacter?.Invoke(targetPosition);
                    Velocity = (targetPosition - Bot.Player.Position).Normalized() * maxVelocity;
                    return;
                }
            }

            // --------------------------------------------------------------------------------
            // 3. ARRIVAL & STOP LOGIC (Anti-Stuck & Anti-Jitter)
            // --------------------------------------------------------------------------------
            float distToTarget2D = Bot.Player.Position.DistanceTo2D(targetPosition);
            bool isArriving = isMoveOrFollow && distToTarget2D < arriveSlowdownRadius;

            if (isArriving)
            {
                float heightDiff = Math.Abs(Bot.Player.Position.Z - targetPosition.Z);

                // STOP CONDITION: Use configurable thresholds
                if (distToTarget3D < arrivalThreshold3D || (distToTarget2D < arrivalThreshold2D && heightDiff < heightTolerance))
                {
                    Velocity = Vector3.Zero;
                    _targetRandomizer = Vector3.Zero;
                    return; // Hartes Return -> Bot steht still.
                }
            }

            // Humanizer Randomizer (Nur berechnen wenn wir weit weg sind)
            if (!isArriving && _targetRandomizer == Vector3.Zero && distToTarget3D > 15.0f)
            {
                _targetRandomizer = new Vector3((float)Random.Shared.NextDouble() - 0.5f, (float)Random.Shared.NextDouble() - 0.5f, 0) * 0.5f;
            }
            Vector3 finalTarget = targetPosition + _targetRandomizer;

            // --------------------------------------------------------------------------------
            // 4. PHYSICS UPDATE
            // --------------------------------------------------------------------------------
            DateTime now = DateTime.UtcNow;
            float dt = (float)(now - LastUpdate).TotalSeconds;
            LastUpdate = now;

            if (dt <= 0.0f)
            {
                dt = 0.001f;
            }

            if (dt > 0.1f)
            {
                dt = 0.1f;
            }

            if (Bot.Player.IsMounted)
            {
                maxVelocity *= 2.0f;
                maxSteering *= 1.5f;
            }

            // Force Calculation
            Vector3 force = GetForce(movementAction, finalTarget, rotation, maxSteering, maxVelocity, seperationDistance, distToTarget3D, separationDisableDistance, arriveSlowdownRadius);

            // WALL UNSTUCK
            if (force.Length() > 0.5f && Velocity.Length() < 0.3f)
            {
                // We are stuck? push away from nearest obstacles
                force += AvoidObstacles(maxSteering, maxVelocity, 1.0f);
                // Also add some random noise to maybe slide off the wall
                force += Wander(maxSteering, maxVelocity, 2.0f);
            }

            // Apply force to velocity
            Velocity += force;

            // Apply stronger damping to reduce oscillation
            Velocity *= velocityDamping;

            // Dead zone: Ignore tiny movements that cause jitter
            if (Velocity.Length() < 0.3f)
            {
                Velocity = Vector3.Zero;
            }

            // Truncate to max velocity
            Velocity.Truncate(maxVelocity);

            // Only send CTM if we have meaningful velocity
            if (Velocity.Length() > 0.5f)
            {
                moveCharacter?.Invoke(Bot.Player.Position + Velocity);
            }
            LastUpdate = DateTime.UtcNow;
        }


        private bool IsSafeShortcut(Vector3 target)
        {
            Vector3 myPos = Bot.Player.Position;
            float heightDiff = myPos.Z - target.Z;

            if (heightDiff < -1.6f)
            {
                return false;
            }

            if (heightDiff > 40.0f)
            {
                return false;
            }

            if (Math.Abs(target.Z) < 0.1f)
            {
                return false;
            }

            if (myPos.DistanceTo2D(target) > 25.0f)
            {
                return false;
            }

            Vector3 start = myPos + new Vector3(0, 0, 1.8f);
            float targetZOffset = 1.0f;
            if (target.Z > myPos.Z)
            {
                targetZOffset = 1.5f;
            }

            Vector3 end = target + new Vector3(0, 0, targetZOffset);

            bool hitWall = Bot.Wow.TraceLine(start, end, (uint)WowWorldFrameHitFlag.HitTestGroundAndStructures);
            return !hitWall;
        }

        private Vector3 GetForce(MovementAction action, Vector3 target, float rotation, float maxSteering, float maxVel, float sepDist, float distToTarget, float separationDisableDistance, float arriveSlowdownRadius)
        {
            Vector3 f = Vector3.Zero;

            // DYNAMIC SEPARATION: Disable separation near target to prevent jitter
            float sepMult = (distToTarget < separationDisableDistance) ? 0.0f : 1.0f;

            switch (action)
            {
                case MovementAction.DirectMove:
                    f += Seek(target, maxSteering, maxVel, 1.0f);
                    f += AvoidObstacles(maxSteering, maxVel, 1.0f);
                    break;

                case MovementAction.Move:
                case MovementAction.Follow:
                    // Soft Arrival using configurable slowdown radius
                    if (distToTarget > arriveSlowdownRadius)
                    {
                        f += Seek(target, maxSteering, maxVel, 1.0f);
                    }
                    else
                    {
                        f += Arrive(target, maxSteering, maxVel, arriveSlowdownRadius);
                    }

                    f += AvoidObstacles(maxSteering, maxVel, 1.0f);
                    f += Separate(sepDist, maxVel, sepMult);
                    break;

                case MovementAction.Chase:
                    f += Seek(target, maxSteering, maxVel, 1.0f);
                    f += AvoidObstacles(maxSteering, maxVel, 0.4f);
                    break;

                case MovementAction.Flee:
                    f += Flee(target, maxSteering, maxVel, 1.0f).ZeroZ();
                    f += AvoidObstacles(maxSteering, maxVel, 1.2f);
                    break;

                case MovementAction.Evade:
                    f += Evade(target, maxSteering, maxVel, 1.0f, rotation);
                    break;

                case MovementAction.Wander:
                    f += Wander(maxSteering, maxVel, 1.0f).ZeroZ();
                    f += AvoidObstacles(maxSteering, maxVel, 2.0f);
                    break;

                case MovementAction.Unstuck:
                    f += Unstuck(maxSteering, maxVel, 1.0f);
                    break;
            }
            return f;
        }

        public Vector3 Seek(Vector3 target, float maxSteer, float maxVel, float mult)
        {
            Vector3 desired = (target - Bot.Player.Position).Normalized() * maxVel;
            return (desired - Velocity) * mult;
        }

        public Vector3 Arrive(Vector3 target, float maxSteer, float maxVel, float slowRad)
        {
            Vector3 toTarget = target - Bot.Player.Position;
            float dist = toTarget.Length();
            if (dist < 0.2f)
            {
                return -Velocity;
            }

            Vector3 desired = toTarget.Normalized();
            if (dist < slowRad)
            {
                desired *= maxVel * (dist / slowRad);
            }
            else
            {
                desired *= maxVel;
            }

            return desired - Velocity;
        }

        public Vector3 Flee(Vector3 target, float maxSteer, float maxVel, float mult)
        {
            Vector3 desired = (Bot.Player.Position - target).Normalized() * maxVel;
            return (desired - Velocity) * mult;
        }

        public Vector3 Wander(float maxSteer, float maxVel, float mult)
        {
            if (Random.Shared.NextDouble() < 0.02)
            {
                _targetRandomizer = new Vector3((float)Random.Shared.NextDouble() - 0.5f, (float)Random.Shared.NextDouble() - 0.5f, 0);
            }

            Vector3 circleCenter = Velocity.Normalized();
            if (circleCenter.LengthSquared() < 0.1f)
            {
                circleCenter = Bot.Player.RotationVector;
            }

            circleCenter *= 6.0f;
            Vector3 displacement = _targetRandomizer * 3.0f;
            return (circleCenter + displacement) * mult;
        }

        public Vector3 Evade(Vector3 pos, float steer, float vel, float mult, float rot, float tVel = 2.0f)
        {
            float x = pos.X + (MathF.Cos(rot) * tVel);
            float y = pos.Y + (MathF.Sin(rot) * tVel);
            return Flee(new Vector3(x, y, pos.Z), steer, vel, mult);
        }

        public Vector3 Unstuck(float steer, float vel, float mult)
        {
            return Arrive(BotMath.CalculatePositionBehind(Bot.Player.Position, Bot.Player.Rotation, 10f), steer, vel, 0f);
        }

        public Vector3 AvoidObstacles(float steer, float vel, float mult)
        {
            return (GetObjectForceAroundMe<IWowGameobject>(steer, vel, 3.5f)
                  + GetNearestBlacklistForce(steer, vel, 6.0f)) * mult;
        }

        public Vector3 Separate(float dist, float vel, float mult)
        {
            return GetObjectForceAroundMe<IWowPlayer>(vel, vel, dist) * mult;
        }

        private Vector3 GetNearestBlacklistForce(float steer, float vel, float dist)
        {
            if (Bot.Db.TryGetBlacklistPosition((int)Bot.Objects.MapId, Bot.Player.Position, dist, out IEnumerable<Vector3> nodes))
            {
                foreach (Vector3 node in nodes)
                {
                    return Flee(node, steer, vel, 2.0f);
                }
            }
            return Vector3.Zero;
        }

        private Vector3 GetObjectForceAroundMe<T>(float steer, float vel, float radius) where T : IWowObject
        {
            Vector3 force = Vector3.Zero;
            int count = 0;
            Vector3 myPos = Bot.Player.Position;
            float radiusSq = radius * radius;
            Vector3 forward = Velocity.Normalized();
            if (forward.LengthSquared() < 0.1f)
            {
                forward = Bot.Player.RotationVector;
            }

            foreach (IWowObject obj in Bot.Objects.All)
            {
                if (obj is T)
                {
                    float distSq = myPos.DistanceToSquared(obj.Position);
                    if (distSq < radiusSq && distSq > 0.001f)
                    {
                        Vector3 toObject = obj.Position - myPos;
                        if (forward.Dot(toObject.Normalized()) > -0.2f)
                        {
                            float dist = MathF.Sqrt(distSq);
                            Vector3 fleeDir = -toObject;
                            fleeDir /= dist;
                            float strength = (1.0f - (dist / radius)) * vel;
                            force += fleeDir * strength;
                            count++;
                        }
                    }
                }
            }
            return count > 0 ? force / count * 1.15f : Vector3.Zero;
        }
    }
}
