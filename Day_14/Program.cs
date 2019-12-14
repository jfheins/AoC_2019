using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Core;
using Core.Combinatorics;
using MoreLinq;

namespace Day_14
{
    class Reaction
    {
        public List<(string name, int amount)> Reagents { get; private set; }
        public (string name, int amount) Result { get; private set; }

        public Reaction(IEnumerable<(string name, int amount)> all)
        {
            Reagents = all.ToList();
            Result = Reagents.Last();
            Reagents.RemoveAt(Reagents.Count - 1);
        }
    }

    class Program
    {
        private static Dictionary<string, int> levels;
        private static Dictionary<string, long> stock;
        private static Dictionary<string, Reaction> reactions;

        static void Main()
        {
            var sw = new Stopwatch();
            sw.Start();

            var input = File.ReadAllLines("../../../input.txt");

            reactions = input
                .Select(line => Regex.Matches(line, @"(\d+) ([A-Z]+)") as IList<Match>)
                .Select(FromMatch)
                .ToDictionary(x => x.Result.name);

            CreateLevels();

            var minOrePerFuel = GetNeededOreFor("FUEL", 1);
            Console.WriteLine($"Part 1: We need {minOrePerFuel } ORE");

            var lowerBound = 1000000000000L / minOrePerFuel;

            var upperBound = lowerBound * 2;
            while (CanProduce("FUEL", upperBound))
                upperBound *= 2;

            var stepSize = CeilPo2((upperBound - lowerBound) / 2);
            var estimate = lowerBound;

            while (stepSize > 0)
            {
                if (CanProduce("FUEL", estimate))
                    estimate += stepSize;
                else
                    estimate -= stepSize;

                stepSize /= 2;
            }

            while (!CanProduce("FUEL", estimate))
                estimate--;


            Console.WriteLine($"Part 2: We can produce {estimate} fuel");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        private static bool CanProduce(string product, long amount = 1)
            => GetNeededOreFor(product, amount) <= 1000000000000L;

        private static int CeilPo2(int v)
        {
            v--;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            return v + 1;
        }
        private static long CeilPo2(long v)
        {
            v--;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            v |= v >> 32;
            return v + 1;
        }

        private static long GetNeededOreFor(string product, long amount = 1)
        {
            if (product == "ORE")
                return amount;

            var needed = new Dictionary<string, long>
            {
                { product, amount }
            };

            checked
            {
                while (needed.Any(x => x.Key != "ORE"))
                {
                    var next = needed.MaxBy(x => levels[x.Key]).First();
                    _ = needed.Remove(next.Key);
                    var react = reactions[next.Key];
                    var reactionCount = (long)Math.Ceiling(next.Value / (double)react.Result.amount);
                    foreach (var reagent in react.Reagents)
                    {
                        needed.AddOrModify(reagent.name, 0, x => x + (reagent.amount * reactionCount));
                    }
                }
            }
            return needed["ORE"];
        }

        private static Reaction FromMatch(IEnumerable<Match> matches)
        {
            var groups = matches.SelectMany(m => (m.Groups as IList<Group>).Skip(1)).ToList();
            return new Reaction(groups.Pairwise().Select(t => (t.Item2.Value, int.Parse(t.Item1.Value))));
        }

        private static void CreateLevels()
        {
            static int getLevel(string product)
                => levels.GetOrAdd(product, p => reactions[p].Reagents.Max(x => getLevel(x.name)) + 1);


            levels = new Dictionary<string, int>();
            levels["ORE"] = 0;

            getLevel("FUEL");
        }
    }
}