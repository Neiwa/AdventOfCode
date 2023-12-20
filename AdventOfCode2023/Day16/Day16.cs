using Core;
using Helpers;
using Helpers.Grid;

namespace AdventOfCode2023.Day16
{
    public class Day16 : BaseAoc<FixedGrid>
    {
        Point Left => new Point(-1, 0);
        Point Right => new Point(1, 0);
        Point Up => new Point(0, -1);
        Point Down => new Point(0, 1);

        public int CalculateEnergy(Point startDirection, Point startLocation, FixedGrid grid)
        {
            var lightMovement = new FixedGrid<HashSet<Point>>(grid.Width, grid.Height, _ => new HashSet<Point>());

            var lightStack = new Stack<(Point Direction, FixedGridCellReference<char> Location)>();
            lightStack.Push((startDirection, grid.At(startLocation)));

            do
            {
                var (direction, pos) = lightStack.Pop();

                if (!pos.IsValid() || lightMovement.At(pos).Value.Contains(direction))
                {
                    continue;
                }
                else
                {
                    lightMovement.At(pos).Value.Add(direction);
                }

                switch (pos.Value)
                {
                    case '.':
                        lightStack.Push((direction, pos + direction));
                        break;
                    case '/':
                        if (direction == Down)
                        {
                            lightStack.Push((Left, pos + Left));
                        }
                        else if (direction == Right)
                        {
                            lightStack.Push((Up, pos + Up));
                        }
                        else if (direction == Up)
                        {
                            lightStack.Push((Right, pos + Right));
                        }
                        else
                        {
                            lightStack.Push((Down, pos + Down));
                        }
                        break;
                    case '\\':
                        if (direction == Down)
                        {
                            lightStack.Push((Right, pos + Right));
                        }
                        else if (direction == Right)
                        {
                            lightStack.Push((Down, pos + Down));
                        }
                        else if (direction == Up)
                        {
                            lightStack.Push((Left, pos + Left));
                        }
                        else
                        {
                            lightStack.Push((Up, pos + Up));
                        }
                        break;
                    case '-':
                        if (direction.Y != 0)
                        {
                            lightStack.Push((Right, pos + Right));
                            lightStack.Push((Left, pos + Left));
                        }
                        else
                        {
                            lightStack.Push((direction, pos + direction));
                        }
                        break;
                    case '|':
                        if (direction.X != 0)
                        {
                            lightStack.Push((Down, pos + Down));
                            lightStack.Push((Up, pos + Up));
                        }
                        else
                        {
                            lightStack.Push((direction, pos + direction));
                        }
                        break;
                }
            } while (lightStack.Any());


            return lightMovement.Count(c => c.Value.Count > 0);
        }

        public override string PartOne(FixedGrid input)
        {
            return CalculateEnergy(new Point(1, 0), new Point(0, 0), input).ToString();
        }

        public override string PartTwo(FixedGrid input)
        {
            var starts = Enumerable.Range(0, input.Width).Select(x => (Down, new Point(x, 0)))
                .Concat(Enumerable.Range(0, input.Width).Select(x => (Up, new Point(x, input.Height - 1))))
                .Concat(Enumerable.Range(0, input.Height).Select(y => (Right, new Point(0, y))))
                .Concat(Enumerable.Range(0, input.Height).Select(y => (Left, new Point(input.Width - 1, y))));

            return starts.Max(v => CalculateEnergy(v.Item1, v.Item2, input)).ToString();
        }

        protected override FixedGrid ParseInput(List<string> lines)
        {
            return lines.ToGrid();
        }
    }
}
