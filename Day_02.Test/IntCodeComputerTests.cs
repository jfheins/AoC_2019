using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Day_02.Test
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
        public void LimitedExecution()
        {
            var computer = new IntCodeComputer(new int[] { 1, 1, 1, 4, 99, 5, 6, 0, 99 });
            computer.Run(1);
            var expected = new int[] { 1, 1, 1, 4, 2, 5, 6, 0, 99 };
            CollectionAssert.AreEqual(expected, computer.Memory);
        }
    }
}
