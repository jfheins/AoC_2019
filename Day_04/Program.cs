using System;
using System.Diagnostics;
using System.Linq;
using Core;

namespace Day_04
{
    class Program
    {
        static void Main(string[] args)
        {
            var range = 134564..585159;
            var input = Enumerable.Range(134564, 585159 - 134564 + 1);

            var sw = new Stopwatch();
            sw.Start();

            var codes = input.Count(Check);

            Console.WriteLine($"Part 1: {codes} codes fulfil this.");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        static bool Check(int num)
        {
            var digits = num.ToString().Select(t => int.Parse(t.ToString())).ToArray();
            var pairs = digits.PairwiseWithOverlap().ToList();
            var isNotDecreasing = pairs.All(x => x.Item1 <= x.Item2);

            var runs = digits.Chunks().ToList();
            var hasdouble = runs.Any(c => c.Count() == 2);

            return hasdouble && isNotDecreasing;
        }
    }
}
