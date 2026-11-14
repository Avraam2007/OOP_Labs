using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main;
using OOP_Main.Entities;
using OOP_Main.Repositories;
using OOP_Main.States;
using System;
using System.Collections.Generic;

namespace OOP_UnitTests.Entities {
    [TestClass]
    public class OrderTests {
        private Order _order;
        private Product _p1;
        private Product _p2;
        private Product _p3;

        [TestInitialize]
        public void Setup() {
            _p1 = new ElectronicProduct("EL-0001", "Laptop", 1000.0, 65, 220, 1);
            _p2 = new Cloth("CL-0001", "Shirt", 50.0, "Cotton", 2);
            _p3 = new Furniture("FN-0001", "Chair", 125.55, 8, 50, 50, 90, "Wood", 3);

            _order = new Order(orderId: 101, buyerId: 5);
            _order.AddProduct(_p1);
            _order.AddProduct(_p2);
            _order.AddProduct(_p3);
        }

        [TestCleanup]
        public void Cleanup() {
            _order = null;
            _p1 = null;
            _p2 = null;
            _p3 = null;
        }

        #region Constructors & Identifiable Tests

        [TestMethod]
        public void ParameterlessConstructor_InitializesEmptyProductsAndPendingStatus() {
            var emptyOrder = new Order();

            Assert.IsNotNull(emptyOrder.Products);
            Assert.IsEmpty(emptyOrder.Products);
            Assert.AreEqual(OrderStatus.Pending, emptyOrder.Status);
        }

        [TestMethod]
        public void ConstructorWithOrderIdAndBuyerId_SetsIdsAndInitializesPendingState() {
            var order = new Order(42, 7);

            Assert.AreEqual(42, order.OrderId);
            Assert.AreEqual(42, ((IIdentifiable<int>)order).Id);
            Assert.AreEqual(7, order.BuyerId);
            Assert.IsNotNull(order.Products);
            Assert.AreEqual(OrderStatus.Pending, order.Status);
        }

        [TestMethod]
        public void JsonConstructor_SetsAllPropertiesAndInitializesPendingState() {
            var date = DateTime.Now;
            var list = new List<Product> { _p1 };

            var order = new Order(15, 3, list, date);

            Assert.AreEqual(15, order.OrderId);
            Assert.AreEqual(3, order.BuyerId);
            Assert.AreSame(list, order.Products);
            Assert.AreEqual(date, order.Date);
            Assert.AreEqual(OrderStatus.Pending, order.Status);
        }

        [TestMethod]
        public void ConstructorWithStatus_SetsProvidedStatus() {
            var list = new List<Product> { _p1 };

            var order = new Order(20, 4, list, OrderStatus.Shipped);

            Assert.AreEqual(20, order.OrderId);
            Assert.AreEqual(4, order.BuyerId);
            Assert.AreSame(list, order.Products);
            Assert.AreEqual(OrderStatus.Shipped, order.Status);
        }

        #endregion

        #region State Pattern Transitions Tests

        [TestMethod]
        public void StateTransitions_PayAndShip_UpdatesOrderStatusAndDescription() {
            Assert.AreEqual("Pending", _order.GetStatusDescription());

            _order.Pay();
            Assert.AreEqual(OrderStatus.Paid, _order.Status);
            Assert.AreEqual("Paid, ready to ship", _order.GetStatusDescription());

            _order.Ship();
            Assert.AreEqual(OrderStatus.Shipped, _order.Status);
            Assert.AreEqual("Shipping", _order.GetStatusDescription());
        }

        [TestMethod]
        public void StateTransitions_CancelPendingOrder_SetsStatusToCancelled() {
            _order.Cancel();

            Assert.AreEqual(OrderStatus.Cancelled, _order.Status);
            Assert.AreEqual("Cancelled", _order.GetStatusDescription());
        }

        #endregion

        #region Clone & DeepCopy Tests

        [TestMethod]
        public void Clone_CreatesShallowCopySharingProductsListReference() {
            var shallowClone = (Order)_order.Clone();

            Assert.AreNotSame(_order, shallowClone);
            Assert.AreEqual(_order.OrderId, shallowClone.OrderId);
            Assert.AreSame(_order.Products, shallowClone.Products);
        }

        [TestMethod]
        public void DeepCopy_CreatesCopyWithIndependentProductsList() {
            Order deepCopy = _order.DeepCopy();

            Assert.AreNotSame(_order, deepCopy);
            Assert.AreEqual(_order.OrderId, deepCopy.OrderId);
            Assert.AreNotSame(_order.Products, deepCopy.Products);
            Assert.AreSequenceEqual(_order.Products, deepCopy.Products);

            deepCopy.AddProduct(new Cloth("CL-9999", "Hat", 15, "Wool", 1));
            Assert.HasCount(3, _order.Products);
            Assert.HasCount(4, deepCopy.Products);
        }

