using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2022.Day14
{
    internal class Day14 : BaseAocV1
    {
        IEnumerable<(int X, int Y)> getLine((int X, int Y) start, (int X, int Y) end)
        {
            var current = start;
            int mod = 1;
            if (start.X == end.X)
            {
                if (start.Y > end.Y)
                {
                    mod = -1;
                }
                while (current.Y != end.Y)
                {
                    yield return (current.X, current.Y);
                    current.Y += mod;
                }
            }
            else
            {
                if (start.X > end.X)
                {
                    mod = -1;
                }
                while (current.X != end.X)
                {
                    yield return (current.X, current.Y);
                    current.X += mod;
                }
            }

            yield return current;
        }

        void Draw(Dictionary<(int X, int Y), char> map, (int X, int Y) topLeft, (int X, int Y) bottomRight, int? floorY = null)
        {
            if (!Debug) return;

            for (int y = topLeft.Y; y < bottomRight.Y + 1; y++)
            {
                for (int x = topLeft.X; x < bottomRight.X + 1; x++)
                {
                    if(floorY.HasValue && y == floorY)
                    {
                        Console.Write('#');
                    }
                    else if (map.TryGetValue((x, y), out char c))
                    {
                        Console.Write(c);
                    }
                    else
                    {
                        Console.Write('.');
                    }
                }
                Console.WriteLine();
            }
        }

        public override void PartOneV1(List<string> lines)
        {
            Dictionary<(int X, int Y), char> cave = new();
            int minX = 500;
            int maxX = 500;
            int maxY = 0;
            foreach (var line in lines)
            {
                var matches = Regex.Matches(line, @"(?<x>\d+),(?<y>\d+)");
                if(matches.Count > 1)
                {
                    (int X, int Y) startPos = (int.Parse(matches[0].Groups["x"].Value), int.Parse(matches[0].Groups["y"].Value));
                    for (int i = 1; i < matches.Count; i++)
                    {
                        (int X, int Y) nextPos = (int.Parse(matches[i].Groups["x"].Value), int.Parse(matches[i].Groups["y"].Value));
                        foreach (var pos in getLine(startPos, nextPos))
                        {
                            cave.TryAdd(pos, '#');

                            if (pos.Y > maxY) maxY = pos.Y;
                            if (pos.X > maxX) maxX = pos.X;
                            if (pos.X < minX) minX = pos.X;
                        }
                        startPos = nextPos;
                    }
                }
            }
            bool sanding = true;
            while(sanding)
            {
                (int X, int Y) sandPos = (500, 0);

                while (true)
                {
                    // Out of map
                    if(sandPos.Y > maxY)
                    {
                        sanding = false;
                        break;
                    }
                    // Down
                    else if(!cave.ContainsKey((sandPos.X, sandPos.Y + 1)))
                    {
                        sandPos.Y += 1;
                    }
                    // Left-Down
                    else if (!cave.ContainsKey((sandPos.X - 1, sandPos.Y + 1)))
                    {
                        sandPos.X -= 1;
                        sandPos.Y += 1;
                    }
                    // Right-Down
                    else if (!cave.ContainsKey((sandPos.X + 1, sandPos.Y + 1)))
                    {
                        sandPos.X += 1;
                        sandPos.Y += 1;
                    }
                    // Stop
                    else
                    {
                        cave.Add(sandPos, 'O');
                        break;
                    }

                }
            }
            if (Debug)
            {
                cave.TryAdd((500, 0), '+');
            }
            Draw(cave, (minX - 2, 0), (maxX + 2, maxY + 1));

            Console.WriteLine($"Sand count: {cave.Values.Count(c => c == 'O')}");
        }

        public override void PartTwoV1(List<string> lines)
        {
            Dictionary<(int X, int Y), char> cave = new();
            int minX = 500;
            int maxX = 500;
            int maxY = 0;
            foreach (var line in lines)
            {
                var matches = Regex.Matches(line, @"(?<x>\d+),(?<y>\d+)");
                if (matches.Count > 1)
                {
                    (int X, int Y) startPos = (int.Parse(matches[0].Groups["x"].Value), int.Parse(matches[0].Groups["y"].Value));
                    for (int i = 1; i < matches.Count; i++)
                    {
                        (int X, int Y) nextPos = (int.Parse(matches[i].Groups["x"].Value), int.Parse(matches[i].Groups["y"].Value));
                        foreach (var pos in getLine(startPos, nextPos))
                        {
                            cave.TryAdd(pos, '#');

                            if (pos.Y > maxY) maxY = pos.Y;
                            if (pos.X > maxX) maxX = pos.X;
                            if (pos.X < minX) minX = pos.X;
                        }
                        startPos = nextPos;
                    }
                }
            }

            int floorY = maxY + 1;
            bool sanding = true;
            while (sanding)
            {
                (int X, int Y) sandPos = (500, 0);

                while (true)
                {
                    // Out of map
                    if (sandPos.Y == floorY)
                    {
                        cave.Add(sandPos, 'O');
                        break;
                    }
                    // Down
                    else if (!cave.ContainsKey((sandPos.X, sandPos.Y + 1)))
                    {
                        sandPos.Y += 1;
                    }
                    // Left-Down
                    else if (!cave.ContainsKey((sandPos.X - 1, sandPos.Y + 1)))
                    {
                        sandPos.X -= 1;
                        sandPos.Y += 1;
                    }
                    // Right-Down
                    else if (!cave.ContainsKey((sandPos.X + 1, sandPos.Y + 1)))
                    {
                        sandPos.X += 1;
                        sandPos.Y += 1;
                    }
                    // Stop
                    else
                    {
                        if(!cave.TryAdd(sandPos, 'O'))
                        {
                            sanding = false;
                        }
                        break;
                    }

                }
            }

            minX = Math.Min(minX, 500 - maxY);
            maxX = Math.Max(maxX, 500 + maxY);

            Draw(cave, (minX - 3, 0), (maxX + 3, maxY + 2), maxY + 2);

            Console.WriteLine($"Sand count: {cave.Values.Count(c => c == 'O')}");
        }
    }
}
