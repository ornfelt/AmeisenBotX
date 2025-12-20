using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AmeisenBotX.Common.Math
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3 : IEquatable<Vector3>
    {
        public static readonly Vector3 Zero = new(0, 0, 0);
        public static readonly Vector3 One = new(1, 1, 1);
        public static readonly Vector3 UnitX = new(1, 0, 0);
        public static readonly Vector3 UnitY = new(0, 1, 0);
        public static readonly Vector3 UnitZ = new(0, 0, 1);

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public Vector3(float value)
        {
            X = value; Y = value; Z = value;
        }

        public Vector3(float x, float y, float z)
        {
            X = x; Y = y; Z = z;
        }

        public Vector3(Vector3 v)
        {
            X = v.X; Y = v.Y; Z = v.Z;
        }

        public Vector3(float[] values)
        {
            if (values == null || values.Length < 3) throw new ArgumentException("Array must have at least 3 elements");
            X = values[0]; Y = values[1]; Z = values[2];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator +(Vector3 a, Vector3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator +(Vector3 a, float b) => new(a.X + b, a.Y + b, a.Z + b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator -(Vector3 a, Vector3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator -(Vector3 a) => new(-a.X, -a.Y, -a.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator -(Vector3 a, float b) => new(a.X - b, a.Y - b, a.Z - b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator *(Vector3 a, Vector3 b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator *(Vector3 a, float b) => new(a.X * b, a.Y * b, a.Z * b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator *(float b, Vector3 a) => new(a.X * b, a.Y * b, a.Z * b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator /(Vector3 a, Vector3 b) => new(a.X / b.X, a.Y / b.Y, a.Z / b.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 operator /(Vector3 a, float b) => new(a.X / b, a.Y / b, a.Z / b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector3 a, Vector3 b) => (a.X == b.X) && (a.Y == b.Y) && (a.Z == b.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector3 a, Vector3 b) => !(a == b);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float Dot(Vector3 vector) => (X * vector.X) + (Y * vector.Y) + (Z * vector.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            return new Vector3(
                (a.Y * b.Z) - (a.Z * b.Y),
                (a.Z * b.X) - (a.X * b.Z),
                (a.X * b.Y) - (a.Y * b.X));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float Length() => MathF.Sqrt((X * X) + (Y * Y) + (Z * Z));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float LengthSquared() => (X * X) + (Y * Y) + (Z * Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float Length2D() => MathF.Sqrt((X * X) + (Y * Y));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float DistanceTo(Vector3 other)
        {
            float dx = X - other.X, dy = Y - other.Y, dz = Z - other.Z;
            return MathF.Sqrt((dx * dx) + (dy * dy) + (dz * dz));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float DistanceTo2D(Vector3 other)
        {
            float dx = X - other.X, dy = Y - other.Y;
            return MathF.Sqrt((dx * dx) + (dy * dy));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            if (t < 0f) t = 0f;
            else if (t > 1f) t = 1f;
            return new Vector3(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t, a.Z + (b.Z - a.Z) * t);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Normalize()
        {
            float lenSq = (X * X) + (Y * Y) + (Z * Z);
            if (lenSq > 1e-10f)
            {
                float lenInv = 1.0f / MathF.Sqrt(lenSq);
                X *= lenInv; Y *= lenInv; Z *= lenInv;
            }
            else { X = Y = Z = 0; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 Normalized()
        {
            Vector3 v = this; v.Normalize(); return v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Normalize2D()
        {
            float lenSq = (X * X) + (Y * Y);
            if (lenSq > 1e-10f)
            {
                float lenInv = 1.0f / MathF.Sqrt(lenSq);
                X *= lenInv; Y *= lenInv;
            }
            else { X = Y = 0; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Truncate(float max)
        {
            if (LengthSquared() > max * max)
            {
                Normalize();
                Multiply(max);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 Truncated(float max)
        {
            if (LengthSquared() > max * max) return Normalized() * max;
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Rotate(float degrees)
        {
            float radians = degrees * (MathF.PI / 180f);
            float ca = MathF.Cos(radians);
            float sa = MathF.Sin(radians);
            float oldX = X;
            X = (ca * oldX) - (sa * Y);
            Y = (sa * oldX) + (ca * Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 ZeroZ() => new(X, Y, 0f);

        public readonly bool AnySubZero() => X < 0f || Y < 0f || Z < 0f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(Vector3 v) { X += v.X; Y += v.Y; Z += v.Z; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Multiply(float n) { X *= n; Y *= n; Z *= n; }

        public readonly float GetDistance(Vector3 v) => DistanceTo(v);
        public readonly float GetDistance2D(Vector3 v) => DistanceTo2D(v);
        public readonly float GetMagnitude() => Length();
        public readonly float GetMagnitude2D() => Length2D();

        public override readonly bool Equals(object obj) => obj is Vector3 v && Equals(v);
        public readonly bool Equals(Vector3 other) => this == other;

        public override readonly int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                hash = hash * 23 + Z.GetHashCode();
                return hash;
            }
        }

        public override readonly string ToString() => $"<{X:F2}, {Y:F2}, {Z:F2}>";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float DistanceToSquared(Vector3 other)
        {
            float dx = X - other.X;
            float dy = Y - other.Y;
            float dz = Z - other.Z;
            return (dx * dx) + (dy * dy) + (dz * dz);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float DistanceTo2DSquared(Vector3 other)
        {
            float dx = X - other.X;
            float dy = Y - other.Y;
            return (dx * dx) + (dy * dy);
        }
    }
}