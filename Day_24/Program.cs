using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

using Core;

namespace Day_24
{
    class Program
    {
        private static Dictionary<int, FiniteGrid2D<char>> _maps = new Dictionary<int, FiniteGrid2D<char>>();

        static void Main()
        {
            var sw = new Stopwatch();
            sw.Start();
            var map = Grid2D.FromFile("../../../input.txt");

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

            for (int i = 0; i < 200; i++)
            {
                StepAllMaps();
            }
            var bugCount = _maps.Sum(m => m.Value.Count(t => t.value == '#'));

            Console.WriteLine($"Part 2: Overall bugs: {bugCount}.");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        private static void StepAllMaps()
        {
            var levels = _maps.Keys.MinMax();
            _maps[levels.min - 1] = new FiniteGrid2D<char>(5, 5, '.');
            _maps[levels.max + 1] = new FiniteGrid2D<char>(5, 5, '.');

            var newmaps = _maps.Select(kvp => (idx: kvp.Key, map: Step2(kvp.Key))).ToList();
            _maps = newmaps.ToDictionary(x => x.idx, x => x.map);
        }

        private static char Step(FiniteGrid2D<char> map, Point p)
        {
            var neighborCount = map.Get4NeighborsOf(p).Count(n => map[n] == '#');
            return StepChar(map[p], neighborCount);
        }

        private static FiniteGrid2D<char> Step2(int mapIndex)
        {
            var newMap = new FiniteGrid2D<char>(_maps[mapIndex]);

            foreach (var (pos, value) in _maps[mapIndex])
            {
                newMap[pos] = StepChar(value, GetNeighborCountOf(mapIndex, pos));
            }
            newMap[2, 2] = '?';
            return newMap;
        }

        private static int GetNeighborCountOf(int mapIdx, Point pos)
        {
            var calculator = new NeighborCalculator(_maps[0].Bounds, new Point(2, 2));
            return calculator.GetNeighborsOf(mapIdx, pos).Where(t => _maps.ContainsKey(t.map)).Count(t => _maps[t.map][t.p] == '#');
        }

        private static char StepChar(char c, int neighborCount)
        {
            if (c == '?')
                return '?';
            if (c == '#')
                return neighborCount == 1 ? '#' : '.';
            return (neighborCount == 1 || neighborCount == 2) ? '#' : '.';
        }

        private static long CalcBiodiversity(FiniteGrid2D<char> map)
        {
            var gridwidth = map.Bounds.Width;
            return map.Sum(t => (long)(t.value == '#' ? Math.Pow(2, t.pos.X + t.pos.Y * gridwidth) : 0));
        }
    }

    class NeighborCalculator
    {
        public Rectangle MapSize { get; }
        public Point RecursePoint { get; }

        public NeighborCalculator(Rectangle mapSize, Point recursePoint)
        {
            MapSize = mapSize;
            RecursePoint = recursePoint;
        }

        public IList<(int map, Point p)> GetNeighborsOf(int mapIdx, Point pos)
        {
            var res = new List<(int map, Point p)>();
            foreach (var dir in Directions.All4)
            {
                var candidate = pos.MoveTo(dir);
                if (candidate == RecursePoint)
                {
                    res.AddRange(MapEdge(dir.Opposite()).Select(p => (mapIdx + 1, p)));
                }
                else if (!MapSize.Contains(candidate))
                {
                    res.Add((mapIdx - 1, RecursePoint.MoveTo(dir)));
                }
                else
                {
                    res.Add((mapIdx, candidate));
                }
            }
            return res;
        }

        private IEnumerable<Point> MapEdge(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    for (int y = 0; y < MapSize.Height; y++)
                        yield return new Point(0, y);
                    break;
                case Direction.Up:
                    for (int x = 0; x < MapSize.Width; x++)
                        yield return new Point(x, 0);
                    break;
                case Direction.Right:
                    for (int y = 0; y < MapSize.Height; y++)
                        yield return new Point(MapSize.Width - 1, y);
                    break;
                case Direction.Down:
                    for (int x = 0; x < MapSize.Width; x++)
                        yield return new Point(x, MapSize.Height - 1);
                    break;
                default:
                    throw null;
            }
        }
    }
}