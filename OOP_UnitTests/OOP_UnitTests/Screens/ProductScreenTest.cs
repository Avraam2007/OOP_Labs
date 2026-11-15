using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Bundles;
using OOP_Main.Decorators;
using OOP_Main.Entities;
using OOP_Main.Repositories;
using OOP_Main.Screens;
using Spectre.Console;
using Spectre.Console.Testing;
using System;
using System.IO;
using System.Linq;

namespace OOP_UnitTests.Screens {
    [TestClass]
    public class ProductScreenTests {
        private string _originalWorkingDir;
        private string _tempWorkingDir;
        private IAnsiConsole _originalConsole;
        private TestConsole _testConsole;
        private Data _appData;
        private ProductScreen _productScreen;

        [TestInitialize]
        public void Setup() {
            _originalWorkingDir = Directory.GetCurrentDirectory();
            _tempWorkingDir = Path.Combine(Path.GetTempPath(), "OOP_ProductScreenTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempWorkingDir);
            Directory.SetCurrentDirectory(_tempWorkingDir);

            _originalConsole = AnsiConsole.Console;
            _testConsole = new TestConsole();
            _testConsole.Profile.Capabilities.Interactive = true;
            _testConsole.Profile.Width = 200;
            AnsiConsole.Console = _testConsole;

            _appData = new Data();
            _productScreen = new ProductScreen(_appData);
        }

        [TestCleanup]
        public void Cleanup() {
            AnsiConsole.Console = _originalConsole;
            Directory.SetCurrentDirectory(_originalWorkingDir);

            if (Directory.Exists(_tempWorkingDir)) {
                Directory.Delete(_tempWorkingDir, recursive: true);
            }
        }

        #region Show & Sorting Tests

        [TestMethod]
        public void Show_SortByPriceAscending_RendersCheapestProductFirst() {
            _appData.Products.Add(new ElectronicProduct("EL-0001", "Laptop", 1200.0, 65, 220, 1));
            _appData.Products.Add(new Cloth("CL-0001", "Socks", 10.0, "Cotton", 1));

            // 1. Вибір сортування: "By price (from the cheapest)" (DownArrow -> Enter)
            // 2. Вихід зі списку ShowListScreen: "Back to menu" (Enter)
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _productScreen.Show();

            Assert.IsLessThan(_testConsole.Output.IndexOf("Laptop"), _testConsole.Output.IndexOf("Socks"));
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void Show_SortByNameAlphabetically_RendersAlphabeticallyFirstProductFirst() {
            _appData.Products.Add(new Cloth("CL-0001", "Zebra Scarf", 30.0, "Silk", 1));
            _appData.Products.Add(new Cloth("CL-0002", "Apron", 20.0, "Linen", 1));

            // 1. Вибір сортування: "By name (A-Z)" (2x DownArrow -> Enter)
            // 2. Вихід зі списку ShowListScreen: "Back to menu" (Enter)
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _productScreen.Show();

            Assert.IsLessThan(_testConsole.Output.IndexOf("Zebra Scarf"), _testConsole.Output.IndexOf("Apron"));
            Assert.AreEqual("Back to menu", result);
        }

        #endregion

        #region Create Product Tests

        [TestMethod]
        public void Create_NoSuppliersInStore_OutputsErrorAndReturnsBackPrompt() {
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _productScreen.Create();

            Assert.Contains("No suppliers found. Call the admin to fix this issue", _testConsole.Output);
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void Create_ClothWithDiscountDecorator_AddsProductToStoreAndSupplierCatalog() {
            var supplier = new Supplier(1, "FashionCorp", "info@fashion.com", "+380501112233", 4.5);
            _appData.Suppliers.Add(supplier);

            // Категорії з групами у SelectionPrompt:
            // [0] Група "Electronic product" (не обирається)
            // [1] Елемент "Electronic product" (курсор тут)
            // [2] Група "Cloth"
            // [3] Елемент "Cloth" (1x DownArrow)
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            _testConsole.Input.PushTextWithEnter("Winter Coat"); // Назва
            _testConsole.Input.PushTextWithEnter("200");         // Ціна
            _testConsole.Input.PushTextWithEnter("y");           // Знижка 20%
            _testConsole.Input.PushTextWithEnter("n");           // Без гарантії
            _testConsole.Input.PushKey(ConsoleKey.Enter);        // Вибір постачальника "FashionCorp"
            _testConsole.Input.PushTextWithEnter("Wool");        // Матеріал у ClothFactory
            _testConsole.Input.PushKey(ConsoleKey.Enter);        // BackToMenuPrompt

            string result = _productScreen.Create();

            Assert.HasCount(1, _appData.Products);
            Assert.HasCount(1, supplier.Catalog);

            Product added = _appData.Products.First();
            Assert.IsInstanceOfType(added, typeof(DiscountDecorator));
            Assert.AreEqual(160.0, added.Price, 0.001); // 200 - 20% = 160
            Assert.Contains("New product is created!", _testConsole.Output);
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void Create_ElectronicProductWithWarranty_AddsDecoratedElectronicProduct() {
            var supplier = new Supplier(2, "ElectroHub", "sales@electro.com", "+380671112233", 4.8);
            _appData.Suppliers.Add(supplier);

            // [1] Елемент "Electronic product" (Enter)
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushTextWithEnter("Gaming Monitor"); // Назва
            _testConsole.Input.PushTextWithEnter("500");            // Ціна
            _testConsole.Input.PushTextWithEnter("n");              // Без знижки
            _testConsole.Input.PushTextWithEnter("y");              // Розширена гарантія (+10%)
            _testConsole.Input.PushKey(ConsoleKey.Enter);           // Постачальник "ElectroHub"
            _testConsole.Input.PushTextWithEnter("65");             // Потужність (Вт)
            _testConsole.Input.PushTextWithEnter("220");            // Макс. напруга (В)
            _testConsole.Input.PushKey(ConsoleKey.Enter);           // BackToMenuPrompt

            string result = _productScreen.Create();

            Assert.HasCount(1, _appData.Products);
            Product added = _appData.Products.First();
            Assert.IsInstanceOfType(added, typeof(ExtendedWarrantyDecorator));
            Assert.AreEqual(550.0, added.Price, 0.001); // 500 + 10% = 550
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void Create_BundleWhenStoreHasNoProducts_CatchesECommerceExceptionAndOutputsError() {
            var supplier = new Supplier(3, "BundleSupply", "b@supply.com", "+380931112233", 4.0);
            _appData.Suppliers.Add(supplier);

            // Список вибору категорій:
            // [1] "Electronic product" -> [3] "Cloth" -> [5] "Sofa" -> [6] "Other furniture" -> [7] "Bundle" (4x DownArrow)
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            _testConsole.Input.PushTextWithEnter("Starter Set"); // Назва набору
            _testConsole.Input.PushTextWithEnter("300");         // Ціна
            _testConsole.Input.PushTextWithEnter("n");           // Без знижки
            _testConsole.Input.PushTextWithEnter("n");           // Без гарантії
            _testConsole.Input.PushKey(ConsoleKey.Enter);        // Постачальник
            _testConsole.Input.PushTextWithEnter("15");          // Відсоток знижки у BundleFactory
            _testConsole.Input.PushKey(ConsoleKey.Enter);        // BackToMenuPrompt після винятку FillBundle

            string result = _productScreen.Create();

            Assert.Contains("No products found. Call the admin to fix this issue.", _testConsole.Output);
            Assert.IsEmpty(_appData.Products);
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void Create_BundleWithExistingProducts_FillsBundleAndAddsToCatalog() {
            var supplier = new Supplier(3, "BundleSupply", "b@supply.com", "+380931112233", 4.0);
            _appData.Suppliers.Add(supplier);
            _appData.Products.Add(new Cloth("CL-0001", "Mousepad", 20.0, "Rubber", 3));

            // Обираємо "Bundle" (4x DownArrow -> Enter)
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            _testConsole.Input.PushTextWithEnter("Gamer Pack");  // Назва набору
            _testConsole.Input.PushTextWithEnter("100");         // Ціна
            _testConsole.Input.PushTextWithEnter("n");           // Без знижки декоратора
            _testConsole.Input.PushTextWithEnter("n");           // Без гарантії
            _testConsole.Input.PushKey(ConsoleKey.Enter);        // Постачальник
            _testConsole.Input.PushTextWithEnter("10");          // Знижка набору 10%
            _testConsole.Input.PushKey(ConsoleKey.Enter);        // У FillBundle обираємо "Mousepad"
            _testConsole.Input.PushKey(ConsoleKey.Enter);        // У FillBundle обираємо "Exit"
            _testConsole.Input.PushKey(ConsoleKey.Enter);        // BackToMenuPrompt
            string result = _productScreen.Create();
            Assert.HasCount(2, _appData.Products);
            var createdBundle = _appData.Products.OfType<ProductBundle>().FirstOrDefault();
            Assert.IsNotNull(createdBundle);
            Assert.HasCount(1, createdBundle.Items);
            Assert.AreEqual("Back to menu", result);
        }
        #endregion
        #region Search & FilterByCategory Tests
        [TestMethod]
        public void Search_FastFilterWithPriceRange_RendersMatchingProductsTable() {
            _appData.Products.Add(new Cloth("CL-0001", "Cheap Hat", 15.0, "Cotton", 1));
            _appData.Products.Add(new Cloth("CL-0002", "Mid Jacket", 120.0, "Denim", 1));
            _appData.Products.Add(new ElectronicProduct("EL-0001", "Expensive TV", 1500.0, 200, 220, 1));
            // 1. "Fast filter" (Enter)
            // 2. maxPrice = "200"
            // 3. minPrice = "50"
            // 4. BackToMenuPrompt (Enter)
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushTextWithEnter("200");
            _testConsole.Input.PushTextWithEnter("50");
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            string result = _productScreen.Search();
            Assert.Contains("Found products: 1", _testConsole.Output);
            Assert.Contains("Mid Jacket", _testConsole.Output);
            Assert.DoesNotContain("Cheap Hat", _testConsole.Output);
            Assert.DoesNotContain("Expensive TV", _testConsole.Output);
            Assert.AreEqual("Back to menu", result);
        }
        [TestMethod]
        public void Search_ThoroughSearchNoMatches_OutputsNotFoundError() {
            _appData.Products.Add(new Cloth("CL-0001", "Cotton Shirt", 40.0, "Cotton", 1));
            // 1. "Thorough search" (DownArrow -> Enter)
            // 2. maxPrice = Enter (default 10000.0)
            // 3. minPrice = Enter (default 0)
            // 4. query = "Submarine"
            // 5. BackToMenuPrompt (Enter)
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushTextWithEnter("Submarine");
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            string result = _productScreen.Search();
            Assert.Contains("No products found for your search.", _testConsole.Output);
            Assert.AreEqual("Back to menu", result);
        }
        [TestMethod]
        public void FilterByCategory_EmptyStore_CatchesNullReferenceExceptionAndReturnsBackPrompt() {
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            string result = _productScreen.FilterByCategory();
            Assert.AreEqual("Back to menu", result);
        }
        [TestMethod]
        public void FilterByCategory_ExistingCategories_RendersOnlySelectedCategoryProducts() {
            _appData.Products.Add(new Cloth("CL-0001", "Linen Pants", 60.0, "Linen", 1));
            _appData.Products.Add(new ElectronicProduct("EL-0001", "Wireless Mouse", 35.0, 5, 5, 1));
            // Категорії: ["Cloth", "ElectronicProduct"] -> обираємо першу ("Cloth") -> BackToMenuPrompt
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            string result = _productScreen.FilterByCategory();
            Assert.Contains("Found products: 1", _testConsole.Output);
            Assert.Contains("Linen Pants", _testConsole.Output);
            Assert.DoesNotContain("Wireless Mouse", _testConsole.Output);
            Assert.AreEqual("Back to menu", result);
        }
        #endregion
        #region Delete & Edit Tests
        [TestMethod]
        public void Delete_ExistingProductConfirmed_RemovesProductFromStore() {
            _appData.Products.Add(new Cloth("CL-0001", "Removable Shirt", 25.0, "Cotton", 1));
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushTextWithEnter("y");
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            string result = _productScreen.Delete();
            Assert.IsEmpty(_appData.Products);
            Assert.Contains("Product successfully deleted!", _testConsole.Output);
            Assert.AreEqual("Back to menu", result);
        }
        [TestMethod]
        public void Edit_ExistingProduct_UpdatesFieldAndSavesData() {
            var cloth = new Cloth("CL-0001", "Old Name", 25.0, "Cotton", 1);
            _appData.Products.Add(cloth);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushTextWithEnter("Updated Name");
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            string result = _productScreen.Edit();
            Assert.AreEqual("Updated Name", cloth.Name);
            Assert.AreEqual("Back to menu", result);
        }
        #endregion
    }
}