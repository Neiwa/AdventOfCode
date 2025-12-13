using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2025.Day11;

public class Day11 : BaseAocV2
{
    public override object PartOne(List<string> lines)
    {
        ValueCreationDictionary<string, List<string>> racks = [];
        foreach (var line in lines)
        {
            var match = Regex.Match(line, @"(?<label>\w+):( (?<output>\w+))+");
            racks[match.Groups["label"].Value] = match.Groups["output"].Captures.Select(c => c.Value).ToList();
        }

        var solver = new MemoizedSolver<string, int>();

        var res = solver.Solve((callback, input) =>
        {
            var rack = racks[input];
            if (rack.Count == 1 && rack[0] == "out")
            {
                return 1;
            }

            return rack.Sum(label => callback(label));
        }, "you");

        return res;
    }

    [Flags]
    public enum RackType
    {
        Unspecified = 0b00,
        Dac = 0b01,
        Fft = 0b10,
        DacFft = 0b11
    }

    public record struct Result(long Count, RackType RackTypeVisited);

    public override object PartTwo(List<string> lines)
    {
        ValueCreationDictionary<string, List<string>> racks = [];
        foreach (var line in lines)
        {
            var match = Regex.Match(line, @"(?<label>\w+):( (?<output>\w+))+");
            racks[match.Groups["label"].Value] = match.Groups["output"].Captures.Select(c => c.Value).ToList();
        }

        var solver = new MemoizedSolver<string, Result>();

        var res = solver.Solve((callback, input) =>
        {
            RackType rackTypeVisited = RackType.Unspecified;
            if (input == "dac")
            {
                rackTypeVisited |= RackType.Dac;
            }
            if (input == "fft")
            {
                rackTypeVisited |= RackType.Fft;
            }

            var rack = racks[input];
            if (rack.Count == 1 && rack[0] == "out")
            {
                return new Result(1, rackTypeVisited);
            }

            var results = rack.Select(label => callback(label)).ToList();

            if (results.Count == 0)
            {
                Debugger.Break();
                return new Result(0, RackType.Unspecified);
            }

            var groupedResults = results.GroupBy(r => r.RackTypeVisited).ToDictionary(g => g.Key);

            switch (rackTypeVisited)
            {
                case RackType.Dac:
                    {
                        if (groupedResults.TryGetValue(RackType.DacFft, out var value))
                        {
                            return new Result(value.Sum(r => r.Count), RackType.DacFft);
                        }
                    }
                    {
                        if (groupedResults.TryGetValue(RackType.Dac, out var value))
                        {
                            return new Result(value.Sum(r => r.Count), RackType.Dac);
                        }
                    }
                    {
                        if (groupedResults.TryGetValue(RackType.Fft, out var value))
                        {
                            return new Result(value.Sum(r => r.Count), RackType.DacFft);
                        }
                    }
                    return new Result(groupedResults[RackType.Unspecified].Sum(r => r.Count), RackType.Dac);
                case RackType.Fft:
                    {
                        if (groupedResults.TryGetValue(RackType.DacFft, out var value))
                        {
                            return new Result(value.Sum(r => r.Count), RackType.DacFft);
                        }
                    }
                    {
                        if (groupedResults.TryGetValue(RackType.Dac, out var value))
                        {
                            return new Result(value.Sum(r => r.Count), RackType.DacFft);
                        }
                    }
                    {
                        if (groupedResults.TryGetValue(RackType.Fft, out var value))
                        {
                            return new Result(value.Sum(r => r.Count), RackType.Fft);
                        }
                    }
                    return new Result(groupedResults[RackType.Unspecified].Sum(r => r.Count), RackType.Fft);
                default:
                    {
                        if (groupedResults.TryGetValue(RackType.DacFft, out var value))
                        {
                            return new Result(value.Sum(r => r.Count), RackType.DacFft);
                        }
                    }
                    {
                        if (groupedResults.TryGetValue(RackType.Dac, out var value))
                        {
                            return new Result(value.Sum(r => r.Count), RackType.Dac);
                        }
                    }
                    {
                        if (groupedResults.TryGetValue(RackType.Fft, out var value))
                        {
                            return new Result(value.Sum(r => r.Count), RackType.Fft);
                        }
                    }
                    return new Result(groupedResults[RackType.Unspecified].Sum(r => r.Count), RackType.Unspecified);
            }
        }, "svr");

        return res;
    }
}
