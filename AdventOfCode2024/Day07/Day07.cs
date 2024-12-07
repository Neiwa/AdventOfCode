using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2024.Day07
{
    public class Day07 : BaseAocV2
    {
        public override object PartOne(List<string> lines)
        {
            var equations = lines.Select(l =>
            {
                var match = Regex.Match(l, @"(?<test>\d+): (?<numbers>.*)");
                return new Equation(long.Parse(match.Groups["test"].Value), match.Groups["numbers"].Value.Split(' ').Select(long.Parse).ToList());
            });

            var sum = equations.Sum(e => IsValid(e.TestValue, e.Numbers) ? e.TestValue : 0);

            return sum;
        }

        public bool IsValid(long testValue, IEnumerable<long> remainingNumbers, bool partTwo = false, long currentSum = 0)
        {
            if (!remainingNumbers.Any())
            {
                return testValue == currentSum;
            }

            var addSum = currentSum + remainingNumbers.First();
            if (addSum <= testValue && IsValid(testValue, remainingNumbers.Skip(1), partTwo, addSum))
            {
                return true;
            }
            var mulSum = currentSum * remainingNumbers.First();
            if (mulSum <= testValue && IsValid(testValue, remainingNumbers.Skip(1), partTwo, mulSum))
            {
                return true;
            }

            if (partTwo)
            {
                if (long.TryParse($"{currentSum}{remainingNumbers.First()}", out var concSum) && concSum <= testValue && IsValid(testValue, remainingNumbers.Skip(1), partTwo, concSum))
                {
                    return true;
                }
            }

            return false;
        }

        public override object PartTwo(List<string> lines)
        {
            var equations = lines.Select(l =>
            {
                var match = Regex.Match(l, @"(?<test>\d+): (?<numbers>.*)");
                return new Equation(long.Parse(match.Groups["test"].Value), match.Groups["numbers"].Value.Split(' ').Select(long.Parse).ToList());
            });

            var sum = equations.Sum(e => IsValid(e.TestValue, e.Numbers, partTwo: true) ? e.TestValue : 0);

            return sum;
        }
    }

    record Equation(long TestValue, List<long> Numbers);
}
