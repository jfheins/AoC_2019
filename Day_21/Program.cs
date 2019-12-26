using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Core;
using Core.Combinatorics;

namespace Day_21
{
    class Program
    {
        static void Main()
        {
            var sw = new Stopwatch();
            sw.Start();

            var c = LongCodeComputer.FromFile(@"../../../input.txt");

            Console.WindowHeight = Math.Max(Console.WindowHeight, 40);

            c.QueueInput("NOT A J");
            c.QueueInput("NOT B T");
            c.QueueInput("OR T J");
            c.QueueInput("NOT C T");
            c.QueueInput("OR T J");
            c.QueueInput("AND D J");
            c.QueueInput("WALK");

            Console.Clear();
            c.Outputs.Clear();

            c.Run();

            if (c.Outputs.Last() < 256)
            {
                Console.WriteLine($"Bad code!");
                Paint(c.Outputs);
                _ = Console.ReadLine();
                return;
            }

            var shipDamage = c.Outputs.Last();
            Console.WriteLine($"Part 1: damage = {shipDamage}");

            c = LongCodeComputer.FromFile(@"../../../input.txt");

            c.QueueInput("NOT A J");
            c.QueueInput("NOT B T");
            c.QueueInput("OR T J");
            c.QueueInput("NOT C T");
            c.QueueInput("OR T J");
            c.QueueInput("AND D J");
            c.QueueInput("NOT E T");
            c.QueueInput("NOT T T");
            c.QueueInput("OR H T");
            c.QueueInput("AND T J");
            c.QueueInput("RUN");

            c.Outputs.Clear();

            c.Run();

            if (c.Outputs.Last() < 256)
            {
                Console.WriteLine($"Bad code!");
                Paint(c.Outputs);
                _ = Console.ReadLine();
                return;
            }

            shipDamage = c.Outputs.Last();
            Console.WriteLine($"Part 2: damage = {shipDamage}");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        private static void Paint(IEnumerable<long> laby)
        {
            var map = laby.Select(l => (char)l).ToArray();
            Console.WriteLine(map);
        }
    }
}