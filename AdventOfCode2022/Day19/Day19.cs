using Core;
using Helpers;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static AdventOfCode2022.Day16_2.Day16_2;

namespace AdventOfCode2022.Day19
{
    public class Day19 : BaseAoc
    {
        class Blueprint
        {
            public int Id { get; set; }
            public Dictionary<string, (int Ore, int Clay, int Obsidian)> Robots { get; set; }
            public int MaxGeodes { get; set; }
        }

        List<Blueprint> ParseInput(IEnumerable<string> lines)
        {
            int Parse(string value)
            {
                if (string.IsNullOrEmpty(value))
                {
                    return 0;
                }
                return int.Parse(value);
            }

            List<Blueprint> blueprints = new();
            foreach (var line in lines)
            {
                var match = Regex.Match(line, @"Blueprint (?<id>\d+): Each ore robot costs (?<oo>\d+) ore. Each clay robot costs (?<co>\d+) ore. Each obsidian robot costs (?<bo>\d+) ore and (?<bc>\d+) clay. Each geode robot costs (?<go>\d+) ore and (?<gb>\d+) obsidian.");
                if (match.Success)
                {
                    blueprints.Add(new Blueprint
                    {
                        Id = int.Parse(match.Groups["id"].Value),
                        Robots = new() {
                            { "ore", (Parse(match.Groups["oo"].Value), Parse(match.Groups["oc"].Value), Parse(match.Groups["ob"].Value)) },
                            { "clay", (Parse(match.Groups["co"].Value), Parse(match.Groups["cc"].Value), Parse(match.Groups["cb"].Value)) },
                            { "obsidian", (Parse(match.Groups["bo"].Value), Parse(match.Groups["bc"].Value), Parse(match.Groups["bb"].Value)) },
                            { "geode", (Parse(match.Groups["go"].Value), Parse(match.Groups["gc"].Value), Parse(match.Groups["gb"].Value)) },
                        }
                    });
                }
            }

            return blueprints;
        }

        struct State
        { 
            public State()
            {

            }

            public State(State state)
            {
                TimeLeft = state.TimeLeft;
                Ore = state.Ore;
                Clay = state.Clay;
                Obsidian = state.Obsidian;
                Geode = state.Geode;

                OreRobots = state.OreRobots;
                ClayRobots = state.ClayRobots;
                ObsidianRobots = state.ObsidianRobots;
                GeodeRobots = state.GeodeRobots;

                Log = state.Log.ToList();
            }

            public int TimeLeft = 0;

            public int Ore = 0;
            public int Clay = 0;
            public int Obsidian = 0;
            public int Geode = 0;

            public int OreRobots = 0;
            public int ClayRobots = 0;
            public int ObsidianRobots = 0;
            public int GeodeRobots = 0;

            public List<(int Minute, string Move)> Log = new();

            public override string ToString()
            {
                return $"<State TimeLeft: {TimeLeft,2}, Ore: {Ore,2}, Clay: {Clay,2}, Obsidian: {Obsidian,2}, Geode: {Geode,2}, Robots: {{Ore: {OreRobots,2}, Clay: {ClayRobots,2}, Obsidian: {ObsidianRobots,2}, Geode: {GeodeRobots,2} }} >";
            }

            public static State Parse(string value)
            {
                var match = Regex.Match(value, @"<\s*State\s+TimeLeft:\s*(?<tl>\d+),\s*Ore:\s*(?<o>\d+),\s*Clay:\s*(?<c>\d+),\s*Obsidian:\s*(?<b>\d+),\s*Geode:\s*(?<g>\d+),\s*Robots:\s*{Ore:\s*(?<ro>\d+),\s*Clay:\s*(?<rc>\d+),\s*Obsidian:\s*(?<rb>\d+),\s*Geode:\s*(?<rg>\d+)\s*}\s*>");
                if (match.Success)
                {
                    var state = new State();
                    state.TimeLeft = int.Parse(match.Groups["tl"].Value);

                    state.Ore = int.Parse(match.Groups["o"].Value);
                    state.Clay = int.Parse(match.Groups["c"].Value);
                    state.Obsidian = int.Parse(match.Groups["b"].Value);
                    state.Geode = int.Parse(match.Groups["g"].Value);

                    state.OreRobots = int.Parse(match.Groups["ro"].Value);
                    state.ClayRobots = int.Parse(match.Groups["rc"].Value);
                    state.ObsidianRobots = int.Parse(match.Groups["rb"].Value);
                    state.GeodeRobots = int.Parse(match.Groups["rg"].Value);

                    return state;
                }
                throw new FormatException();
            }
        }

