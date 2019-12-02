using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Day_02
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = new int[] { 1, 0, 0, 3, 1, 1, 2, 3, 1, 3, 4, 3, 1, 5, 0, 3, 2, 1, 9, 19, 1, 13, 19, 23, 2, 23, 9, 27, 1, 6, 27, 31, 2, 10, 31, 35, 1, 6, 35, 39, 2, 9, 39, 43, 1, 5, 43, 47, 2, 47, 13, 51, 2, 51, 10, 55, 1, 55, 5, 59, 1, 59, 9, 63, 1, 63, 9, 67, 2, 6, 67, 71, 1, 5, 71, 75, 1, 75, 6, 79, 1, 6, 79, 83, 1, 83, 9, 87, 2, 87, 10, 91, 2, 91, 10, 95, 1, 95, 5, 99, 1, 99, 13, 103, 2, 103, 9, 107, 1, 6, 107, 111, 1, 111, 5, 115, 1, 115, 2, 119, 1, 5, 119, 0, 99, 2, 0, 14, 0 };

            var sw = new Stopwatch();
            sw.Start();

            var computer = new IntCodeComputer(input);
            computer.Memory[1] = 12;
            computer.Memory[2] = 2;

            computer.Run();

            Console.WriteLine($"Part 1: {computer.Memory[0]} after {computer.StepCount} steps.");

            for (var noun = 0; noun < 100; noun++)
            {
                for (var verb = 0; verb < 100; verb++)
                {
                    computer = new IntCodeComputer(input);
                    computer.Memory[1] = noun;
                    computer.Memory[2] = verb;
                    computer.Run();

                    if (computer.Memory[0] == 19690720)
                    {
                        Console.WriteLine($"Part 2: noun={noun}, verb={verb} :-)");
                    }
                }
            }

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }
    }
}
