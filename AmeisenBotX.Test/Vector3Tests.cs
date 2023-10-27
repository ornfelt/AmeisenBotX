using AmeisenBotX.Common.Math;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AmeisenBotX.Test
{
    /// <summary>
    /// Test class for Vector3 addition.
    /// </summary>
    [TestClass]
    public class Vector3Tests
    {
        /// <summary>
        /// Test method for Vector3 addition.
        /// </summary>
        [TestMethod]
        public void Vector3AddTest()
        {
            Vector3 a = new(1, 1, 1);
            Vector3 b = new(1, 1, 1);

            Assert.AreEqual(new(2, 2, 2), a + 1f);
            Assert.AreEqual(new(2, 2, 2), a + b);

            Assert.AreEqual(new(1, 1, 1), a);
            Assert.AreEqual(new(1, 1, 1), b);

            a.Add(b);

            Assert.AreEqual(new(2, 2, 2), a);
            Assert.AreEqual(new(1, 1, 1), b);

            a.Add(1f);

            Assert.AreEqual(new(3, 3, 3), a);
        }

        /// <summary>
        /// Test the comparison methods of the Vector3 class.
        /// </summary>
        [TestMethod]
        public void Vector3ComparisonTest()
        {
            Vector3 a = new(2, 2, 2);
            Vector3 b = new(2, 2, 2);

            if (a != b)
            {
                Assert.Fail();
            }

            b = new(2, 1, 2);

            if (a == b)
            {
                Assert.Fail();
            }

            if (a > b)
            {
                Assert.Fail();
            }

            b = new(2, 2, 2);

            if (!a.Equals(b))
            {
                Assert.Fail();
            }

            if (a < b || a > b)
            {
                Assert.Fail();
            }

            b = new(2, 3, 2);

            if (a < b)
            {
                Assert.Fail();
            }
        }

        /// <summary>
        /// This method tests the functionality of the Vector3DistanceTest method.
        /// It creates two Vector3 objects, a and b, with coordinates (2, 2, 2) and (2, 2, 2) respectively.
        /// It checks if the distance between a and b using the GetDistance method is equal to 0.0.
        /// If the condition is not met, the test fails.
        /// It then assigns new coordinates (2, 2, 1) to b and checks if the distance between a and b using the GetDistance method is equal to 1.0.
        /// If the condition is not met, the test fails.
        /// It also checks if the distance between a and b in the XY plane using the GetDistance2D method is equal to 0.0.
        /// If the condition is not met, the test fails.
        /// It assigns new coordinates (2, 2, 0) to b and checks if the distance between a and b using the GetDistance method is equal to 2.0.
        /// If the condition is not met, the test fails.
        /// It also checks if the distance between a and b in the XY plane using the GetDistance2D method is equal to 0.0.
        /// If the condition is not met, the test fails.
        /// </summary>
        [TestMethod]
        public void Vector3DistanceTest()
        {
            Vector3 a = new(2, 2, 2);
            Vector3 b = new(2, 2, 2);

            if (a.GetDistance(b) != 0.0)
            {
                Assert.Fail();
            }

            b = new(2, 2, 1);

            if (a.GetDistance(b) != 1.0)
            {
                Assert.Fail();
            }

            if (a.GetDistance2D(b) != 0.0)
            {
                Assert.Fail();
            }

            b = new(2, 2, 0);

            if (a.GetDistance(b) != 2.0)
            {
                Assert.Fail();
            }

            if (a.GetDistance2D(b) != 0.0)
            {
                Assert.Fail();
            }
        }

        /// <summary>
        /// Test method for Vector3 Divide operation.
        /// </summary>
        [TestMethod]
        public void Vector3DivideTest()
        {
            Vector3 a = new(2, 2, 2);
            Vector3 b = new(2, 2, 2);

            Assert.AreEqual(new(1, 1, 1), a / 2f);
            Assert.AreEqual(new(1, 1, 1), a / b);

            Assert.AreEqual(new(2, 2, 2), a);
            Assert.AreEqual(new(2, 2, 2), b);

            a.Divide(b);

            Assert.AreEqual(new(1, 1, 1), a);
            Assert.AreEqual(new(2, 2, 2), b);

            a.Divide(2f);

            Assert.AreEqual(new(0.5f, 0.5f, 0.5f), a);
        }

        /// <summary>
        /// This method tests the behavior of the Limit method in the Vector3 class.
        /// It creates two instances of Vector3 object, a and b, with values (2, 2, 2) and (-2, -2, -2) respectively.
        /// It then calls the Limit method on object a with a limit of 1.
        /// After the limit is applied, it checks if any of the components (X, Y, Z) of object a are exceeding the limit of 1.
        /// If any component exceeds the limit, the test fails.
        /// Next, the Limit method is called on object b with a limit of -1.
        /// After the limit is applied, it checks if any of the components (X, Y, Z) of object b are lesser than the limit of -1.
        /// If any component is lesser than the limit, the test fails.
        /// </summary>
        [TestMethod]
        public void Vector3LimitTest()
        {
            Vector3 a = new(2, 2, 2);
            Vector3 b = new(-2, -2, -2);

            a.Limit(1);

            if (a.X > 1 || a.Y > 1 || a.Z > 1)
            {
                Assert.Fail();
            }

            b.Limit(-1);

            if (b.X < -1 || b.Y < -1 || b.Z < -1)
            {
                Assert.Fail();
            }
        }

        /// <summary>
        /// Tests the calculation of the magnitude of a Vector3 in 2D and 3D space.
        /// </summary>
        [TestMethod]
        public void Vector3MagnitudeTest()
        {
            Vector3 a = new(2, 0, 2000);
            Vector3 b = new(-2, 0, -2000);

            Assert.AreEqual(2f, a.GetMagnitude2D());
            Assert.AreEqual(2f, b.GetMagnitude2D());

            a = new(0, 0, 2);
            b = new(0, 0, -2);

            Assert.AreEqual(2f, a.GetMagnitude());
            Assert.AreEqual(2f, b.GetMagnitude());
        }

        /// <summary>
        /// This is a test method for miscellaneous operations on the Vector3 class.
        /// It verifies the correctness of GetHashCode, ToArray, FromArray, and copy constructor functionalities.
        /// </summary>
        [TestMethod]
        public void Vector3MiscTest()
        {
            Vector3 a = new(2, 1, 2000);
            Vector3 b = new(2, 1, 2000);

            if (a.GetHashCode() != b.GetHashCode())
            {
                Assert.Fail();
            }

            float[] x = new float[] { 2f, 1f, 2000f };
            float[] y = a.ToArray();

            Assert.AreEqual(x[0], y[0]);
            Assert.AreEqual(x[1], y[1]);
            Assert.AreEqual(x[2], y[2]);

            Assert.AreEqual(a, Vector3.FromArray(y));

            Vector3 c = new(a);

            Assert.AreEqual(a, c);
        }

        /// <summary>
        /// Tests the multiplication functionality of the Vector3 class.
        /// </summary>
        [TestMethod]
        public void Vector3MultiplyTest()
        {
            Vector3 a = new(2, 2, 2);
            Vector3 b = new(2, 2, 2);

            Assert.AreEqual(new(4, 4, 4), a * 2f);

            Assert.AreEqual(new(4, 4, 4), a * b);

            Assert.AreEqual(new(2, 2, 2), a);
            Assert.AreEqual(new(2, 2, 2), b);

            a.Multiply(b);

            Assert.AreEqual(new(4, 4, 4), a);
            Assert.AreEqual(new(2, 2, 2), b);

            a.Multiply(2f);

            Assert.AreEqual(new(8, 8, 8), a);
        }

        /// <summary>
        /// This method tests the normalization of a Vector3 object.
        /// </summary>
        [TestMethod]
        public void Vector3NormalizingTest()
        {
            Vector3 a = new(2, 1, 2000);
            Vector3 b = new(-2, -1, -2000);

            a.Normalize2D();

            Assert.AreEqual(0.8944, Math.Round(a.X, 4));
            Assert.AreEqual(0.4472, Math.Round(a.Y, 4));
            Assert.AreEqual(2000f, a.Z);

            b.Normalize2D();

            Assert.AreEqual(-0.8944, Math.Round(b.X, 4));
            Assert.AreEqual(-0.4472, Math.Round(b.Y, 4));
            Assert.AreEqual(-2000f, b.Z);

            a = new(1, 2, 4);
            b = new(-1, -2, -4);

            a.Normalize();

            Assert.AreEqual(0.2182, Math.Round(a.X, 4));
            Assert.AreEqual(0.4364, Math.Round(a.Y, 4));
            Assert.AreEqual(0.8729, Math.Round(a.Z, 4));

            b.Normalize();

            Assert.AreEqual(-0.2182, Math.Round(b.X, 4));
            Assert.AreEqual(-0.4364, Math.Round(b.Y, 4));
            Assert.AreEqual(-0.8729, Math.Round(b.Z, 4));
        }

        /// <summary>
        /// This method tests the rotation of a Vector3 object in degrees and radians.
        /// It creates a Vector3 object, initializes it with the values (1, 0, 0), and then rotates it 180 degrees.
        /// After the rotation, the X component of the Vector3 object is asserted to be -1f.
        /// The method then resets the Vector3 object with the values (1, 0, 0) and rotates it by MathF.PI radians.
        /// Again, the X component of the Vector3 object is asserted to be -1f.
        /// </summary>
        [TestMethod]
        public void Vector3RotationTest()
        {
            Vector3 a = new(1, 0, 0);
            a.Rotate(180);

            Assert.AreEqual(-1f, a.X);

            a = new(1, 0, 0);
            a.RotateRadians(MathF.PI);

            Assert.AreEqual(-1f, a.X);
        }

        /// <summary>
        /// Tests the subtract operation of Vector3.
        /// </summary>
        [TestMethod]
        public void Vector3SubtractTest()
        {
            Vector3 a = new(1, 1, 1);
            Vector3 b = new(1, 1, 1);

            Assert.AreEqual(new(0, 0, 0), a - 1f);
            Assert.AreEqual(new(0, 0, 0), a - b);

            Assert.AreEqual(new(1, 1, 1), a);
            Assert.AreEqual(new(1, 1, 1), b);

            a.Subtract(b);

            Assert.AreEqual(new(0, 0, 0), a);
            Assert.AreEqual(new(1, 1, 1), b);

            a.Subtract(1f);

            Assert.AreEqual(new(-1, -1, -1), a);
        }

        /// <summary>
        /// This method tests the behavior of the Vector3.Zero property.
        /// It creates a new Vector3 object with all components set to zero and compares it with the Vector3.Zero property.
        /// The test passes if the two objects are equal.
        /// </summary>
        [TestMethod]
        public void Vector3ZeroTest()
        {
            Vector3 a = new(0, 0, 0);
            Assert.AreEqual(a, Vector3.Zero);
        }
    }
}