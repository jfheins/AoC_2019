using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Core;
using Core.Combinatorics;

namespace Day_18
{
    class Program
    {
        static void Main()
        {
            var sw = new Stopwatch();
            sw.Start();

            //var input = File.ReadAllText("../../../input.txt").ParseLongs();
            var computer = LongCodeComputer.FromFile(@"../../../input.txt");


            Console.WriteLine($"Part 1: ");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }
    }
}
