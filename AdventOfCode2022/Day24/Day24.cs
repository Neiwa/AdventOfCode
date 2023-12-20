using Core;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022.Day24
{
    public class Day24 : BaseAoc<Board>
    {
        public override string PartOne(Board input)
        {
            throw new NotImplementedException();
        }

        public override string PartTwo(Board input)
        {
            throw new NotImplementedException();
        }

        protected override Board ParseInput(List<string> lines)
        {
            Board board = new();
            IntPoint lastPos = new(0, 0);
            for (int y = 0; y < lines.Count; y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    switch (lines[y][x])
                    {
                        case '^':
                            board.Blizzards.Add((new IntPoint(x, y), Board.Direction.East));
                            break;
                        case '>':
                            board.Blizzards.Add((new IntPoint(x, y), Board.Direction.South));
                            break;
                        case 'v':
                            board.Blizzards.Add((new IntPoint(x, y), Board.Direction.West));
                            break;
                        case '<':
                            board.Blizzards.Add((new IntPoint(x, y), Board.Direction.North));
                            break;
                        case '.':
                            if (y == 0)
                            {
                                board.Start = new IntPoint(x, y);
                            }
                            break;
                        case '#':
                            lastPos = new IntPoint(x, y);
                            break;
                    }
                }
            }
            board.Goal = lastPos + new IntPoint(-1, 0);
            board.Bounds = new Rectangle(new IntPoint(1, 1), lastPos + new IntPoint(-1, -1));

            return board;
        }
    }

    public class Board
    {
        public enum Direction
        {
            East = 0,
            South = 1,
            West = 2,
            North = 3
        }

        public List<(IntPoint Pos, Direction Direction)> Blizzards { get; set; } = new();
        public Rectangle Bounds { get; set; }
        public IntPoint Start { get; set; }
        public IntPoint Goal { get; set; }
    }
}
