using Core;
using Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022.Day23
{
    public class Day23 : BaseAoc<Board>
    {
        Dictionary<int, List<Point>> directionToPoints = new()
        {
            {0, new(){ new(0, -1), new(-1, -1), new(1, -1)} },
            {1, new(){ new(0, 1), new(-1, 1), new(1, 1)} },
            {2, new(){ new(-1, 0), new(-1, 1), new(-1, -1)} },
            {3, new(){ new(1, 0), new(1, 1), new(1, -1)} },
        };
        List<Point> around = new()
        {
            new Point(-1, -1),
            new Point(0, -1),
            new Point(1, -1),
            new Point(1, 0),
            new Point(1, 1),
            new Point(0, 1),
            new Point(-1, 1),
            new Point(-1, 0)
        };

        public override string PartOne(Board input)
        {
            Rectangle drawMinBounds = new Rectangle(-1, -1, 14, 12);
            Draw(drawMinBounds);
            for (int round = 0; round < 10; round++)
            {
                ValueCreationDictionary<Point, List<Point>> proposals = new(); // Key: To, Value(s): From
                List<Point> newElves = new();
                foreach (var elf in Input.Elves)
                {
                    Point? move = null;
                    if (around.Any(p => Input.Elves.Contains(elf + p)))
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            var scoutPoints = directionToPoints[(Input.CurrentDirection + i) % 4];
                            bool thisDirction = true;
                            foreach (var point in scoutPoints)
                            {
                                if (Input.Elves.Contains(elf + point))
                                {
                                    thisDirction = false;
                                    break;
                                }
                            }
                            if (thisDirction)
                            {
                                move = elf + scoutPoints[0];
                                break;
                            }
                        }
                    }
                    if (move is not null)
                    {
                        proposals[move].Add(elf);
                    }
                    else
                    {
                        newElves.Add(elf);
                    }
                }

                foreach (var (to, from) in proposals)
                {
                    if (from.Count > 1)
                    {
                        newElves.AddRange(from);
                    }
                    else
                    {
                        newElves.Add(to);
                    }
                }
                Input.Elves = newElves;

                WriteLine($"Round {round+1}", ActionLevel.Trace);
                WriteLine($"Primary direction {(Input.CurrentDirection switch { 0 => "North", 1 => "South", 2 => "West", 3 => "East" })}", ActionLevel.Trace);
                Draw(drawMinBounds);

                Input.CurrentDirection = (Input.CurrentDirection + 1) % 4;
            }

            int minX = Input.Elves.Min(p => p.X);
            int maxX = Input.Elves.Max(p => p.X);
            int minY = Input.Elves.Min(p => p.Y);
            int maxY = Input.Elves.Max(p => p.Y);

            var emptySpaces = (maxX - minX + 1) * (maxY - minY + 1) - Input.Elves.Count;

            return emptySpaces.ToString();
        }

        public override string PartTwo(Board input)
        {
            Rectangle drawMinBounds = new Rectangle(-1, -1, 14, 12);
            Draw(drawMinBounds);
            int round = 0;
            bool elfMoved = false;
            do
            {
                elfMoved = false;
                ValueCreationDictionary<Point, List<Point>> proposals = new(); // Key: To, Value(s): From
                List<Point> newElves = new();
                foreach (var elf in Input.Elves)
                {
                    Point? move = null;
                    if (around.Any(p => Input.Elves.Contains(elf + p)))
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            var scoutPoints = directionToPoints[(Input.CurrentDirection + i) % 4];
                            bool thisDirction = true;
                            foreach (var point in scoutPoints)
                            {
                                if (Input.Elves.Contains(elf + point))
                                {
                                    thisDirction = false;
                                    break;
                                }
                            }
                            if (thisDirction)
                            {
                                move = elf + scoutPoints[0];
                                break;
                            }
                        }
                    }
                    if (move is not null)
                    {
                        proposals[move].Add(elf);
                    }
                    else
                    {
                        newElves.Add(elf);
                    }
                }

                foreach (var (to, from) in proposals)
                {
                    if (from.Count > 1)
                    {
                        newElves.AddRange(from);
                    }
                    else
                    {
                        newElves.Add(to);
                        elfMoved = true;
                    }
                }
                Input.Elves = newElves;

                if (IsTrace)
                {
                    WriteLine($"Round {round + 1}", ActionLevel.Trace);
                    WriteLine($"Primary direction {(Input.CurrentDirection switch { 0 => "North", 1 => "South", 2 => "West", 3 => "East" })}", ActionLevel.Trace); 
                }
                Draw(drawMinBounds);

                Input.CurrentDirection = (Input.CurrentDirection + 1) % 4;
                round++;
            } while (elfMoved);

            return round.ToString();
        }

        protected override Board ParseInput(List<string> lines)
        {
            List<Point> elves = new();
            for (int y = 0; y < lines.Count; y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    if (lines[y][x] == '#')
                    {
                        elves.Add(new Point(x, y));
                    }
                }
            }
            return new Board(elves);
        }

        void Draw(Rectangle drawMinBounds)
        {
            if (!IsTrace) return;

            int minX = Math.Min(Input.Elves.Min(p => p.X), drawMinBounds.Position.X);
            int maxX = Math.Max(Input.Elves.Max(p => p.X), drawMinBounds.Position.X + drawMinBounds.Width);
            int minY = Math.Min(Input.Elves.Min(p => p.Y), drawMinBounds.Position.Y);
            int maxY = Math.Max(Input.Elves.Max(p => p.Y), drawMinBounds.Position.Y + drawMinBounds.Height);

            for (int i = 0; i < 3; i++)
            {
                Write("   ", ActionLevel.Trace);
                for (int x = minX; x <= maxX; x++)
                {
                    Write(i switch
                    {
                        0 => x < 0 ? "|" : " ",
                        1 => (x / 10).ToString(),
                        2 => (((x % 10) + 10) % 10).ToString()
                    }, ActionLevel.Trace);
                }
                WriteLine(ActionLevel.Trace);
            }
            for (int y = minY; y <= maxY; y++)
            {
                Write($"{y,3}", ActionLevel.Trace);
                for (int x = minX; x <= maxX; x++)
                {
                    if(Input.Elves.Contains(new(x, y)))
                    {
                        Write('#', ActionLevel.Trace);
                    }
                    else
                    {
                        Write('.', ActionLevel.Trace);
                    }
                }
                WriteLine(ActionLevel.Trace);
            }
            WriteLine(ActionLevel.Trace);
        }
    }

    public struct Move
    {
        public Move(Point source, Point target)
        {
            Source = source;
            Target = target;
        }
        public Point Source { get; set; }
        public Point Target { get; set; }
        public bool Valid { get; set; } = true;
    }

    public class Board
    {
        public Board(List<Point> elves)
        {
            Elves = elves;
        }

        public List<Point> Elves { get; set; }
        public int CurrentDirection { get; set; } = 0;
    }
}
