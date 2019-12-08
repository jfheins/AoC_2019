using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Core;
using Core.Combinatorics;
using MoreLinq;

namespace Day_08
{
    class Program
    {
        static void Main()
        {
            var input = File.ReadAllText("../../../input.txt").Select(chr => chr - '0').ToList();
            const int imgWidth = 25;
            const int imgHeight = 6;


            var sw = new Stopwatch();
            sw.Start();

            var layers = input.Chunks(imgWidth * imgHeight).Select(chunk => chunk.ToArray()).ToList();
            var minlayer = layers.MinBy(l => l.Count(pixel => pixel == 0)).First().ToArray();
            var checksum = minlayer.Count(p => p == 1) * minlayer.Count(p => p == 2);
            Console.WriteLine($"Part 1: Pixel product = { checksum }");

            var combined = layers
                .Aggregate((a, b) => a.Zip(b).Select(tupel => tupel.First == 2 ? tupel.Second : tupel.First).ToArray());

            Console.WriteLine();
            for (int y = 0; y < imgHeight; y++)
            {
                for (int x = 0; x < imgWidth; x++)
                {
                    var chr = (combined[x + y * imgWidth]) switch { 0 => ' ', 1 => '█', 2 => 'o' };
                    Console.Write(chr);
                }
                Console.WriteLine();
            }
            Console.WriteLine();

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }
    }
}
