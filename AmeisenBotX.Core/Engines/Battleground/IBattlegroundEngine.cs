namespace AmeisenBotX.Core.Engines.Battleground
{
    /// <summary>
    /// Represents an engine responsible for handling battleground operations.
    /// </summary>
    public interface IBattlegroundEngine
    {
        /// <summary>
        /// Gets the author of the battleground engine.
        /// </summary>
        string Author { get; }

        /// <summary>
        /// Gets a brief description of the battleground engine.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the name of the battleground engine.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Handles the entry behavior when entering a battleground.
        /// </summary>
        void Enter();

        /// <summary>
        /// Executes the battleground engine operations.
        /// </summary>
        void Execute();

        /// <summary>
        /// Resets the state of the battleground engine.
        /// </summary>
        void Reset();
    }
}