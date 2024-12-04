using Helpers;
using Helpers.Grid;

namespace AdventOfCode2024.Day04
{
    public class Day04 : BaseAocV2
    {
        public override object PartOne(List<string> lines)
        {
            var grid = lines.ToGrid();
            var count = 0;

            foreach (var cell in grid)
            {
                if (cell.Value == 'X')
                {
                    foreach (var neighbour in cell.GetNeighbours(true))
                    {
                        if (neighbour.Value == 'M')
                        {
                            var direction = neighbour - cell;

                            var aPos = neighbour + direction;
                            var sPos = neighbour + direction + direction;
                            if (aPos.IsValid() && sPos.IsValid() && aPos.Value == 'A' && sPos.Value == 'S')
                            {
                                count++;
                            }
                        }
                    }
                }
            }

            return count;
        }

        public override object PartTwo(List<string> lines)
        {
            var grid = lines.ToGrid();
            var count = 0;

            foreach (var cell in grid)
            {
                if (cell.Value == 'A')
                {
                    var n1 = cell + new Point(1, 1);
                    var n2 = cell + new Point(-1, 1);
                    var n3 = cell + new Point(-1, -1);
                    var n4 = cell + new Point(1, -1);
                    
                    if (n1.IsValid() && n2.IsValid() && n3.IsValid() && n4.IsValid())
                    {
                        var s = $"{n1.Value}{n2.Value}{n3.Value}{n4.Value}";
                        switch (s)
                        {
                            case "MSSM":
                            case "MMSS":
                            case "SMMS":
                            case "SSMM":
                                count++;
                                break;
                            default:
                                continue;
                        }
                    }
                }
            }

            return count;
        }
    }
}
