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
        private static int[] signalCache2;

        private const int phaseCount = 100;

        static void Main()
        {
            var sw = new Stopwatch();
            sw.Start();

            var inputString = File.ReadAllText("../../../input.txt");
            var input = inputString.Select(c => c - '0').ToArray();
            //input = "12345678".Select(c => c - '0').ToArray();

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

            //Console.WriteLine($"Part 1: After {phaseCount} phases: {string.Join("", input)}");
            // ============================================================


            input = inputString.Select(c => c - '0').ToArray();
            input = Enumerable.Repeat(input, 1).SelectMany(x => x).ToArray();

            signalCache2 = new int[input.Length * (phaseCount + 1)];
            for (int i = 0; i < signalCache2.Length; i++)
                signalCache2[i] = int.MaxValue;

            for (int i = 0; i < input.Length; i++)
            {
                signalCache2[i * (phaseCount + 1)] = input[i];
            }

            var messageOffset = int.Parse(inputString.Substring(0, 7));

            Console.Write(CalculateSignal2(input, 0, phaseCount));
            Console.Write(CalculateSignal2(input, 1, phaseCount));
            Console.Write(CalculateSignal2(input, 2, phaseCount));
            Console.Write(CalculateSignal2(input, 3, phaseCount));
            Console.Write(CalculateSignal2(input, 4, phaseCount));
            Console.Write(CalculateSignal2(input, 5, phaseCount));
            Console.Write(CalculateSignal2(input, 6, phaseCount));
            Console.Write(CalculateSignal2(input, 7, phaseCount));
            Console.WriteLine();

            //Console.WriteLine($"Part 2: After 100 phases: {string.Join("", input.Take(8))}");

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
            var cacheIdx = index * (phaseCount + 1) + phase;
            if (signalCache2[cacheIdx] < int.MaxValue)
                return signalCache2[cacheIdx];

            var signal = 0;

            if (index * 4 < initialSignal.Length)
            {
                var basePattern = new int[] { 0, 1, 0, -1 };
                for (int j = index; j < initialSignal.Length; j++)
                {
                    var multiplier = basePattern[((j + 1) / (index + 1)) % 4];
                    if (multiplier != 0)
                    {
                        if (phase == 1)
                            signal += multiplier * signalCache2[j * (phaseCount + 1)];
                        else
                            signal += multiplier * CalculateSignal2(initialSignal, j, phase - 1);
                    }
                    else
                    {
                        j += index;
                    }
                }
            }
            else
            {
                var maxLoop = Math.Min(2 * index + 1, initialSignal.Length);
                for (int j = index; j < maxLoop; j++)
                {
                    if (phase == 1)
                        signal += signalCache2[j * (phaseCount + 1)];
                    else
                        signal += CalculateSignal2(initialSignal, j, phase - 1);
                }

                maxLoop = Math.Min(4 * index + 3, initialSignal.Length);
                for (int j = 3 * index + 2; j < maxLoop; j++)
                {
                    if (phase == 1)
                        signal -= signalCache2[j * (phaseCount + 1)];
                    else
                        signal -= CalculateSignal2(initialSignal, j, phase - 1);
                }
            }


            return signalCache2[cacheIdx] = Normalize(signal);
        }

        private static int Normalize(int v)
        {
            if (v < 0)
                v = -v;

            return v % 10;
        }
    }
}