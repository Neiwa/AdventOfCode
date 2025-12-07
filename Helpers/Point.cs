using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Helpers
{
    [DebuggerDisplay("({X}, {Y})")]
    public class Point(long x, long y)
    {
        public long X { get; set; } = x;
        public long Y { get; set; } = y;

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
            return HashCode.Combine(X, Y);
        }

        public override string ToString()
        {
            return $"Point ({X}, {Y})";
        }

        public static Point Parse(string input, char sep = ',')
        {
            if (TryParse(input, out var point, sep))
            {
                return point;
            }

            throw new ArgumentException("Invalid input string");
        }

        public static bool TryParse(string input, [NotNullWhen(true)] out Point? point, char sep = ',')
        {
            var regex = $"(?<x>-?\\d+)\\s*{sep}\\s*(?<y>-?\\d+)";
            var match = Regex.Match(input, regex);
            if (match.Success)
            {
                point = match.GetPoint("x", "y");
                return true;
            }
            point = null;
            return false;
        }
    }

    public static class PointExtensions
    {
        public static Point GetPoint(this Match match, string xName = "x", string yName = "y")
        {
            return new Point(long.Parse(match.Groups[xName].Value), long.Parse(match.Groups[yName].Value));
        }
    }
}
