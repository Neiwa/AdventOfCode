using Helpers.Grid;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2024.Day14
{
    public record Robot(Point Position, Point Velocity);
    public record Puzzle(List<Robot> Robots, Point Bounds);

    public class Day14 : BaseAoc<Puzzle>
    {
        public override object PartOne(Puzzle input)
        {
            var robotPoints = input.Robots
                .Select(r => r.Position + r.Velocity * 100)
                .Select(p => new Point(
                    p.X > 0 ? p.X % input.Bounds.X : (input.Bounds.X + (p.X % input.Bounds.X)) % input.Bounds.X,
                    p.Y > 0 ? p.Y % input.Bounds.Y : (input.Bounds.Y + (p.Y % input.Bounds.Y)) % input.Bounds.Y
                    ))
                .ToList();

            long q1 = 0, q2 = 0, q3 = 0, q4 = 0;

            Point middle = new Point(input.Bounds.X/2, input.Bounds.Y/2);

            foreach (var robotPoint in robotPoints)
            {
                if (robotPoint.X < middle.X && robotPoint.Y < middle.Y)
                {
                    q1++;
                }
                else if (robotPoint.X < middle.X && robotPoint.Y > middle.Y)
                {
                    q2++;
                }
                else if (robotPoint.X > middle.X && robotPoint.Y < middle.Y)
                {
                    q3++;
                }
                else if (robotPoint.X > middle.X && robotPoint.Y > middle.Y)
                {
                    q4++;
                }
            }

            Draw(robotPoints, input.Bounds, middle);


            return q1 * q2 * q3 * q4;
        }

        void Draw(List<Point> points, Point bounds, Point? middle = null)
        {
            if (!IsDebug) return;

            var grid = new FixedGrid<char>(bounds.X, bounds.Y, '.');
            foreach (var g in points.GroupBy(p => p))
            {
                grid.SetAt(g.Key, (char)('0' + g.Count()));
            }

            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    var value = grid.ValueAt(x, y);
                    if (middle is not null && value == '.' && (x == middle.X || y == middle.Y))
                    {
                        Write(' ');
                    }
                    else
                    {
                        Write(value);
                    }
                }
                WriteLine();
            }
            WriteLine();
        }

        void Draw2(IEnumerable<List<Point>> ppoints, Point bounds)
        {
            if (!IsDebug) return;

            var grids = ppoints.Select(points =>
            {
                var grid = new FixedGrid<char>(bounds.X, bounds.Y, '.');
                foreach (var g in points.GroupBy(p => p))
                {
                    grid.SetAt(g.Key, (char)('0' + g.Count()));
                }

                return grid;
            }).ToList();

            for (int y = 0; y < bounds.Y; y++)
            {
                for (int g = 0; g < grids.Count; g++)
                {
                    var grid = grids[g];
                    if (g > 0)
                    {
                        Write(' ');
                    }

                    for (int x = 0; x < grid.Width; x++)
                    {
                        var value = grid.ValueAt(x, y);
                        Write(value);
                    }
                }

                WriteLine();
            }
            WriteLine();
        }

        public override object PartTwo(Puzzle input)
        {
            long second = 0;
            long minDistanceToMiddle = long.MaxValue;

            Point middle = new Point(input.Bounds.X / 2, input.Bounds.Y / 2);
            while (true)
            {
                var robotPoints = input.Robots
                    .Select(r => r.Position + r.Velocity * second)
                    .Select(p => new Point(
                        p.X > 0 ? p.X % input.Bounds.X : (input.Bounds.X + (p.X % input.Bounds.X)) % input.Bounds.X,
                        p.Y > 0 ? p.Y % input.Bounds.Y : (input.Bounds.Y + (p.Y % input.Bounds.Y)) % input.Bounds.Y
                        ))
                    .ToList();

                var dist = robotPoints.Aggregate(0L, (acc, p) => acc + (middle - p).ManhattanLength);
                if (dist < minDistanceToMiddle)
                {
                    minDistanceToMiddle = dist;

                    WriteLine($"Frame {second}");
                    Draw2(robotPoints.AsEnumerable(), input.Bounds);
                    var key = Console.ReadKey();
                    if (key.Key == ConsoleKey.Escape)
                        break;
                }

                second++;
            }

            return second;
        }

        protected override Puzzle ParseInput(List<string> lines)
        {
            var bounds = this.FileName.Contains("example") ? new Point(11, 7) : new Point(101, 103);

            var robots = lines.Select(line =>Regex.Match(line, @"p=(?<px>-?\d+),(?<py>-?\d+)\s+v=(?<vx>-?\d+),(?<vy>-?\d+)"))
                .Where(m => m.Success)
                .Select(m => new Robot(m.GetPoint("px", "py"), m.GetPoint("vx", "vy")));

            return new Puzzle(robots.ToList(), bounds);
        }
    }
}
