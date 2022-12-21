using Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022.Day13
{
    internal class Day13 : BaseAocV1
    {
        bool isList(string s) => s.Length >= 1 && s[0] == '[';
        bool isInt(string s) => s.Length >= 1 && s[0] >= '0' && s[0] <= '9';

        int compareInt(string l, string r) => int.Parse(r) - int.Parse(l);

        IEnumerable<string> getListElements(string list)
        {
            int depth = 0;
            string output = "";
            for (int i = 1; i < list.Length; i++)
            {
                if (depth < 0)
                {
                    break;
                }

                char c = list[i];

                if (c == '[')
                {
                    depth++;
                    output += c;
                }
                else if (c == ']')
                {
                    if (depth == 0)
                    {
                        if (output.Length > 0)
                        {
                            yield return output;
                        }
                    }
                    else
                    {
                        output += c;
                    }
                    depth--;
                }
                else if (c == ',')
                {
                    if (depth == 0)
                    {
                        yield return output;
                        output = "";
                    }
                    else
                    {
                        output += c;
                    }
                }
                else
                {
                    output += c;
                }
            }
            yield break;
        }

        protected override void Write(string value, int indent = 0)
        {
            if (Debug)
            {
                Console.WriteLine($"{string.Join("", Enumerable.Repeat("  ", indent))}- {value}");
            }
        }

        int compare(string left, string right)
        {
            return compare(left, right, 0);
        }

        int compare(string left, string right, int depth)
        {
            if (Debug)
            {
                Write($"Compare {left} vs {right}", depth);
            }

            if(isList(left) || isList(right))
            {
                List<string> leftElements;
                List<string> rightElements;
                if (isList(left))
                {
                    leftElements = getListElements(left).ToList();
                }
                else if(isInt(left))
                {
                    Write($"Mixed types; convert left to [{left}] and retry", depth);
                    leftElements = new() { left };
                }
                else
                {
                    throw new Exception("Invalid state");
                }

                if (isList(right))
                {
                    rightElements = getListElements(right).ToList();
                }
                else if (isInt(right))
                {
                    Write($"Mixed types; convert right to [{right}] and retry", depth);
                    rightElements = new() { right };
                }
                else
                {
                    throw new Exception("Invalid state");
                }

                for (int i = 0; i < rightElements.Count; i++)
                {
                    if (i + 1 > leftElements.Count)
                    {
                        Write("Left side ran out of items, input OK", depth);
                        return 1;
                    }
                    var res = compare(leftElements[i], rightElements[i], depth + 1);
                    if (res != 0)
                    {
                        return res;
                    }
                }
                if (leftElements.Count > rightElements.Count)
                {
                    Write("Right side ran out of items, input NOK", depth);
                    return -1;
                }
            }
            else if (isInt(left) && isInt(right))
            {
                int v = compareInt(left, right);
                if (Debug)
                {
                    if(v>0)
                    {
                        Write($"Left side is smaller, input OK", depth);
                    }
                    else if(v<0)
                    {
                        Write($"Right side is smaller, input NOK", depth);
                    }
                }
                return v;
            }
            else
            {
                throw new Exception("Invalid state");
            }

            return 0;
        }

        public override void PartOneV1(List<string> lines)
        {
            int indexSum = 0;
            for (int i = 0; i < lines.Count; i+=3)
            {
                var left = lines[i];
                var right = lines[i + 1];
                var res = compare(left, right);
                if (res >= 0)
                {
                    indexSum += 1 + i / 3;
                }
                if(Debug)
                {
                    Console.WriteLine($"({1+i/3:D2}) Compare {left} vs {right} => {res >= 0} ({res})");
                    Console.WriteLine();
                }
            }
            Console.WriteLine(indexSum);
        }

        public override void PartTwoV1(List<string> lines)
        {
            var sortLines = lines.Where(l => l != "").Append("[[2]]").Append("[[6]]").ToList();
            sortLines.Sort(compare);
            sortLines.Reverse();
            if(Debug)
            {
                sortLines.ForEach(Console.WriteLine);
            }
            var iTwo = sortLines.IndexOf("[[2]]");
            var iSix = sortLines.IndexOf("[[6]]");
            Console.WriteLine($"[[2]] at {iTwo}");
            Console.WriteLine($"[[6]] at {iSix}");
            Console.WriteLine($"{iTwo + 1} x {iSix + 1} = {(iTwo + 1) * (iSix + 1)}");
        }

        [TestCase("[1,1,3,1,1]", "[1,1,5,1,1]", true)]
        [TestCase("[1,1,5,1,1]", "[1,1,3,1,1]", false)]
        [TestCase("[[1],[2,3,4]]", "[[1],4]", true)]
        [TestCase("[9]", "[[8,7,6]]", false)]
        [TestCase("[9]", "[[9,7,6]]", true)]
        [TestCase("[8]", "[[9,7,6]]", true)]
        [TestCase("[8]", "[[8,7,6]]", true)]
        [TestCase("[[8,7,6]]", "[9]", true)]
        [TestCase("[[4,4],4,4]", "[[4,4],4,4,4]", true)]
        [TestCase("[]", "[3]", true)]
        [TestCase("[[[]]]", "[[]]", false)]
        [TestCase("[[]]", "[[[]]]", true)]
        [TestCase("[10]", "[9]", false)]
        [TestCase("[9]", "[10]", true)]
        public void compare_Tests(string left, string right, bool result)
        {
            Debug = true;
            Assert.That(compare(left, right) >= 0, Is.EqualTo(result));
        }


        [TestCase("[]", 0)]
        [TestCase("[1]", 1)]
        [TestCase("[1,2]", 2)]
        [TestCase("[[]]", 1)]
        [TestCase("[[],[]]", 2)]
        [TestCase("[[1,2,3],[]]", 2)]
        [TestCase("[[],[1,2,3],[]]", 3)]
        public void getListElements_Tests(string input, int elemCount)
        {
            Debug = true;
            var elemns = getListElements(input).ToList();
            Assert.That(elemns, Has.Count.EqualTo(elemCount));
        }
    }
}
