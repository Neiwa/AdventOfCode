using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2025.Day10;

public record Machine(int LightsCount, uint Target, List<uint> Buttons, List<int> Joltage);

public class Day10 : BaseAoc<List<Machine>>
{
    public override object PartOne(List<Machine> input)
    {
        var sum = 0;

        foreach (var machine in input)
        {
            var state = 0u;

            var path = Search.AStar(state, machine.Target, s => long.MaxValue, (l, r) => l == r, s => machine.Buttons.Select(button => s ^ button));

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
        throw new NotImplementedException();
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
