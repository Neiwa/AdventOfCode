using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2025.Day01;

public class Day01 : BaseAocV2
{
    public override object PartOne(List<string> lines)
    {
        var currentPos = 50;
        var counter = 0;

        foreach(var line in lines)
        {
            var right = line[0] == 'R';
            if (right)
            {
                currentPos = (currentPos + int.Parse(line[1..])) % 100;
            }
            else
            {
                currentPos = (currentPos - int.Parse(line[1..])) % 100;
            }

            if (currentPos == 0)
            {
                counter++;
            }
        }

        return counter;
    }

    public override object PartTwo(List<string> lines)
    {
        var currentPos = 50;
        var counter = 0;

        foreach (var line in lines)
        {
            var right = line[0] == 'R';
            var count = int.Parse(line[1..]);

            for (var i = 0; i < count; i++)
            {

                if (right)
                {
                    currentPos = (currentPos + 1) % 100;
                }
                else
                {
                    currentPos = (currentPos - 1) % 100;
                }

                if (currentPos == 0)
                {
                    counter++;
                }
            }
        }

        return counter;
    }
}
