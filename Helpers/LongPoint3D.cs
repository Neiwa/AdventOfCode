using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Helpers
{
    [DebuggerDisplay("({X}, {Y}, {Z})")]
    public readonly struct LongPoint3D
    {
        public LongPoint3D(long x, long y, long z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public LongPoint3D(Point point, long z)
        {
            X = point.X;
            Y = point.Y;
            Z = z;
        }

        public long X { get; }
        public long Y { get; }
        public long Z { get; }

        public static LongPoint3D operator +(LongPoint3D left, LongPoint3D right)
        {
            return new LongPoint3D(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        public static LongPoint3D operator -(LongPoint3D left, LongPoint3D right)
        {
            return new LongPoint3D(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        public static bool operator ==(LongPoint3D left, LongPoint3D right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LongPoint3D left, LongPoint3D right)
        {
            return !left.Equals(right);
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (null != obj && obj is LongPoint3D other)
            {
                return X == other.X && Y == other.Y && Z == other.Z;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (int)(107 * Z + 31 * X + Y);
        }
    }
}
