using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main;
using OOP_Main.Comparers;
using OOP_Main.Entities;

namespace OOP_UnitTests.Comparers {
    [TestClass]
    public class SupplierComparersTest {
        #region SupplierRatingComparer Tests

        [TestMethod]
        public void SupplierRatingComparer_ComparesTotalRatingsCorrectly() {
            var comparer = new SupplierRatingComparer();
            var worseSupplier = new Supplier(1, "Supplier 1", "a@b.com", "0589634562", 2.3);

            var betterSupplier = new Supplier(2, "Supplier 2", "test@probe.com", "0755894562", 4.1);

            var equalSupplier = new Supplier(3, "Supplier 3", "alpha@beta.com", "0486512589", 2.3);

            Assert.IsLessThan(0, comparer.Compare(worseSupplier, betterSupplier));
            Assert.IsGreaterThan(0, comparer.Compare(betterSupplier, worseSupplier));
            Assert.AreEqual(0, comparer.Compare(worseSupplier, equalSupplier));
        }

        [TestMethod]
        public void OrderPriceComparer_HandlesNullValues() {
            var comparer = new OrderPriceComparer();
            var order = new Order(1, 1);

            Assert.AreEqual(0, comparer.Compare(null, null));
            Assert.AreEqual(-1, comparer.Compare(null, order));
            Assert.AreEqual(1, comparer.Compare(order, null));
        }

        #endregion

        #region SupplierNameComparer Tests

        [TestMethod]
        public void SupplierNameComparer_ComparesNamesIgnoringCase() {
            var comparer = new SupplierNameComparer();
            var suppAlpha = new Supplier(1, "alpha Corp", "a@a.com", "0634567891", 4.0);
            var suppBeta = new Supplier(2, "Beta Corp", "b@b.com", "0235896314", 4.0);
            var suppAlphaUpper = new Supplier(3, "ALPHA CORP", "c@c.com", "0147895623", 4.0);

            Assert.IsLessThan(0, comparer.Compare(suppAlpha, suppBeta));
            Assert.IsGreaterThan(0, comparer.Compare(suppBeta, suppAlpha));
            Assert.AreEqual(0, comparer.Compare(suppAlpha, suppAlphaUpper));
        }

        [TestMethod]
        public void SupplierNameComparer_HandlesNullValues() {
            var comparer = new SupplierNameComparer();
            var supplier = new Supplier(1, "Supp", "a@a.com", "0147895623", 4.0);

            Assert.AreEqual(0, comparer.Compare(null, null));
            Assert.AreEqual(-1, comparer.Compare(null, supplier));
            Assert.AreEqual(1, comparer.Compare(supplier, null));
        }

        #endregion

        #region SupplierIDComparer Tests

        [TestMethod]
        public void SupplierIDComparer_ComparesSupplierIdsCorrectly() {
            var comparer = new SupplierIDComparer();
            var supplier1 = new Supplier(1, "Supplier 1", "a@b.com", "0589634562", 3.2);
            var supplier2 = new Supplier(2, "Supplier 2", "test@probe.com", "0755894562", 4.1);

            var supplier1Duplicate = new Supplier(1, "Supplier 3", "alpha@beta.com", "0486512589", 2.3);

            Assert.IsLessThan(0, comparer.Compare(supplier1, supplier2));
            Assert.IsGreaterThan(0, comparer.Compare(supplier2, supplier1));
            Assert.AreEqual(0, comparer.Compare(supplier1, supplier1Duplicate));
        }

        [TestMethod]
        public void SupplierIDComparer_HandlesNullValues() {
            var comparer = new SupplierIDComparer();
            var supplier = new Supplier(1, "Supplier 1", "a@b.com", "0589634562", 3.2);

            Assert.AreEqual(0, comparer.Compare(null, null));
            Assert.AreEqual(-1, comparer.Compare(null, supplier));
            Assert.AreEqual(1, comparer.Compare(supplier, null));
        }

        #endregion
    }
}
