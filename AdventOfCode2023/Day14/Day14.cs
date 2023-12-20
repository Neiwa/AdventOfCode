using Core;
using Helpers;
using Helpers.Grid;

namespace AdventOfCode2023.Day14
{
    public class Day14 : BaseAoc<FixedIntGrid>
    {
        public void Draw(FixedIntGrid<char> grid)
        {
            if (!IsTrace) return;

            for (int y = 0; y < grid.Height; y++)
            {
                for (global::System.Int32 x = 0; x < grid.Width; x++)
                {
                    Write(grid.At(x, y).Value);
                }
                WriteLine();
            }
        }

        public override string PartOne(FixedIntGrid input)
        {
            var rocks = input.Where(c => c.Value == 'O');

            foreach (var rock in rocks)
            {
                var nextPos = rock - new IntPoint(0, 1);
                var newPos = rock;
                while (nextPos.IsValid() && nextPos.Value == '.')
                {
                    newPos = nextPos;
                    nextPos -= new IntPoint(0, 1);
                }
                if (newPos != rock)
                {
                    newPos.Value = 'O';
                    rock.Value = '.';
                }
            }

            Draw(input);

            return input.Where(c => c.Value == 'O').Sum(c => input.Height - c.Y).ToString();
        }

        public override string PartTwo(FixedIntGrid input)
        {
            var cycle = new List<IntPoint>
            {
                new IntPoint(0, -1),
                new IntPoint(-1, 0),
                new IntPoint(0, 1),
                new IntPoint(1, 0)
            };

            var historyStack = new List<FixedIntGrid<char>>();

            for (var i = 0; i < 1_000_000_000; i++)
            {
                foreach (var shift in cycle)
                {
                    var rocks = input.Iterate(new IntPoint(0, 0) - shift).Where(c => c.Value == 'O');

                    foreach (var rock in rocks)
                    {
                        var nextPos = rock + shift;
                        var newPos = rock;
                        while (nextPos.IsValid() && nextPos.Value == '.')
                        {
                            newPos = nextPos;
                            nextPos += shift;
                        }
                        if (newPos != rock)
                        {
                            newPos.Value = 'O';
                            rock.Value = '.';
                        }
                    }
                }

                var historyIndex = historyStack.FindIndex(g => g.All(c => c.Value == input.At(c).Value));
                if (historyIndex == -1)
                {
                    historyStack.Insert(0, input.Clone());
                }
                else
                {
                    var cycleOfRepeat = i + 1;
                    var remainingCycles = 1_000_000_000 - cycleOfRepeat;
                    var repeatLength = historyIndex + 1;
                    WriteLine($"Repeat found at cycleOfRepeat={cycleOfRepeat} and history index = {historyIndex}");
                    WriteLine($"Remaining cycles ={remainingCycles}, repeatLength = {repeatLength}");

                    var index = historyIndex - (remainingCycles % repeatLength);

                    return historyStack[index].Where(c => c.Value == 'O').Sum(c => input.Height - c.Y).ToString();
                }
            }

            return input.Where(c => c.Value == 'O').Sum(c => input.Height - c.Y).ToString();
        }

        protected override FixedIntGrid ParseInput(List<string> lines)
        {
            return lines.ToFixedIntGrid();
        }
    }
}
