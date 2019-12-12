using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using Core;
using Core.Combinatorics;
using MoreLinq;

namespace Day_12
{
    class Program
    {
        static void Main()
        {
            var input = File.ReadAllLines("../../../input.txt");

            var sw = new Stopwatch();
            sw.Start();

            var bodies = input.Select(s => new Body(Point3.FromArray(s.ParseInts(3)))).ToList();
            var states = new List<int[]>();

            for (int i = 0; i < 100000; i++)
            {
                Step(bodies);
                foreach (var b in bodies)
                {
                    b.SaveState(i);
                }
                if (bodies.All(b => b.IsPeriodic))
                {
                    Console.WriteLine("All periodic");
                    foreach (var b in bodies)
                    {
                        Console.WriteLine($"Period: {b.Period}");
                    }

                    break;
                }
            }

            Console.WriteLine($"Part 1: {CalculateEnergy(bodies)}");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        private static object CalculateEnergy(List<Body> bodies) => bodies.Sum(b => b.Energy);

        static void Step(ICollection<Body> bodies)
        {
            CalculateVelocity(bodies);
            CalculateNewPositions(bodies);
        }

        private static void CalculateVelocity(ICollection<Body> bodies)
        {
            foreach (var pair in bodies.Subsets(2))
            {
                var delta = CalculateDelta(pair[0].Position, pair[1].Position);
                pair[0].Velocity += delta;
                pair[1].Velocity -= delta;
            }
        }

        private static Vector3 CalculateDelta(Point3 a, Point3 b)
        {
            return new Vector3(Math.Sign(b.X - a.X), Math.Sign(b.Y - a.Y), Math.Sign(b.Z - a.Z));
        }

        private static void CalculateNewPositions(ICollection<Body> bodies)
        {
            foreach (var b in bodies)
            {
                b.Position = b.Position.TranslateBy(b.Velocity);
            }
        }
    }

    class Body
    {
        public Point3 Position;
        public Vector3 Velocity = Vector3.Zero;

        public List<Point3> PositionHistory = new List<Point3>();
        public List<Vector3> VelHist = new List<Vector3>();

        public HashSet<Point3> PastPos = new HashSet<Point3>();
        public HashSet<Vector3> PastVel = new HashSet<Vector3>();

        public int Period = 0;

        public bool IsPeriodic => Period > 0;

        public Body(Point3 point3)
        {
            Position = point3;
        }

        public void SaveState(int iteration)
        {
            if (IsPeriodic)
                return;

            if (PastPos.Contains(Position))
            {
                if (PastPos.Contains(PositionHistory[PositionHistory.Count - 1])
                    && PastPos.Contains(PositionHistory[PositionHistory.Count - 2]))
                {

                    // Possibly periodic
                    var search = PositionHistory.GetRange(PositionHistory.Count - 10, 10);
                    var first = PositionHistory.StartingIndex(search).First();
                    if (first < PositionHistory.Count - 10)
                    {
                        Period = PositionHistory.Count - first - 10;
                    }
                }
            }

            PositionHistory.Add(Position);
            PastPos.Add(Position);
            //VelHist.Add(Velocity);
            //PastVel.Add(Velocity);
        }

        public int Energy => (Math.Abs(Position.X) + Math.Abs(Position.Y) + Math.Abs(Position.Z))
            * (Math.Abs((int)Velocity.X) + Math.Abs((int)Velocity.Y) + Math.Abs((int)Velocity.Z));

        public override string ToString() => $"pos={Position}, vel={Velocity}";
        public int[] ToArray()
            => new int[] { Position.X, Position.Y, Position.Z, (int)Velocity.X, (int)Velocity.Y, (int)Velocity.Z };
    }
}