using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2025.Day02;

public class Day02 : BaseAocV2
{
    public override object PartOne(List<string> lines)
    {
        var ranges = lines[0].Split(',').Select(range => range.Split('-')).Select(v => (start: long.Parse(v[0]), stop: long.Parse(v[1])));

        long sum = 0;

        foreach (var (start, stop) in ranges)
        {
            for (var i = start; i <= stop; i++)
            {
                var str = i.ToString();
                var middle = str.Length / 2;
                if (str[..middle] == str[middle..])
                {
                    WriteLine(str);
                    sum += i;
                }
            }

        }

        return sum;
    }

    public override object PartTwo(List<string> lines)
    {
        var ranges = lines[0].Split(',').Select(range => range.Split('-')).Select(v => (start: long.Parse(v[0]), stop: long.Parse(v[1])));

        long sum = 0;

        foreach (var (start, stop) in ranges)
        {
            for (var i = start; i <= stop; i++)
            {
                var str = i.ToString();
                if (IsSilly(str, 1))
                {
                    WriteLine(str);
                    sum += i;
                }
            }

        }

        return sum;
    }

    private bool IsSilly(string str, int len)
    {
        if (len * 2 > str.Length)
        {
            return false;
        }

        if (str.Length % len == 0)
        {
            var part = str[..len];
            var pos = len;
            bool silly = true;
            while (pos <= str.Length - len)
            {
                if (str.Substring(pos, len) != part)
                {
                    silly = false;
                    break;
                }
                pos += len;
            }
            if (silly) return true;
        }

        return IsSilly(str, len + 1);
    }
}
