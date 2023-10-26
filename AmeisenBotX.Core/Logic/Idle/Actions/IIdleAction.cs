using System;

namespace AmeisenBotX.Core.Logic.Idle.Actions
{
    /// <summary>
    /// Represents an idle action.
    /// </summary>
    public interface IIdleAction
    {
        /// <summary>
        /// Gets a boolean value indicating whether the autopilot is the only mode available.
        /// </summary>
        bool AutopilotOnly { get; }

        /// <summary>
        /// Gets or sets the DateTime value that represents the cooldown.
        /// </summary>
        DateTime Cooldown { get; set; }

        /// <summary>
        /// Gets the maximum cooldown value.
        /// </summary>
        int MaxCooldown { get; }

        /// <summary>
        /// Gets the maximum duration.
        /// </summary>
        int MaxDuration { get; }

        /// <summary>
        /// Gets the minimum cooldown value.
        /// </summary>
        int MinCooldown { get; }

        /// <summary>
        /// Gets the minimum duration.
        /// </summary>
        int MinDuration { get; }

        /// <summary>
        /// Represents a method for entering a certain task or action.
        /// </summary>
        /// <returns>True if the entering process is successful, otherwise false.</returns>
        bool Enter();

        /// <summary>
        /// Executes the code block.
        /// </summary>
        void Execute();
    }
}