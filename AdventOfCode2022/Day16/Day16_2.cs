using AdventOfCode2022.Day11;
using Core;
using Helpers;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static AdventOfCode2022.Day16_2.Day16_2;

namespace AdventOfCode2022.Day16_2
{
    [TestFixture(TestName="Day16")]
    public class Day16_2 : BaseAocV1
    {
        public class Room
        {
            public string Name { get; set; }
            public int Flow { get; set; }
            public List<string> Paths { get; set; }
        }

        Dictionary<string, Room> ReadInput(List<string> lines)
        {
            Dictionary<string, Room> rooms = new();
            foreach (var line in lines)
            {
                var match = Regex.Match(line, @"Valve (?<name>\w\w) has flow rate=(?<flow>\d+); tunnels? leads? to valves? (?<paths>[\w, ]+)");
                if (match.Success)
                {
                    Room room = new Room
                    {
                        Name = match.Groups["name"].Value,
                        Flow = int.Parse(match.Groups["flow"].Value),
                        Paths = match.Groups["paths"].Value.Split(",").Select(s => s.Trim()).ToList()
                    };
                    rooms.Add(room.Name, room);
                }
            }

            return rooms;
        }

        public struct State
        {
            public State()
            {
            }

            public List<string> Open = new();
            public string Current = string.Empty;
            public int MinutesLeft = 0;
            public int Score = 0;
            public int Phase = 1;
        }

        static int CurrentBestPossibleRemainingPressureRelease(State current, Dictionary<string, Dictionary<string, int>> dist, Dictionary<string, Room> rooms)
        {
            if (current.Phase == 2)
            {
                int one = dist[current.Current]
                                    .Where(kvp => !current.Open.Contains(kvp.Key))
                                    .Select(kvp => rooms[kvp.Key].Flow * (current.MinutesLeft - kvp.Value))
                                    .Where(v => v > 0)
                                    .Sum();
                int two = dist["AA"]
                                    .Where(kvp => !current.Open.Contains(kvp.Key))
                                    .Select(kvp => rooms[kvp.Key].Flow * (26 - kvp.Value))
                                    .Where(v => v > 0)
                                    .Sum();
                return Math.Max(one, two);
                return Math.Max(dist[current.Current].Select(kvp => current.Open.Contains(kvp.Key) ? 0 : rooms[kvp.Key].Flow * (26 - kvp.Value - 1)).Sum(),
                    dist["AA"].Select(kvp => current.Open.Contains(kvp.Key) ? 0 : rooms[kvp.Key].Flow * (26 - kvp.Value - 1)).Sum());
            }
            return dist[current.Current]
                .Where(kvp => !current.Open.Contains(kvp.Key))
                .Select(kvp => rooms[kvp.Key].Flow * (current.MinutesLeft - kvp.Value))
                .Where(v => v > 0)
                .Sum();
        }

