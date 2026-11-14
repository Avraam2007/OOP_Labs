using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main;
using OOP_Main.Comparers;
using OOP_Main.Entities;

namespace OOP_UnitTests.Comparers {
    [TestClass]
    public class OrderComparersTests {
        #region OrderPriceComparer Tests

        [TestMethod]
        public void OrderPriceComparer_ComparesTotalPricesCorrectly() {
            var comparer = new OrderPriceComparer();
            var cheapOrder = new Order(1, 1);
            cheapOrder.Products.Add(new ElectronicProduct("EL-0001", "Mouse", 50, 10, 5, 1));

            var expensiveOrder = new Order(2, 1);
            expensiveOrder.Products.Add(new ElectronicProduct("EL-0002", "Laptop", 1000, 65, 220, 1));

            var equalOrder = new Order(3, 1);
            equalOrder.Products.Add(new ElectronicProduct("EL-0003", "Keyboard", 50, 10, 5, 1));

            Assert.IsLessThan(0, comparer.Compare(cheapOrder, expensiveOrder));
            Assert.IsGreaterThan(0, comparer.Compare(expensiveOrder, cheapOrder));
            Assert.AreEqual(0, comparer.Compare(cheapOrder, equalOrder));
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

        #region OrderIDComparer Tests

        [TestMethod]
        public void OrderIDComparer_ComparesOrderIdsCorrectly() {
            var comparer = new OrderIDComparer();
            var order1 = new Order(10, 1);
            var order2 = new Order(25, 1);
            var order1Duplicate = new Order(10, 2);

            Assert.IsLessThan(0, comparer.Compare(order1, order2));
            Assert.IsGreaterThan(0, comparer.Compare(order2, order1));
            Assert.AreEqual(0, comparer.Compare(order1, order1Duplicate));
        }

        [TestMethod]
        public void OrderIDComparer_HandlesNullValues() {
            var comparer = new OrderIDComparer();
            var order = new Order(1, 1);

            Assert.AreEqual(0, comparer.Compare(null, null));
            Assert.AreEqual(-1, comparer.Compare(null, order));
            Assert.AreEqual(1, comparer.Compare(order, null));
        }

        #endregion
    }
}
