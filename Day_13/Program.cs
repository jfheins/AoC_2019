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
        static Dictionary<Point, int> tiles;

        static IEnumerable<(int x, int y, int tile)> ToTiles(IEnumerable<long> o) => o.Select(x => (int)x).Triplewise();

        static void Main()
        {
            var input = File.ReadAllText(@"../../../input.txt").ParseLongs();

            var sw = new Stopwatch();
            sw.Start();

            var c = new LongCodeComputer(input);
            c.Store(0, 2);
            _ = c.RunUntilInputRequired();
            tiles = ToTiles(c.Outputs)
                .ToDictionary(t => new Point(t.x, t.y), x => x.tile);

            c.Outputs.Clear();

            var blockCount = tiles.Count(x => x.Value == 2);
            Console.WriteLine($"Part 1: {blockCount}");
            //_ = Console.ReadLine();


            var width = 42;
            var height = 22;
            Console.BufferWidth = Math.Max(width, Console.BufferWidth);
            Console.BufferHeight = Math.Max(height + 10, Console.BufferHeight);

            var allMoves = new List<int>();
            var maxScore = 0;
            if (File.Exists(@"../../../moves.txt"))
            {
                allMoves.AddRange(File.ReadAllText(@"../../../moves.txt").ParseInts());

                foreach (var move in allMoves)
                    c.Inputs.Enqueue(move);

                _ = c.RunUntilInputRequired();
                AddTiles(c.Outputs);
                maxScore = tiles[new Point(-1, 0)];
            }


            do
            {
                AddTiles(c.Outputs);
                PaintGame();
                var key = Console.ReadKey();

                var nextmove = key.KeyChar - '0' - 2;
                if (nextmove > 1 || nextmove < -1)
                    break;
                c.Inputs.Enqueue(nextmove);
                allMoves.Add(nextmove);
            } while (c.RunUntilInputRequired());

            // Last Paint

            AddTiles(c.Outputs);
            PaintGame();

            //sw.Stop();
            //Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            //Console.Clear();
            //Console.WriteLine("Game over");

            //File.WriteAllText(@"../../../moves.txt", string.Join(',', allMoves));
            Console.WriteLine("Moves saved.");

            _ = Console.ReadLine();
        }

        private static Dictionary<int, char> _mapTileIfToChar = new Dictionary<int, char> {
            {0, ' '},{ 1, 'M'},{ 2, '#'},{ 3, '='},{ 4, 'o'} };

        private static void AddTiles(Queue<long> computerOutput)
        {
            foreach (var (x, y, value) in ToTiles(computerOutput))
                tiles[new Point(x, y)] = value;

            computerOutput.Clear();
        }

        private static void PaintGame()
        {
            foreach (var tile in tiles)
            {
                if (tile.Key.X < 0)
                {
                    Console.Title = $"Score: {tile.Value}";
                }
                else
                {
                    Console.SetCursorPosition(tile.Key.X, tile.Key.Y);
                    Console.Write(_mapTileIfToChar[tile.Value]);
                }
            }
            Console.SetCursorPosition(0, 23);
        }
    }
}
