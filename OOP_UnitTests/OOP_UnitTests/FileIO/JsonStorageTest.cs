using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Entities;
using OOP_Main.FileIO;
using System;
using System.Collections.Generic;
using System.IO;

namespace OOP_UnitTests.FileIO {
    [TestClass]
    public class JsonStorageTests {
        private string _tempDir;
        private JsonStorage<List<Product>> _productListStorage;

        [TestInitialize]
        public void Setup() {
            _tempDir = Path.Combine(Path.GetTempPath(), "OOP_JsonStorageTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDir);
            _productListStorage = new JsonStorage<List<Product>>();
        }

        [TestCleanup]
        public void Cleanup() {
            if (Directory.Exists(_tempDir)) {
                Directory.Delete(_tempDir, recursive: true);
            }
        }

        #region SaveToFile & LoadFromFile Happy Path Tests

        [TestMethod]
        public void SaveToFileAndLoadFromFile_PolymorphicProducts_PreservesConcreteTypesAndProperties() {
            string nestedPath = Path.Combine(_tempDir, "SubDir", "catalog.json");
            var productsToSave = new List<Product> {
                new ElectronicProduct("EL-0001", "Laptop", 1500.0, 65, 220, 1),
                new Cloth("CL-0001", "Hoodie", 65.50, "Cotton", 2),
                new Sofa("SF-0001", "Corner Sofa", 900.0, 50, 100, 200, 85, "Velvet", true, 3)
            };

            _productListStorage.SaveToFile(nestedPath, productsToSave);

            Assert.IsTrue(File.Exists(nestedPath));

            List<Product> loadedProducts = _productListStorage.LoadFromFile(nestedPath);

            Assert.IsNotNull(loadedProducts);
            Assert.HasCount(3, loadedProducts);

            Assert.IsInstanceOfType(loadedProducts[0], typeof(ElectronicProduct));
            Assert.AreEqual("EL-0001", loadedProducts[0].Article);
            Assert.AreEqual(65, ((ElectronicProduct)loadedProducts[0]).Power);

            Assert.IsInstanceOfType(loadedProducts[1], typeof(Cloth));
            Assert.AreEqual("Cotton", ((Cloth)loadedProducts[1]).Material);

            Assert.IsInstanceOfType(loadedProducts[2], typeof(Sofa));
            Assert.IsTrue(((Sofa)loadedProducts[2]).IsAssemble);
        }

        #endregion

        #region Edge Cases & Missing/Empty File Tests

        [TestMethod]
        public void LoadFromFile_NonExistentFile_ReturnsNewDefaultInstance() {
            string missingPath = Path.Combine(_tempDir, "does_not_exist.json");

            List<Product> result = _productListStorage.LoadFromFile(missingPath);

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [TestMethod]
        public void LoadFromFile_EmptyOrWhitespaceFile_ReturnsNewDefaultInstance() {
            string emptyPath = Path.Combine(_tempDir, "empty.json");
            File.WriteAllText(emptyPath, "   \r\n  ");

            List<Product> result = _productListStorage.LoadFromFile(emptyPath);

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [TestMethod]
        public void LoadFromFile_JsonNullLiteral_ReturnsNewDefaultInstance() {
            string nullJsonPath = Path.Combine(_tempDir, "null_literal.json");
            File.WriteAllText(nullJsonPath, "null");

            List<Product> result = _productListStorage.LoadFromFile(nullJsonPath);

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        #endregion

        #region Exception Handling Tests

        [TestMethod]
        public void LoadFromFile_InvalidJsonSyntax_HandlesErrorAndReturnsNewInstance() {
            string corruptedPath = Path.Combine(_tempDir, "corrupted.json");
            File.WriteAllText(corruptedPath, "{ invalid json : [ ");

            List<Product> result = _productListStorage.LoadFromFile(corruptedPath);

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [TestMethod]
        public void LoadFromFile_MismatchedJsonStructure_HandlesSerializationErrorAndReturnsNewInstance() {
            string wrongStructurePath = Path.Combine(_tempDir, "wrong_type.json");
            // Очікується List<Product>, але передається рядок замість масиву -> JsonSerializationException
            File.WriteAllText(wrongStructurePath, "\"not_a_list\"");

            List<Product> result = _productListStorage.LoadFromFile(wrongStructurePath);

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [TestMethod]
        public void LoadFromFile_LockedByAnotherStream_HandlesIOExceptionAndReturnsNewInstance() {
            string lockedPath = Path.Combine(_tempDir, "locked_read.json");

            using (var lockStream = new FileStream(lockedPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None)) {
                List<Product> result = _productListStorage.LoadFromFile(lockedPath);

                Assert.IsNotNull(result);
                Assert.IsEmpty(result);
            }
        }

        [TestMethod]
        public void SaveToFile_LockedByAnotherStream_HandlesIOExceptionWithoutThrowing() {
            string lockedPath = Path.Combine(_tempDir, "locked_write.json");

            using (var lockStream = new FileStream(lockedPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None)) {
                _productListStorage.SaveToFile(lockedPath, new List<Product>());
            }

            Assert.IsTrue(File.Exists(lockedPath));
        }

        [TestMethod]
        public void SaveToFile_ReadOnlyFile_HandlesUnauthorizedAccessExceptionWithoutThrowing() {
            string readOnlyPath = Path.Combine(_tempDir, "readonly.json");
            File.WriteAllText(readOnlyPath, "[]");
            File.SetAttributes(readOnlyPath, FileAttributes.ReadOnly);

            try {
                _productListStorage.SaveToFile(readOnlyPath, new List<Product> {
                    new Cloth("CL-0002", "Scarf", 20.0, "Silk", 1)
                });

                Assert.AreEqual("[]", File.ReadAllText(readOnlyPath));
            }
            finally {
                File.SetAttributes(readOnlyPath, FileAttributes.Normal);
            }
        }

        #endregion
    }
}
