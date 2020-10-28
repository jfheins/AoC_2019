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


        private Vector128<int> referencePos;
        private Vector128<int> referenceVel;

        public Problem1D(Vector128<int> positions)
        {
            Positions = positions;
        }

        public void Init()
        {
            referencePos = Positions;
            referenceVel = Velocities;
        }

        public int Solve()
        {
            var stepCount = 0;
            while (true)
            {
                stepCount++;
                Step();
                if (Positions.Equals(referencePos) && Velocities.Equals(referenceVel))
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
    }
}