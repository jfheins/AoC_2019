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
        private static int[] _input;

        static void Main()
        {
            _input = File.ReadAllText("../../../input.txt").ParseInts();
            // input = new int[] { 3, 26, 1001, 26, -4, 26, 3, 27, 1002, 27, 2, 27, 1, 27, 26, 27, 4, 27, 1001, 28, -1, 28, 1005, 28, 6, 99, 0, 0, 5 };

            var sw = new Stopwatch();
            sw.Start();

            var phases = new int[] { 0, 1, 2, 3, 4 };
            var result = Exec(phases, TryPhaseSetting);
            Console.WriteLine($"Part 1: Setting {string.Join(",", result.Key)} has end signal of { result.Value }");

            phases = new int[] { 5, 6, 7, 8, 9 };
            result = Exec(phases, TryPhaseSettingFeedback);
            Console.WriteLine($"Part 2: Setting {string.Join(",", result.Key)} has end signal of { result.Value }");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        private static KeyValuePair<int[], int> Exec(int[] phases, Func<IList<int>, int> callback)
        {
            var phaseCombinations = new Permutations<int>(phases);
            var result = new Dictionary<int[], int>();

            foreach (IList<int> phasesetting in phaseCombinations)
            {
                result.Add(phasesetting.ToArray(), callback(phasesetting));
            }

            return result.MaxBy(kvp => kvp.Value).First();
        }

        static int TryPhaseSetting(IList<int> phases)
        {
            Debug.Assert(phases.Count == 5);
            var output = 0;

            for (var ampIndex = 0; ampIndex < 5; ampIndex++)
            {
                var c = new IntCodeComputer(_input, phases[ampIndex]);
                output = c.RunWith(output).Value;
            }
            return output;
        }

        static int TryPhaseSettingFeedback(IList<int> phases)
        {
            Debug.Assert(phases.Count == 5);
            var output = 0;

            var computers = phases.Select(p => new IntCodeComputer(_input, p)).ToList();

            for (int i = 0; i < 1000; i++)
            {
                for (var ampIndex = 0; ampIndex < 5; ampIndex++)
                {
                    var newOutput = computers[ampIndex].RunWith(output);
                    if (newOutput.HasValue)
                        output = newOutput.Value;
                    else
                        return output;
                }
            }
            throw new Exception(); // Program did not halt
        }
    }
}
