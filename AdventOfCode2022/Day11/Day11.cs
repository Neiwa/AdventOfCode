using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022.Day11
{
    class Item
    {
        public Item(BigInteger worryLevel)
        {
            WorryLevel= worryLevel;
        }
        public BigInteger WorryLevel { get; set; }
    }
    class Monkey
    {
        public int Inspections { get; set; }
        public Queue<Item> Items { get; set; } = new();
        public Func<BigInteger, BigInteger> Operation { get; set; }
        public Func<BigInteger, int> Test { get; set; }
    }

    public class Day11 : BaseAocV1
    {
        private List<Monkey> GetMonkeys()
        {
            var monkies = new List<Monkey>();
            var items = new List<Item> { new Item(53), new Item(89), new Item(62), new Item(57), new Item(74), new Item(51), new Item(83), new Item(97) };
            Monkey monkey = new Monkey
            {
                Operation = l => l * 3,
                Test = l => l % 13 == 0 ? 1 : 5,
            };
            items.ForEach(i => monkey.Items.Enqueue(i));
            monkies.Add(monkey);

            items = new List<Item> { new Item(85), new Item(94), new Item(97), new Item(92), new Item(56) };
            monkey = new Monkey
            {
                Operation = l => l + 2,
                Test = l => l % 19 == 0 ? 5 : 2,
            };
            items.ForEach(i => monkey.Items.Enqueue(i));
            monkies.Add(monkey);

            items = new List<Item> { new Item(86), new Item(82), new Item(82) };
            monkey = new Monkey
            {
                Operation = l => l + 1,
                Test = l => l % 11 == 0 ? 3 : 4,
            };
            items.ForEach(i => monkey.Items.Enqueue(i));
            monkies.Add(monkey);

            items = new List<Item> { new Item(94), new Item(68) };
            monkey = new Monkey
            {
                Operation = l => l + 5,
                Test = l => l % 17 == 0 ? 7 : 6,
            };
            items.ForEach(i => monkey.Items.Enqueue(i));
            monkies.Add(monkey);

            items = new List<Item> { new Item(83), new Item(62), new Item(74), new Item(58), new Item(96), new Item(68), new Item(85) };
            monkey = new Monkey
            {
                Operation = l => l + 4,
                Test = l => l % 3 == 0 ? 3 : 6,
            };
            items.ForEach(i => monkey.Items.Enqueue(i));
            monkies.Add(monkey);

            items = new List<Item> { new Item(50), new Item(68), new Item(95), new Item(82) };
            monkey = new Monkey
            {
                Operation = l => l + 8,
                Test = l => l % 7 == 0 ? 2 : 4,
            };
            items.ForEach(i => monkey.Items.Enqueue(i));
            monkies.Add(monkey);

            items = new List<Item> { new Item(75) };
            monkey = new Monkey
            {
                Operation = l => l * 7,
                Test = l => l % 5 == 0 ? 7 : 0,
            };
            items.ForEach(i => monkey.Items.Enqueue(i));
            monkies.Add(monkey);

            items = new List<Item> { new Item(92), new Item(52), new Item(85), new Item(89), new Item(68), new Item(82) };
            monkey = new Monkey
            {
                Operation = l => l * l,
                Test = l => l % 2 == 0 ? 0 : 1,
            };
            items.ForEach(i => monkey.Items.Enqueue(i));
            monkies.Add(monkey);

            return monkies;
        }

        private List<Monkey> GetExampleMonkeys()
        {
            var monkies = new List<Monkey>();
            var items = new List<Item> { new Item(79), new Item(98) };
            Monkey monkey = new Monkey
            {
                Operation = l => l * 19,
                Test = l => l % 23 == 0 ? 2 : 3,
            };
            items.ForEach(i => monkey.Items.Enqueue(i));
            monkies.Add(monkey);

            items = new List<Item> { new Item(54), new Item(65), new Item(75), new Item(74) };
            monkey = new Monkey
            {
                Operation = l => l + 6,
                Test = l => l % 19 == 0 ? 2 : 0,
            };
            items.ForEach(i => monkey.Items.Enqueue(i));
            monkies.Add(monkey);

            items = new List<Item> { new Item(79), new Item(60), new Item(97) };
            monkey = new Monkey
            {
                Operation = l => l * l,
                Test = l => l % 13 == 0 ? 1 : 3,
            };
            items.ForEach(i => monkey.Items.Enqueue(i));
            monkies.Add(monkey);

            items = new List<Item> { new Item(74) };
            monkey = new Monkey
            {
                Operation = l => l + 3,
                Test = l => l % 17 == 0 ? 0 : 1,
            };
            items.ForEach(i => monkey.Items.Enqueue(i));
            monkies.Add(monkey);

            return monkies;
        }

        public override void PartOneV1(List<string> lines)
        {
            var monkies = GetMonkeys();

            for (int i = 0; i < 20; i++)
            {
                foreach (var monkey in monkies)
                {
                    while (monkey.Items.Any())
                    {
                        var item = monkey.Items.Dequeue();
                        monkey.Inspections++;
                        item.WorryLevel = monkey.Operation(item.WorryLevel);
                        item.WorryLevel = item.WorryLevel / 3;
                        var newMonkey = monkey.Test(item.WorryLevel);
                        monkies[newMonkey].Items.Enqueue(item);
                    }
                }
            }
            var m = monkies.OrderByDescending(m => m.Inspections).Take(2).ToList();
            Console.Write(m[0].Inspections * m[1].Inspections);
        }

        public override void PartTwoV1(List<string> lines)
        {
            var monkies = GetMonkeys();
            BigInteger mod = monkies.Count == 4 ? (23 * 19 * 13 * 17) : (13*19*11*17*3*7*5*2);

            for (int i = 0; i < 10_000; i++)
            {
                foreach (var monkey in monkies)
                {
                    while (monkey.Items.Any())
                    {
                        var item = monkey.Items.Dequeue();
                        monkey.Inspections++;
                        item.WorryLevel = item.WorryLevel % mod;
                        item.WorryLevel = monkey.Operation(item.WorryLevel);
                        var newMonkey = monkey.Test(item.WorryLevel);
                        monkies[newMonkey].Items.Enqueue(item);
                    }
                }
            }
            var m = monkies.OrderByDescending(m => m.Inspections).Take(2).ToList();
            Console.WriteLine(BigInteger.Multiply(m[0].Inspections, m[1].Inspections));
            Console.Write((long)m[0].Inspections * (long)m[1].Inspections);
        }
    }
}
