using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2024.Day13
{
    public record Machine(Point A, Point B, Point Prize);

    public class Day13 : BaseAoc<List<Machine>>
    {
        public override object PartOne(List<Machine> input)
        {
            long totalCost = 0;
            foreach (var machine in input)
            {
                //var maxA = Math.Min(machine.Prize.X / machine.A.X, machine.Prize.Y / machine.A.Y);
                //var maxB = Math.Min(machine.Prize.X / machine.B.Y, machine.Prize.Y / machine.B.Y);
                var maxA = 100;
                var maxB = 100;

                long? minCost = null;

                for (var a = 0; a <= maxA; a++)
                {
                    for (var b = 0; b <= maxB; b++)
                    {
                        if (machine.A * a + machine.B * b == machine.Prize)
                        {
                            var cost = a * 3 + b;
                            if (minCost is null || cost < minCost)
                            {
                                minCost = cost;
                            }
                        }
                    }
                }

                totalCost += minCost ?? 0;
            }

            return totalCost;
        }

        public override object PartTwo(List<Machine> input)
        {
            long totalCost = 0;
            foreach (var machine in input)
            {
                var prize = machine.Prize * 10000000000000;
                var maxA = Math.Min(prize.X / machine.A.X, prize.Y / machine.A.Y);
                var maxB = Math.Min(prize.X / machine.B.Y, prize.Y / machine.B.Y);

                long? minCost = null;

                for (var a = 0; a <= maxA; a++)
                {
                    for (var b = 0; b <= maxB; b++)
                    {
                        if (machine.A * a + machine.B * b == prize)
                        {
                            var cost = a * 3 + b;
                            if (minCost is null || cost < minCost)
                            {
                                minCost = cost;
                            }
                        }
                    }
                }

                totalCost += minCost ?? 0;
            }

            return totalCost;
        }

        protected override List<Machine> ParseInput(List<string> lines)
        {
            List<Machine> machines = [];
            Point A = new Point(0, 0);
            Point B = new Point(0, 0);
            Point Prize;
            foreach (var line in lines)
            {
                var match = Regex.Match(line, @"(?<type>(Button A|Button B|Prize)): X.(?<x>\d+), Y.(?<y>\d+)");
                if (match.Success)
                {
                    switch (match.Groups["type"].Value)
                    {
                        case "Button A":
                            A = new Point(long.Parse(match.Groups["x"].Value), long.Parse(match.Groups["y"].Value));
                            break;
                        case "Button B":
                            B = new Point(long.Parse(match.Groups["x"].Value), long.Parse(match.Groups["y"].Value));
                            break;
                        case "Prize":
                            Prize = new Point(long.Parse(match.Groups["x"].Value), long.Parse(match.Groups["y"].Value));
                            machines.Add(new Machine(A, B, Prize));
                            break;
                    }
                }
            }

            return machines;
        }
    }
}
