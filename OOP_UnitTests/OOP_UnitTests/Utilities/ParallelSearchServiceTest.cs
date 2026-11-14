using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Entities;
using OOP_Main.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace OOP_UnitTests.Utilities {
    [TestClass]
    public class ParallelSearchServiceTests {
        private List<Product> _sampleProducts;

        [TestInitialize]
        public void Setup() {
            _sampleProducts = new List<Product> {
                new ElectronicProduct("EL-0001", "Gaming Laptop", 1500.0, 180, 220, 1),
                new ElectronicProduct("EL-0002", "Wireless Mouse", 45.0, 5, 5, 1),
                new Cloth("CL-0001", "Cotton T-Shirt", 25.0, "Cotton", 2),
                new Cloth("CL-0002", "Winter Laptop Sleeve", 35.0, "Neoprene", 2),
                new Furniture("FN-0001", "Oak Dining Table", 400.0, 30, 90, 160, 75, "Oak", 3),
                new Sofa("SF-0001", "Velvet Corner Sofa", 950.0, 55, 110, 230, 85, "Velvet", true, 3)
            };
        }

        [TestCleanup]
        public void Cleanup() {
            _sampleProducts = null;
        }

        #region Query Matching Tests

        [TestMethod]
        public void FastSearch_MatchesNameCaseInsensitive_ReturnsAllMatchingProducts() {
            List<Product> results = ParallelSearchService.FastSearch(_sampleProducts, "lApToP", maxPrice: 2000.0);

            Assert.HasCount(2, results);
            Assert.Contains(p => p.Article == "EL-0001", results);
            Assert.Contains(p => p.Article == "CL-0002", results);
        }

        [TestMethod]
        public void FastSearch_MatchesCategoryCaseInsensitive_ReturnsProductsOfThatCategory() {
            List<Product> results = ParallelSearchService.FastSearch(_sampleProducts, "electronicproduct", maxPrice: 5000.0);

            Assert.HasCount(2, results);
            Assert.IsTrue(results.All(p => p is ElectronicProduct));
        }

        [TestMethod]
        public void FastSearch_EmptyQueryString_ReturnsAllProductsWithinPriceRange() {
            List<Product> results = ParallelSearchService.FastSearch(_sampleProducts, string.Empty, maxPrice: 50.0, minPrice: 20.0);

            Assert.HasCount(3, results);
            Assert.Contains(p => p.Article == "EL-0002", results); // 45.0
            Assert.Contains(p => p.Article == "CL-0001", results); // 25.0
            Assert.Contains(p => p.Article == "CL-0002", results); // 35.0
        }

        [TestMethod]
        public void FastSearch_NoMatchingProducts_ReturnsEmptyList() {
            List<Product> results = ParallelSearchService.FastSearch(_sampleProducts, "NonExistentQuery", maxPrice: 10000.0);

            Assert.IsNotNull(results);
            Assert.IsEmpty(results);
        }

        #endregion

        #region Price Range & Boundary Tests

        [TestMethod]
        public void FastSearch_InclusivePriceBoundaries_IncludesExactMinAndMaxPrices() {
            // EL-0002 має ціну 45.0, FN-0001 має ціну 400.0
            List<Product> results = ParallelSearchService.FastSearch(_sampleProducts, string.Empty, maxPrice: 400.0, minPrice: 45.0);

            Assert.HasCount(2, results);
            Assert.Contains(p => p.Article == "EL-0002", results);
            Assert.Contains(p => p.Article == "FN-0001", results);
        }

        [TestMethod]
        public void FastSearch_MinPriceGreaterThanMaxPrice_ReturnsEmptyList() {
            List<Product> results = ParallelSearchService.FastSearch(_sampleProducts, "Laptop", maxPrice: 100.0, minPrice: 500.0);

            Assert.IsEmpty(results);
        }

        #endregion

        #region Empty Collection & Concurrency Tests

        [TestMethod]
        public void FastSearch_EmptyProductsCollection_ReturnsEmptyList() {
            var emptyProducts = new List<Product>();

            List<Product> results = ParallelSearchService.FastSearch(emptyProducts, "Laptop", maxPrice: 5000.0);

            Assert.IsNotNull(results);
            Assert.IsEmpty(results);
        }

        [TestMethod]
        public void FastSearch_LargeCollectionInParallel_FindsAllExpectedItemsThreadSafely() {
            var largeCatalog = new List<Product>();
            for (int i = 1; i <= 500; i++) {
                largeCatalog.Add(new Cloth($"CL-{i:D4}", $"TargetItem #{i}", 50.0, "Cotton", 1));
                largeCatalog.Add(new Cloth($"FN-{i:D4}", $"OtherItem #{i}", 50.0, "Wool", 1));
            }

            List<Product> results = ParallelSearchService.FastSearch(largeCatalog, "TargetItem", maxPrice: 100.0, minPrice: 10.0);

            Assert.HasCount(500, results);
        }

        #endregion
    }
}
