using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Core;
using MoreLinq;

namespace Day_05
{
    class Program
    {
        static void Main()
        {
            var input = File.ReadAllText("../../../input.txt").ParseInts();

            var sw = new Stopwatch();
            sw.Start();

            var computer = new IntCodeComputer(input);
            computer.Memory[1] = 12;
            computer.Memory[2] = 2;


            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }
    }
}
