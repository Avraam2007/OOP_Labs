using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.States;
using System.Linq;

namespace OOP_UnitTests.States {
    [TestClass]
    public class OrderStatusTests {
        #region GetDescription Tests

        [TestMethod]
        [DataRow(OrderStatus.Pending, "Pending")]
        [DataRow(OrderStatus.Paid, "Paid, ready to ship")]
        [DataRow(OrderStatus.Shipped, "Shipping")]
        [DataRow(OrderStatus.Delivered, "Delievered successfully")]
        [DataRow(OrderStatus.Cancelled, "Cancelled")]
        [DataRow((OrderStatus)999, "Unknown")]
        public void GetDescription_VariousStatuses_ReturnsExpectedString(OrderStatus status, string expectedDescription) {
            string actualDescription = OrderStatusExtensions.GetDescription(status);

            Assert.AreEqual(expectedDescription, actualDescription);
        }

        #endregion

        #region GetNamesArray & GetOrderStatuses Tests

        [TestMethod]
        public void GetNamesArray_ReturnsAllEnumNamesInOrder() {
            string[] expectedNames = {
                "Pending",
                "Paid",
                "Shipped",
                "Delivered",
                "Cancelled"
            };

            string[] actualNames = OrderStatusExtensions.GetNamesArray();

            Assert.AreSequenceEqual(expectedNames, actualNames);
        }

        [TestMethod]
        public void GetOrderStatuses_ReturnsAllEnumValuesInOrder() {
            OrderStatus[] expectedStatuses = {
                OrderStatus.Pending,
                OrderStatus.Paid,
                OrderStatus.Shipped,
                OrderStatus.Delivered,
                OrderStatus.Cancelled
            };

            var actualStatuses = OrderStatusExtensions.GetOrderStatuses().ToArray();

            Assert.AreSequenceEqual(expectedStatuses, actualStatuses);
        }

        #endregion

        #region CanBeCancelled Tests

        [TestMethod]
        [DataRow(OrderStatus.Pending, true)]
        [DataRow(OrderStatus.Paid, true)]
        [DataRow(OrderStatus.Shipped, false)]
        [DataRow(OrderStatus.Delivered, false)]
        [DataRow(OrderStatus.Cancelled, false)]
        public void CanBeCancelled_VariousStatuses_ReturnsExpectedResult(OrderStatus status, bool expectedResult) {
            bool actualResult = OrderStatusExtensions.CanBeCancelled(status);

            Assert.AreEqual(expectedResult, actualResult);
        }

        #endregion
    }
}
