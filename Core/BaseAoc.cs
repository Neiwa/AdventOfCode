using NUnit.Framework;
using System.Reflection;

namespace Core
{
    public abstract class BaseAoc
    {
        public bool Debug { get; set; } = false;

        public List<string> GetInput(string fileName = "input.txt")
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
            PartOne(GetInput(fileName));
        }

        [TestCase("input.txt", true)]
        [TestCase("input.txt", false)]
        [TestCase("example.txt", true)]
        public void PartTwoTest(string fileName, bool debug)
        {
            Debug = debug;
            PartTwo(GetInput(fileName));
        }

        public abstract void PartOne(List<string> lines);

        public abstract void PartTwo(List<string> lines);
    }
}