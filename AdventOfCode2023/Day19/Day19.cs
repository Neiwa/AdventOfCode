using Core;
using System.Data;
using System.Text.RegularExpressions;

namespace AdventOfCode2023.Day19
{
    public class Day19 : BaseAocV2
    {
        public record Rule(string Field, string Operator, int Value, string OnAccept);
        public record Workflow(string Name, List<Rule> Rules);
        public record Part(int X, int M, int A, int S);

        public int ParseWithFallback(string s, int fallback)
        {
            if (int.TryParse(s, out var result))
            {
                return result;
            }
            return fallback;
        }

        public override string PartOne(List<string> lines)
        {
            Dictionary<string, Workflow> workflows = lines.TakeWhile(l => !string.IsNullOrEmpty(l)).Select(l =>
            {
                var m = Regex.Match(l, @"(?<name>\w+){(?<rules>.+)}");
                var m2 = Regex.Matches(m.Groups["rules"].Value, @"((?<field>[xmas])(?<op>[<>])(?<value>\d+):(?<target>\w+)|(?<target>\w+)),?");

                var rules = m2.Select(m => new Rule(m.Groups["field"].Value, m.Groups["op"].Value, ParseWithFallback(m.Groups["value"].Value, 0), m.Groups["target"].Value)).ToList();

                return new Workflow(m.Groups["name"].Value, rules);
            }).ToDictionary(w => w.Name);

            var parts = lines.Reverse<string>().TakeWhile(l => !string.IsNullOrEmpty(l)).Select(l =>
            {
                var m = Regex.Match(l, @"{x=(?<x>\d+),m=(?<m>\d+),a=(?<a>\d+),s=(?<s>\d+)}");
                return new Part(int.Parse(m.Groups["x"].Value), int.Parse(m.Groups["m"].Value), int.Parse(m.Groups["a"].Value), int.Parse(m.Groups["s"].Value));
            }).Reverse();

            var acceptedParts = new List<Part>();

            foreach (var part in parts)
            {
                var workflow = workflows["in"];
                var currentRule = 0;

                while (true)
                {
                    var rule = workflow.Rules[currentRule];
                    if (!string.IsNullOrEmpty(rule.Field))
                    {
                        var fieldValue = 0;
                        switch (rule.Field)
                        {
                            case "x":
                                fieldValue = part.X;
                                break;
                            case "m":
                                fieldValue = part.M;
                                break;
                            case "a":
                                fieldValue = part.A;
                                break;
                            case "s":
                                fieldValue = part.S;
                                break;
                        }
                        switch (rule.Operator)
                        {
                            case "<":
                                if (fieldValue >= rule.Value)
                                {
                                    currentRule++;
                                    continue;
                                }
                                break;
                            case ">":
                                if (fieldValue <= rule.Value)
                                {
                                    currentRule++;
                                    continue;
                                }
                                break;
                        }
                    }

                    if (rule.OnAccept == "R")
                    {
                        WriteLine($"Rejected part with x={part.X}");
                        break;
                    }
                    else if (rule.OnAccept == "A")
                    {
                        WriteLine($"Accepted part with x={part.X}");
                        acceptedParts.Add(part);
                        break;
                    }
                    else
                    {
                        workflow = workflows[rule.OnAccept];
                        currentRule = 0;
                    }
                }
            }

            return acceptedParts.Sum(p => p.X + p.M + p.A + p.S).ToString();
        }

        public record State(Range XRange, Range MRange, Range ARange, Range SRange, string Workflow, int RuleIndex);

        public (Range, Range) SplitRange(Range range, string op, int value)
        {
            if (op == "<")
            {
                return (new Range(range.Start, value - 1), new Range(value, range.End));
            }

            return (new Range(range.Start, value), new Range(value + 1, range.End));
        }

