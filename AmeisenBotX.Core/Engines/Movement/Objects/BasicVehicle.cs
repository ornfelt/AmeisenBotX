using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Movement.Enums;
using AmeisenBotX.Wow.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotX.Core.Engines.Movement.Objects
{
    /// <summary>
    /// Represents a basic vehicle.
    /// </summary>
    public class BasicVehicle
    {
        /// <summary>
        /// Initializes a new instance of the BasicVehicle class.
        /// </summary>
        /// <param name="bot">The AmeisenBotInterfaces object that represents the bot.</param>
        public BasicVehicle(AmeisenBotInterfaces bot)
        {
            Bot = bot;
        }

        /// <summary>
        /// Delegate used to move a character to a specified position.
        /// </summary>
        /// <param name="positionToGoTo">The position the character should move to.</param>
        public delegate void MoveCharacter(Vector3 positionToGoTo);

        /// <summary>
        /// Gets or sets the last update date and time.
        /// </summary>
        public DateTime LastUpdate { get; private set; }

        /// <summary>
        /// Gets or sets the velocity of the object in 3-dimensional space.
        /// </summary>
        public Vector3 Velocity { get; private set; }

        /// <summary>
        /// Gets or sets the instance of the AmeisenBotInterfaces.
        /// </summary>
        private AmeisenBotInterfaces Bot { get; }

        /// <summary>
        /// This method calculates the acceleration needed to avoid obstacles. 
        /// It combines the forces exerted by objects around the game object and 
        /// the nearest blacklisted force. The resulting acceleration is 
        /// truncated to the specified maximum steering value and multiplied 
        /// by the given multiplier. The calculated acceleration is then returned.
        /// </summary>
        public Vector3 AvoidObstacles(float maxSteering, float maxVelocity, float multiplier)
        {
            Vector3 acceleration = GetObjectForceAroundMe<IWowGameobject>(maxSteering, maxVelocity)
                                 + GetNearestBlacklistForce(maxSteering, maxVelocity, 12.0f);

            return acceleration.Truncated(maxSteering) * multiplier;
        }

        /// <summary>
        /// Evade method used to calculate the position ahead and return the result of the Flee method.
        /// </summary>
        /// <param name="position">The current position of the object.</param>
        /// <param name="maxSteering">The maximum amount the object can steer.</param>
        /// <param name="maxVelocity">The maximum velocity of the object.</param>
        /// <param name="multiplier">The multiplier used in calculating the desired velocity.</param>
        /// <param name="targetRotation">The target rotation of the object.</param>
        /// <param name="targetVelocity">The target velocity of the object.</param>
        /// <returns>The result of the Flee method using the position ahead.</returns>
        public Vector3 Evade(Vector3 position, float maxSteering, float maxVelocity, float multiplier, float targetRotation, float targetVelocity = 2.0f)
        {
            Vector3 positionAhead = CalculateFuturePosition(position, targetRotation, targetVelocity);
            return Flee(positionAhead, maxSteering, maxVelocity, multiplier);
        }

        /// <summary>
        /// Calculates the acceleration vector for fleeing from a given position.
        /// </summary>
        /// <param name="position">The position to flee from.</param>
        /// <param name="maxSteering">The maximum steering value.</param>
        /// <param name="maxVelocity">The maximum velocity value.</param>
        /// <param name="multiplier">The multiplier applied to the acceleration.</param>
        /// <returns>The acceleration vector for fleeing.</returns>
        public Vector3 Flee(Vector3 position, float maxSteering, float maxVelocity, float multiplier)
        {
            Vector3 currentPosition = Bot.Player.Position;
            Vector3 desired = currentPosition;
            float distanceToTarget = currentPosition.GetDistance(position);

            desired -= position;
            desired.Normalize2D(desired.GetMagnitude2D());

            if (Bot.Player.IsMounted)
            {
                maxVelocity *= 2;
            }

            desired.Multiply(maxVelocity);

            if (distanceToTarget > 20)
            {
                float slowdownMultiplier = 20 / distanceToTarget;

                if (slowdownMultiplier < 0.1)
                {
                    slowdownMultiplier = 0;
                }

                desired.Multiply(slowdownMultiplier);
            }

            Vector3 steering = desired;
            steering -= Velocity;

            if (Bot.Player.IsInCombat)
            {
                float maxSteeringCombat = maxSteering;

                if (Bot.Player.IsMounted)
                {
                    maxSteeringCombat *= 2;
                }

                steering.Truncate(maxSteeringCombat);
            }
            else
            {
                if (Bot.Player.IsMounted)
                {
                    maxSteering *= 2;
                }

                steering.Truncate(maxSteering);
            }

            Vector3 acceleration = new();
            acceleration += steering;

            if (Bot.Player.IsInCombat)
            {
                if (Bot.Player.IsMounted)
                {
                    maxVelocity *= 2;
                }

                acceleration.Truncate(maxVelocity);
            }
            else
            {
                if (Bot.Player.IsMounted)
                {
                    maxVelocity *= 2;
                }

                acceleration.Truncate(maxVelocity);
            }

            acceleration.Multiply(multiplier);
            return acceleration;
        }

        /// <summary>
        /// Calculates the pursuit behavior for an object in 3D space.
        /// </summary>
        /// <param name="position">The current position of the object.</param>
        /// <param name="maxSteering">The maximum steering value for the object.</param>
        /// <param name="maxVelocity">The maximum velocity value for the object.</param>
        /// <param name="multiplier">A multiplier value for the pursuit behavior.</param>
        /// <param name="targetRotation">The rotation of the target object.</param>
        /// <param name="targetVelocity">The velocity of the target object (default value is 2.0f).</param>
        /// <returns>The resulting Vector3 representing the pursuit behavior.</returns>
        public Vector3 Pursuit(Vector3 position, float maxSteering, float maxVelocity, float multiplier, float targetRotation, float targetVelocity = 2.0f)
        {
            Vector3 positionAhead = CalculateFuturePosition(position, targetRotation, targetVelocity);
            return Seek(positionAhead, maxSteering, maxVelocity, multiplier);
        }

        /// <summary>
        /// Seeks a target position with a given maximum steering, maximum velocity, and multiplier.
        /// </summary>
        /// <param name="position">The position to seek.</param>
        /// <param name="maxSteering">The maximum steering force.</param>
        /// <param name="maxVelocity">The maximum velocity.</param>
        /// <param name="multiplier">The multiplier for the resulting vector.</param>
        /// <returns>The resulting vector after seeking the target position.</returns>
        public Vector3 Seek(Vector3 position, float maxSteering, float maxVelocity, float multiplier)
        {
            Vector3 desiredVelocity = (position - Bot.Player.Position).Normalized() * maxVelocity;

            const float slowRad = 3.5f;
            float distance = Bot.Player.DistanceTo(position);

            if (distance < slowRad)
            {
                desiredVelocity *= distance / slowRad;
            }

            return (desiredVelocity - Velocity).Truncated(maxSteering) * multiplier;
        }

        /// <summary>
        /// Calculates and returns the force vector to separate objects within a certain distance.
        /// </summary>
        /// <param name="seperationDistance">The distance within which objects are considered for separation.</param>
        /// <param name="maxVelocity">The maximum velocity limit for the objects.</param>
        /// <param name="multiplier">The multiplier applied to the calculated force.</param>
        /// <returns>The force vector for separating objects.</returns>
        public Vector3 Seperate(float seperationDistance, float maxVelocity, float multiplier)
        {
            return GetObjectForceAroundMe<IWowPlayer>(seperationDistance, maxVelocity) * multiplier;
        }

        /// <summary>
        /// Unstucks the bot by calculating a position behind the player and then 
        /// seeks towards that position using the specified maximum steering, maximum 
        /// velocity, and multiplier values.
        /// </summary>
        /// <param name="maxSteering">The maximum steering value for the bot.</param>
        /// <param name="maxVelocity">The maximum velocity value for the bot.</param>
        /// <param name="multiplier">The multiplier value to adjust the steering force.</param>
        /// <returns>The resulting vector representing the movement towards the calculated position.</returns>
        public Vector3 Unstuck(float maxSteering, float maxVelocity, float multiplier)
        {
            Vector3 positionBehindMe = BotMath.CalculatePositionBehind(Bot.Player.Position, Bot.Player.Rotation, 8.0f);
            return Seek(positionBehindMe, maxSteering, maxVelocity, multiplier);
        }

        /// <summary>
        /// Updates the character's movement based on the specified parameters.
        /// If the movement action is DirectMove, the character is moved directly to the target position.
        /// Otherwise, the character's velocity is adjusted based on the force calculated from the movement action, target position,
        /// rotation, maximum steering, maximum velocity, and separation distance.
        /// The character's position is updated accordingly.
        /// </summary>
        /// <param name="moveCharacter">The delegate to invoke to move the character.</param>
        /// <param name="movementAction">The type of movement action to perform.</param>
        /// <param name="targetPosition">The target position for movement.</param>
        /// <param name="rotation">The rotation of the character.</param>
        /// <param name="maxSteering">The maximum steering value for the character's movement.</param>
        /// <param name="maxVelocity">The maximum velocity for the character's movement.</param>
        /// <param name="seperationDistance">The separation distance for avoiding collisions.</param>
        public void Update(MoveCharacter moveCharacter, MovementAction movementAction, Vector3 targetPosition, float rotation, float maxSteering, float maxVelocity, float seperationDistance)
        {
            if (movementAction == MovementAction.DirectMove)
            {
                moveCharacter?.Invoke(targetPosition);
                return;
            }

            // adjust max steering based on time passed since last Update() call
            float timedelta = (float)(DateTime.UtcNow - LastUpdate).TotalSeconds;
            float maxSteeringNormalized = maxSteering * timedelta;

            Vector3 totalforce = GetForce(movementAction, targetPosition, rotation, maxSteeringNormalized, maxVelocity, seperationDistance);
            Velocity += totalforce;
            Velocity.Truncate(maxVelocity);

            moveCharacter?.Invoke(Bot.Player.Position + Velocity);
            LastUpdate = DateTime.UtcNow;
        }

        /// <summary>
        /// Generates a random position for the target to wander within a given radius.
        /// </summary>
        /// <param name="multiplier">The multiplier for the steering force.</param>
        /// <param name="maxSteering">The maximum steering force.</param>
        /// <param name="maxVelocity">The maximum velocity.</param>
        /// <returns>A Vector3 representing the desired position for the target to seek towards.</returns>
        public Vector3 Wander(float multiplier, float maxSteering, float maxVelocity)
        {
            // TODO: implement some sort of radius where the target wanders around. maybe add a very
            // weak force keeping it inside a given circle...
            // TODO: implement some sort of delay so that the target is not constantly walking
            Random rnd = new();
            Vector3 currentPosition = Bot.Player.Position;

            Vector3 newRandomPosition = new();
            newRandomPosition += CalculateFuturePosition(currentPosition, Bot.Player.Rotation, ((float)rnd.NextDouble() * 4.0f) + 4.0f);

            // rotate the vector by random amount of degrees
            newRandomPosition.Rotate(rnd.Next(-14, 14));

            return Seek(newRandomPosition, maxSteering, maxVelocity, multiplier);
        }

        /// <summary>
        /// Calculates the future position based on the current position, target rotation, and target velocity.
        /// </summary>
        /// <param name="position">The current position.</param>
        /// <param name="targetRotation">The target rotation.</param>
        /// <param name="targetVelocity">The target velocity.</param>
        /// <returns>The calculated future position in a Vector3 format.</returns>
        private static Vector3 CalculateFuturePosition(Vector3 position, float targetRotation, float targetVelocity)
        {
            float rotation = targetRotation;
            float x = position.X + (MathF.Cos(rotation) * targetVelocity);
            float y = position.Y + (MathF.Sin(rotation) * targetVelocity);

            return new()
            {
                X = x,
                Y = y,
                Z = position.Z
            };
        }

        /// <summary>
        /// Returns the force applied to an object based on the specified movement action, target position, rotation, maximum steering,
        /// maximum velocity, and separation distance. 
        /// </summary>
        /// <param name="movementAction">The type of movement action to perform.</param>
        /// <param name="targetPosition">The target position to seek or flee from.</param>
        /// <param name="rotation">The rotation of the object.</param>
        /// <param name="maxSteering">The maximum steering force that can be applied.</param>
        /// <param name="maxVelocity">The maximum velocity that can be achieved.</param>
        /// <param name="seperationDistance">The distance at which objects should separate.</param>
        /// <returns>The calculated force based on the specified parameters.</returns>
        private Vector3 GetForce(MovementAction movementAction, Vector3 targetPosition, float rotation, float maxSteering, float maxVelocity, float seperationDistance)
        {
            return movementAction switch
            {
                MovementAction.Move => Seek(targetPosition, maxSteering, maxVelocity, 0.9f)
                                     + AvoidObstacles(maxSteering, maxVelocity, 0.05f)
                                     + Seperate(seperationDistance, maxVelocity, 0.05f),

                MovementAction.Follow => Seek(targetPosition, maxSteering, maxVelocity, 0.9f)
                                       + AvoidObstacles(maxSteering, maxVelocity, 0.05f)
                                       + Seperate(seperationDistance, maxVelocity, 0.05f),

                MovementAction.Chase => Seek(targetPosition, maxSteering, maxVelocity, 1.0f),
                MovementAction.Flee => Flee(targetPosition, maxSteering, maxVelocity, 1.0f).ZeroZ(),
                MovementAction.Evade => Evade(targetPosition, maxSteering, maxVelocity, 1.0f, rotation),
                MovementAction.Wander => Wander(maxSteering, maxVelocity, 1.0f).ZeroZ(),
                MovementAction.Unstuck => Unstuck(maxSteering, maxVelocity, 1.0f),

                _ => Vector3.Zero,
            };
        }

        /// <summary>
        /// Returns the force needed to avoid blacklisted positions within a certain distance.
        /// </summary>
        /// <param name="maxSteering">The maximum steering force.</param>
        /// <param name="maxVelocity">The maximum velocity.</param>
        /// <param name="maxDistance">The maximum distance within which to consider blacklisted positions. Defaults to 8.0f.</param>
        /// <returns>The force needed to avoid blacklisted positions.</returns>
        private Vector3 GetNearestBlacklistForce(float maxSteering, float maxVelocity, float maxDistance = 8.0f)
        {
            Vector3 force = new();

            if (Bot.Db.TryGetBlacklistPosition((int)Bot.Objects.MapId, Bot.Player.Position, maxDistance, out IEnumerable<Vector3> nodes))
            {
                force += Flee(nodes.First(), 0.5f, maxSteering, maxVelocity);
            }

            return force;
        }

        ///<summary>
        ///Calculates the force exerted on the object by other objects within a specified distance.
        ///</summary>
        ///<typeparam name="T">The type of objects to consider for force calculation.</typeparam>
        ///<param name="maxSteering">The maximum steering force to apply in response to nearby objects.</param>
        ///<param name="maxVelocity">The maximum velocity to apply in response to nearby objects.</param>
        ///<param name="maxDistance">The maximum distance within which objects are considered for force calculation. Defaults to 3.0f.</param>
        ///<returns>The cumulative force exerted on the object.</returns>
        private Vector3 GetObjectForceAroundMe<T>(float maxSteering, float maxVelocity, float maxDistance = 3.0f) where T : IWowObject
        {
            int count = 0;
            Vector3 force = new();
            Vector3 vehiclePosition = Bot.Player.Position;
            List<(Vector3, float)> objectDistances = new();

            // we need to know every objects position and distance to later apply a force pushing us
            // back from it that is relational to the objects distance.

            foreach (T obj in Bot.Objects.All.OfType<T>())
            {
                float distance = obj.Position.GetDistance(vehiclePosition);

                if (distance < maxDistance)
                {
                    objectDistances.Add((obj.Position, distance));
                }
            }

            if (objectDistances.Count == 0)
            {
                return Vector3.Zero;
            }

            // get the biggest distance to normalize the fleeing forces
            float normalizingMultiplier = objectDistances.Max(e => e.Item2);

            for (int i = 0; i < objectDistances.Count; ++i)
            {
                force += Flee(objectDistances[i].Item1, objectDistances[i].Item2 * normalizingMultiplier, maxSteering, maxVelocity);
                count++;
            }

            // return the average force
            return force / count;
        }
    }
}