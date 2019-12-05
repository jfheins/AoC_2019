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
            computer.Inputs.Add(1);
            computer.Run();
            var code = computer.Outputs.Last();

            Console.WriteLine($"Part 1: output is {code} after {computer.StepCount} steps.");

            computer = new IntCodeComputer(input);
            computer.Inputs.Add(5);

            computer.Run();

            code = computer.Outputs.Last();
            Console.WriteLine($"Part 2: output is {code} after {computer.StepCount} steps.");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }
    }
}
