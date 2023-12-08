using Core;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2023.Day08
{
    public class Day08 : BaseAocV2
    {
        public class Node
        {
            public Node(string input)
            {
                var m = Regex.Match(input, @"(?<name>.{3}) = \((?<L>.{3}), (?<R>.{3})\)");
                Name = m.Groups["name"].Value;
                L = m.Groups["L"].Value;
                R = m.Groups["R"].Value;
            }

            public string Name { get; }
            public string L { get; }
            public string R { get; }
        }

        public class Cursor
        {
            public List<string> History { get; set; } = new List<string>();

            public Node CurrentNode { get; set; }

            public bool LoopDetected { get; set; }

            public int LoopingFrom { get; set; }
            public int LoopLength { get; set; }

            public Cursor(Node initialNode)
            {
                CurrentNode = initialNode;
            }
        }

        public override string PartOne(List<string> lines)
        {
            var charFeed = Enumerations.CircularCharFeed(lines[0]).GetEnumerator();

            var nodes = lines.Skip(2).Select(l => new Node(l)).ToDictionary(n => n.Name);

            int steps = 0;
            var curr = nodes["AAA"];
            var end = "ZZZ";
            while (curr.Name != end)
            {
                charFeed.MoveNext();
                if(charFeed.Current == 'L')
                {
                    curr = nodes[curr.L];
                }
                else
                {
                    curr = nodes[curr.R];
                }
                steps++;
            }

            return steps.ToString();
        }

        public override string PartTwo(List<string> lines)
        {
            var charFeed = Enumerations.CircularCharFeed(lines[0]).GetEnumerator();

            var nodes = lines.Skip(2).Select(l => new Node(l)).ToDictionary(n => n.Name);

            int steps = 0;
            var curr = nodes.Values.Where(n => n.Name.EndsWith("A")).Select(n => new Cursor(n)).ToList();
            var loopingCurr = new List<Cursor>();

            while (curr.Any())
            {
                steps++;
                charFeed.MoveNext();
                curr = curr.Select(c =>
                {
                    c.History.Add(c.CurrentNode.Name);
                    if (charFeed.Current == 'L')
                    {
                        c.CurrentNode = nodes[c.CurrentNode.L];
                    }
                    else
                    {
                        c.CurrentNode = nodes[c.CurrentNode.R];
                    }

                    if (c.History.Count >= lines[0].Length)
                    {
                        for (var i = lines[0].Length; i < c.History.Count; i+= lines[0].Length)
                        {
                            var pos = steps - i;
                            var oldNode = c.History[pos];
                            if (oldNode == c.CurrentNode.Name)
                            {
                                c.LoopDetected = true;
                                c.LoopingFrom = pos;
                                c.LoopLength = i;
                                loopingCurr.Add(c);
                            }
                        }
                    }

                    return c;
                }).Where(c => !c.LoopDetected).ToList();
            }

            foreach (var cur in loopingCurr)
            {
                WriteLine($"{string.Join(" ", cur.History)}", ActionLevel.Trace);
                WriteLine($"LoopFrom = {cur.LoopingFrom} LoopLength = {cur.LoopLength}");
            }

            var poss = loopingCurr.Select(c => c.LoopingFrom).ToList();
            //while(true)
            //{


            //    if (poss.All(p => poss[0] == p))
            //    {
            //        break;
            //    }
            //}

            return poss[0].ToString();
        }
    }
}
