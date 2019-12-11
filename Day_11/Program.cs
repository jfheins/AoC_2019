using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Core;
using Core.Combinatorics;
using MoreLinq;

namespace Day_11
{
    class Program
    {
        private static readonly Dictionary<Direction, Size> _mapDirectionToSize = new Dictionary<Direction, Size>
        {
            {Direction.Left, new Size(-1, 0)},
            {Direction.Up, new Size(0, -1)},
            {Direction.Right, new Size(1, 0)},
            {Direction.Down, new Size(0, 1)}
        };

        public enum Direction
        {
            Left,
            Up,
            Right,
            Down
        }

        static void Main()
        {
            var input = File.ReadAllText("../../../input.txt").ParseLongs();

            var sw = new Stopwatch();
            sw.Start();

            var panels = new Dictionary<Point, char>();
            var c = new LongCodeComputer(input);
            var position = new Point();
            var heading = Direction.Up;

            while (c.CurrentOpcode != LongCodeComputer.OpCode.Halt)
            {
                var currentPanel = panels.GetValueOrDefault(position, '.');
                var colorToPaint = c.RunWith(currentPanel == '█' ? 1 : 0, 2);
                if (colorToPaint == null)
                {
                    break;
                }
                panels[position] = colorToPaint.Value == 1 ? '█' : '.';
                heading = Turn(heading, c.Outputs.Dequeue());
                position += _mapDirectionToSize[heading];
            }

            Console.WriteLine($"Part 1: {panels.Count}");

            panels = new Dictionary<Point, char>();
            c = new LongCodeComputer(input);
            position = new Point();
            heading = Direction.Up;
            panels[Point.Empty] = '█';

            while (c.CurrentOpcode != LongCodeComputer.OpCode.Halt)
            {
                var currentPanel = panels.GetValueOrDefault(position, '.');
                var colorToPaint = c.RunWith(currentPanel == '█' ? 1 : 0, 2);
                if (colorToPaint == null)
                {
                    break;
                }
                panels[position] = colorToPaint.Value == 1 ? '█' : ' ';
                heading = Turn(heading, c.Outputs.Dequeue());
                position += _mapDirectionToSize[heading];
            }

            var minx = panels.Keys.Min(p => p.X);
            var maxx = panels.Keys.Max(p => p.X);
            var miny = panels.Keys.Min(p => p.Y);
            var maxy = panels.Keys.Max(p => p.Y);

            for (int y = miny; y <= maxy; y++)
            {
                for (int x = minx; x <= maxx; x++)
                {
                    var pixel = panels.GetValueOrDefault(new Point(x, y), ' ');
                    Console.Write(pixel);
                }
                Console.WriteLine();
            }
            Console.WriteLine();

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        private static Direction Turn(Direction heading, long turnDirection)
        {
            if (turnDirection == 0) // left            
                heading -= 1;

            else
                heading += 1;

            return (Direction)(((int)heading + 4) % 4);
        }
    }
}
