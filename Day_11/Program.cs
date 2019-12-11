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
        private enum HullColor { Black, White };

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

            var panels = PaintHull(input);
            Console.WriteLine($"Part 1: {panels.Count}");

            panels = PaintHull(input, HullColor.White);

            foreach (var row in panels.Keys.PointsInBoundingRect())
            {
                foreach (var point in row)
                {
                    var pixel = panels.GetValueOrDefault(point, HullColor.Black);
                    Console.Write(pixel == HullColor.Black ? ' ' : '█');
                }
                Console.WriteLine();
            }
            Console.WriteLine();

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        private static Dictionary<Point, HullColor> PaintHull(long[] input, HullColor? initial = null)
        {
            var panels = new Dictionary<Point, HullColor>();
            var c = new LongCodeComputer(input);
            var position = new Point();
            var heading = Direction.Up;

            if (initial.HasValue)
                panels[Point.Empty] = initial.Value;

            while (c.CurrentOpcode != LongCodeComputer.OpCode.Halt)
            {
                var currentPanel = panels.GetValueOrDefault(position, HullColor.Black);
                var colorToPaint = c.RunWith((long)currentPanel, 2);
                if (colorToPaint == null)
                {
                    break;
                }
                panels[position] = (HullColor)colorToPaint.Value;
                heading = Turn(heading, c.Outputs.Dequeue());
                position += _mapDirectionToSize[heading];
            }
            return panels;
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
