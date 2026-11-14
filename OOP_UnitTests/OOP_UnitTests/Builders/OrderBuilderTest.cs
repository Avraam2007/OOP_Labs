using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main;
using OOP_Main.Builders;
using OOP_Main.Entities;
using OOP_Main.States;
using System;
using System.Collections.Generic;

namespace OOP_UnitTests.Builders {
    [TestClass]
    public class OrderBuilderAndDirectorTests {
        private OrderBuilder _builder;
        private Director _director;

        [TestInitialize]
        public void Setup() {
            _builder = new OrderBuilder();
            _director = new Director {
                Builder = _builder
            };
        }

        [TestCleanup]
        public void Cleanup() {
            _builder = null;
            _director = null;
        }

        #region OrderBuilder Direct Tests

        [TestMethod]
        public void OrderBuilder_SetAllPropertiesAndGetOrder_ReturnsConfiguredOrderAndResets() {
            var products = new List<Product> {
                new ElectronicProduct("EL-0001", "Laptop", 1500, 65, 220, 1)
            };
            var expectedDate = new DateTime(2026, 11, 14, 12, 0, 0);

            _builder.SetOrderId(101);
            _builder.SetCustomer(55);
            _builder.AddCatalog(products);
            _builder.AddDate(expectedDate);
            _builder.SetStatus(OrderStatus.Paid);

            Order firstOrder = _builder.GetOrder();

            Assert.IsNotNull(firstOrder);
            Assert.AreEqual(101, firstOrder.OrderId);
            Assert.AreEqual(55, firstOrder.BuyerId);
            Assert.AreSame(products, firstOrder.Products);
            Assert.AreEqual(expectedDate, firstOrder.Date);
            Assert.AreEqual(OrderStatus.Paid, firstOrder.Status);

            Order secondOrder = _builder.GetOrder();
            Assert.AreNotSame(firstOrder, secondOrder);
            Assert.AreEqual(0, secondOrder.OrderId);
            Assert.AreEqual(0, secondOrder.BuyerId);
        }

        [TestMethod]
        public void OrderBuilder_Reset_ClearsPreviouslySetFields() {
            _builder.SetOrderId(99);
            _builder.SetCustomer(77);
            _builder.SetStatus(OrderStatus.Shipped);

            _builder.Reset();
            Order order = _builder.GetOrder();

            Assert.AreEqual(0, order.OrderId);
            Assert.AreEqual(0, order.BuyerId);
            Assert.AreEqual(default(OrderStatus), order.Status);
        }

        #endregion

        #region Director Tests

        [TestMethod]
        public void Director_BuildValidationOrder_SetsCatalogAndCurrentDate() {
            var products = new List<Product> {
                new Cloth("CL-0001", "Shirt", 45, "Cotton", 2)
            };
            DateTime beforeBuild = DateTime.Now;

            _director.BuildValidationOrder(products);
            Order order = _builder.GetOrder();

            DateTime afterBuild = DateTime.Now;

            Assert.AreSame(products, order.Products);
            Assert.IsTrue(order.Date >= beforeBuild && order.Date <= afterBuild);
            Assert.AreEqual(0, order.OrderId);
            Assert.AreEqual(0, order.BuyerId);
        }

        [TestMethod]
        public void Director_BuildEmptyOrder_SetsOrderIdCustomerAndDate() {
            DateTime beforeBuild = DateTime.Now;

            _director.BuildEmptyOrder(orderId: 12, buyerId: 34);
            Order order = _builder.GetOrder();

            DateTime afterBuild = DateTime.Now;

            Assert.AreEqual(12, order.OrderId);
            Assert.AreEqual(34, order.BuyerId);
            Assert.IsTrue(order.Date >= beforeBuild && order.Date <= afterBuild);
        }

        [TestMethod]
        public void Director_BuildOrderWithProducts_SetsOrderIdCustomerDateAndCatalog() {
            var products = new List<Product> {
                new ElectronicProduct("EL-0002", "Mouse", 30, 5, 5, 1)
            };
            DateTime beforeBuild = DateTime.Now;

            _director.BuildOrderWithProducts(orderId: 15, buyerId: 42, products);
            Order order = _builder.GetOrder();

            DateTime afterBuild = DateTime.Now;

            Assert.AreEqual(15, order.OrderId);
            Assert.AreEqual(42, order.BuyerId);
            Assert.AreSame(products, order.Products);
            Assert.IsTrue(order.Date >= beforeBuild && order.Date <= afterBuild);
        }

        [TestMethod]
        public void Director_BuildFullOrder_SetsAllOrderProperties() {
            var products = new List<Product> {
                new Furniture("FN-0001", "Desk", 250, 25, 120, 60, 75, "Oak")
            };
            DateTime beforeBuild = DateTime.Now;

            _director.BuildFullOrder(orderId: 20, buyerId: 88, products, OrderStatus.Delivered);
            Order order = _builder.GetOrder();

            DateTime afterBuild = DateTime.Now;

            Assert.AreEqual(20, order.OrderId);
            Assert.AreEqual(88, order.BuyerId);
            Assert.AreSame(products, order.Products);
            Assert.AreEqual(OrderStatus.Delivered, order.Status);
            Assert.IsTrue(order.Date >= beforeBuild && order.Date <= afterBuild);
        }

        #endregion
    }
}
