using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Core;
using Core.Combinatorics;

namespace Day_16
{
    class Program
    {
        static void Main()
        {
            var input = File.ReadAllText("../../../input.txt").Select(c => c - '0').ToArray();
            //input = "19617804207202209144916044189917".Select(c => c - '0').ToArray();

            var sw = new Stopwatch();
            sw.Start();

            var basePattern = new int[] { 0, 1, 0, -1 };

            for (int phase = 0; phase < 100; phase++)
            {
                var tmp = (int[])input.Clone();
                for (int i = 0; i < input.Length; i++)
                {
                    var signal = 0;
                    for (int j = 0; j < tmp.Length; j++)
                    {
                        var v = ((j + 1) / (i + 1)) % 4;
                        signal += basePattern[v] * tmp[j];
                    }

                    input[i] = Normalize(signal);
                }
            }

            Console.WriteLine($"Part 1: After 100 phases: {string.Join("", input.Take(8))}");
            Console.WriteLine($"Part 1 Solution =         40921727");

            input = File.ReadAllText("../../../input.txt").Select(c => c - '0').ToArray();
            input = Enumerable.Repeat(input, 10).SelectMany(x => x).ToArray();

            for (int phase = 0; phase < 100; phase++)
            {
                var tmp = (int[])input.Clone();
                for (int i = 0; i < input.Length; i++)
                {
                    var signal = 0;
                    for (int j = 0; j < tmp.Length; j++)
                    {
                        var v = ((j + 1) / (i + 1)) % 4;
                        signal += basePattern[v] * tmp[j];
                    }

                    input[i] = Normalize(signal);
                }
            }

            Console.WriteLine($"Part 2: After 100 phases: {string.Join("", input.Take(8))}");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        private static IEnumerable<int> ExpandPattern(int round)
        {
            var basePattern = new int[] { 0, 1, 0, -1 };
            while (true)
            {
                foreach (var item in basePattern)
                {
                    for (int i = 0; i <= round; i++)
                    {
                        yield return item;
                    }
                }
            }
        }

        private static int Normalize(int v)
        {
            if (v < 0)
                v = -v;

            return v % 10;
        }
    }
}