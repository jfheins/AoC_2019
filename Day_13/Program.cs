using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Core;
using Core.Combinatorics;
using MoreLinq;

namespace Day_13
{
    class Program
    {
        static void Main()
        {
            var input = File.ReadAllText("../../../input.txt").ParseLongs();

            var sw = new Stopwatch();
            sw.Start();

            var c = new LongCodeComputer(input);

            c.Run();

            var blocks = c.Outputs.Chunks(3).Count(x => x.ElementAt(2) == 2);

            Console.WriteLine($"Part 1: {blocks}");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }
    }
}
