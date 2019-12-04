using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Core;
using MoreLinq;

namespace Day_04
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = new string[] { "134564", "585159" };
            const int runs = 10000;
            var counts = new int[runs];

            _ = NonDecreasingSequences(input[0], input[1]).Where(HasRunsof2orMore).Count();
            _ = NonDecreasingSequences(input[0], input[1]).Where(HasRunsof2).Count();

            var sw = new Stopwatch();
            sw.Start();

            for (var i = 0; i < runs; i++)
            {
                // Part1: HasRunsof2orMore
                // Part2: HasRunsof2
                counts[i] = NonDecreasingSequences(input[0], input[1]).Where(HasRunsof2orMore).Count();
            }

            Console.WriteLine($"Input range: {input[0]} to {input[1]}");
            Console.WriteLine($"Codes that fulfil the conditions: {counts[0]}");
            sw.Stop();

            Debug.Assert(counts.AreAllEqual());

            Console.WriteLine();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms for {runs} runs.");
            Console.WriteLine($"Average time per solution: {sw.ElapsedMilliseconds * 1000 / runs}µs.");
            _ = Console.ReadLine();
        }

        // For Part 1
        static bool HasRunsof2orMore(char[] code)
        {
            var last = 'x';
            foreach (var digit in code)
            {
                if (digit == last)
                    return true;
                last = digit;
            }
            return false;
        }

        // For Part 2
        static bool HasRunsof2(char[] code)
        {
            var last = 'x';
            var run = 0;
            foreach (var digit in code)
            {
                if (digit != last && run == 2)
                    return true;

                run = digit == last ? run + 1 : 1;
                last = digit;
            }
            return run == 2;
        }

        static int toNumber(char[] x)
        {
            static int chartoNum(char c, int multiplier) => (c - '0') * multiplier;

            return chartoNum(x[0], 100000)
                + chartoNum(x[1], 10000)
                + chartoNum(x[2], 1000)
                + chartoNum(x[3], 100)
                + chartoNum(x[4], 10)
                + chartoNum(x[5], 1);
        }

        static IEnumerable<char[]> NonDecreasingSequences(string start, string end)
        {
            var current = start.ToCharArray();
            var endNumber = int.Parse(end);
            while (toNumber(current) <= endNumber)
            {
                yield return current;

                if (current[5] != '9')
                {
                    current[5]++;
                }
                else
                {
                    // Last digit is 9 already. Therfore:
                    // - Find the next digit to the left that is not 9
                    // - increase it and
                    // - set all digits to its right to the same digit
                    // For example 345599 becomes 345666
                    for (var i = 4; i >= 0; i--)
                    {
                        if (current[i] != '9')
                        {
                            current[i]++;
                            for (var j = i + 1; j < 6; j++)
                                current[j] = current[i];
                            break;
                        }
                    }
                }
            }
        }
    }
}
