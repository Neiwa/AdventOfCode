using Helpers.Extensions;
using Helpers.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Day12
{
    public class Day12 : BaseAocV2
    {
        public override object PartOne(List<string> lines)
        {
            var garden = lines.ToGrid();

            var visitedPlots = new HashSet<Point>();

            long totalPrice = 0;

            foreach (var plot in garden)
            {
                if (visitedPlots.Contains(plot))
                {
                    continue;
                }

                var crop = plot.Value;
                var regionPlots = new HashSet<Point>();
                var queue = new Queue<Point>();
                queue.Enqueue(plot);
                while (queue.TryDequeue(out var p))
                {
                    if (regionPlots.Contains(p))
                    {
                        continue;
                    }
                    regionPlots.Add(p);

                    var ps = garden.At(p).GetNeighbours().Where(n => n.Value == crop && !regionPlots.Contains(n));
                    queue.EnqueueRange(ps.Select(pp => pp.Point));
                }

                var area = regionPlots.Count;
                var perimeter = regionPlots.Sum(p => garden.At(p).GetNeighbours(includeInvalid: true).Count(n => !n.IsValid() || n.Value != crop));
                totalPrice += area * perimeter;

                visitedPlots.AddRange(regionPlots);
            }

            return totalPrice;
        }

        public override object PartTwo(List<string> lines)
        {
            var garden = lines.ToGrid();

            var visitedPlots = new HashSet<Point>();

            long totalPrice = 0;

            foreach (var plot in garden)
            {
                if (visitedPlots.Contains(plot))
                {
                    continue;
                }

                var crop = plot.Value;
                var regionPlots = new HashSet<Point>();
                var queue = new Queue<Point>();
                queue.Enqueue(plot);
                while (queue.TryDequeue(out var p))
                {
                    if (regionPlots.Contains(p))
                    {
                        continue;
                    }
                    regionPlots.Add(p);

                    var ps = garden.At(p).GetNeighbours().Where(n => n.Value == crop && !regionPlots.Contains(n));
                    queue.EnqueueRange(ps.Select(pp => pp.Point));
                }

                var area = regionPlots.Count;

                var fences = new HashSet<Fence>();
                var fenceQueue = new Queue<Fence>();
                // Besök alla plot, för varje plot utforska fence i varje riktning som är giltig
                // Lagra i 'fences' vilka som har besökts, så att ingen räknas dubbelt (och tidig abort)
                // 'fencesQueue' behövs nog inte, utan utgå från 'regionPlots'
                var sideCount = 0;

                var allFences = regionPlots.SelectMany(p => garden.At(p).GetNeighbours(includeInvalid: true).Where(n => !n.IsValid() || n.Value != crop).Select(n => new Fence(p, GetSide(n - p)))).ToList();

                //var drawMap = new SparseGrid(' ');

                foreach (var fence in allFences)
                {
                    if (fences.Contains(fence))
                    {
                        continue;
                    }
                    sideCount++;
                    //WriteLine($"side {sideCount}");

                    fenceQueue.Enqueue(fence);
                    while (fenceQueue.TryDequeue(out var queueFence))
                    {
                        if (fences.Contains(queueFence))
                        {
                            continue;
                        }
                        //drawMap.SetAt(queueFence.Point, (char)('a' + sideCount - 1));
                        fences.Add(queueFence);

                        switch(queueFence.Side)
                        {
                            case 0:
                                {
                                    var point = queueFence.Point + new Point(-1, 0);
                                    var cornerPoint = point + new Point(0, -1);
                                    if (garden.Valid(point) && garden.At(point).Value == crop && (!garden.Valid(cornerPoint) || garden.ValueAt(cornerPoint) != crop))
                                    {
                                        fenceQueue.Enqueue(new Fence(point, fence.Side));
                                    }
                                    point = queueFence.Point + new Point(1, 0);
                                    cornerPoint = point + new Point(0, -1);
                                    if (garden.Valid(point) && garden.At(point).Value == crop && (!garden.Valid(cornerPoint) || garden.ValueAt(cornerPoint) != crop))
                                    {
                                        fenceQueue.Enqueue(new Fence(point, fence.Side));
                                    }
                                    break;
                                }
                            case 1:
                                {
                                    var point = queueFence.Point + new Point(0, -1);
                                    var cornerPoint = point + new Point(1, 0);
                                    if (garden.Valid(point) && garden.At(point).Value == crop && (!garden.Valid(cornerPoint) || garden.ValueAt(cornerPoint) != crop))
                                    {
                                        fenceQueue.Enqueue(new Fence(point, fence.Side));
                                    }
                                    point = queueFence.Point + new Point(0, 1);
                                    cornerPoint = point + new Point(1, 0);
                                    if (garden.Valid(point) && garden.At(point).Value == crop && (!garden.Valid(cornerPoint) || garden.ValueAt(cornerPoint) != crop))
                                    {
                                        fenceQueue.Enqueue(new Fence(point, fence.Side));
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    var point = queueFence.Point + new Point(-1, 0);
                                    var cornerPoint = point + new Point(0, 1);
                                    if (garden.Valid(point) && garden.At(point).Value == crop && (!garden.Valid(cornerPoint) || garden.ValueAt(cornerPoint) != crop))
                                    {
                                        fenceQueue.Enqueue(new Fence(point, fence.Side));
                                    }
                                    point = queueFence.Point + new Point(1, 0);
                                    cornerPoint = point + new Point(0, 1);
                                    if (garden.Valid(point) && garden.At(point).Value == crop && (!garden.Valid(cornerPoint) || garden.ValueAt(cornerPoint) != crop))
                                    {
                                        fenceQueue.Enqueue(new Fence(point, fence.Side));
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    var point = queueFence.Point + new Point(0, -1);
                                    var cornerPoint = point + new Point(-1, 0);
                                    if (garden.Valid(point) && garden.At(point).Value == crop && (!garden.Valid(cornerPoint) || garden.ValueAt(cornerPoint) != crop))
                                    {
                                        fenceQueue.Enqueue(new Fence(point, fence.Side));
                                    }
                                    point = queueFence.Point + new Point(0, 1);
                                    cornerPoint = point + new Point(-1, 0);
                                    if (garden.Valid(point) && garden.At(point).Value == crop && (!garden.Valid(cornerPoint) || garden.ValueAt(cornerPoint) != crop))
                                    {
                                        fenceQueue.Enqueue(new Fence(point, fence.Side));
                                    }
                                    break;
                                }
                        }
                    }
                    //Draw(drawMap);
                }
                //WriteLine($"crop={crop};area={area};sideCount={sideCount}");

                totalPrice += area * sideCount;

                visitedPlots.AddRange(regionPlots);
            }

            return totalPrice;
        }

        void Draw(Grid<char> grid)
        {
            for (int y = 0; y <= grid.Height; y++)
            {
                for (int x = 0; x <= grid.Width; x++)
                {
                    Write(grid.ValueAt(x, y));
                }
                WriteLine();
            }
            WriteLine();
        }

        int GetSide(Point point)
        {
            if (point.Y < 0)
            {
                return 0;
            }
            if (point.X > 0)
            {
                return 1;
            }
            if (point.Y > 0)
            {
                return 2;
            }
            if (point.X < 0)
            {
                return 3;
            }

            throw new InvalidOperationException();
        }

        //  0 
        // 3P1
        //  2
        record Fence(Point Point, int Side);
    }
}
