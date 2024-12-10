using Helpers.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Day10
{
    public class Day10 : BaseAocV2
    {
        public override object PartOne(List<string> lines)
        {
            var map = lines.ToFixedGrid(c => c - '0');

            return map.Where(c => c.Value == 0).Sum(c => GetScore(c).Distinct().Count());
        }

        IEnumerable<Point> GetScore(GridCellReference<int> start)
        {
            WriteLine($"{start.Value}: {start.Point}");
            if (start.Value == 9)
            {
                return [start];
            }

            return start.GetNeighbours().Where(c => c.Value == start.Value + 1).SelectMany(GetScore);
        }

        public override object PartTwo(List<string> lines)
        {
            var map = lines.ToFixedGrid(c => c - '0');

            return map.Where(c => c.Value == 0).Sum(GetScore2);
        }

        int GetScore2(GridCellReference<int> start)
        {
            if (start.Value == 9)
            {
                return 1;
            }

            return start.GetNeighbours().Where(c => c.Value == start.Value + 1).Sum(GetScore2);
        }
    }
}
