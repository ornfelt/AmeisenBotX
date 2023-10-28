/// <summary>
/// Namespace containing common mathematical operations and structures.
/// </summary>
namespace AmeisenBotX.Common.Math
{
    /// <summary>
    /// Represents a 3x3 matrix, primarily used for 3D transformations and operations.
    /// </summary>
    public struct Matrix3x3
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix3x3"/> struct using the specified elements.
        /// </summary>
        /// <param name="x1">The element at row 1, column 1.</param>
        /// <param name="x2">The element at row 2, column 1.</param>
        /// <param name="x3">The element at row 3, column 1.</param>
        /// <param name="y1">The element at row 1, column 2.</param>
        /// <param name="y2">The element at row 2, column 2.</param>
        /// <param name="y3">The element at row 3, column 2.</param>
        /// <param name="z1">The element at row 1, column 3.</param>
        /// <param name="z2">The element at row 2, column 3.</param>
        /// <param name="z3">The element at row 3, column 3.</param>
        public Matrix3x3(float x1, float x2, float x3, float y1, float y2, float y3, float z1, float z2, float z3)
        {
            X1 = x1;
            X2 = x2;
            X3 = x3;
            Y1 = y1;
            Y2 = y2;
            Y3 = y3;
            Z1 = z1;
            Z2 = z2;
            Z3 = z3;
        }

        /// <summary>
        /// Gets the first column of the matrix as a vector.
        /// </summary>
        public Vector3 FirstCol => new(X1, Y1, Z1);

        /// <summary>
        /// Gets or sets the X1 value.
        /// </summary>
        public float X1 { get; set; }

        /// <summary>
        /// Gets or sets the value of X2, which represents a float number.
        /// </summary>
        public float X2 { get; set; }

        /// <summary>
        /// Gets or sets the X3 value.
        /// </summary>
        public float X3 { get; set; }

        /// <summary>
        /// Gets or sets the Y1 coordinate.
        /// </summary>
        public float Y1 { get; set; }

        /// <summary>
        /// Gets or sets the Y2 coordinate.
        /// </summary>
        public float Y2 { get; set; }

        /// <summary>
        /// Gets or sets the Y3 value.
        /// </summary>
        public float Y3 { get; set; }

        /// <summary>
        /// Gets or sets the value of Z1.
        /// </summary>
        public float Z1 { get; set; }

        /// <summary>
        /// Gets or sets the Z2 value.
        /// </summary>
        public float Z2 { get; set; }

        /// <summary>
        /// Gets or sets the value of Z3.
        /// </summary>
        public float Z3 { get; set; }

        /// <summary>
        /// Multiplies a vector by a 3x3 matrix.
        /// </summary>
        /// <param name="v">The vector to be multiplied.</param>
        /// <param name="m">The matrix to multiply by.</param>
        /// <returns>The resulting vector after the multiplication.</returns>
        public static Vector3 operator *(Vector3 v, Matrix3x3 m)
        {
            return new Vector3(m.X1 * v.X + m.Y1 * v.Y + m.Z1 * v.Z,
                               m.X2 * v.X + m.Y2 * v.Y + m.Z2 * v.Z,
                               m.X3 * v.X + m.Y3 * v.Y + m.Z3 * v.Z);
        }

        /// <summary>
        /// Calculates the determinant (or dot product) of this 3x3 matrix.
        /// </summary>
        /// <returns>The determinant value of the matrix.</returns>
        public float Dot()
        {
            return (X1 * Y2 * Z3) + (X2 * Y3 * Z1) + (X3 * Y1 * Z2)
                 - (X3 * Y2 * Z1) - (X2 * Y1 * Z3) - (X1 * Y3 * Z2);
        }

        /// <summary>
        /// Calculates the inverse of this 3x3 matrix.
        /// </summary>
        /// <returns>The inverse matrix.</returns>
        /// <remarks>The method calculates the inverse based on the determinant of the matrix. It may throw an exception if the determinant is zero, indicating the matrix is non-invertible.</remarks>
        public Matrix3x3 Inverse()
        {
            float d = 1 / Dot();
            return new(d * (Y2 * Z3 - Y3 * Z2), d * (X3 * Z2 - X2 * Z3), d * (X2 * Y3 - X3 * Y2),
                       d * (Y3 * Z1 - Y1 * Z3), d * (X1 * Z3 - X3 * Z1), d * (X3 * Y1 - X1 * Y3),
                       d * (Y1 * Z2 - Y2 * Z1), d * (X2 * Z1 - X1 * Z2), d * (X1 * Y2 - X2 * Y1));
        }
    }
}