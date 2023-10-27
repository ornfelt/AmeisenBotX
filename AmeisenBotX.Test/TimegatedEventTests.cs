﻿using AmeisenBotX.Common.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace AmeisenBotX.Test
{
    /// <summary>
    /// Tests the functionality of the TimegateTest method.
    /// </summary>
    [TestClass]
    public class TimegatedEventTests
    {
        /// <summary>
        /// Tests the functionality of the TimegateTest method.
        /// </summary>
        [TestMethod]
        public void TimegateTest()
        {
            int counter = 0;
            TimegatedEvent eventA = new(TimeSpan.FromMilliseconds(1), () => { ++counter; });

            for (int i = 0; i < 6; ++i)
            {
                eventA.Run();
                Thread.Sleep(1);
            }

            Assert.AreEqual(6, counter);
        }
    }
}