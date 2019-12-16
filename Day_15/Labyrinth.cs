using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace Day_15
{
    class Labyrinth
    {
        public enum Tile { Origin, Empty, Wall, Oxygen };

        public ArraySegment<long> Data { get; }

        public Point Origin { get; private set; }
        public Point OxygenPos { get; private set; }
        private Rectangle InnerBounds = new Rectangle(1, 1, 39, 39);


        public Labyrinth(long[] input)
        {
            var dataOffset = (int)input[207];
            Debug.Assert(dataOffset == 252);

            Data = new ArraySegment<long>(input, dataOffset, 780);

            var playerX = input[1034];
            var playerY = input[1035];
            Origin = new Point((int)playerX, (int)playerY);
            OxygenPos = new Point((int)input[146], (int)input[153]);
        }

        public Tile GetTileAt(Point p)
        {
            if (!InnerBounds.Contains(p))
            {
                return Tile.Wall;
            }
            else if (p == Origin)
            {
                return Tile.Origin;
            }
            else if (p == OxygenPos)
            {
                return Tile.Oxygen;
            }
            else if ((p.X + p.Y) % 2 == 0)
            {
                // Both even: automatic wall, both odd: automatic empty
                return (p.X % 2) == 0 ? Tile.Wall : Tile.Empty;
            }
            else
            {
                var dataIndex = (((p.Y - 1) / 2) * 39) + p.X - 1;
                return Data[dataIndex] < 42 ? Tile.Empty : Tile.Wall;
            }
        }

        private static readonly Dictionary<Tile, char> _mapping = new Dictionary<Tile, char> {
            { Tile.Origin, '0' },
            { Tile.Empty, ' ' },
            { Tile.Wall, '█' },
            { Tile.Oxygen, 'x' }
        };

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (int y = 0; y <= 40; y++)
            {
                var line = new char[41];
                for (int x = 0; x <= 40; x++)
                {
                    var tile = GetTileAt(new Point(x, y));
                    line[x] = _mapping[tile];
                }
                _ = sb.Append(line);
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
