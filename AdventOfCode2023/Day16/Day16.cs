using Core;
using Helpers;
using Helpers.Grid;

namespace AdventOfCode2023.Day16
{
    public class Day16 : BaseAoc<FixedIntGrid>
    {
        IntPoint Left => new IntPoint(-1, 0);
        IntPoint Right => new IntPoint(1, 0);
        IntPoint Up => new IntPoint(0, -1);
        IntPoint Down => new IntPoint(0, 1);

        public int CalculateEnergy(IntPoint startDirection, IntPoint startLocation, FixedIntGrid grid)
        {
            var lightMovement = new FixedIntGrid<HashSet<IntPoint>>(grid.Width, grid.Height, _ => new HashSet<IntPoint>());

            var lightStack = new Stack<(IntPoint Direction, FixedIntGridCellReference<char> Location)>();
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

        public override string PartOne(FixedIntGrid input)
        {
            return CalculateEnergy(new IntPoint(1, 0), new IntPoint(0, 0), input).ToString();
        }

        public override string PartTwo(FixedIntGrid input)
        {
            var starts = Enumerable.Range(0, input.Width).Select(x => (Down, new IntPoint(x, 0)))
                .Concat(Enumerable.Range(0, input.Width).Select(x => (Up, new IntPoint(x, input.Height - 1))))
                .Concat(Enumerable.Range(0, input.Height).Select(y => (Right, new IntPoint(0, y))))
                .Concat(Enumerable.Range(0, input.Height).Select(y => (Left, new IntPoint(input.Width - 1, y))));

            return starts.Max(v => CalculateEnergy(v.Item1, v.Item2, input)).ToString();
        }

        protected override FixedIntGrid ParseInput(List<string> lines)
        {
            return lines.ToFixedIntGrid();
        }
    }
}
