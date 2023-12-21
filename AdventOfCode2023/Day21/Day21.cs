using Core;
using Helpers;
using Helpers.Grid;

namespace AdventOfCode2023.Day21
{
    public class Day21 : BaseAoc<Grid<char>>
    {
        public override string PartOne(Grid<char> input)
        {
            var steps = this.FileName.Contains("example") ? 6 : 64;

            var start = input.First(c => c.Value == 'S');

            var positions = new HashSet<Point> { start };

            for (var i = 0; i < steps; i++)
            {
                var newPositions = new HashSet<Point>();
                foreach (var pos in positions)
                {
                    foreach (var neighbour in input.At(pos).GetNeighbours().Where(c => c.Value != '#'))
                    {
                        newPositions.Add(neighbour);
                    }
                }
                positions = newPositions;
            }

            return positions.Count.ToString();
        }

        public override string PartTwo(Grid<char> input)
        {
            throw new NotImplementedException();
        }

        protected override Grid<char> ParseInput(List<string> lines)
        {
            return lines.ToFixedGrid();
        }
    }
}
