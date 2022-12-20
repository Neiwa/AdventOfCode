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
        public Figure(long x, long y, Shape shape)
        {
            Position = new LongPoint(x, y);
            Shape = shape;
        }

        public LongPoint Position { get; }
        public Shape Shape { get; }

        public IEnumerable<LongPoint> LongPoints
        {
            get => Shape.Points.Select(p => Position + p);
        }

        public long Top
        {
            get => Position.Y;
        }

        public long Right
        {
            get => Position.X + Shape.Box.Width - 1;
        }
        public long Bottom
        {
            get => Position.Y + Shape.Box.Height - 1;
        }
    }

    public class SegementDictionary
    {
        private Dictionary<long, List<LongPoint>> _dict;
        public SegementDictionary(long segmentInterval)
        {
            _dict = new();
            SegmentInterval = segmentInterval;
        }

        public long SegmentInterval { get; }
        public List<LongPoint> GetSegmentAt(long key)
        {
            long segmentKey = key / SegmentInterval;
            if(!_dict.TryGetValue(segmentKey, out var result))
            {
                result = new();
                _dict[segmentKey] = result;
            }
            return result;
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

        public override string PartOne(List<string> lines)
        {
            return Sim(lines.First(), 2022).ToString();
        }

        public override string PartTwo(List<string> lines)
        {
            return Sim2(lines.First(), 1_000_000_000_000).ToString();
        }

        long Sim2(string line, long figureCount)
        {
            var figuresLeft = figureCount;
            int shapeIndex = 0;
            int jetIndex = 0;
            Figure? currentRock = null;
            SegementDictionary segments = new(100);
            long minY = 1;
            bool loopDetected = false;

            IndexCreationDictionary<int, IndexCreationDictionary<int, IndexCreationDictionary<long, (long, long)>>> stopLocations = new();

            while (figuresLeft >= 0)
            {
                if (currentRock == null)
                {
                    Shape shape = shapes[shapeIndex];
                    currentRock = new Figure(2, minY - 3 - shape.Box.Height, shape);
                    shapeIndex = ++shapeIndex % shapes.Count;
                    figuresLeft--;
                    //Draw(currentRock, segments, minY, 'C');
                }

                char c = line[jetIndex];
                int dir = c == '<' ? -1 : 1;
                jetIndex = ++jetIndex % line.Length;

                var seg1 = segments.GetSegmentAt(currentRock.Top);
                var seg2 = segments.GetSegmentAt(currentRock.Bottom);

                if (dir > 0 && currentRock.Right + dir <= MaxX)
                {
                    currentRock.Position.X += dir;
                    if (currentRock.LongPoints.Any(p => seg1.Contains(p) || seg2.Contains(p)))
                    {
                        currentRock.Position.X -= dir;
                    }
                }
                if (dir < 0 && currentRock.Position.X + dir >= 0)
                {
                    currentRock.Position.X += dir;
                    if (currentRock.LongPoints.Any(p => seg1.Contains(p) || seg2.Contains(p)))
                    {
                        currentRock.Position.X -= dir;
                    }
                }
                //Draw(currentRock, segments, minY, c);
                bool stop = false;
                if (currentRock.Bottom + 1 > 0)
                {
                    stop = true;
                }
                else
                {
                    currentRock.Position.Y += 1;
                    seg1 = segments.GetSegmentAt(currentRock.Top);
                    seg2 = segments.GetSegmentAt(currentRock.Bottom);
                    if (currentRock.LongPoints.Any(p => seg1.Contains(p) || seg2.Contains(p)))
                    {
                        currentRock.Position.Y -= 1;
                        stop = true;
                    }
                }

                if (stop)
                {
                    foreach (var point in currentRock.LongPoints)
                    {
                        segments.GetSegmentAt(point.Y).Add(point);
                        if (point.Y < minY)
                        {
                            minY = point.Y;
                        }
                    }

                    if (!loopDetected)
                    {
                        var stopDict = stopLocations[shapeIndex][jetIndex];
                        long profile = 0;
                        for (var i = 0; i < 64/7; i++)
                        {
                            var y = currentRock.Bottom + 20 + i; // 20 is the magic number!!!
                            var segment = segments.GetSegmentAt(y);
                            for (var x = 0; x <= MaxX; x++)
                            {
                                if (segment.Contains(new(x, y)))
                                {
                                    profile |= (1L << (i * MaxX + x));
                                }
                            }
                        }

                        if(currentRock.Position.Y == -1547)
                        {
                            Draw(null, segments, minY - 3, 'c', minY + 12);
                            Write($"profile = {Convert.ToString(profile, 2)}");
                        }


                        if(stopDict.TryGetValue(profile, out var tuple))
                        {
                            (var oldY, var fCount) = tuple;
                            var currentFigureCount = figureCount - figuresLeft;
                            // Repeat detected
                            Write($"profile = {Convert.ToString(profile, 2)}");
                            Write($"currentRock.Position.Y = {currentRock.Position.Y}, oldY = {oldY}, fCount = {fCount}, currentFigureCount = {currentFigureCount}");
                            Write($"Bf = {currentFigureCount - fCount}");
                            var figureRepeatCount = (figureCount-fCount)/(currentFigureCount - fCount);
                            Write($"figureRepeatCount(x coord) = {figureRepeatCount}");
                            Write($"A+Bx={oldY + (currentRock.Position.Y - oldY) * figureRepeatCount}");

                            Draw(null, segments, minY - 3, 'D', minY + 12);
                            Draw(null, segments, oldY - 3, 'O', oldY + 12);

                            var yIncrease = (currentRock.Position.Y - oldY) * (figureRepeatCount - 1);
                            for (var y = minY; y <= 0; y++)
                            {
                                var segment = segments.GetSegmentAt(y);
                                foreach (var point in segment)
                                {
                                    var newY = point.Y + yIncrease;
                                    segments.GetSegmentAt(newY).Add(new LongPoint(point.X, newY));
                                }
                            }
                            Write($"OLD: minY = {minY}, figuresLeft = {figuresLeft}");
                            minY = minY + yIncrease;
                            figuresLeft = figuresLeft - (currentFigureCount - fCount) * (figureRepeatCount - 1);
                            Write($"NEW: minY = {minY}, figuresLeft = {figuresLeft}");
                            loopDetected = true;
                            //return oldY + figureCount/(currentRock.Position.Y - oldY);
                        }
                        else
                        {
                            stopDict[profile] = (currentRock.Position.Y, figureCount - figuresLeft);
                        }

                    }

                    currentRock = null;

                }
                

                //Draw(currentRock, segments, minY, 'V');
            }

            return -minY + 1;
        }

        long Sim(string line, long figureCount)
        {
            var figuresLeft = figureCount;
            int shapeIndex = 0;
            int jetIndex = 0;
            Figure? currentRock = null;
            SegementDictionary segments = new(100);
            long minY = 1;

            while (figuresLeft >= 0)
            {
                if (currentRock == null)
                {
                    Shape shape = shapes[shapeIndex];
                    currentRock = new Figure(2, minY - 3 - shape.Box.Height, shape);
                    shapeIndex = ++shapeIndex % shapes.Count;
                    figuresLeft--;
                    //Draw(currentRock, segments, minY, 'C');
                }

                char c = line[jetIndex];
                int dir = c == '<' ? -1 : 1;
                jetIndex = ++jetIndex % line.Length;

                var seg1 = segments.GetSegmentAt(currentRock.Top);
                var seg2 = segments.GetSegmentAt(currentRock.Bottom);

                if (dir > 0 && currentRock.Right + dir <= MaxX)
                {
                    currentRock.Position.X += dir;
                    if (currentRock.LongPoints.Any(p => seg1.Contains(p) || seg2.Contains(p)))
                    {
                        currentRock.Position.X -= dir;
                    }
                }
                if (dir < 0 && currentRock.Position.X + dir >= 0)
                {
                    currentRock.Position.X += dir;
                    if (currentRock.LongPoints.Any(p => seg1.Contains(p) || seg2.Contains(p)))
                    {
                        currentRock.Position.X -= dir;
                    }
                }
                //Draw(currentRock, segments, minY, c);

                if (currentRock.Bottom + 1 > 0)
                {
                    foreach (var point in currentRock.LongPoints)
                    {
                        segments.GetSegmentAt(point.Y).Add(point);
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
                    seg1 = segments.GetSegmentAt(currentRock.Top);
                    seg2 = segments.GetSegmentAt(currentRock.Bottom);
                    if (currentRock.LongPoints.Any(p => seg1.Contains(p) || seg2.Contains(p)))
                    {
                        currentRock.Position.Y -= 1;
                        foreach (var point in currentRock.LongPoints)
                        {
                            segments.GetSegmentAt(point.Y).Add(point);
                            if (point.Y < minY)
                            {
                                minY = point.Y;
                            }
                        }
                        currentRock = null;
                    }
                }

                //Draw(currentRock, segments, minY, 'V');
            }

            return -minY + 1;
        }

        private void Draw(Figure? currentRock, SegementDictionary segments, long minY, char action, long maxY = 0)
        {
            if (!Debug) return;
            Console.WriteLine($"Row {minY-1}");
            Console.WriteLine($"{action}   0123456");
            for (var y = minY; y <= Math.Min(maxY, 0); y++)
            {
                Console.Write($"-{-y % 100:D2}|");
                var segment = segments.GetSegmentAt(y);
                for (var x = 0; x <= MaxX; x++)
                {
                    var point = new LongPoint(x, y);
                    if (null != currentRock && currentRock.LongPoints.Contains(point))
                    {
                        Console.Write('@');
                    }
                    else if (segment.Contains(point))
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
            if (maxY >= 0)
            {
                Console.WriteLine("   +-------+"); 
            }
            Console.WriteLine($"Row {maxY+1}");
            Console.WriteLine();
        }
    }
}
