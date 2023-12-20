using Core;
using Helpers;
using Helpers.Grid;

namespace AdventOfCode2023.Day17
{
    public class Day17 : BaseAoc<FixedIntGrid<int>>
    {
        public void Draw(FixedIntGrid<int> grid, IList<State> path)
        {
            if (!IsDebug) return;

            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    var node = path.FirstOrDefault(s => s.Position == new IntPoint(x, y));
                    if (node is not null)
                    {
                        if (node.Direction.X > 0)
                        {
                            Write('>');
                        }
                        else if (node.Direction.X < 0)
                        {
                            Write('<');
                        }
                        else if (node.Direction.Y > 0)
                        {
                            Write('v');
                        }
                        else if (node.Direction.Y < 0)
                        {
                            Write('^');
                        }
                        else
                        {
                            Write('#');
                        }
                    }
                    else
                    {
                        Write(grid.ValueAt(x, y).ToString());
                    }
                }
                WriteLine();
            }
        }

        public record State(IntPoint Position, IntPoint Direction, int Length);

        public override string PartOne(FixedIntGrid<int> input)
        {
            var start = new State(input.At(0, 0), new IntPoint(0, 1), 0);
            var goal = input.At(input.Width - 1, input.Height - 1);

            IEnumerable<State> getNeighbors(State current)
            {
                var direction = current.Direction;
                var potentialPaths = input.At(current.Position)
                    .GetNeighbours()
                    .Where(neighbour => neighbour - current.Position != direction * -1);

                if (current.Length >= 3)
                {
                    potentialPaths = potentialPaths.Where(neighbour => neighbour - current.Position != direction);
                }

                return potentialPaths.Select(neighbour =>
                {
                    var pathDirection = neighbour.Point - current.Position;
                    return new State(neighbour, pathDirection, pathDirection == current.Direction ? current.Length + 1 : 1);
                });
            }

            var path = Search.AStar(start,
                c => c.Position == goal.Point && c.Length < 3,
                c => Math.Abs(goal.X - c.Position.X) + Math.Abs(goal.Y - c.Position.Y),
                (l, r) => l == r,
                getNeighbors,
                (curr, neigh) => input.At(neigh.Position).Value);

            Draw(input, path);

            return path.Skip(1).Sum(c => input.At(c.Position).Value).ToString();
        }

        public override string PartTwo(FixedIntGrid<int> input)
        {
            var start = new State(input.At(0, 0), new IntPoint(0, 1), 0);
            var goal = input.At(input.Width - 1, input.Height - 1);

            IEnumerable<State> getNeighbors(State current)
            {
                var direction = current.Direction;

                if (current.Length != 0 && current.Length < 4)
                {
                    var next = current.Position + direction;
                    if (input.At(next).IsValid())
                    {
                        return Enumerable.Empty<State>().Append(new State(next, current.Direction, current.Length + 1));
                    }
                    else
                    {
                        return Enumerable.Empty<State>();
                    }
                }

                var potentialPaths = input.At(current.Position)
                    .GetNeighbours()
                    .Where(neighbour => neighbour - current.Position != direction * -1);

                if (current.Length >= 10)
                {
                    potentialPaths = potentialPaths.Where(neighbour => neighbour - current.Position != direction);
                }

                return potentialPaths.Select(neighbour =>
                {
                    var pathDirection = neighbour.Point - current.Position;
                    return new State(neighbour, pathDirection, pathDirection == current.Direction ? current.Length + 1 : 1);
                });
            }

            var path = Search.AStar(start,
            c => c.Position == goal.Point && c.Length > 3,
                c => Math.Abs(goal.X - c.Position.X) + Math.Abs(goal.Y - c.Position.Y),
                (l, r) => l == r,
                getNeighbors,
                (curr, neigh) => input.At(neigh.Position).Value);

            Draw(input, path);

            var minHeatLoss = path.Skip(1).Sum(c => input.At(c.Position).Value);

            return minHeatLoss.ToString();
        }

        protected override FixedIntGrid<int> ParseInput(List<string> lines)
        {
            return lines.ToFixedIntGrid(c => c - 48);
        }
    }
}
