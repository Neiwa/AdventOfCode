using Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022.Day12
{
    public class Day12 : BaseAoc
    {

        List<T> AStar<T>(T start, T goal, Func<T, int> h, Func<T, T, int> compare, Func<T, IEnumerable<T>> getNeighbors)
        {
            List<T> openSet = new();
            openSet.Add(start);
            Dictionary<T, T> cameFrom = new();
            Dictionary<T, int> gScore = new();
            gScore.Add(start, 0);
            Dictionary<T, int> fScore = new();
            fScore.Add(start, h(start));
            int getGScore(T pos)
            {
                return gScore.ContainsKey(pos) ? gScore[pos] : int.MaxValue;
            }
            int getFScore(T pos)
            {
                return fScore.ContainsKey(pos) ? fScore[pos] : int.MaxValue;
            }

            while (openSet.Count > 0)
            {
                var current = openSet.MinBy(n => getGScore(n));
                if (compare(current, goal) == 0)
                {
                    List<T> path = new() { current };
                    while(cameFrom.Keys.Any(n => compare(n, current) == 0))
                    {
                        current = cameFrom[current];
                        path.Add(current);
                    }
                    path.Reverse();
                    return path;
                }

                openSet.Remove(current);
                foreach (var neighbor in getNeighbors(current))
                {
                    var tentative_gScore = getGScore(current) + compare(current, neighbor);
                    if(tentative_gScore < getGScore(neighbor))
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentative_gScore;
                        fScore[neighbor] = tentative_gScore + h(neighbor);
                        if(!openSet.Any(n => compare(n, neighbor) == 0))
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }
            }
            return new();
        }

        public override void PartOne(List<string> lines)
        {
            (int X, int Y) S_pos = (0, 0);
            (int X, int Y) E_pos = (0, 0);
            for (int y = 0; y < lines.Count; y++)
            {
                for (int x = 0; x < lines[0].Length; x++)
                {
                    switch (lines[y][x])
                    {
                        case 'S':
                            S_pos = (x, y);
                            break;
                        case 'E':
                            E_pos = (x, y);
                            break;
                    }
                }
            }

            var path = AStar(S_pos, E_pos, h, (a, b) =>
            {
                if (a == b) return 0;
                if (getPos(a) > getPos(b))
                    return 2;
                return 1;
            }, n =>
            {
                List<(int X, int Y)> ns = new();

                foreach (var pos in new[]
                {
                    (n.X, n.Y - 1),
                    (n.X, n.Y + 1),
                    (n.X - 1, n.Y),
                    (n.X + 1, n.Y)
                })
                {
                    try
                    {
                        char next = getPos(pos);
                        char curr = getPos(n);
                        if (curr + 1 >= next)
                        {
                            ns.Add(pos);
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                    }
                    catch(ArgumentOutOfRangeException)
                    { }
                }

                return ns;
            });

            char getPos((int X, int Y) pos)
            {
                char c = lines[pos.Y][pos.X];
                switch(c)
                {
                    case 'S':
                        return 'a';
                    case 'E':
                        return 'z';
                }
                return c;
            };

            int h((int X, int Y) currPos)
            {
                return (int)Math.Sqrt(Math.Pow(Math.Abs(currPos.X - E_pos.X), 2) + Math.Pow(Math.Abs(currPos.Y - E_pos.Y), 2));
            }
            if (Debug)
            {
                foreach (var item in path)
                {
                    Console.WriteLine($"{item.X}, {item.Y} => {getPos(item)}");
                }
                for (int y = 0; y < lines.Count; y++)
                {
                    for (int x = 0; x < lines[0].Length; x++)
                    {
                        var i = path.FindIndex(e => e == (x, y));
                        if (i >= 0) Console.Write(i % 10);
                        else Console.Write(".");
                    }
                    Console.WriteLine();
                } 
            }

            Console.WriteLine($"{path.Count - 1}");
        }

        public override void PartTwo(List<string> lines)
        {
            (int X, int Y) S_pos = (0, 0);
            (int X, int Y) E_pos = (0, 0);
            for (int y = 0; y < lines.Count; y++)
            {
                for (int x = 0; x < lines[0].Length; x++)
                {
                    switch (lines[y][x])
                    {
                        case 'S':
                            S_pos = (x, y);
                            break;
                        case 'E':
                            E_pos = (x, y);
                            break;
                    }
                }
            }

            Dictionary<(int X, int Y), List<(int X, int Y)>> ss = new();
            foreach (var pos in Enumerable.Range(0, lines.Count).SelectMany(y => Enumerable.Range(0, lines[0].Length).Select(x => (x, y))))
            {
                if (getPos(pos) != 'a' || (getPos(pos) == 'a' && !getNeighbors(pos).Any(n => getPos(n) == 'b')))
                {
                    continue;
                }
                var p = AStar(pos, E_pos, h, (a, b) =>
                {
                    if (a == b) return 0;
                    if (getPos(a) > getPos(b))
                        return 2;
                    return 1;
                }, getNeighbors);

                ss.Add(pos, p);
            }

            var path = ss.Values.MinBy(p => p.Count);
            Console.WriteLine($"{path.Count - 1}");

            if (Debug)
            {
                foreach (var key in ss.Keys)
                {
                    Console.WriteLine($"{key} {ss[key].Count}");
                }


                foreach (var item in path)
                {
                    Console.WriteLine($"{item.X}, {item.Y} => {getPos(item)}");
                }
                for (int y = 0; y < lines.Count; y++)
                {
                    for (int x = 0; x < lines[0].Length; x++)
                    {
                        var i = path.FindIndex(e => e == (x, y));
                        if (i >= 0) Console.Write(i % 10);
                        else Console.Write(".");
                    }
                    Console.WriteLine();
                } 
            }



            IEnumerable<(int X, int Y)> getNeighbors((int X, int Y) n)
            {
                List<(int X, int Y)> ns = new();

                foreach (var pos in new[]
                {
                    (n.X, n.Y - 1),
                    (n.X, n.Y + 1),
                    (n.X - 1, n.Y),
                    (n.X + 1, n.Y)
                                })
                {
                    try
                    {
                        char next = getPos(pos);
                        char curr = getPos(n);
                        if (curr + 1 >= next)
                        {
                            ns.Add(pos);
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                    }
                    catch (ArgumentOutOfRangeException)
                    { }
                }

                return ns;
            }

            char getPos((int X, int Y) pos)
            {
                char c = lines[pos.Y][pos.X];
                switch (c)
                {
                    case 'S':
                        return 'a';
                    case 'E':
                        return 'z';
                }
                return c;
            };

            int h((int X, int Y) currPos)
            {
                return (int)Math.Sqrt(Math.Pow(Math.Abs(currPos.X - E_pos.X), 2) + Math.Pow(Math.Abs(currPos.Y - E_pos.Y), 2));
            }
        }
    }
}
