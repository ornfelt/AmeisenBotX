using System.Runtime.InteropServices;

namespace AmeisenBotX.Memory.Win32
{
    /// <summary>
    /// Represents a rectangle with four integer properties: Left, Top, Right, Bottom.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        /// <summary>
        /// Gets or sets the value representing the left position.
        /// </summary>
        public int Left { get; set; }

        /// <summary>
        /// Gets or sets the top value of the element.
        /// </summary>
        public int Top { get; set; }

        /// <summary>
        /// Gets or sets the value representing the right position.
        /// </summary>
        public int Right { get; set; }

        /// <summary>
        /// Gets or sets the bottom coordinate of the element.
        /// </summary>
        public int Bottom { get; set; }

        /// <summary>
        /// Determines whether two Rect objects are not equal.
        /// </summary>
        public static bool operator !=(Rect left, Rect right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Determines whether two Rect objects are equal by comparing their values.
        /// </summary>
        public static bool operator ==(Rect left, Rect right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// Overrides the base Equals method.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return obj.GetType() == typeof(Rect)
                       && ((Rect)obj).Left == Left
                       && ((Rect)obj).Top == Top
                       && ((Rect)obj).Right == Right
                       && ((Rect)obj).Bottom == Bottom;
        }

        /// <summary>
        /// Computes a hash code for the current object.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return 17 + (Left * 23) + (Top * 23) + (Right * 23) + (Bottom * 23);
            }
        }

        /// <summary>
        /// Converts the current instance of the object to a string representation.
        /// The string representation includes the left, top, right, and bottom values.
        /// </summary>
        /// <returns>A string representation of the object.</returns>
        public override string ToString()
        {
            return $"Left: {Left} Top: {Top} Right: {Right} Bottom: {Bottom}";
        }
    }
}