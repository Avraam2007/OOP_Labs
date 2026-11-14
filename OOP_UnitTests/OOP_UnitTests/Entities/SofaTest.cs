using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Entities;

namespace OOP_UnitTests.Entities {
    [TestClass]
    public class SofaTests {
        private Sofa _sofa;

        [TestInitialize]
        public void Setup() {
            _sofa = new Sofa(
                article: "SF-0001",
                name: "Corner Velvet Sofa",
                price: 950.0,
                weight: 55.0,
                width: 110.0,
                length: 230.0,
                height: 85.0,
                material: "Velvet",
                isAssemble: true,
                supplierId: 9
            );
        }

        [TestCleanup]
        public void Cleanup() {
            _sofa = null;
        }

        #region Constructor & Inheritance Tests

        [TestMethod]
        public void Constructor_ValidArguments_InitializesSofaAndInheritedProperties() {
            Assert.IsInstanceOfType(_sofa, typeof(Furniture));
            Assert.IsInstanceOfType(_sofa, typeof(Product));
            Assert.AreEqual("SF-0001", _sofa.Article);
            Assert.AreEqual("Corner Velvet Sofa", _sofa.Name);
            Assert.AreEqual(950.0, _sofa.Price, 0.001);
            Assert.AreEqual(55.0, _sofa.Weight, 0.001);
            Assert.AreEqual(110.0, _sofa.Width, 0.001);
            Assert.AreEqual(230.0, _sofa.Length, 0.001);
            Assert.AreEqual(85.0, _sofa.Height, 0.001);
            Assert.AreEqual("Velvet", _sofa.Material);
            Assert.IsTrue(_sofa.IsAssemble);
            Assert.AreEqual(9, _sofa.SupplierId);
        }

        [TestMethod]
        public void Constructor_NotAssembledAndDefaultSupplierId_SetsExpectedValues() {
            var unassembledSofa = new Sofa(
                article: "SF-0002",
                name: "Modular Sofa",
                price: 600.0,
                weight: 40.0,
                width: 90.0,
                length: 190.0,
                height: 80.0,
                material: "Linen",
                isAssemble: false
            );

            Assert.IsFalse(unassembledSofa.IsAssemble);
            Assert.AreEqual(0, unassembledSofa.SupplierId);
        }

        #endregion

        #region Shipping Calculation Tests

        [TestMethod]
        public void CalculateShippingCost_DelegatesToFurnitureFormula() {
            // Weight: 55 * 0.5 = 27.5
            // Volume: 110 * 230 * 85 * 0.0001 = 215.05
            // Total: 27.5 + 215.05 = 242.55
            double shippingCost = _sofa.CalculateShippingCost();

            Assert.AreEqual(242.55, shippingCost, 0.001);
        }

        #endregion
    }
}
