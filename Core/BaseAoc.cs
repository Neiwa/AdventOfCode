using NUnit.Framework;
using System;
using System.Reflection;

namespace Core
{
    public class WriteEventArgs : EventArgs
    {
        public bool Intercepted { get; set; } = false;
        public bool NewLine { get; set; }
        public string? Value { get; set; } = null;
        public int Indent { get; set; } = 0;
    }

    public abstract class BaseAoc
    {
        public bool Debug { get; protected set; } = false;

        public TextWriter Out { get; set; }

        public BaseAoc()
        {
            Out = Console.Out;
        }

        [SetUp]
        public void SetUp()
        {
            Out = TestContext.Out;
        }

        public event EventHandler<WriteEventArgs> Writing;

        public delegate WriteEventArgs WriteEventHandler(object sender, WriteEventArgs e);

        protected virtual bool OnWriteLine(string? value = null, int indent = 0)
        {
            WriteEventArgs e = new()
            {
                Value = value,
                Indent = indent,
                NewLine = true
            };
            Writing?.Invoke(this, e);
            //e = Writing?.Invoke(this, e) ?? e;
            return e.Intercepted;
        }

        protected virtual bool OnWrite(string value)
        {
            WriteEventArgs e = new()
            {
                Value = value,
                Indent = 0,
                NewLine = false
            };
            Writing?.Invoke(this, e);
            return e.Intercepted;
        }

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

        protected virtual void WriteLine(string value, int indent = 0, bool force = false)
        {
            if ((Debug || force) && !OnWriteLine(value, indent))
            {
                Out.WriteLine($"[{DateTime.Now:HH:mm:ss}] {string.Join("", Enumerable.Repeat("  ", indent))}{value}");
            }
        }
        protected virtual void WriteLine(bool force = false)
        {
            if ((Debug || force) && !OnWriteLine())
            {
                Out.WriteLine();
            }
        }
        protected virtual void Write(string value, bool force = false)
        {
            if ((Debug || force) && !OnWrite(value))
            {
                Out.Write(value);
            }
        }
        protected virtual void Write(char value, bool force = false)
        {
            if ((Debug || force) && !OnWrite(value.ToString()))
            {
                Out.Write(value);
            }
        }

        [TestCase("input.txt", true)]
        [TestCase("input.txt", false)]
        [TestCase("example.txt", true)]
        public void PartOneTest(string fileName, bool debug)
        {
            Debug = debug;
            var res = PartOne(GetInput(fileName));
            WriteLine(res, force: true);
        }

        [TestCase("input.txt", true)]
        [TestCase("input.txt", false)]
        [TestCase("example.txt", true)]
        public void PartTwoTest(string fileName, bool debug)
        {
            Debug = debug;
            var res = PartTwo(GetInput(fileName));
            WriteLine(res, force: true);
        }

        public abstract string PartOne(List<string> lines);

        public abstract string PartTwo(List<string> lines);
    }
}