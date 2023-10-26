using AmeisenBotX.Core;

namespace AmeisenBotX.StateConfig
{
    /// <summary>
    /// Represents an interface for a state configuration window.
    /// </summary>
    /// <remarks>
    /// This interface provides properties to access the configuration and save status of the window.
    /// </remarks>
    public interface IStateConfigWindow
    {
        /// <summary>
        /// Gets the AmeisenBotConfig object.
        /// </summary>
        AmeisenBotConfig Config { get; }

        /// <summary>
        /// Gets a value indicating whether the state should be saved.
        /// </summary>
        bool ShouldSave { get; }
    }
}