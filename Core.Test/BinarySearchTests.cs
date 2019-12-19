using Microsoft.VisualStudio.TestTools.UnitTesting;

using Core;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Core.Test
{
    [TestClass]
    public class BinarySearchTests
    {
        [TestMethod]
        public void FindsFirstItemOpenInterval()
        {
            var search = new BinarySearch<int>(x => x > 22).FindFirst();
        }

        [TestMethod]
        public void GridGetsPopulated()
        {
            var grid = new Grid2<int>(p => p.X + p.Y);

            Assert.AreEqual(grid[1, 1], 2);
        }
    }
}
