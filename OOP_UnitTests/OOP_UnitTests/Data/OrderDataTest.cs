using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main;
using OOP_Main.Entities;
using OOP_Main.Repositories;
using OOP_Main.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace OOP_UnitTests.Repositories {
    [TestClass]
    public class OrderDataTests {
        private OrderData _orderData;

        [TestInitialize]
        public void Setup() {
            _orderData = new OrderData();
        }

        [TestCleanup]
        public void Cleanup() {
            _orderData = null;
        }

        [TestMethod]
        public void AddOrder_WithCatalog_AutoIncrementsOrderIdFromMax() {
            var products = new List<Product> {
                new ElectronicProduct("EL-0001", "Laptop", 1200, 1500, 220, 1)
            };

            _orderData.AddOrder(buyerId: 5, products);
            _orderData.AddOrder(buyerId: 5, products);

            _orderData.DeleteOrder(1);

            _orderData.AddOrder(buyerId: 5, products);

            Assert.AreEqual(2, _orderData.Orders.Count);
            Assert.IsTrue(_orderData.GetOrderById(2).HasValue());
            Assert.IsTrue(_orderData.GetOrderById(3).HasValue());
            Assert.IsTrue(_orderData.GetOrderById(1).IsEmpty());
        }

        [TestMethod]
        public void AddOrder_DirectOrderObject_AddsToCollection() {
            var order = new Order(orderId: 42, buyerId: 7);

            _orderData.AddOrder(order);

            var found = _orderData.GetOrderById(42);
            Assert.IsTrue(found.HasValue());
            Assert.AreEqual(7, found.Value.BuyerId);
        }

        [TestMethod]
        public void DeleteOrder_NonExistentId_ReturnsFalse() {
            bool result = _orderData.DeleteOrder(999);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetOrdersFromUser_NoMatchingOrders_ReturnsEmptyList() {
            var products = new List<Product>();
            _orderData.AddOrder(buyerId: 1, products);

            var result = _orderData.GetOrdersFromUser(userId: 99);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void OrderQueue_SetAndGet_RetainsQueueInstance() {
            var queue = new OrderProcessingQueue();

            _orderData.OrderQueue = queue;

            Assert.AreSame(queue, _orderData.OrderQueue);
        }
    }
}
