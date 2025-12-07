using Helpers.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2025.Day07;

public class Day07 : BaseAocV2
{
    public override object PartOne(List<string> lines)
    {
        var map = lines.ToGrid();

        var start = map.First(cell => cell.Value == 'S');
        var direction = new Point(0, 1);
        var left = new Point(-1, 0);
        var right = new Point(1, 0);

        var splits = 0;

        List<GridCellReference<char>> beams = [start];

        for (int i = 1; i < map.Height; i++)
        {
            List<GridCellReference<char>> newBeams = [];

            foreach (var beam in beams)
            {
                var newBeam = beam + direction;
                if (newBeam.Value == '^')
                {
                    newBeams.AddRange([newBeam + left, newBeam + right]);
                    splits++;
                }
                else
                {
                    newBeams.Add(newBeam);
                }
            }

            beams = newBeams.DistinctBy(b => b.Point).ToList();
        }


        return splits;
    }

    public override object PartTwo(List<string> lines)
    {
        var map = lines.ToGrid();

        var start = map.First(cell => cell.Value == 'S');
        var direction = new Point(0, 1);
        var left = new Point(-1, 0);
        var right = new Point(1, 0);

        List<(GridCellReference<char>, long)> beams = [(start, 1)];

        for (int i = 1; i < map.Height; i++)
        {
            List<(GridCellReference<char>, long)> newBeams = [];

            foreach (var beam in beams)
            {
                var newBeam = beam.Item1 + direction;
                if (newBeam.Value == '^')
                {
                    newBeams.AddRange([(newBeam + left, beam.Item2), (newBeam + right, beam.Item2)]);
                }
                else
                {
                    newBeams.Add((newBeam, beam.Item2));
                }
            }

            beams = newBeams.GroupBy(b => b.Item1.Point).Select(g => (g.First().Item1, g.Sum(b => b.Item2))).ToList();
        }


        return beams.Sum(b => b.Item2);
    }
}
