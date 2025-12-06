using Helpers.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2025.Day06;

public class Day06 : BaseAocV2
{
    public override object PartOne(List<string> lines)
    {
        var cells = lines.Select(line => 
            Regex.Matches(line, @"((?<cell>[\d*+]+)\s*)+")
            .SelectMany(match => match.Groups["cell"].Captures.Select(c => c.Value)).ToList())
            .ToList();

        var sum = 0L;

        for (int i = 0; i < cells[0].Count; i++)
        {
            switch (cells.Last()[i])
            {
                case "+":
                    sum += cells.Take(cells.Count - 1).Select(row => long.Parse(row[i])).Sum();
                    break;
                case "*":
                    sum += cells.Take(cells.Count - 1).Select(row => long.Parse(row[i])).Aggregate(1L, (agg, val) => agg * val);
                    break;
            }
        }

        return sum;
    }

    public override object PartTwo(List<string> lines)
    {
        var grid = lines.ToGrid();

        long sum = 0;

        char op = ' ';
        List<long> numbers = [];

        for (int x = 0; x <= grid.Width; x++)
        {
            bool blank = true;
            var val = "";
            if (x < grid.Width)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    var c = grid.ValueAt(x, y);
                    switch (c)
                    {
                        case (>= '0') and (<= '9'):
                            val += c;
                            blank = false;
                            break;
                        case '*':
                            op = c;
                            blank = false;
                            break;
                        case '+':
                            op = c;
                            blank = false;
                            break;
                        default:
                            break;
                    }
                }
            }

            if (blank)
            {
                if (IsDebug)
                {
                    WriteLine($"{op} {string.Join(' ', numbers)}");
                }

                sum += op switch  {
                    '+' => numbers.Sum(),
                    '*' => numbers.Aggregate(1L, (agg, val) => agg * val),
                    _ => 0
                };
                numbers = [];
            }
            else if (val.Length > 0)
            {
                numbers.Add(long.Parse(val));
            }
        }

        return sum;
    }
}
