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
                            Number = long.Parse(match.Groups["number"].Value),
                            Operation = 'y'
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
            Dictionary<string, string> monkeyLinks = new(); // Key: Child, Value: Parent
            foreach (var (name, monkey) in monkies)
            {
                if (monkey.Operation != 'y')
                {
                    monkeyLinks.Add(monkey.LeftMonkey, name);
                    monkeyLinks.Add(monkey.RightMonkey, name);
                }
            }

            List<string> path = new();
            string current = "humn";
            do
            {
                path.Add(current);
                current = monkeyLinks[current];
            } while (current != "root");

            if (monkies["root"].LeftMonkey == path.Last())
            {
                current = monkies["root"].RightMonkey;
            }
            else
            {
                current = monkies["root"].LeftMonkey;
            }

            long target = monkies[current].Yell(monkies);
            WriteLine($"current = {current}", ActionLevel.Trace);
            WriteLine($"target = {target}");
            if (IsTrace)
            {
                foreach (var monkey in path)
                {
                    WriteLine(monkey, ActionLevel.Trace);
                }
            }

            Stack<string> unYellers = new(path);

            var shouldYell = monkies[unYellers.Pop()].UnYell(monkies, unYellers, target, this);

            return shouldYell.ToString();
        }

        public void TraceWriteLine(string value)
        {
            WriteLine(value, ActionLevel.Trace);
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
        public long UnYell(Dictionary<string, Monkey> monkies, Stack<string> unYellers, long target, Day21 day)
        {
            if (Operation == 'y') //Should only be humn
            {
                return target;
            }

            var unYeller = unYellers.Pop();


            long newTarget = 0;
            if (LeftMonkey == unYeller)
            {
                switch (Operation)
                {
                    case '+':
                        newTarget = target - monkies[RightMonkey].Yell(monkies);
                        break;
                    case '-':
                        newTarget = target + monkies[RightMonkey].Yell(monkies);
                        break;
                    case '*':
                        newTarget = target / monkies[RightMonkey].Yell(monkies);
                        break;
                    case '/':
                        newTarget = target * monkies[RightMonkey].Yell(monkies);
                        break;
                }
            }
            else
            {
                switch (Operation)
                {
                    case '+':
                        newTarget = target - monkies[LeftMonkey].Yell(monkies);
                        break;
                    case '-':
                        newTarget = monkies[LeftMonkey].Yell(monkies) - target;
                        break;
                    case '*':
                        newTarget = target / monkies[LeftMonkey].Yell(monkies);
                        break;
                    case '/':
                        newTarget = monkies[LeftMonkey].Yell(monkies) / target;
                        break;
                }
            }
            day.TraceWriteLine($"unYeller = {unYeller}, newTarget = {newTarget}");

            return monkies[unYeller].UnYell(monkies, unYellers, newTarget, day);
        }
    }
}
