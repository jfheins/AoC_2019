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

namespace Day_18
{
    class Program
    {
        private static Dictionary<Point, char> _map;
        private static Point initialPos;
        public static readonly Dictionary<char, Point> _mapKeyToPosition = new Dictionary<char, Point>();
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
                        _mapKeyToPosition.Add(input[y][x], new Point(x, y));
                        _map[initialPos] = '.';
                    }
                    if (char.IsLower(input[y][x]))
                        _mapKeyToPosition.Add(input[y][x], new Point(x, y));
                }
            }
            _allKeys = new HashSet<char>(_mapKeyToPosition.Keys);

            // Populate key path cache
            var allKeyPoints = new HashSet<Point>(_mapKeyToPosition.Values);
            foreach (var key in _mapKeyToPosition)
            {
                var search = new BreadthFirstSearch<Point>(EqualityComparer<Point>.Default, PointExpandThroughDoors) { PerformParallelSearch = false };
                var others = search.FindAll(key.Value, p => p != key.Value && allKeyPoints.Contains(p));

                foreach (var other in others)
                {
                    var doorsOnTheWay = string.Concat(other.Steps.Select(p => _map[p]).Where(x => char.IsUpper(x)));
                    var neededKeys = new HashSet<char>(doorsOnTheWay.ToLower());
                    _keyPaths.Add((key.Key, _map[other.Target]), (other.Length, neededKeys));
                }
            }

            var strategySearch = new AStarSearch<string>(new StateComparer(), Expander);

            var path = strategySearch.FindFirst(
                "@",
                node => node.Length == _allKeys.Count,
                EstimateRemainder);

            Console.WriteLine($"Part 1: {path.Cost} steps for {path.Target}");
            Console.WriteLine(PathSteps(path.Target));
            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        private static float EstimateRemainder(string keys)
        {
            if (keys.Length == _allKeys.Count)
                return 0;

            var thisKey = keys[^1];
            var neededKeys = _allKeys.Except(keys).ToList();

            if (neededKeys.Count == 1)
                return _keyPaths[(thisKey, neededKeys[0])].length;

            var minFirst = neededKeys.Select(n => _keyPaths[(thisKey, n)]).Select(p => p.length).Min();
            var minBetween = new Combinations<char>(neededKeys, 2)
                .Select(pair => _keyPaths[(pair[0], pair[1])].length)
                .MinBy(x => x)
                .Take(neededKeys.Count - 1)
                .Sum();

            return minFirst + minBetween;
        }

        private static IEnumerable<Point> PointExpandThroughDoors(Point arg)
        {
            return arg.MoveLURD().Where(x => _map[x] != '#');
        }

        private static int PathSteps(string path)
        {
            return path.PairwiseWithOverlap().Sum(p => _keyPaths[(p.Item1, p.Item2)].length);
        }

        private static Dictionary<string, long> _reachCache = new Dictionary<string, long>();
        private static readonly int[] primes = new int[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97 };

        public static long ReachableKeys(string ownedKeys)
        {
            if (_reachCache.TryGetValue(ownedKeys, out var result))
                return result;

            static long compressor(ICollection<int> x) => (x.Sum() * 100) + x.Count;

            if (ownedKeys == "")
                return compressor(_firstKeySteps.Values);

            var thisKey = ownedKeys[^1];
            var res = new List<int>();
            foreach (var otherKey in _allKeys.Except(ownedKeys))
            {
                res.Add(_keyPaths[(thisKey, otherKey)].length);
            }
            return _reachCache[ownedKeys] = compressor(res);
        }

        private static IEnumerable<(string node, float cost)> Expander(string keys)
        {
            var ownedKeys = new HashSet<char>(keys);
            Func<Point, IEnumerable<Point>> expander = P => ExpandPoint(P, ownedKeys);

            var thisKey = keys[^1];
            var result = new List<(string node, float cost)>();

            foreach (var otherKey in _allKeys.Except(ownedKeys))
            {
                var path = _keyPaths[(thisKey, otherKey)];
                if (path.necessaryKeys.IsSubsetOf(ownedKeys))
                {
                    result.Add((keys + otherKey, path.length));
                }
            }

            if (counter++ % 10000 == 0)
                Console.WriteLine($"Expanding {keys} to {string.Join(", ", result.Select(x => x.node))} missing {_mapKeyToPosition.Count - keys.Length}");

            return result;

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

        class StateComparer : IEqualityComparer<string>
        {
            public bool Equals([AllowNull]string left, [AllowNull] string right)
            {
                // Two states are equal if we can reach the same number of keys and with the same amount of steps in total
                if (left.Length != right.Length)
                {
                    return false;
                }
                return ReachableKeys(left) == ReachableKeys(right);
            }

            public int GetHashCode([DisallowNull]  string obj)
            {
                return HashCode.Combine(obj.Length, SortString(obj));
            }
            static string SortString(string input)
            {
                char[] characters = input.ToArray();
                Array.Sort(characters);
                return new string(characters);
            }
        }
    }
}