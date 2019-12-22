using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Core;
using Core.Combinatorics;
using System.Numerics;

namespace Day_22
{
    class Program
    {
        static void Main()
        {
            var sw = new Stopwatch();
            sw.Start();

            var input = File.ReadAllLines("../../../input.txt");

            var posOfcardOfInterest = 2019L;
            var deckSize1 = 10007;

            SimplifySteps(input, deckSize1, out var factor1, out var offset1);

            var finalPos = Mod(checked(posOfcardOfInterest * factor1 + offset1), deckSize1);
            Console.WriteLine($"Part 1: card of interest is now at {finalPos}");

            var deckSize2 = 119315717514047L;
            var repetition = new BigInteger(101741582076661L);

            SimplifySteps(input, deckSize2, out var factor2, out var offset2);

            var bigFactor = new BigInteger(factor2);
            var bigOffset = new BigInteger(offset2);
            var bigDeckSize = new BigInteger(deckSize2);
            var finalFactor = BigInteger.ModPow(bigFactor, repetition, bigDeckSize);
            var finalOffset = CalcReihe(bigFactor, bigOffset, repetition, bigDeckSize);

            // mx + b = 2020
            // => x = (2020 - b) / m
            var card = Divide(new BigInteger(2020) - finalOffset, finalFactor, bigDeckSize);
            card = Mod(card, bigDeckSize);            
            Console.WriteLine($"Part 2: card @ 2020 is {card}");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        private static BigInteger CalcReihe(BigInteger bigFactor, BigInteger bigOffset, BigInteger repetition, BigInteger bigDeckSize)
        {
            // result = b * (1-m^(reps))/(1-m)
            var numerator = BigInteger.One - BigInteger.ModPow(bigFactor, repetition, bigDeckSize);
            var denominator = BigInteger.One - bigFactor;
            var result = Divide(numerator, denominator, bigDeckSize);
            return Mod(bigOffset * result, bigDeckSize);
        }

        private static BigInteger Divide(BigInteger numerator, BigInteger denominator, BigInteger ringSize)
        {
            // Multiplicative inverse element
            denominator = BigInteger.ModPow(denominator, ringSize - 2, ringSize);
            var result = BigInteger.Multiply(numerator, denominator);
            return Mod(result, ringSize);
        }

        private static BigInteger Mod(BigInteger value, BigInteger ringSize)
        {
            var r = value % ringSize;
            return r < 0 ? r + ringSize : r;
        }

        private static long Mod(long value, long ringSize)
        {
            var r = value % ringSize;
            return r < 0 ? r + ringSize : r;
        }

        private static void SimplifySteps(string[] input, long deckSize, out long factor, out long offset)
        {
            factor = 1L;
            offset = 0L;
            foreach (var command in input)
            {
                checked
                {
                    if (MatchDealWithIncrement(command, out var increment))
                    {
                        factor *= increment;
                        offset *= increment;
                    }
                    else if (MatchDealNewStack(command))
                    {
                        factor *= -1;
                        offset = -offset - 1;
                    }
                    else if (MatchCut(command, out var amount))
                    {
                        offset -= amount;
                    }
                }
                factor = Mod(factor, deckSize);
                offset = Mod(offset, deckSize);
            }
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
