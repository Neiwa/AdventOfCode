using Helpers.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Day06
{
    public class Day06 : BaseAocV2
    {
        enum Direction
        {
            North,
            East,
            South,
            West
        }

        Dictionary<Direction, Point> dirPointMap = new Dictionary<Direction, Point>()
        {
            { Direction.North, new Point(0, -1) },
            { Direction.East, new Point(1, 0) },
            { Direction.South, new Point(0, 1) },
            { Direction.West, new Point(-1, 0) },
        };

        public override object PartOne(List<string> lines)
        {
            var map = lines.ToFixedGrid();
            var guardPos = map.First(c => c.Value == '^');
            var guardDir = Direction.North;

            HashSet<Point> visitedPoints = [guardPos];

            while(true)
            {
                if (!(guardPos + dirPointMap[guardDir]).IsValid())
                {
                    break;
                }

                if ((guardPos + dirPointMap[guardDir]).Value == '#')
                {
                    guardDir = (Direction)(((int)guardDir + 1) % 4);
                }
                else
                {
                    guardPos += dirPointMap[guardDir];
                    visitedPoints.Add(guardPos);
                }
            }

            return visitedPoints.Count;
        }

        public override object PartTwo(List<string> lines)
        {
            var maps = Enumerations.Range(0, 1, lines.Count).SelectMany(y => Enumerations.Range(0, 1, lines[0].Length).Select(x => new Point(x, y)))
            
            .Select(point =>
            {
                var map = lines.ToFixedGrid();

                return (point, map);
            })
            .Where(t => t.map.At(t.point).Value == '.')
            .Select(t =>
            {
                t.map.At(t.point).Value = '#';
                return t.map;
            });

            var count = 0;

            foreach (var map in maps)
            {
                var guardPos = map.First(c => c.Value == '^');
                var guardDir = Direction.North;

                HashSet<State> visitedStates = [new State(guardDir, guardPos)];

                while (true)
                {
                    if (!(guardPos + dirPointMap[guardDir]).IsValid())
                    {
                        break;
                    }

                    if ((guardPos + dirPointMap[guardDir]).Value == '#')
                    {
                        guardDir = (Direction)(((int)guardDir + 1) % 4);
                    }
                    else
                    {
                        guardPos += dirPointMap[guardDir];

                        var state = new State(guardDir, guardPos);

                        if (visitedStates.Contains(state))
                        {
                            count++;
                            break;
                        }
                        visitedStates.Add(state);
                    }
                }
            }

            return count;
        }

        record State(Direction Direction, Point Position);
    }
}
