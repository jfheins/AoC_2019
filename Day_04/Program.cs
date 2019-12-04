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
            const int runs = 100000;
            var counts = new int[runs];

            var sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < runs; i++)
            {
                var codes = NonDecreasingSequences(input[0].ToCharArray(), input[1].ToCharArray());
                var codesWithDoubles = codes.Where(HasRunsof2orMore);

                counts[i] = codesWithDoubles.Count();
            }

            Console.WriteLine($"Part 1: {counts[0]} codes fulfil this.");
            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds * 1000 / runs}µs on average.");
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

        static IEnumerable<char[]> NonDecreasingSequences(char[] start, char[] end)
        {
            static int toNumber(char[] x) => (x[0] * 100000) + (x[1] * 10000) + (x[2] * 1000) + (x[3] * 100) + (x[4] * 10) + x[5];

            var current = start;
            var endNumber = toNumber(end);
            while (toNumber(current) <= endNumber)
            {
                yield return current;

                if (current[5] != '9')
                {
                    current[5]++;
                }
                else
                {
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
