using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Entities;

namespace OOP_UnitTests.Entities {
    [TestClass]
    public class ElectronicProductTests {
        private ElectronicProduct _electronic;

        [TestInitialize]
        public void Setup() {
            _electronic = new ElectronicProduct(
                article: "EL-0001",
                name: "Gaming Laptop",
                price: 1500.0,
                power: 180,
                maxVoltage: 220,
                supplierId: 4
            );
        }

        [TestCleanup]
        public void Cleanup() {
            _electronic = null;
        }

        #region Constructor & Properties Tests

        [TestMethod]
        public void Constructor_ValidArguments_InitializesAllProperties() {
            Assert.IsInstanceOfType(_electronic, typeof(Product));
            Assert.AreEqual("EL-0001", _electronic.Article);
            Assert.AreEqual("Gaming Laptop", _electronic.Name);
            Assert.AreEqual(1500.0, _electronic.Price, 0.001);
            Assert.AreEqual(180, _electronic.Power);
            Assert.AreEqual(220, _electronic.MaxVoltage);
            Assert.AreEqual(4, _electronic.SupplierId);
        }

        #endregion

        #region Shipping Calculation Tests

        [TestMethod]
        public void CalculateShippingCost_StandardPower_ReturnsExpectedRoundedCost() {
            // 0.1 + (180 * 0.005) = 0.1 + 0.9 = 1.0
            double shippingCost = _electronic.CalculateShippingCost();

            Assert.AreEqual(1.0, shippingCost, 0.001);
        }

        [TestMethod]
        public void CalculateShippingCost_FractionalPowerResult_RoundsToTwoDecimalPlaces() {
            var router = new ElectronicProduct("EL-0002", "Wi-Fi Router", 80.0, power: 15, maxVoltage: 12, supplierId: 2);

            // 0.1 + (15 * 0.005) = 0.1 + 0.075 = 0.175 -> 0.18
            double shippingCost = router.CalculateShippingCost();

            Assert.AreEqual(0.18, shippingCost, 0.001);
        }

        [TestMethod]
        public void CalculateShippingCost_ZeroPower_ReturnsBaseCost() {
            var passiveAccessory = new ElectronicProduct("EL-0003", "Passive Cable", 15.0, power: 0, maxVoltage: 5, supplierId: 2);

            // 0.1 + (0 * 0.005) = 0.1
            double shippingCost = passiveAccessory.CalculateShippingCost();

            Assert.AreEqual(0.1, shippingCost, 0.001);
        }

        #endregion
    }
}
