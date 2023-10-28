using AmeisenBotX.Core.Engines.Grinding.Profiles;

/// <summary>
/// Represents an interface for a grinding engine.
/// </summary>
namespace AmeisenBotX.Core.Engines.Grinding
{
    /// <summary>
    /// Represents an interface for a grinding engine.
    /// </summary>
    public interface IGrindingEngine
    {
        /// <summary>
        /// Gets or sets the grinding profile for the current instance.
        /// </summary>
        IGrindingProfile Profile { get; set; }

        /// <summary>
        /// Executes the code.
        /// </summary>
        void Execute();

        ///<summary>
        ///Loads the specified grinding profile.
        ///</summary>
        void LoadProfile(IGrindingProfile profile);
    }
}