        #endregion

        #region Price & Statistics Calculations Tests

        [TestMethod]
        public void PriceCalculations_EmptyOrNullProducts_ReturnsZeros() {
            var emptyOrder = new Order();

            Assert.AreEqual(0, emptyOrder.GetTotalPrice());
            Assert.AreEqual(0, emptyOrder.GetTotalShippingCost());
            Assert.AreEqual(0, emptyOrder.GetAveragePrice());
            Assert.AreEqual(0, emptyOrder.GetMinPrice());
            Assert.AreEqual(0, emptyOrder.GetMaxPrice());
            Assert.AreSequenceEqual(new double[] { 0, 0 }, emptyOrder.FindMinMaxPrices(null));
        }

        [TestMethod]
        public void GetTotalPrice_WithMultipleProducts_ReturnsExactSum() {
            // 1000.0 + 50.0 + 125.55 = 1175.55
            Assert.AreEqual(1175.55, _order.GetTotalPrice(), 0.001);
        }

        [TestMethod]
        public void GetTotalShippingCost_WithMultipleProducts_ReturnsSumOfShippingCosts() {
            double expected = _p1.CalculateShippingCost() + _p2.CalculateShippingCost() + _p3.CalculateShippingCost();

            Assert.AreEqual(expected, _order.GetTotalShippingCost(), 0.001);
        }

        [TestMethod]
        public void GetAveragePrice_WithMultipleProducts_ReturnsRoundedAverage() {
            // 1175.55 / 3 = 391.85
            Assert.AreEqual(391.85, _order.GetAveragePrice(), 0.001);
        }

        [TestMethod]
        public void GetMinAndMaxPrice_WithMultipleProducts_ReturnsLowestAndHighestPrices() {
            Assert.AreEqual(50.0, _order.GetMinPrice(), 0.001);
            Assert.AreEqual(1000.0, _order.GetMaxPrice(), 0.001);
        }

        [TestMethod]
        public void FindMinMaxPrices_ValidList_ReturnsMinAtZeroAndMaxAtOne() {
            double[] minMax = _order.FindMinMaxPrices(_order.Products);

            Assert.HasCount(2, minMax);
            Assert.AreEqual(50.0, minMax[0], 0.001);
            Assert.AreEqual(1000.0, minMax[1], 0.001);
        }

        #endregion

        #region Generic List Helpers (Reverse & RemoveDuplicates) Tests

        [TestMethod]
        public void Reverse_NullOrEmptyList_ReturnsNull() {
            Assert.IsNull(_order.Reverse<int>(null));
            Assert.IsNull(_order.Reverse(new List<int>()));
        }

        [TestMethod]
        public void Reverse_ValidList_ReversesElementsInPlace() {
            var list = new List<int> { 1, 2, 3, 4, 5 };

            var reversed = _order.Reverse(list);

            Assert.AreSequenceEqual(new List<int> { 5, 4, 3, 2, 1 }, reversed);
        }

        [TestMethod]
        public void RemoveDuplicates_NullOrEmptyList_ReturnsNull() {
            Assert.IsNull(_order.RemoveDuplicates<string>(null));
            Assert.IsNull(_order.RemoveDuplicates(new List<string>()));
        }

        [TestMethod]
        public void RemoveDuplicates_ListWithDuplicates_ReturnsOnlyUniqueElements() {
            var list = new List<int> { 1, 2, 2, 3, 4, 3, 5 };

            var uniqueOnly = _order.RemoveDuplicates(list);

            Assert.AreSequenceEqual(new List<int> { 1, 4, 5 }, uniqueOnly);
        }

        #endregion

        #region Equality & Formatting Tests

        [TestMethod]
        public void EqualsAndGetHashCode_CompareByOrderId() {
            var sameIdOrder = new Order(101, 999);
            var differentIdOrder = new Order(202, 5);

            Assert.IsTrue(_order.Equals(sameIdOrder));
            Assert.AreEqual(_order.GetHashCode(), sameIdOrder.GetHashCode());
            Assert.IsFalse(_order.Equals(differentIdOrder));
            Assert.IsFalse(_order.Equals(null));
        }

        [TestMethod]
        public void ToString_ReturnsFormattedOrderString() {
            Assert.AreEqual("|Order| #101 (From: 5)", _order.ToString());
        }

        #endregion
    }
}
