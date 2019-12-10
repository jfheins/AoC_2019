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
            Console.WriteLine($"Part 1: {max.Key} sees {max.Value} asteroids.");

            var laserLocation = max.Key;
            var targetsWithAngle = asteroids
                .ExceptFor(laserLocation)
                .Select(o => ToPolar(laserLocation, o))
                .ToLookup(x => x.angle, new DoubleComparer());

            var targetsWithRound = targetsWithAngle
                    .SelectMany(group => group
                        .OrderBy(x => x.distance)
                        .Select((x, idx) => (target: x.other, round: idx, x.angle)))
                    .OrderBy(x => x.round)
                    .ThenBy(x => x.angle)
                    .ToList();

            Console.WriteLine($"200th target is at: {targetsWithRound[199].target}");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        static double AngleBetween(Point a, Point b) => Math.Atan2(b.Y - a.Y, b.X - a.X);

        static (Point other, double distance, double angle) ToPolar(Point a, Point b)
        {
            var dx = b.X - a.X;
            var dy = b.Y - a.Y;
            var angle = Math.Atan2(-dy, dx);
            angle = (Math.PI / 2) - angle;
            angle = (angle + (2 * Math.PI)) % (2 * Math.PI);
            return (b, (dx * dx) + (dy * dy), angle);
        }
    }

    class DoubleComparer : IEqualityComparer<double>
    {
        public bool Equals([AllowNull] double x, [AllowNull] double y) => NearlyEqual(x, y, 0.00001);
        public int GetHashCode([DisallowNull] double obj) => obj.GetHashCode();

        public static bool NearlyEqual(double a, double b, double epsilon)
        {
            const double MinNormal = 2.2250738585072014E-308d;
            var absA = Math.Abs(a);
            var absB = Math.Abs(b);
            var diff = Math.Abs(a - b);

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
