using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Core;
using Core.Combinatorics;

namespace Day_17
{
    class Program
    {

        static void Main()
        {
            var sw = new Stopwatch();
            sw.Start();

            //var input = File.ReadAllText("../../../input.txt").ParseLongs();
            var c = LongCodeComputer.FromFile(@"../../../input.txt");
            c.Store(0, 2);

            var path = "R8L12R8R8L12R8L10L10R8L12L12L10R10L10L10R8L12L12L10R10L10L10R8R8L12R8L12L12L10R10R8L12R8";

            var segments = new Stack<(char name, string value)>();
            _ = FindPathFunctions(path, segments);

            foreach (var func in segments)
            {
                Console.WriteLine($"Function {func.name} should be {func.value}");
            }
            Console.ReadLine();

            Console.WindowHeight = Math.Max(Console.WindowHeight, 40);

            Queue(c, "A,A,B,C,B,C,B,A,C,A\n");
            Queue(c, "R,8,L,12,R,8\n");
            Queue(c, "L,10,L,10,R,8\n");
            Queue(c, "L,12,L,12,L,10,R,10\n");
            Queue(c, "Y\n");
            c.RunForOutputs(2167);
            Console.Clear();
            c.Outputs.Clear();

            while (c.RunForOutputs(2101))
            {
                Paint(c.Outputs);
                c.Outputs.Clear();
            }
            var collectedDust = c.Outputs.Dequeue();

            Console.WriteLine($"Part 2: {collectedDust} dust collected.");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        private static bool FindPathFunctions(string path, Stack<(char, string)> segments, int depth = 0)
        {
            if (depth == 3)
                return !path.Contains('L') && !path.Contains('R');

            var funcName = (char)('A' + depth);

            foreach (var prefix in GetPossiblePrefixSegments(path))
            {
                segments.Push((funcName, prefix));
                var remainder = path.Replace(prefix, funcName + " ");
                if (FindPathFunctions(remainder, segments, depth + 1))
                    return true;
                _ = segments.Pop();
            }
            return false;
        }

        // L10L10R8L12L12L10R10
        private static IEnumerable<string> GetPossiblePrefixSegments(string path)
        {

            var nextSegment = Regex.Match(path, @"\b[RL0-9]+");
            var tokens = Regex.Matches(nextSegment.Value, @"[RL]|\d{1,2}");

            var tokenLengths = tokens.Select(g => g.Length)
                .CumulativeSum()
                .Where(l => l <= 11).ToArray();

            for (int i = tokenLengths.Length - 1; i >= 0; i--)
            {
                yield return path.Substring(nextSegment.Index, tokenLengths[i]);
            }
        }



        private static void Queue(LongCodeComputer c, string input)
        {
            foreach (var chr in input)
            {
                c.Inputs.Enqueue(chr);
            }
        }

        private static void Paint(IEnumerable<long> laby)
        {
            Console.SetCursorPosition(0, 0);
            var map = laby.Select(l => (char)l).ToArray();
            Console.WriteLine(map);
        }
    }
}