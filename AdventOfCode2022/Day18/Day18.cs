using Core;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022.Day18
{
    public class Day18 : BaseAocV2
    {
        List<Point3D> ParseInput(List<string> lines)
        {
            return lines.Select(l =>
            {
                var coords = l.Split(",").Select(s => int.Parse(s)).ToList();
                return new Point3D(coords[0], coords[1], coords[2]);
            }).ToList();
        }

        public override string PartOne(List<string> lines)
        {
            var points = ParseInput(lines);
            int maxX = points[0].X, minX = points[0].X, maxY = points[0].Y, minY = points[0].Y, maxZ = points[0].Z, minZ = points[0].Z;
            foreach (var point in points)
            {
                if (point.X > maxX)
                {
                    maxX = point.X;
                }
                if (point.X < minX)
                {
                    minX = point.X;
                }
                if (point.Y > maxY)
                {
                    maxY = point.Y;
                }
                if (point.Y < minY)
                {
                    minY = point.Y;
                }
                if (point.Z > maxZ)
                {
                    maxZ = point.Z;
                }
                if (point.Z < minZ)
                {
                    minZ = point.Z;
                }
            }

            int totalSurfaceArea = points.Count * 6;
            foreach (var point in points)
            {
                if (points.Contains(point + new Point3D(1, 0, 0)))
                {
                    totalSurfaceArea -= 2;
                }
                if (points.Contains(point + new Point3D(0, 1, 0)))
                {
                    totalSurfaceArea -= 2;
                }
                if (points.Contains(point + new Point3D(0, 0, 1)))
                {
                    totalSurfaceArea -= 2;
                }
            }

            for (int z = minZ; z <= maxZ; z++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    for (int x = minX; x <= maxX; x++)
                    {
                        if (points.Contains(new(x, y, z)))
                        {
                            Write('#');
                        }
                        else
                        {
                            Write('.');
                        }
                    }
                    WriteLine();
                }
                WriteLine();
            }

            WriteLine("[green]Green[/]");

            return $"{totalSurfaceArea}";
        }

        List<Point3D> Fill(Point3D init, List<Point3D> points, (Point3D TopLeft, Point3D BottomRight) bounds, bool stopOnOOB=true)
        {
            var fillPoints = new List<Point3D>() { init };
            var activePoints = new Queue<Point3D>();
            activePoints.Enqueue(init);

            while (activePoints.TryDequeue(out var current))
            {
                var newPoints = Enumerable.Range(0, 6).Select(i => current + i switch
                {
                    0 => new Point3D(1, 0, 0),
                    1 => new Point3D(-1, 0, 0),
                    2 => new Point3D(0, 1, 0),
                    3 => new Point3D(0, -1, 0),
                    4 => new Point3D(0, 0, 1),
                    5 => new Point3D(0, 0, -1)
                });
                foreach (var newPoint in newPoints)
                {
                    if (newPoint.X >= bounds.TopLeft.X && newPoint.X <= bounds.BottomRight.X &&
                        newPoint.Y >= bounds.TopLeft.Y && newPoint.Y <= bounds.BottomRight.Y &&
                        newPoint.Z >= bounds.TopLeft.Z && newPoint.Z <= bounds.BottomRight.Z)
                    {
                        if (!points.Contains(newPoint) && !fillPoints.Contains(newPoint))
                        {
                            fillPoints.Add(newPoint);
                            activePoints.Enqueue(newPoint);
                        }
                    }
                    else if (stopOnOOB)
                    {
                        return new();
                    }
                }
            }

            return fillPoints;
        }

        public override string PartTwo(List<string> lines)
        {
            var points = ParseInput(lines);
            var fillPoints = new List<Point3D>();
            int maxX = points[0].X, minX = points[0].X, maxY = points[0].Y, minY = points[0].Y, maxZ = points[0].Z, minZ = points[0].Z;
            foreach (var point in points)
            {
                if (point.X > maxX)
                {
                    maxX = point.X;
                }
                if (point.X < minX)
                {
                    minX = point.X;
                }
                if (point.Y > maxY)
                {
                    maxY = point.Y;
                }
                if (point.Y < minY)
                {
                    minY = point.Y;
                }
                if (point.Z > maxZ)
                {
                    maxZ = point.Z;
                }
                if (point.Z < minZ)
                {
                    minZ = point.Z;
                }
            }

            (Point3D TopLeft, Point3D BottomRight) bounds = (new(minX, minY, minZ), new(maxX, maxY, maxZ));

            var negativePoints = Fill(bounds.TopLeft, points, bounds, false);

            for (int z = minZ; z <= maxZ; z++)
            {
                WriteLine($"Z = {z}");
                Write("  ");
                for (int x = minX; x <= maxX; x++)
                {
                    Write((x / 10).ToString());
                }
                WriteLine();
                Write("  ");
                for (int x = minX; x <= maxX; x++)
                {
                    Write((x % 10).ToString());
                }
                WriteLine();
                for (int y = minY; y <= maxY; y++)
                {
                    Write($"{y:D2}");
                    for (int x = minX; x <= maxX; x++)
                    {
                        Point3D current = new(x, y, z);
                        if (points.Contains(current))
                        {
                            Write('#');
                        }
                        else if (fillPoints.Contains(current))
                        {
                            Write('+');
                        }
                        else if (negativePoints.Contains(current))
                        {
                            Write(' ');
                        }
                        else
                        {
                            var fill = Fill(current, points, bounds);
                            if (fill.Any())
                            {
                                fillPoints.AddRange(fill);
                                Write('+');
                            }
                            else
                            {
                                Write('.');
                            }
                        }
                    }
                    WriteLine();
                }
                WriteLine();
            }

            int totalSurfaceArea = points.Count * 6;
            foreach (var point in points)
            {
                Point3D newPoint = point + new Point3D(1, 0, 0);
                if (points.Contains(newPoint) || fillPoints.Contains(newPoint))
                {
                    totalSurfaceArea -= 2;
                }
                newPoint = point + new Point3D(0, 1, 0);
                if (points.Contains(newPoint) || fillPoints.Contains(newPoint))
                {
                    totalSurfaceArea -= 2;
                }
                newPoint = point + new Point3D(0, 0, 1);
                if (points.Contains(newPoint) || fillPoints.Contains(newPoint))
                {
                    totalSurfaceArea -= 2;
                }
            }

            return totalSurfaceArea.ToString();
        }
    }
}
