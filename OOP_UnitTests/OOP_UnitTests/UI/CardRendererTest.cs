using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main;
using OOP_Main.Bundles;
using OOP_Main.Decorators;
using OOP_Main.Entities;
using OOP_Main.UI;
using Spectre.Console;
using Spectre.Console.Rendering;
using Spectre.Console.Testing;
using System.Collections.Generic;
using System.Linq;

namespace OOP_UnitTests.UI {
    [TestClass]
    public class CardRendererTests {
        private string RenderCardToString(Dictionary<string, IRenderable> card) {
            var testConsole = new TestConsole();
            testConsole.Write(new Rows(card.Values.ToList()));
            return testConsole.Output;
        }

        #region Default / Unknown Object Tests

        [TestMethod]
        public void GetCardInfo_NullItem_ReturnsEmptyHeaderCard() {
            var card = CardRenderer.GetCardInfo(null);

            Assert.HasCount(1, card);
            Assert.IsTrue(card.ContainsKey("header"));
            Assert.AreEqual(string.Empty, RenderCardToString(card).Trim());
        }

        [TestMethod]
        public void GetCardInfo_UnknownObject_ReturnsToStringInHeader() {
            var card = CardRenderer.GetCardInfo(12345);

            Assert.HasCount(1, card);
            Assert.Contains("12345", RenderCardToString(card));
        }

        #endregion

        #region User Card Tests

        [TestMethod]
        public void GetCardInfo_User_ReturnsFormattedUserCard() {
            var user = new User(7, "AliceAdmin", "pass1234", startSum: 450.50, isAdmin: true);

            var card = CardRenderer.GetCardInfo(user);
            string output = RenderCardToString(card);

            Assert.IsTrue(card.ContainsKey("header"));
            Assert.IsTrue(card.ContainsKey("id"));
            Assert.IsTrue(card.ContainsKey("role"));
            Assert.IsTrue(card.ContainsKey("account"));

            Assert.Contains("AliceAdmin", output);
            Assert.Contains("ID: 7", output);
            Assert.Contains("Role: Administrator", output);
            Assert.Contains("Total balance:", output);
        }

        #endregion

        #region Individual Product Types Card Tests

        [TestMethod]
        public void GetCardInfo_ElectronicProduct_IncludesPowerAndVoltage() {
            var laptop = new ElectronicProduct("EL-0001", "Gaming Laptop", 1500.0, power: 180, maxVoltage: 220, supplierId: 1);

            var card = CardRenderer.GetCardInfo(laptop);
            string output = RenderCardToString(card);

            Assert.IsTrue(card.ContainsKey("power"));
            Assert.IsTrue(card.ContainsKey("voltage"));
            Assert.Contains("ELECTRONIC PRODUCT \"Gaming Laptop\"", output);
            Assert.Contains("Article: EL-0001", output);
            Assert.Contains("Power: 180 W", output);
            Assert.Contains("Max Voltage: 220 V", output);
        }

        [TestMethod]
        public void GetCardInfo_Cloth_IncludesMaterial() {
            var shirt = new Cloth("CL-0001", "Silk Shirt", 85.0, material: "Pure Silk", supplierId: 2);

            var card = CardRenderer.GetCardInfo(shirt);
            string output = RenderCardToString(card);

            Assert.IsTrue(card.ContainsKey("material"));
            Assert.Contains("CLOTH \"Silk Shirt\"", output);
            Assert.Contains("Material: Pure Silk", output);
        }

        [TestMethod]
        public void GetCardInfo_Furniture_IncludesDimensionsWeightAndMaterial() {
            var table = new Furniture("FN-0001", "Oak Table", 320.0, weight: 25.5, width: 80, length: 160, height: 75, material: "Oak");

            var card = CardRenderer.GetCardInfo(table);
            string output = RenderCardToString(card);

            Assert.IsTrue(card.ContainsKey("dimensions"));
            Assert.IsTrue(card.ContainsKey("weight"));
            Assert.IsTrue(card.ContainsKey("material"));
            Assert.IsFalse(card.ContainsKey("assemble"));

            Assert.Contains("FURNITURE \"Oak Table\"", output);
            Assert.Contains("Dimensions: 160x80x75 cm", output);
            Assert.Contains("Weight: 25,5 kg", output);
            Assert.Contains("Material: Oak", output);
        }

        [TestMethod]
        public void GetCardInfo_Sofa_IncludesAssembleStatusAlongWithDimensions() {
            var sofa = new Sofa("SF-0001", "Corner Sofa", 950.0, weight: 55.0, width: 110, length: 230, height: 85, material: "Velvet", isAssemble: true);

            var card = CardRenderer.GetCardInfo(sofa);
            string output = RenderCardToString(card);

            Assert.IsTrue(card.ContainsKey("assemble"));
            Assert.Contains("SOFA \"Corner Sofa\"", output);
            Assert.Contains("Dimensions: 230x110x85 cm", output);
            Assert.Contains("Is Assembled: True", output);
        }

        #endregion

        #region Order Card Tests

        [TestMethod]
        public void GetCardInfo_OrderWithEmptyProducts_IncludesEmptyWarning() {
            var order = new Order(orderId: 55, buyerId: 3);

            var card = CardRenderer.GetCardInfo(order);
            string output = RenderCardToString(card);

            Assert.IsTrue(card.ContainsKey("empty_products"));
            Assert.Contains("ORDER #55", output);
            Assert.Contains("Buyer ID: 3", output);
            Assert.Contains("Status: Pending", output);
            Assert.Contains("(No products in this order)", output);
        }

