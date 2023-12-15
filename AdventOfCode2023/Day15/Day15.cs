using Core;
using Helpers;
using System.Text.RegularExpressions;

namespace AdventOfCode2023.Day15
{
    public class Day15 : BaseAocV2
    {
        public int Hash(string text)
        {
            return text.Aggregate(0, (acc, v) => (acc + v) * 17 % 256);
        }

        public override string PartOne(List<string> lines)
        {
            var steps = lines[0].Split(',');

            return steps.Sum(step => Hash(step)).ToString();
        }

        public override string PartTwo(List<string> lines)
        {
            var steps = lines[0].Split(',');

            var boxes = new ValueCreationDictionary<int, List<(string Label, int Focal)>>();

            foreach (var step in steps)
            {
                var match = Regex.Match(step, @"(?<label>\w+)(?<op>[=-])(?<focal>\d?)");
                var label = match.Groups["label"].Value;
                var boxIndex = Hash(label);
                var op = match.Groups["op"].Value;

                var box = boxes[boxIndex];

                switch (op)
                {
                    case "=":
                        {
                            var focal = int.Parse(match.Groups["focal"].Value);
                            var index = box.FindIndex(v => v.Label == label);
                            if (index != -1)
                            {
                                box[index] = (label, focal);
                            }
                            else
                            {
                                box.Add((label, focal));
                            }
                            break;
                        }
                    case "-":
                        {
                            var removeIndex = box.FindIndex(v => v.Label == label);
                            if (removeIndex != -1)
                            {
                                box.RemoveAt(removeIndex);
                            }
                            break;
                        }
                    default:
                        throw new NotImplementedException();
                }
            }

            var focusingPower = boxes.Sum((kvp) => kvp.Value.Select((lens, index) => (kvp.Key + 1) * (index + 1) * lens.Focal).Sum());
            return focusingPower.ToString();
        }
    }
}
