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
            //input = new int[] { 3, 23, 3, 24, 1002, 24, 10, 24, 1002, 23, -1, 23, 101, 5, 23, 23, 1, 24, 23, 23, 4, 23, 99, 0, 0 };

            var sw = new Stopwatch();
            sw.Start();

            var phases = new int[] { 0, 1, 2, 3, 4 };
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

            for (var ampIndex = 0; ampIndex < 5; ampIndex++)
            {
                var c = new IntCodeComputer(input);
                c.Inputs.Add(phases[ampIndex]);
                c.Inputs.Add(output);
                c.Run();
                output = c.Outputs.First();
            }
            return output;
        }
    }
}
