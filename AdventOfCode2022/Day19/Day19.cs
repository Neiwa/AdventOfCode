using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2022.Day19
{
    public class Day19 : BaseAoc
    {
        class Blueprint
        {
            public int Id { get; set; }
            public Dictionary<string, (int Ore, int Clay, int Obsidian)> Robots { get; set; }
            public int MaxGeodes { get; set; }
        }

        List<Blueprint> ParseInput(List<string> lines)
        {
            int Parse(string value)
            {
                if (string.IsNullOrEmpty(value))
                {
                    return 0;
                }
                return int.Parse(value);
            }

            List<Blueprint> blueprints = new();
            foreach (var line in lines)
            {
                var match = Regex.Match(line, @"Blueprint (?<id>\d+): Each ore robot costs (?<oo>\d+) ore. Each clay robot costs (?<co>\d+) ore. Each obsidian robot costs (?<bo>\d+) ore and (?<bc>\d+) clay. Each geode robot costs (?<go>\d+) ore and (?<gb>\d+) obsidian.");
                if (match.Success)
                {
                    blueprints.Add(new Blueprint
                    {
                        Id = int.Parse(match.Groups["id"].Value),
                        Robots = new() {
                            { "ore", (Parse(match.Groups["oo"].Value), Parse(match.Groups["oc"].Value), Parse(match.Groups["ob"].Value)) },
                            { "clay", (Parse(match.Groups["co"].Value), Parse(match.Groups["cc"].Value), Parse(match.Groups["cb"].Value)) },
                            { "obsidian", (Parse(match.Groups["bo"].Value), Parse(match.Groups["bc"].Value), Parse(match.Groups["bb"].Value)) },
                            { "geode", (Parse(match.Groups["go"].Value), Parse(match.Groups["gc"].Value), Parse(match.Groups["gb"].Value)) },
                        }
                    });
                }
            }

            return blueprints;
        }

        public override string PartOne(List<string> lines)
        {
            var blueprints = ParseInput(lines);
            WriteLine($"{blueprints.Count} blueprints");

            foreach (var blueprint in blueprints)
            {
                
            }

            return "";
        }

        public override string PartTwo(List<string> lines)
        {
            throw new NotImplementedException();
        }
    }
}