        [TestMethod]
        public void GetCardInfo_OrderWithProducts_RendersStatisticsAndProductItems() {
            var order = new Order(orderId: 56, buyerId: 4);
            order.AddProduct(new ElectronicProduct("EL-0001", "Monitor", 400.0, 45, 220, 1));
            order.AddProduct(new Cloth("CL-0001", "Scarf", 20.0, "Wool", 2));

            var card = CardRenderer.GetCardInfo(order);
            string output = RenderCardToString(card);

            Assert.IsFalse(card.ContainsKey("empty_products"));
            Assert.IsTrue(card.ContainsKey("prod_0_name"));
            Assert.IsTrue(card.ContainsKey("prod_0_power"));
            Assert.IsTrue(card.ContainsKey("prod_1_name"));
            Assert.IsTrue(card.ContainsKey("prod_1_material"));

            Assert.Contains("1. [EL-0001] Monitor", output);
            Assert.Contains("2. [CL-0001] Scarf", output);
        }

        #endregion

        #region ProductBundle Card Tests

        [TestMethod]
        public void GetCardInfo_EmptyProductBundle_IncludesEmptyBundleWarning() {
            var bundle = new ProductBundle("BDL-0001", "Starter Bundle", supplierId: 1, discountPercent: 15);

            var card = CardRenderer.GetCardInfo(bundle);
            string output = RenderCardToString(card);

            Assert.IsTrue(card.ContainsKey("empty_products"));
            Assert.Contains("PRODUCT BUNDLE \"Starter Bundle\"", output);
            Assert.Contains("Dicsount: -15%", output);
            Assert.Contains("(No products in this bundle)", output);
        }

        [TestMethod]
        public void GetCardInfo_ProductBundleWithItems_RendersBundleItems() {
            var bundle = new ProductBundle("BDL-0002", "Work Set", supplierId: 1, discountPercent: 10);
            bundle.AddProduct(new Cloth("CL-0005", "Desk Pad", 30.0, "Leather", 1));

            var card = CardRenderer.GetCardInfo(bundle);
            string output = RenderCardToString(card);

            Assert.IsTrue(card.ContainsKey("prod_0_name"));
            Assert.IsTrue(card.ContainsKey("prod_0_material"));
            Assert.Contains("1. [CL-0005] Desk Pad", output);
        }

        #endregion

        #region ProductDecorator Card Tests

        [TestMethod]
        public void GetCardInfo_EmptyProductDecorator_IncludesNoProductMessage() {
            var emptyDecorator = new DiscountDecorator {
                Article = "DISC10-FN-0001",
                Name = "Empty Promo",
                Price = 100.0,
                DiscountPercent = 10
            };

            var card = CardRenderer.GetCardInfo(emptyDecorator);
            string output = RenderCardToString(card);

            Assert.IsTrue(card.ContainsKey("empty_product"));
            Assert.Contains("(No product in this decorator)", output);
        }

        [TestMethod]
        public void GetCardInfo_DecoratedProduct_RendersDecoratorAndInnerProductSpecs() {
            var basePhone = new ElectronicProduct("EL-0010", "Smartphone", 800.0, 20, 5, 1);
            var discountedPhone = new DiscountDecorator(basePhone, 25);

            var card = CardRenderer.GetCardInfo(discountedPhone);
            string output = RenderCardToString(card);

            Assert.IsTrue(card.ContainsKey("prod_name"));
            Assert.IsTrue(card.ContainsKey("prod_power"));
            Assert.Contains("DISCOUNT DECORATOR \"Smartphone (25% OFF)\"", output);
            Assert.Contains("Article: DISC25-EL-0010", output);
            Assert.Contains("[EL-0010] Smartphone", output);
            Assert.Contains("Power: 20 W", output);
        }

        #endregion

        #region Supplier Card Tests

        [TestMethod]
        public void GetCardInfo_SupplierWithBadReputationAndEmptyCatalog_RendersWarningAndBadReputation() {
            var supplier = new Supplier(12, "Unreliable Supply", "bad@supply.com", "+380501112233", rating: 2.0);

            var card = CardRenderer.GetCardInfo(supplier);
            string output = RenderCardToString(card);

            Assert.IsTrue(card.ContainsKey("empty_products"));
            Assert.Contains("SUPPLIER \"Unreliable Supply\"", output);
            Assert.Contains("Rating: 2/5.0", output);
            Assert.Contains("Reputation: Bad", output);
            Assert.Contains("(No products that supplier can provide)", output);
        }

        [TestMethod]
        public void GetCardInfo_SupplierWithGoodReputationAndCatalog_RendersGoodReputationAndProducts() {
            var supplier = new Supplier(15, "Top Supplier", "top@supply.com", "+380671234567", rating: 4.8);
            supplier.AddProductToCatalog(new Cloth("CL-0099", "Linen Suit", 200.0, "Linen", 15));
            var card = CardRenderer.GetCardInfo(supplier);
            string output = RenderCardToString(card);
            Assert.IsTrue(card.ContainsKey("prod_0_name"));
            Assert.IsTrue(card.ContainsKey("prod_0_material"));
            Assert.Contains("Reputation: Good", output);
            Assert.Contains("1. [CL-0099] Linen Suit", output);
        }
        #endregion
    }
}