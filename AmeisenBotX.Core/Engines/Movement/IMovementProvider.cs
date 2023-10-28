using AmeisenBotX.Common.Math;
using AmeisenBotX.Core.Engines.Movement.Enums;

/// <summary>
/// Contains classes and interfaces related to the movement engine of the AmeisenBotX.
/// </summary>
namespace AmeisenBotX.Core.Engines.Movement
{
    /// <summary>
    /// Interface for a movement provider that retrieves the current position and movement action.
    /// </summary>
    /// <param name="position">The current position in the world</param>
    /// <param name="type">The type of movement action being performed</param>
    /// <returns>A boolean value indicating if the retrieval was successful</returns>
    public interface IMovementProvider
    {
        /// <summary>
        /// Retrieves the position and movement action type.
        /// </summary>
        bool Get(out Vector3 position, out MovementAction type);
    }
}