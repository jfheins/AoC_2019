using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Core;
using MoreLinq;

namespace Day_06
{
    class Program
    {
        static void Main()
        {
            var input = File.ReadAllLines("../../../input.txt");

            var sw = new Stopwatch();
            sw.Start();

            var items = input.Select(s => ValueTuple.Create(s.Split(')')[0], s.Split(')')[1]));
            var groups = items.ToLookup(x => x.Item1);

            var known = new Dictionary<string, int>() { { "COM", 0 } };
            var todo = new List<string>() { "COM" };
            while (todo.Count > 0)
            {
                var current = todo.First();
                todo.RemoveAt(0);
                var suborbits = groups[current];
                foreach (var item in suborbits)
                {
                    todo.Add(item.Item2);
                    known.Add(item.Item2, known[current] + 1);
                }
            }

            var sum = known.Sum(lvp => lvp.Value);
            Console.WriteLine(sum);


            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }
    }
}
