using Helpers.Extensions;
using Helpers.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
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
            throw new NotImplementedException();
        }
    }
}
