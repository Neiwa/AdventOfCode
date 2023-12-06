using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023.Day06
{
    public class Day06 : BaseAocV2
    {
        public override string PartOne(List<string> lines)
        {
            // T = 7 time
            // D = 9 distance
            // p: pressed time
            // p * (T-p) > D
            // T - p > D/p
            // T > D/p + p

            var times = lines[0].Substring(10).Split(' ').Where(l => l != string.Empty).Select(int.Parse).ToList();
            var distances = lines[1].Substring(10).Split(' ').Where(l => l != string.Empty).Select(int.Parse).ToList();

            var sum = 1;
            for (int i = 0; i < times.Count; i++)
            {
                var count = 0;
                for (int p = 1; p < times[i]; p++)
                {
                    if (p * (times[i] - p) > distances[i])
                    {
                        count++;
                    }
                }
                sum *= count;
            }

            return sum.ToString();
        }

        public override string PartTwo(List<string> lines)
        {

            var time = long.Parse(lines[0].Substring(10).Replace(" ",""));
            var distance = long.Parse(lines[1].Substring(10).Replace(" ", ""));

            long count = 0;
            for (long p = 1; p < time; p++)
            {
                if (p * (time - p) > distance)
                {
                    count++;
                }

                if(p % 10_000 == 0)
                {
                    WriteLine($"p={p} of {time}");
                }
            }

            return count.ToString();
        }
    }
}
