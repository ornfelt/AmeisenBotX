using System.Runtime.InteropServices;

/// <summary>
/// Represents the Win32 memory namespace for AmeisenBotX.
/// </summary>
namespace AmeisenBotX.Memory.Win32
{
    /// <summary>
    /// Represents the margins of a rectangular area.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Margins
    {
        /// <summary>
        /// Gets or sets the value of the Left property.
        /// </summary>
        public int Left { get; set; }

        /// <summary>
        /// Gets or sets the right side value.
        /// </summary>
        public int Right { get; set; }

        /// <summary>
        /// Gets or sets the top value.
        /// </summary>
        public int Top { get; set; }

        /// <summary>
        /// Gets or sets the bottom value.
        /// </summary>
        public int Bottom { get; set; }

        /// <summary>
        /// Determines whether the specified object is equal to the current Margins object by comparing their Left, Right, Top, and Bottom values.
        /// </summary>
        /// <param name="obj">The object to compare with the current Margins object.</param>
        /// <returns>true if the specified object is equal to the current Margins object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return obj != null
                && obj.GetType() == typeof(Margins)
                && ((Margins)obj).Left == Left
                && ((Margins)obj).Right == Right
                && ((Margins)obj).Top == Top
                && ((Margins)obj).Bottom == Bottom;
        }

        /// <summary>
        /// Overrides the GetHashCode method to provide a unique hash code based on the values of the Left, Right, Top, and Bottom properties.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                return Left * 23 + Right * 23 + Top * 23 + Bottom * 23;
            }
        }

        /// <summary>
        /// Determines whether two instances of Margins are equal.
        /// </summary>
        public static bool operator ==(Margins left, Margins right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two Margins objects are not equal.
        /// </summary>
        /// <param name="left">The first Margins object to compare.</param>
        /// <param name="right">The second Margins object to compare.</param>
        /// <returns>true if the Margins objects are not equal; otherwise, false.</returns>
        public static bool operator !=(Margins left, Margins right)
        {
            return !(left == right);
        }
    }
}