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

            var computer = LongCodeComputer.FromFile(@"../../../input.txt");
            var sum = 0;
            for (int y = 0; y < 50; y++)
            {
                for (int x = 0; x < 50; x++)
                {
                    computer = LongCodeComputer.FromFile(@"../../../input.txt");
                    computer.Inputs.Enqueue(x);
                    computer.Inputs.Enqueue(y);
                    var p = computer.RunForOutputs(1);
                    var output = computer.Outputs.Dequeue();
                    sum += (int)output;
                    Console.Write(output == 1 ? '#' : '.');
                }
                Console.WriteLine();
            }

            Console.WriteLine($"Part 1: {sum}");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }
    }
}
