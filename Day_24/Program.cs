using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Core;
using Core.Combinatorics;

namespace Day_24
{
    class Program
    {
        private static FiniteGrid2D<char> _map;

        static void Main()
        {
            var sw = new Stopwatch();
            sw.Start();
            _map = Grid2D.FromFile("../../../input.txt");

            var seenRatings = new HashSet<long>();
            var currentDiversity = CalcBiodiversity(_map);
            while (!seenRatings.Contains(currentDiversity))
            {
                _ = seenRatings.Add(currentDiversity);
                _map = new FiniteGrid2D<char>(_map.Bounds.Size, Step);
                currentDiversity = CalcBiodiversity(_map);
                Console.WriteLine(_map.ToString());
            }

            Console.WriteLine($"Part 1: Biodiversity rating: {currentDiversity}.");



            Console.WriteLine($"Part 2: Path has 0 steps.");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        private static char Step(Point p)
        {
            var neighborCount = _map.Get4NeighborsOf(p).Count(n => _map[n] == '#');
            if (_map[p] == '#')
            {
                return neighborCount == 1 ? '#' : '.';
            }
            return (neighborCount == 1 || neighborCount == 2) ? '#' : '.';
        }

        private static long CalcBiodiversity(FiniteGrid2D<char> map)
        {
            var gridwidth = map.Bounds.Width;
            return map.Sum(t => (long)(t.value == '#' ? Math.Pow(2, t.pos.X + t.pos.Y * gridwidth) : 0));
        }
    }
}