namespace AmeisenBotX.Core.Engines.Battleground.Jannis.Profiles
{
    /// <summary>
    /// Represents a profile for a bot in a battleground.
    /// </summary>
    public interface IBattlegroundProfile
    {
        /// <summary>
        /// Gets the CTF (Capture The Flag) blackboard associated with the profile.
        /// </summary>
        CtfBlackboard JBgBlackboard { get; }

        /// <summary>
        /// Executes the profile, performing actions associated with the battleground.
        /// </summary>
        void Execute();
    }
}