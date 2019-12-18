using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Core;
using Core.Combinatorics;

namespace Day_18
{
    class Program
    {
        private static Dictionary<Point, char> _map;

        private static Dictionary<char, Point> _keys = new Dictionary<char, Point>();
        private static readonly Dictionary<(char, char), (int length, HashSet<char> necessaryKeys)> _keyPaths
            = new Dictionary<(char, char), (int length, HashSet<char> necessaryKeys)>();

        static void Main()
        {
            var sw = new Stopwatch();
            sw.Start();

            var input = File.ReadAllLines("../../../bsp3.txt");

            _map = new Dictionary<Point, char>();
            var pos = Point.Empty;

            for (int y = 0; y < input.Length; y++)
            {
                for (int x = 0; x < input[y].Length; x++)
                {
                    _map.Add(new Point(x, y), input[y][x]);
                    if (input[y][x] == '@')
                    {
                        pos = new Point(x, y);
                        _map[pos] = '.';
                    }
                    if (char.IsLower(input[y][x]))
                        _keys.Add(input[y][x], new Point(x, y));
                }
            }

            // Populate key path cache
            foreach (var key in _keys)
            {
                var search = new BreadthFirstSearch<Point>(EqualityComparer<Point>.Default, PointExpandThroughDoors) { PerformParallelSearch = false };
                var others = search.FindAll(key.Value, p => _keys.ContainsValue(p));
                foreach (var other in others)
                {
                    var doorsOnTheWay = string.Concat(other.Steps.Select(p => _map[p]).Where(x => char.IsUpper(x)));
                    var neededKeys = new HashSet<char>(doorsOnTheWay.ToLower());
                    _keyPaths.Add((key.Key, _map[other.Target]), (other.Length, neededKeys));
                }
            }


            var strategySearch = new DijkstraSearch<(Point pos, string keys)>(EqualityComparer<(Point, string)>.Default, Expander);

            var path = strategySearch.FindFirst((pos, ""), node => node.keys.Length == _keys.Count);

            Console.WriteLine($"Part 1: {path.Cost} steps for {path.Target.keys}");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        private static IEnumerable<Point> PointExpandThroughDoors(Point arg)
        {
            return arg.MoveLURD().Where(x => _map[x] != '#');
        }

        private static IEnumerable<((Point pos, string keys) node, float cost)> Expander((Point pos, string keys) arg)
        {
            var ownedKeys = new HashSet<char>(arg.keys);
            Func<Point, IEnumerable<Point>> expander = P => ExpandPoint(P, ownedKeys);

            if (_keys.ContainsKey(_map[arg.pos])) // If we start at a key pos
            {
                var thisKey = _map[arg.pos];
                var result = new List<((Point pos, string keys) node, float cost)>();

                foreach (var nextKey in _keys.Where(x => !ownedKeys.Contains(x.Key)))
                {
                    var path = _keyPaths[(thisKey, nextKey.Key)];
                    if (path.necessaryKeys.IsSubsetOf(ownedKeys))
                    {
                        result.Add(((nextKey.Value, arg.keys + nextKey.Key), path.length));
                    }
                }

                Console.WriteLine($"pos = {_map[arg.pos]} having {arg.keys} explore to {string.Join(", ", result.Select(x => _map[x.node.pos]))}");

                return result;
            }
            else
            {
                var keySearch = new BreadthFirstSearch<Point>(EqualityComparer<Point>.Default, expander) { PerformParallelSearch = false };
                var possibleNextKeys = keySearch.FindAll(arg.pos, p => char.IsLower(_map[p]) && !ownedKeys.Contains(_map[p]));

                var nextStates = possibleNextKeys
                    .Select(nextKey => ((nextKey.Target, arg.keys + _map[nextKey.Target]), (float)nextKey.Length))
                    .ToList();

                Console.WriteLine($"possible: {string.Join(", ", possibleNextKeys.Select(x => _map[x.Target] + x.Target.ToString()))}");

                return nextStates;
            }
        }

        private static IEnumerable<Point> ExpandPoint(Point p, HashSet<char> availableKeys)
        {
            foreach (var next in p.MoveLURD())
            {
                var tile = _map[next];
                if (tile == '.')
                    yield return next;
                else if (tile == '#')
                    continue;
                else if (char.IsLower(tile)) // Is another key                
                    yield return next;
                else if (availableKeys.Contains(char.ToLower(tile)))
                    yield return next;

            }
        }
    }
}