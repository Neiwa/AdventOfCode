using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022.Day20
{
    public class Day20 : BaseAocV2
    {
        class Number
        {
            public Number(long value)
            {
                Value = value;
            }
            public long Value { get; set; }

            public override int GetHashCode()
            {
                return Value.GetHashCode();
            }
        }

        List<Number> ParseInput(IEnumerable<string> lines)
        {
            return lines.Where(l => !string.IsNullOrEmpty(l)).Select(l => new Number(int.Parse(l))).ToList();
        }

        public override string PartOne(List<string> lines)
        {
            var input = ParseInput(lines);

            if (IsLevel(ActionLevel.Debug))
            {
                WriteLine(string.Join(", ", input.Select(n => n.Value)), ActionLevel.Trace);
            }

            var mixedList = Mix(input);

            if (IsLevel(ActionLevel.Debug))
            {
                WriteLine(string.Join(", ", mixedList.Select(n => n.Value)), ActionLevel.Debug);
            }

            var zeroIndex = mixedList.FindIndex(n => n.Value == 0);

            var a = mixedList[(zeroIndex + 1000) % mixedList.Count].Value;
            WriteLine($"1000th = {a}");
            var b = mixedList[(zeroIndex + 2000) % mixedList.Count].Value;
            WriteLine($"2000th = {b}");
            var c = mixedList[(zeroIndex + 3000) % mixedList.Count].Value;
            WriteLine($"3000th = {c}");
            return $"{a + b + c}";
        }

        private List<Number> Mix(List<Number> input, List<Number> mixList = null)
        {
            var workingList = mixList?.ToList() ?? input.ToList();
            foreach (var number in input)
            {
                int currentPos = workingList.IndexOf(number);

                workingList.RemoveAt(currentPos);
                if (IsLevel(ActionLevel.Trace))
                {
                    WriteLine($"Pop {number.Value}");
                    WriteLine(string.Join(", ", workingList.Select(n => n.Value)), ActionLevel.Trace);
                }

                int newPos = (int)((currentPos + number.Value) % workingList.Count);
                if (newPos < 0)
                {
                    newPos = workingList.Count + newPos;
                }

                if (IsLevel(ActionLevel.Trace))
                {
                    WriteLine($"Move {number.Value} from {currentPos} to {newPos}", ActionLevel.Trace);
                }
                workingList.Insert(newPos, number);

                if (IsLevel(ActionLevel.Trace))
                {
                    WriteLine(string.Join(", ", workingList.Select(n => n == number ? $"({n.Value})" : n.Value.ToString())), ActionLevel.Trace);
                }
            }

            return workingList;
        }

        public override string PartTwo(List<string> lines)
        {
            var input = ParseInput(lines);


            foreach (var number in input)
            {
                number.Value *= 811589153;
            }

            if (IsLevel(ActionLevel.Debug))
            {
                WriteLine(string.Join(", ", input.Select(n => n.Value)), ActionLevel.Trace);
            }

            List<Number> mixList = input;
            for (int i = 0; i < 10; i++)
            {
                mixList = Mix(input, mixList);

                if (IsLevel(ActionLevel.Debug))
                {
                    WriteLine(string.Join(", ", mixList.Select(n => n.Value)), ActionLevel.Debug);
                }
            }

            var zeroIndex = mixList.FindIndex(n => n.Value == 0);

            var a = mixList[(zeroIndex + 1000) % mixList.Count].Value;
            WriteLine($"1000th = {a}");
            var b = mixList[(zeroIndex + 2000) % mixList.Count].Value;
            WriteLine($"2000th = {b}");
            var c = mixList[(zeroIndex + 3000) % mixList.Count].Value;
            WriteLine($"3000th = {c}");
            return $"{a + b + c}";
        }
    }
}
