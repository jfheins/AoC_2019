using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test
{
    [TestClass]
    public class IntCodeComputerTests
    {
        [TestMethod]
        public void CanCreate()
        {
            var state = new int[] { 17 };
            var computer = new IntCodeComputer(state);

            Assert.IsTrue(computer.Memory.Length == 1);
            Assert.IsTrue(computer.Memory[0] == 17);
        }

        [TestMethod]
        public void CanOverride()
        {
            var state = new int[] { 17, 33 };
            var computer = new IntCodeComputer(state, new Dictionary<int, int> { { 1, 6 } });

            Assert.IsTrue(computer.Memory.Length == 2);
            Assert.IsTrue(computer.Memory[0] == 17);
            Assert.IsTrue(computer.Memory[1] == 6);
        }

        [TestMethod]
        public void Program_1()
        {
            var computer = new IntCodeComputer(new int[] { 1, 0, 0, 0, 99 });
            computer.Run();
            var expected = new int[] { 2, 0, 0, 0, 99 };
            CollectionAssert.AreEqual(expected, computer.Memory);
            Assert.AreEqual(2, computer.StepCount);
        }

        [TestMethod]
        public void Program_2()
        {
            var computer = new IntCodeComputer(new int[] { 2, 3, 0, 3, 99 });
            computer.Run();
            var expected = new int[] { 2, 3, 0, 6, 99 };
            CollectionAssert.AreEqual(expected, computer.Memory);
        }

        [TestMethod]
        public void Program_3()
        {
            var computer = new IntCodeComputer(new int[] { 2, 4, 4, 5, 99, 0 });
            computer.Run();
            var expected = new int[] { 2, 4, 4, 5, 99, 9801 };
            CollectionAssert.AreEqual(expected, computer.Memory);
        }

        [TestMethod]
        public void Program_4()
        {
            var computer = new IntCodeComputer(new int[] { 1, 1, 1, 4, 99, 5, 6, 0, 99 });
            computer.Run();
            var expected = new int[] { 30, 1, 1, 4, 2, 5, 6, 0, 99 };
            CollectionAssert.AreEqual(expected, computer.Memory);
        }

        [TestMethod]
        public void Program_D5_negative_instr()
        {
            var computer = new IntCodeComputer(new int[] { 1101, 100, -1, 4, 0 });
            computer.Run(5);
            var expected = new int[] { 1101, 100, -1, 4, 99 };
            CollectionAssert.AreEqual(expected, computer.Memory);
        }

        [TestMethod]
        public void LimitedExecution()
        {
            var computer = new IntCodeComputer(new int[] { 1, 1, 1, 4, 99, 5, 6, 0, 99 });
            computer.Run(1);
            var expected = new int[] { 1, 1, 1, 4, 2, 5, 6, 0, 99 };
            CollectionAssert.AreEqual(expected, computer.Memory);
        }

        [TestMethod]
        public void InputOutputWorks()
        {
            var computer = new IntCodeComputer(new int[] { 3, 0, 4, 0, 99 });
            computer.Inputs.Add(37);
            computer.Run();
            Assert.AreEqual(37, computer.Outputs.First());
        }

        [TestMethod]
        public void ImmediateParamWorks()
        {
            var computer = new IntCodeComputer(new int[] { 1002, 4, 3, 4, 33 });
            computer.Run(2);
            var expected = new int[] { 1002, 4, 3, 4, 99 };
            CollectionAssert.AreEqual(expected, computer.Memory);
        }

        [DataTestMethod]
        [DataRow(6, 0)]
        [DataRow(7, 0)]
        [DataRow(8, 1)]
        [DataRow(9, 0)]
        public void CompareJump_1_equals_8(int input, int expected)
        {
            var computer = new IntCodeComputer(new int[] { 3, 9, 8, 9, 10, 9, 4, 9, 99, -1, 8 });
            computer.Inputs.Add(input);
            computer.Run(10);
            Assert.AreEqual(expected, computer.Outputs.First());
        }

        [DataTestMethod]
        [DataRow(6, 1)]
        [DataRow(7, 1)]
        [DataRow(8, 0)]
        [DataRow(9, 0)]
        public void CompareJump_2_lessthan_8(int input, int expected)
        {
            var computer = new IntCodeComputer(new int[] { 3, 9, 7, 9, 10, 9, 4, 9, 99, -1, 8 });
            computer.Inputs.Add(input);
            computer.Run(10);
            Assert.AreEqual(expected, computer.Outputs.First());
        }

        [DataTestMethod]
        [DataRow(6, 0)]
        [DataRow(7, 0)]
        [DataRow(8, 1)]
        [DataRow(9, 0)]
        public void CompareJump_3_equal_8(int input, int expected)
        {
            var computer = new IntCodeComputer(new int[] { 3, 3, 1108, -1, 8, 3, 4, 3, 99 });
            computer.Inputs.Add(input);
            computer.Run(10);
            Assert.AreEqual(expected, computer.Outputs.First());
        }

        [DataTestMethod]
        [DataRow(6, 1)]
        [DataRow(7, 1)]
        [DataRow(8, 0)]
        [DataRow(9, 0)]
        public void CompareJump_4_lessthan_8(int input, int expected)
        {
            var computer = new IntCodeComputer(new int[] { 3, 3, 1107, -1, 8, 3, 4, 3, 99 });
            computer.Inputs.Add(input);
            computer.Run(10);
            Assert.AreEqual(expected, computer.Outputs.First());
        }
    }
}
