using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
        private static Point initialPos;
        public static readonly Dictionary<char, Point> _keys = new Dictionary<char, Point>();
        private static HashSet<char> _allKeys;
        private static int counter = 0;
        private static readonly Dictionary<(char, char), (int length, HashSet<char> necessaryKeys)> _keyPaths
            = new Dictionary<(char, char), (int length, HashSet<char> necessaryKeys)>();

        private static readonly Dictionary<char, int> _firstKeySteps = new Dictionary<char, int>();

        static void Main()
        {
            var sw = new Stopwatch();
            sw.Start();

            var input = File.ReadAllLines("../../../input.txt");

            _map = new Dictionary<Point, char>();
            initialPos = Point.Empty;

            for (int y = 0; y < input.Length; y++)
            {
                for (int x = 0; x < input[y].Length; x++)
                {
                    _map.Add(new Point(x, y), input[y][x]);
                    if (input[y][x] == '@')
                    {
                        initialPos = new Point(x, y);
                        _map[initialPos] = '.';
                    }
                    if (char.IsLower(input[y][x]))
                        _keys.Add(input[y][x], new Point(x, y));
                }
            }
            _allKeys = new HashSet<char>(_keys.Keys);

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

            var strategySearch = new AStarSearch<(Point pos, string keys)>(new StateComparer(), Expander);

            var path = strategySearch.FindFirst(
                (initialPos, ""),
                node => node.keys.Length == _keys.Count,
                EstimateRemainder);

            Console.WriteLine($"Part 1: {path.Cost} steps for {path.Target.keys}");
            Console.WriteLine(PathSteps(path.Target.keys));
            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        private static float EstimateRemainder((Point pos, string keys) arg)
        {
            if (!_keys.ContainsKey(_map[arg.pos]) || arg.keys.Length == _allKeys.Count) // If we start at a key pos
            {
                return 0;
            }
            else
            {
                var thisKey = _map[arg.pos];
                var ownedKeys = new HashSet<char>(arg.keys);
                var neededKeys = _allKeys.Except(ownedKeys).ToList();

                if (neededKeys.Count == 1)
                    return _keyPaths[(thisKey, neededKeys[0])].length;

                var minFirst = neededKeys.Select(n => _keyPaths[(thisKey, n)]).Select(p => p.length).Min();
                var minBetween = new Combinations<char>(neededKeys, 2).Select(pair => _keyPaths[(pair[0], pair[1])].length).Min();

                return minFirst + neededKeys.Count * minBetween;
            }
        }

        private static IEnumerable<Point> PointExpandThroughDoors(Point arg)
        {
            return arg.MoveLURD().Where(x => _map[x] != '#');
        }

        private static int PathSteps(string path)
        {
            return _firstKeySteps[path[0]] + path.PairwiseWithOverlap().Sum(p => _keyPaths[(p.Item1, p.Item2)].length);
        }

        private static Dictionary<string, long> _reachCache = new Dictionary<string, long>();
        private static readonly int[] primes = new int[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97 };

        public static long ReachableKeys(string ownedKeys)
        {
            //Func<ICollection<int>, long> compressor = x => x.Sum() * 100 + x.Count;
            Func<ICollection<int>, long> compressor = x => (x.OrderByDescending(x => x).Select((x, i) => x * (long)primes[i]).Sum() * 100) + x.Count;

            if (ownedKeys == "")
            {
                return compressor(_firstKeySteps.Values);
            }
            else
            {
                return _reachCache.GetOrAdd(ownedKeys, keys =>
                {
                    var thisKey = keys[^1];
                    var result = new List<int>();
                    var ownedKeys = new HashSet<char>(keys);
                    foreach (var otherKey in _keys.Where(x => !ownedKeys.Contains(x.Key)))
                    {
                        var (length, necessaryKeys) = _keyPaths[(thisKey, otherKey.Key)];
                        if (necessaryKeys.IsSubsetOf(keys))
                        {
                            result.Add(length);
                        }
                    }
                    return compressor(result);
                });

            }
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

                if (counter++ % 10000 == 0)
                    Console.WriteLine($"pos = {_map[arg.pos]} having {arg.keys} explore to {string.Join(", ", result.Select(x => _map[x.node.pos]))} missing {_keys.Count - arg.keys.Length}");

                if (arg.keys.Length == _keys.Count - 1)
                {
                    Console.WriteLine($"path {result[0].node.keys} has all keys with {PathSteps(result[0].node.keys)} steps. (Too long?).");
                }

                return result;
            }
            else
            {
                var keySearch = new BreadthFirstSearch<Point>(EqualityComparer<Point>.Default, expander) { PerformParallelSearch = false };
                var possibleNextKeys = keySearch.FindAll(arg.pos, p => char.IsLower(_map[p]) && !ownedKeys.Contains(_map[p]));

                var nextStates = possibleNextKeys
                    .Select(nextKey => ((nextKey.Target, arg.keys + _map[nextKey.Target]), (float)nextKey.Length))
                    .ToList();

                foreach (var s in possibleNextKeys)
                {
                    _firstKeySteps.Add(_map[s.Target], s.Length);
                }
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

        class StateComparer : IEqualityComparer<(Point pos, string keys)>
        {
            public bool Equals([AllowNull] (Point pos, string keys) left, [AllowNull] (Point pos, string keys) right)
            {
                // Two states are equal if we can reach the same number of keys and with the same amount of steps in total
                if (left.keys.Length != right.keys.Length)
                {
                    return false;
                }
                return ReachableKeys(left.keys) == ReachableKeys(right.keys);
            }

            public int GetHashCode([DisallowNull] (Point pos, string keys) obj)
            {
                return obj.keys.Length;
            }
        }
    }
}