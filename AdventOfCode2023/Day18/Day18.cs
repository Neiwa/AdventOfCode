using Core;
using Helpers;
using System.Text.RegularExpressions;

namespace AdventOfCode2023.Day18
{
    public class Day18 : BaseAocV2
    {
        public record Instruction(Point Direction, int Steps, string Color);

        public Instruction Parse(string line)
        {
            var m = Regex.Match(line, @"(?<dir>.) (?<steps>\d+) \(#(?<color>[\da-f]+)\)");
            var direction = m.Groups["dir"].Value switch
            {
                "U" => new Point(0, -1),
                "R" => new Point(1, 0),
                "D" => new Point(0, 1),
                "L" => new Point(-1, 0),
                _ => throw new NotImplementedException(),
            };
            var steps = int.Parse(m.Groups["steps"].Value);

            return new Instruction(direction, steps, m.Groups["color"].Value);
        }

        public Instruction ParsePartTwo(string line)
        {
            var m = Regex.Match(line, @". \d+ \(#(?<steps>[\da-f]{5})(?<dir>.)\)");
            var direction = m.Groups["dir"].Value switch
            {
                "3" => new Point(0, -1),
                "0" => new Point(1, 0),
                "1" => new Point(0, 1),
                "2" => new Point(-1, 0),
                _ => throw new NotImplementedException(),
            };
            var steps = Convert.ToInt32(m.Groups["steps"].Value, 16);

            return new Instruction(direction, steps, "");
        }

        public void Draw(Grid grid)
        {
            if (!IsTrace) return;

            for (int y = 0; y < grid.Height; y++)
            {
                for (global::System.Int32 x = 0; x < grid.Width; x++)
                {
                    Write(grid.ValueAt(x, y));
                }
                WriteLine();
            }
        }

        public override string PartOne(List<string> lines)
        {
            var instructions = lines.Select(Parse);
            var sparseGrid = new HashSet<Point>();
            var currentPoint = new Point(0, 0);
            sparseGrid.Add(currentPoint);

            var topLeft = new Point(0, 0);
            var bottomRight = new Point(0, 0);

            foreach (var instruction in instructions)
            {
                for (var i = 0; i < instruction.Steps; i++)
                {
                    currentPoint += instruction.Direction;
                    sparseGrid.Add(currentPoint);
                }

                if (instruction.Direction.X > 0 && bottomRight.X < currentPoint.X)
                {
                    bottomRight.X = currentPoint.X;
                }
                else if (instruction.Direction.X < 0 && topLeft.X > currentPoint.X)
                {
                    topLeft.X = currentPoint.X;
                }
                else if (instruction.Direction.Y < 0 && topLeft.Y > currentPoint.Y)
                {
                    topLeft.Y = currentPoint.Y;
                }
                else if (instruction.Direction.Y > 0 && bottomRight.Y < currentPoint.Y)
                {
                    bottomRight.Y = currentPoint.Y;
                }
            }

            var bounds = new Rectangle(topLeft, bottomRight);

            var shift = new Point(1, 1) - topLeft;

            var map = new Grid(bounds.Width + 2, bounds.Height + 2, '.');
            foreach (var point in sparseGrid)
            {
                map.At(point + shift).Value = '#';
            }

            Draw(map);

            var floodStack = new Stack<GridCellReference<char>>();
            floodStack.Push(map.At(0, 0));
            while (floodStack.Any())
            {
                var fill = floodStack.Pop();
                fill.Value = 'O';
                foreach (var neighbour in fill.GetNeighbours().Where(n => n.Value == '.'))
                {
                    floodStack.Push(neighbour);
                }
            }

            Draw(map);

            return map.Count(c => c.Value == '#' || c.Value == '.').ToString();
        }

        public override string PartTwo(List<string> lines)
        {
            var instructions = lines.Select(ParsePartTwo);
            var sparseGrid = new HashSet<Point>();
            var currentPoint = new Point(0, 0);
            sparseGrid.Add(currentPoint);

            var topLeft = new Point(0, 0);
            var bottomRight = new Point(0, 0);

            foreach (var instruction in instructions)
            {
                currentPoint += instruction.Direction * instruction.Steps;
                sparseGrid.Add(currentPoint);

                if (instruction.Direction.X > 0 && bottomRight.X < currentPoint.X)
                {
                    bottomRight.X = currentPoint.X;
                }
                else if (instruction.Direction.X < 0 && topLeft.X > currentPoint.X)
                {
                    topLeft.X = currentPoint.X;
                }
                else if (instruction.Direction.Y < 0 && topLeft.Y > currentPoint.Y)
                {
                    topLeft.Y = currentPoint.Y;
                }
                else if (instruction.Direction.Y > 0 && bottomRight.Y < currentPoint.Y)
                {
                    bottomRight.Y = currentPoint.Y;
                }
            }

            var bounds = new Rectangle(topLeft, bottomRight);

            throw new NotImplementedException();
        }
    }
}
