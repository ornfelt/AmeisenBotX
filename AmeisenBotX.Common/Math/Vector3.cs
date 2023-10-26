using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AmeisenBotX.Common.Math
{
    /// <summary>
    /// Represents a three-dimensional vector.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3 : IEquatable<Vector3>
    {
        /// <summary>
        /// Gets a Vector3 representing the zero vector.
        /// </summary>
        public static Vector3 Zero { get; } = new(0, 0, 0);

        /// <summary>
        /// Initializes a new instance of the Vector3 struct using a single value for X, Y, and Z components.
        /// </summary>
        /// <param name="a">The value to initialize X, Y, and Z components with.</param>
        public Vector3(float a)
        {
            X = a;
            Y = a;
            Z = a;
        }

        /// <summary>
        /// Initializes a new instance of the Vector3 struct using separate values for X, Y, and Z components.
        /// </summary>
        /// <param name="x">The X component value.</param>
        /// <param name="y">The Y component value.</param>
        /// <param name="z">The Z component value.</param>
        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Checks if any component of the vector is less than zero.
        /// </summary>
        /// <returns>True if any component is less than zero, false otherwise.</returns>
        public bool AnySubZero()
        {
            return X < 0.0f || Y < 0.0f || Z < 0.0f;
        }

        /// <summary>
        /// Copy constructor: Initializes a new instance of the Vector3 structure using the values from another Vector3 instance.
        /// </summary>
        /// <param name="position">The source Vector3 instance to copy values from.</param>
        public Vector3(Vector3 position) : this(position.X, position.Y, position.Z)
        {
        }

        /// <summary>
        /// Gets or sets the X coordinate.
        /// </summary>
        public float X { get; set; }

        ///<summary>
        /// Gets or sets the Y coordinate.
        ///</summary>
        public float Y { get; set; }

        /// <summary>
        /// Gets or sets the value of Z, a float variable.
        /// </summary>
        public float Z { get; set; }

        /// <summary>
        /// Creates a new Vector3 instance from an array of floats.
        /// </summary>
        /// <param name="array">Array containing the X, Y, and Z values for the vector. The array should have at least 3 elements.</param>
        /// <returns>A new Vector3 instance using values from the given array.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 FromArray(float[] array)
        {
            return new(array[0], array[1], array[2]);
        }

        /// <summary>
        /// Subtraction operator: Subtracts the values of two Vector3 instances.
        /// </summary>
        /// <param name="a">The left-hand side Vector3 instance.</param>
        /// <param name="b">The right-hand side Vector3 instance.</param>
        /// <returns>A new Vector3 instance with values resulting from the subtraction.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        /// <summary>
        /// Subtraction operator: Subtracts a scalar value from a Vector3 instance.
        /// </summary>
        /// <param name="a">The Vector3 instance.</param>
        /// <param name="b">The scalar value to subtract from the vector's components.</param>
        /// <returns>A new Vector3 instance with values resulting from the subtraction.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator -(Vector3 a, float b)
        {
            return new(a.X - b, a.Y - b, a.Z - b);
        }

        /// <summary>
        /// Inequality operator: Checks if two Vector3 instances are not equal.
        /// </summary>
        /// <param name="a">The left-hand side Vector3 instance.</param>
        /// <param name="b">The right-hand side Vector3 instance.</param>
        /// <returns>True if the two vectors are not equal, otherwise false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return a.X != b.X
                && a.Y != b.Y
                && a.Z != b.Z;
        }

        /// <summary>
        /// Multiplication operator: Multiplies a Vector3 instance by a scalar value.
        /// </summary>
        /// <param name="a">The Vector3 instance.</param>
        /// <param name="b">The scalar value to multiply with the vector's components.</param>
        /// <returns>A new Vector3 instance with values resulting from the multiplication.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator *(Vector3 a, float b)
        {
            return new(a.X * b, a.Y * b, a.Z * b);
        }

        /// <summary>
        /// Multiplication operator: Multiplies the components of two Vector3 instances.
        /// </summary>
        /// <param name="a">The left-hand side Vector3 instance.</param>
        /// <param name="b">The right-hand side Vector3 instance.</param>
        /// <returns>A new Vector3 instance with values resulting from the component-wise multiplication.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator *(Vector3 a, Vector3 b)
        {
            return new(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        /// <summary>
        /// Division operator: Divides the components of a Vector3 instance by the components of another.
        /// </summary>
        /// <param name="a">The dividend Vector3 instance.</param>
        /// <param name="b">The divisor Vector3 instance.</param>
        /// <returns>A new Vector3 instance with values resulting from the component-wise division.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator /(Vector3 a, Vector3 b)
        {
            return new(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }

        /// <summary>
        /// Division operator: Divides the components of a Vector3 instance by a scalar value.
        /// </summary>
        /// <param name="a">The Vector3 instance.</param>
        /// <param name="b">The scalar divisor.</param>
        /// <returns>A new Vector3 instance with values resulting from the division.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator /(Vector3 a, float b)
        {
            return new(a.X / b, a.Y / b, a.Z / b);
        }

        /// <summary>
        /// Addition operator: Adds a scalar value to the components of a Vector3 instance.
        /// </summary>
        /// <param name="a">The Vector3 instance.</param>
        /// <param name="b">The scalar value to add.</param>
        /// <returns>A new Vector3 instance with values resulting from the addition.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator +(Vector3 a, float b)
        {
            return new(a.X + b, a.Y + b, a.Z + b);
        }

        /// <summary>
        /// Addition operator: Adds the components of two Vector3 instances.
        /// </summary>
        /// <param name="a">The left-hand side Vector3 instance.</param>
        /// <param name="b">The right-hand side Vector3 instance.</param>
        /// <returns>A new Vector3 instance with values resulting from the component-wise addition.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        /// <summary>
        /// Less-than operator: Checks if all components of one Vector3 instance are less than the components of another.
        /// </summary>
        /// <param name="a">The left-hand side Vector3 instance.</param>
        /// <param name="b">The right-hand side Vector3 instance.</param>
        /// <returns>True if all components of 'a' are less than the components of 'b', otherwise false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(Vector3 a, Vector3 b)
        {
            return a.X < b.X
                && a.Y < b.Y
                && a.Z < b.Z;
        }

        /// <summary>
        /// Equality operator: Checks if the components of two Vector3 instances are equal.
        /// </summary>
        /// <param name="a">The left-hand side Vector3 instance.</param>
        /// <param name="b">The right-hand side Vector3 instance.</param>
        /// <returns>True if the two vectors are equal, otherwise false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return a.X == b.X
                && a.Y == b.Y
                && a.Z == b.Z;
        }

        /// <summary>
        /// Greater-than operator: Checks if all components of one Vector3 instance are greater than the components of another.
        /// </summary>
        /// <param name="a">The left-hand side Vector3 instance.</param>
        /// <param name="b">The right-hand side Vector3 instance.</param>
        /// <returns>True if all components of 'a' are greater than the components of 'b', otherwise false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(Vector3 a, Vector3 b)
        {
            return a.X > b.X
                && a.Y > b.Y
                && a.Z > b.Z;
        }

        /// <summary>
        /// Adds the components of the provided vector to this vector's components.
        /// </summary>
        /// <param name="vector">The vector to add.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(Vector3 vector)
        {
            X += vector.X;
            Y += vector.Y;
            Z += vector.Z;
        }

        /// <summary>
        /// Adds a scalar value to each component of this vector.
        /// </summary>
        /// <param name="n">The scalar value to add.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(float n)
        {
            X += n;
            Y += n;
            Z += n;
        }

        /// <summary>
        /// Divides this vector's components by the provided vector's components.
        /// If a component of the provided vector is 0, the corresponding component of this vector will be set to 0.
        /// </summary>
        /// <param name="v">The vector to divide by.</param
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Divide(Vector3 v)
        {
            X = v.X > 0f ? X / v.X : 0f;
            Y = v.Y > 0f ? Y / v.Y : 0f;
            Z = v.Z > 0f ? Z / v.Z : 0f;
        }

        /// <summary>
        /// Truncates this vector to the specified maximum value and returns the modified vector.
        /// </summary>
        /// <param name="max">The maximum value for truncating the vector.</param>
        /// <returns>The truncated vector.</returns>
        public Vector3 Truncated(float max)
        {
            Truncate(max);
            return this;
        }

        /// <summary>
        /// Limits this vector to the specified limit value and returns the modified vector.
        /// </summary>
        /// <param name="limit">The limit value for the vector.</param>
        /// <returns>The limited vector.</returns>
        public Vector3 Limited(float limit)
        {
            Limit(limit);
            return this;
        }

        /// <summary>
        /// Divides each component of this vector by the specified scalar.
        /// If the scalar is 0, each component of this vector will be set to 0.
        /// </summary>
        /// <param name="n">The scalar value to divide by.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Divide(float n)
        {
            X = n > 0f ? X / n : 0f;
            Y = n > 0f ? Y / n : 0f;
            Z = n > 0f ? Z / n : 0f;
        }

        /// <summary>
        /// Returns a normalized version of this vector, scaled to a length of 1.
        /// If the vector's magnitude is 0, the original vector is returned.
        /// </summary>
        /// <returns>The normalized vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 Normalized()
        {
            return Normalized(GetMagnitude());
        }

        /// <summary>
        /// Returns a normalized version of this vector using a specified magnitude.
        /// If the specified magnitude is 0, the original vector is returned.
        /// </summary>
        /// <param name="magnitude">The magnitude to use for normalization.</param>
        /// <returns>The normalized vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 Normalized(float magnitude)
        {
            if (magnitude > 0.0f)
            {
                return new
                (
                    X /= magnitude,
                    Y /= magnitude,
                    Z /= magnitude
                );
            }

            return this;
        }

        /// <summary>
        /// Sets the Z component of this vector to 0 and returns the adjusted vector.
        /// </summary>
        /// <returns>The vector with the Z component set to 0.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 ZeroZ()
        {
            return AdjustedZ(0.0f);
        }

        /// <summary>
        /// Adjusts the Z component of this vector to the specified value and returns the adjusted vector.
        /// </summary>
        /// <param name="z">The new Z value.</param>
        /// <returns>The vector with the adjusted Z component.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 AdjustedZ(float z)
        {
            Z = z;
            return this;
        }

        /// <summary>
        /// Calculates the Euclidean distance between this vector and the specified vector.
        /// </summary>
        /// <param name="v">The vector to measure the distance to.</param>
        /// <returns>The distance between the vectors.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetDistance(Vector3 v)
        {
            return MathF.Sqrt(MathF.Pow(X - v.X, 2) + MathF.Pow(Y - v.Y, 2) + MathF.Pow(Z - v.Z, 2));
        }

        /// <summary>
        /// Calculates the Euclidean distance between this vector and the specified vector in the XY plane.
        /// </summary>
        /// <param name="v">The vector to measure the distance to.</param>
        /// <returns>The 2D distance between the vectors.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetDistance2D(Vector3 v)
        {
            return MathF.Sqrt(MathF.Pow(X - v.X, 2) + MathF.Pow(Y - v.Y, 2));
        }

        /// <summary>
        /// Computes a hash code for this vector based on its components.
        /// </summary>
        /// <returns>The hash code for this vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            unchecked
            {
                return (int)(17 + (X * 23) + (Y * 23) + (Z * 23));
            }
        }

        /// <summary>
        /// Calculates the dot product of this vector with itself.
        /// </summary>
        /// <returns>The dot product.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Dot()
        {
            return MathF.Pow(X, 2) + MathF.Pow(Y, 2) + MathF.Pow(Z, 2);
        }

        /// <summary>
        /// Calculates the dot product of this vector with itself in the XY plane.
        /// </summary>
        /// <returns>The 2D dot product.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Dot2D()
        {
            return MathF.Pow(X, 2) + MathF.Pow(Y, 2);
        }

        /// <summary>
        /// Computes the magnitude (length) of this vector.
        /// </summary>
        /// <returns>The magnitude of the vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetMagnitude()
        {
            return MathF.Sqrt(Dot());
        }

        /// <summary>
        /// Computes the magnitude (length) of this vector in the XY plane.
        /// </summary>
        /// <returns>The 2D magnitude of the vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetMagnitude2D()
        {
            return MathF.Sqrt(Dot2D());
        }

        /// <summary>
        /// Limits each component of the vector to the specified value, taking into account the sign.
        /// </summary>
        /// <param name="limit">The scalar limit value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Limit(float limit)
        {
            X = X < 0f ? MathF.Max(X, limit * -1f) : MathF.Min(X, limit);
            Y = Y < 0f ? MathF.Max(Y, limit * -1f) : MathF.Min(Y, limit);
            Z = Z < 0f ? MathF.Max(Z, limit * -1f) : MathF.Min(Z, limit);
        }

        /// <summary>
        /// Scales the vector such that its magnitude does not exceed the specified maximum value.
        /// </summary>
        /// <param name="max">The maximum allowed magnitude.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Truncate(float max)
        {
            float factor = max / GetMagnitude();
            Scale(factor < 1.0f ? factor : 1.0f);
        }

        /// <summary>
        /// Multiplies each component of the vector by the specified scalar factor.
        /// </summary>
        /// <param name="factor">The scalar multiplication factor.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Scale(float factor)
        {
            X *= factor;
            Y *= factor;
            Z *= factor;
        }

        /// <summary>
        /// Multiplies this vector by another vector, component-wise.
        /// </summary>
        /// <param name="vector">The vector to multiply with.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Multiply(Vector3 vector)
        {
            X = vector.X > 0f ? X * vector.X : 0f;
            Y = vector.Y > 0f ? Y * vector.Y : 0f;
            Z = vector.Z > 0f ? Z * vector.Z : 0f;
        }

        /// <summary>
        /// Multiplies each component of the vector by the specified scalar value if the value is greater than 0.
        /// </summary>
        /// <param name="n">The scalar value to multiply by.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Multiply(float n)
        {
            X = n > 0f ? X * n : 0f;
            Y = n > 0f ? Y * n : 0f;
            Z = n > 0f ? Z * n : 0f;
        }

        /// <summary>
        /// Normalizes the vector to have a magnitude of 1.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Normalize()
        {
            Normalize(GetMagnitude());
        }

        /// <summary>
        /// Normalizes the vector using the provided magnitude.
        /// </summary>
        /// <param name="magnitude">The magnitude to normalize with.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Normalize(float magnitude)
        {
            if (magnitude > 0f)
            {
                X /= magnitude;
                Y /= magnitude;
                Z /= magnitude;
            }
        }

        /// <summary>
        /// Normalizes the vector in 2D (X and Y components) to have a magnitude of 1.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Normalize2D()
        {
            Normalize2D(GetMagnitude2D());
        }

        /// <summary>
        /// Normalizes the vector in 2D (X and Y components) using the provided magnitude.
        /// </summary>
        /// <param name="magnitude">The magnitude to normalize with in 2D.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Normalize2D(float magnitude)
        {
            if (magnitude > 0)
            {
                X /= magnitude;
                Y /= magnitude;
            }
        }

        /// <summary>
        /// Rotates the vector around the origin by the specified angle in degrees.
        /// </summary>
        /// <param name="degrees">The angle in degrees to rotate the vector by.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Rotate(float degrees)
        {
            RotateRadians(degrees * (MathF.PI / 180f));
        }

        /// <summary>
        /// Rotates the vector around the origin by the specified angle in radians.
        /// </summary>
        /// <param name="radians">The angle in radians to rotate the vector by.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RotateRadians(float radians)
        {
            float ca = MathF.Cos(radians);
            float sa = MathF.Sin(radians);

            X = ca * X - sa * Y;
            Y = sa * X + ca * Y;
        }

        /// <summary>
        /// Subtracts a scalar value from each component of the vector.
        /// </summary>
        /// <param name="n">The scalar value to subtract.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Subtract(float n)
        {
            X -= n;
            Y -= n;
            Z -= n;
        }

        /// <summary>
        /// Subtracts the specified vector from this vector.
        /// </summary>
        /// <param name="vector">The vector to subtract from this vector.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Subtract(Vector3 vector)
        {
            X -= vector.X;
            Y -= vector.Y;
            Z -= vector.Z;
        }

        /// <summary>
        /// Converts this vector to an array of floats.
        /// </summary>
        /// <returns>An array of floats representing the components of this vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float[] ToArray()
        {
            return new float[3] { X, Y, Z };
        }

        /// <summary>
        /// Converts the vector to a string representation.
        /// </summary>
        /// <returns>A string representation of the vector in the form "X, Y, Z".</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return $"{X}, {Y}, {Z}";
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current vector.
        /// </summary>
        /// <param name="obj">The object to compare with the current vector.</param>
        /// <returns>True if the specified object is equal to the current vector; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Vector3 vector)
            {
                return this == vector;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified vector is equal to the current vector.
        /// </summary>
        /// <param name="other">The vector to compare with the current vector.</param>
        /// <returns>True if the specified vector is equal to the current vector; otherwise, false.</returns>
        public bool Equals(Vector3 other)
        {
            return this == other;
        }
    }
}