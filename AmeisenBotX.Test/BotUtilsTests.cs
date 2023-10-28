using AmeisenBotX.Common.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Tests the BigValueToString method with various input values and verifies the expected output.
/// </summary>
namespace AmeisenBotX.Test
{
    /// <summary>
    /// Tests the BigValueToString method with various input values and verifies the expected output.
    /// </summary>
    [TestClass]
    public class BotUtilsTests
    {
        /// <summary>
        /// Tests the BigValueToString method with various input values and verifies the expected output.
        /// </summary>
        [TestMethod]
        public void BigValueToStringTest()
        {
            Assert.AreEqual("1", BotUtils.BigValueToString(1));
            Assert.AreEqual("10", BotUtils.BigValueToString(10));
            Assert.AreEqual("100", BotUtils.BigValueToString(100));
            Assert.AreEqual("1000", BotUtils.BigValueToString(1000));
            Assert.AreEqual("10000", BotUtils.BigValueToString(10000));
            Assert.AreEqual("100K", BotUtils.BigValueToString(100000));
            Assert.AreEqual("1000K", BotUtils.BigValueToString(1000000));
            Assert.AreEqual("10000K", BotUtils.BigValueToString(10000000));
            Assert.AreEqual("100M", BotUtils.BigValueToString(100000000));
            Assert.AreEqual("1000M", BotUtils.BigValueToString(1000000000));
        }

        /// <summary>
        /// Tests the method ByteArrayToString in the BotUtils class.
        /// Converts a byte array to a string representation in the format "00 35 FF".
        /// The byte array to convert is { 0x0, 0x35, 0xff }.
        /// The expected string representation is "00 35 FF".
        /// </summary>
        [TestMethod]
        public void ByteArrayToStringTest()
        {
            byte[] bytes = new byte[] { 0x0, 0x35, 0xff };
            string s = BotUtils.ByteArrayToString(bytes);
            Assert.AreEqual("00 35 FF", s);
        }

        /// <summary>
        /// Tests the FastRandomStringOnlyLetters method to ensure that the generated string contains only letters.
        /// </summary>
        [TestMethod]
        public void FastRandomStringOnlyLettersTest()
        {
            List<char> numbers = new() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            for (int i = 0; i < 16; ++i)
            {
                Assert.IsFalse(BotUtils.FastRandomStringOnlyLetters().Any(e => numbers.Contains(e)));
            }
        }

        /// <summary>
        /// Test method to check the functionality of the FastRandomString method.
        /// It generates a random string and verifies that the length is greater than 0.
        /// </summary>
        [TestMethod]
        public void FastRandomStringTest()
        {
            for (int i = 0; i < 16; ++i)
            {
                Assert.IsTrue(BotUtils.FastRandomString().Length > 0);
            }
        }

        /// <summary>
        /// Unit test for the ObfuscateLua method.
        /// It generates a random string using the FastRandomString method from the BotUtils class.
        /// Then, it creates a sample string with the format "{{v:0}}={x}" where x is the generated random string.
        /// The ObfuscateLua method is called with the sample string as the input and it returns a tuple (string, string).
        /// The test asserts that the first item of the tuple is equal to "{secondItem}={x}" where secondItem is the second item of the tuple and x is the generated random string.
        /// </summary>
        [TestMethod]
        public void ObfuscateLuaTest()
        {
            string x = BotUtils.FastRandomString();
            string sample = $"{{v:0}}={x}";
            (string, string) result = BotUtils.ObfuscateLua(sample);

            Assert.AreEqual(result.Item1, $"{result.Item2}={x}");
        }
    }
}