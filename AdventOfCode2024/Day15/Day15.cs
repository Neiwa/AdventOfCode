using Helpers.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Day15
{
    public record Input(Grid<char> Map, List<Point> Movements);

    public class Day15 : BaseAoc<Input>
    {
        public override object PartOne(Input input)
        {
            var robotPos = input.Map.First(m => m.Value == '@');
            robotPos.Value = '.';

            GridCellReference<char>? movementValid(Point move)
            {
                var loc = robotPos;

                do
                {
                    loc += move;
                } while (loc.Value == 'O');

                return loc.Value == '.' ? loc : null;
            }

            foreach (var movement in input.Movements)
            {
                var pos = movementValid(movement);
                if (pos is not null)
                {
                    robotPos += movement;
                    if (robotPos.Value == 'O')
                    {
                        robotPos.Value = '.';
                        pos.Value = 'O';
                    }
                }
            }

            return input.Map.Where(c => c.Value == 'O').Sum(c => 100 * c.Y + c.X);
        }

        public override object PartTwo(Input input)
        {
            var map = new FixedGrid<char>(input.Map.Width * 2, input.Map.Height, p =>

                input.Map.ValueAt(p.X / 2, p.Y) switch
                {
                    '#' => '#',
                    'O' => p.X % 2 == 0 ? '[' : ']',
                    '.' => '.',
                    '@' => p.X % 2 == 0 ? '@' : '.',
                    _ => throw new NotImplementedException(),
                }
            );

            Draw(map);

            var robotPos = map.First(m => m.Value == '@');
            robotPos.Value = '.';

            GridCellReference<char>? xMovementValid(Point move)
            {
                var loc = robotPos;

                do
                {
                    loc += move;
                } while (loc.Value == '[' || loc.Value == ']');

                return loc.Value == '.' ? loc : null;
            }

            IEnumerable<GridCellReference<char>>? yMovementValid(GridCellReference<char> pos, Point dir)
            {
                var targetLoc = pos + dir;
                if (targetLoc.Value == '.')
                {
                    return [];
                }
                if (targetLoc.Value == '#')
                {
                    return null;
                }
                if (targetLoc.Value == '[')
                {
                    var l = yMovementValid(targetLoc, dir);
                    var r = yMovementValid(targetLoc + new Point(1, 0), dir);
                    return l is null || r is null ? null : [targetLoc, targetLoc + new Point(1, 0), .. l, .. r];
                }
                if (targetLoc.Value == ']')
                {
                    var l = yMovementValid(targetLoc + new Point(-1, 0), dir);
                    var r = yMovementValid(targetLoc, dir);
                    return l is null || r is null ? null : [targetLoc + new Point(-1, 0), targetLoc, .. l, .. r];
                }

                return null;
            }

            foreach (var movement in input.Movements)
            {
                if (movement.Y == 0)
                {
                    var xPos = xMovementValid(movement);
                    if (xPos is not null)
                    {
                        robotPos += movement;
                        if (robotPos.Value == '[' || robotPos.Value == ']')
                        {
                            var cursor = robotPos + movement;
                            do
                            {
                                cursor.Value = cursor.Value switch
                                {
                                    '[' => ']',
                                    ']' => '[',
                                    '.' => robotPos.Value switch
                                    {
                                        '[' => ']',
                                        ']' => '[',
                                        _ => throw new NotImplementedException(),
                                    },
                                    _ => throw new NotImplementedException(),
                                };
                                cursor += movement;
                            } while (cursor != xPos + movement);
                        }
                        robotPos.Value = '.';
                    }
                }
                else
                {
                    var yMoves = yMovementValid(robotPos, movement);
                    if (yMoves is not null)
                    {
                        var instructions = yMoves.Select(cell => (Source: cell, Target: cell + movement, TargetValue: cell.Value)).ToList();

                        foreach (var instruction in instructions)
                        {
                            instruction.Source.Value = '.';
                        }

                        foreach (var instruction in instructions)
                        {
                            instruction.Target.Value = instruction.TargetValue;
                        }

                        robotPos += movement;
                        robotPos.Value = '.';

                    }

                }

            }
            Draw(map, robotPos);

            return map.Where(c => c.Value == '[').Sum(c => 100 * c.Y + c.X);
        }

        void Draw(Grid<char> map, Point? robotPos = null)
        {
            for (var y = 0; y < map.Height; y++)
            {
                for (var x = 0; x < map.Width; x++)
                {
                    if (robotPos is not null && robotPos == new Point(x, y))
                    {
                        Write('@');
                    }
                    else
                    {
                        var value = map.ValueAt(x, y);
                        Write(value switch
                        {
                            '[' => "[[",
                            ']' => "]]",
                            _ => value.ToString()
                        });
                    }
                }
                WriteLine();
            }
        }

        protected override Input ParseInput(List<string> lines)
        {
            var map = lines.TakeWhile(l => l.Length > 0).ToGrid();

            var movements = lines.Skip((int)map.Height).SelectMany(line => line.Select(c => c switch
            {
                '^' => new Point(0, -1),
                '>' => new Point(1, 0),
                '<' => new Point(-1, 0),
                'v' => new Point(0, 1),
                _ => throw new NotImplementedException(),
            })).ToList();

            return new Input(map, movements);
        }
    }
}
