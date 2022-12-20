using Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2022.Day15
{
    internal class Day15 : BaseAocV1
    {
        static (int X, int Y) GetCornerPos((int X, int Y, int Size) rect, int corner)
        {
            switch (corner)
            {
                case 0:
                    return (rect.X, rect.Y);
                case 1:
                    return (rect.X, rect.Y+ rect.Size);
                case 2:
                    return (rect.X+ rect.Size, rect.Y);
                case 3:
                    return (rect.X+ rect.Size, rect.Y+ rect.Size);
            }
            return (rect.X, rect.Y);
        }

        static int GetDistance((int X, int Y) a, (int X, int Y) b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        class Sensor
        {
            public (int X, int Y) Pos { get; set; }
            public (int X, int Y) BeaconPos { get; set; }
            private int? _distance;
            public int Distance
            {
                get
                {
                    return _distance ??= Math.Abs(Pos.X - BeaconPos.X) + Math.Abs(Pos.Y - BeaconPos.Y);
                }
            }

            public bool IsWithin((int X, int Y, int Size) rect)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (Distance < GetDistance(Pos, GetCornerPos(rect, i)))
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        public override void PartOneV1(List<string> lines)
        {
            List<Sensor> sensors = new();
            HashSet<(int x, int y)> beacons = new();
            foreach (var line in lines)
            {
                var match = Regex.Match(line, @"Sensor at x=(?<sx>-?\d+), y=(?<sy>-?\d+): closest beacon is at x=(?<bx>-?\d+), y=(?<by>-?\d+)");
                if (match.Success)
                {
                    (int X, int Y) beacon = (int.Parse(match.Groups["bx"].Value), int.Parse(match.Groups["by"].Value));
                    beacons.Add(beacon);
                    sensors.Add(new Sensor
                    {
                        Pos = (int.Parse(match.Groups["sx"].Value), int.Parse(match.Groups["sy"].Value)),
                        BeaconPos = beacon
                    });
                }
            }
            var minX = sensors.Min(x => x.Pos.X - x.Distance);
            var maxX = sensors.Max(x => x.Pos.X + x.Distance);
            HashSet<(int X, int Y)> spaces = new();
            var y = 2000000;
            var count = 0;
            for (int i = minX; i < maxX + 1; i++)
            {
                (int X, int Y) pos = (i, y);
                foreach (var sensor in sensors)
                {
                    var dist = Math.Abs(sensor.Pos.X - pos.X) + Math.Abs(sensor.Pos.Y - pos.Y);
                    if (dist <= sensor.Distance)
                    {
                        spaces.Add(pos);
                        continue;
                    }
                }
            }
            foreach (var pos in beacons.Concat(sensors.Select(s => s.Pos)))
            {
                if (spaces.Contains(pos))
                {
                    spaces.Remove(pos);
                } 
            }
            Console.WriteLine(spaces.Count);
        }

        IEnumerable<(int X, int Y, int Size)> Partition((int X, int Y, int Size) rect)
        {
            return Partition((rect.X, rect.Y), rect.Size);
        }

        IEnumerable<(int X, int Y, int Size)> Partition((int X, int Y) topLeft, int size)
        {
            var middle = size/2;
            yield return (topLeft.X, topLeft.Y, middle);
            yield return (topLeft.X + middle + 1, topLeft.Y, middle);
            yield return (topLeft.X, topLeft.Y + middle + 1, middle);
            yield return (topLeft.X + middle + 1, topLeft.Y + middle + 1, middle);
        }

        public override void PartTwoV1(List<string> lines)
        {
            List<Sensor> sensors = new();
            HashSet<(int x, int y)> beacons = new();
            foreach (var line in lines)
            {
                var match = Regex.Match(line, @"Sensor at x=(?<sx>-?\d+), y=(?<sy>-?\d+): closest beacon is at x=(?<bx>-?\d+), y=(?<by>-?\d+)");
                if (match.Success)
                {
                    (int X, int Y) beacon = (int.Parse(match.Groups["bx"].Value), int.Parse(match.Groups["by"].Value));
                    beacons.Add(beacon);
                    sensors.Add(new Sensor
                    {
                        Pos = (int.Parse(match.Groups["sx"].Value), int.Parse(match.Groups["sy"].Value)),
                        BeaconPos = beacon
                    });
                }
            }

            int max = 4_000_000;
            if (Debug) max = 20;

            Queue<(int X, int Y, int Size)> q = new();
            //foreach (var part in Partition((0,0), max))
            //{
            //    q.Enqueue(part);
            //}
            Partition((0, 0), max).ToList().ForEach(p => q.Enqueue(p));
            while (q.Any())
            {
                var rect = q.Dequeue();

                if (rect.Size <= (Debug ? 5 : 100))
                {
                    for (int y = rect.Y; y < rect.Y + rect.Size; y++)
                    {
                        for (int x = rect.X; x < rect.X + rect.Size; x++)
                        {
                            (int X, int Y) pos = (x, y);

                            bool match = false;
                            foreach (var sensor in sensors)
                            {
                                var dist = Math.Abs(sensor.Pos.X - pos.X) + Math.Abs(sensor.Pos.Y - pos.Y);
                                if (dist <= sensor.Distance)
                                {
                                    match = true;
                                    break;
                                }
                            }
                            if (match)
                            {
                                continue;
                            }
                            if (beacons.Concat(sensors.Select(s => s.Pos)).Contains(pos)) continue;

                            Console.WriteLine($"{x} {y} = {(long)x * (long)4_000_000 + y}");
                            return;
                        }
                    }
                }

                if (!sensors.Any(s => s.IsWithin(rect)))
                {
                    foreach(var p in Partition(rect))
                    {
                        q.Enqueue(p);
                    }
                }
            }

            
        }
    }
}
