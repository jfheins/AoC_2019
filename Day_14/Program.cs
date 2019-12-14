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
        static void Main()
        {
            var input = File.ReadAllLines("../../../input.txt");

            var sw = new Stopwatch();
            sw.Start();

            Func<Match, IEnumerable<string>> MatchToGroups = m => (m.Groups as IList<Group>).Skip(1).Select(g => g.Value);

            var reactions = input.Select(line => Regex.Matches(line, @"(\d+) ([A-Z]+)") as IList<Match>)
            .Select(FromMatch).ToDictionary(x => x.Result.name);

            var allProducts = reactions.Select(r => r.Value.Result.name).ToList();

            var needed = new Dictionary<string, int>();
            var fuelReaction = reactions["FUEL"];
            foreach (var item in fuelReaction.Reagents)
            {
                needed.Add(item.name, item.amount);
            }

            var levels = new Dictionary<string, int>();
            levels["ORE"] = 0;
            foreach (var item in allProducts)
            {
                levels.Add(item, getLevel(item, reactions));
            }

            while (needed.Count > 1)
            {
                var next = needed.MaxBy(x => levels[x.Key]).First();
                needed.Remove(next.Key);
                var react = reactions[next.Key];
                var multi = (int)Math.Ceiling(next.Value / (double)react.Result.amount);
                foreach (var reagent in react.Reagents)
                {
                    if (needed.ContainsKey(reagent.name))
                        needed[reagent.name] += reagent.amount * multi;
                    else
                        needed.Add(reagent.name, reagent.amount * multi);
                }
            }

            Console.WriteLine($"Part 1: We need {needed["ORE"]} ORE");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        private static Reaction FromMatch(IEnumerable<Match> matches)
        {
            var groups = matches.SelectMany(m => (m.Groups as IList<Group>).Skip(1)).ToList();
            return new Reaction(groups.Pairwise().Select(t => (t.Item2.Value, int.Parse(t.Item1.Value))));
        }
        private static int getLevel(string product, Dictionary<string, Reaction> reactions)
        {
            if (product == "ORE")
            {
                return 0;
            }
            var r = reactions[product];
            if (r.Reagents.All(x => x.name == "ORE"))
                return 1;
            else
                return r.Reagents.Max(x => getLevel(x.name, reactions)) + 1;
        }
    }
}