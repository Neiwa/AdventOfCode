using Helpers.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Day16
{
    public record Puzzle(Grid<char> Map, GridCellReference<char> Start, GridCellReference<char> End, Direction Direction);

    public enum Direction
    {
        North,
        East,
        South,
        West
    }

    public class Day16 : BaseAoc<Puzzle>
    {
        public record State(Point Pos, Direction Direction);

        public Point DirectionAsPoint(Direction direction)
        {
            return direction switch
            {
                Direction.North => new Point(0, -1),
                Direction.East => new Point(1, 0),
                Direction.South => new Point(0, 1),
                Direction.West => new Point(-1, 0),
                _ => throw new NotImplementedException()
            };
        }

        public override object PartOne(Puzzle input)
        {
            var path = Search.AStar(
                new State(input.Start, input.Direction),
                s => s.Pos == input.End,
                s => (s.Pos - input.End).ManhattanLength,
                (l, r) => l == r,
                s =>
                {
                    var nextPos = s.Pos + DirectionAsPoint(s.Direction);
                    List<State> rotations = [
                        new State(s.Pos, (Direction)(((int)s.Direction + 1) % 4)),
                        new State(s.Pos, (Direction)(((int)s.Direction + 3) % 4))
                        ];
                    return input.Map.ValueAt(nextPos) != '#' ?
                    rotations.Append(new State(nextPos, s.Direction))
                    : rotations;
                },
                (l, r) => l.Direction != r.Direction ? 1000 : 1
                );

            var points = 0;
            for (int i = 0; i < path.Count - 1; i++)
            {
                var l = path[i];
                var r = path[i + 1];
                points += l.Direction != r.Direction ? 1000 : 1;
            }

            return points;
        }

        public override object PartTwo(Puzzle input)
        {
            throw new NotImplementedException();
        }

        protected override Puzzle ParseInput(List<string> lines)
        {
            var map = lines.ToGrid();
            var start = map.First(c => c.Value == 'S');
            var end = map.First(c => c.Value == 'E');

            return new Puzzle(map, start, end, Direction.East);
        }
    }
}
