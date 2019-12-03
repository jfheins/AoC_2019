using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using MoreLinq.Extensions;

namespace Day_03
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("../../../input.txt");

            var sw = new Stopwatch();
            sw.Start();

            var wires = input.Select(WireToPoints).ToList();
            var crossings = wires[0].Keys.Intersect(wires[1].Keys);

            var closest = crossings.MinBy(DistanceFromOrigin).First();
            Console.WriteLine($"Part 1: point {closest} is in both wires and close to the origin.");

            var shortest = crossings.MinBy(x => wires[0][x] + wires[1][x]).First();
            var delay = wires[0][shortest] + wires[1][shortest];
            Console.WriteLine($"Part 2: point {shortest} has signal delay {delay}");


            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        private static int DistanceFromOrigin(Point a)
        {
            return Math.Abs(a.X) + Math.Abs(a.Y);
        }


        private static Dictionary<Point, int> WireToPoints(string text)
        {
            var current = new Point();
            var steps = 0;
            var points = new Dictionary<Point, int>();

            foreach (var segment in text.Split(","))
            {
                var dir = _mapDirectionToSize[segment[0]];
                var count = int.Parse(segment.Substring(1));
                for (int i = 0; i < count; i++)
                {
                    current += dir;
                    steps += 1;
                    if (!points.ContainsKey(current))
                        points.Add(current, steps);
                }
            }
            return points;
        }

        private static readonly Dictionary<char, Size> _mapDirectionToSize = new Dictionary<char, Size>
        {
            {'L', new Size(-1, 0)},
            {'U', new Size(0, -1)},
            {'R', new Size(1, 0)},
            {'D', new Size(0, 1)}
        };

        private class CompareKvpByKeyComparer : IEqualityComparer<KeyValuePair<Point, int>>
        {
            public bool Equals([AllowNull] KeyValuePair<Point, int> x, [AllowNull] KeyValuePair<Point, int> y) => x.Key.Equals(y.Key);
            public int GetHashCode([DisallowNull] KeyValuePair<Point, int> obj) => obj.Key.GetHashCode();
        }
    }
}
