using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Decorators;
using OOP_Main.Entities;
using System;

namespace OOP_UnitTests.Decorators {
    [TestClass]
    public class ProductDecoratorsTests {
        private Product _baseProduct;

        [TestInitialize]
        public void Setup() {
            _baseProduct = new ElectronicProduct("EL-0001", "Laptop", 1000.0, 65, 220, 1);
        }

        [TestCleanup]
        public void Cleanup() {
            _baseProduct = null;
        }

        #region ProductDecorator Base & Null Handling Tests

        [TestMethod]
        public void DiscountDecorator_NullProductPassedToConstructor_ThrowsException() {
            Assert.Throws<NullReferenceException>(() =>
                new DiscountDecorator(null, 10));
        }

        [TestMethod]
        public void ProductDecorator_CalculateShippingCost_DelegatesToTargetProduct() {
            var decorated = new DiscountDecorator(_baseProduct, 15);

            double expectedShipping = _baseProduct.CalculateShippingCost();
            double actualShipping = decorated.CalculateShippingCost();

            Assert.AreEqual(expectedShipping, actualShipping, 0.001);
        }

        [TestMethod]
        public void ProductDecorator_WithoutTargetProduct_ReturnsDefaultFallbackValues() {
            var emptyDecorator = new DiscountDecorator {
                Article = "BDL-0001",
                Name = "Base Item",
                Price = 200.0,
                DiscountPercent = 20
            };

            Assert.IsFalse(emptyDecorator.TargetProduct.HasValue());
            Assert.AreEqual("BDL-0001", emptyDecorator.Article);
            Assert.AreEqual("Base Item", emptyDecorator.Name);
            Assert.AreEqual(200.0, emptyDecorator.Price, 0.001);
            Assert.AreEqual(0, emptyDecorator.CalculateShippingCost());
            Assert.AreEqual(0, emptyDecorator.GetProductPrice());
        }

        [TestMethod]
        public void ProductDecorator_GetNameWithoutTargetProduct_ThrowsException() {
            Assert.Throws<InvalidOperationException>(() =>
                new DiscountDecorator().GetName());
        }

        [TestMethod]
        public void ProductDecorator_EqualsAndGetHashCode_UseReferenceEquality() {
            var decorator1 = new DiscountDecorator(_baseProduct, 10);
            var decorator2 = new DiscountDecorator(_baseProduct, 10);

            Assert.IsTrue(decorator1.Equals(decorator1));
            Assert.IsFalse(decorator1.Equals(decorator2));
            Assert.AreNotEqual(decorator1.GetHashCode(), decorator2.GetHashCode());
        }

        #endregion

        #region DiscountDecorator Tests

        [TestMethod]
        public void DiscountDecorator_ValidProductAndDiscount_ModifiesPriceNameAndArticle() {
            var discounted = new DiscountDecorator(_baseProduct, 20);

            Assert.AreEqual(800.0, discounted.Price, 0.001);
            Assert.AreEqual("Laptop (20% OFF)", discounted.Name);
            Assert.AreEqual("DISC20-EL-0001", discounted.Article);
        }

        [TestMethod]
        public void DiscountDecorator_UpdateDiscountPercent_DynamicallyUpdatesProperties() {
            var discounted = new DiscountDecorator(_baseProduct, 10) {
                DiscountPercent = 25
            };

            Assert.AreEqual(25, discounted.DiscountPercent, 0.001);
            Assert.AreEqual(750.0, discounted.Price, 0.001);
            Assert.AreEqual("Laptop (25% OFF)", discounted.Name);
            Assert.AreEqual("DISC25-EL-0001", discounted.Article);
        }

        #endregion

        #region ExtendedWarrantyDecorator Tests

        [TestMethod]
        public void ExtendedWarrantyDecorator_DefaultOneYear_AddsTenPercentToPriceAndUpdatesMetadata() {
            var withWarranty = new ExtendedWarrantyDecorator(_baseProduct);

            Assert.AreEqual(1, withWarranty.Years);
            Assert.AreEqual(1100.0, withWarranty.Price, 0.001);
            Assert.AreEqual("Laptop (+1 yr Warranty)", withWarranty.Name);
            Assert.AreEqual("EXTWAR1-EL-0001", withWarranty.Article);
        }

        [TestMethod]
        public void ExtendedWarrantyDecorator_MultipleYears_CalculatesCorrectPriceAndMetadata() {
            var withWarranty = new ExtendedWarrantyDecorator(_baseProduct, 3);

            Assert.AreEqual(3, withWarranty.Years);
            Assert.AreEqual(1300.0, withWarranty.Price, 0.001);
            Assert.AreEqual("Laptop (+3 yr Warranty)", withWarranty.Name);
            Assert.AreEqual("EXTWAR3-EL-0001", withWarranty.Article);
        }

        [TestMethod]
        public void ExtendedWarrantyDecorator_SetInvalidYears_DefaultsToOneYear() {
            var withWarranty = new ExtendedWarrantyDecorator(_baseProduct, 2) {
                Years = 0
            };
            Assert.AreEqual(1, withWarranty.Years);

            withWarranty.Years = -5;
            Assert.AreEqual(1, withWarranty.Years);
        }

        #endregion

        #region Chained Decorators Tests

        [TestMethod]
        public void ChainedDecorators_WarrantyAppliedAfterDiscount_StacksModificationsCorrectly() {
            // 1. Базова ціна 1000 -> знижка 20% -> 800
            Product discounted = new DiscountDecorator(_baseProduct, 20);
            // 2. Гарантія на 2 роки (+20% від 800 = +160) -> 960
            Product discountedWithWarranty = new ExtendedWarrantyDecorator(discounted, 2);

            Assert.AreEqual(960.0, discountedWithWarranty.Price, 0.001);
            Assert.AreEqual("Laptop (20% OFF) (+2 yr Warranty)", discountedWithWarranty.Name);
            Assert.AreEqual("EXTWAR2-DISC20-EL-0001", discountedWithWarranty.Article);
            Assert.AreEqual(_baseProduct.CalculateShippingCost(), discountedWithWarranty.CalculateShippingCost(), 0.001);
        }

        #endregion
    }
}
