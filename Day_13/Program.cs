using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Core;
using Core.Combinatorics;

namespace Day_13
{
    class Program
    {
        static int score = 0;
        static Dictionary<Point, int> tiles = new Dictionary<Point, int>();
        static int paddlePosition = 0;
        static int ballPosition = 0;

        static IEnumerable<(int x, int y, int tile)> ToTiles(IEnumerable<long> o) => o.Select(x => (int)x).Triplewise();

        static void Main()
        {
            var input = File.ReadAllText(@"../../../input.txt").ParseLongs();

            var sw = new Stopwatch();
            sw.Start();

            var c = new LongCodeComputer(input);
            c.Store(0, 2);
            _ = c.RunUntilInputRequired();

            AddTiles(c.Outputs);

            var blockCount = tiles.Count(x => x.Value == 2);
            Console.WriteLine($"Part 1: {blockCount}");
            _ = Console.ReadLine();

            var width = 42;
            var height = 22;
            Console.BufferWidth = Math.Max(width, Console.BufferWidth);
            Console.BufferHeight = Math.Max(height + 10, Console.BufferHeight);

            var frameCounter = 0;

            do
            {
                AddTiles(c.Outputs);
                if (frameCounter++ % 10 == 0)
                    PaintGame();

                var relPos = Math.Sign(ballPosition.CompareTo(paddlePosition));
                c.Inputs.Enqueue(relPos);
            } while (c.RunUntilInputRequired());

            // Last Paint

            AddTiles(c.Outputs);
            PaintGame();

            _ = Console.ReadLine();
        }

        private static Dictionary<int, char> _mapTileIfToChar = new Dictionary<int, char> {
            {0, ' '},{ 1, 'M'},{ 2, '#'},{ 3, '='},{ 4, 'o'} };

        private static void AddTiles(Queue<long> computerOutput)
        {
            foreach (var (x, y, value) in ToTiles(computerOutput))
            {
                tiles[new Point(x, y)] = value;
            }
            computerOutput.Clear();

            foreach (var obj in tiles)
            {
                if (obj.Value == 4)
                    ballPosition = obj.Key.X;
                if (obj.Value == 3)
                    paddlePosition = obj.Key.X;
            }
            score = tiles[new Point(-1, 0)];
        }

        private static void PaintGame()
        {
            Console.SetCursorPosition(0, 0);
            for (int y = 0; y < 22; y++)
            {
                var line = new char[43];
                for (int x = 0; x < 43; x++)
                {
                    var p = new Point(x, y);
                    var tile = tiles.GetValueOrDefault(p, 0);
                    line[x] = _mapTileIfToChar[tile];
                }
                Console.WriteLine(line);
            }
            Console.Title = $"Score: {score}";
        }
    }
}
