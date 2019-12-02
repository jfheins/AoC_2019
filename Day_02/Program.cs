﻿using System;

namespace Day_02
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = new int[] { 1, 0, 0, 3, 1, 1, 2, 3, 1, 3, 4, 3, 1, 5, 0, 3, 2, 1, 9, 19, 1, 13, 19, 23, 2, 23, 9, 27, 1, 6, 27, 31, 2, 10, 31, 35, 1, 6, 35, 39, 2, 9, 39, 43, 1, 5, 43, 47, 2, 47, 13, 51, 2, 51, 10, 55, 1, 55, 5, 59, 1, 59, 9, 63, 1, 63, 9, 67, 2, 6, 67, 71, 1, 5, 71, 75, 1, 75, 6, 79, 1, 6, 79, 83, 1, 83, 9, 87, 2, 87, 10, 91, 2, 91, 10, 95, 1, 95, 5, 99, 1, 99, 13, 103, 2, 103, 9, 107, 1, 6, 107, 111, 1, 111, 5, 115, 1, 115, 2, 119, 1, 5, 119, 0, 99, 2, 0, 14, 0 };

            input[1] = 12;
            input[2] = 2;

            //input = new int[] { 1, 0, 0, 0, 99 };

            Compute(input);

            Console.WriteLine(string.Join(",", input));

            Console.ReadLine();
        }

        private static void Compute(int[] program)
        {

            var opIdx = 0;

            while (program[opIdx] != 99)
            {
                var op = program[opIdx];

                if (op == 1)
                {
                    var sum = program[program[opIdx + 1]] + program[program[opIdx + 2]];
                    program[program[opIdx + 3]] = sum;
                }
                else if (op == 2)
                {
                    var prod = program[program[opIdx + 1]] * program[program[opIdx + 2]];
                    program[program[opIdx + 3]] = prod;
                }
                opIdx += 4;
            }
        }
    }
}
