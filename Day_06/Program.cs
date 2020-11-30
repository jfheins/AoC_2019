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

            //var items = input.Select(s => s.Split(')'));
            //var orbitedby = items.ToLookup(x => x[0], x => x[1]);
            //var orbits = items.ToDictionary(x => x[1], x => x[0]);

            //var search = new BreadthFirstSearch<string>(
            //    EqualityComparer<string>.Default,
            //    node => orbitedby[node].Concat(orbits.GetOrEmpty(node)))
            //{ PerformParallelSearch = false };

            //var depths = search.FindAll("COM", x => true);
            //var sum = depths.Sum(path => path.Length);
            //Console.WriteLine($"Part 1: The total number of orbits is {sum}.");

            //var path = search.FindFirst("SAN", x => x == "YOU");
            //Console.WriteLine($"Part 2: The route von Santa to you has {path.Length - 2} transfers.");

            var graph = Graph.FromEdges(input, s => s.Split(')').ToTuple2());
            var search = new BreadthFirstSearch<GraphNode<string, string>>(
                graph.NodeComparer,
                node => node.Neighbors)
            { PerformParallelSearch = false };

            var depths = search.FindAll(graph.Nodes["COM"], x => true);
            var sum = depths.Sum(path => path.Length);
            Console.WriteLine($"Part 1: The total number of orbits is {sum}.");

            var path = search.FindFirst(graph.Nodes["SAN"], x => x.Value == "YOU");
            Console.WriteLine($"Part 2: The route von Santa to you has {path.Length - 2} transfers.");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }
    }
}
