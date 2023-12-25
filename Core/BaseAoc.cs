using NUnit.Framework;
using System.Reflection;
using System.Text.RegularExpressions;

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

    public abstract class BaseAoc<TInput> : IBaseAoc
    {
        protected ActionLevel Level { get; set; } = ActionLevel.Info;
        protected bool IsLevel(ActionLevel level)
        {
            return Level <= level;
        }

        public bool IsTrace => IsLevel(ActionLevel.Trace);
        public bool IsDebug => IsLevel(ActionLevel.Debug);
        public bool IsInfo => IsLevel(ActionLevel.Info);

        protected string FileName { get; private set; }

        public TextWriter Out { get; set; }

        public BaseAoc()
        {
            Out = Console.Out;
        }
        public BaseAoc(string sessionCookie)
        {
            Out = Console.Out;
            SessionCookie = sessionCookie;
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
            var yearFolder = GetType().Namespace.Substring(0, GetType().Namespace.IndexOf('.'));
            var dayFolder = GetType().Name;

            var attr = GetType().GetCustomAttribute<TestFixtureAttribute>();
            if (attr != null)
            {
                if (!string.IsNullOrEmpty(attr.TestName))
                {
                    dayFolder = attr.TestName;
                }
            }


            string filePath = Path.Join(TestContext.CurrentContext.TestDirectory, yearFolder, dayFolder, fileName);

            if (!File.Exists(filePath) && fileName == "input.txt" && !string.IsNullOrEmpty(SessionCookie))
            {
                string year = Regex.Match(yearFolder, @"(\d{4})").Value;
                string day = Regex.Match(dayFolder, @"(\d{1,2})").Value.TrimStart('0');
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"https://adventofcode.com/{year}/day/{day}/input");
                request.Headers.Add("Cookie", $"session={SessionCookie}");
                using HttpClient httpClient = new HttpClient();
                using var res = httpClient.Send(request).Content.ReadAsStream();
                using var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                res.CopyTo(fs);
            }

            return File.ReadAllLines(filePath).ToList();
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

        protected TInput Input { get; private set; }
        public string? SessionCookie { get; set; }

        protected abstract TInput ParseInput(List<string> lines);
        public virtual async Task<object> PartOneAsync(TInput input)
        {
            return await Task.FromResult(PartOne(input));
        }

        public virtual async Task<object> PartTwoAsync(TInput input)
        {
            return await Task.FromResult(PartTwo(input));
        }

        public abstract object PartOne(TInput input);
        public abstract object PartTwo(TInput input);

        [TestCase("input.txt", ActionLevel.Debug)]
        [TestCase("input.txt", ActionLevel.Info)]
        [TestCase("example.txt", ActionLevel.Debug)]
        [TestCase("example.txt", ActionLevel.Trace)]
        public async Task PartOneTest(string fileName, ActionLevel level)
        {
            await RunTest(PartOneAsync, fileName, level);
        }

        [TestCase("input.txt", ActionLevel.Debug)]
        [TestCase("input.txt", ActionLevel.Info)]
        [TestCase("example.txt", ActionLevel.Debug)]
        [TestCase("example.txt", ActionLevel.Trace)]
        public async Task PartTwoTest(string fileName, ActionLevel level)
        {
            await RunTest(PartTwoAsync, fileName, level);
        }

        private async Task RunTest(Func<TInput, Task<object>> partMethod, string fileName, ActionLevel level)
        {
            Level = level;
            FileName = fileName;
            Input = ParseInput(GetInput(fileName));
            var res = await partMethod(Input);
            WriteLine($"Answer: {res}", level: ActionLevel.Info);
        }
    }
}