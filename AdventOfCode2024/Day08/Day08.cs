using Helpers.Grid;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Day08
{
    public class Day08 : BaseAocV2
    {
        void Draw(Grid<char> grid, Grid<char> grid2)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    Write(grid.ValueAt(x, y));
                }
                Write(" | ");
                for (int x = 0; x < grid2.Width; x++)
                {
                    Write(grid2.ValueAt(x, y));
                }
                WriteLine();
            }
        }

        public override object PartOne(List<string> lines)
        {
            var map = lines.ToFixedGrid();

            var antinodeLocations = new HashSet<Point>();

            var antinodeMap = new FixedGrid(map.Width, map.Height, '.');

            var antennaKinds = map.Where(n => n.Value != '.').GroupBy(n => n.Value);

            foreach (var antennaKind in antennaKinds)
            {
                var nodes = antennaKind.ToList();
                for (var i = 0; i < nodes.Count - 1; i++)
                {
                    for (var j = i + 1; j < nodes.Count; j++)
                    {
                        antinodeMap.SetAt(nodes[i], '@');
                        antinodeMap.SetAt(nodes[j], '@');

                        var antiNodeOne = nodes[i] + (Point)(nodes[i] - nodes[j]);
                        if (antiNodeOne.IsValid())
                        {
                            antinodeLocations.Add(antiNodeOne);
                            antinodeMap.SetAt(antiNodeOne, '+');
                            WriteLine($"{antennaKind.Key}: {(Point)nodes[i]} {(Point)nodes[j]} {antiNodeOne.Point}");
                            Draw(map, antinodeMap);
                            WriteLine();
                            antinodeMap.SetAt(antiNodeOne, '#');
                        }

                        var antiNodeTwo = nodes[j] - (Point)(nodes[i] - nodes[j]);
                        if (antiNodeTwo.IsValid())
                        {
                            antinodeLocations.Add(antiNodeTwo);
                            antinodeMap.SetAt(antiNodeTwo, '+');
                            WriteLine($"{antennaKind.Key}: {(Point)nodes[i]} {(Point)nodes[j]} {antiNodeTwo.Point}");
                            Draw(map, antinodeMap);
                            WriteLine();
                            antinodeMap.SetAt(antiNodeTwo, '#');
                        }

                        antinodeMap.SetAt(nodes[i], antennaKind.Key);
                        antinodeMap.SetAt(nodes[j], antennaKind.Key);
                    }
                }
            }

            Draw(map, antinodeMap);

            return antinodeLocations.Count;
        }

        public override object PartTwo(List<string> lines)
        {
            var map = lines.ToFixedGrid();

            var antinodeLocations = new HashSet<Point>();

            var antennaKinds = map.Where(n => n.Value != '.').GroupBy(n => n.Value);

            foreach (var antennaKind in antennaKinds)
            {
                var nodes = antennaKind.ToList();
                for (var i = 0; i < nodes.Count - 1; i++)
                {
                    for (var j = i + 1; j < nodes.Count; j++)
                    {
                        Point step = nodes[i] - nodes[j];

                        var antiNodeOne = nodes[i];
                        do
                        {
                            antinodeLocations.Add(antiNodeOne);
                            antiNodeOne += step;
                        } while (antiNodeOne.IsValid());

                        var antiNodeTwo = nodes[j];
                        do
                        {
                            antinodeLocations.Add(antiNodeTwo);
                            antiNodeTwo -= step;
                        } while (antiNodeTwo.IsValid());
                    }
                }
            }

            return antinodeLocations.Count;
        }
    }
}
