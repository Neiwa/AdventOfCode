using AdventOfCode2024.Day14;
using Helpers.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Day20
{
    public record Puzzle(Grid<char> Map, GridCellReference<char> Start, GridCellReference<char> End);

    public class Day20 : BaseAoc<Puzzle>
    {
        public override object PartOne(Puzzle input)
        {
            bool isExample = !FileName.Contains("input");

            var path = Search.AStar(
                input.Start.Point,
                input.End.Point,
                c => (input.End.Point - c).ManhattanLength,
                (l, r) => l == r,
                c => input.Map.At(c).GetNeighbours().Where(n => n.Value != '#').Select(c => c.Point));

            var potentialCheats = new HashSet<Point>(
                path.SelectMany(c => input.Map.At(c).GetNeighbours().Where(n => n.Value == '#' && n.GetNeighbours().Any(n2 => n2.Value != '#' && n2 != c))).Select(c => c.Point)
                );

            var count = 0;
            ValueCreationDictionary<int, int> results = [];
            foreach (var wallCell in potentialCheats)
            {
                var cheatedPath = Search.AStar(
                input.Start.Point,
                input.End.Point,
                c => (input.End.Point - c).ManhattanLength,
                (l, r) => l == r,
                c => input.Map.At(c).GetNeighbours().Where(n => n == wallCell || n.Value != '#').Select(c => c.Point));

                if (cheatedPath.Count > 0)
                {
                    var saved = path.Count - cheatedPath.Count;
                    results[saved]++;
                }
            }

            return results.Sum(kvp => kvp.Key >= (isExample ? 1 : 100) ? kvp.Value : 0);
        }

        public override object PartTwo(Puzzle input)
        {
            throw new NotImplementedException();
        }

        protected override Puzzle ParseInput(List<string> lines)
        {
            var map = lines.ToGrid();

            return new Puzzle(
                map,
                map.First(c => c.Value == 'S'),
                map.First(c => c.Value == 'E')
                );
        }
    }
}
