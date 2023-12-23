using Core;
using Helpers;
using Helpers.Extensions;
using Helpers.Grid;
using System.Text.RegularExpressions;

namespace AdventOfCode2023.Day22
{
    public class Day22 : BaseAocV2
    {
        public void Draw(Grid<(long Height, int Brick)> grid)
        {
            if (!IsTrace) return;

            Point start = new Point(0, 0);
            Point end = this.FileName.Contains("example") ? new Point(2, 2) : new Point(9, 9);

            char[] bricks = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();
            string[] colors = new[] { "white", "maroon", "green", "olive", "navy", "purple", "teal", "silver", "grey", "red", "lime", "yellow", "blue", "fuchsia", "aqua" };

            for (int y = 0; y <= end.Y - start.Y; y++)
            {
                for (global::System.Int32 x = 0; x <= end.X - start.X; x++)
                {
                    int brick = grid.ValueAt(x, y).Brick;
                    base.Write($"[{colors[(brick / bricks.Length) % colors.Length]}]{(brick == -1 ? ' ' : bricks[brick % bricks.Length])}[/]");
                }
                Write(" | ");
                for (global::System.Int32 x = 0; x <= end.X - start.X; x++)
                {
                    var (height, brick) = grid.ValueAt(x, y);
                    Write($"[{colors[(brick / bricks.Length) % colors.Length]}]{height}[/] ");
                }
                WriteLine();
            }
            WriteLine();
        }

        public override string PartOne(List<string> lines)
        {
            var bricks = lines.Select(l =>
            {
                var matches = Regex.Matches(l, @"(?<x>\d+),(?<y>\d+),(?<z>\d+)");

                return new Cube(
                    new LongPoint3D(
                        long.Parse(matches[0].Groups["x"].Value),
                        long.Parse(matches[0].Groups["y"].Value),
                        long.Parse(matches[0].Groups["z"].Value)),
                    new LongPoint3D(
                        long.Parse(matches[1].Groups["x"].Value),
                        long.Parse(matches[1].Groups["y"].Value),
                        long.Parse(matches[1].Groups["z"].Value)));
            }).OrderBy(c => Math.Min(c.FirstCorner.Z, c.SecondCorner.Z)).ToList();

            var restingOn = new ValueCreationDictionary<int, List<int>>();
            var supporting = new ValueCreationDictionary<int, List<int>>();

            var topDownGrid = new SparseGrid<(long Height, int Brick)>((0, -1));

            for (var i = 0; i < bricks.Count; i++)
            {
                var brick = bricks[i];

                var bottomPoints = new List<LongPoint3D>();
                if (brick.FirstCorner.Z == brick.SecondCorner.Z)
                {
                    // Lying down
                    Point shift;
                    if (brick.FirstCorner.X == brick.SecondCorner.X)
                    {
                        if (brick.FirstCorner.Y > brick.SecondCorner.Y)
                        {
                            shift = new Point(0, -1);
                        }
                        else
                        {
                            shift = new Point(0, 1);
                        }
                    }
                    else
                    {
                        if (brick.FirstCorner.X > brick.SecondCorner.X)
                        {
                            shift = new Point(-1, 0);
                        }
                        else
                        {
                            shift = new Point(1, 0);
                        }
                    }

                    var curr = new Point(brick.FirstCorner.X, brick.FirstCorner.Y);
                    var target = new Point(brick.SecondCorner.X, brick.SecondCorner.Y);
                    bottomPoints.Add(new LongPoint3D(curr, brick.FirstCorner.Z));
                    while (curr != target)
                    {
                        curr += shift;
                        bottomPoints.Add(new LongPoint3D(curr, brick.FirstCorner.Z));
                    }
                }
                else
                {
                    // Standing up
                    bottomPoints.Add(new LongPoint3D(brick.FirstCorner.X, brick.FirstCorner.Y, Math.Min(brick.FirstCorner.Z, brick.SecondCorner.Z)));
                    bottomPoints.Add(new LongPoint3D(brick.FirstCorner.X, brick.FirstCorner.Y, Math.Max(brick.FirstCorner.Z, brick.SecondCorner.Z)));
                }

                var highestPoint = bottomPoints.MaxBy(p => topDownGrid.ValueAt(p.X, p.Y).Height);
                var height = topDownGrid.ValueAt(highestPoint.X, highestPoint.Y).Height;
                var diff = new LongPoint3D(0, 0, highestPoint.Z - height - 1);

                bottomPoints = bottomPoints.Select(p => p - diff).ToList();

                var bricksBelow = bottomPoints.Select(p => topDownGrid.ValueAt(p.X, p.Y)).Where(v => v.Height == bottomPoints[0].Z - 1).Select(v => v.Brick).Distinct();

                restingOn[i].AddRange(bricksBelow);
                foreach (var brickBelow in bricksBelow)
                {
                    supporting[brickBelow].Add(i);
                }

                foreach (var bottomPoint in bottomPoints)
                {
                    topDownGrid.At(bottomPoint.X, bottomPoint.Y).Value = (bottomPoint.Z, i);
                }

                Draw(topDownGrid);
            }

            return Enumerable.Range(0, bricks.Count).Count(brick => supporting[brick].All(supported => restingOn[supported].Count > 1)).ToString();
        }

