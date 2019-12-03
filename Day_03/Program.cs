using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var crossings = wires[0].Intersect(wires[1]);

            var closest = crossings.MinBy(DistanceFromOrigin).First();

            Console.WriteLine($"Part1: point {closest} is in both wires and close to the origin.");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        private static int DistanceFromOrigin(Point a)
        {
            return Math.Abs(a.X) + Math.Abs(a.Y);
        }


        private static HashSet<Point> WireToPoints(string text)
        {
            var current = new Point();
            var points = new HashSet<Point>();
            //points.Add(current);

            foreach (var segment in text.Split(","))
            {
                var dir = _mapDirectionToSize[segment[0]];
                var count = int.Parse(segment.Substring(1));
                for (int i = 0; i < count; i++)
                {
                    current += dir;
                    _ = points.Add(current);
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
    }
}
