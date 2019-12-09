using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Core;
using Core.Combinatorics;
using MoreLinq;

namespace Day_09
{
    class Program
    {
        static void Main()
        {
            var input = File.ReadAllText("../../../input.txt").ParseLongs();

            var sw = new Stopwatch();
            sw.Start();

            var c = new LongCodeComputer(input);
            var result = c.RunWith(1);

            Console.WriteLine($"Part 1: {result}");

            c = new LongCodeComputer(input);
            result = c.RunWith(2);

            Console.WriteLine($"Part 2: {result}");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }
    }
}
