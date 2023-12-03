using Core;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023.Day02
{
    public class Day02 : BaseAocV2
    {
        public override string PartOne(List<string> lines)
        {
            var max = new Dictionary<string, int>()
            {
                {"red", 12 },
                {"green", 13 },
                {"blue", 14 }
            };
            var sum = 0;
            foreach (var line in lines)
            {
                var match = Regex.Match(line, @"Game (?<game>\d+): (?<value>.*)");
                var game = int.Parse(match.Groups["game"].Value);
                bool validGame = true;

                var record = match.Groups["value"].Value;
                foreach(var set in record.Split(";"))
                {
                    foreach(Match m in Regex.Matches(set, @"(?<n>\d+) (?<c>\w+)"))
                    {
                        var count = int.Parse(m.Groups["n"].Value);
                        var color = m.Groups["c"].Value;
                        if (max[color] < count)
                        {
                            validGame = false;
                            break;
                        }
                    }
                    if(!validGame) { break; }
                }
                if(!validGame) { continue; }
                sum += game;
            }

            return sum.ToString();
        }

        public override string PartTwo(List<string> lines)
        {
            var sum = 0;
            foreach (var line in lines)
            {
                var min = new ValueCreationDictionary<string, int>();
                var match = Regex.Match(line, @"Game (?<game>\d+): (?<value>.*)");
                var game = int.Parse(match.Groups["game"].Value);

                var record = match.Groups["value"].Value;
                foreach (var set in record.Split(";"))
                {
                    foreach (Match m in Regex.Matches(set, @"(?<n>\d+) (?<c>\w+)"))
                    {
                        var count = int.Parse(m.Groups["n"].Value);
                        var color = m.Groups["c"].Value;
                        if (min[color] < count)
                        {
                            min[color] = count;
                        }
                    }
                }
                sum += min["red"] * min["blue"] * min["green"] ;
            }

            return sum.ToString();
        }
    }
}
