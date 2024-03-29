﻿using Core;
using Helpers;
using Helpers.Grid;

namespace AdventOfCode2023.Day10
{
    public class Day10 : BaseAoc<FixedIntGrid>
    {
        public bool[] GetPipeConfig(char p)
        {
            switch (p)
            {
                case '|':
                    return new[] { true, false, true, false }; // 0b_1010;
                case '-':
                    return new[] { false, true, false, true }; // 0b_0101;
                case 'L':
                    return new[] { true, true, false, false }; // 0b_1100;
                case 'J':
                    return new[] { true, false, false, true }; // 0b_1001;
                case '7':
                    return new[] { false, false, true, true }; // 0b_0011;
                case 'F':
                    return new[] { false, true, true, false }; // 0b_0110;
                case 'S':
                    return new[] { true, true, true, true }; // 0b_1111;
                default:
                    return new[] { false, false, false, false }; // 0b_0000;
            }
        }

        public bool Connected(FixedIntGridCellReference<char> left, FixedIntGridCellReference<char> right)
        {
            //   R
            // - L -
            //   |
            if (left - right == new IntPoint(0, 1))
            {
                return GetPipeConfig(left.Value)[0] && GetPipeConfig(right.Value)[2];
            }
            //   |
            // - L R
            //   |
            if (left - right == new IntPoint(-1, 0))
            {
                return GetPipeConfig(left.Value)[1] && GetPipeConfig(right.Value)[3];
            }
            //   |
            // - L -
            //   R
            if (left - right == new IntPoint(0, -1))
            {
                return GetPipeConfig(left.Value)[2] && GetPipeConfig(right.Value)[0];
            }
            //   |
            // R L -
            //   |
            if (left - right == new IntPoint(1, 0))
            {
                return GetPipeConfig(left.Value)[3] && GetPipeConfig(right.Value)[1];
            }
            return false;
        }

        public void Draw(FixedIntGrid map, IntPoint? pos = null)
        {
            if (!IsTrace) { return; }

            for (int y = 0; y < map.Height; y++)
            {
                for (global::System.Int32 x = 0; x < map.Width; x++)
                {
                    if (pos is not null && pos.X == x && pos.Y == y)
                    {
                        Write('O');
                    }
                    else
                    {
                        Write(map.ValueAt(x, y));
                    }
                }
                WriteLine();
            }
        }

        public override string PartOne(FixedIntGrid input)
        {
            var startPos = input.First(c => c.Value == 'S');
            var currPos = startPos;
            var lastPos = startPos.GetNeighbours().First();
            var length = 0;

            do
            {
                foreach (var pos in currPos.GetNeighbours())
                {
                    if (pos == lastPos) continue;

                    if (Connected(pos, currPos))
                    {
                        lastPos = currPos;
                        currPos = pos;
                        length++;
                        Draw(input, currPos.Point);
                        break;
                    }
                }
            } while (startPos != currPos);

            return (length / 2).ToString();
        }

        public override string PartTwo(FixedIntGrid input)
        {
            // Plot real map

            // Explode map, i.e. start in top left, expand each to 3x3, so
            // F becomes ...
            //           .F-
            //           .|.
            // and so on

            var realMap = new FixedIntGrid(input.Width * 3, input.Height * 3, '.');
            var cleanMap = new FixedIntGrid(input.Width, input.Height, '.');

            var startPos = input.First(c => c.Value == 'S');
            var currPos = startPos;
            var lastPos = startPos.GetNeighbours().First();
            var length = 0;

            do
            {
                foreach (var pos in currPos.GetNeighbours())
                {
                    if (pos == lastPos) continue;

                    if (Connected(pos, currPos))
                    {
                        var conf = GetPipeConfig(pos.Value);
                        var realMapPos = realMap.At(pos.Point * 3 + new IntPoint(1, 1));
                        realMapPos.Value = pos.Value;
                        if (conf[0]) (realMapPos + new IntPoint(0, -1)).Value = '|';
                        if (conf[1]) (realMapPos + new IntPoint(1, 0)).Value = '-';
                        if (conf[2]) (realMapPos + new IntPoint(0, 1)).Value = '|';
                        if (conf[3]) (realMapPos + new IntPoint(-1, 0)).Value = '-';
                        //Draw(realMap, realMapPos.Point);

                        cleanMap.At(pos.Point).Value = pos.Value;

                        lastPos = currPos;
                        currPos = pos;
                        length++;
                        //Draw(input, currPos.Point);
                        break;
                    }
                }
            } while (startPos != currPos);

            Draw(realMap);
            Draw(cleanMap);
            // Flood map
            // choose a . in edge and flood from there (replace . with W). Collapse map and count remaining .
            // Collapse, keep only center of every expanded node

            HashSet<IntPoint> fillPoints = new HashSet<IntPoint>
            {
                new IntPoint(0, 0)
            };

            while (fillPoints.Any())
            {
                var fillPoint = fillPoints.First();
                fillPoints.Remove(fillPoint);
                var fillPos = realMap.At(fillPoint);

                if (fillPos.Value == '.')
                {
                    fillPos.Value = 'W';
                    if (fillPos.X % 3 == 1 && fillPos.Y % 3 == 1)
                    {
                        cleanMap.At(fillPos.Point / 3).Value = 'W';
                    }
                }
                foreach (var pos in fillPos.GetNeighbours())
                {
                    if (pos.Value == '.')
                    {
                        fillPoints.Add(pos.Point);
                    }
                }
            }
            Draw(realMap);
            Draw(cleanMap);

            return cleanMap.Count(c => c.Value == '.').ToString();
        }

        protected override FixedIntGrid ParseInput(List<string> lines)
        {
            return lines.ToFixedIntGrid();
        }
    }
}
