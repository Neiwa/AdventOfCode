using NUnit.Framework;
using System.Reflection;

namespace Core
{
    public abstract class BaseAoc
    {
        public bool Debug { get; protected set; } = false;

        protected virtual List<string> GetInput(string fileName = "input.txt")
        {
            var name = GetType().Name;

            var attr = GetType().GetCustomAttribute<TestFixtureAttribute>();
            if (attr != null)
            {
                if (!string.IsNullOrEmpty(attr.TestName))
                {
                    name = attr.TestName;
                }
            }

            return File.ReadAllLines(Path.Join(TestContext.CurrentContext.TestDirectory, name, fileName)).ToList();
        }
        protected virtual void Write(string value, int indent = 0)
        {
            if (Debug)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {string.Join("", Enumerable.Repeat("  ", indent))}{value}");
            }
        }

        [TestCase("input.txt", true)]
        [TestCase("input.txt", false)]
        [TestCase("example.txt", true)]
        public void PartOneTest(string fileName, bool debug)
        {
            Debug = debug;
            var res = PartOne(GetInput(fileName));
            Console.WriteLine(res);
        }

        [TestCase("input.txt", true)]
        [TestCase("input.txt", false)]
        [TestCase("example.txt", true)]
        public void PartTwoTest(string fileName, bool debug)
        {
            Debug = debug;
            var res = PartTwo(GetInput(fileName));
            Console.WriteLine(res);
        }

        public abstract string PartOne(List<string> lines);

        public abstract string PartTwo(List<string> lines);
    }
}