        public override string PartTwo(List<string> lines)
        {
            Dictionary<string, Workflow> workflows = lines.TakeWhile(l => !string.IsNullOrEmpty(l)).Select(l =>
            {
                var m = Regex.Match(l, @"(?<name>\w+){(?<rules>.+)}");
                var m2 = Regex.Matches(m.Groups["rules"].Value, @"((?<field>[xmas])(?<op>[<>])(?<value>\d+):(?<target>\w+)|(?<target>\w+)),?");

                var rules = m2.Select(m => new Rule(m.Groups["field"].Value, m.Groups["op"].Value, ParseWithFallback(m.Groups["value"].Value, 0), m.Groups["target"].Value)).ToList();

                return new Workflow(m.Groups["name"].Value, rules);
            }).ToDictionary(w => w.Name);

            var initialState = new State(1..4000, 1..4000, 1..4000, 1..4000, "in", 0);
            var stateQueue = new Queue<State>();
            stateQueue.Enqueue(initialState);

            var acceptedStates = new List<State>();

            while (stateQueue.Any())
            {
                var currentState = stateQueue.Dequeue();

                var workflow = workflows[currentState.Workflow];
                var rule = workflow.Rules[currentState.RuleIndex];
                if (!string.IsNullOrEmpty(rule.Field))
                {
                    State left, right;

                    switch (rule.Field)
                    {
                        case "x":
                            {
                                var (leftRange, rightRange) = SplitRange(currentState.XRange, rule.Operator, rule.Value);
                                left = new State(leftRange, currentState.MRange, currentState.ARange, currentState.SRange, currentState.Workflow, currentState.RuleIndex);
                                right = new State(rightRange, currentState.MRange, currentState.ARange, currentState.SRange, currentState.Workflow, currentState.RuleIndex);
                                break;
                            }
                        case "m":
                            {
                                var (leftRange, rightRange) = SplitRange(currentState.MRange, rule.Operator, rule.Value);
                                left = new State(currentState.XRange, leftRange, currentState.ARange, currentState.SRange, currentState.Workflow, currentState.RuleIndex);
                                right = new State(currentState.XRange, rightRange, currentState.ARange, currentState.SRange, currentState.Workflow, currentState.RuleIndex);
                                break;
                            }
                        case "a":
                            {
                                var (leftRange, rightRange) = SplitRange(currentState.ARange, rule.Operator, rule.Value);
                                left = new State(currentState.XRange, currentState.MRange, leftRange, currentState.SRange, currentState.Workflow, currentState.RuleIndex);
                                right = new State(currentState.XRange, currentState.MRange, rightRange, currentState.SRange, currentState.Workflow, currentState.RuleIndex);
                                break;
                            }
                        case "s":
                            {
                                var (leftRange, rightRange) = SplitRange(currentState.SRange, rule.Operator, rule.Value);
                                left = new State(currentState.XRange, currentState.MRange, currentState.ARange, leftRange, currentState.Workflow, currentState.RuleIndex);
                                right = new State(currentState.XRange, currentState.MRange, currentState.ARange, rightRange, currentState.Workflow, currentState.RuleIndex);
                                break;
                            }
                        default:
                            throw new NotImplementedException();
                    }

                    if (rule.Operator == "<")
                    {
                        stateQueue.Enqueue(new State(right.XRange, right.MRange, right.ARange, right.SRange, right.Workflow, right.RuleIndex + 1));
                        currentState = left;
                    }
                    else
                    {
                        stateQueue.Enqueue(new State(left.XRange, left.MRange, left.ARange, left.SRange, left.Workflow, left.RuleIndex + 1));
                        currentState = right;
                    }
                }

                if (rule.OnAccept == "R")
                {
                    continue;
                }
                else if (rule.OnAccept == "A")
                {
                    acceptedStates.Add(currentState);
                }
                else
                {
                    stateQueue.Enqueue(new State(currentState.XRange, currentState.MRange, currentState.ARange, currentState.SRange, rule.OnAccept, 0));
                }
            }

            return acceptedStates.Sum(s =>
            {
                var xLength = s.XRange.End.Value - s.XRange.Start.Value + 1;
                var mLength = s.MRange.End.Value - s.MRange.Start.Value + 1;
                var aLength = s.ARange.End.Value - s.ARange.Start.Value + 1;
                var sLength = s.SRange.End.Value - s.SRange.Start.Value + 1;
                return (long)xLength * mLength * aLength * sLength;
            }).ToString();

            throw new NotImplementedException();
        }
    }
}
