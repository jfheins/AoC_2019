using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Core;
using Core.Combinatorics;
using MoreLinq;

namespace Day_07
{
    class Program
    {
        static void Main()
        {
            var input = File.ReadAllText("../../../input.txt").ParseInts();
            // input = new int[] { 3, 26, 1001, 26, -4, 26, 3, 27, 1002, 27, 2, 27, 1, 27, 26, 27, 4, 27, 1001, 28, -1, 28, 1005, 28, 6, 99, 0, 0, 5 };

            var sw = new Stopwatch();
            sw.Start();

            var phases = new int[] { 5, 6, 7, 8, 9 };
            var phaseCombinations = new Permutations<int>(phases);
            var result = new Dictionary<int[], int>();

            foreach (IList<int> phasesetting in phaseCombinations)
            {
                result.Add(phasesetting.ToArray(), TryPhaseSetting(phasesetting, input));
            }

            var maxout = result.MaxBy(kvp => kvp.Value).First();
            Console.WriteLine($"Part 1: Setting {string.Join(",", maxout.Key)} has end signal of { maxout.Value }");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        static int TryPhaseSetting(IList<int> phases, int[] input)
        {
            Debug.Assert(phases.Count == 5);
            var output = 0;

            var computers = phases.Select(p => new IntCodeComputer(input, p)).ToList();

            for (int i = 0; i < 1000; i++)
            {
                for (var ampIndex = 0; ampIndex < 5; ampIndex++)
                {
                    computers[ampIndex].Inputs.Add(output);
                    computers[ampIndex].Run(int.MaxValue, true);
                    if (computers[ampIndex].CurrentInstruction != OpCode.Halt)
                        output = computers[ampIndex].Outputs[i];
                }
                if (computers.Last().CurrentInstruction == OpCode.Halt)
                {
                    return output;
                }
            }
            throw new Exception(); // Program did not halt
        }
    }
}