        public override string PartTwo(List<string> lines)
        {
            var bricks = lines.Select(l =>
            {
                var matches = Regex.Matches(l, @"(?<x>\d+),(?<y>\d+),(?<z>\d+)");

                return new Cube(
                    new LongPoint3D(
                        long.Parse(matches[0].Groups["x"].Value),
                        long.Parse(matches[0].Groups["y"].Value),
                        long.Parse(matches[0].Groups["z"].Value)),
                    new LongPoint3D(
                        long.Parse(matches[1].Groups["x"].Value),
                        long.Parse(matches[1].Groups["y"].Value),
                        long.Parse(matches[1].Groups["z"].Value)));
            }).OrderBy(c => Math.Min(c.FirstCorner.Z, c.SecondCorner.Z)).ToList();

            var restingOn = new ValueCreationDictionary<int, List<int>>();
            var supporting = new ValueCreationDictionary<int, List<int>>();

            var topDownGrid = new SparseGrid<(long Height, int Brick)>((0, -1));

            for (var i = 0; i < bricks.Count; i++)
            {
                var brick = bricks[i];

                var bottomPoints = new List<LongPoint3D>();
                if (brick.FirstCorner.Z == brick.SecondCorner.Z)
                {
                    // Lying down
                    Point shift;
                    if (brick.FirstCorner.X == brick.SecondCorner.X)
                    {
                        if (brick.FirstCorner.Y > brick.SecondCorner.Y)
                        {
                            shift = new Point(0, -1);
                        }
                        else
                        {
                            shift = new Point(0, 1);
                        }
                    }
                    else
                    {
                        if (brick.FirstCorner.X > brick.SecondCorner.X)
                        {
                            shift = new Point(-1, 0);
                        }
                        else
                        {
                            shift = new Point(1, 0);
                        }
                    }

                    var curr = new Point(brick.FirstCorner.X, brick.FirstCorner.Y);
                    var target = new Point(brick.SecondCorner.X, brick.SecondCorner.Y);
                    bottomPoints.Add(new LongPoint3D(curr, brick.FirstCorner.Z));
                    while (curr != target)
                    {
                        curr += shift;
                        bottomPoints.Add(new LongPoint3D(curr, brick.FirstCorner.Z));
                    }
                }
                else
                {
                    // Standing up
                    bottomPoints.Add(new LongPoint3D(brick.FirstCorner.X, brick.FirstCorner.Y, Math.Min(brick.FirstCorner.Z, brick.SecondCorner.Z)));
                    bottomPoints.Add(new LongPoint3D(brick.FirstCorner.X, brick.FirstCorner.Y, Math.Max(brick.FirstCorner.Z, brick.SecondCorner.Z)));
                }

                var highestPoint = bottomPoints.MaxBy(p => topDownGrid.ValueAt(p.X, p.Y).Height);
                var height = topDownGrid.ValueAt(highestPoint.X, highestPoint.Y).Height;
                var diff = new LongPoint3D(0, 0, highestPoint.Z - height - 1);

                bottomPoints = bottomPoints.Select(p => p - diff).ToList();

                var bricksBelow = bottomPoints.Select(p => topDownGrid.ValueAt(p.X, p.Y)).Where(v => v.Height == bottomPoints[0].Z - 1).Select(v => v.Brick).Distinct();

                restingOn[i].AddRange(bricksBelow);
                foreach (var brickBelow in bricksBelow)
                {
                    supporting[brickBelow].Add(i);
                }

                foreach (var bottomPoint in bottomPoints)
                {
                    topDownGrid.At(bottomPoint.X, bottomPoint.Y).Value = (bottomPoint.Z, i);
                }

                Draw(topDownGrid);
            }

            var supportSum = new Dictionary<int, int>();
            var chain = new ValueCreationDictionary<int, int>();

            int calcSupportValue(int i)
            {
                HashSet<int> removed = new() { i };

                var resting = new Queue<int>();

                var currentBrick = i;
                var supportValue = 0;

                var freeFloating = supporting[i].Where(brick => !restingOn[brick].Except(removed).Any());

                resting.Enqueue(freeFloating);

                while (resting.TryDequeue(out var brick))
                {
                    if (removed.Contains(brick))
                    {
                        continue;
                    }

                    removed.Add(brick);
                    supportValue++;
                    freeFloating = supporting[brick].Where(brick => !restingOn[brick].Except(removed).Any());
                    resting.Enqueue(freeFloating);
                }

                return supportValue;
            }

            for (int i = bricks.Count - 1; i >= 0; i--)
            {
                chain[i] = calcSupportValue(i);
            }

            return chain.Values.Sum().ToString();
        }
    }
}
