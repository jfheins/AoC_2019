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
    public class LinqTests
    {
        [TestMethod]
        public void RightNumberOfDoubles()
        {
            static bool Check(int num) => num.ToString().ToCharArray().Chunks().Any(c => c.Count() == 2);

            var range = Enumerable.Range(134564, 450596);
            Assert.AreEqual(166392, range.Count(Check));
        }
    }
}
