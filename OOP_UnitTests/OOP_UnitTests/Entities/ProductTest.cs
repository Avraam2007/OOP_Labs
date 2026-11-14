using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Entities;
using OOP_Main.Repositories;
using System;

namespace OOP_UnitTests.Entities {
    [TestClass]
    public class ProductTests {
        private Product _product;

        [TestInitialize]
        public void Setup() {
            _product = new TestableProduct(
                article: "  PR-0001  ",
                name: "  Sample Item  ",
                price: 199.995,
                supplierId: 15
            );
        }

        [TestCleanup]
        public void Cleanup() {
            _product = null;
        }

        #region Constructor, Trimming & Identifiable Tests

        [TestMethod]
        public void Constructor_ValidArguments_TrimsStringsRoundsPriceAndSetsProperties() {
            Assert.AreEqual("PR-0001", _product.Article);
            Assert.AreEqual("PR-0001", ((IIdentifiable<string>)_product).Id);
            Assert.AreEqual("Sample Item", _product.Name);
            Assert.AreEqual(200.00, _product.Price, 0.001);
            Assert.AreEqual(15, _product.SupplierId);
            Assert.AreEqual(15, _product.GetSupplierId());
            Assert.AreEqual("TestableProduct", _product.Category);
            Assert.AreEqual(3, _product.GetDeliveryDays());
        }

        [TestMethod]
        public void ParameterlessConstructor_InitializesDefaultEmptyState() {
            var emptyProduct = new TestableProduct();

            Assert.IsNull(emptyProduct.Article);
            Assert.IsNull(emptyProduct.Name);
            Assert.AreEqual(0, emptyProduct.Price);
            Assert.AreEqual(0, emptyProduct.GetHashCode());
        }

        #endregion

        #region Property Validation Tests

        [TestMethod]
        public void Article_NullValue_ThrowsNullReferenceException() {
            Assert.Throws<NullReferenceException>(() =>
                _product.Article = null);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public void Article_EmptyOrWhitespace_ThrowsArgumentNullException(string emptyArticle) {
            Assert.Throws<ArgumentNullException>(() =>
                _product.Article = emptyArticle);
        }

        [TestMethod]
        [DataRow("INVALID")]
        [DataRow("12-345")]
        [DataRow("PR_0001")]
        public void Article_InvalidFormat_ThrowsArgumentException(string invalidArticle) {
            Assert.Throws<ArgumentException>(() =>
                _product.Article = invalidArticle);
        }

        [TestMethod]
        public void Name_NullValue_ThrowsNullReferenceException() {
            Assert.Throws<NullReferenceException>(() =>
                _product.Name = null);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public void Name_EmptyOrWhitespace_ThrowsArgumentException(string emptyName) {
            Assert.Throws<ArgumentException>(() =>
                _product.Name = emptyName);
        }

        [TestMethod]
        public void Price_NegativeValue_ThrowsArgumentOutOfRangeException() {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                _product.Price = -0.01);
        }

        [TestMethod]
        public void Price_ZeroValue_SetsZeroPriceSuccessfully() {
            _product.Price = 0.0;

            Assert.AreEqual(0.0, _product.Price, 0.001);
        }

        #endregion

        #region Equality, Comparison & Formatting Tests

        [TestMethod]
        public void EqualsAndGetHashCode_CompareByArticle() {
            var sameArticle = new TestableProduct("PR-0001", "Different Name", 50.0, 99);
            var differentArticle = new TestableProduct("PR-0002", "Sample Item", 200.0, 15);

            Assert.IsTrue(_product.Equals(sameArticle));
            Assert.AreEqual(_product.GetHashCode(), sameArticle.GetHashCode());
            Assert.IsFalse(_product.Equals(differentArticle));
            Assert.IsFalse(_product.Equals(null));
        }

        [TestMethod]
        public void CompareTo_ComparesByArticleIgnoringCase() {
            var first = new TestableProduct("CL-0001", "First", 10.0, 1);
            var second = new TestableProduct("FN-0001", "Second", 20.0, 1);

            Assert.IsLessThan(0, first.CompareTo(second));
            Assert.IsGreaterThan(0, second.CompareTo(first));
            Assert.AreEqual(1, first.CompareTo(null));
        }

        [TestMethod]
        public void ToString_ReturnsFormattedProductString() {
            Assert.AreEqual("(PR-0001) Sample Item - 200$", _product.ToString());
        }

        #endregion

        private class TestableProduct : Product {
            public TestableProduct() : base() { }

            public TestableProduct(string article, string name, double price, int supplierId)
                : base(article, name, price, supplierId) {
            }

            public override double CalculateShippingCost() => 0;
        }
    }
}
