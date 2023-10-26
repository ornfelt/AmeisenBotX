using System;

namespace AmeisenBotX.Core.Logic
{
    /// <summary>
    /// Represents the logic for an AmeisenBot interacting with World of Warcraft.
    /// </summary>
    public interface IAmeisenBotLogic
    {
        /// <summary>
        /// Represents an event that is raised when the World of Warcraft game is started.
        /// </summary>
        event Action OnWoWStarted;

        /// <summary>
        /// Executes a single tick or iteration of the program.
        /// </summary>
        void Tick();
    }
}