        IEnumerable<State> NextStates(State state, Blueprint blueprint)
        {
            if (state.TimeLeft == 0)
            {
                return Enumerable.Empty<State>();
            }

            return new[] { "pass", "ore", "clay", "obsidian", "geode" }.Select(move =>
            {
                var newState = new State(state);

                if (move == "pass")
                {
                    if (newState.GeodeRobots > 0)
                    {
                        newState.Ore += newState.OreRobots * newState.TimeLeft;
                        newState.Clay += newState.ClayRobots * newState.TimeLeft;
                        newState.Obsidian += newState.ObsidianRobots * newState.TimeLeft;
                        newState.Geode += newState.GeodeRobots * newState.TimeLeft;
                        newState.TimeLeft = 0;
                    }
                    else
                    {
                        newState.TimeLeft = -1;
                    }
                }
                else
                {
                    while (newState.TimeLeft > 0)
                    {
                        if (newState.Ore >= blueprint.Robots[move].Ore &&
                            newState.Clay >= blueprint.Robots[move].Clay &&
                            newState.Obsidian >= blueprint.Robots[move].Obsidian)
                        {
                            break;
                        }
                        newState.TimeLeft--;
                        newState.Ore += newState.OreRobots;
                        newState.Clay += newState.ClayRobots;
                        newState.Obsidian += newState.ObsidianRobots;
                        newState.Geode += newState.GeodeRobots;
                    }

                    newState.TimeLeft--;

                    newState.Ore += newState.OreRobots;
                    newState.Clay += newState.ClayRobots;
                    newState.Obsidian += newState.ObsidianRobots;
                    newState.Geode += newState.GeodeRobots;

                    newState.Ore -= blueprint.Robots[move].Ore;
                    newState.Clay -= blueprint.Robots[move].Clay;
                    newState.Obsidian -= blueprint.Robots[move].Obsidian;

                    switch (move)
                    {
                        case "ore":
                            newState.OreRobots++;
                            break;
                        case "clay":
                            newState.ClayRobots++;
                            break;
                        case "obsidian":
                            newState.ObsidianRobots++;
                            break;
                        case "geode":
                            newState.GeodeRobots++;
                            break;
                    }
                }

                if (IsLevel(ActionLevel.Trace))
                {
                    newState.Log.Add((newState.TimeLeft, move)); 
                }
                return newState;
            }).Where(s => s.TimeLeft >= 0);
        }

        [TestCase("<State TimeLeft:  5, Ore: 17, Clay: 15, Obsidian: 12, Geode:  1, Robots: {Ore:  6, Clay:  5, Obsidian:  5, Geode:  1 } >")]
        [TestCase("<State TimeLeft:  4, Ore: 20, Clay: 20, Obsidian:  5, Geode:  2, Robots: {Ore:  6, Clay:  5, Obsidian:  5, Geode:  2 } >")]
        public void NextStates_Test(string sState)
        {
            Blueprint b = new Blueprint()
            {
                Id = 1,
                Robots = new()
                {
                    { "ore", (4, 0, 0) },
                    { "clay", (2, 0, 0) },
                    { "obsidian", (3, 14, 0) },
                    { "geode", (2, 0, 7) }
                }
            };

            State s = State.Parse(sState);

            WriteLine(s.ToString());
            WriteLine($"h(s)={Heuristic(s)}");

            var nextStates = NextStates(s, b).ToList();
            foreach (var state in nextStates)
            {
                WriteLine(state.ToString());
                WriteLine($"prio={state.Geode + Heuristic(state)}, h(state)={Heuristic(state)}");
            }
        }

        int Heuristic(State state)
        {
            return (int)Math.Floor((state.GeodeRobots * 2 + state.TimeLeft) * (state.TimeLeft / 2d));
        }

