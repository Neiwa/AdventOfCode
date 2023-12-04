using Core;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023.Day04
{
    public class Day04 : BaseAocV2
    {
        public override string PartOne(List<string> lines)
        {
            var sum = 0;

            foreach (var line in lines)
            {
                var matches = 0;
                var m = Regex.Match(line, @"Card\s+\d+: (?<win>[0-9 ]+) \| (?<have>[0-9 ]+)");
                var wins = m.Groups["win"].Value.Split(' ').Select(s => s.Trim()).Where(s => s != string.Empty);
                var haves = m.Groups["have"].Value.Split(" ").Select(s => s.Trim()).Where(s => s != string.Empty);
                foreach (var have in haves)
                {
                    if (wins.Contains(have))
                    {
                        matches++;
                    }
                }
                var points = (int)Math.Pow(2, matches - 1);
                if (matches > 0)
                {
                    sum += points;
                }

                WriteLine($"{line} => {points}", ActionLevel.Trace);
            }

            return sum.ToString();
        }

        public override string PartTwo(List<string> lines)
        {
            var sum = 0;
            var cards = Enumerable.Repeat(1, lines.Count).ToList();

            for(var i = 0; i < lines.Count; i++)
            {
                var line = lines[i];

                var matches = 0;
                var m = Regex.Match(line, @"Card\s+(?<card>\d+): (?<win>[0-9 ]+) \| (?<have>[0-9 ]+)");
                var wins = m.Groups["win"].Value.Split(' ').Select(s => s.Trim()).Where(s => s != string.Empty);
                var haves = m.Groups["have"].Value.Split(" ").Select(s => s.Trim()).Where(s => s != string.Empty);
                foreach (var have in haves)
                {
                    if (wins.Contains(have))
                    {
                        matches++;
                    }
                }

                for (int j = 0; j < matches; j++)
                {
                    cards[i + j + 1] += cards[i];
                }
            }

            sum = cards.Sum();

            return sum.ToString();
        }
    }
}
