using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Comparers;
using OOP_Main.Entities;
using System.Collections.Generic;

namespace OOP_UnitTests.Comparers {
    [TestClass]
    public class ProductComparersTest {
        private ProductPriceComparer _priceComparer;
        private ProductNameComparer _nameComparer;

        #region ProductPriceComparer Tests

        [TestMethod]
        public void ProductPriceComparer_BothNull_ReturnsZero() {
            _priceComparer = new ProductPriceComparer();
            int result = _priceComparer.Compare(null, null);

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void ProductPriceComparer_FirstNull_ReturnsNegative() {
            _priceComparer = new ProductPriceComparer();
            var product = new Cloth("CL-0001", "Shirt", 50, "Cotton", 1);

            int result = _priceComparer.Compare(null, product);

            Assert.IsLessThan(0, result);
        }

        [TestMethod]
        public void ProductPriceComparer_SecondNull_ReturnsPositive() {
            _priceComparer = new ProductPriceComparer();
            var product = new Cloth("CL-0001", "Shirt", 50, "Cotton", 1);

            int result = _priceComparer.Compare(product, null);

            Assert.IsGreaterThan(0, result);
        }

        [TestMethod]
        public void ProductPriceComparer_FirstCheaperThanSecond_ReturnsNegative() {
            _priceComparer = new ProductPriceComparer();
            var cheapProduct = new Cloth("CL-0001", "Socks", 10, "Cotton", 1);
            var expensiveProduct = new ElectronicProduct("EL-0001", "Laptop", 1000, 65, 220, 1);

            int result = _priceComparer.Compare(cheapProduct, expensiveProduct);

            Assert.IsLessThan(0, result);
        }

        [TestMethod]
        public void ProductPriceComparer_FirstMoreExpensiveThanSecond_ReturnsPositive() {
            _priceComparer = new ProductPriceComparer();
            var expensiveProduct = new ElectronicProduct("EL-0001", "Laptop", 1000, 65, 220, 1);
            var cheapProduct = new Cloth("CL-0001", "Socks", 10, "Cotton", 1);

            int result = _priceComparer.Compare(expensiveProduct, cheapProduct);

            Assert.IsGreaterThan(0, result);
        }

        [TestMethod]
        public void ProductPriceComparer_EqualPrices_ReturnsZero() {
            _priceComparer = new ProductPriceComparer();
            var first = new Cloth("CL-0001", "Shirt", 50, "Cotton", 1);
            var second = new ElectronicProduct("EL-0001", "Mouse", 50, 5, 5, 2);

            int result = _priceComparer.Compare(first, second);

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void ProductPriceComparer_SortList_OrdersProductsByPriceAscending() {
            _priceComparer = new ProductPriceComparer();
            var p1 = new Cloth("CL-0001", "Jacket", 150, "Wool", 1);
            var p2 = new Cloth("CL-0002", "Socks", 15, "Cotton", 1);
            var p3 = new ElectronicProduct("EL-0001", "Monitor", 300, 40, 220, 1);
            var products = new List<Product> { p1, p2, p3 };

            products.Sort(_priceComparer);

            Assert.AreSequenceEqual(new List<Product> { p2, p1, p3 }, products);
        }

        #endregion

        #region ProductNameComparer Tests

        [TestMethod]
        public void ProductNameComparer_BothNull_ReturnsZero() {
            _nameComparer = new ProductNameComparer();
            int result = _nameComparer.Compare(null, null);

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void ProductNameComparer_FirstNull_ReturnsNegative() {
            _nameComparer = new ProductNameComparer();
            var product = new Cloth("CL-0001", "Shirt", 50, "Cotton", 1);

            int result = _nameComparer.Compare(null, product);

            Assert.IsLessThan(0, result);
        }

        [TestMethod]
        public void ProductNameComparer_SecondNull_ReturnsPositive() {
            _nameComparer = new ProductNameComparer();
            var product = new Cloth("CL-0001", "Shirt", 50, "Cotton", 1);

            int result = _nameComparer.Compare(product, null);

            Assert.IsGreaterThan(0, result);
        }

        [TestMethod]
        public void ProductNameComparer_FirstAlphabeticallyBeforeSecond_ReturnsNegative() {
            _nameComparer = new ProductNameComparer();
            var first = new Cloth("CL-0001", "Apron", 20, "Linen", 1);
            var second = new Cloth("CL-0002", "Belt", 30, "Leather", 1);

            int result = _nameComparer.Compare(first, second);

            Assert.IsLessThan(0, result);
        }

        [TestMethod]
        public void ProductNameComparer_FirstAlphabeticallyAfterSecond_ReturnsPositive() {
            _nameComparer = new ProductNameComparer();
            var first = new Cloth("CL-0001", "Jacket", 120, "Wool", 1);
            var second = new Cloth("CL-0002", "Hat", 25, "Cotton", 1);

            int result = _nameComparer.Compare(first, second);

            Assert.IsGreaterThan(0, result);
        }

        [TestMethod]
        public void ProductNameComparer_SameNamesDifferentCase_ReturnsZero() {
            _nameComparer = new ProductNameComparer();
            var first = new Cloth("CL-0001", "laptop stand", 40, "Plastic", 1);
            var second = new Furniture("FN-0001", "LAPTOP STAND", 80, 3, 30, 25, 15, "Metal", 2);

            int result = _nameComparer.Compare(first, second);

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void ProductNameComparer_SortList_OrdersProductsAlphabeticallyIgnoringCase() {
            _nameComparer = new ProductNameComparer();
            var p1 = new Cloth("CL-0001", "zebra Print Scarf", 30, "Silk", 1);
            var p2 = new ElectronicProduct("EL-0001", "Adapter", 20, 15, 220, 1);
            var p3 = new Furniture("FN-0001", "bookshelf", 150, 20, 80, 30, 180, "Wood", 1);
            var products = new List<Product> { p1, p2, p3 };

            products.Sort(_nameComparer);

            Assert.AreSequenceEqual(new List<Product> { p2, p3, p1 }, products);
        }

        #endregion
    }
}
