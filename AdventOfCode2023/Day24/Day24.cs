using Core;
using Helpers;
using System.Text.RegularExpressions;

namespace AdventOfCode2023.Day24
{
    public class Day24 : BaseAocV2
    {
        public record Hailstone(LongPoint3D Pos, (double X, double Y, double Z) Vector);

        public static Hailstone Parse(string line)
        {
            var m = Regex.Match(line, @"(?<px>-?\d+),\s+(?<py>-?\d+),\s+(?<pz>-?\d+)\s+@\s+(?<vx>-?\d+),\s+(?<vy>-?\d+),\s+(?<vz>-?\d+)");
            var pos = new LongPoint3D(long.Parse(m.Groups["px"].Value), long.Parse(m.Groups["py"].Value), long.Parse(m.Groups["pz"].Value));
            var vector = (double.Parse(m.Groups["vx"].Value), double.Parse(m.Groups["vy"].Value), double.Parse(m.Groups["vz"].Value));

            return new Hailstone(pos, vector);
        }

        public Point GetPoint(LongPoint3D point3d)
        {
            return new Point(point3d.X, point3d.Y);
        }

        public bool Intersect(Hailstone left, Hailstone right, long lowBound, long highBound)
        {
            // https://stackoverflow.com/a/1968345
            var rightMult = (-left.Vector.Y * (left.Pos.X - right.Pos.X) + left.Vector.X * (left.Pos.Y - right.Pos.Y)) / (-right.Vector.X * left.Vector.Y + left.Vector.X * right.Vector.Y);
            var leftMult = (right.Vector.X * (left.Pos.Y - right.Pos.Y) - right.Vector.Y * (left.Pos.X - right.Pos.X)) / (-right.Vector.X * left.Vector.Y + left.Vector.X * right.Vector.Y);
            if (leftMult <= 0 || rightMult <= 0)
            {
                return false;
            }

            var intersectX = left.Pos.X + left.Vector.X * leftMult;
            var intersectY = left.Pos.Y + left.Vector.Y * leftMult;

            if (lowBound <= intersectX && intersectX <= highBound && lowBound <= intersectY && intersectY <= highBound)
            {
                return true;
            }

            return false;
        }

        public override object PartOne(List<string> lines)
        {
            var hailstones = lines.Select(Parse).ToList();

            var lowBound = FileName.Contains("example") ? 7 : 200000000000000;
            var highBound = FileName.Contains("example") ? 27 : 400000000000000;

            var res = hailstones.SelectMany((h, index) => hailstones.Skip(index), (l, r) => (l, r)).Count((p) => Intersect(p.l, p.r, lowBound, highBound));

            return res;
        }

        public override object PartTwo(List<string> lines)
        {
            throw new NotImplementedException();
        }
    }
}
