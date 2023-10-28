using System;

/// <summary>
/// Specifies the flags used for path requests.
/// </summary>
namespace AmeisenBotX.Core.Engines.Movement.Pathfinding.Enums
{
    /// <summary>
    /// Specifies the flags used for path requests.
    /// </summary>
    [Flags]
    public enum PathRequestFlag
    {
        /// <summary>
        /// This method calculates the sum of two integer numbers and returns the result.
        /// </summary>
        /// <param name="num1">The first integer number to be summed.</param>
        /// <param name="num2">The second integer number to be summed.</param>
        /// <returns>The sum of the two integer numbers.</returns>
        None = 0,
        /// <summary>
        /// Represents the Chaikin curve with a value of 1.
        /// </summary>
        ChaikinCurve = 1,
        /// <summary>
        /// Represents a Catmull-Rom spline with a tension value of 2.
        /// </summary>
        CatmullRomSpline = 2,
    }
}