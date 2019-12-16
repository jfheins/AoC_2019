using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Core;
using Core.Combinatorics;
using MoreLinq;

namespace Day_15
{
    class Program
    {
        static void Main()
        {
            var input = File.ReadAllText("../../../input.txt").ParseLongs();

            var sw = new Stopwatch();
            sw.Start();

            var laby = new Dictionary<Point, char>();
            var pos = Point.Empty;
            laby[pos] = 'D';

            var c = new LongCodeComputer(input);
            var lastCommand = Direction.Down;
            c.Inputs.Enqueue(KeyToDroidInput(lastCommand));
            var stepcount = 0;

            var labyrinth = string.Concat(input.Skip(252).Select(x => x < 42 ? '#' : ' '));



            //foreach (var line in labyrinth)
            //{
            //    foreach (var point in line)
            //    {
            //        Console.Write(point ? '#' : ' ');
            //    }
            //}

            while (c.RunUntilInputRequired())
            {
                Console.WriteLine($"x: {c.Load(1034)}, y: {c.Load(1035)}");
                var status = c.Outputs.Dequeue();

                if (status == 0)
                    laby[pos.MoveTo(lastCommand)] = '#';
                else
                {
                    stepcount++;
                    laby[pos] = ' ';
                    pos = pos.MoveTo(lastCommand);
                    laby[pos] = status == 1 ? 'D' : 'X';
                }
                Console.Title = $"{stepcount} steps";

                Paint(laby);

                Console.WriteLine($"1036: {c.Load(1036)}, 1037: {c.Load(1037)}, 1038: {c.Load(1038)}");
                Console.WriteLine($"1041: {c.Load(1041)}, 1042: {c.Load(1042)}, 1043: {c.Load(1043)}");

                lastCommand = Console.ReadKey().ToDirection();
                if ((int)lastCommand == -1)
                    break;

                c.Inputs.Enqueue(KeyToDroidInput(lastCommand));
            }

            Console.WriteLine($"Part 1: ");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        private static int KeyToDroidInput(Direction d)
        {
            return d switch
            {
                Direction.Left => 3,
                Direction.Up => 1,
                Direction.Right => 4,
                Direction.Down => 2,
                _ => -1,
            };
        }

        private static void Paint(Dictionary<Point, char> laby)
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            foreach (var y in laby.Keys.PointsInBoundingRect())
            {
                var line = new char[y.Count];
                var x = 0;
                foreach (var p in y)
                {
                    if (p == Point.Empty)
                        line[x++] = '0';
                    else
                        line[x++] = laby.GetValueOrDefault(p, ' ');
                }
                Console.WriteLine(line);
            }
        }
    }
}
