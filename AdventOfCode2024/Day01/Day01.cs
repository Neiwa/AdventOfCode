namespace AdventOfCode2024.Day01;

public class Day01 : BaseAocV2
{
    public override object PartOne(List<string> lines)
    {
        var list1 = new List<long>();
        var list2 = new List<long>();

        lines.ForEach(l =>
        {
            var s = l.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            list1.Add(long.Parse(s[0]));
            list2.Add(long.Parse(s[1]));

        });

        list1.Sort();
        list2.Sort();

        return list1.Zip(list2).Select((t)=> Math.Abs(t.First - t.Second)).Sum();
    }

    public override object PartTwo(List<string> lines)
    {
        var list1 = new List<long>();
        var list2 = new List<long>();

        lines.ForEach(l =>
        {
            var s = l.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            list1.Add(long.Parse(s[0]));
            list2.Add(long.Parse(s[1]));

        });

        return list1.Select(v => list2.Count(v2 => v2 == v) * v).Sum();
    }
}
