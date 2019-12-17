using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Core;
using Core.Combinatorics;

namespace Day_17
{
    class Program
    {
        static void Main()
        {
            var sw = new Stopwatch();
            sw.Start();

            //var input = File.ReadAllText("../../../input.txt").ParseLongs();
            var c = LongCodeComputer.FromFile(@"../../../input.txt");
            c.Store(0, 2);

            _ = c.RunUntilInputRequired();

            Queue(c, "A,A,B,C,B,C,B,A,C,A\n");
            Queue(c, "R,8,L,12,R,8\n");
            Queue(c, "L,10,L,10,R,8\n");
            Queue(c, "L,12,L,12,L,10,R,10\n");
            Queue(c, "Y\n");
            c.Outputs.Clear();

            while (c.RunForOutputs(2200))
            {
                Paint(c.Outputs);
                c.Outputs.Clear();
            }

            Console.WriteLine($"Part 1: ");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        private static void Queue(LongCodeComputer c, string input)
        {
            foreach (var chr in input)
            {
                c.Inputs.Enqueue(chr);
            }
        }

        private static void Paint(IEnumerable<long> laby)
        {
            Console.SetCursorPosition(0, 0);
            var map = laby.Select(l => (char)l).ToArray();
            Console.WriteLine(map);
        }
    }
}