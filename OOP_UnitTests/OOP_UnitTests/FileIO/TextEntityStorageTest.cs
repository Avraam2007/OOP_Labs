using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Bundles;
using OOP_Main.Entities;
using OOP_Main.FileIO;
using OOP_Main.FileIO.TextEntities;
using OOP_Main.States;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace OOP_UnitTests.FileIO {
    [TestClass]
    public class TextStorageTests {
        private string _tempDir;
        private TextProductStorage _productStorage;
        private TextUserStorage _userStorage;
        private TextEntityStorage _facadeStorage;
        private CultureInfo _originalCulture;

        [TestInitialize]
        public void Setup() {
            _originalCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            _tempDir = Path.Combine(Path.GetTempPath(), "OOP_TextTests_" + Guid.NewGuid().ToString("N"));
            _productStorage = new TextProductStorage();
            _userStorage = new TextUserStorage();
            _facadeStorage = new TextEntityStorage();
        }

        [TestCleanup]
        public void Cleanup() {
            Thread.CurrentThread.CurrentCulture = _originalCulture;

            if (Directory.Exists(_tempDir)) {
                Directory.Delete(_tempDir, recursive: true);
            }
        }

        #region TextProductStorage Tests

        [TestMethod]
        public void TextProductStorage_LoadFromNonExistentFile_ReturnsEmptyList() {
            string missingPath = Path.Combine(_tempDir, "missing_products.txt");

            List<Product> result = _productStorage.LoadFromFile(missingPath);

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [TestMethod]
        public void TextProductStorage_SaveAndLoad_CreatesDirectoryAndRestoresProducts() {
            string nestedPath = Path.Combine(_tempDir, "ProductsDir", "products.txt");
            var productsToSave = new List<Product> {
                new ElectronicProduct("EL-0001", "Laptop", 1250.75, 65, 220, 1),
                new Furniture("FN-0001", "Desk", 300.00, 20, 120, 60, 75, "Oak", 2),
                new Sofa("SF-0001", "Sofa", 850.50, 50, 100, 200, 85, "Velvet", true, 3),
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
            Assert.AreEqual(1250.75, loadedProducts[0].Price, 0.001);
            Assert.AreEqual(1, loadedProducts[0].SupplierId);

            Assert.IsInstanceOfType(loadedProducts[1], typeof(Furniture));
            Assert.IsInstanceOfType(loadedProducts[2], typeof(Sofa));
            Assert.IsInstanceOfType(loadedProducts[3], typeof(ProductBundle));
            Assert.IsInstanceOfType(loadedProducts[4], typeof(Cloth));
            Assert.AreEqual(25.99, loadedProducts[4].Price, 0.001);
        }

        [TestMethod]
        public void TextProductStorage_LoadWithMalformedAndEmptyLines_SkipsInvalidLines() {
            string filePath = Path.Combine(_tempDir, "mixed_products.txt");
            Directory.CreateDirectory(_tempDir);

            string[] lines = {
                "",
                "   ",
                "INVALID_LINE_WITHOUT_SEMICOLONS",
                "EL-0002;BrokenPrice;not_a_number;1",
                "EL-0003;BrokenSupplier;100.00;not_an_int",
                "CL-0002;Valid Scarf;45.50;7"
            };
            File.WriteAllLines(filePath, lines, Encoding.UTF8);

            List<Product> loaded = _productStorage.LoadFromFile(filePath);

            Assert.HasCount(1, loaded);
            Assert.AreEqual("CL-0002", loaded[0].Article);
            Assert.AreEqual("Valid Scarf", loaded[0].Name);
            Assert.AreEqual(45.50, loaded[0].Price, 0.001);
            Assert.AreEqual(7, loaded[0].SupplierId);
        }

        #endregion

        #region TextUserStorage Tests

        [TestMethod]
        public void TextUserStorage_LoadFromNonExistentFile_ReturnsEmptyList() {
            string missingPath = Path.Combine(_tempDir, "missing_users.txt");

            List<User> result = _userStorage.LoadFromFile(missingPath);

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [TestMethod]
        public void TextUserStorage_SaveAndLoad_PreservesUsersRolesAndBalances() {
            string filePath = Path.Combine(_tempDir, "UsersDir", "users.txt");
            var usersToSave = new List<User> {
                new User(1, "AdminUser", "hash1", startSum: 500.25, isAdmin: true),
                new User(2, "RegularCustomer", "hash2", startSum: 99.90, isAdmin: false)
            };

            _userStorage.SaveToFile(filePath, usersToSave);

            Assert.IsTrue(File.Exists(filePath));

            List<User> loadedUsers = _userStorage.LoadFromFile(filePath);

            Assert.HasCount(2, loadedUsers);

            Assert.AreEqual(1, loadedUsers[0].Id);
            Assert.AreEqual("AdminUser", loadedUsers[0].Username);
            Assert.AreEqual("hash1", loadedUsers[0].Password);
            Assert.AreEqual(UserRole.Administrator, loadedUsers[0].Role);
            Assert.AreEqual(500.25, loadedUsers[0].Account.Sum, 0.001);

            Assert.AreEqual(2, loadedUsers[1].Id);
            Assert.AreEqual("RegularCustomer", loadedUsers[1].Username);
            Assert.AreEqual("hash2", loadedUsers[1].Password);
            Assert.AreEqual(UserRole.Customer, loadedUsers[1].Role);
            Assert.AreEqual(99.90, loadedUsers[1].Account.Sum, 0.001);
        }

        [TestMethod]
        public void TextUserStorage_LoadWithMalformedLines_SkipsInvalidEntries() {
            string filePath = Path.Combine(_tempDir, "mixed_users.txt");
            Directory.CreateDirectory(_tempDir);

            string[] lines = {
                "",
                "bad_id;User1;pass;Customer;100.0",
                "1;User2;pass;NonExistentRole;100.0",
                "2;User3;pass;Customer;invalid_sum",
                "3;ValidUser;secretPass;Administrator;750.00"
            };
            File.WriteAllLines(filePath, lines, Encoding.UTF8);

            List<User> loaded = _userStorage.LoadFromFile(filePath);

            Assert.HasCount(1, loaded);
            Assert.AreEqual(3, loaded[0].Id);
            Assert.AreEqual("ValidUser", loaded[0].Username);
            Assert.AreEqual(UserRole.Administrator, loaded[0].Role);
            Assert.AreEqual(750.00, loaded[0].Account.Sum, 0.001);
        }

        #endregion

        #region TextEntityStorage Facade Tests

        [TestMethod]
        public void TextEntityStorage_SaveAndLoadProductsAndUsers_ReturnsExpectedData() {
            string productsPath = Path.Combine(_tempDir, "facade_products.txt");
            string usersPath = Path.Combine(_tempDir, "facade_users.txt");

            var products = new List<Product> {
                new Cloth("CL-0010", "Jacket", 120.0, "Leather", 2)
            };
            var users = new List<User> {
                new User(10, "FacadeCustomer", "pass123", startSum: 300.0)
            };

            _facadeStorage.SaveProductsToText(productsPath, products);
            List<Product> loadedProducts = _facadeStorage.LoadProductsFromText(productsPath);

            _facadeStorage.SaveUsersToText(usersPath, users);
            List<User> loadedUsers = _facadeStorage.LoadUsersFromText(usersPath);

            Assert.HasCount(1, loadedProducts);
            Assert.AreEqual("CL-0010", loadedProducts[0].Article);

            Assert.HasCount(1, loadedUsers);
            Assert.AreEqual("FacadeCustomer", loadedUsers[0].Username);
            Assert.AreEqual(300.0, loadedUsers[0].Account.Sum, 0.001);
        }

        #endregion
    }
}
