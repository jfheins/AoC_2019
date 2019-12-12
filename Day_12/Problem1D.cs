using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Day_12
{
    class Problem1D
    {
        public Vector128<int> Positions;
        public Vector128<int> Velocities;


        private new Vector128<int>[] referenceArray;
        private new Vector128<int>[] window;

        public Problem1D(Vector128<int> positions)
        {
            Positions = positions;
        }

        public void Init(int windowSize)
        {
            referenceArray = new Vector128<int>[windowSize];
            window = new Vector128<int>[windowSize];

            for (int i = 0; i < windowSize; i++)
            {
                Step();
                referenceArray[i] = Positions;
            }
        }

        public int Solve()
        {
            var stepCount = 0;
            while (true)
            {
                stepCount++;
                Array.Copy(window, 1, window, 0, window.Length - 1);
                Step();
                window[^1] = Positions;
                if (ArrayEquals(referenceArray, window))
                {
                    return stepCount;
                }
            }

        }

        public void Step()
        {
            var shifted1 = Sse2.Shuffle(Positions, 0b10010011);
            var shifted2 = Sse2.Shuffle(Positions, 0b01001110);
            var shifted3 = Sse2.Shuffle(Positions, 0b00111001);

            // Calculate velocity additions
            var adds = Sse2.CompareGreaterThan(Positions, shifted1);
            adds = Sse2.Add(adds, Sse2.CompareGreaterThan(Positions, shifted2));
            adds = Sse2.Add(adds, Sse2.CompareGreaterThan(Positions, shifted3));

            // Calculate velocity subtractions
            adds = Sse2.Subtract(adds, Sse2.CompareLessThan(Positions, shifted1));
            adds = Sse2.Subtract(adds, Sse2.CompareLessThan(Positions, shifted2));
            adds = Sse2.Subtract(adds, Sse2.CompareLessThan(Positions, shifted3));

            Velocities = Sse2.Add(Velocities, adds);
            Positions = Sse2.Add(Positions, Velocities);
        }

        private bool ArrayEquals<T>(T[] fa, T[] sa)
        {
            var cmp = EqualityComparer<T>.Default;

            for (int j1 = 0; j1 < fa.Length; ++j1)
            {
                if (!cmp.Equals(fa[j1], sa[j1]))
                    return false;
            }

            return true;
        }
    }
}