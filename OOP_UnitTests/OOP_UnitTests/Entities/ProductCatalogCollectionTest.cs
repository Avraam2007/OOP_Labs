using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OOP_UnitTests.Entities {
    [TestClass]
    public class ProductCatalogCollectionTests {
        private ProductCatalogCollection _collection;

        [TestInitialize]
        public void Setup() {
            _collection = new ProductCatalogCollection(initialCapacity: 2);
        }

        [TestCleanup]
        public void Cleanup() {
            _collection = null;
        }

        #region Add & Capacity Tests

        [TestMethod]
        public void Constructor_DefaultCapacity_InitializesEmptyCollection() {
            var defaultCollection = new ProductCatalogCollection();

            Assert.AreEqual(0, defaultCollection.Count);
        }

        [TestMethod]
        public void Add_ExceedingInitialCapacity_ResizesArrayAndPreservesAllItems() {
            var p1 = new Cloth("CL-0001", "Shirt", 30, "Cotton", 1);
            var p2 = new Cloth("CL-0002", "Pants", 50, "Denim", 1);
            var p3 = new ElectronicProduct("EL-0001", "Mouse", 25, 5, 5, 2);

            _collection.Add(p1);
            _collection.Add(p2);
            _collection.Add(p3);

            Assert.AreEqual(3, _collection.Count);
            var items = _collection.ToList();
            Assert.AreSequenceEqual(new List<Product> { p1, p2, p3 }, items);
        }

        #endregion

        #region Enumeration & Iterator Bounds Tests

        [TestMethod]
        public void Foreach_EmptyCollection_DoesNotIterate() {
            int iteratedCount = 0;

            foreach (var _ in _collection) {
                iteratedCount++;
            }

            Assert.AreEqual(0, iteratedCount);
        }

        [TestMethod]
        public void GetEnumerator_NonGenericInterface_IteratesThroughAllItems() {
            var p1 = new Cloth("CL-0001", "Hat", 15, "Wool", 1);
            var p2 = new Cloth("CL-0002", "Scarf", 20, "Silk", 1);
            _collection.Add(p1);
            _collection.Add(p2);

            IEnumerable nonGenericCollection = _collection;
            var result = new List<object>();

            foreach (var item in nonGenericCollection) {
                result.Add(item);
            }

            Assert.AreSequenceEqual(new List<object> { p1, p2 }, result);
        }

        [TestMethod]
        public void Enumerator_Reset_RestartsIterationFromBeginning() {
            var p1 = new Cloth("CL-0001", "Shirt", 30, "Cotton", 1);
            var p2 = new Cloth("CL-0002", "Jacket", 90, "Leather", 1);
            _collection.Add(p1);
            _collection.Add(p2);

            using (var enumerator = _collection.GetEnumerator()) {
                Assert.IsTrue(enumerator.MoveNext());
                Assert.AreSame(p1, enumerator.Current);

                enumerator.Reset();

                Assert.IsTrue(enumerator.MoveNext());
                Assert.AreSame(p1, enumerator.Current);
                Assert.IsTrue(enumerator.MoveNext());
                Assert.AreSame(p2, enumerator.Current);
                Assert.IsFalse(enumerator.MoveNext());
            }
        }

        [TestMethod]
        public void Enumerator_CurrentBeforeMoveNext_ThrowsInvalidOperationException() {
            _collection.Add(new Cloth("CL-0001", "Shirt", 30, "Cotton", 1));

            using (var enumerator = _collection.GetEnumerator()) {
                Assert.Throws<InvalidOperationException>(() => 
                    _ = enumerator.Current);
            }
        }

        [TestMethod]
        public void Enumerator_CurrentAfterEndOfCollection_ThrowsInvalidOperationException() {
            _collection.Add(new Cloth("CL-0001", "Shirt", 30, "Cotton", 1));

            using (var enumerator = _collection.GetEnumerator()) {
                Assert.IsTrue(enumerator.MoveNext());
                Assert.IsFalse(enumerator.MoveNext());
                Assert.Throws<InvalidOperationException>(() => 
                    _ = enumerator.Current);
            }
        }

        #endregion
    }
}
