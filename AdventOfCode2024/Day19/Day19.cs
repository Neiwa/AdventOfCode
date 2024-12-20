using Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Day19
{
    public record Puzzle(List<string> Towels, List<string> Patterns);

    public class Day19 : BaseAoc<Puzzle>
    {
        public override object PartOne(Puzzle input)
        {
            var solver = new MemoizedSolver<string, bool>();

            var biggestTowel = input.Towels.Max(t => t.Length);

            bool fn(Func<string, bool> callback, string remainder)
            {
                if (remainder.Length == 0)
                {
                    return true;
                }

                for (int i = 1; i <= Math.Min(remainder.Length, biggestTowel); i++)
                {
                    if (input.Towels.Contains(remainder[..i]) && callback(remainder[i..]))
                    {
                        return true;
                    }
                }
                return false;
            }

            var count = input.Patterns.Count(pattern => solver.Solve(fn, pattern));

            return count;
        }

        public override object PartTwo(Puzzle input)
        {
            var solver = new MemoizedSolver<string, long>();

            var biggestTowel = input.Towels.Max(t => t.Length);

            long fn(Func<string, long> callback, string remainder)
            {
                var count = 0L;
                if (input.Towels.Contains(remainder))
                {
                    count++;
                }

                if (remainder.Length == 1)
                {
                    return count;
                }

                for (int i = 1; i <= Math.Min(remainder.Length, biggestTowel); i++)
                {
                    if (input.Towels.Contains(remainder[..i]))
                    {
                        count += callback(remainder[i..]);
                    }
                }
                return count;
            }

            var count = input.Patterns.Sum(pattern => solver.Solve(fn, pattern));

            return count;
        }

        protected override Puzzle ParseInput(List<string> lines)
        {
            var towels = lines[0].Split(',').Select(s => s.Trim()).ToList();

            var patterns = lines.Skip(2).ToList();

            return new Puzzle(towels, patterns);
        }
    }
}
