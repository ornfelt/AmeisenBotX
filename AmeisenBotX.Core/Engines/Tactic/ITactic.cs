using AmeisenBotX.Common.Math;
using AmeisenBotX.Wow.Objects.Enums;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Tactic
{
    /// <summary>
    /// Represents a tactic for a specific area in the game world.
    /// </summary>
    public interface ITactic
    {
        /// <summary>
        /// Gets the area.
        /// </summary>
        Vector3 Area { get; }

        /// <summary>
        /// Gets the area of a circle with a given radius.
        /// </summary>
        float AreaRadius { get; }

        /// <summary>
        /// Gets the dictionary of configurables.
        /// </summary>
        /// <remarks>
        /// The configurables dictionary is used to store key-value pairs where the keys are strings
        /// and the values can be of any type.
        /// </remarks>
        /// <returns>
        /// The dictionary of configurables.
        /// </returns>
        Dictionary<string, dynamic> Configurables { get; }

        /// <summary>
        /// Gets the value of the MapId property.
        /// </summary>
        WowMapId MapId { get; }

        /// <summary>
        /// Executes the tactic for the specified role, determining if the role is melee or ranged.
        /// </summary>
        /// <param name="role">The role of the character.</param>
        /// <param name="isMelee">Specifies if the character is a melee fighter.</param>
        /// <param name="handlesMovement">Outputs a value indicating if the tactic handles movement.</param>
        /// <param name="allowAttacking">Outputs a value indicating if the tactic allows attacking.</param>
        /// <returns>A boolean value indicating whether the tactic was successfully executed.</returns>
        bool ExecuteTactic(WowRole role, bool isMelee, out bool handlesMovement, out bool allowAttacking);

        /// <summary>
        /// Determines whether the given position is within the specified area.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <returns>True if the position is within the area; otherwise, false.</returns>
        bool IsInArea(Vector3 position)
        {
            return position.GetDistance(Area) < AreaRadius;
        }
    }
}