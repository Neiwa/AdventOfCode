using Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022.Day10
{
    public class Day10 : BaseAoc
    {
        public override void PartOne(List<string> lines)
        {
            var cycle = 0;
            var X = 1;
            var signal = 0;
            void incCycle()
            {
                cycle++;
                if ((cycle - 20) % 40 == 0)
                {
                    signal += cycle * X;
                }
            }

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                switch (line[0])
                {
                    case 'a':
                        incCycle();
                        incCycle();
                        X += int.Parse(line.Split(" ")[1]);
                        break;
                    case 'n':
                        incCycle();
                        break;
                }
            }
            Console.WriteLine(signal);
        }

        public override void PartTwo(List<string> lines)
        {
            var cycle = 0;
            var X = 1;
            void incCycle()
            {
                cycle++;
                var pixel = (cycle % 40)-1;
                Console.Write(pixel == X || pixel == X-1 || pixel == X+1 ? "#" : ".");
                if (cycle % 40 == 0) Console.WriteLine();
            }

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                switch (line[0])
                {
                    case 'a':
                        incCycle();
                        incCycle();
                        X += int.Parse(line[5..]);
                        break;
                    case 'n':
                        incCycle();
                        break;
                }
            }
        }
    }
}
