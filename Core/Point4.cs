﻿using System;

namespace Core
{
    public struct Point4 : IEquatable<Point4>
    {
        public static readonly Point4 Empty = new Point4(0, 0, 0, 0);

        public Point4(int x, int y, int z, int t)
        {
            X = x;
            Y = y;
            Z = z;
            T = t;
        }

        public bool IsEmpty => this.Equals(Empty);
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int T { get; set; }


        public override bool Equals(object? obj) => (obj is Point4 p) && this.Equals(p);

        public bool Equals(Point4 other) => (X == other.X) && (Y == other.Y) && (Z == other.Z) && (T == other.T);

        public override int GetHashCode() => HashCode.Combine(X, Y, Z, T);

        public Point4 TranslateBy(int dx, int dy, int dz, int dt) => new Point4(X + dx, Y + dy, Z + dz, T + dt);
        public static Point4 FromArray(int[] arr, int offset = 0)
        {
            if (arr == null)
                throw new ArgumentNullException(nameof(arr));

            if (arr.Length < offset + 4)
                throw new ArgumentException("Not enough elements in the array!");

            return new Point4(arr[offset], arr[offset + 1], arr[offset + 2], arr[offset + 3]);
        }

        public static bool operator ==(Point4 left, Point4 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Point4 left, Point4 right)
        {
            return !(left == right);
        }
    }
}
