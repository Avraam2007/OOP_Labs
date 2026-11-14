using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Bundles;
using OOP_Main.Entities;
using OOP_Main.FileIO;
using OOP_Main.FileIO.BinaryEntities;
using OOP_Main.States;
using System;
using System.Collections.Generic;
using System.IO;

namespace OOP_UnitTests.FileIO {
    [TestClass]
    public class BinaryStorageTests {
        private string _tempDir;
        private BinaryProductStorage _productStorage;
        private BinaryUserStorage _userStorage;
        private BinaryStorage _facadeStorage;

        [TestInitialize]
        public void Setup() {
            _tempDir = Path.Combine(Path.GetTempPath(), "OOP_BinaryTests_" + Guid.NewGuid().ToString("N"));
            _productStorage = new BinaryProductStorage();
            _userStorage = new BinaryUserStorage();
            _facadeStorage = new BinaryStorage();
        }

        [TestCleanup]
        public void Cleanup() {
            if (Directory.Exists(_tempDir)) {
                Directory.Delete(_tempDir, recursive: true);
            }
        }

        #region BinaryProductStorage Tests

        [TestMethod]
        public void BinaryProductStorage_LoadFromNonExistentFile_ReturnsEmptyList() {
            string missingPath = Path.Combine(_tempDir, "non_existent_products.bin");

            List<Product> result = _productStorage.LoadFromFile(missingPath);

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [TestMethod]
        public void BinaryProductStorage_SaveAndLoad_CreatesDirectoryAndRestoresProductsByArticle() {
            string nestedPath = Path.Combine(_tempDir, "SubFolder", "products.bin");
            var productsToSave = new List<Product> {
                new ElectronicProduct("EL-0001", "Laptop", 1200.50, 65, 220, 1),
                new Furniture("FN-0001", "Desk", 300.00, 20, 120, 60, 75, "Oak", 2),
                new Sofa("SF-0001", "Sofa", 850.00, 50, 100, 200, 85, "Velvet", true, 3),
                new ProductBundle("BDL-0001", "Office Kit", 4, 15),
                new Cloth("CL-0001", "T-Shirt", 25.99, "Cotton", 5)
            };

            _productStorage.SaveToFile(nestedPath, productsToSave);

            Assert.IsTrue(File.Exists(nestedPath));

            List<Product> loadedProducts = _productStorage.LoadFromFile(nestedPath);

            Assert.HasCount(5, loadedProducts);

            Assert.IsInstanceOfType(loadedProducts[0], typeof(ElectronicProduct));
            Assert.AreEqual("EL-0001", loadedProducts[0].Article);
            Assert.AreEqual("Laptop", loadedProducts[0].Name);
            Assert.AreEqual(1200.50, loadedProducts[0].Price, 0.001);
            Assert.AreEqual(1, loadedProducts[0].SupplierId);

            Assert.IsInstanceOfType(loadedProducts[1], typeof(Furniture));
            Assert.AreEqual("FN-0001", loadedProducts[1].Article);

            Assert.IsInstanceOfType(loadedProducts[2], typeof(Sofa));
            Assert.AreEqual("SF-0001", loadedProducts[2].Article);

            Assert.IsInstanceOfType(loadedProducts[3], typeof(ProductBundle));
            Assert.AreEqual("BDL-0001", loadedProducts[3].Article);

            Assert.IsInstanceOfType(loadedProducts[4], typeof(Cloth));
            Assert.AreEqual("CL-0001", loadedProducts[4].Article);
            Assert.AreEqual(25.99, loadedProducts[4].Price, 0.001);
        }

        [TestMethod]
        public void BinaryProductStorage_SaveEmptyList_LoadsEmptyList() {
            string filePath = Path.Combine(_tempDir, "empty_products.bin");

            _productStorage.SaveToFile(filePath, new List<Product>());
            List<Product> loaded = _productStorage.LoadFromFile(filePath);

            Assert.IsNotNull(loaded);
            Assert.IsEmpty(loaded);
        }

        #endregion

        #region BinaryUserStorage Tests

        [TestMethod]
        public void BinaryUserStorage_LoadFromNonExistentFile_ReturnsEmptyList() {
            string missingPath = Path.Combine(_tempDir, "non_existent_users.bin");

            List<User> result = _userStorage.LoadFromFile(missingPath);

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [TestMethod]
        public void BinaryUserStorage_SaveAndLoad_PreservesUsersAndProperties() {
            string filePath = Path.Combine(_tempDir, "UsersDir", "users.bin");
            var usersToSave = new List<User> {
                new User(1, "AdminUser", "hash1", startSum: 500.0, isAdmin: true),
                new User(2, "CustomerUser", "hash2", startSum: 120.5, isAdmin: false)
            };

            _userStorage.SaveToFile(filePath, usersToSave);

            Assert.IsTrue(File.Exists(filePath));

            List<User> loadedUsers = _userStorage.LoadFromFile(filePath);

            Assert.HasCount(2, loadedUsers);
            Assert.AreEqual(1, loadedUsers[0].Id);
            Assert.AreEqual("AdminUser", loadedUsers[0].Username);
            Assert.AreEqual(UserRole.Administrator, loadedUsers[0].Role);
            Assert.AreEqual(500.0, loadedUsers[0].Account.Sum, 0.001);

            Assert.AreEqual(2, loadedUsers[1].Id);
            Assert.AreEqual("CustomerUser", loadedUsers[1].Username);
            Assert.AreEqual(UserRole.Customer, loadedUsers[1].Role);
            Assert.AreEqual(120.5, loadedUsers[1].Account.Sum, 0.001);
        }

        #endregion

        #region BinaryStorage Facade Tests

        [TestMethod]
        public void BinaryStorage_SaveAndLoadProductsAndUsers_ExecutesWithoutErrors() {
            string productsPath = Path.Combine(_tempDir, "facade_products.bin");
            string usersPath = Path.Combine(_tempDir, "facade_users.bin");

            var products = new List<Product> {
                new Cloth("CL-0010", "Scarf", 30.0, "Silk", 1)
            };
            var users = new List<User> {
                new User(10, "FacadeUser", "pass123")
            };

            _facadeStorage.SaveProducts(productsPath, products);
            _facadeStorage.LoadProducts(productsPath);

            _facadeStorage.SaveUsers(usersPath, users);
            _facadeStorage.LoadUsers(usersPath);

            Assert.IsTrue(File.Exists(productsPath));
            Assert.IsTrue(File.Exists(usersPath));
        }

        #endregion
    }
}
