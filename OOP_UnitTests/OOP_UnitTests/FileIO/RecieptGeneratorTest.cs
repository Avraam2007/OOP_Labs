using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main;
using OOP_Main.Decorators;
using OOP_Main.Entities;
using OOP_Main.FileIO;
using OOP_Main.Interfaces;
using System;
using static OOP_Main.Utilities.Exceptions;

namespace OOP_UnitTests.FileIO {
    [TestClass]
    public class ReceiptGeneratorTests {
        #region BuildOrderReceipt Tests

        [TestMethod]
        public void BuildOrderReceipt_ValidOrderAndUser_FormatsHeaderItemsAndTotalsCorrectly() {
            var buyer = new User(1, "JohnDoe", "pass1234");
            var laptop = new ElectronicProduct("EL-0001", "Gaming Laptop", 1000.0, 100, 220, 1);
            var shirt = new Cloth("CL-0001", "Cotton Shirt", 50.0, "Cotton", 2);

            var order = new Order(orderId: 77, buyerId: buyer.Id);
            order.AddProduct(laptop);
            order.AddProduct(shirt);

            string receipt = ReceiptGenerator.BuildOrderReceipt(order, buyer);

            Assert.StartsWith(">>> E-COMMERCE SYSTEM PAYMENT PROOF <<<\n\n", receipt);
            Assert.Contains("OFFICIAL STORE RECEIPT", receipt);
            Assert.Contains("Order ID: 77", receipt);
            Assert.Contains("Customer: JohnDoe", receipt);
            Assert.Contains("EL-0001", receipt);
            Assert.Contains("Gaming Laptop", receipt);
            Assert.Contains("CL-0001", receipt);
            Assert.Contains("Cotton Shirt", receipt);
            Assert.Contains("Subtotal:", receipt);
            Assert.Contains("Shipping Cost:", receipt);
            Assert.Contains("TOTAL AMOUNT:", receipt);
            Assert.Contains("Thank you for shopping with us!", receipt);
        }

        [TestMethod]
        public void BuildOrderReceipt_NullBuyer_DisplaysAnonymousCustomer() {
            var order = new Order(orderId: 88, buyerId: 0);
            order.AddProduct(new Cloth("CL-0002", "Scarf", 20.0, "Silk", 1));

            string receipt = ReceiptGenerator.BuildOrderReceipt(order, buyer: null);

            Assert.Contains("Order ID: 88", receipt);
            Assert.Contains("Customer: Anonymous", receipt);
        }

        [TestMethod]
        public void BuildOrderReceipt_EmptyProductsList_GeneratesReceiptWithZeroTotals() {
            var buyer = new User(2, "EmptyCartUser", "pass1234");
            var emptyOrder = new Order(orderId: 99, buyerId: buyer.Id);

            string receipt = ReceiptGenerator.BuildOrderReceipt(emptyOrder, buyer);

            Assert.Contains("Order ID: 99", receipt);
            Assert.Contains("Customer: EmptyCartUser", receipt);
            Assert.Contains("Subtotal:", receipt);
            Assert.Contains("TOTAL AMOUNT:", receipt);
        }

        [TestMethod]
        public void BuildOrderReceipt_LongArticleAndName_TruncatesWithEllipsis() {
            var buyer = new User(3, "Alice", "pass1234");
            var baseProduct = new ElectronicProduct("EL-0001", "UltraWideCurvedGamingMonitor2026", 500.0, 50, 220, 1);
            // Декоратор подовжує артикул до "EXTWAR2-DISC15-EL-0001" (> 15 символів)
            Product decorated = new ExtendedWarrantyDecorator(new DiscountDecorator(baseProduct, 15), 2);

            var order = new Order(orderId: 105, buyerId: buyer.Id);
            order.AddProduct(decorated);

            string receipt = ReceiptGenerator.BuildOrderReceipt(order, buyer);

            // Артикул > 15 символів -> перші 12 символів + "..." -> "EXTWAR2-DISC..."
            Assert.Contains("EXTWAR2-DISC...", receipt);
            // Назва > 18 символів -> перші 15 символів + "..." -> "UltraWideCurved..."
            Assert.Contains("UltraWideCurved...", receipt);
        }

        #endregion

        #region SaveReceipt Tests

        [TestMethod]
        public void SaveReceipt_ValidStorage_PassesReceiptTextAndPathToStorage() {
            var fakeStorage = new FakeStringStorage();
            string receiptContent = "Sample Receipt Content";
            string targetPath = @"C:\Temp\receipt_101.txt";

            ReceiptGenerator.SaveReceipt(receiptContent, targetPath, fakeStorage);

            Assert.AreEqual(1, fakeStorage.SaveCallCount);
            // Зверніть увагу на порядок аргументів у ReceiptGenerator.SaveReceipt:
            // storage.SaveToFile(receiptText, outputFilePath)
            Assert.AreEqual(receiptContent, fakeStorage.LastFirstArg);
            Assert.AreEqual(targetPath, fakeStorage.LastSecondArg);
        }

        [TestMethod]
        public void SaveReceipt_StorageThrowsException_WrapsInECommerceException() {
            var failingStorage = new FailingStringStorage();
            string targetPath = @"C:\InvalidPath\receipt.txt";

            var ex = Assert.Throws<ECommerceException>(() =>
                ReceiptGenerator.SaveReceipt("Receipt", targetPath, failingStorage)
            );

            Assert.Contains($"Error with saving receipt in file '{targetPath}'", ex.Message);
            Assert.IsInstanceOfType(ex.InnerException, typeof(InvalidOperationException));
        }

        #endregion

        #region Test Doubles

        private class FakeStringStorage : IStorage<string> {
            public int SaveCallCount { get; private set; }
            public string LastFirstArg { get; private set; }
            public string LastSecondArg { get; private set; }

            public string LoadFromFile(string filePath) => string.Empty;

            public void SaveToFile(string filePath, string data) {
                SaveCallCount++;
                LastFirstArg = filePath;
                LastSecondArg = data;
            }
        }

        private class FailingStringStorage : IStorage<string> {
            public string LoadFromFile(string filePath) => throw new InvalidOperationException("Read failure");

            public void SaveToFile(string filePath, string data) {
                throw new InvalidOperationException("Disk write failure");
            }
        }

        #endregion
    }
}
