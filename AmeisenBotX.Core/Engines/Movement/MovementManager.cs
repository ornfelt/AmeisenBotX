using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Movement.Enums;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Movement
{
    public class MovementManager
    {
        /// <summary>
        /// Initializes a new instance of the MovementManager class with the specified collection of movement providers.
        /// </summary>
        public MovementManager(IEnumerable<IMovementProvider> providers)
        {
            Providers = providers;
        }

        /// <summary>
        /// Gets or sets the collection of movement providers.
        /// </summary>
        /// <returns>A collection of objects implementing the IMovementProvider interface.</returns>
        public IEnumerable<IMovementProvider> Providers { get; set; }

        /// <summary>
        /// Gets or sets the target Vector3.
        /// </summary>
        public Vector3 Target { get; private set; }

        /// <summary>
        /// Gets or sets the Type of movement action.
        /// </summary>
        public MovementAction Type { get; private set; }

        /// <summary>
        /// Checks if any movement provider needs to move and sets the movement target and type accordingly.
        /// </summary>
        /// <returns>True if a movement provider needs to move, otherwise false.</returns>
        public bool NeedToMove()
        {
            foreach (IMovementProvider provider in Providers)
            {
                if (provider.Get(out Vector3 position, out MovementAction type))
                {
                    Target = position;
                    Type = type;
                    return true;
                }
            }

            Type = MovementAction.None;
            Target = Vector3.Zero;
            return false;
        }
    }
}