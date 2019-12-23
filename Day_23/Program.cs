using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Core;
using Core.Combinatorics;

namespace Day_23
{
    class Program
    {
        static void Main()
        {
            var sw = new Stopwatch();
            sw.Start();

            var computers = new LongCodeComputer[50];
            for (int i = 0; i < 50; i++)
            {
                computers[i] = LongCodeComputer.FromFile(@"../../../input.txt");
                ;
                computers[i].Inputs.Enqueue(i);
                computers[i].DefaultInput = -1;
            }

            Packet santaPacket = new Packet();
            while (santaPacket.addr == 0)
            {
                foreach (var c in computers)
                {
                    c.Run(20);
                }
                var packets = computers.SelectMany(c => CollectPackets(c)).ToList();
                foreach (var p in packets)
                {
                    if (p.addr == 255)
                        santaPacket = p;
                    else
                        QueuePacket(computers[p.addr], p);
                }
            }


            Console.WriteLine($"Part 1: santa packet has y value {santaPacket.y}");

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }

        private static void QueuePacket(LongCodeComputer c, Packet  p)
        {
            c.Inputs.Enqueue(p.x);
            c.Inputs.Enqueue(p.y);
        }

        private static IEnumerable<Packet> CollectPackets(LongCodeComputer c)
        {
            while (c.Outputs.Count >= 3)
            {
                var addr = c.Outputs.Dequeue();
                var x = c.Outputs.Dequeue();
                var y = c.Outputs.Dequeue();
                yield return new Packet { addr = addr, x = x, y = y };
            }
        }
    }

    struct Packet
    {
        public long addr; 
        public long x;
        public long y;
    }
}
