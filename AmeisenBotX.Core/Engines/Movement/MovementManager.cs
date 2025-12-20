using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Movement.Enums;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Movement
{
    public class MovementManager(IEnumerable<IMovementProvider> providers)
    {
        public IEnumerable<IMovementProvider> Providers { get; set; } = providers;

        public float Rotation { get; private set; }

        public Vector3 Target { get; private set; }

        public MovementAction Type { get; private set; }

        public bool NeedToMove()
        {
            foreach (IMovementProvider provider in Providers)
            {
                if (provider.Get(out Vector3 position, out MovementAction type, out float rotation))
                {
                    Target = position;
                    Type = type;
                    Rotation = rotation;
                    return true;
                }
            }

            Type = MovementAction.None;
            Target = Vector3.Zero;
            Rotation = 0f;
            return false;
        }
    }
}