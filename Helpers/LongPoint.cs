namespace Helpers
{
    public class LongPoint
    {
        public LongPoint(long x, long y)
        {
            X = x;
            Y = y;
        }

        public long X { get; set; }
        public long Y { get; set; }


        public static implicit operator LongPoint(Point p) => new LongPoint(p.X, p.Y);
        public static LongPoint operator +(LongPoint left, LongPoint right)
        {
            return new LongPoint(left.X + right.X, left.Y + right.Y);
        }
        public static LongPoint operator +(LongPoint left, Point right)
        {
            return new LongPoint(left.X + right.X, left.Y + right.Y);
        }
        public static LongPoint operator +(Point left, LongPoint right)
        {
            return new LongPoint(left.X + right.X, left.Y + right.Y);
        }
        public static LongPoint operator -(LongPoint left, LongPoint right)
        {
            return new LongPoint(left.X - right.X, left.Y - right.Y);
        }
        public static bool operator ==(LongPoint left, LongPoint right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(LongPoint left, LongPoint right)
        {
            return !left.Equals(right);
        }

        public static LongPoint operator *(LongPoint left, long right)
        {
            return new LongPoint(left.X * right, left.Y * right);
        }

        public static LongPoint operator /(LongPoint left, long right)
        {
            return new LongPoint(left.X / right, left.Y / right);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (null != obj && obj is LongPoint other)
            {
                return X == other.X && Y == other.Y;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (int)((31 * X + Y) % int.MaxValue);
        }

        public override string ToString()
        {
            return $"LongPoint ({X}, {Y})";
        }
    }
}
