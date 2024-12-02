namespace AdventOfCode2024.Day01;

public class Day02 : BaseAocV2
{
    public override object PartOne(List<string> lines)
    {
        return lines.Sum(l =>
        {
            var nums = l.Split(' ').Select(v => long.Parse(v)).ToList();
            var incr = nums[0] < nums[1];
            var diff = Math.Abs(nums[0] - nums[1]);
            switch (diff)
            {
                case 1:
                case 2:
                case 3:
                    break;
                default:
                    return 0;
            }
            for (int i = 1; i < nums.Count; i++)
            {
                var dir = nums[i - 1] < nums[i];
                if (dir != incr)
                {
                    return 0;
                }

                diff = Math.Abs(nums[i-1] - nums[i]);
                switch (diff)
                {
                    case 1:
                    case 2:
                    case 3:
                        break;
                    default:
                        return 0;
                }
            }
            return 1;
        });
    }

    public override object PartTwo(List<string> lines)
    {
        return lines.Sum(l =>
        {
            var nums = l.Split(' ').Select(v => long.Parse(v)).ToList();
            if (IsSafe(nums))
            {
                return 1;
            }

            for (int i = 0; i < nums.Count; i++)
            {
                var nums2 = new List<long>(nums);
                nums2.RemoveAt(i);
                if (IsSafe(nums2))
                {
                    return 1;
                }
            }

            return 0;
        });
    }

    public bool IsSafe(List<long> nums)
    {
        var incr = nums[0] < nums[1];
        var diff = Math.Abs(nums[0] - nums[1]);
        switch (diff)
        {
            case 1:
            case 2:
            case 3:
                break;
            default:
                return false;
        }
        for (int i = 1; i < nums.Count; i++)
        {
            var dir = nums[i - 1] < nums[i];
            if (dir != incr)
            {
                return false;
            }

            diff = Math.Abs(nums[i - 1] - nums[i]);
            switch (diff)
            {
                case 1:
                case 2:
                case 3:
                    break;
                default:
                    return false;
            }
        }

        return true;
    }
}
