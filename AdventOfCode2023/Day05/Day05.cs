using Core;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023.Day05
{
    public class Day05 : BaseAocV2
    {
        public override string PartOne(List<string> lines)
        {
            var maps = new Dictionary<string, (string Destination, List<RangeMap> Mappings)>();
            var seeds = lines[0].Substring(7).Split(' ').Select(s => long.Parse(s)).ToList();

            var currMapSource = "";
            var currMapDestination = "";
            var currMap = new List<RangeMap>();
            foreach (var line in lines.Skip(2))
            {
                if (string.Empty == line)
                {
                    if (currMapSource != string.Empty)
                    {
                        maps.Add(currMapSource, (currMapDestination, currMap));
                        currMap = new List<RangeMap>();
                    }
                    currMapSource = "";
                    continue;
                }

                var m = Regex.Match(line, @"(?<source>\w+)-to-(?<destination>\w+) map:");
                if (m.Success)
                {
                    currMapSource = m.Groups["source"].Value;
                    currMapDestination = m.Groups["destination"].Value;
                    continue;
                }

                currMap.Add(new RangeMap(line));
            }

            var curr = "seed";
            var end = "location";
            while (curr != end)
            {
                var map = maps[curr];
                var shifted = new List<long>();
                foreach (var mapper in map.Mappings)
                {
                    for (var i = seeds.Count - 1; i >= 0; i--)
                    {
                        var seed = seeds[i];
                        if(seed >= mapper.SourceStart && seed <= mapper.SourceEnd)
                        {
                            var shiftedValue = mapper.Convert(seed);
                            WriteLine($"{curr}: {seed} -> {shiftedValue}", ActionLevel.Trace);
                            shifted.Add(shiftedValue);
                            seeds.RemoveAt(i);
                            continue;
                        }
                    }
                }
                foreach (var seed in seeds)
                {
                    WriteLine($"{curr}: {seed} -> {seed}", ActionLevel.Trace);
                }
                shifted.AddRange(seeds);

                seeds = shifted;
                curr = map.Destination;
            }

            return seeds.Min().ToString();
        }

        public override string PartTwo(List<string> lines)
        {
            var maps = new Dictionary<string, (string Destination, List<RangeMap> Mappings)>();
            var seedRangeValues = lines[0].Substring(7).Split(' ').Select(s => long.Parse(s)).ToList();
            var seedRanges = new List<SeedRange>();

            for (var i = 0; i < seedRangeValues.Count; i+=2)
            {
                seedRanges.Add(new SeedRange(seedRangeValues[i], seedRangeValues[i+1]));
            }

            var currMapSource = "";
            var currMapDestination = "";
            var currMap = new List<RangeMap>();
            foreach (var line in lines.Skip(2))
            {
                if (string.Empty == line)
                {
                    if (currMapSource != string.Empty)
                    {
                        maps.Add(currMapSource, (currMapDestination, currMap));
                        currMap = new List<RangeMap>();
                    }
                    currMapSource = "";
                    continue;
                }

                var m = Regex.Match(line, @"(?<source>\w+)-to-(?<destination>\w+) map:");
                if (m.Success)
                {
                    currMapSource = m.Groups["source"].Value;
                    currMapDestination = m.Groups["destination"].Value;
                    continue;
                }

                currMap.Add(new RangeMap(line));
            }

            var curr = "seed";
            var end = "location";
            while (curr != end)
            {
                var map = maps[curr];
                var toShift = new Queue<SeedRange>(seedRanges);
                var shifted = new List<SeedRange>();
                while (toShift.Any())
                {
                    var range = toShift.Dequeue();
                    var hasShifted = false;
                    foreach(var mapper in map.Mappings)
                    {
                        if(range.End < mapper.SourceStart || range.Start > mapper.SourceEnd)
                        {
                            continue;
                        }else if(range.Start >= mapper.SourceStart && range.End <= mapper.SourceEnd)
                        {
                            var newRange = range.Shift(mapper.Shift);
                            WriteLine($"{curr}: {range.Start}-{range.End} -> {newRange.Start}-{newRange.End}", ActionLevel.Trace);
                            shifted.Add(newRange);
                            hasShifted = true;
                        }
                        else if(range.Start >= mapper.SourceStart)
                        {
                            WriteLine($"Split {range.Start}-{range.End} --- {mapper.SourceStart}-{mapper.SourceEnd}", ActionLevel.Trace);
                            var excess = range.Shorten(mapper.SourceEnd);
                            WriteLine($"{range.Start}-{range.End}", ActionLevel.Trace, 1);
                            WriteLine($"{excess.Start}-{excess.End}", ActionLevel.Trace, 1);
                            toShift.Enqueue(excess);
                            var newRange = range.Shift(mapper.Shift);
                            WriteLine($"{curr}: {range.Start}-{range.End} -> {newRange.Start}-{newRange.End}", ActionLevel.Trace);
                            shifted.Add(newRange);
                            hasShifted = true;
                        }
                        else if(range.End <= mapper.SourceEnd)
                        {
                            WriteLine($"Split {range.Start}-{range.End} --- {mapper.SourceStart}-{mapper.SourceEnd}", ActionLevel.Trace);
                            var excess = range.Shorten(mapper.SourceStart - 1);
                            WriteLine($"{range.Start}-{range.End}", ActionLevel.Trace, 1);
                            WriteLine($"{excess.Start}-{excess.End}", ActionLevel.Trace, 1);
                            toShift.Enqueue(range);
                            var newRange = excess.Shift(mapper.Shift);
                            WriteLine($"{curr}: {excess.Start}-{excess.End} -> {newRange.Start}-{newRange.End}", ActionLevel.Trace);
                            shifted.Add(newRange);
                            hasShifted = true;
                        }
                        else
                        {
                            // 2 3 4 5 6 7  ---  4 5
                            // 2 3 | 4 5 | 6 7


                            WriteLine($"Split {range.Start}-{range.End} --- {mapper.SourceStart}-{mapper.SourceEnd}", ActionLevel.Trace);
                            // rightExcess: 6 7
                            var rightExcess = range.Shorten(mapper.SourceEnd);
                            // leftExcess: 4 5
                            var leftExcess = range.Shorten(mapper.SourceStart - 1);
                            WriteLine($"{range.Start}-{range.End}", ActionLevel.Trace, 1);
                            WriteLine($"{leftExcess.Start}-{leftExcess.End}", ActionLevel.Trace, 1);
                            WriteLine($"{rightExcess.Start}-{rightExcess.End}", ActionLevel.Trace, 1);
                            toShift.Enqueue(rightExcess);
                            toShift.Enqueue(range);
                            var newRange = leftExcess.Shift(mapper.Shift);
                            WriteLine($"{curr}: {leftExcess.Start}-{leftExcess.End} -> {newRange.Start}-{newRange.End}", ActionLevel.Trace);
                            shifted.Add(newRange);
                            hasShifted = true;
                        }
                    }
                    if (!hasShifted)
                    {
                        shifted.Add(range);
                        WriteLine($"{curr}: {range.Start}-{range.End} -> {range.Start}-{range.End}", ActionLevel.Trace);
                    }
                }
                seedRanges = shifted;
                curr = map.Destination;
            }

            return seedRanges.Min(s => s.Start).ToString();
        }
    }

    public class SeedRange
    {
        public SeedRange(long start, long length)
        {
            Start = start;
            Length = length;
        }
        public long Length { get; private set; }
        public long Start { get; set; }
        public long End => Start + Length - 1;
        public SeedRange Shorten(long endIndex)
        {
            // 2 3 4 5
            // i = 3 -> 2 3 | 4 5
            var excess = new SeedRange(endIndex + 1, End - endIndex);
            Length = endIndex - Start + 1;
            return excess;
        }

        public SeedRange Shift(long shift)
        {
            return new SeedRange(Start + shift, Length);
        }
    }

    public class RangeMap
    {
        public RangeMap(string input)
        {
            var ss = input.Split(' ');
            DestinationStart = long.Parse(ss[0]);
            SourceStart = long.Parse(ss[1]);
            Range = long.Parse(ss[2]);
        }

        public long SourceStart { get; set; }
        public long Range { get; set; }

        public long SourceEnd => SourceStart + Range - 1;

        public long DestinationStart { get; set; }

        public long Convert(long value)
        {
            return value - SourceStart + DestinationStart;
        }

        public long Shift => DestinationStart - SourceStart;
    }
}
