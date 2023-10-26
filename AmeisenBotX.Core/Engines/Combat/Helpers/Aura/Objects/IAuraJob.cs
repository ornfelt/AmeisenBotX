using AmeisenBotX.Wow.Objects;
using System.Collections.Generic;

namespace AmeisenBotX.Core.Engines.Combat.Helpers.Aura.Objects
{
    /// <summary>
    /// Represents an interface for an Aura Job.
    /// </summary>
    /// <param name="auras">The collection of Wow Auras.</param>
    /// <returns>True if the job is successfully executed, otherwise false.</returns>
    public interface IAuraJob
    {
        /// <summary>
        /// Executes a logic for running a collection of World of Warcraft auras.
        /// </summary>
        /// <param name="auras">The collection of auras to process.</param>
        /// <returns>Returns a boolean value indicating the success of the operation.</returns>
        bool Run(IEnumerable<IWowAura> auras);
    }
}