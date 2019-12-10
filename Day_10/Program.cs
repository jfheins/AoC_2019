using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using Core;
using Core.Combinatorics;
using MoreLinq;

namespace Day_10
{
    class Program
    {
        static void Main()
        {
            var input = File.ReadAllLines("../../../input.txt");

            var sw = new Stopwatch();
            sw.Start();

            var asteroids = input.SelectMany(
                (line, y) => line.IndexWhere(chr => chr == '#').Select(x => new Point(x, y)))
                .ToList();

            var visibleAsteroids = new Dictionary<Point, int>();
            foreach (var possibleLocation in asteroids)
            {
                var others = asteroids.ExceptFor(possibleLocation);
                var visible = others.Select(o => AngleBetween(possibleLocation, o)).Distinct(new DoubleComparer());
                visibleAsteroids[possibleLocation] = visible.Count();
            }

            var max = visibleAsteroids.MaxBy(x => x.Value).First();
            Console.WriteLine($"Part 1: {max.Key} sees {max.Value}");


            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        static double AngleBetween(Point a, Point b)
        {
            return Math.Atan2(b.Y - a.Y, b.X - a.X);
        }
    }

    class DoubleComparer : IEqualityComparer<double>
    {
        public bool Equals([AllowNull] double x, [AllowNull] double y) => NearlyEqual(x, y, 0.00001);
        public int GetHashCode([DisallowNull] double obj) => obj.GetHashCode();

        public static bool NearlyEqual(double a, double b, double epsilon)
        {
            const double MinNormal = 2.2250738585072014E-308d;
            double absA = Math.Abs(a);
            double absB = Math.Abs(b);
            double diff = Math.Abs(a - b);

            if (a.Equals(b))
            { // shortcut, handles infinities
                return true;
            }
            else if (a == 0 || b == 0 || absA + absB < MinNormal)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < (epsilon * MinNormal);
            }
            else
            { // use relative error
                return diff / (absA + absB) < epsilon;
            }
        }
    }
}
