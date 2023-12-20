using Core;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023.Day03
{
    public class Map
    {
        public List<(string, Rectangle)> Numbers { get; set; } = new List<(string, Rectangle)> ();
        public Dictionary<IntPoint, char> Symbols { get; set; } = new Dictionary<IntPoint, char>();
    }

    public class Day03 : BaseAoc<Map>
    {
        public override string PartOne(Map input)
        {
            var sum = 0;
            foreach (var number in input.Numbers)
            {
                var expandedRectangle = new Rectangle(number.Item2.Position.X-1, number.Item2.Position.Y-1, number.Item2.Width+2, number.Item2.Height+2);
                if(input.Symbols.Keys.Any(k => expandedRectangle.Contains(k)))
                {
                    WriteLine($"{number.Item1}", ActionLevel.Trace);
                    sum += int.Parse(number.Item1);
                }
            }

            return sum.ToString();
        }

        public override string PartTwo(Map input)
        {
            var sum = 0;
            foreach(var symbol in input.Symbols.Where(s => s.Value == '*'))
            {
                var numbers = input.Numbers.Where(number => {
                    var expandedRectangle = new Rectangle(number.Item2.Position.X - 1, number.Item2.Position.Y - 1, number.Item2.Width + 2, number.Item2.Height + 2);
                    return expandedRectangle.Contains(symbol.Key);
                }).Take(3).ToList();
                if(numbers.Count == 2)
                {
                    sum += int.Parse(numbers[0].Item1) * int.Parse(numbers[1].Item1);
                }
            }

            return sum.ToString();
        }

        protected override Map ParseInput(List<string> lines)
        {
            var map = new Map();

            for (int i = 0; i < lines.Count; i++)
            {
                string buffer = "";
                for(int j = 0; j < lines[i].Length; j++)
                {
                    if (lines[i][j] == '.')
                    {
                        if (buffer.Length > 0)
                        {
                            map.Numbers.Add((buffer, new Rectangle(j - buffer.Length, i, buffer.Length, 1)));
                            buffer = "";
                        }
                    }
                    else if ("1234567890".Contains(lines[i][j]))
                    {
                        buffer += lines[i][j];
                    }
                    else
                    {
                        map.Symbols.Add(new IntPoint(j, i), lines[i][j]);
                        if (buffer.Length > 0)
                        {
                            map.Numbers.Add((buffer, new Rectangle(j - buffer.Length, i, buffer.Length, 1)));
                            buffer = "";
                        }
                    }
                }
                if (buffer.Length > 0)
                {
                    map.Numbers.Add((buffer, new Rectangle(lines[i].Length - buffer.Length, i, buffer.Length, 1)));
                    buffer = "";
                }
            }

            return map;
        }
    }
}
