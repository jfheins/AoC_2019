using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
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

            IEnumerable<(int value, int layer)> ValuesAt(int x, int y)
                => layers.Select((arr, idx) => (arr[x + y * imgWidth], idx));

            (int value, int layer) FirstNonTransparentLayer(IEnumerable<(int value, int layer)> data)
                => data.First(x => x.value != 2);

            Color LayerToColor(int layer)
                => Color.FromArgb(255 - layer, 255 - layer, 255 - layer);

            var combined = layers
                .Aggregate((a, b) => a.Zip(b).Select(tupel => tupel.First == 2 ? tupel.Second : tupel.First).ToArray());

            Console.WriteLine();
            var bmp = new Bitmap(imgWidth, imgHeight);
            for (int y = 0; y < imgHeight; y++)
            {
                for (int x = 0; x < imgWidth; x++)
                {
                    var pixel = FirstNonTransparentLayer(ValuesAt(x, y));
                    var color = pixel.value == 0 ? Color.Black : LayerToColor(pixel.layer);
                    bmp.SetPixel(x, y, color);

                    var chr = (combined[x + y * imgWidth]) switch { 0 => ' ', 1 => '█', 2 => 'o' };
                    Console.Write(chr);
                }
                Console.WriteLine();
            }
            bmp.Save("../../../output.png", ImageFormat.Png);
            Console.WriteLine();

            sw.Stop();
            Console.WriteLine($"Solving took {sw.ElapsedMilliseconds}ms.");
            _ = Console.ReadLine();
        }
    }
}
