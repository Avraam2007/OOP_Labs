using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Bundles;
using OOP_Main.Entities;
using OOP_Main.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OOP_UnitTests {
    [TestClass]
    public class ProductHelperTest {
        private List<Product> GetSampleProducts() {
            var testBundle = new ProductBundle(
                article: "BDL-0001",
                name: "Bundle",
                supplierId: 1,
                discountPercent: 10.0
            );
            
            testBundle.AddProduct(
                new Cloth(
                    article: "CL-0002",
                    name: "Beanie",
                    price: 100.0,
                    supplierId: 1,
                    material: "Wool"
                )
            );

            return new List<Product> {
                new Cloth (
                    article: "CL-0001", 
                    name: "Jeans", 
                    price: 15.0, 
                    supplierId: 1, 
                    material: "Cotton"
                ),
                new ElectronicProduct (
                    article: "EL-0001", 
                    name: "Fan", 
                    price: 150.0, 
                    supplierId: 1, 
                    power: 1500, 
                    maxVoltage: 230 
                ),
                new Furniture (
                    article: "FN-0001", 
                    name: "Chair", 
                    price: 300.0, 
                    supplierId: 1, 
                    material: "Pine", 
                    weight: 300, 
                    height: 20, 
                    length: 20, 
                    width: 20),
                new Sofa (
                    article: "SF-0001", 
                    name: "SuperSofa", 
                    price: 800.0, 
                    supplierId: 1, 
                    material: "Pine", 
                    weight: 300, 
                    height: 20, 
                    length: 20, 
                    width: 20, 
                    isAssemble: true
                ),
                testBundle
            };
        }

        #region NormalizeProductName Tests

        [DataTestMethod]
        [DataRow("Laptop", "laptop")]
        [DataRow("  Gaming Mouse  ", "gaming mouse")]
        [DataRow("\t\nWireless Keyboard\r\n", "wireless keyboard")]
        [DataRow("IPHONE 15 PRO", "iphone 15 pro")]
        [DataRow("already normalized", "already normalized")]
        [DataRow("   ", "")]
        [DataRow("", "")]
        public void NormalizeProductName_ValidStrings_ReturnsTrimmedLowercaseString(string input, string expected) {
            string actual = ProductHelpers.NormalizeProductName(input);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void NormalizeProductName_NullInput_ThrowsNullReferenceException() {
            Assert.ThrowsException<NullReferenceException>(() => ProductHelpers.NormalizeProductName(null));
        }

        #endregion


        #region FindProductsInPriceRange Tests

        [TestMethod]
        public void FindProductsInPriceRange_ValidRange_ReturnsSortedProductsWithinBounds() {
            var products = GetSampleProducts();

            var result = products.FindProductsInPriceRange(150.0, 800.0);

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(150.0, result[0].Price);
            Assert.AreEqual(300.0, result[1].Price);
            Assert.AreEqual(800.0, result[2].Price);
        }

        [TestMethod]
        public void FindProductsInPriceRange_NoProductsInRange_ReturnsEmptyList() {
            var products = GetSampleProducts();

            var result = products.FindProductsInPriceRange(2000.0, 5000.0);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void FindProductsInPriceRange_MinGreaterThanMax_ReturnsEmptyList() {
            var products = GetSampleProducts();

            var result = products.FindProductsInPriceRange(500.0, 100.0);

            Assert.AreEqual(0, result.Count);
        }

        #endregion

        #region GetCategoriesList Tests

        [TestMethod]
        public void GetCategoriesList_ValidProducts_ReturnsUniqueSortedCategories() {
            var products = GetSampleProducts();

            var categories = products.GetCategoriesList();

            Assert.AreEqual(5, categories.Count);
            CollectionAssert.AreEqual(new List<string> { "Cloth", "ElectronicProduct", "Furniture", "ProductBundle", "Sofa" }, categories);
        }

        [TestMethod]
        public void GetCategoriesList_EmptyCollection_ThrowsNullReferenceException() {
            var emptyProducts = new List<Product>();

            Assert.ThrowsException<NullReferenceException>(() => emptyProducts.GetCategoriesList());
        }

        #endregion

        #region FilterByCategory Tests

        [TestMethod]
        [DataRow("ELECTRONICPRODUCT")]
        [DataRow("electronicproduct")]
        [DataRow("EleCTRonIcpRoduCT")]
        public void FilterByCategory_ExistingCategoryDifferentCase_ReturnsAllMatchingProducts(string wantedCategory) {
            var products = GetSampleProducts();

            var result = products.FilterByCategory(wantedCategory);

            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result.All(p => p.Category.Equals("ElectronicProduct", StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public void FilterByCategory_NonExistentCategory_ReturnsEmptyList() {
            var products = GetSampleProducts();

            var result = products.FilterByCategory("Clothing");

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void FilterByCategory_NullCategory_ReturnsEmptyList() {
            var products = GetSampleProducts();

            var result = products.FilterByCategory(null);

            Assert.AreEqual(0, result.Count);
        }

        #endregion
    }
}
