using Core;
using Helpers;
using Helpers.Extensions;
using Helpers.Grid;

namespace AdventOfCode2023.Day23
{
    public class Day23 : BaseAocV2
    {
        public record Route(Point Start, Point End);

        public override object PartOne(List<string> lines)
        {
            var map = lines.ToFixedGrid();

            var start = map.At(1, 0);
            var goal = map.At(map.Width - 2, map.Height - 1);

            var compressedMap = new ValueCreationDictionary<Route, int>();
            var calculatedRoutes = new HashSet<(Point Pos, Point PrevPos)>();

            var queue = new Queue<(GridCellReference<char> Pos, GridCellReference<char> PrevPos)>();
            queue.Enqueue((start, map.At(0, 0)));
            calculatedRoutes.Add((start, new(0, 0)));

            while (queue.TryDequeue(out var state))
            {
                var currentPos = state.PrevPos;

                IEnumerable<GridCellReference<char>> neighbours = state.Pos.AsEnumerable();

                var distance = 0;
                do
                {
                    var prevPos = currentPos;
                    currentPos = neighbours.First();
                    distance++;

                    neighbours = currentPos.GetNeighbours().Where(neighbour =>
                    {
                        if (neighbour == prevPos || neighbour.Value == '#')
                        {
                            return false;
                        }

                        switch (neighbour.Value)
                        {
                            case '^':
                                {
                                    return neighbour.Y < currentPos.Y;
                                }
                            case 'v':
                                {
                                    return neighbour.Y > currentPos.Y;
                                }
                            case '<':
                                {
                                    return neighbour.X < currentPos.X;
                                }
                            case '>':
                                {
                                    return neighbour.X > currentPos.X;
                                }
                            default:
                                return true;
                        }
                        return true;
                    });

                    if (currentPos.GetNeighbours().Count(n => n.Value != '#') > 2)
                    {
                        break;
                    }
                } while (neighbours.Count() == 1 && neighbours.First() != goal);


                var path = new Route(state.PrevPos, currentPos);
                compressedMap[path] = Math.Max(distance, compressedMap[path]);
                var routes = neighbours.Select(neighbour => (neighbour, currentPos)).Where(v => !calculatedRoutes.Contains(v));
                queue.EnqueueRange(routes);
                calculatedRoutes.AddRange(routes.Select(r => (r.neighbour.Point, r.currentPos.Point)));
            }

            foreach (var item in compressedMap)
            {
                WriteLine($"{item.Key.Start} -> {item.Key.End} = {item.Value}");
            }

            int longestRoute(Point current)
            {
                if (current == goal)
                {
                    return 0;
                }
                return compressedMap.Where(e => e.Key.Start == current).Max(e => longestRoute(e.Key.End) + e.Value);
            }

            return longestRoute(new Point(0, 0)) - 1;
        }

        public override object PartTwo(List<string> lines)
        {
            var map = lines.ToFixedGrid();

            var start = map.At(1, 0);
            var goal = map.At(map.Width - 2, map.Height - 1);

            var compressedMap = new ValueCreationDictionary<Route, int>();
            var calculatedRoutes = new HashSet<(Point Pos, Point PrevPos)>();

            var queue = new Queue<(GridCellReference<char> Pos, GridCellReference<char> PrevPos)>();
            queue.Enqueue((start, map.At(0, 0)));
            calculatedRoutes.Add((start, new(0, 0)));

            while (queue.TryDequeue(out var state))
            {
                var currentPos = state.PrevPos;

                IEnumerable<GridCellReference<char>> neighbours = state.Pos.AsEnumerable();

                var distance = 0;
                do
                {
                    var prevPos = currentPos;
                    currentPos = neighbours.First();
                    distance++;

                    neighbours = currentPos.GetNeighbours().Where(neighbour =>
                    {
                        if (neighbour == prevPos || neighbour.Value == '#')
                        {
                            return false;
                        }
                        return true;
                    });

                    if (currentPos.GetNeighbours().Count(n => n.Value != '#') > 2)
                    {
                        break;
                    }
                } while (neighbours.Count() == 1 && neighbours.First() != goal);


                var path = new Route(state.PrevPos, currentPos);
                compressedMap[path] = Math.Max(distance, compressedMap[path]);
                var routes = neighbours.Select(neighbour => (neighbour, currentPos)).Where(v => !calculatedRoutes.Contains(v));
                queue.EnqueueRange(routes);
                calculatedRoutes.AddRange(routes.Select(r => (r.neighbour.Point, r.currentPos.Point)));
            }

            foreach (var item in compressedMap)
            {
                WriteLine($"{item.Key.Start} -> {item.Key.End} = {item.Value}");
            }

            int longestRoute(Point current, HashSet<Route> usedRoutes)
            {
                if (current == goal)
                {
                    return 0;
                }
                return compressedMap.Keys.Except(usedRoutes).Where(e => e.Start == current).Max(e => longestRoute(e.End, [.. usedRoutes, e]) + compressedMap[e]);
            }

            return longestRoute(new Point(0, 0), []) - 1;
        }
    }
}
