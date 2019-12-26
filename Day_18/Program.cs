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
        public static readonly Dictionary<char, Point> _mapKeyToPosition = new Dictionary<char, Point>();
        private static HashSet<char> _allKeys;
        private static int counter = 0;
        private static readonly Dictionary<(char, char), (int length, HashSet<char> necessaryKeys)> _keyPaths
            = new Dictionary<(char, char), (int length, HashSet<char> necessaryKeys)>();

        private static readonly Dictionary<string, long> _reachCache = new Dictionary<string, long>();
        private static int _minStepsBetweenKeys;

        static void Main()
        {
            var sw = new Stopwatch();
            sw.Start();

            var input = File.ReadAllLines(@"../../../input2.txt");

            _map = new Dictionary<Point, char>();
            var robotMarker = '1';
            var robots = new List<char>();

            for (int y = 0; y < input.Length; y++)
            {
                for (int x = 0; x < input[y].Length; x++)
                {
                    var digit = input[y][x];
                    if (digit == '@')
                    {
                        robots.Add(robotMarker);
                        digit = robotMarker++;
                    }
                    _map.Add(new Point(x, y), digit);

                    if (char.IsLower(digit))
                        _mapKeyToPosition.Add(digit, new Point(x, y));
                    if (char.IsDigit(digit))
                        _mapKeyToPosition.Add(digit, new Point(x, y));
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
            _minStepsBetweenKeys = _keyPaths.Min(x => x.Value.length);

            var strategySearch = new AStarSearch<string[]>(new StateComparer2(), Expander);

            var path = strategySearch.FindFirst(
                robots.Select(c => c.ToString()).ToArray(),
                IsFinished,
                EstimateRemainder);

            Console.WriteLine($"Part 2: {path.Cost} steps for {string.Join(", ", path.Target)}");
            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        private static bool IsFinished(string[] node) => node.Sum(q => q.Length) == _allKeys.Count;

        private static float EstimateRemainder(string[] keys) => keys.Sum(EstimateRemainder);

        private static float EstimateRemainder(string keys)
        {
            var thisKey = keys[^1];
            var neededKeys = _allKeys.Except(keys).Where(k => _keyPaths.ContainsKey((thisKey, k))).ToList();

            if (neededKeys.Count == 0)
                return 0;

            if (neededKeys.Count == 1)
                return _keyPaths[(thisKey, neededKeys[0])].length;

            var minFirst = neededKeys.Select(n => _keyPaths[(thisKey, n)]).Select(p => p.length).Min();
            var minBetween = new Combinations<char>(neededKeys, 2)
                .Select(pair => _keyPaths[(pair[0], pair[1])].length)
                .OrderBy(x => x)
                .Take(neededKeys.Count - 1)
                .Sum();

            return minFirst + minBetween;
        }

        private static IEnumerable<Point> PointExpandThroughDoors(Point arg)
        {
            return arg.MoveLURD().Where(x => _map[x] != '#');
        }

        private static IEnumerable<(string[] node, float cost)> Expander(string[] keys)
        {
            if (counter++ % 800 == 0)
                Console.WriteLine($"Expanding {string.Join(", ", keys)}, still missing {_allKeys.Count - keys.Sum(arr => arr.Length)}");

            var keyCopy = (string[])keys.Clone();
            var ownedKeys = new HashSet<char>(keys.SelectMany(x => x));
            for (int i = 0; i < keys.Length; i++)
            {
                foreach (var newState in Expander(keys[i], ownedKeys))
                {
                    keyCopy[i] = newState.node;
                    yield return ((string[])keyCopy.Clone(), newState.cost);
                }
                keyCopy[i] = keys[i];
            }
        }

        private static IEnumerable<(string node, float cost)> Expander(string keys, HashSet<char> ownedKeys)
        {
            Func<Point, IEnumerable<Point>> expander = P => ExpandPoint(P, ownedKeys);

            var thisKey = keys[^1];
            keys = SortString(keys);
            var result = new List<(string node, float cost)>();

            foreach (var otherKey in _allKeys.Except(ownedKeys))
            {
                if (_keyPaths.TryGetValue((thisKey, otherKey), out var path))
                {
                    if (path.necessaryKeys.IsSubsetOf(ownedKeys))
                    {
                        result.Add((keys + otherKey, path.length));
                    }
                }
            }

            return result;

        }
        static string SortString(string input)
        {
            var characters = input.ToCharArray();
            Array.Sort(characters);
            return new string(characters);
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

        class StateComparer2 : IEqualityComparer<string[]>
        {
            public bool Equals([AllowNull] string[] left, [AllowNull] string[] right)
            {
                Debug.Assert(left.Length == right.Length);

                var res = true;
                for (int i = 0; i < left.Length; i++)
                    res = res && (left[i] == right[i]);

                return res;
            }

            public int GetHashCode([DisallowNull] string[] obj) => obj.Length > 2 ? HashCode.Combine(obj[0], obj[1], obj[2]) : obj[0].GetHashCode();
        }
    }
}