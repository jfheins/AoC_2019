using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Core;
using Core.Combinatorics;

namespace Day_19
{
    class Program
    {
        private static long[] _input;
        private static Dictionary<Point, bool> _pointCache;

        static void Main()
        {
            var sw = new Stopwatch();
            sw.Start();

            _input = File.ReadAllText("../../../input.txt").ParseLongs();

            _pointCache = new Dictionary<Point, bool>();

            var computer = LongCodeComputer.FromFile(@"../../../input.txt");
            var sum = 0;
            for (int y = 0; y < 50; y++)
            {
                for (int x = 0; x < 50; x++)
                {
                    var inBeam = getPoint(new Point(x, y));
                    sum += inBeam ? 1 : 0;
                    Console.Write(inBeam ? '#' : '.');
                }
                Console.WriteLine();
            }
            Console.WriteLine($"Part 1: {sum}");

            var probe = new Point(0, 10);

            var diagMove = new Size(1, 1);
            var topLeft = Point.Empty;

            while (topLeft == Point.Empty)
            {

                while (!getPoint(probe))
                    probe = probe.MoveTo(Direction.Right);

                var firstxAfterBeam = new BinarySearchLong(x => !getPoint(new Point((int)x, probe.Y))).FindFirst(probe.X);
                var probe2 = new Point((int)firstxAfterBeam, probe.Y);

                var beamWidth = probe2.X - probe.X;

                for (int x = 0; x < beamWidth - 90; x++)
                {
                    probe2 = probe + new Size(x, 0);
                    var rect = new Rectangle(probe2, new Size(100, 100));
                    if (allPointsInBeam(rect))
                    {
                        topLeft = rect.Location;
                        //Paint(rect.Location, 100);
                        break;
                    }
                }
                probe = probe.MoveTo(Direction.Down);
            }

            Console.WriteLine($"Part 2: topleft point is at {topLeft} => answer = {topLeft.X * 10000 + topLeft.Y}");
            Console.WriteLine("15231022");
            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        private static void Paint(Point start, int size)
        {
            for (int y = start.Y; y < start.Y + size; y++)
            {
                for (int x = start.X; x < start.X + size; x++)
                {
                    var inBeam = getPoint(new Point(x, y));
                    Console.Write(inBeam ? '#' : '.');
                }
                Console.WriteLine();
            }
        }

        private static bool getPoint(int x, int y)
        {
            return _pointCache.GetOrAdd(new Point(x, y), pos =>
            {
                var computer = new LongCodeComputer(_input);
                computer.Inputs.Enqueue(pos.X);
                computer.Inputs.Enqueue(pos.Y);
                var p = computer.RunForOutputs(1);
                return computer.Outputs.Dequeue() == 1;
            });
        }

        private static bool allPointsInBeam(Rectangle rect)
        {
            var res = true;
            res = res && getPoint(rect.Left + rect.Width - 1, rect.Top);
            res = res && getPoint(rect.Left, rect.Top + rect.Height - 1);
            res = res && getPoint(rect.Left + rect.Width - 1, rect.Top + rect.Height - 1);
            res = res && getPoint(rect.Left, rect.Top);
            return res;
        }
    }
}
