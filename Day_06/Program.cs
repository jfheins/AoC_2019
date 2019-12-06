using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Core;

namespace Day_06
{
    class Program
    {
        static void Main()
        {
            var input = File.ReadAllLines("../../../input.txt");

            var sw = new Stopwatch();
            sw.Start();

            var items = input.Select(s => s.Split(')'));
            var orbitedby = items.ToLookup(x => x[0], x => x[1]);
            var orbits = items.ToDictionary(x => x[1], x => x[0]);

            var searcher = new BreadthFirstSearch<string, int>(EqualityComparer<string>.Default,
                node =>
                {
                    if (orbits.TryGetValue(node, out var parent))
                        return orbitedby[node].Append(parent);
                    else
                        return orbitedby[node];
                });

            var path = searcher.FindFirst("SAN", x => x == "YOU");
            Console.WriteLine($"Part 2: The route von Santa to you has {path.Length - 2} transfers.");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }
    }
}
