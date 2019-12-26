using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Core;
using Core.Combinatorics;

namespace Day_25
{
    class Program
    {
        static void Main()
        {
            var sw = new Stopwatch();
            sw.Start();

            var c = LongCodeComputer.FromFile(@"../../../input.txt");

            var allCommands = new List<string>();

            if (File.Exists(@"../../../commands.txt"))
            {
                allCommands = File.ReadAllLines(@"../../../commands.txt").ToList();
                foreach (var item in allCommands)
                    c.QueueInput(item);

            }
            _ = c.RunUntilInputRequired();
            PrintOutputs(c);

            var items = new string[] { "asterisk", "ornament", "cake", "space heater", "festive hat", "semiconductor", "food ration", "sand" };

            TryAllCombinations(c, items);

            while (c.RunUntilInputRequired())
            {
                PrintOutputs(c);
                var cmd = Console.ReadLine();
                allCommands.Add(cmd);
                c.QueueInput(cmd);
            }

            PrintOutputs(c);
            Console.WriteLine($"Part 1: ");
            sw.Stop();

            Console.WriteLine("Save commands?");
            if (Console.ReadLine() == "y")
            {
                File.WriteAllLines(@"../../../commands.txt", allCommands);
            }

            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        private static void TryAllCombinations(LongCodeComputer c, string[] items)
        {
            for (int itemCount = 1; itemCount <= items.Length; itemCount++)
            {
                foreach (var dropping in new Combinations<string>(items, itemCount))
                {
                    foreach (var item in dropping)
                        TypeIn(c, "drop " + item);

                    TypeIn(c, "inv");
                    _ = c.RunUntilInputRequired();
                    PrintOutputs(c);

                    TypeIn(c, "west");

                    if (!c.RunUntilInputRequired())
                        return;

                    PrintOutputs(c);

                    foreach (var item in dropping)
                        TypeIn(c, "take " + item);
                }
            }
        }

        private static void TypeIn(LongCodeComputer c, string txt)
        {
            Console.WriteLine(txt);
            c.QueueInput(txt);
        }

        private static void PrintOutputs(LongCodeComputer c)
        {
            var msg = string.Concat(c.Outputs.Select(x => (char)x));
            Console.WriteLine(msg);
            c.Outputs.Clear();
        }
    }
}