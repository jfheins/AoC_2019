using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Core;
using Core.Combinatorics;
using MoreLinq;

namespace Day_15
{
    class Program
    {
        private static Labyrinth laby;

        static void Main()
        {
            var input = File.ReadAllText("../../../input.txt").ParseLongs();

            var sw = new Stopwatch();
            sw.Start();

            laby = new Labyrinth(input);

            var search = new BreadthFirstSearch<Point, Direction>(EqualityComparer<Point>.Default,
                Expander)
            { PerformParallelSearch = false };

            var oxygen = search.FindFirst(laby.Origin, n => n == laby.OxygenPos);
            Console.WriteLine($"Part 1: The droid need {oxygen.Length} steps.");

            var filled = search.FindAll(laby.OxygenPos, n => true);

            var longestPath = filled.Max(path => path.Length);
            Console.WriteLine($"Part 2: The point furthest from the oxygen is {longestPath} steps away.");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");

            Console.WriteLine("Labyrinth map:");
            Console.WriteLine(laby.ToString());

            _ = Console.ReadLine();
        }

        private static IEnumerable<Point> Expander(Point p)
            => p.MoveLURD().Where(next => laby.GetTileAt(next) != Labyrinth.Tile.Wall);
    }
}
