using Helpers.Extensions;
using Helpers.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2025.Day09;

public class Day09 : BaseAocV2
{
    public override object PartOne(List<string> lines)
    {
        var points = lines.Select(line => Point.Parse(line));

        var areas = points.SelfJoin((l, r) => (1 + Math.Abs(l.X - r.X)) * (1 + Math.Abs(l.Y - r.Y)));

        return areas.Max();
    }

    public override object PartTwo(List<string> lines)
    {
        var grid = new SparseGrid('.');
        var points = lines.Select(line => grid.At(Point.Parse(line))).ToList();
        points.Add(points[0]);

        points[0].Value = '#';

        for (int index = 1; index < points.Count; index++)
        {
            points[index].Value = '#';
            var dir = (points[index] - points[index - 1]).Point.Normalized();
            var curr = points[index - 1];
            while (curr != points[index])
            {
                curr += dir;
                if (curr.Value != '#')
                {
                    curr.Value = 'X';
                }
            }
        }

        grid.At(grid.MaxX + 1, grid.MaxY + 1).Value = '.';

        FloodFill(grid.First(cell => cell.Value == '.'), 'O', '.');

        Draw(grid);


        //var areas = points.SelfJoin((l, r) => (l, r, area: (1 + Math.Abs(l.X - r.X)) * (1 + Math.Abs(l.Y - r.Y)))).OrderByDescending(a => a.area);
        //foreach (var (l, r, area) in areas)
        //{
            
        //}

        throw new NotImplementedException();
    }

    public void FloodFill(GridCellReference<char> start, char v, char replace)
    {
        var queue = new Queue<GridCellReference<char>>();
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var cell = queue.Dequeue();
            if (cell.Value == replace)
            {
                cell.Value = v;
            }
            queue.EnqueueRange(cell.GetNeighbours().Where(c => c.Value == replace));
        }
    }

    public object PartTwo2(List<string> ilines)
    {
        var points = ilines.Select(line => Point.Parse(line)).ToList();

        List<Line> lines = [];
        lines.Add(new(points[^1], points[0]));

        for (int i = 1; i < points.Count; i++)
        {
            lines.Add(new(points[i - 1], points[i]));
        }

        return lines.SelfJoin((l, r) => (l, r)).Any(t => Intersects(t.l, t.r));
    }

    public void Draw(Grid<char> grid)
    {
        if (Level > ActionLevel.Debug)
        {
            return;
        }

        for (int y = 0; y <= grid.Height + 2; y++)
        {
            for (int x = 0; x <= grid.Width + 2; x++)
            {
                Write(grid.ValueAt(x, y));
            }
            WriteLine();
        }
    }

    public record Line(Point Point1, Point Point2)
    {
        public Axis Axis => Point1.X == Point2.X ? Axis.Y : Axis.X;
        public long MinX => Math.Min(Point1.X, Point2.X);
        public long MaxX => Math.Max(Point1.X, Point2.X);
        public long MinY => Math.Min(Point1.Y, Point2.Y);
        public long MaxY => Math.Max(Point1.Y, Point2.Y);
    };
    public enum Axis
    {
        X, Y
    }

    public bool Intersects(Line line1, Line line2)
    {
        // ---
        // ---

        // | |
        // | |
        if (line1.Axis == line2.Axis)
        {
            return false;
        }

        //  |
        // -+-
        //  |
        switch (line1.Axis)
        {
            case Axis.X:
                {
                    if (line2.MinX < line1.Point1.X && line1.Point1.X < line2.MaxX &&
                        line1.MinY < line2.Point1.Y && line2.Point1.Y < line1.MaxY)
                    {
                        return true;
                    }
                    break;
                }
            case Axis.Y:
                {
                    if (line1.MinX < line2.Point1.X && line2.Point1.X < line1.MaxX &&
                        line2.MinY < line1.Point1.Y && line1.Point1.Y < line2.MaxY)
                    {
                        return true;
                    }
                    break;
                }
        }

        // |    ---
        // |

        return false;
    }
}
