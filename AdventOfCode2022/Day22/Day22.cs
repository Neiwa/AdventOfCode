using Core;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022.Day22
{
    public class Day22 : BaseAoc<Board>
    {
        Dictionary<int, Point> facingToPoint = new()
        {
            {0, new Point(1, 0) },
            {1, new Point(0, 1) },
            {2, new Point(-1, 0) },
            {3, new Point(0, -1) }
        };

        public char GetChar(Point pos)
        {
            if (Input.Map.Count <= pos.Y || pos.Y < 0)
            {
                return ' ';
            }
            string row = Input.Map[pos.Y];
            if (row.Length <= pos.X || pos.X < 0)
            {
                return ' ';
            }
            return row[pos.X];
        }

        public Point NextPos(Point pos, int facing)
        {
            switch (GetChar(pos + facingToPoint[facing]))
            {
                case '#':
                    return pos;
                case '.':
                    return pos + facingToPoint[facing];
            }

            var nextPos = pos;
            var move = facingToPoint[(facing + 2) % 4];
            do
            {
                nextPos += move;
            } while (GetChar(nextPos + move) != ' ');
            if (GetChar(nextPos) == '.')
            {
                return nextPos;
            }
            return pos;
        }

        public (Point NewPos, int NewFacing) NextPosCube(Point pos, int facing)
        {
            Point nextPos = pos + facingToPoint[facing];
            int nextFacing = facing;

            var posCubeIndex = Input.Cubes.FindIndex(cube => cube.Bounds.Contains(pos));
            var nextPosCubeIndex = Input.Cubes.FindIndex(cube => cube.Bounds.Contains(nextPos));
            if (posCubeIndex != nextPosCubeIndex)
            {
                var translation = Input.Cubes[posCubeIndex].OutOfBoundsTranslation[nextPos];
                nextPos = translation.Dest;
                nextFacing = translation.Facing;
            }

            switch (GetChar(nextPos))
            {
                case '#':
                    return (pos, facing);
                case '.':
                    return (nextPos, nextFacing);
                default:
                    throw new ArgumentException();
            }
        }

        void Draw(Point player, int facing)
        {
            for (int y = 0; y < Input.Map.Count; y++)
            {
                for (int x = 0; x < Input.Map[y].Length; x++)
                {
                    if (player == new Point(x, y))
                    {
                        switch (facing)
                        {
                            case 0:
                                Write('>');
                                break;
                            case 1:
                                Write('v');
                                break;
                            case 2:
                                Write('<');
                                break;
                            case 3:
                                Write('^');
                                break;
                        }
                    }
                    else
                    {
                        Write(Input.Map[y][x], ActionLevel.Trace);
                    }
                }
                WriteLine(ActionLevel.Trace);
            }
        }

        public override string PartOne(Board board)
        {
            int facing = 0;
            Point pos = new(Input.Map[0].IndexOf('.'), 0);

            foreach (var instruction in board.Instructions)
            {
                switch (instruction.Action)
                {
                    case 'R':
                        facing = (facing + 1) % 4;
                        break;
                    case 'L':
                        facing = (facing + 3) % 4;
                        break;
                    default:
                        {
                            for (int i = 0; i < instruction.Steps; i++)
                            {
                                pos = NextPos(pos, facing);
                            }
                            break;
                        }
                }
                if (IsTrace)
                {
                    Draw(pos, facing);
                    WriteLine(ActionLevel.Trace);
                }
            }

            long sum = 1_000 * (pos.Y + 1) + 4 * (pos.X + 1) + facing;
            return sum.ToString();
        }

        public override string PartTwo(Board input)
        {
            int facing = 0;
            Point pos = new(Input.Map[0].IndexOf('.'), 0);

            foreach (var instruction in Input.Instructions)
            {
                switch (instruction.Action)
                {
                    case 'R':
                        facing = (facing + 1) % 4;
                        break;
                    case 'L':
                        facing = (facing + 3) % 4;
                        break;
                    default:
                        {
                            for (int i = 0; i < instruction.Steps; i++)
                            {
                                (pos, facing) = NextPosCube(pos, facing);
                            }
                            break;
                        }
                }
                if (IsTrace)
                {
                    Draw(pos, facing);
                    WriteLine(ActionLevel.Trace);
                }
            }

            long sum = 1_000 * (pos.Y + 1) + 4 * (pos.X + 1) + facing;
            return sum.ToString();
        }

        protected override Board ParseInput(List<string> lines)
        {
            List<string> map = lines.TakeWhile(l => l.Length > 0).ToList();
            List<Instruction> instructions = new();
            string instructionString = lines.Skip(map.Count).SkipWhile(l => l.Length == 0).First();
            string currentNumber = string.Empty;
            foreach (var c in instructionString)
            {
                switch (c)
                {
                    case 'R':
                        instructions.Add(new Instruction { Action = 'M', Steps = int.Parse(currentNumber) });
                        instructions.Add(new Instruction { Action = 'R' });
                        currentNumber = string.Empty;
                        break;
                    case 'L':
                        instructions.Add(new Instruction { Action = 'M', Steps = int.Parse(currentNumber) });
                        instructions.Add(new Instruction { Action = 'L' });
                        currentNumber = string.Empty;
                        break;
                    default:
                        currentNumber += c;
                        break;
                }
            }
            instructions.Add(new Instruction { Action = 'M', Steps = int.Parse(currentNumber) });

            Board board = new Board(map, instructions);

            switch (FileName)
            {
                case "input.txt":
                    {
                        board.CubeSize = 50;
                        List<Rectangle> sides = new()
                        {
                            new Rectangle(50, 0, 50, 50),
                            new Rectangle(100, 0, 50, 50),
                            new Rectangle(50, 50, 50, 50),
                            new Rectangle(0, 100, 50, 50),
                            new Rectangle(50, 100, 50, 50),
                            new Rectangle(0, 150, 50, 50)
                        };
                        //   0011
                        //   0011
                        //   22
                        //   22
                        // 3344
                        // 3344
                        // 55
                        // 55

                        Dictionary<(int Side, int Facing), (int Side, int Facing, int Corner, int Direction)> cubeRelations = new()
                        {
                            { (0, 0), (1, 2, 3, 1) }, // Right
                            { (0, 1), (2, 3, 0, 2) }, // Down
                            { (0, 2), (3, 2, 3, 1) }, // Left
                            { (0, 3), (5, 2, 3, 1) }, // Up
                            { (1, 0), (4, 0, 1, 3) }, // Right
                            { (1, 1), (2, 0, 1, 3) }, // Down
                            { (1, 2), (0, 0, 1, 3) }, // Left
                            { (1, 3), (5, 1, 2, 0) }, // Up
                            { (2, 0), (1, 1, 2, 0) }, // Right
                            { (2, 1), (4, 3, 0, 2) }, // Down
                            { (2, 2), (3, 3, 0, 2) }, // Left
                            { (2, 3), (0, 1, 2, 0) }, // Up
                            { (3, 0), (4, 2, 3, 1) }, // Right
                            { (3, 1), (5, 3, 0, 2) }, // Down
                            { (3, 2), (0, 2, 3, 1) }, // Left
                            { (3, 3), (2, 2, 3, 1) }, // Up
                            { (4, 0), (1, 0, 1, 3) }, // Right
                            { (4, 1), (5, 0, 1, 3) }, // Down
                            { (4, 2), (3, 0, 1, 3) }, // Left
                            { (4, 3), (2, 1, 2, 0) }, // Up
                            { (5, 0), (4, 1, 2, 0) }, // Right
                            { (5, 1), (1, 3, 0, 2) }, // Down
                            { (5, 2), (0, 3, 0, 2) }, // Left
                            { (5, 3), (3, 1, 2, 0) }  // Up
                        };
                        
                        for (int s = 0; s < sides.Count; s++)
                        {
                            var side = sides[s];
                            Dictionary<Point, (Point Dest, int Facing)> oobDict = new();
                            for (int f = 0; f < 4; f++)
                            {
                                var relation = cubeRelations[(s, f)];
                                Point sourcePos = side.Position + f switch
                                {
                                    0 => facingToPoint[0] * side.Width,
                                    1 => facingToPoint[0] * (side.Width - 1) + facingToPoint[1] * side.Width,
                                    2 => facingToPoint[2] + facingToPoint[1] * (side.Width - 1),
                                    3 => facingToPoint[3],
                                    _ => throw new ArgumentException()
                                };
                                Point sourceStep = facingToPoint[(f + 1) % 4];

                                Point targetPos = sides[relation.Side].Position + relation.Corner switch
                                {
                                    0 => facingToPoint[0] * (side.Width - 1),
                                    1 => facingToPoint[0] * (side.Width - 1) + facingToPoint[1] * (side.Width - 1),
                                    2 => facingToPoint[1] * (side.Width - 1),
                                    3 => new Point(0, 0),
                                    _ => throw new ArgumentException()
                                };
                                Point targetStep = facingToPoint[relation.Direction];

                                for (int i = 0; i < side.Width; i++)
                                {
                                    oobDict.Add(sourcePos, (targetPos, (relation.Facing + 2) % 4));
                                    sourcePos += sourceStep;
                                    targetPos += targetStep;
                                }
                            }
                            board.Cubes.Add((side, oobDict));
                        }
                        break;
                    }
                case "example.txt":
                    //board.CubeSize = 4;
                    //board.Cubes = new()
                    //{
                    //    new Rectangle(8, 0, 4, 4),
                    //    new Rectangle(0, 4, 4, 4),
                    //    new Rectangle(4, 4, 4, 4),
                    //    new Rectangle(8, 4, 4, 4),
                    //    new Rectangle(8, 8, 4, 4),
                    //    new Rectangle(12, 8, 4, 4)
                    //};
                    break;
            }
            return board;
        }
    }

    public class Instruction
    {
        public char Action { get; set; }
        public int Steps { get; set; }
    }

    public class Board
    {
        public Board(List<string> map, List<Instruction> instructions)
        {
            Map = map;
            Instructions = instructions;
        }

        public List<string> Map { get; }
        public List<Instruction> Instructions { get; }

        public List<(Rectangle Bounds, Dictionary<Point, (Point Dest, int Facing)> OutOfBoundsTranslation)> Cubes { get; set; } = new();
        public int CubeSize { get; set; }
    }
}