        State Score(State state, Blueprint blueprint)
        {
            if (state.TimeLeft == 0)
            {
                return state;
            }

            PriorityQueue<State, int> queue = new();
            queue.Enqueue(state, 0);

            int maxScore = Heuristic(state);

            State maxState = state;

            while (queue.TryDequeue(out var current, out var priority))
            {
                foreach (var nextState in NextStates(current, blueprint))
                {
                    if (nextState.Geode > maxState.Geode)
                    {
                        maxState = nextState;
                        WriteLine($"New max: {nextState}");
                    }
                    var score = Heuristic(nextState);
                    if (nextState.Geode + score > maxState.Geode)
                    {
                        queue.Enqueue(nextState, maxScore - score);
                    }
                }
            }

            return maxState;
        }

        State Score2(State state, Blueprint blueprint)
        {
            if (state.TimeLeft == 0)
            {
                return state;
            }

            Dictionary<int, State> best = new();
            Stack<State> dfs = new();
            dfs.Push(state);
            PriorityQueue<State, int> queue = new();
            queue.Enqueue(state, 0);

            int maxScore = Heuristic(state);

            State maxState = state;
            while(dfs.TryPop(out var current))
            //while (queue.TryDequeue(out var current, out var priority))
            {
                foreach (var nextState in NextStates(current, blueprint))
                {
                    if (nextState.Geode > maxState.Geode)
                    {
                        maxState = nextState;
                        WriteLine($"New max: {nextState}");
                    }
                    var score = Heuristic(nextState);
                    if (nextState.Geode + score > maxState.Geode)
                    {
                        dfs.Push(nextState);
                    }
                }
            }

            return maxState;
        }

        void Replay(State goalState, State initialState, Blueprint blueprint, int indent = 0)
        {
            if (!IsLevel(ActionLevel.Trace))
            {
                return;
            }
            State current = initialState;
            State next;
            foreach (var row in goalState.Log)
            {
                IEnumerable<State> nextStates = NextStates(current, blueprint);
                next = nextStates.First(s => s.Log.Last().Minute == row.Minute && s.Log.Last().Move == row.Move);
                for (int i = current.TimeLeft - 1; i > next.TimeLeft; i--)
                {
                    current.TimeLeft--;
                    current.Ore += current.OreRobots;
                    current.Clay += current.ClayRobots;
                    current.Obsidian += current.ObsidianRobots;
                    current.Geode += current.GeodeRobots;
                    WriteLine($"{initialState.TimeLeft - current.TimeLeft,2}: {"",8} {current}", indent: indent);
                }
                current = next;
                WriteLine($"{initialState.TimeLeft - row.Minute,2}: {row.Move,8} {current}", indent: indent);
            }
        }

        public override string PartOne(List<string> lines)
        {
            var blueprints = ParseInput(lines);
            WriteLine($"{blueprints.Count} blueprints");

            var totalScore = 0;

            foreach (var blueprint in blueprints)
            {
                var state = new State();
                state.OreRobots = 1;
                state.TimeLeft = 24;
                State goalState = Score2(state, blueprint);
                blueprint.MaxGeodes = goalState.Geode;

                totalScore += blueprint.Id * blueprint.MaxGeodes;

                //var goalState = new State();
                //goalState.TimeLeft = 0;

                //var res = Search.AStar(state, goalState, s => 0, (a, b) => a.TimeLeft == b.TimeLeft, s => NextStates(s, blueprint));

                WriteLine($"Blueprint {blueprint.Id} = {blueprint.MaxGeodes}");
                WriteLine(goalState.ToString(), indent: 1);
                //foreach (var (row, move) in goalState.Log)
                //{
                //    WriteLine($"{row} - {move}", 2);
                //}
                Replay(goalState, state, blueprint, 2);
                WriteLine();
            }

            return $"{totalScore}";
        }

        public override string PartTwo(List<string> lines)
        {
            var blueprints = ParseInput(lines.Take(3));
            WriteLine($"{blueprints.Count} blueprints");

            foreach (var blueprint in blueprints)
            {
                var state = new State();
                state.OreRobots = 1;
                state.TimeLeft = 32;
                State goalState = Score2(state, blueprint);
                blueprint.MaxGeodes = goalState.Geode;

                WriteLine($"Blueprint {blueprint.Id} = {blueprint.MaxGeodes}");
                WriteLine(goalState.ToString(), indent: 1);
                Replay(goalState, state, blueprint, 2);
                WriteLine();
            }

            var totalScore = blueprints.Aggregate(1, (a, b) => a * b.MaxGeodes);

            return $"{totalScore}";
        }
    }
}
