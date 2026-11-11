using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Structures;
using System;

namespace OOP_UnitTests.Structures {
    [TestClass]
    public class DimensionsTest {
        [TestMethod]
        [TestCategory("Basic")]
        public void Constructor_ValidValues_SetsAndRoundsProperties() {
            var dimensions = new Dimensions(10.1234, 20.5678, 30.9999);

            Assert.AreEqual(10.123, dimensions.Width);
            Assert.AreEqual(20.568, dimensions.Height);
            Assert.AreEqual(31.0, dimensions.Length);
        }


        [DataTestMethod]
        [TestCategory("InvalidParams")]
        [DataRow(0)]
        [DataRow(-5.5)]
        public void Constructor_InvalidWidth_ThrowsArgumentOutOfRangeException(double invalidWidth) {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Dimensions(invalidWidth, 10, 10));
        }

        [DataTestMethod]
        [TestCategory("InvalidParams")]
        [DataRow(0)]
        [DataRow(-1.2)]
        public void Constructor_InvalidHeight_ThrowsArgumentOutOfRangeException(double invalidHeight) {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Dimensions(10, invalidHeight, 10));
        }

        [DataTestMethod]
        [TestCategory("InvalidParams")]
        [DataRow(0)]
        [DataRow(-100)]
        public void Constructor_InvalidLength_ThrowsArgumentOutOfRangeException(double invalidLength) {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Dimensions(10, 10, invalidLength));
        }

        [TestMethod]
        [TestCategory("NewParam")]
        public void WithWidth_ValidValue_ReturnsNewInstanceWithUpdatedWidth() {
            var original = new Dimensions(10, 20, 30);
            var updated = original.WithWidth(15.5555);

            Assert.AreEqual(10, original.Width);
            Assert.AreEqual(15.556, updated.Width);
            Assert.AreEqual(20, updated.Height);
            Assert.AreEqual(30, updated.Length);
        }

        [TestMethod]
        [TestCategory("NewParam")]
        public void WithHeight_ValidValue_ReturnsNewInstanceWithUpdatedHeight() {
            var original = new Dimensions(10, 20, 30);
            var updated = original.WithHeight(25.1111);

            Assert.AreEqual(20, original.Height);
            Assert.AreEqual(10, updated.Width);
            Assert.AreEqual(25.111, updated.Height);
            Assert.AreEqual(30, updated.Length);
        }

        [TestMethod]
        [TestCategory("NewParam")]
        public void WithLength_ValidValue_ReturnsNewInstanceWithUpdatedLength() {
            var original = new Dimensions(10, 20, 30);
            var updated = original.WithLength(35.8888);

            Assert.AreEqual(30, original.Length);
            Assert.AreEqual(10, updated.Width);
            Assert.AreEqual(20, updated.Height);
            Assert.AreEqual(35.889, updated.Length);
        }
    }
}
