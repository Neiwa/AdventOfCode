using Core;
using Helpers;

namespace AdventOfCode2023.Day12
{
    [TestFixture]
    public class Day12 : BaseAocV2
    {
        [TestCase("#.#.### 1,1,3")]
        [TestCase(".#...#....###. 1,1,3")]
        [TestCase(".#.###.#.###### 1,3,1,6")]
        [TestCase("####.#...#... 4,1,1")]
        [TestCase("#....######..#####. 1,6,5")]
        [TestCase(".###.##....# 3,2,1")]
        public void IsValid_Tests(string input)
        {
            var ss = input.Split(' ');
            var groups = ss[1].Split(',').Select(int.Parse).ToList();

            var actual = IsValid(ss[0], groups);

            Assert.That(actual, Is.True);
        }

        public bool IsValid(string record, List<int> groups, bool unknownIsSuccess = false)
        {
            int curGroup = 0;
            int count = 0;

            var trimmedRecord = record.Trim('.') + ".";

            foreach (var spring in trimmedRecord)
            {
                if (spring == '.' && count > 0)
                {
                    if (count != groups[curGroup])
                    {
                        return false;
                    }

                    curGroup++;
                    count = 0;
                }
                else if (spring == '#')
                {
                    count++;
                }
                else if (unknownIsSuccess && spring == '?')
                {
                    return true;
                }
            }

            return true;
        }

        public override string PartOne(List<string> lines)
        {
            var sum = 0L;

            foreach (var line in lines)
            {
                var ss = line.Split(' ');
                var groups = ss[1].Split(',').Select(int.Parse).ToList();
                var known = ss[0].Count(c => c == '#');
                var missing = groups.Sum() - known;

                var unknownPositions = ss[0].AllIndexesOf("?").ToHashSet();

                foreach (var subSet in Enumerations.GetAllSubSets(unknownPositions).Where(set => set.Count == missing))
                {
                    var record = ss[0].Replace('?', '.').ToArray();
                    foreach (var pos in subSet)
                    {
                        record[pos] = '#';
                    }

                    var updatedRecord = new string(record);
                    if (IsValid(updatedRecord, groups))
                    {
                        WriteLine(updatedRecord);
                        sum++;
                    }
                }
            }

            return sum.ToString();
        }

        public override string PartTwo(List<string> lines)
        {
            var sum = 0L;

            foreach (var line in lines)
            {
                var ss = line.Split(' ');
                var groups = ss[1].Split(',').Select(int.Parse).ToList();

                var unfoldedGroups = groups.Concat(groups).Concat(groups).Concat(groups).Concat(groups).ToList();

                var unfoldedRecord = $"{ss[0]}?{ss[0]}?{ss[0]}?{ss[0]}?{ss[0]}";

                var known = unfoldedRecord.Count(c => c == '#');
                var missing = groups.Sum() - known;

                var unknownPositions = unfoldedRecord.AllIndexesOf("?").ToHashSet();

                foreach (var subSet in Enumerations.GetAllSubSets(unknownPositions).Where(set => set.Count == missing))
                {
                    var record = unfoldedRecord.Replace('?', '.').ToArray();
                    foreach (var pos in subSet)
                    {
                        record[pos] = '#';
                    }

                    var updatedRecord = new string(record);
                    if (IsValid(updatedRecord, groups))
                    {
                        WriteLine(updatedRecord);
                        sum++;
                    }
                }
            }

            return sum.ToString();
        }
    }
}
