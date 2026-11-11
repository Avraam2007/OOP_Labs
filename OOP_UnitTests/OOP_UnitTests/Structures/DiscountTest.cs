using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Structures;
using System;

namespace OOP_UnitTests.Structures {
    [TestClass]
    public class DiscountTests {
        [DataTestMethod]
        [DataRow(0)]
        [DataRow(15.5)]
        [DataRow(50)]
        [DataRow(100)]
        public void Constructor_ValidPercent_SetsPercentProperty(double validPercent) {
            var discount = new Discount(validPercent);

            Assert.AreEqual(validPercent, discount.Percent);
        }

        [DataTestMethod]
        [DataRow(-0.01)]
        [DataRow(-50)]
        [DataRow(100.01)]
        [DataRow(150)]
        public void Constructor_OutOfRangePercent_ThrowsArgumentOutOfRangeException(double invalidPercent) {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new Discount(invalidPercent));
        }
    }
}
