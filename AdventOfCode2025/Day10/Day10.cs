using Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2025.Day10;

public record Machine(int LightsCount, uint Target, List<uint> LightButtons, List<int[]> JoltageButtons, int[] Joltage)
{
    public static int[] PushButton(int[] button, int[] state, int times = 1)
    {
        var newState = (int[])state.Clone();
        foreach (var i in button)
        {
            newState[i] += times;
        }
        return newState;
    }
}

public class Day10 : BaseAoc<List<Machine>>
{
    public override object PartOne(List<Machine> input)
    {
        var sum = 0;

        foreach (var machine in input)
        {
            var state = 0u;

            var path = Search.AStar(state, machine.Target, s => long.MaxValue, (l, r) => l == r, s => machine.LightButtons.Select(button => s ^ button));

            if (IsTrace)
            {
                var format = $"B{machine.LightsCount}";
                for (int i = 1; i < path.Count; i++)
                {
                    var step = path[i];
                    var diff = path[i] ^ path[i - 1];
                    WriteLine($"Pressed {diff.ToString(format)} reached {step.ToString(format)}");
                }
            }

            sum += path.Count - 1;
        }

        return sum;
    }

    public override object PartTwo(List<Machine> input)
    {
        var sum = 0;

        foreach(var machine in input)
        {
            var maxPresses = machine.JoltageButtons.Select((button, i) => (Index: i, Button: button, Max: button.Select(counter => machine.Joltage[counter]).Min())).ToList();

            var start = new int[machine.JoltageButtons.Count];

            var res = Search.AStar(
                start,
                s => Enumerable.Range(0, s.Length)
                        .Aggregate(start, (agg, i) => Machine.PushButton(maxPresses[i].Button, agg, s[i]))
                        .SequenceEqual(machine.Joltage),
                _ => 1_000,
                (l, r) => l == r,
                s => Enumerable.Range(0, machine.JoltageButtons.Count).Select(i => s.WithElement(i, s[i] + 1)),
                equalityComparer: EqualityComparers.IntArrayEqualityComparer);

            if (res.Count == 0)
            {
                throw new Exception();
            }

            sum += res.Last().Sum();

        }

        return sum;
    }

    public object PartTwo1(List<Machine> input)
    {
        var sum = 0;


        var equalityComparer = EqualityComparer<int[]>.Create((x, y) =>
        {
            if ((x == null && y == null) || ReferenceEquals(x, y))
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }

            return x.SequenceEqual(y);
        }, arr =>
        {
            int hash = arr.Length;
            foreach (var val in arr)
            {
                hash = unchecked(hash * 314159 + val);
            }
            return hash;
        });

        Parallel.ForEach(input, machine =>
        {
            int[] state = new int[machine.Joltage.Length];

            var path = Search.AStar(
                state,
                machine.Joltage,
                s => Heuristic(s, machine.Joltage),
                Compare,
                s => machine.JoltageButtons
                    .Select(button => Machine.PushButton(button, s))
                    .Where(s => Heuristic(s, machine.Joltage) < 1_000_000),
                equalityComparer: equalityComparer);

            //if (IsTrace)
            //{
            //    var format = $"B{machine.LightsCount}";
            //    for (int i = 1; i < path.Count; i++)
            //    {
            //        var step = path[i];
            //        var diff = path[i] ^ path[i - 1];
            //        WriteLine($"Pressed {diff.ToString(format)} reached {step.ToString(format)}");
            //    }
            //}

            Interlocked.Add(ref sum, path.Count - 1);
        });

        return sum;
    }

    private static long Heuristic(int[] state, int[] target)
    {
        var stateMax = 0;
        var targetMax = 0;

        for (int i = 0; i < state.Length; i++)
        {
            if (state[i] > target[i])
            {
                return 1_000_000;
            }
            stateMax = Math.Max(state[i], stateMax);
            targetMax = Math.Max(target[i], targetMax);
        }

        return targetMax - stateMax;
    }

    private static bool Compare(int[] l, int[] r)
    {
        for (int i = 0; i < l.Length; i++)
        {
            if (l[i] != r[i])
            {
                return false;
            }
        }

        return true;
    }

    protected override List<Machine> ParseInput(List<string> lines)
    {
        return [.. lines.Select(line =>
        {
            var match = Regex.Match(line, @"\[(?<target>[#.]+)\] (\((?<button>(\d+,?)+)\) )+{(?<joltage>(\d+,?)+)}");
            if (match.Success)
            {
                var target = match.Groups["target"].Value;
                uint t = 0;
                for (var i = 0; i < target.Length; i++)
                {
                    t |= (target[i] == '#' ? 1u : 0u) << i;
                }

                return new Machine(
                    target.Length,
                    t,
                    [.. match.Groups["button"].Captures.Select(c => ToButton(c.Value.Split(',').Select(int.Parse)))],
                    [.. match.Groups["button"].Captures.Select(c => c.Value.Split(',').Select(int.Parse).ToArray())],
                    [.. match.Groups["joltage"].Value.Split(',').Select(int.Parse)]);
            }
            throw new InvalidOperationException();
        })];
    }

    private static uint ToButton(IEnumerable<int> button)
    {
        uint init = 0u;

        foreach (var item in button)
        {
            init |= 1u << item;
        }

        return init;
    }
}
