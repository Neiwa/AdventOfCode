using AdventOfCode2022.Day11;
using Core;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2022.Day16
{
    internal class Day16 : BaseAoc
    {
        class Room
        {
            public string Name { get; set; }
            public int Flow { get; set; }
            public List<string> Paths { get; set; }
        }

        class Path
        {
            public Room CurrentNode { get; set; }
            public HashSet<string> OpenNodes { get; set; } = new();
            public int RemainingTime { get; set; }

            public Path Clone()
            {
                return new Path
                {
                    CurrentNode = CurrentNode,
                    OpenNodes = OpenNodes.ToHashSet(),
                    RemainingTime = RemainingTime
                };
            }
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

        static int CurrentBestPossibleRemainingPressureReleaseOpenCurrentValve(Path current, Dictionary<string, Room> rooms)
        {
            if (current.CurrentNode.Flow == 0 || current.OpenNodes.Contains(current.CurrentNode.Name))
            {
                return 0;
            }
            return current.CurrentNode.Flow * current.RemainingTime
                + CurrentBestPossibleRemainingPressureRelease(current, rooms);
        }

        static int CurrentBestPossibleRemainingPressureRelease(Path current, Dictionary<string, Room> rooms)
        {
            return rooms.Values.Where(r => !current.OpenNodes.Contains(r.Name) && r.Flow > 0)
                .OrderByDescending(r => r.Flow)
                .Zip(Enumerations.Range(current.RemainingTime, -2, current.RemainingTime))
                .Sum(t => t.First.Flow * t.Second);
        }

        IEnumerable<HashSet<T>> GetAllSubSets<T>(IEnumerable<T> set)
        {
            if (!set.Any())
            {
                yield return new HashSet<T>();
                yield break; 
            }
            T? first = set.First();

            foreach (var subSet in GetAllSubSets(set.Skip(1)))
            {
                yield return subSet;
                var nSubSet = new HashSet<T>(subSet);
                nSubSet.Add(first);

                yield return nSubSet;
            }
        }

        [TestCase("", "")]
        [TestCase("a", ",a")]
        [TestCase("ab", ",a,b,ba")]
        [TestCase("abc", ",a,b,c,cb,ca,ba,cba")]
        [TestCase("abcd", ",a,b,c,d,dc,db,da,cb,ca,ba,dcb,dca,dba,cba,dcba")]
        public void GetAllSubSets_(string value, string expected)
        {
            // Arrange
            var set = value.ToCharArray();

            var subSets = expected.Split(',').Select(s => s.ToCharArray().ToHashSet());

            // Act
            var actual = GetAllSubSets(set);

            // Assert
            Assert.That(actual, Is.EquivalentTo(subSets));
        }

        class KeyValuePair<T>
        {
            public KeyValuePair(Path key, T value)
            {
                Key = key;
                Value = value;
            }
            public Path Key;
            public T Value;
        }
        class PathDict<T> : List<KeyValuePair<T>>
        {
            public bool HasDefault { get; set; }
            public T Default { get; set; }

            public T this[Path key]
            {
                get
                {
                    if (HasDefault)
                    {
                        KeyValuePair<T>? pair = Enumerable.FirstOrDefault(this, t => t.Key.CurrentNode == key.CurrentNode &&
                                                                t.Key.OpenNodes.SetEquals(key.OpenNodes) &&
                                                                t.Key.RemainingTime == key.RemainingTime);
                        if (null == pair)
                            return Default;
                        return pair.Value;
                    }
                    return Enumerable.First(this, t => t.Key.CurrentNode == key.CurrentNode &&
                                        t.Key.OpenNodes.SetEquals(key.OpenNodes) &&
                                        t.Key.RemainingTime == key.RemainingTime).Value;
                }
                set
                {
                    var pair = Enumerable.FirstOrDefault(this, t => t.Key.CurrentNode == key.CurrentNode &&
                                        t.Key.OpenNodes.SetEquals(key.OpenNodes) &&
                                        t.Key.RemainingTime == key.RemainingTime);
                    if(null == pair)
                    {
                        Add(new KeyValuePair<T>(key, value));
                    }
                    else
                    {
                        pair.Value = value;
                    }
                }
            }
        }

        public override void PartOne(List<string> lines)
        {
            var rooms = ReadInput(lines);
            var roomsWithValves = rooms.Values.Where(r => r.Flow > 0).ToList();
            var startRoom = rooms["AA"];

            var bestPossiblePressureRelease = rooms.Values.OrderByDescending(r => r.Flow).Zip(Enumerable.Range(30 - roomsWithValves.Count + 1, roomsWithValves.Count).Reverse()).Sum(t => t.First.Flow*t.Second);

            List<Path> queue = new();
            PathDict<int> Dist = new();
            Dist.HasDefault = true;
            Dist.Default = int.MaxValue;
            PathDict<Path> Prev = new();
            foreach (var subSet in GetAllSubSets(roomsWithValves.Select(r => r.Name)))
            {
                foreach (var room in rooms.Values)
                {
                    for (int i = 0; i <= 30; i++)
                    {
                        Path path = new Path { CurrentNode = room, OpenNodes = subSet, RemainingTime = i };
                        queue.Add(path);
                        //Dist[path] = int.MaxValue;
                    }
                }
            }

            Path start = new Path { CurrentNode = startRoom, RemainingTime = 30 };

            Dist[start] = 0;

            while (queue.Any())
            {
                var path = queue.MinBy(p => Dist[p]);
                queue.Remove(path);

                if (path.RemainingTime == 0 || roomsWithValves.Count == path.OpenNodes.Count)
                {
                    // Opened all valves with pressure > 0
                    // Or used all 30 min
                    continue;
                }
                Path newPath;
                int alternatePath;
                if (path.CurrentNode.Flow > 0)
                {
                    newPath = path.Clone();
                    newPath.OpenNodes.Add(path.CurrentNode.Name);
                    newPath.RemainingTime--;
                    alternatePath = Dist[path] + (bestPossiblePressureRelease - CurrentBestPossibleRemainingPressureReleaseOpenCurrentValve(newPath, rooms));
                    if (alternatePath < Dist[newPath])
                    {
                        Dist[newPath] = alternatePath;
                        Prev[newPath] = path;
                    }
                }

                foreach (var roomName in rooms[path.CurrentNode.Name].Paths)
                {
                    newPath = path.Clone();
                    newPath.CurrentNode = rooms[roomName];
                    newPath.RemainingTime--;
                    alternatePath = Dist[path] + (bestPossiblePressureRelease - CurrentBestPossibleRemainingPressureRelease(newPath, rooms));
                    if (alternatePath < Dist[newPath])
                    {
                        Dist[newPath] = alternatePath;
                        Prev[newPath] = path;
                    }
                }
            }

            ;
        }

        public override void PartTwo(List<string> lines)
        {
            throw new NotImplementedException();
        }
    }
}
