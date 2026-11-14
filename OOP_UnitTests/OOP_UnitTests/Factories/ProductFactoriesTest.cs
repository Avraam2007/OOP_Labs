using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Bundles;
using OOP_Main.Entities;
using OOP_Main.Factories;
using Spectre.Console;
using Spectre.Console.Testing;

namespace OOP_UnitTests.Factories {
    [TestClass]
    public class ProductFactoriesTests {
        private IAnsiConsole _originalConsole;
        private TestConsole _testConsole;

        [TestInitialize]
        public void Setup() {
            _originalConsole = AnsiConsole.Console;
            _testConsole = new TestConsole();
            _testConsole.Profile.Capabilities.Interactive = true;
            AnsiConsole.Console = _testConsole;
        }

        [TestCleanup]
        public void Cleanup() {
            AnsiConsole.Console = _originalConsole;
        }

        #region BundleFactory Tests

        [TestMethod]
        public void BundleFactory_CreateProduct_ReturnsProductBundleWithEnteredDiscount() {
            _testConsole.Input.PushTextWithEnter("15.5");
            ProductFactory factory = new BundleFactory();

            Product result = factory.CreateProduct("BDL-0001", "Office Bundle", 500, 10);

            Assert.IsInstanceOfType(result, typeof(ProductBundle));
            var bundle = (ProductBundle)result;
            Assert.AreEqual("BDL-0001", bundle.Article);
            Assert.AreEqual("Office Bundle", bundle.Name);
            Assert.AreEqual(10, bundle.SupplierId);
            Assert.AreEqual(15.5, bundle.BundleDiscountPercent, 0.001);
        }

        #endregion

        #region ClothFactory Tests

        [TestMethod]
        public void ClothFactory_CreateProduct_ReturnsClothWithEnteredMaterial() {
            _testConsole.Input.PushTextWithEnter("Wool");
            ProductFactory factory = new ClothFactory();

            Product result = factory.CreateProduct("CL-0001", "Winter Coat", 120, 5);

            Assert.IsInstanceOfType(result, typeof(Cloth));
            var cloth = (Cloth)result;
            Assert.AreEqual("CL-0001", cloth.Article);
            Assert.AreEqual("Winter Coat", cloth.Name);
            Assert.AreEqual(120, cloth.Price);
            Assert.AreEqual(5, cloth.SupplierId);
            Assert.AreEqual("Wool", cloth.Material);
        }

        #endregion

        #region ElectronicProductFactory Tests

        [TestMethod]
        public void ElectronicProductFactory_CreateProduct_ReturnsElectronicProductWithEnteredSpecs() {
            _testConsole.Input.PushTextWithEnter("750");
            _testConsole.Input.PushTextWithEnter("230");
            ProductFactory factory = new ElectronicProductFactory();

            Product result = factory.CreateProduct("EL-0001", "Microwave", 200, 3);

            Assert.IsInstanceOfType(result, typeof(ElectronicProduct));
            var electronic = (ElectronicProduct)result;
            Assert.AreEqual("EL-0001", electronic.Article);
            Assert.AreEqual("Microwave", electronic.Name);
            Assert.AreEqual(200, electronic.Price);
            Assert.AreEqual(3, electronic.SupplierId);
            Assert.AreEqual(750, electronic.Power);
            Assert.AreEqual(230, electronic.MaxVoltage);
        }

        #endregion

        #region FurnitureFactory Tests

        [TestMethod]
        public void FurnitureFactory_CreateProductWithNonSofaCategory_ReturnsFurniture() {
            _testConsole.Input.PushTextWithEnter("25.5");
            _testConsole.Input.PushTextWithEnter("80");
            _testConsole.Input.PushTextWithEnter("160");
            _testConsole.Input.PushTextWithEnter("75");
            _testConsole.Input.PushTextWithEnter("Oak");
            ProductFactory factory = new FurnitureFactory();

            Product result = factory.CreateProduct("FN-0001", "Dining Table", 350, 7, "Table");

            Assert.IsInstanceOfType(result, typeof(Furniture));
            Assert.IsNotInstanceOfType(result, typeof(Sofa));
            var furniture = (Furniture)result;
            Assert.AreEqual("FN-0001", furniture.Article);
            Assert.AreEqual("Dining Table", furniture.Name);
            Assert.AreEqual(350, furniture.Price);
            Assert.AreEqual(7, furniture.SupplierId);
            Assert.AreEqual(25.5, furniture.Weight, 0.001);
            Assert.AreEqual(80, furniture.Width, 0.001);
            Assert.AreEqual(160, furniture.Length, 0.001);
            Assert.AreEqual(75, furniture.Height, 0.001);
            Assert.AreEqual("Oak", furniture.Material);
        }

