using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Threading.Tasks;
using Core;
using Core.Combinatorics;
using MoreLinq;

namespace Day_12
{

    class Program
    {
        static void Main()
        {
            var input = File.ReadAllLines("../../../input.txt");
            var coords = input.Select(s => Point3.FromArray(s.ParseInts(3))).ToList();

            var sw = new Stopwatch();
            sw.Start();

            var problems = new Problem1D[3];
            problems[0] = new Problem1D(Vector128.Create(coords[0].X, coords[1].X, coords[2].X, coords[3].X));
            problems[1] = new Problem1D(Vector128.Create(coords[0].Y, coords[1].Y, coords[2].Y, coords[3].Y));
            problems[2] = new Problem1D(Vector128.Create(coords[0].Z, coords[1].Z, coords[2].Z, coords[3].Z));

            foreach (var p in problems)
            {
                p.Init();
            }

            var periods = problems.Select(p => p.Solve()).ToList();

            //Console.WriteLine($"Part 1: {CalculateEnergy(bodies)}");

            Console.WriteLine($"Part 2: Period X = {periods[0]}");
            Console.WriteLine($"Part 2: Period Y = {periods[1]}");
            Console.WriteLine($"Part 2: Period Z = {periods[2]}");

            var bodiesGkv = periods.Aggregate(1UL, (gkv, b) => determineLCM((ulong)b, gkv));

            Console.WriteLine($"Part 2: Period = {bodiesGkv}");
            //Console.WriteLine($"Should: Period = 4686774924");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        private static ulong determineLCM(ulong a, ulong b)
        {
            ulong num1, num2;
            if (a > b)
            {
                num1 = a;
                num2 = b;
            }
            else
            {
                num1 = b;
                num2 = a;
            }

            for (ulong i = 1; i < num2; i++)
            {
                if ((num1 * i) % num2 == 0)
                {
                    return i * num1;
                }
            }
            return num1 * num2;
        }
    }
}