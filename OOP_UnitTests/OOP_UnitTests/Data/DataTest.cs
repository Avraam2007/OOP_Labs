using Microsoft.VisualStudio.TestTools.UnitTesting;
using NOptional;
using OOP_Main;
using OOP_Main.Entities;
using OOP_Main.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OOP_UnitTests.Repositories {
    [TestClass]
    public class DataTests {
        private string _tempDir;
        private string _originalDir;
        private Data _data;

        [TestInitialize]
        public void Setup() {
            _originalDir = Directory.GetCurrentDirectory();
            _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(Path.Combine(_tempDir, "DataStorage"));
            Directory.SetCurrentDirectory(_tempDir);

            _data = new Data();
        }

        [TestCleanup]
        public void Cleanup() {
            Directory.SetCurrentDirectory(_originalDir);
            if (Directory.Exists(_tempDir)) {
                Directory.Delete(_tempDir, true);
            }

            _data = null;
        }

        #region Initialization & Properties Tests

        [TestMethod]
        public void Constructor_InitializesEmptyCollectionsAndQueue() {
            Assert.IsNotNull(_data.Users);
            Assert.IsNotNull(_data.Products);
            Assert.IsNotNull(_data.Orders);
            Assert.IsNotNull(_data.Suppliers);
            Assert.IsNotNull(_data.OrderQueue);

            Assert.AreEqual(0, _data.Users.Count);
            Assert.AreEqual(0, _data.Products.Count);
            Assert.AreEqual(0, _data.Orders.Count);
            Assert.AreEqual(0, _data.Suppliers.Count);
        }

        [TestMethod]
        public void CurrentUser_SetAndGet_RetainsUser() {
            var user = new User(1, "Admin", "pass1234", 12.0, true);
            _data.CurrentUser = Optional.Of(user);

            Assert.IsTrue(_data.CurrentUser.HasValue());
            Assert.AreEqual("Admin", _data.CurrentUser.Value.Username);
        }

        #endregion

        #region Async Load, Save & Backup Tests

        [TestMethod]
        public async Task SaveAllDataAndLoadAllData_PersistsAndRestoresAllRepositories() {
            _data.AddUser("TestUser", "password123");
            _data.AddProduct(new ElectronicProduct("EL-0001", "Laptop", 1500, 65, 220, 1));
            _data.AddSupplier("TechSupply", "tech@supply.com", "0501234567", 4.8);
            _data.AddOrder(1, new List<Product>());

            await _data.SaveAllData();

            var loadedData = new Data();
            await loadedData.LoadAllData();

            Assert.AreEqual(1, loadedData.Users.Count);
            Assert.AreEqual("TestUser", loadedData.Users[0].Username);

            Assert.AreEqual(1, loadedData.Products.Count);
            Assert.AreEqual("EL-0001", loadedData.Products[0].Article);

            Assert.AreEqual(1, loadedData.Suppliers.Count);
            Assert.AreEqual("TechSupply", loadedData.Suppliers[0].Name);

            Assert.AreEqual(1, loadedData.Orders.Count);
            Assert.AreEqual(1, loadedData.Orders[0].BuyerId);
        }

        [TestMethod]
        public async Task SaveBackup_CreatesZipFileInBackupsDirectory() {
            _data.AddUser("BackupUser", "pass1234");
            await _data.SaveAllData();

            _data.SaveBackup();

            string backupDir = Path.Combine(_tempDir, "Backups");
            Assert.IsTrue(Directory.Exists(backupDir));
            var zipFiles = Directory.GetFiles(backupDir, "*.zip");
            Assert.IsTrue(zipFiles.Length > 0);
        }

        #endregion

        #region User Delegation Tests

        [TestMethod]
        public void UserOperations_AddGetDelete_DelegatesCorrectly() {
            var user1 = new User(1, "UserOne", "pass1");
            _data.AddUser(user1);
            _data.AddUser("UserTwo", "pass2", isAdmin: true);

            Assert.AreEqual(2, _data.Users.Count);
            Assert.IsTrue(_data.GetUserById(1).HasValue());
            Assert.IsTrue(_data.GetUserByUsername("UserTwo").HasValue());

            Assert.IsTrue(_data.DeleteUserById(1));
            Assert.IsTrue(_data.DeleteUserByUsername("UserTwo"));
            Assert.AreEqual(0, _data.Users.Count);
        }

        #endregion

        #region Order Delegation Tests

        [TestMethod]
        public void OrderOperations_AddGetDelete_DelegatesCorrectly() {
            var order = new Order(10, 5);
            _data.AddOrder(order);
            _data.AddOrder(5, new List<Product> { new ElectronicProduct("EL-0001", "Phone", 500, 10, 5, 1) });

            Assert.AreEqual(2, _data.Orders.Count);
            Assert.IsTrue(_data.GetOrderById(10).HasValue());
            Assert.AreEqual(2, _data.GetOrdersFromUser(5).Count);

            Assert.IsTrue(_data.DeleteOrder(10));
            Assert.AreEqual(1, _data.Orders.Count);
        }

        #endregion

        #region Supplier Delegation Tests

        [TestMethod]
        public void SupplierOperations_AddGetDelete_DelegatesCorrectly() {
            var supplier = new Supplier(1, "Supp1", "a@b.com", "0568965478", 4.5);
            _data.AddSupplier(supplier);
            _data.AddSupplier("Supp2", "c@d.com", "0987654321", 4.0);

            Assert.AreEqual(2, _data.Suppliers.Count);
            Assert.IsTrue(_data.GetSupplierById(1).HasValue());
            Assert.IsTrue(_data.GetSupplierByName("Supp2").HasValue());

            Assert.IsTrue(_data.DeleteSupplierById(1));
            Assert.IsTrue(_data.DeleteSupplierByName("Supp2"));
            Assert.AreEqual(0, _data.Suppliers.Count);
        }

        #endregion

        #region Product & Cascade Delete Tests

        [TestMethod]
        public void ProductOperations_AddGetUpdateGenerate_DelegatesCorrectly() {
            string nextArticle = _data.GenerateNextProductArticle(typeof(ElectronicProduct));
            Assert.AreEqual("EL-0001", nextArticle);

            var product = new ElectronicProduct(nextArticle, "Monitor", 300, 20, 220, 2);
            _data.AddProduct(product);

            Assert.IsTrue(_data.IsArticleExists("EL-0001"));
            Assert.IsTrue(_data.GetProductByArticle("EL-0001").HasValue());
            Assert.IsTrue(_data.GetProductByName("Monitor").HasValue());

            _data.ChangeProductPriceByArticle("EL-0001", 275.0);
            Assert.AreEqual(275.0, _data.GetProductByArticle("EL-0001").Value.Price);
        }

        [TestMethod]
        public void DeleteProductByName_ExistingProduct_RemovesFromProductsAndAllSupplierCatalogs() {
            var product1 = new ElectronicProduct("EL-0001", "Gaming Mouse", 50, 5, 5, 10);
            var product2 = new ElectronicProduct("EL-0002", "Keyboard", 100, 5, 5, 10);
            _data.AddProduct(product1);
            _data.AddProduct(product2);

            var supplier1 = new Supplier(1, "Supplier A", "a@test.com", "0254568912", 4.5);
            supplier1.Catalog.Add(product1);
            supplier1.Catalog.Add(product2);

            var supplier2 = new Supplier(2, "Supplier B", "b@test.com", "0698547896", 4.0);
            supplier2.Catalog.Add(new ElectronicProduct("EL-0001", "gaming mouse", 50, 5, 5, 10));

            _data.AddSupplier(supplier1);
            _data.AddSupplier(supplier2);

            bool result = _data.DeleteProductByName("Gaming Mouse");

            Assert.IsTrue(result);
            Assert.AreEqual(1, _data.Products.Count);
            Assert.IsFalse(_data.IsArticleExists("EL-0001"));

            Assert.AreEqual(1, supplier1.Catalog.Count);
            Assert.AreEqual("Keyboard", supplier1.Catalog[0].Name);
            Assert.AreEqual(0, supplier2.Catalog.Count);
        }

        [TestMethod]
        public void DeleteProductByName_NonExistentProduct_ReturnsFalseAndLeavesCatalogsIntact() {
            var product = new ElectronicProduct("EL-0001", "Monitor", 300, 20, 220, 2);
            _data.AddProduct(product);

            var supplier = new Supplier(1, "Supplier A", "a@test.com", "+380584567896", 4.5);
            supplier.Catalog.Add(product);
            _data.AddSupplier(supplier);

            bool result = _data.DeleteProductByName("NonExistentItem");

            Assert.IsFalse(result);
            Assert.AreEqual(1, _data.Products.Count);
            Assert.AreEqual(1, supplier.Catalog.Count);
        }

        [TestMethod]
        public void DeleteProductByArticle_ExistingProduct_RemovesFromProductsAndAllSupplierCatalogs() {
            var product1 = new Cloth("CL-0001", "Jacket", 120, "Leather", 5);
            var product2 = new Cloth("CL-0002", "Hat", 25, "Wool", 15);
            _data.AddProduct(product1);
            _data.AddProduct(product2);

            var supplier = new Supplier(1, "Fashion Supp", "f@test.com", "0105894563", 4.9);
            supplier.Catalog.Add(product1);
            supplier.Catalog.Add(product2);
            _data.AddSupplier(supplier);

            bool result = _data.DeleteProductByArticle("CL-0001");

            Assert.IsTrue(result);
            Assert.AreEqual(1, _data.Products.Count);
            Assert.IsFalse(_data.IsArticleExists("CL-0001"));
            Assert.AreEqual(1, supplier.Catalog.Count);
            Assert.AreEqual("CL-0002", supplier.Catalog[0].Article);
        }

        [TestMethod]
        public void DeleteProductByArticle_NonExistentArticle_ReturnsFalse() {
            bool result = _data.DeleteProductByArticle("EL-9999");

            Assert.IsFalse(result);
        }

        #endregion
    }
}
