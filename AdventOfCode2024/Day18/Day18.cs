using Helpers.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Day18
{
    public class Day18 : BaseAocV2
    {
        public override object PartOne(List<string> lines)
        {
            bool isExample = FileName.Contains("example");
            var max = isExample ? 6 : 70;
            var grid = Parse(lines.Take(isExample ? 12 : 1024), max + 1);

            var start = grid.At(0, 0).Point;
            var goal = grid.At(max, max).Point;

            Draw(grid);

            var path = Search.AStar(start, goal, c => (goal - c).ManhattanLength, (l, r) => l == r, c => grid.At(c).GetNeighbours().Where(n => n.Value).Select(n => n.Point));


            return path.Count;
        }

        public override object PartTwo(List<string> lines)
        {
            bool isExample = FileName.Contains("example");
            var max = isExample ? 6 : 70;
            var byteIndex = isExample ? 12 : 1024;
            var grid = Parse(lines.Take(byteIndex), max + 1);

            var start = grid.At(0, 0).Point;
            var goal = grid.At(max, max).Point;

            Draw(grid);

            var path = Search.AStar(start, goal, c => (goal - c).ManhattanLength, (l, r) => l == r, c => grid.At(c).GetNeighbours().Where(n => n.Value).Select(n => n.Point));

            Point bytePos;
            do
            {
                do
                {
                    byteIndex++;
                    bytePos = Point.Parse(lines[byteIndex]);
                    grid.At(bytePos).Value = false;
                } while (!path.Contains(bytePos));

                path = Search.AStar(start, goal, c => (goal - c).ManhattanLength, (l, r) => l == r, c => grid.At(c).GetNeighbours().Where(n => n.Value).Select(n => n.Point));
            } while (path.Count > 0);

            return bytePos;
        }

        protected Grid<bool> Parse(IEnumerable<string> lines, int size)
        {
            var grid = new FixedGrid<bool>(size, size, true);

            foreach (var line in lines)
            {
                if (Point.TryParse(line, out var point))
                {
                    grid.SetAt(point!, false);
                }
            }

            return grid;
        }

        void Draw(Grid<bool> map)
        {
            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    Write(map.At(x, y).Value ? "." : "#");
                }
                WriteLine();
            }
            WriteLine();
        }
    }
}
