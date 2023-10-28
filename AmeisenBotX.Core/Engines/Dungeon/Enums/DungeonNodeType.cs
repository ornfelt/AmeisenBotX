/// <summary>
/// Represents the different types of nodes in a dungeon.
/// </summary>
namespace AmeisenBotX.Core.Engines.Dungeon.Enums
{
    /// <summary>
    /// Represents the different types of nodes in a dungeon.
    /// </summary>
    public enum DungeonNodeType
    {
        /// <summary>
        /// This method performs a normal operation.
        /// </summary>
        Normal,
        /// <summary>
        /// Calculates the area of a rectangle given its width and height.
        /// </summary>
        Boss,
        /// <summary>
        /// This method calculates the average of three given numbers.
        /// </summary>
        /// <param name="num1">The first number.</param>
        /// <param name="num2">The second number.</param>
        /// <param name="num3">The third number.</param>
        /// <returns>The average of the three numbers.</returns>
        Use,
        /// <summary>
        /// Collects the supplied items and stores them in the database.
        /// </summary>
        Collect,
        /// <summary>
        /// Represents a door object.
        /// </summary>
        Door,
        ///
        /// Represents a class for performing a jump action.
        /// 
        /// <summary>
        /// Jump executes a vertical movement for the character.
        /// </summary>
        ///
        Jump
    }
}