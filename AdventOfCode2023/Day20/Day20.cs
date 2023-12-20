using Core;
using System.Text.RegularExpressions;

namespace AdventOfCode2023.Day20
{
    public class Day20 : BaseAocV2
    {
        public abstract class Module
        {
            public string Name { get; }

            public Module(string name)
            {
                Name = name;
            }

            public List<string> Output { get; } = new List<string>();

            public virtual void ConnectOutput(Module module)
            {
                Output.Add(module.Name);
                module.ConnectInput(this);
            }

            public virtual void ConnectInput(Module module)
            {

            }

            public abstract IEnumerable<(bool Signal, string Target)> Act(bool signal, string source);

            protected IEnumerable<(bool Signal, string Target)> Transmit(bool signal)
            {
                return Output.Select(m => (signal, m));
            }
        }

        public class BroadcastModule : Module
        {
            public BroadcastModule(string name) : base(name)
            {
            }

            public override IEnumerable<(bool Signal, string Target)> Act(bool signal, string source)
            {
                return Transmit(signal);
            }
        }

        public class FlipFlopModule : Module
        {
            public FlipFlopModule(string name) : base(name)
            {
            }

            public bool State { get; private set; } = false;

            public override IEnumerable<(bool Signal, string Target)> Act(bool signal, string source)
            {
                if (signal)
                {
                    return Enumerable.Empty<(bool, string)>();
                }

                State = !State;
                return Transmit(State);
            }
        }

        public class ConjunctionModule : Module
        {
            public ConjunctionModule(string name) : base(name)
            {
            }

            public Dictionary<string, bool> Memory { get; } = new();

            public override void ConnectInput(Module module)
            {
                Memory[module.Name] = false;
                base.ConnectInput(module);
            }

            public override IEnumerable<(bool Signal, string Target)> Act(bool signal, string source)
            {
                Memory[source] = signal;

                if (Memory.Values.All(v => v))
                {
                    return Transmit(false);
                }

                return Transmit(true);
            }
        }

        public class OutputModule : Module
        {
            public OutputModule(string name) : base(name)
            {

            }

            public override IEnumerable<(bool Signal, string Target)> Act(bool signal, string source)
            {
                return Enumerable.Empty<(bool, string)>();
            }
        }

        public class ButtonModule : Module
        {
            public ButtonModule(string name) : base(name)
            {

            }

            public override IEnumerable<(bool Signal, string Target)> Act(bool signal, string source)
            {
                throw new NotImplementedException();
            }
        }

        public static Module CreateModule(string name, string type)
        {
            switch (type)
            {
                case "b":
                    return new BroadcastModule(name);
                case "%":
                    return new FlipFlopModule(name);
                case "&":
                    return new ConjunctionModule(name);
                default:
                    return new OutputModule(name);
            }
        }

        public Dictionary<string, Module> CreateModules(IList<string> lines)
        {
            var modules = new Dictionary<string, Module>();

            var outputMapping = lines.Select<string, (string Module, string Output)>(l =>
            {
                var m = Regex.Match(l, @"(?<type>[%&b])(?<name>\w+)\s+->(?<output>.*)");
                var module = CreateModule(m.Groups["name"].Value, m.Groups["type"].Value);
                modules.Add(module.Name, module);
                return (module.Name, m.Groups["output"].Value);
            }).ToDictionary(e => e.Module);

            foreach (var entry in outputMapping)
            {
                var outputs = entry.Value.Output.Split(',').Select(e => e.Trim());
                foreach (var output in outputs.Where(o => !string.IsNullOrEmpty(o)))
                {
                    if (!modules.TryGetValue(output, out var outputModule))
                    {
                        outputModule = new OutputModule(output);
                        modules.Add(outputModule.Name, outputModule);
                    }

                    modules[entry.Key].ConnectOutput(outputModule);
                }
            }

            var buttonModule = new ButtonModule("button");
            buttonModule.ConnectOutput(modules["roadcaster"]);
            modules.Add(buttonModule.Name, buttonModule);

            return modules;
        }

        public override string PartOne(List<string> lines)
        {
            var modules = CreateModules(lines);

            var queue = new Queue<(string Source, bool Signal, string Target)>();

            long highPulses = 0;
            long lowPulses = 0;

            for (int i = 0; i < 1_000; i++)
            {
                queue.Enqueue(("button", false, "roadcaster"));

                while (queue.Any())
                {
                    var (source, signal, target) = queue.Dequeue();

                    if (signal)
                    {
                        highPulses++;
                    }
                    else
                    {
                        lowPulses++;
                    }

                    var signals = modules[target].Act(signal, source);

                    foreach (var newSignal in signals)
                    {
                        if (IsTrace)
                        {
                            WriteLine($"{target} -{(newSignal.Signal ? "high" : "low")}-> {newSignal.Target}");
                        }
                        queue.Enqueue((target, newSignal.Signal, newSignal.Target));
                    }
                }
                WriteLine(ActionLevel.Trace);
            }

            return (highPulses * lowPulses).ToString();
        }

        public override string PartTwo(List<string> lines)
        {
            var modules = CreateModules(lines);

            var queue = new Queue<(string Source, bool Signal, string Target)>();

            long clicks = 0;

            while (true)
            {
                long rxLowSignals = 0;

                queue.Enqueue(("button", false, "roadcaster"));
                clicks++;

                while (queue.Any())
                {
                    var (source, signal, target) = queue.Dequeue();

                    if (target == "rx")
                    {
                        if (signal)
                        {
                            rxLowSignals = 2;
                        }
                        else
                        {
                            rxLowSignals++;
                        }
                    }

                    var signals = modules[target].Act(signal, source);

                    foreach (var newSignal in signals)
                    {
                        if (IsTrace)
                        {
                            WriteLine($"{target} -{(newSignal.Signal ? "high" : "low")}-> {newSignal.Target}");
                        }
                        queue.Enqueue((target, newSignal.Signal, newSignal.Target));
                    }
                }
                WriteLine(ActionLevel.Trace);

                if (rxLowSignals == 1)
                {
                    break;
                }
            }

            return clicks.ToString();
        }
    }
}
