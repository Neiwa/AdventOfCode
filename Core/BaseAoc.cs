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
        public ActionLevel Level { get; set; }
    }

    public enum ActionLevel
    {
        Trace = 0,
        Debug = 10,
        Info = 20
    }

    public abstract class BaseAoc
    {
        public bool Debug
        {
            get => Level <= ActionLevel.Debug;
            protected set => Level = value ? ActionLevel.Debug : ActionLevel.Info;
        }
        protected ActionLevel Level { private get; set; } = ActionLevel.Info;
        public bool IsLevel(ActionLevel level)
        {
            return Level <= level;
        }

        public TextWriter Out { get; set; }

        public BaseAoc()
        {
            Out = Console.Out;
        }

        [SetUp]
        public void SetUp()
        {
            Out = TestContext.Out;
            Level = ActionLevel.Debug;
        }

        public event EventHandler<WriteEventArgs> Writing;

        public delegate WriteEventArgs WriteEventHandler(object sender, WriteEventArgs e);

        protected virtual bool OnWriteLine(ActionLevel level, string? value = null, int indent = 0)
        {
            WriteEventArgs e = new()
            {
                Value = value,
                Indent = indent,
                NewLine = true,
                Level = level
            };
            Writing?.Invoke(this, e);
            //e = Writing?.Invoke(this, e) ?? e;
            return e.Intercepted;
        }

        protected virtual bool OnWrite(ActionLevel level, string value)
        {
            WriteEventArgs e = new()
            {
                Value = value,
                Indent = 0,
                NewLine = false,
                Level = level
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

        protected virtual void WriteLine(string value, ActionLevel level = ActionLevel.Debug, int indent = 0, bool force = false)
        {
            if ((level >= Level || force) && !OnWriteLine(level, value, indent))
            {
                Out.WriteLine($"[{DateTime.Now:HH:mm:ss}] {string.Join("", Enumerable.Repeat("  ", indent))}{value}");
            }
        }
        protected virtual void WriteLine(ActionLevel level = ActionLevel.Debug, bool force = false)
        {
            if ((level >= Level || force) && !OnWriteLine(level))
            {
                Out.WriteLine();
            }
        }
        protected virtual void Write(string value, ActionLevel level = ActionLevel.Debug, bool force = false)
        {
            if ((level >= Level || force) && !OnWrite(level, value))
            {
                Out.Write(value);
            }
        }
        protected virtual void Write(char value, ActionLevel level = ActionLevel.Debug, bool force = false)
        {
            if ((level >= Level || force) && !OnWrite(level, value.ToString()))
            {
                Out.Write(value);
            }
        }

        [TestCase("input.txt", ActionLevel.Debug)]
        [TestCase("input.txt", ActionLevel.Info)]
        [TestCase("example.txt", ActionLevel.Debug)]
        [TestCase("example.txt", ActionLevel.Trace)]
        public void PartOneTest(string fileName, ActionLevel level)
        {
            Level = level;
            var res = PartOne(GetInput(fileName));
            WriteLine(res, level: ActionLevel.Info);
        }

        [TestCase("input.txt", ActionLevel.Debug)]
        [TestCase("input.txt", ActionLevel.Info)]
        [TestCase("example.txt", ActionLevel.Debug)]
        [TestCase("example.txt", ActionLevel.Trace)]
        public void PartTwoTest(string fileName, ActionLevel level)
        {
            Level = level;
            var res = PartTwo(GetInput(fileName));
            WriteLine(res, level: ActionLevel.Info);
        }

        public abstract string PartOne(List<string> lines);

        public abstract string PartTwo(List<string> lines);
    }
}