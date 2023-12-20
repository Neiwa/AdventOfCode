namespace Helpers
{
    public class IntPoint
    {
        public IntPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public static IntPoint operator +(IntPoint left, IntPoint right)
        {
            return new IntPoint(left.X + right.X, left.Y + right.Y);
        }
        public static IntPoint operator -(IntPoint left, IntPoint right)
        {
            return new IntPoint(left.X - right.X, left.Y - right.Y);
        }
        public static bool operator ==(IntPoint left, IntPoint right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(IntPoint left, IntPoint right)
        {
            return !left.Equals(right);
        }

        public static IntPoint operator *(IntPoint left, int right)
        {
            return new IntPoint(left.X * right, left.Y * right);
        }

        public static IntPoint operator /(IntPoint left, int right)
        {
            return new IntPoint(left.X / right, left.Y / right);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (null != obj && obj is IntPoint other)
            {
                return X == other.X && Y == other.Y;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return 31 * X + Y;
        }

        public override string ToString()
        {
            return $"IntPoint ({X}, {Y})";
        }
    }
}
