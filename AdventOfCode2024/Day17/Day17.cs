using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2024.Day17
{
    public class Program
    {
        public long A { get; set; }
        public long B { get; set; }
        public long C { get; set; }

        public List<int> Instructions { get; set; } = [];
    }

    public class Day17 : BaseAoc<Program>
    {
        public override object PartOne(Program input)
        {
            int pointer = 0;

            List<long> output = [];

            while (pointer < input.Instructions.Count)
            {
                var instruction = input.Instructions[pointer];
                var operand = input.Instructions[pointer + 1];

                var comboValue = operand switch
                {
                    <= 3 => operand,
                    4 => input.A,
                    5 => input.B,
                    6 => input.C,
                    7 => 99,
                    _ => throw new InvalidOperationException(),
                };

                switch (instruction)
                {
                    case 0:
                        input.A = input.A / (long)Math.Pow(2, comboValue);
                        break;
                    case 1:
                        input.B = input.B ^ operand;
                        break;
                    case 2:
                        input.B = comboValue % 8;
                        break;
                    case 3:
                        if (input.A != 0)
                        {
                            pointer = operand - 2;
                        }
                        break;
                    case 4:
                        input.B = input.B ^ input.C;
                        break;
                    case 5:
                        output.Add(comboValue % 8);
                        break;
                    case 6:
                        input.B = input.A / (long)Math.Pow(2, comboValue);
                        break;
                    case 7:
                        input.C = input.A / (long)Math.Pow(2, comboValue);
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                pointer += 2;
            }

            string result = string.Join(',', output);
            WriteLine($"Register A: {input.A}");
            WriteLine($"Register B: {input.B}");
            WriteLine($"Register C: {input.C}");
            WriteLine($"Output: {result}");


            return result;
        }

        public override async Task<object> PartTwoAsync(Program input)
        {
            System.Collections.Concurrent.ConcurrentBag<long> goalAs = [];

            string program = string.Join(',', input.Instructions);

            var aValues = Enumerations.Range(0, 1, long.MaxValue);

            CancellationTokenSource cts = new();

            await Parallel.ForEachAsync(aValues, new ParallelOptions
            {
                CancellationToken = cts.Token,
            }, async (long a, CancellationToken cancellationToken) =>
            {
                var newInput = new Program()
                { A = a, B = input.B, C = input.C, Instructions = input.Instructions
                };

                var result = PartOne(newInput) as string;
                if (result == program)
                {
                    goalAs.Add(a);
                    await cts.CancelAsync();
                }
            });

            
            return goalAs.First();
        }

        public override object PartTwo(Program input)
        {
            throw new NotImplementedException();
        }

        protected override Program ParseInput(List<string> lines)
        {
            var program = new Program();

            foreach (var line in lines)
            {
                var match = Regex.Match(line, @"(?<k>(Register .|Program)): (?<v>.+)");

                if (match.Success)
                {
                    switch (match.Groups["k"].Value)
                    {
                        case "Register A":
                            program.A = long.Parse(match.Groups["v"].Value);
                            break;
                        case "Register B":
                            program.B = long.Parse(match.Groups["v"].Value);
                            break;
                        case "Register C":
                            program.C = long.Parse(match.Groups["v"].Value);
                            break;
                        case "Program":
                            program.Instructions = match.Groups["v"].Value.Split(',').Select(int.Parse).ToList();
                            break;
                        default:
                            break;
                    }
                }
            }

            return program;
        }

        [Test]
        public void Part2Example()
        {
            var input = """
                Register A: 117440
                Register B: 0
                Register C: 0

                Program: 0,3,5,4,3,0
                """;

            var result = PartOne(ParseInput(input.Split(Environment.NewLine).ToList()));

            Assert.That(result, Is.EqualTo("0,3,5,4,3,0"));
        }
    }
}
