using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Entities;
using System;

namespace OOP_UnitTests.Entities {
    [TestClass]
    public class FurnitureTests {
        private Furniture _furniture;

        [TestInitialize]
        public void Setup() {
            _furniture = new Furniture(
                article: "FN-0001",
                name: "Oak Dining Table",
                price: 450.0,
                weight: 28.4567,
                width: 90.0,
                length: 180.0,
                height: 75.0,
                material: "Solid Oak",
                supplierId: 6
            );
        }

        [TestCleanup]
        public void Cleanup() {
            _furniture = null;
        }

        #region Constructor & Properties Validation Tests

        [TestMethod]
        public void Constructor_ValidArguments_InitializesAllPropertiesAndRoundsWeight() {
            Assert.IsInstanceOfType(_furniture, typeof(Product));
            Assert.AreEqual("FN-0001", _furniture.Article);
            Assert.AreEqual("Oak Dining Table", _furniture.Name);
            Assert.AreEqual(450.0, _furniture.Price, 0.001);
            Assert.AreEqual(28.457, _furniture.Weight, 0.0001);
            Assert.AreEqual(90.0, _furniture.Width, 0.001);
            Assert.AreEqual(180.0, _furniture.Length, 0.001);
            Assert.AreEqual(75.0, _furniture.Height, 0.001);
            Assert.AreEqual("Solid Oak", _furniture.Material);
            Assert.AreEqual(6, _furniture.SupplierId);
        }

        [TestMethod]
        public void Constructor_DefaultSupplierId_SetsSupplierIdToZero() {
            var chair = new Furniture("FN-0002", "Chair", 80.0, 5.5, 45, 45, 90, "Pine");

            Assert.AreEqual(0, chair.SupplierId);
        }

        [TestMethod]
        [DataRow(0.0)]
        [DataRow(-10.5)]
        public void Constructor_ZeroOrNegativeWeight_ThrowsArgumentOutOfRangeException(double invalidWeight) {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new Furniture("FN-0003", "Table", 100.0, invalidWeight, 50, 50, 50, "Wood"));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void Constructor_EmptyOrNullMaterial_ThrowsArgumentException(string invalidMaterial) {
            Assert.Throws<ArgumentException>(() =>
                new Furniture("FN-0004", "Table", 100.0, 10.0, 50, 50, 50, invalidMaterial));
        }

        [TestMethod]
        public void ProtectedDimensionSetters_UpdateWidthLengthAndHeightCorrectly() {
            var testableFurniture = new TestableFurniture("FN-0005", "Desk", 200.0, 15.0, 60, 120, 75, "MDF");

            testableFurniture.UpdateDimensions(width: 70, length: 140, height: 80);

            Assert.AreEqual(70.0, testableFurniture.Width, 0.001);
            Assert.AreEqual(140.0, testableFurniture.Length, 0.001);
            Assert.AreEqual(80.0, testableFurniture.Height, 0.001);
            Assert.AreEqual(70.0, testableFurniture.Dimensions.Width, 0.001);
            Assert.AreEqual(140.0, testableFurniture.Dimensions.Length, 0.001);
            Assert.AreEqual(80.0, testableFurniture.Dimensions.Height, 0.001);
        }

        #endregion

        #region Shipping Calculation Tests

        [TestMethod]
        public void CalculateShippingCost_ReturnsRoundedWeightAndVolumeBasedCost() {
            // Weight = 28.457 -> 28.457 * 0.5 = 14.2285
            // Volume = 90 * 180 * 75 * 0.0001 = 121.5
            // Sum = 14.2285 + 121.5 = 135.7285 -> RoundPrice -> 135.73
            double shippingCost = _furniture.CalculateShippingCost();

            Assert.AreEqual(135.73, shippingCost, 0.001);
        }

        #endregion

        private class TestableFurniture : Furniture {
            public TestableFurniture(string article, string name, double price, double weight, double width, double length, double height, string material)
                : base(article, name, price, weight, width, length, height, material) {
            }

            public void UpdateDimensions(double width, double length, double height) {
                Width = width;
                Length = length;
                Height = height;
            }
        }
    }
}
