using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Bundles;
using OOP_Main.Decorators;
using OOP_Main.Entities;
using OOP_Main.Repositories;
using System;
using System.IO;
using System.Threading.Tasks;
using static OOP_Main.Utilities.Exceptions;

namespace OOP_UnitTests.Repositories {
    [TestClass]
    public class ProductDataTest {
        private string _tempDir;
        private string _originalDir;
        private ProductData _productData;

        [TestInitialize]
        public void Setup() {
            _originalDir = Directory.GetCurrentDirectory();
            _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(Path.Combine(_tempDir, "DataStorage"));
            Directory.SetCurrentDirectory(_tempDir);

            _productData = new ProductData();
        }

        [TestCleanup]
        public void Cleanup() {
            Directory.SetCurrentDirectory(_originalDir);
            if (Directory.Exists(_tempDir)) {
                Directory.Delete(_tempDir, true);
            }

            _productData = null;
        }

        #region Add & IsArticleExists Tests

        [TestMethod]
        public void AddProduct_StandardProduct_AddsToListAndRegistersArticle() {
            var product = new ElectronicProduct("EL-0001", "Laptop", 1500, 65, 220, 1);

            _productData.AddProduct(product);

            Assert.AreEqual(1, _productData.Products.Count);
            Assert.IsTrue(_productData.IsArticleExists("EL-0001"));
            Assert.IsFalse(_productData.IsArticleExists("EL-9999"));
        }

        [TestMethod]
        public void AddProduct_DecoratedProduct_RegistersBaseProductArticle() {
            var baseProduct = new Cloth("CL-0005", "T-Shirt", 50, "Cotton", 1);

            var decoratedProduct = new DiscountDecorator(baseProduct, 10);

            _productData.AddProduct(decoratedProduct);

            Assert.IsTrue(_productData.IsArticleExists("CL-0005"));
        }

        #endregion

        #region Load & LoadAsync Tests

        [TestMethod]
        public void Load_AfterSavingProducts_PopulatesProductsAndRegisteredArticles() {
            var repoToSave = new ProductData();
            repoToSave.AddProduct(new ElectronicProduct("EL-0010", "Phone", 900, 15, 5, 2));

            var repoToLoad = new ProductData();
            repoToLoad.Load();

            Assert.AreEqual(1, repoToLoad.Products.Count);
            Assert.IsTrue(repoToLoad.IsArticleExists("EL-0010"));
        }

        [TestMethod]
        public async Task LoadAsync_AfterSavingProducts_PopulatesProductsAndRegisteredArticles() {
            var repoToSave = new ProductData();
            repoToSave.AddProduct(new Furniture("FN-0003", "Chair", 120, 20, 30, 40, 30, "Wood"));

            var repoToLoad = new ProductData();
            await repoToLoad.LoadAsync();

            Assert.AreEqual(1, repoToLoad.Products.Count);
            Assert.IsTrue(repoToLoad.IsArticleExists("FN-0003"));
        }

        #endregion

        #region Get & Delete Tests

        [TestMethod]
        public void GetProductByArticleAndName_ExistingProduct_ReturnsProduct() {
            var repo = new ProductData();
            var product = new Sofa("SF-0001", "Comfort Sofa", 800, 30, 50, 50, 50, "Leather", false);
            repo.AddProduct(product);

            var byArticle = repo.GetProductByArticle("SF-0001");
            var byName = repo.GetProductByName("Comfort Sofa");

            Assert.IsTrue(byArticle.HasValue());
            Assert.AreEqual("Comfort Sofa", byArticle.Value.Name);

            Assert.IsTrue(byName.HasValue());
            Assert.AreEqual("SF-0001", byName.Value.Article);
        }

        [TestMethod]
        public void DeleteProductByName_ExistingProduct_RemovesFromListAndUnregistersArticle() {
            var repo = new ProductData();
            repo.AddProduct(new ElectronicProduct("EL-0002", "Mouse", 40, 10, 5, 5));

            bool isDeleted = repo.DeleteProductByName("Mouse");

            Assert.IsTrue(isDeleted);
            Assert.AreEqual(0, repo.Products.Count);
            Assert.IsFalse(repo.IsArticleExists("EL-0002"));
        }

        [TestMethod]
        public void DeleteProductByName_NonExistentProduct_ReturnsFalse() {
            var repo = new ProductData();

            bool isDeleted = repo.DeleteProductByName("NonExistent");

            Assert.IsFalse(isDeleted);
        }

        [TestMethod]
        public void DeleteProductByArticle_ExistingProduct_RemovesAndUnregistersArticle() {
            var repo = new ProductData();
            repo.AddProduct(new ElectronicProduct("EL-0003", "Keyboard", 80, 10, 5, 3));

            bool isDeleted = repo.DeleteProductByArticle("EL-0003");

            Assert.IsTrue(isDeleted);
            Assert.AreEqual(0, repo.Products.Count);
            Assert.IsFalse(repo.IsArticleExists("EL-0003"));
        }

        [TestMethod]
        public void DeleteProductByArticle_NonExistentArticle_ReturnsFalse() {
            bool isDeleted = _productData.DeleteProductByArticle("FN-9999");

            Assert.IsFalse(isDeleted);
        }

        #endregion

        #region ChangeProductPriceByArticle Tests

        [TestMethod]
        public void ChangeProductPriceByArticle_ExistingProduct_UpdatesPrice() {
            var repo = new ProductData();
            repo.AddProduct(new ElectronicProduct("EL-0001", "Monitor", 300, 10, 5, 1));

            repo.ChangeProductPriceByArticle("EL-0001", 250.50);

            var updated = repo.GetProductByArticle("EL-0001");
            Assert.AreEqual(250.50, updated.Value.Price);
        }

        [TestMethod]
        public void ChangeProductPriceByArticle_NonExistentArticle_ThrowsEntityNotFoundException() {
            var repo = new ProductData();

            Assert.ThrowsException<EntityNotFoundException>(() =>
                repo.ChangeProductPriceByArticle("EL-4040", 100));
        }

        #endregion

        #region GenerateNextProductArticle Tests

        [DataTestMethod]
        [DataRow(typeof(ElectronicProduct), "EL-0001")]
        [DataRow(typeof(Cloth), "CL-0001")]
        [DataRow(typeof(Sofa), "SF-0001")]
        [DataRow(typeof(Furniture), "FN-0001")]
        [DataRow(typeof(ProductBundle), "BDL-0001")]
        [DataRow(typeof(Product), "PRD-0001")]
        public void GenerateNextProductArticle_EmptyRepository_ReturnsFirstFormattedArticle(Type productType, string expectedArticle) {
            var repo = new ProductData();

            string generated = repo.GenerateNextProductArticle(productType);

            Assert.AreEqual(expectedArticle, generated);
        }

        [TestMethod]
        public void GenerateNextProductArticle_ExistingProducts_IncrementsFromMaxNumber() {
            var repo = new ProductData();
            repo.AddProduct(new ElectronicProduct("EL-0002", "Item 2", 10, 20, 2, 1));
            repo.AddProduct(new ElectronicProduct("EL-0009", "Item 9", 20, 20, 2, 1));
            repo.AddProduct(new Cloth("CL-0050", "Shirt", 30, "Silk", 1));

            string nextElectronic = repo.GenerateNextProductArticle(typeof(ElectronicProduct));
            string nextCloth = repo.GenerateNextProductArticle(typeof(Cloth));

            Assert.AreEqual("EL-0010", nextElectronic);
            Assert.AreEqual("CL-0051", nextCloth);
        }

        #endregion
    }
}
