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
        private const int phaseCount = 100;

        static void Main()
        {
            var sw = new Stopwatch();
            sw.Start();

            var inputString = File.ReadAllText("../../../input.txt");
            var input = inputString.Select(c => c - '0').ToArray();

            for (var phase = 0; phase < phaseCount; phase++)
            {
                CalculateNextPhase(input);
            }

            var firstDigits = string.Concat(input.Take(8));
            Console.WriteLine($"Part 1: After {phaseCount} phases: {firstDigits}");

            // ============================================================

            input = inputString.Select(c => c - '0').ToArray();
            input = Enumerable.Repeat(input, 10000).SelectMany(x => x).ToArray();

            var messageOffset = int.Parse(inputString.Substring(0, 7));

            for (var phase = 0; phase < phaseCount; phase++)
            {
                CalculateNextPhase(input, messageOffset);
            }

            var messageDigits = string.Concat(input.Skip(messageOffset).Take(8));
            Console.WriteLine($"Part 2: After {phaseCount} phases: {messageDigits}");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        private static void CalculateNextPhase(int[] signals, int start = 0)
        {
            if (start < signals.Length / 2)
            {
                var tmp = (int[])signals.Clone();
                for (var i = start; i < signals.Length / 2; i++)
                {
                    signals[i] = CalculateSignal(tmp, i);
                }
                start = signals.Length / 2;
            }

            // Crucial optimization for high starting indicies (makes the algorithm linear in input length)
            long value = 0;
            for (var i = signals.Length - 1; i >= start; i--)
            {
                value += signals[i];
                signals[i] = Normalize(value);
            }
        }

        private static int CalculateSignal(int[] oldSignals, int index)
        {
            var basePattern = new int[] { 0, 1, 0, -1 };
            var signal = 0;
            for (var j = index; j < oldSignals.Length; j++)
            {
                var patternIdx = (j + 1) / (index + 1) % 4;
                signal += basePattern[patternIdx] * oldSignals[j];
            }

            return Normalize(signal);
        }

        private static int Normalize(long v)
        {
            if (v < 0)
                v = -v;

            return (int)(v % 10);
        }
    }
}