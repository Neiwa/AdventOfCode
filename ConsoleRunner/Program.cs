using System.Diagnostics;

namespace ConsoleRunner
{
    public class Options
    {
        public int Year { get; set; }
        public int Day { get; set; }
        public int Part { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var options = CommandLine.Parser.Default.ParseArguments<Options>(args).Value;


            var p = new AdventOfCode2022.Day17.Day17();

            var sw = Stopwatch.StartNew();

            //p.PartOneTest("input.txt", false);
            p.PartTwoTest("example.txt", true);

            sw.Stop();
            Console.WriteLine($"Run time: {sw.Elapsed}");
        }
    }
}