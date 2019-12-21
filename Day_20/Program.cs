﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Core;
using Core.Combinatorics;

namespace Day_20
{
    class Program
    {
        private static FiniteGrid2D<char> _laby;
        private static Dictionary<Point, Point> _portals;

        static void Main()
        {
            var sw = new Stopwatch();
            sw.Start();
            _laby = Grid2D.FromFile("../../../bsp1.txt");

            var labels = ReadLabels().ToList();
            Debug.Assert(labels.Count == 2 * labels.Select(x => x.name).Distinct().Count() - 2);

            _portals = new Dictionary<Point, Point>();
            var openLabels = new Dictionary<string, Point>();
            foreach (var labelGroup in labels.ToLookup(x => x.name))
            {
                if (labelGroup.Count() == 2)
                {
                    var pos = labelGroup.Select(x => x.pos).ToArray();
                    _portals.Add(pos[0], pos[1]);
                    _portals.Add(pos[1], pos[0]);
                }
                else
                    openLabels.Add(labelGroup.Key, labelGroup.First().pos);
            }

            var search = new DijkstraSearch<Point>(EqualityComparer<Point>.Default, Expander);
            var path = search.FindFirst(openLabels["AA"], p => p == openLabels["ZZ"]);

            Console.WriteLine($"Part 1: Path has {path.Length} steps.");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        private static IEnumerable<(Point node, float cost)> Expander(Point arg)
        {
            foreach (var pos in _laby.Get4NeighborsOf(arg).Where(n => _laby[n] == '.'))
            {
                yield return (pos, 1);
            }
            if (_portals.ContainsKey(arg))
            {
                yield return (_portals[arg], 1);
            }
        }

        private static IEnumerable<(Point pos, string name)> ReadLabels()
        {
            var locations = GetPortalPositions(_laby).ToList();

            foreach (var portal in locations)
            {
                foreach (Direction dir in Directions.Reading)
                {
                    if (char.IsUpper(_laby.GetValueOrDefault(portal.MoveTo(dir))))
                    {
                        var name = "" + _laby[portal.MoveTo(dir)] + _laby[portal.MoveTo(dir, 2)];
                        yield return (portal, name);
                        break;
                    }
                }
                foreach (Direction dir in Directions.AntiReading)
                {
                    if (char.IsUpper(_laby.GetValueOrDefault(portal.MoveTo(dir))))
                    {
                        var name = "" + _laby[portal.MoveTo(dir, 2)] + _laby[portal.MoveTo(dir)];
                        yield return (portal, name);
                        break;
                    }
                }
            }
        }

        private static IEnumerable<Point> GetPortalPositions(FiniteGrid2D<char> laby)
        {
            return laby.Where(t => t.value == '.' &&
            t.pos.MoveLURD().Any(n => char.IsUpper(laby.GetValueOrDefault(n))))
                .Select(t => t.pos);
        }
    }
}