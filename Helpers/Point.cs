namespace Helpers
{
    public class Point
    {
        public Point(long x, long y)
        {
            X = x;
            Y = y;
        }

        public long X { get; set; }
        public long Y { get; set; }

        public long ManhattanLength => Math.Abs(X) + Math.Abs(Y);


        public static implicit operator Point(IntPoint p) => new Point(p.X, p.Y);
        public static Point operator +(Point left, Point right)
        {
            return new Point(left.X + right.X, left.Y + right.Y);
        }
        public static Point operator +(Point left, IntPoint right)
        {
            return new Point(left.X + right.X, left.Y + right.Y);
        }
        public static Point operator +(IntPoint left, Point right)
        {
            return new Point(left.X + right.X, left.Y + right.Y);
        }
        public static Point operator -(Point left, Point right)
        {
            return new Point(left.X - right.X, left.Y - right.Y);
        }
        public static bool operator ==(Point left, Point right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Point left, Point right)
        {
            return !left.Equals(right);
        }

        public static Point operator *(Point left, long right)
        {
            return new Point(left.X * right, left.Y * right);
        }

        public static Point operator /(Point left, long right)
        {
            return new Point(left.X / right, left.Y / right);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (null != obj && obj is Point other)
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
            return $"Point ({X}, {Y})";
        }
    }
}
