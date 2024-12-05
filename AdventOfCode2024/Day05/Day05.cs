using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Day05
{
    public class Day05 : BaseAoc<Input>
    {
        public override object PartOne(Input input)
        {
            long sum = 0;
            foreach (var update in input.Updates)
            {
                bool correct = true;
                for (int i = 0; i < update.Count; i++)
                {
                    foreach (var rule in input.Rules)
                    {
                        if (update[i] == rule.Left)
                        {
                            var index = update.IndexOf(rule.Right);
                            if (index >= 0 && index < i)
                            {
                                correct = false;
                                break;
                            }
                        }
                    }

                    if (!correct) break;
                }
                if (correct)
                {
                    WriteLine(string.Join(',', update));
                    sum += update[update.Count / 2];
                }
            }

            return sum;
        }

        public override object PartTwo(Input input)
        {
            List<List<long>> incorrectUpdates = [];
            foreach (var update in input.Updates)
            {
                bool correct = true;
                for (int i = 0; i < update.Count; i++)
                {
                    foreach (var rule in input.Rules)
                    {
                        if (update[i] == rule.Left)
                        {
                            var index = update.IndexOf(rule.Right);
                            if (index >= 0 && index < i)
                            {
                                correct = false;
                                incorrectUpdates.Add(update);
                                break;
                            }
                        }
                    }

                    if (!correct) break;
                }
            }

            Dictionary<long, List<long>> rulings = input.Rules.GroupBy(r => r.Left).ToDictionary(g => g.Key, g => g.Select(r => r.Right).ToList());

            long sum = 0;
            foreach (var update in incorrectUpdates)
            {
                var fixedUpdate = new List<long>();
                for (int i = 0; i < update.Count; i++)
                {
                    if (!rulings.TryGetValue(update[i], out var ruling))
                    {
                        fixedUpdate.Add(update[i]);
                        continue;
                    }

                    var lowestIndex = ruling.Select(r => fixedUpdate.IndexOf(r)).Where(i => i >= 0).MinOrDefault(i => i, -1);
                    if (lowestIndex >= 0 && lowestIndex < i)
                    {
                        fixedUpdate.Insert(lowestIndex, update[i]);
                    }
                    else
                    {
                        fixedUpdate.Add(update[i]);
                    }
                }
                WriteLine(string.Join(",", update));
                WriteLine(string.Join(",", fixedUpdate));
                sum += fixedUpdate[fixedUpdate.Count / 2];
            }

            return sum;
        }

        public class Node(long value, List<Node> children, long parentCount)
        {
            public long Value { get; init; } = value;
            public List<Node> Children { get; init; } = children;

            public long ParentCount { get; set; } = parentCount;
        }

        protected override Input ParseInput(List<string> lines)
        {
            var input = new Input();

            bool rules = true;
            foreach (var line in lines)
            {
                if (line == string.Empty)
                {
                    rules = false;
                    continue;
                }

                if (rules)
                {
                    input.Rules.Add(Input.Rule.Parse(line));
                }
                else
                {
                    input.Updates.Add(line.Split(',').Select(long.Parse).ToList());
                }
            }

            return input;
        }
    }

    public class Input
    {
        public record Rule(long Left, long Right)
        {
            public static Rule Parse(string line)
            {
                var s = line.Split('|');
                return new Rule(long.Parse(s[0]), long.Parse(s[1]));
            }
        }

        public List<Rule> Rules { get; set; } = [];

        public List<List<long>> Updates { get; set; } = [];
    }
}
