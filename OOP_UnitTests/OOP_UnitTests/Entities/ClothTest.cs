using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Entities;
using System;

namespace OOP_UnitTests.Entities {
    [TestClass]
    public class ClothTests {
        private Cloth _cloth;

        [TestInitialize]
        public void Setup() {
            _cloth = new Cloth(
                article: "CL-0001",
                name: "Cotton T-Shirt",
                price: 49.99,
                material: "Organic Cotton",
                supplierId: 12
            );
        }

        [TestCleanup]
        public void Cleanup() {
            _cloth = null;
        }

        #region Constructor & Properties Tests

        [TestMethod]
        public void Constructor_ValidArguments_InitializesAllProperties() {
            Assert.IsInstanceOfType(_cloth, typeof(Product));
            Assert.AreEqual("CL-0001", _cloth.Article);
            Assert.AreEqual("Cotton T-Shirt", _cloth.Name);
            Assert.AreEqual(49.99, _cloth.Price, 0.001);
            Assert.AreEqual("Organic Cotton", _cloth.Material);
            Assert.AreEqual(12, _cloth.SupplierId);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void Material_ProtectedSetterWithInvalidValue_ThrowsArgumentException(string invalidMaterial) {
            Assert.Throws<ArgumentException>(() =>
                new Cloth("CL-0002", "Test", 10.0, invalidMaterial, 1));
        }

        #endregion

        #region Shipping Calculation Tests

        [TestMethod]
        public void CalculateShippingCost_ReturnsFivePercentOfPriceRounded() {
            // 49.99 * 0.05 = 2.4995 -> 2.50
            double shippingCost = _cloth.CalculateShippingCost();

            Assert.AreEqual(2.50, shippingCost, 0.001);
        }

        [TestMethod]
        public void CalculateShippingCost_ZeroPrice_ReturnsZero() {
            var freeCloth = new Cloth("CL-0003", "Promo Scarf", 0.0, "Polyester", 1);

            Assert.AreEqual(0.0, freeCloth.CalculateShippingCost(), 0.001);
        }

        #endregion
    }
}
