using Core;
using Helpers.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023.Day11
{
    public class Day11 : BaseAoc<FixedIntGrid>
    {
        long GetDistance(FixedIntGrid input, long age)
        {
            var colWeights = Enumerable.Repeat(age, input.Width).ToList();
            var rowWeights = Enumerable.Repeat(age, input.Height).ToList();

            var galaxies = input.Where(c => c.Value == '#').ToList();
            foreach (var galaxy in galaxies)
            {
                colWeights[galaxy.X] = 1;
                rowWeights[galaxy.Y] = 1;
            }

            var res = galaxies.Sum((leftGalaxy) => galaxies
            .Where(rightGalaxy => rightGalaxy.Y > leftGalaxy.Y || (rightGalaxy.Y == leftGalaxy.Y && rightGalaxy.X > leftGalaxy.X))
            .Sum(rightGalaxy =>
                (leftGalaxy.X > rightGalaxy.X ?
                    colWeights.Skip(rightGalaxy.X + 1).Take(leftGalaxy.X - rightGalaxy.X).Sum() :
                    colWeights.Skip(leftGalaxy.X + 1).Take(rightGalaxy.X - leftGalaxy.X).Sum()) +
                rowWeights.Skip(leftGalaxy.Y + 1).Take(rightGalaxy.Y - leftGalaxy.Y).Sum()));

            return res;
        }

        public override string PartOne(FixedIntGrid input)
        {
            return GetDistance(input, 2).ToString();
        }

        public override string PartTwo(FixedIntGrid input)
        {
            return GetDistance(input, 1_000_000).ToString();
        }

        protected override FixedIntGrid ParseInput(List<string> lines)
        {
            return lines.ToFixedIntGrid();
        }
    }
}
