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
        private static Dictionary<int, FiniteGrid2D<char>> _maps = new Dictionary<int, FiniteGrid2D<char>>();

        static void Main()
        {
            var sw = new Stopwatch();
            sw.Start();
            var map = Grid2D.FromFile("../../../bsp.txt");

            var seenRatings = new HashSet<long>();
            var currentDiversity = CalcBiodiversity(map);
            while (!seenRatings.Contains(currentDiversity))
            {
                _ = seenRatings.Add(currentDiversity);
                map = new FiniteGrid2D<char>(map.Bounds.Size, p => Step(map, p));
                currentDiversity = CalcBiodiversity(map);
                Console.WriteLine(map.ToString());
            }

            Console.WriteLine($"Part 1: Biodiversity rating: {currentDiversity}.");

            map = Grid2D.FromFile("../../../input.txt");
            _maps.Add(0, map);

            for (int i = 0; i < 10; i++)
            {
                StepAllMaps(_maps);
            }

            Console.WriteLine($"Part 2: Path has 0 steps.");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        private static void StepAllMaps(Dictionary<int, FiniteGrid2D<char>> maps)
        {
            var levels = maps.Keys.MinMax();
            _ = maps.GetOrAdd(levels.min - 1, l => new FiniteGrid2D<char>(5, 5, '.'));
           _ =  maps.GetOrAdd(levels.max + 1l, l => new FiniteGrid2D<char>(5, 5, '.'));
            for (int lvl = levels.min - 1; lvl <= levels.max + 1; lvl++)
            {
                
            }
        }

        private static char Step(FiniteGrid2D<char> map, Point p)
        {
            var neighborCount = map.Get4NeighborsOf(p).Count(n => map[n] == '#');
            if (map[p] == '#')
            {
                return neighborCount == 1 ? '#' : '.';
            }
            return (neighborCount == 1 || neighborCount == 2) ? '#' : '.';
        }

        private static char Step2(FiniteGrid2D<char> map, Point p)
        {
            var neighborCount = map.Get4NeighborsOf(p).Count(n => map[n] == '#');
            if (map[p] == '#')
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