using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2025.Day05;

public class Day05 : BaseAocV2
{
    public override object PartOne(List<string> lines)
    {
        var ranges = lines.TakeWhile(line => line.Length > 0).Select(line =>
        {
            var match = line.Split('-');
            return (Start: long.Parse(match[0]), End: long.Parse(match[1]));
        }).ToList();
        var ingredients = lines.Skip(ranges.Count + 1).Select(long.Parse);

        return ingredients.Count(ing => ranges.Any(range => range.Start <= ing && ing <= range.End));
    }

    public override object PartTwo(List<string> lines)
    {
        var ranges = lines.TakeWhile(line => line.Length > 0).Select(line =>
        {
            var match = line.Split('-');
            return (Start: long.Parse(match[0]), End: long.Parse(match[1]));
        }).OrderBy(range => range.Start).ThenByDescending(range => range.End).ToList();

        List<(long Start, long End)> newRanges = [];

        // A
        // |-----|
        // |--|
        
        // B
        // |---|
        //   |---|

        // C
        // |-----|
        //  |--|

        // D
        // |---|
        //     |--|

        // E
        // |---|
        //       |--|

        for (int i = 0; i < ranges.Count - 1; i++)
        {
            var range = ranges[i];

            var newEnd = range.End;
            for(int j = i + 1; j < ranges.Count; j++)
            {
                var range2 = ranges[j];

                // E
                if (range2.Start > newEnd)
                {
                    break;
                }
                // A, B, C, D
                if (range2.Start <= newEnd)
                {
                    newEnd = long.Max(newEnd, range2.End);
                    i++;
                }

            }
            newRanges.Add((range.Start, newEnd));
        }

        if (IsTrace)
        {
            newRanges.ForEach(range => WriteLine($"{range.Start}-{range.End}", ActionLevel.Trace));
        }

        return newRanges.Sum(r => r.End - r.Start + 1);
    }
}