        public State R2(string start, Dictionary<string, Dictionary<string, int>> dist, Dictionary<string, Room> rooms, int minutesLeft = 30, int phases = 1)
        {
            PriorityQueue<State, int> states = new();
            int maxFlow = rooms.Values.Select(r => r.Flow * minutesLeft).Sum();

            foreach (var (to, weight) in dist[start])
            {
                int score = rooms[to].Flow * (minutesLeft - weight);
                states.Enqueue(new State {
                    Current = to,
                    MinutesLeft = minutesLeft - weight,
                    Phase = phases,
                    Score = score,
                    Open = new() { to, $"{weight}" }
                }, maxFlow - score);
            }

            var bestState = new State { Score = 0 };
            int minPerState = rooms.Values.Where(r => r.Flow > 0).Count() / 3;

            while (states.TryDequeue(out var state, out var priority))
            {
                State newState;
                int potentialScore;

                if (phases > 1 && state.Phase == 2 && state.Open.Count > minPerState)
                {
                    foreach (var (to, weight) in dist[start])
                    {
                        if (state.Open.Contains(to)) continue;

                        var newOpen = state.Open.Append("switch").Append(to);
                        if (Debug)
                        {
                            newOpen = newOpen.Append($"{minutesLeft - state.MinutesLeft - weight}");
                        }

                        newState = new State
                        {
                            Current = to,
                            MinutesLeft = minutesLeft - weight,
                            Score = state.Score + rooms[to].Flow * (minutesLeft - weight),
                            Open = newOpen.ToList(),
                            Phase = 1
                        };
                        if (newState.Score > bestState.Score)
                        {
                            bestState = newState;
                            if (bestState.Score > 100)
                            {
                                WriteLine($"New best: {bestState.Score}");
                                WriteLine(string.Join(" -> ", bestState.Open), indent: 1);
                            }
                        }

                        potentialScore = newState.Score + CurrentBestPossibleRemainingPressureRelease(newState, dist, rooms);
                        if (potentialScore > bestState.Score)
                        {
                            states.Enqueue(newState, maxFlow - potentialScore);
                        }
                    }
                }

                foreach (var (to, weight) in dist[state.Current])
                {
                    if(state.MinutesLeft - weight < 0)
                    {
                        continue;
                    }

                    if(state.Open.Contains(to))
                    {
                        continue;
                    }

                    var newOpen = state.Open.Append(to);
                    if (Debug)
                    {
                        newOpen = newOpen.Append($"{minutesLeft - state.MinutesLeft + 1}");
                    }

                    newState = new State
                    {
                        Current = to,
                        MinutesLeft = state.MinutesLeft - weight,
                        Score = state.Score + rooms[to].Flow * (state.MinutesLeft - weight),
                        Open = newOpen.ToList(),
                        Phase = state.Phase
                    };

                    if (newState.Score > bestState.Score)
                    {
                        bestState = newState;
                        if (bestState.Score > 100)
                        {
                            WriteLine($"New best: {bestState.Score}");
                            WriteLine(string.Join(" -> ", bestState.Open), indent: 1);
                        }
                    }
                    potentialScore = newState.Score + CurrentBestPossibleRemainingPressureRelease(newState, dist, rooms);
                    if (potentialScore > bestState.Score)
                    {
                        states.Enqueue(newState, maxFlow - potentialScore);
                    }
                }
            }

            return bestState;
        }

        public override void PartOneV1(List<string> lines)
        {
            var rooms = ReadInput(lines);
            var roomsWithValves = rooms.Values.Where(r => r.Flow > 0).ToList();
            var startRoom = rooms["AA"];

            Dictionary<string, Dictionary<string, int>> Dist = new();

            foreach (var start in roomsWithValves.Append(startRoom))
            {
                Dictionary<string, int> d = new();
                foreach (var goal in roomsWithValves)
                {
                    if (start.Name == goal.Name)
                    {
                        continue;
                    }
                    d[goal.Name] = Search.AStar(start, goal, _ => 100, (a, b) => a.Name == b.Name, r => r.Paths.Select(roomName => rooms[roomName])).Count();
                }
                if (d.Count > 0)
                {
                    Dist[start.Name] = d;
                }
            }

            if (Debug)
            {
                foreach (var (from, toDict) in Dist)
                {
                    foreach (var (to, weight) in toDict)
                    {
                        WriteLine($"{from} -> {to} = {weight}");
                    }
                } 
            }

            var res = R2(startRoom.Name, Dist, rooms);

            Console.WriteLine(res.Score);
            WriteLine($"{res.Score}");
            WriteLine($"{string.Join(" -> ", res.Open)}");
        }

        public override void PartTwoV1(List<string> lines)
        {
            var rooms = ReadInput(lines);
            var roomsWithValves = rooms.Values.Where(r => r.Flow > 0).ToList();
            var startRoom = rooms["AA"];

            Dictionary<string, Dictionary<string, int>> Dist = new();

            foreach (var start in roomsWithValves.Append(startRoom))
            {
                Dictionary<string, int> d = new();
                foreach (var goal in roomsWithValves)
                {
                    if (start.Name == goal.Name)
                    {
                        continue;
                    }
                    d[goal.Name] = Search.AStar(start, goal, _ => 100, (a, b) => a.Name == b.Name, r => r.Paths.Select(roomName => rooms[roomName])).Count();
                }
                if (d.Count > 0)
                {
                    Dist[start.Name] = d;
                }
            }

            if (Debug)
            {
                foreach (var (from, toDict) in Dist)
                {
                    foreach (var (to, weight) in toDict)
                    {
                        WriteLine($"{from} -> {to} = {weight}");
                    }
                }
            }

            var res = R2(startRoom.Name, Dist, rooms, 26, 2);

            Console.WriteLine(res.Score);
            WriteLine($"{res.Score}");
            WriteLine($"{string.Join(" -> ", res.Open)}");
        }
    }
}
