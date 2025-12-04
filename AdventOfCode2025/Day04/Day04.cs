using Helpers.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2025.Day04;

public class Day04 : BaseAocV2
{
    public override object PartOne(List<string> lines)
    {
        var map = lines.ToGrid();

        return map.Count(cell => cell.Value == '@' && cell.GetNeighbours(true).Count(cell => cell.Value == '@') < 4);
    }

    public override object PartTwo(List<string> lines)
    {
        var map = lines.ToGrid();
        var count = 0;
        bool removed = false;
        do
        {
            removed = false;
            foreach (var cell in map)
            {
                if (cell.Value == '@' && cell.GetNeighbours(true).Count(cell => cell.Value == '@') < 4)
                {
                    count++;
                    cell.Value = 'x';
                    removed = true;
                }
            }
        } while (removed);

        return count;
    }
}
