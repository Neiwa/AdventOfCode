using Core;
using Helpers;

namespace AdventOfCode2023.Day13
{
    public class Day13 : BaseAocV2
    {
        public void Draw(Grid grid, Rectangle left, Rectangle reflection)
        {
            if (!IsTrace) return;

            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    var cell = grid.At(x, y);
                    if (left.Contains(cell))
                    {
                        Write($"[blue]{cell.Value}[/]");
                    }
                    else if (reflection.Contains(cell))
                    {
                        Write($"[red]{cell.Value}[/]");
                    }
                    else
                    {
                        Write($"{cell.Value}");
                    }
                }
                WriteLine();
            }
        }

        public override string PartOne(List<string> lines)
        {
            var grids = new List<Grid>();
            var start = 0;

            for (int i = 0; i <= lines.Count; i++)
            {
                if (i == lines.Count || string.IsNullOrEmpty(lines[i]))
                {
                    grids.Add(lines.Skip(start).Take(i - start).ToGrid());
                    start = i + 1;
                }
            }

            var sum = 0;

            foreach (var grid in grids)
            {
                var reflectionFound = false;

                // Horizontal reflection
                for (int hPos = 1; hPos < grid.Height; hPos++)
                {
                    var validReflection = true;

                    var height = Math.Min(hPos, grid.Height - hPos);
                    var bounds = new Rectangle(0, hPos - height, grid.Width, height);

                    foreach (var cell in grid.Where(c => bounds.Contains(c)))
                    {
                        //  0
                        //  1
                        //  2 c
                        //  3 
                        // v4
                        // ^5
                        //  6 
                        //  7 r

                        var reflection = cell + new Point(0, (hPos - cell.Y) * 2 - 1);
                        if (reflection.Value != cell.Value)
                        {
                            validReflection = false;
                            break;
                        }
                    }

                    if (validReflection)
                    {
                        Draw(grid, bounds, new Rectangle(0, hPos, grid.Width, height));
                        reflectionFound = true;
                        sum += hPos * 100;
                        WriteLine($"Reflection between row {hPos - 1} and {hPos}");
                        break;
                    }
                }

                if (reflectionFound)
                {
                    continue;
                }

                // Vertical reflection
                for (int vPos = 1; vPos < grid.Width; vPos++)
                {
                    var validReflection = true;

                    var width = Math.Min(vPos, grid.Width - vPos);
                    var bounds = new Rectangle(vPos - width, 0, width, grid.Height);

                    foreach (var cell in grid.Where(c => bounds.Contains(c)))
                    {
                        var reflection = cell + new Point((vPos - cell.X) * 2 - 1, 0);
                        if (reflection.Value != cell.Value)
                        {
                            validReflection = false;
                            break;
                        }
                    }

                    if (validReflection)
                    {
                        Draw(grid, bounds, new Rectangle(vPos, 0, width, grid.Height));
                        reflectionFound = true;
                        sum += vPos;
                        WriteLine($"Reflection between col {vPos - 1} and {vPos}");
                        break;
                    }
                }
            }

            return sum.ToString();
        }

        public override string PartTwo(List<string> lines)
        {
            throw new NotImplementedException();
        }
    }
}