        [TestMethod]
        public void FurnitureFactory_CreateProductWithSofaCategory_ReturnsSofa() {
            _testConsole.Input.PushTextWithEnter("45");
            _testConsole.Input.PushTextWithEnter("95");
            _testConsole.Input.PushTextWithEnter("210");
            _testConsole.Input.PushTextWithEnter("85");
            _testConsole.Input.PushTextWithEnter("Velvet");
            _testConsole.Input.PushTextWithEnter("y");
            ProductFactory factory = new FurnitureFactory();

            Product result = factory.CreateProduct("SF-0001", "Corner Sofa", 800, 8, "Sofa");

            Assert.IsInstanceOfType(result, typeof(Sofa));
            var sofa = (Sofa)result;
            Assert.AreEqual("SF-0001", sofa.Article);
            Assert.AreEqual("Corner Sofa", sofa.Name);
            Assert.AreEqual(800, sofa.Price);
            Assert.AreEqual(8, sofa.SupplierId);
            Assert.AreEqual(45, sofa.Weight, 0.001);
            Assert.AreEqual(95, sofa.Width, 0.001);
            Assert.AreEqual(210, sofa.Length, 0.001);
            Assert.AreEqual(85, sofa.Height, 0.001);
            Assert.AreEqual("Velvet", sofa.Material);
            Assert.IsTrue(sofa.IsAssemble);
        }

        #endregion

        #region ProductRestorer Tests

        [TestMethod]
        public void ProductRestorer_ElectronicArticlePrefix_ReturnsElectronicProductWithDefaults() {
            Product product = ProductRestorer.CreateProductByArticle("EL-1234", "Monitor", 300, 1);

            Assert.IsInstanceOfType(product, typeof(ElectronicProduct));
            var electronic = (ElectronicProduct)product;
            Assert.AreEqual(100, electronic.Power);
            Assert.AreEqual(220, electronic.MaxVoltage);
        }

        [TestMethod]
        public void ProductRestorer_FurnitureArticlePrefix_ReturnsFurnitureWithDefaults() {
            Product product = ProductRestorer.CreateProductByArticle("FN-1234", "Chair", 90, 2);

            Assert.IsInstanceOfType(product, typeof(Furniture));
            Assert.IsNotInstanceOfType(product, typeof(Sofa));
            var furniture = (Furniture)product;
            Assert.AreEqual(10, furniture.Weight);
            Assert.AreEqual(50, furniture.Width);
            Assert.AreEqual(50, furniture.Length);
            Assert.AreEqual(80, furniture.Height);
            Assert.AreEqual("Wood", furniture.Material);
        }

        [TestMethod]
        public void ProductRestorer_SofaArticlePrefix_ReturnsSofaWithDefaults() {
            Product product = ProductRestorer.CreateProductByArticle("SF-1234", "Lounge Sofa", 600, 3);

            Assert.IsInstanceOfType(product, typeof(Sofa));
            var sofa = (Sofa)product;
            Assert.AreEqual(30, sofa.Weight);
            Assert.AreEqual(100, sofa.Width);
            Assert.AreEqual(200, sofa.Length);
            Assert.AreEqual(90, sofa.Height);
            Assert.AreEqual("Fabric", sofa.Material);
            Assert.IsTrue(sofa.IsAssemble);
        }

        [TestMethod]
        public void ProductRestorer_BundleArticlePrefix_ReturnsProductBundleWithDefaultDiscount() {
            Product product = ProductRestorer.CreateProductByArticle("BDL-1234", "Starter Kit", 400, 4);

            Assert.IsInstanceOfType(product, typeof(ProductBundle));
            var bundle = (ProductBundle)product;
            Assert.AreEqual(10, bundle.BundleDiscountPercent);
        }

        [TestMethod]
        public void ProductRestorer_UnknownOrClothPrefix_ReturnsClothWithDefaultMaterial() {
            Product product = ProductRestorer.CreateProductByArticle("CL-1234", "T-Shirt", 25, 5);

            Assert.IsInstanceOfType(product, typeof(Cloth));
            var cloth = (Cloth)product;
            Assert.AreEqual("Cotton", cloth.Material);
        }

        #endregion
    }
}
