using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Bundles;
using OOP_Main.Entities;

namespace OOP_UnitTests.Bundles {
    [TestClass]
    public class ProductBundleTests {
        private ProductBundle _bundle;

        [TestInitialize]
        public void Setup() {
            _bundle = new ProductBundle("BDL-0001", "Home Office Set", supplierId: 10, discountPercent: 15);
        }

        [TestCleanup]
        public void Cleanup() {
            _bundle = null;
        }

        #region Constructor & Properties Tests

        [TestMethod]
        public void Constructor_WithDefaultDiscount_SetsPropertiesCorrectly() {
            var defaultBundle = new ProductBundle("BDL-0002", "Starter Pack", 5);

            Assert.AreEqual("BDL-0002", defaultBundle.Article);
            Assert.AreEqual("Starter Pack", defaultBundle.Name);
            Assert.AreEqual(5, defaultBundle.SupplierId);
            Assert.AreEqual(10, defaultBundle.BundleDiscountPercent, 0.001);
            Assert.IsNotNull(defaultBundle.Items);
            Assert.IsEmpty(defaultBundle.Items);
        }

        [TestMethod]
        public void ParameterlessConstructor_InitializesEmptyItemsList() {
            var emptyBundle = new ProductBundle();

            Assert.IsNotNull(emptyBundle.Items);
            Assert.IsEmpty(emptyBundle.Items);
            Assert.AreEqual(0, emptyBundle.BundleDiscountPercent, 0.001);
        }

        [TestMethod]
        public void BundleDiscountPercent_SetNewValue_UpdatesDiscount() {
            _bundle.BundleDiscountPercent = 25.5;

            Assert.AreEqual(25.5, _bundle.BundleDiscountPercent, 0.001);
        }

        #endregion

        #region AddProduct & RemoveProduct Tests

        [TestMethod]
        public void AddProduct_ValidProduct_AddsProductToItems() {
            var laptop = new ElectronicProduct("EL-0001", "Laptop", 1000, 65, 220, 1);

            _bundle.AddProduct(laptop);

            Assert.HasCount(1, _bundle.Items);
            Assert.AreSame(laptop, _bundle.Items[0]);
        }

        [TestMethod]
        public void AddProduct_NullProduct_DoesNotAddToItems() {
            _bundle.AddProduct(null);

            Assert.IsEmpty(_bundle.Items);
        }

        [TestMethod]
        public void RemoveProduct_ExistingProduct_RemovesProductFromItems() {
            var laptop = new ElectronicProduct("EL-0001", "Laptop", 1000, 65, 220, 1);
            var mouse = new ElectronicProduct("EL-0002", "Mouse", 50, 5, 5, 1);
            _bundle.AddProduct(laptop);
            _bundle.AddProduct(mouse);

            _bundle.RemoveProduct(laptop);

            Assert.HasCount(1, _bundle.Items);
            Assert.AreSame(mouse, _bundle.Items[0]);
        }

        #endregion

        #region Price & Shipping Calculation Tests

        [TestMethod]
        public void Price_EmptyBundle_ReturnsZero() {
            Assert.AreEqual(0, _bundle.Price);
        }

        [TestMethod]
        public void Price_WithMultipleProducts_CalculatesDiscountedAndRoundedSum() {
            _bundle.BundleDiscountPercent = 15;
            _bundle.AddProduct(new ElectronicProduct("EL-0001", "Laptop", 1000, 65, 220, 1));
            _bundle.AddProduct(new Cloth("CL-0001", "Desk Mat", 33.33, "Felt", 2));

            // Total = 1033.33; 15% discount -> 1033.33 * 0.85 = 878.3305 -> 878.33
            Assert.AreEqual(878.33, _bundle.Price, 0.001);
        }

        [TestMethod]
        public void CalculateShippingCost_EmptyBundle_ReturnsZero() {
            Assert.AreEqual(0, _bundle.CalculateShippingCost());
        }

        [TestMethod]
        public void CalculateShippingCost_WithMultipleProducts_ReturnsSumOfItemsShippingCosts() {
            var item1 = new ElectronicProduct("EL-0001", "Laptop", 1000, 65, 220, 1);
            var item2 = new Cloth("CL-0001", "Shirt", 50, "Cotton", 2);
            _bundle.AddProduct(item1);
            _bundle.AddProduct(item2);

            double expectedShipping = item1.CalculateShippingCost() + item2.CalculateShippingCost();

            Assert.AreEqual(expectedShipping, _bundle.CalculateShippingCost(), 0.001);
        }

        #endregion

        #region ToString Tests

        [TestMethod]
        public void ToString_ReturnsFormattedBundleString() {
            _bundle.BundleDiscountPercent = 10;
            _bundle.AddProduct(new ElectronicProduct("EL-0001", "Monitor", 200, 40, 220, 1));
            _bundle.AddProduct(new ElectronicProduct("EL-0002", "Keyboard", 100, 5, 5, 1));

            // Total = 300; 10% discount -> 270
            string expected = "(BDL-0001) Home Office Set |2 items| - 270$";

            Assert.AreEqual(expected, _bundle.ToString());
        }

        #endregion
    }
}
