﻿using Core;
using Helpers;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using Spectre.Console;
using System.Text;
using System;

namespace ConsoleRunner
{
    public class Options
    {
        public int Year { get; set; }
        public int Day { get; set; }
        public int Part { get; set; }
    }

    public class MenuChoice
    {
        public MenuChoice(int part, string? fileName, ActionLevel level)
        {
            FileName = fileName;
            Level = level;
            Part = part;
        }

        public string? FileName { get; set; }
        public ActionLevel Level { get; set; }
        public int Part { get; set; }


        static Dictionary<ActionLevel, string> levelToColor = new()
        {
            { ActionLevel.Trace, "orange1" },
            { ActionLevel.Debug, "red" },
            { ActionLevel.Info, "grey" }
        };
        public string GetFormatted()
        {
            return $"{(Part == 1 ? "[blue]PartOne" : "[yellow]PartTwo")}[/] - {(FileName is null ? "<Input>" : Path.GetFileNameWithoutExtension(FileName))} - [{levelToColor[Level]}]{Level}[/]";
        }
    }

    class AnsiConsoleWriter : TextWriter
    {
        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(char value)
        {
            AnsiConsole.Write(value);
        }
    }

    internal class Program
    {
        static List<MenuChoice> GenerateChoices()
        {
            return new()
            {
                new MenuChoice(1, "example.txt", ActionLevel.Trace),
                new MenuChoice(2, "example.txt", ActionLevel.Trace),
                new MenuChoice(1, "input.txt", ActionLevel.Debug),
                new MenuChoice(2, "input.txt", ActionLevel.Debug),
                new MenuChoice(1, "example.txt", ActionLevel.Debug),
                new MenuChoice(2, "example.txt", ActionLevel.Debug),
                new MenuChoice(1, "input.txt", ActionLevel.Info),
                new MenuChoice(2, "input.txt", ActionLevel.Info),
                new MenuChoice(1, null, ActionLevel.Trace),
                new MenuChoice(2, null, ActionLevel.Trace)
            };
        }

        static void Main(string[] args)
        {
            var options = CommandLine.Parser.Default.ParseArguments<Options>(args).Value;

            
            new AdventOfCode2022.ReferenceMe();

            ValueCreationDictionary<int, ValueCreationDictionary<int, List<Type>>> types = new();

            foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => t.GetInterfaces().Contains(typeof(Core.IBaseAoc)) && t.IsAbstract == false))
            {
                var match = Regex.Match(type?.Namespace ?? "", @"(\d{4})");
                if (!match.Success)
                {
                    continue;
                }
                int year = int.Parse(match.Groups[1].Value);
                match = Regex.Match(type.Name, @"Day(\d{1,2})");
                if (!match.Success)
                {
                    continue;
                }
                int day = int.Parse(match.Groups[1].Value);
                types[year][day].Add(type);
            }

            do
            {
                int yearChoice = types.Keys.Max();
                if (types.Count() > 1)
                {
                    yearChoice = AnsiConsole.Prompt(new SelectionPrompt<int>()
                        .Title("Select year")
                        .AddChoices(types.Keys.OrderByDescending(k => k)));
                }

                int dayChoice = types[yearChoice].Keys.Max();
                if (types[yearChoice].Count() > 1)
                {
                    dayChoice = AnsiConsole.Prompt(new SelectionPrompt<int>()
                            .Title($"[bold green]{yearChoice}[/] - Select day")
                            .PageSize(10)
                            .AddChoices(types[yearChoice].Keys.OrderByDescending(k => k)));
                }
                Type typeChoice = types[yearChoice][dayChoice].First();
                if (types[yearChoice][dayChoice].Count > 1)
                {
                    typeChoice = AnsiConsole.Prompt(new SelectionPrompt<Type>()
                        .Title($"[bold green]{yearChoice}-{dayChoice}[/] - Select type")
                        .AddChoices(types[yearChoice][dayChoice])
                        .UseConverter(t => t.Name));
                }

                var choices = GenerateChoices();
                var inputChoice = AnsiConsole.Prompt(new SelectionPrompt<MenuChoice>()
                    .Title($"[bold green]{yearChoice}-{dayChoice}[/] ({typeChoice.Name}) - Select part")
                    .AddChoices(choices)
                    .UseConverter(c => c.GetFormatted()));

                while (string.IsNullOrEmpty(inputChoice.FileName))
                {
                    inputChoice.FileName = AnsiConsole.Ask<string>("Enter path to file:");
                }

                IBaseAoc? instance = Activator.CreateInstance(typeChoice) as IBaseAoc;

                var sw = new Stopwatch();

                if (instance != null)
                {
                    instance.Writing += Instance_Writing;

                    AnsiConsole.MarkupLine($"[bold green]{yearChoice}-{dayChoice}[/] ({instance.GetType().Name})");
                    AnsiConsole.MarkupLine(inputChoice.GetFormatted());

                    sw.Start();
                    try
                    {
                        if (inputChoice.Part == 1)
                        {
                            instance.PartOneTest(inputChoice.FileName, inputChoice.Level);
                        }
                        else
                        {
                            instance.PartTwoTest(inputChoice.FileName, inputChoice.Level);
                        }
                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.WriteException(ex);
                    }

                    sw.Stop();
                    AnsiConsole.MarkupLine($"Run time: {sw.Elapsed}");
                }
                else
                {
                    AnsiConsole.WriteLine("[bold red underline]Failed to instanciate class[/]");
                }


            } while (!AnsiConsole.Confirm("Exit?"));
        }

        private static void Instance_Writing(object? sender, WriteEventArgs e)
        {
            e.Intercepted = true;

            if (e.NewLine)
            {
                if (e.Value == null)
                {
                    AnsiConsole.WriteLine();
                }
                else
                {
                    AnsiConsole.MarkupLine($"[[{DateTime.Now:HH:mm:ss}]] {string.Join("", Enumerable.Repeat("  ", e.Indent))}{e.Value}");
                }
            }
            else
            {
                AnsiConsole.Write(e.Value);
            }
        }
    }
}