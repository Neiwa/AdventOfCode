using Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                var solution = Solve1(machine);
                WriteLine($"cost={solution.Cost};a={solution.A};b={solution.B}");
                totalCost += solution.Cost;
            }

            return totalCost;
        }

        public override object PartTwo(List<Machine> input)
        {
            long totalCost = 0;
            int index = 0;
            foreach (var machine in input)
            {
                var solution = Solve3(machine, prizeAdjust: 10000000000000);

                WriteLine($"cost={solution.Cost};a={solution.A};b={solution.B}");
                totalCost += solution.Cost;
                index++;
            }

            return totalCost;
        }

        public (long Cost, long? A, long? B) Solve3(Machine machine, long prizeAdjust = 0, long? maxClick = null)
        {
            var prize = machine.Prize + new Point(prizeAdjust, prizeAdjust);

            var a = (prize.X * machine.B.Y - machine.B.X * prize.Y) / (machine.A.X * machine.B.Y - machine.B.X * machine.A.Y);

            var b = (machine.A.X * prize.Y - prize.X * machine.A.Y) / (machine.A.X * machine.B.Y - machine.B.X * machine.A.Y);

            if (machine.A * a + machine.B * b == prize)
            {
                return (a * 3 + b, a, b);
            }
            else
            {
                return (0, null, null);
            }
        }

        public (long Cost, long? A, long? B) Solve2(Machine machine, long prizeAdjust = 0, long? maxClick = null)
        {
            var prize = machine.Prize + new Point(prizeAdjust, prizeAdjust);

            var maxA = Math.Min(prize.X / machine.A.X, prize.Y / machine.A.Y);
            var maxB = Math.Min(prize.X / machine.B.X, prize.Y / machine.B.Y);

            if (maxClick is not null)
            {
                maxA = Math.Min(maxA, maxClick.Value);
                maxB = Math.Min(maxB, maxClick.Value);
            }

            long? minCost = null;
            long? solA = null;
            long? solB = null;

            for (var a = 0; a <= maxA; a++)
            {
                var aPoint = machine.A * a;

                var bRemain = prize - aPoint;

                if (bRemain.X % machine.B.X == 0 && bRemain.Y % machine.B.Y == 0)
                {
                    var bX = bRemain.X / machine.B.X;
                    var bY = bRemain.Y / machine.B.Y;
                    if (bX != bY)
                    {
                        continue;
                    }

                    var cost = a * 3 + bX;
                    if (minCost is null || cost < minCost)
                    {
                        minCost = cost;
                        solA = a;
                        solB = bX;
                    }
                }
            }

            return (minCost ?? 0, solA, solB);
        }

        public (long Cost, long? A, long? B) Solve1(Machine machine, long prizeAdjust = 0)
        {
            var prize = machine.Prize + new Point(prizeAdjust, prizeAdjust);

            var maxA = Math.Min(Math.Min(prize.X / machine.A.X, prize.Y / machine.A.Y), 100);
            var maxB = Math.Min(Math.Min(prize.X / machine.B.X, prize.Y / machine.B.Y), 100);

            long? minCost = null;
            long? solA = null;
            long? solB = null;

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
                            solA = a;
                            solB = b;
                        }
                    }
                }
            }

            return (minCost ?? 0, solA, solB);
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
