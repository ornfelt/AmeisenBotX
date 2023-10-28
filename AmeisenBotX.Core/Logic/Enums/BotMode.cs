/// <summary>
/// Represents the available modes for a bot.
/// </summary>
namespace AmeisenBotX.Core.Logic.Enums
{
    /// <summary>
    /// Represents the available modes for a bot.
    /// </summary>
    public enum BotMode
    {
        /// <summary>
        /// This method calculates the sum of two integers and returns the result.
        /// </summary>
        /// <param name="num1">The first integer.</param>
        /// <param name="num2">The second integer.</param>
        /// <returns>The sum of num1 and num2.</returns>
        None,
        /// <summary>
        /// This code represents a grinding operation.
        /// </summary>
        Grinding,
        /// <summary>
        /// Represents a class that manages jobs.
        /// </summary>
        Jobs,
        /// <summary>
        /// Represents a Player vs Player (PvP) match in a game.
        /// </summary>
        PvP,
        /// <summary>
        /// This method represents a questing action. It takes in a character name and 
        /// a quest name as parameters to initiate the quest for the given character.
        /// </summary>
        Questing,
        /// <summary>
        /// This is a test method used for testing purposes.
        /// </summary>
        Testing
    }
}