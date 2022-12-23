using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2022.Day21
{
    public class Day21 : BaseAoc<Dictionary<string, Monkey>>
    {
        protected override Dictionary<string, Monkey> ParseInput(List<string> lines)
        {
            Dictionary<string, Monkey> monkies = new();

            foreach (var line in lines)
            {
                var match = Regex.Match(line, @"(?<name>\w+): ((?<number>\d+)|(?<left>\w+) (?<op>.) (?<right>\w+))");
                if (match.Success)
                {
                    var name = match.Groups["name"].Value;
                    if (match.Groups["number"].Success)
                    {
                        monkies.Add(name, new Monkey
                        {
                            Number = long.Parse(match.Groups["number"].Value)
                        });
                    }
                    else
                    {
                        monkies.Add(name, new Monkey
                        {
                            LeftMonkey = match.Groups["left"].Value,
                            RightMonkey = match.Groups["right"].Value,
                            Operation = match.Groups["op"].Value.First()
                        });
                    }
                }
            }

            return monkies;
        }

        public override string PartOne(Dictionary<string, Monkey> monkies)
        {
            return monkies["root"].Yell(monkies).ToString();
        }

        public override string PartTwo(Dictionary<string, Monkey> monkies)
        {
            throw new NotImplementedException();
        }
    }

    public class Monkey
    {
        public long? Number { get; set; }

        public char Operation { get; set; }

        public string LeftMonkey { get; set; }

        public string RightMonkey { get; set; }

        public long Yell(Dictionary<string, Monkey> monkies)
        {
            switch (Operation)
            {
                case '+':
                    Number = Number ?? monkies[LeftMonkey].Yell(monkies) + monkies[RightMonkey].Yell(monkies);
                    break;
                case '-':
                    Number = Number ?? monkies[LeftMonkey].Yell(monkies) - monkies[RightMonkey].Yell(monkies);
                    break;
                case '*':
                    Number = Number ?? monkies[LeftMonkey].Yell(monkies) * monkies[RightMonkey].Yell(monkies);
                    break;
                case '/':
                    Number = Number ?? monkies[LeftMonkey].Yell(monkies) / monkies[RightMonkey].Yell(monkies);
                    break;
            }
            return Number.Value;
        }
    }
}
