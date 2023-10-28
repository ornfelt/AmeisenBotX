/// <summary>
/// Represents an interface for a tactic engine.
/// </summary>
namespace AmeisenBotX.Core.Engines.Tactic
{
    /// <summary>
    /// Represents an interface for a tactic engine.
    /// </summary>
    public interface ITacticEngine
    {
        /// <summary>
        /// Gets or sets a value indicating whether attacking is allowed.
        /// </summary>
        bool AllowAttacking { get; }

        /// <summary>
        /// Gets or sets a value indicating whether movement is prevented.
        /// </summary>
        bool PreventMovement { get; }

        /// <summary>
        /// Executes the specified action and returns a boolean value indicating the success of the execution.
        /// </summary>
        bool Execute();
    }
}