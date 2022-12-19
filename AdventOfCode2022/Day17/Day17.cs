using Core;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022.Day17
{
    public class Shape
    {
        public List<Point> Points { get; }
        public Rectangle Box { get; }

        public Shape(params Point[] points)
        {
            Points = points.ToList();
            Box = new Rectangle(new Point(0, 0), Points.Max(p => p.X) + 1, Points.Max(p => p.Y) + 1);
        }
    }

    public class Figure
    {
        public Figure(int x, int y, Shape shape)
        {
            Position = new Point(x, y);
            Shape = shape;
        }

        public Point Position { get; }
        public Shape Shape { get; }

        public IEnumerable<Point> Points
        {
            get => Shape.Points.Select(p => Position + p);
        }

        public int Right
        {
            get => Position.X + Shape.Box.Width - 1;
        }
        public int Bottom 
        {
            get => Position.Y + Shape.Box.Height - 1;
        }
    }

    public class Day17 : BaseAoc
    {
        List<Shape> shapes = new()
        {
            new(new(0, 0), new(1, 0), new(2, 0), new(3,0)),
            new(new(1, 0), new(0, 1), new(1, 1), new(2, 1), new(1, 2)),
            new(new(2, 0), new(2, 1), new(0, 2), new(1, 2), new(2, 2)),
            new(new(0, 0), new(0, 1), new(0, 2), new(0, 3)),
            new(new(0, 0), new(0, 1), new(1, 0), new(1, 1))
        };

        const int MaxX = 6;

        public override void PartOne(List<string> lines)
        {
            Sim(lines.First(), 2022);
        }

        public override void PartTwo(List<string> lines)
        {
            Sim(lines.First(), 1_000_000_000_000);
        }

        void Sim(string line, long figureCount)
        {
            var figuresLeft = figureCount;
            int shapeIndex = 0;
            int jetIndex = 0;
            Figure? currentRock = null;
            List<Point> rocks = new();
            int minY = 1;
            long total = 0;

            while (figuresLeft >= 0)
            {
                if (currentRock == null)
                {
                    Shape shape = shapes[shapeIndex];
                    currentRock = new Figure(2, minY - 3 - shape.Box.Height, shape);
                    shapeIndex = ++shapeIndex % shapes.Count;
                    figuresLeft--;
                    //Draw(currentRock, rocks, minY, 'C');
                }

                char c = line[jetIndex];
                int dir = c == '<' ? -1 : 1;
                jetIndex = ++jetIndex % line.Length;

                if (dir > 0 && currentRock.Right + dir <= MaxX)
                {
                    currentRock.Position.X += dir;
                    if (currentRock.Points.Any(p => rocks.Contains(p)))
                    {
                        currentRock.Position.X -= dir;
                    }
                }
                if (dir < 0 && currentRock.Position.X + dir >= 0)
                {
                    currentRock.Position.X += dir;
                    if (currentRock.Points.Any(p => rocks.Contains(p)))
                    {
                        currentRock.Position.X -= dir;
                    }
                }
                //Draw(currentRock, rocks, minY, c);

                if (currentRock.Bottom + 1 > 0)
                {
                    foreach (var point in currentRock.Points)
                    {
                        rocks.Add(point);
                        if (point.Y < minY)
                        {
                            minY = point.Y;
                        }
                    }
                    currentRock = null;
                }
                else
                {
                    currentRock.Position.Y += 1;
                    if (currentRock.Points.Any(p => rocks.Contains(p)))
                    {
                        currentRock.Position.Y -= 1;
                        foreach (var point in currentRock.Points)
                        {
                            rocks.Add(point);
                            if (point.Y < minY)
                            {
                                minY = point.Y;
                            }
                        }
                        currentRock = null;

                        if (shapeIndex == 0)
                        {
                            int mask = 0;
                            int terminalY = 1;
                            for (int y = minY; y <= 0; y++)
                            {
                                int yMask = 0;
                                for (int x = 0; x <= MaxX; x++)
                                {
                                    if (rocks.Contains(new(x, y)))
                                    {
                                        yMask = yMask | (1 << x);
                                    }
                                }
                                mask = mask | yMask;
                                if (mask == 127)
                                {
                                    break;
                                }
                            }
                            if (mask == 127)
                            {
                                rocks = rocks.Where(p => p.Y < terminalY).Select(p => new Point(p.X, p.Y - terminalY)).ToList();
                                minY -= terminalY;
                                total -= terminalY;
                            } 
                        }
                    }
                }

                //Draw(currentRock, rocks, minY, 'V');
            }
            Console.WriteLine(total);
            Console.WriteLine(minY);
            Console.WriteLine(-minY+1);
            Console.WriteLine(total-minY + 1);
        }

        private void Draw(Figure? currentRock, List<Point> rocks, int minY, char action)
        {
            if (!Debug) return;

            Console.WriteLine($"{action}   0123456");
            for (int y = minY - 6; y <= 0; y++)
            {
                Console.Write($"-{-y:D2}|");
                for (int x = 0; x <= MaxX; x++)
                {
                    var point = new Point(x, y);
                    if (null != currentRock && currentRock.Points.Contains(point))
                    {
                        Console.Write('@');
                    }
                    else if (rocks.Contains(point))
                    {
                        Console.Write('#');
                    }
                    else
                    {
                        Console.Write('.');
                    }

                }
                Console.WriteLine('|');
            }
            Console.WriteLine("   +-------+");
            Console.WriteLine();
        }
    }
}
