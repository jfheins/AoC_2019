using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using Core;
using Core.Combinatorics;
using MoreLinq;

namespace Day_12
{
    struct Problem1D
    {
        public Vector128<int> Positions;
        public Vector128<int> Velocities;
    }

    class Program
    {
        static void Main()
        {
            var input = File.ReadAllLines("../../../input.txt");

            var sw = new Stopwatch();
            sw.Start();

            var coords = input.Select(s => Point3.FromArray(s.ParseInts(3))).ToList();
            var bodies = coords.Select(c => new Body(c)).ToList();

            var problems = new Problem1D[3];
            var periods = new int[3];
            var stepCount = 0;

            const int wndSize = 16;
            var referenceX = new Vector128<int>[wndSize];
            var referenceY = new Vector128<int>[wndSize];
            var referenceZ = new Vector128<int>[wndSize];

            var windowX = new Vector128<int>[wndSize];
            var windowY = new Vector128<int>[wndSize];
            var windowZ = new Vector128<int>[wndSize];

            problems[0].Positions = Vector128.Create(coords[0].X, coords[1].X, coords[2].X, coords[3].X);
            problems[1].Positions = Vector128.Create(coords[0].Y, coords[1].Y, coords[2].Y, coords[3].Y);
            problems[2].Positions = Vector128.Create(coords[0].Z, coords[1].Z, coords[2].Z, coords[3].Z);

            bool ArrayEquals<T>(T[] fa, T[] sa)
            {
                var cmp = EqualityComparer<T>.Default;

                for (int j1 = 0; j1 < fa.Length; ++j1)
                    if (!cmp.Equals(fa[j1], sa[j1]))
                        return false;
                return true;
            }

            // Initial Window
            for (int i = 0; i < wndSize; i++)
            {
                Step(ref problems[0]);
                Step(ref problems[1]);
                Step(ref problems[2]);

                referenceX[i] = problems[0].Positions;
                referenceY[i] = problems[1].Positions;
                referenceZ[i] = problems[2].Positions;

                windowX[i] = problems[0].Positions;
            }

            for (int i = 0; i < 1000000; i++)
            {
                if (periods[0] == 0)
                {
                    Array.Copy(windowX, 1, windowX, 0, wndSize - 1);
                    Step(ref problems[0]);
                    windowX[wndSize - 1] = problems[0].Positions;
                    if (ArrayEquals(referenceX, windowX))
                    {
                        periods[0] = stepCount;
                    }
                }


                if (periods[1] == 0)
                {
                    Step(ref problems[1]);
                    Array.Copy(windowY, 1, windowY, 0, wndSize - 1);
                    windowY[wndSize - 1] = problems[1].Positions;
                    if (ArrayEquals(referenceY, windowY))
                    {
                        periods[1] = stepCount;
                    }
                }


                if (periods[2] == 0)
                {
                    Step(ref problems[2]);
                    Array.Copy(windowZ, 1, windowZ, 0, wndSize - 1);
                    windowZ[wndSize - 1] = problems[2].Positions;
                    if (ArrayEquals(referenceZ, windowZ))
                    {
                        periods[2] = stepCount;
                    }
                }

                stepCount++;
            }
            //Console.WriteLine($"Part 1: {CalculateEnergy(bodies)}");

            Console.WriteLine($"Part 2: Period X = {periods[0]}");
            Console.WriteLine($"Part 2: Period Y = {periods[1]}");
            Console.WriteLine($"Part 2: Period Z = {periods[2]}");

            var bodiesGkv = periods.Aggregate(1UL, (gkv, b) => determineLCM((ulong)b, gkv));

            Console.WriteLine($"Part 2: Period = {bodiesGkv}");
            Console.WriteLine($"Should: Period = 4686774924");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        private static void Step(ref Problem1D p)
        {
            var shifted1 = Sse2.Shuffle(p.Positions, 0b10010011);
            var shifted2 = Sse2.Shuffle(p.Positions, 0b01001110);
            var shifted3 = Sse2.Shuffle(p.Positions, 0b00111001);

            // Calculate velocity additions
            var adds = Sse2.CompareGreaterThan(p.Positions, shifted1);
            adds = Sse2.Add(adds, Sse2.CompareGreaterThan(p.Positions, shifted2));
            adds = Sse2.Add(adds, Sse2.CompareGreaterThan(p.Positions, shifted3));

            // Calculate velocity subtractions
            adds = Sse2.Subtract(adds, Sse2.CompareLessThan(p.Positions, shifted1));
            adds = Sse2.Subtract(adds, Sse2.CompareLessThan(p.Positions, shifted2));
            adds = Sse2.Subtract(adds, Sse2.CompareLessThan(p.Positions, shifted3));

            // Add to velocity
            p.Velocities = Sse2.Add(p.Velocities, adds);
            // Add to pos
            p.Positions = Sse2.Add(p.Positions, p.Velocities);
        }

        private static object CalculateEnergy(List<Body> bodies) => bodies.Sum(b => b.Energy);



        private static ulong determineLCM(ulong a, ulong b)
        {
            ulong num1, num2;
            if (a > b)
            {
                num1 = a;
                num2 = b;
            }
            else
            {
                num1 = b;
                num2 = a;
            }

            for (ulong i = 1; i < num2; i++)
            {
                if ((num1 * i) % num2 == 0)
                {
                    return i * num1;
                }
            }
            return num1 * num2;
        }


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

    struct PeriodicCollection
    {
        public List<int> History;
        public HashSet<(int, int, int)> Items;
        public ulong Period;

        private int last;
        private int secondlast;

        public PeriodicCollection(ulong x)
        {
            History = new List<int>();
            Items = new HashSet<(int, int, int)>();
            Period = x;
            last = secondlast = 0;
        }

        public void Add(int v)
        {
            if (Period > 0)
                return;

            const int wndSize = 2000;
            var currenttuple = (secondlast, last, v);

            if (Items.Contains(currenttuple) && History.Count > wndSize)
            {
                Console.WriteLine("Possible");
                // Possibly periodic
                var search = History.GetRange(History.Count - wndSize, wndSize);
                var foundIndicies = History.StartingIndex(search);
                var canidate = foundIndicies.OrderByDescending(x => x).Skip(1).FirstOrDefault();
                if (canidate > 0 && canidate < History.Count - wndSize)
                {
                    Period = (ulong)(History.Count - canidate - wndSize);
                    Console.WriteLine($"Period found between {canidate} and {History.Count - wndSize}");
                }
            }

            History.Add(v);

            if (History.Count > 3)
                _ = Items.Add(currenttuple);

            secondlast = last;
            last = v;
        }
    }

    class Body
    {
        public Point3 Position;
        public Vector3 Velocity = Vector3.Zero;

        public PeriodicCollection x = new PeriodicCollection(0);
        public PeriodicCollection y = new PeriodicCollection(0);
        public PeriodicCollection z = new PeriodicCollection(0);

        public ulong Period => determineLCM(determineLCM(x.Period, y.Period), z.Period);

        public bool IsPeriodic => x.Period > 0 && y.Period > 0 && z.Period > 0;

        public Body(Point3 point3)
        {
            Position = point3;
        }

        public void SaveState(int iteration)
        {
            if (IsPeriodic)
                return;

            x.Add(Position.X);
            y.Add(Position.Y);
            z.Add(Position.Z);
        }


        private static ulong determineLCM(ulong a, ulong b)
        {
            ulong num1, num2;
            if (a > b)
            {
                num1 = a;
                num2 = b;
            }
            else
            {
                num1 = b;
                num2 = a;
            }

            for (ulong i = 1; i < num2; i++)
            {
                if ((num1 * i) % num2 == 0)
                {
                    return i * num1;
                }
            }
            return num1 * num2;
        }

        public int Energy => (Math.Abs(Position.X) + Math.Abs(Position.Y) + Math.Abs(Position.Z))
            * (Math.Abs((int)Velocity.X) + Math.Abs((int)Velocity.Y) + Math.Abs((int)Velocity.Z));

        public override string ToString() => $"pos={Position}, vel={Velocity}";
        public int[] ToArray()
            => new int[] { Position.X, Position.Y, Position.Z, (int)Velocity.X, (int)Velocity.Y, (int)Velocity.Z };
    }
}