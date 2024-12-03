using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2024.Day03
{
    public class Day03 : BaseAocV2
    {
        public override object PartOne(List<string> lines)
        {
            long sum = 0;
            foreach (string line in lines)
            {
                var matches = Regex.Matches(line, @"mul\((?<x>\d{1,3}),(?<y>\d{1,3})\)");
                foreach (Match match in matches)
                {
                    WriteLine(match.Value);
                    sum += long.Parse(match.Groups["x"].Value) * long.Parse(match.Groups["y"].Value);
                }
            }

            return sum;
        }

        public override object PartTwo(List<string> lines)
        {
            long sum = 0;
            bool enabled = true;
            foreach (string line in lines)
            {
                var matches = Regex.Matches(line, @"(?<i>mul|do|don't)\((|(?<x>\d{1,3}),(?<y>\d{1,3}))\)");
                foreach (Match match in matches)
                {
                    switch (match.Groups["i"].Value)
                    {
                        case "do":
                            enabled = true;
                            break;
                        case "don't":
                            enabled = false;
                            break;
                        case "mul":
                            if (enabled)
                            {
                                sum += long.Parse(match.Groups["x"].Value) * long.Parse(match.Groups["y"].Value);
                            }

                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                }
            }

            return sum;
        }
    }
}
