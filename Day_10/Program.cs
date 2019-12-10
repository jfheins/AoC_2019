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

            var laserLocation = max.Key;
            var targets = asteroids
                .ExceptFor(laserLocation)
                .Select(o => (o, ToPolar(laserLocation, o)))
                .OrderBy(t => t.Item2.angle)
                .ThenBy(t => t.Item2.distance)
                .ToLookup(x => x.Item2.angle, new DoubleComparer());

            var hits = new List<Point>();
            for (int round = 0; round < 1000; round++)
            {
                var thisRound = targets
                    .SelectMany(group => group
                    .OrderBy(x => x.Item2.distance)
                    .Skip(round)
                    .Take(1))
                    .ToList();
                foreach (var target in thisRound)
                {
                    hits.Add(target.o);
                    Console.WriteLine($"{hits.Count} {target.o} @ {target.Item2.angle}");
                }
            }

            Console.WriteLine($"coords: {hits[199]}");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        static double AngleBetween(Point a, Point b)
        {
            return Math.Atan2(b.Y - a.Y, b.X - a.X);
        }

        static (double distance, double angle) ToPolar(Point a, Point b)
        {
            var dx = b.X - a.X;
            var dy = b.Y - a.Y;
            var angle = Math.Atan2(-dy, dx);
            angle = (Math.PI / 2) - angle;
            angle = (angle + 2 * Math.PI) % (2 * Math.PI);
            return (dx * dx + dy * dy, angle);
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
