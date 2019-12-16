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

        private static readonly Dictionary<(int, int), int> signalCache = new Dictionary<(int, int), int>();


        static void Main()
        {
            var input = File.ReadAllText("../../../input.txt").Select(c => c - '0').ToArray();
            //input = "12345678".Select(c => c - '0').ToArray();

            var phaseCount = 100;

            var sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < input.Length; i++)
            {
                signalCache.Add((i, 0), input[i]);
            }

            Console.Write(CalculateSignal2(input, 0, phaseCount));
            Console.Write(CalculateSignal2(input, 1, phaseCount));
            Console.Write(CalculateSignal2(input, 2, phaseCount));
            Console.Write(CalculateSignal2(input, 3, phaseCount));
            Console.Write(CalculateSignal2(input, 4, phaseCount));
            Console.Write(CalculateSignal2(input, 5, phaseCount));
            Console.Write(CalculateSignal2(input, 6, phaseCount));
            Console.Write(CalculateSignal2(input, 7, phaseCount));
            Console.WriteLine();

            Console.WriteLine($"Filling: {input.Length * phaseCount / (double)signalCache.Count}");

            var basePattern = new int[] { 0, 1, 0, -1 };

            for (int phase = 0; phase < phaseCount; phase++)
            {
                var tmp = (int[])input.Clone();
                for (int i = 0; i < input.Length; i++)
                {
                    input[i] = CalculateSignal(tmp, i);
                }
            }

            Console.WriteLine($"Part 1: After 100 phases: {string.Join("", input.Take(8))}");
            Console.WriteLine($"Part 1 Solution =         40921727");


            //input = File.ReadAllText("../../../input.txt").Select(c => c - '0').ToArray();
            //input = Enumerable.Repeat(input, 10).SelectMany(x => x).ToArray();

            //for (int phase = 0; phase < 100; phase++)
            //{
            //    var tmp = (int[])input.Clone();
            //    for (int i = 0; i < input.Length; i++)
            //    {
            //        var signal = 0;
            //        for (int j = 0; j < tmp.Length; j++)
            //        {
            //            var v = ((j + 1) / (i + 1)) % 4;
            //            signal += basePattern[v] * tmp[j];
            //        }

            //        input[i] = Normalize(signal);
            //    }
            //}

            // Console.WriteLine($"Part 2: After 100 phases: {string.Join("", input.Take(8))}");

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

        private static int CalculateSignal(int[] oldSignals, int index)
        {
            var basePattern = new int[] { 0, 1, 0, -1 };
            var signal = 0;
            for (int j = 0; j < oldSignals.Length; j++)
            {
                var v = ((j + 1) / (index + 1)) % 4;
                signal += basePattern[v] * oldSignals[j];
            }

            return Normalize(signal);
        }

        private static int CalculateSignal2(int[] initialSignal, int index, int phase)
        {
            if (signalCache.TryGetValue((index, phase), out var val))
                return val;


            var basePattern = new int[] { 0, 1, 0, -1 };
            var signal = 0;

            for (int j = 0; j < initialSignal.Length; j++)
            {
                var multiplier = basePattern[((j + 1) / (index + 1)) % 4];
                if (multiplier != 0)
                {
                    signal += multiplier * CalculateSignal2(initialSignal, j, phase - 1);
                }
            }

            return signalCache[(index, phase)] = Normalize(signal);
        }

        private static int Normalize(int v)
        {
            if (v < 0)
                v = -v;

            return v % 10;
        }
    }
}