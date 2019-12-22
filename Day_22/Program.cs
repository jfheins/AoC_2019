using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Core;
using Core.Combinatorics;

namespace Day_22
{
    class Program
    {
        static void Main()
        {
            var sw = new Stopwatch();
            sw.Start();

            var input = File.ReadAllLines("../../../input.txt");

            var posOfcardOfInterest = 2019;
            var deckSize = 10007;
            foreach (var command in input)
            {
                if (MatchDealWithIncrement(command, out var increment))
                {
                    posOfcardOfInterest = posOfcardOfInterest * increment % deckSize;
                }
                else if (MatchDealNewStack(command))
                {
                    posOfcardOfInterest = deckSize - posOfcardOfInterest - 1;
                }
                else if (MatchCut(command, out var amount))
                {
                    posOfcardOfInterest = Mod(posOfcardOfInterest - amount, deckSize);
                }
            }
            Console.WriteLine($"Part 1: card of interest is now at {posOfcardOfInterest}");

            posOfcardOfInterest = 2019;
            deckSize = 119315717514047L;
            foreach (var command in input.Repeat(101741582076661L))
            {
                if (MatchDealWithIncrement(command, out var increment))
                {
                    posOfcardOfInterest = posOfcardOfInterest * increment % deckSize;
                }
                else if (MatchDealNewStack(command))
                {
                    posOfcardOfInterest = deckSize - posOfcardOfInterest - 1;
                }
                else if (MatchCut(command, out var amount))
                {
                    posOfcardOfInterest = Mod(posOfcardOfInterest - amount, deckSize);
                }
            }

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        private static int Mod(int value, int ringSize)
        {
            while (value < 0)
                value += ringSize;
            return value % ringSize;
        }

        private static bool MatchDealWithIncrement(string s, out int increment)
        {
            var match = Regex.Match(s, @"deal with increment (-?\d+)");
            increment = match.Success ? int.Parse(match.Groups[1].Value) : 0;
            return match.Success;
        }

        private static bool MatchDealNewStack(string s) => s == "deal into new stack";

        private static bool MatchCut(string s, out int amount)
        {
            var match = Regex.Match(s, @"cut (-?\d+)");
            amount = match.Success ? int.Parse(match.Groups[1].Value) : 0;
            return match.Success;
        }
    }
}
