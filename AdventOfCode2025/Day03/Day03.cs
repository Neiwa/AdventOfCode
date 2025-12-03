using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2025.Day03;

public class Day03 : BaseAocV2
{
    public override object PartOne(List<string> lines)
    {
        long sum = 0;

        foreach(var bank in lines)
        {
            var firstDigit = bank[..^1].Max();
            var secondDigit = bank[(bank.IndexOf(firstDigit)+1)..].Max();

            var max = int.Parse($"{firstDigit}{secondDigit}");

            WriteLine(max.ToString());

            sum += max;
        }

        return sum;
    }

    public override object PartTwo(List<string> lines)
    {
        long sum = 0;

        foreach (var bank in lines)
        {
            var max = Finder(bank, 12);

            WriteLine(max);

            sum += long.Parse(max);
        }

        return sum;
    }

    private static string Finder(string bank, int length)
    {
        if (length == 0) return "";

        if (length == 1)
        {
            return bank.Max().ToString();
        }

        var digit = bank[..^(length - 1)].Max();
        var index = bank.IndexOf(digit);

        return $"{digit}{Finder(bank[(index + 1)..], length - 1)}";
    }
}
