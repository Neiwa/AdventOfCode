using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023.Day09
{
    public class Day09 : BaseAocV2
    {
        IEnumerable<long> GetDiffs(IEnumerable<long> values)
        {
            return values.Zip(values.Skip(1), (l, r) => r - l);
        }

        public override string PartOne(List<string> lines)
        {
            var histories = lines.Select(l => l.Split(' ').Select(long.Parse).ToList());
            long sum = 0;

            foreach (var history in histories)
            {
                List<long> lastValues = new List<long> { history.Last() };
                IEnumerable<long> diffs = history;
                do
                {
                    diffs = GetDiffs(diffs).ToList();
                    lastValues.Add(diffs.Last());
                } while (diffs.Any(i => i != 0));

                var value = 0L;
                for (var i = lastValues.Count - 2; i >= 0; i--)
                {
                    value = lastValues[i] + value;
                }

                sum += value;
            }

            return sum.ToString();
        }

        public override string PartTwo(List<string> lines)
        {
            var histories = lines.Select(l => l.Split(' ').Select(long.Parse).ToList());
            long sum = 0;

            foreach (var history in histories)
            {
                List<long> firstValues = new List<long> { history.First() };
                IEnumerable<long> diffs = history;
                do
                {
                    diffs = GetDiffs(diffs).ToList();
                    firstValues.Add(diffs.First());
                } while (diffs.Any(i => i != 0));

                var value = 0L;
                for (var i = firstValues.Count - 2; i >= 0; i--)
                {
                    value = firstValues[i] - value;
                }

                sum += value;
            }

            return sum.ToString();
        }
    }
}
