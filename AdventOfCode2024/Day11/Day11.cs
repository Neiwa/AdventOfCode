using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Day11
{
    public class Day11 : BaseAocV2
    {
        public override object PartOne(List<string> lines)
        {
            var stones = lines[0].Split(' ').Select(long.Parse).ToList();

            
            for (int i = 0; i < 25; i++)
            {
                List<long> nextGen = [];

                foreach (var stone in stones)
                {
                    if (stone == 0)
                    {
                        nextGen.Add(1);
                    }
                    else if ((int)Math.Log10(stone) % 2 == 1)
                    {
                        var str = stone.ToString();
                        nextGen.Add(long.Parse(str[..(str.Length / 2)]));
                        nextGen.Add(long.Parse(str[(str.Length / 2)..]));
                    }
                    else
                    {
                        nextGen.Add(stone * 2024);
                    }
                }

                stones = nextGen;

                WriteLine(string.Join(" ", stones), level: ActionLevel.Trace);
            }

            return stones.Count;
        }

        class Node(long stone)
        {
            public long Stone { get; set; } = stone;

            public List<Node>? Next { get; set; }
        }

        record Input(long Stone, int Gen);

        record Result(long Count);

        public override object PartTwo(List<string> lines)
        {
            var stones = lines[0].Split(' ').Select(long.Parse).ToList();

            var memo = new Dictionary<Input, Result>();

            Result memoFn(Input input)
            {
                if (memo.TryGetValue(input, out var memoResult))
                {
                    return memoResult;
                }

                if (input.Gen == 0)
                {
                    return new Result(1);
                }

                Result result;

                if (input.Stone == 0)
                {
                    result = memoFn(new Input(1, input.Gen - 1));
                }
                else if ((int)Math.Log10(input.Stone) % 2 == 1)
                {
                    var str = input.Stone.ToString();
                    var leftStone = long.Parse(str[..(str.Length / 2)]);
                    var rightStone = long.Parse(str[(str.Length / 2)..]);

                    result = new Result(memoFn(new Input(leftStone, input.Gen - 1)).Count +
                        memoFn(new Input(rightStone, input.Gen - 1)).Count);
                }
                else
                {
                    result = memoFn(new Input(input.Stone * 2024, input.Gen - 1));
                }

                memo[input] = result;
                return result;
            }

            return stones.Sum(s => memoFn(new Input(s, 75)).Count);
        }
    }
}
