using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Entities;
using OOP_Main.Repositories;

namespace OOP_UnitTests.Repositories {
    [TestClass]
    public class SupplierDataTest {
        private SupplierData _supplierData;

        [TestInitialize]
        public void Setup() {
            _supplierData = new SupplierData();
        }

        [TestCleanup]
        public void Cleanup() {
            _supplierData = null;
        }

        #region Add Tests

        [TestMethod]
        public void AddSupplier_StandardSupplier_AddsToList() {
            var supplier = new Supplier(1, "Suppplier 1", "ad@frem.com", "0589634589", 4.6);

            _supplierData.AddSupplier(supplier);

            Assert.AreEqual(1, _supplierData.Suppliers.Count);
        }

        [TestMethod]
        public void AddSupplier_SupplierBuilder_AddsToListAndDefinesNewID() {
            _supplierData.AddSupplier("Suppplier 1", "ad@frem.com", "0589634589", 4.6);
            _supplierData.AddSupplier("Suppplier 2", "def@dert.com", "0892345678", 3.2);

            Assert.AreEqual(2, _supplierData.Suppliers.Count);
            Assert.AreEqual(1, _supplierData.Suppliers[0].SupplierId);
            Assert.AreEqual(2, _supplierData.Suppliers[1].SupplierId);
        }

        #endregion

        #region Get & Delete Tests

        [TestMethod]
        public void GetSupplierByIdAndName_ExistingSupplier_ReturnsSupplier() {
            var testSupplier = new Supplier(1, "Suppplier 1", "ad@frem.com", "0589634589", 4.6);
            _supplierData.AddSupplier(testSupplier);

            var byId = _supplierData.GetSupplierById(1);
            var byName = _supplierData.GetSupplierByName("Suppplier 1");

            Assert.IsTrue(byId.HasValue());
            Assert.AreEqual("Suppplier 1", byId.Value.Name);

            Assert.IsTrue(byName.HasValue());
            Assert.AreEqual(1, byName.Value.SupplierId);
        }

        [TestMethod]
        public void GetSupplierByIdAndName_NonExistingSupplier_ReturnsEmptyObject() {
            var byName = _supplierData.GetSupplierByName("Suppplier 999");
            var byId = _supplierData.GetSupplierById(999);

            Assert.IsTrue(byId.IsEmpty());
            Assert.IsTrue(byName.IsEmpty());
        }

        [TestMethod]
        public void DeleteSupplierByName_ExistingSupplier_RemovesFromList() {
            _supplierData.AddSupplier(new Supplier(5, "Supppliers Inc.", "gfd@teds.edu", "0458965678", 3.7));

            bool isDeleted = _supplierData.DeleteSupplierByName("Supppliers Inc.");

            Assert.IsTrue(isDeleted);
            Assert.AreEqual(0, _supplierData.Suppliers.Count);
        }

        [TestMethod]
        public void DeleteSupplierByName_NonExistentSupplier_ReturnsFalse() {
            bool isDeleted = _supplierData.DeleteSupplierByName("NonExistent");

            Assert.IsFalse(isDeleted);
        }

        [TestMethod]
        public void DeleteSupplierById_ExistingSupplier_RemovesSupplier() {
            _supplierData.AddSupplier(new Supplier(3, "Defs Inc.", "cuddle@var.org", "0157989545", 2.1));

            bool isDeleted = _supplierData.DeleteSupplierById(3);

            Assert.IsTrue(isDeleted);
            Assert.AreEqual(0, _supplierData.Suppliers.Count);
        }

        [TestMethod]
        public void DeleteSupplierId_NonExistentId_ThrowsExceptionOrReturnsFalse() {
            bool isDeleted = _supplierData.DeleteSupplierById(999);

            Assert.IsFalse(isDeleted);
        }

        #endregion
    }
}
