using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Entities;
using OOP_Main.Repositories;
using System;

namespace OOP_UnitTests.Entities {
    [TestClass]
    public class SupplierTests {
        private Supplier _supplier;

        [TestInitialize]
        public void Setup() {
            _supplier = new Supplier(
                supplierId: 10,
                name: "TechSupply Corp",
                contactEmail: "contact@techsupply.com",
                phone: "+380501234567",
                rating: 4.5
            );
        }

        [TestCleanup]
        public void Cleanup() {
            _supplier = null;
        }

        #region Constructor & Properties Validation Tests

        [TestMethod]
        public void Constructor_ValidArguments_InitializesAllPropertiesAndIdentifiableId() {
            Assert.AreEqual(10, _supplier.SupplierId);
            Assert.AreEqual(10, ((IIdentifiable<int>)_supplier).Id);
            Assert.AreEqual("TechSupply Corp", _supplier.Name);
            Assert.AreEqual("contact@techsupply.com", _supplier.ContactEmail);
            Assert.AreEqual("+380501234567", _supplier.Phone);
            Assert.AreEqual(4.5, _supplier.Rating, 0.001);
            Assert.IsNotNull(_supplier.Catalog);
            Assert.IsEmpty(_supplier.Catalog);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("invalid-email")]
        [DataRow("user@")]
        public void ContactEmail_InvalidEmail_ThrowsArgumentException(string invalidEmail) {
            Assert.Throws<ArgumentException>(() =>
                _supplier.ContactEmail = invalidEmail);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("123")]
        [DataRow("not-a-phone")]
        public void Phone_InvalidPhone_ThrowsArgumentException(string invalidPhone) {
            Assert.Throws<ArgumentException>(() =>
                 _supplier.Phone = invalidPhone);
        }

        [TestMethod]
        [DataRow(0.9)]
        [DataRow(0.0)]
        [DataRow(-1.0)]
        [DataRow(5.1)]
        public void Rating_OutOfRangeValue_ThrowsArgumentOutOfRangeException(double invalidRating) {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                _supplier.Rating = invalidRating);
        }

        [TestMethod]
        [DataRow(1.0)]
        [DataRow(3.0)]
        [DataRow(5.0)]
        public void Rating_BoundaryValidValues_SetsRatingSuccessfully(double validRating) {
            _supplier.Rating = validRating;

            Assert.AreEqual(validRating, _supplier.Rating, 0.001);
        }

        #endregion

        #region Catalog Management Tests

        [TestMethod]
        public void AddProductToCatalog_ValidProduct_AddsProductToList() {
            var product = new ElectronicProduct("EL-0001", "Laptop", 1200, 65, 220, 10);

            _supplier.AddProductToCatalog(product);

            Assert.HasCount(1, _supplier.Catalog);
            Assert.AreSame(product, _supplier.Catalog[0]);
        }

        [TestMethod]
        public void AddProductToCatalog_NullProduct_DoesNotAddToList() {
            _supplier.AddProductToCatalog(null);

            Assert.IsEmpty(_supplier.Catalog);
        }

        [TestMethod]
        public void GetProductFromCatalogByName_ExistingAndNonExisting_ReturnsExpectedProduct() {
            var monitor = new ElectronicProduct("EL-0002", "Monitor 4K", 400, 45, 220, 10);
            _supplier.AddProductToCatalog(monitor);

            Product found = _supplier.GetProductFromCatalogByName("Monitor 4K");
            Product notFound = _supplier.GetProductFromCatalogByName("Keyboard");

            Assert.AreSame(monitor, found);
            Assert.IsNull(notFound);
        }

        [TestMethod]
        public void GetProductFromCatalogByArticle_ExistingAndNonExisting_ReturnsExpectedProduct() {
            var desk = new Furniture("FN-0001", "Office Desk", 300, 25, 140, 70, 75, "Oak", 10);
            _supplier.AddProductToCatalog(desk);

            Product found = _supplier.GetProductFromCatalogByArticle("FN-0001");
            Product notFound = _supplier.GetProductFromCatalogByArticle("FN-9999");

            Assert.AreSame(desk, found);
            Assert.IsNull(notFound);
        }

        [TestMethod]
        public void CanProvidePart_CaseInsensitiveMatch_ReturnsExpectedBoolean() {
            var shirt = new Cloth("CL-0001", "Cotton Shirt", 35, "Cotton", 10);
            _supplier.AddProductToCatalog(shirt);

            Assert.IsTrue(_supplier.CanProvidePart("cotton shirt"));
            Assert.IsTrue(_supplier.CanProvidePart("COTTON SHIRT"));
            Assert.IsFalse(_supplier.CanProvidePart("Silk Tie"));
        }

        #endregion

        #region Inspection, Equality & Formatting Tests

        [TestMethod]
        [DataRow(1.0, false)]
        [DataRow(2.99, false)]
        [DataRow(3.0, true)]
        [DataRow(5.0, true)]
        public void Inspect_VariousRatings_ReturnsTrueWhenRatingAtLeastThree(double rating, bool expectedResult) {
            _supplier.Rating = rating;

            Assert.AreEqual(expectedResult, _supplier.Inspect());
        }

        [TestMethod]
        public void EqualsAndGetHashCode_CompareBySupplierId() {
            var sameIdSupplier = new Supplier(10, "Other Name", "other@mail.com", "+380671112233", 2.0);
            var differentIdSupplier = new Supplier(20, "TechSupply Corp", "contact@techsupply.com", "+380501234567", 4.5);

            Assert.IsTrue(_supplier.Equals(sameIdSupplier));
            Assert.AreEqual(_supplier.GetHashCode(), sameIdSupplier.GetHashCode());
            Assert.IsFalse(_supplier.Equals(differentIdSupplier));
            Assert.IsFalse(_supplier.Equals(null));
        }

        [TestMethod]
        public void ToString_ReturnsFormattedSupplierString() {
            Assert.AreEqual("|Supplier| TechSupply Corp", _supplier.ToString());
        }

        #endregion
    }
}
