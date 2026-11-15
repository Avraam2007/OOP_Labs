using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class SupplierScreenTests {
        private string _originalWorkingDir;
        private string _tempWorkingDir;
        private IAnsiConsole _originalConsole;
        private TestConsole _testConsole;
        private Data _appData;
        private SupplierScreen _supplierScreen;

        [TestInitialize]
        public void Setup() {
            _originalWorkingDir = Directory.GetCurrentDirectory();
            _tempWorkingDir = Path.Combine(Path.GetTempPath(), "OOP_SupplierScreenTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempWorkingDir);
            Directory.SetCurrentDirectory(_tempWorkingDir);

            _originalConsole = AnsiConsole.Console;
            _testConsole = new TestConsole();
            _testConsole.Profile.Capabilities.Interactive = true;
            _testConsole.Profile.Width = 200;
            AnsiConsole.Console = _testConsole;

            _appData = new Data();
            _supplierScreen = new SupplierScreen(_appData);
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
        public void Show_SortByIdDefault_RendersSuppliersOrderedByIdAscending() {
            _appData.Suppliers.Add(new Supplier(20, "SecondSupply", "s2@mail.com", "+380501112233", 4.0));
            _appData.Suppliers.Add(new Supplier(5, "FirstSupply", "s1@mail.com", "+380502223344", 3.5));

            // 1. "By ID (default)" (Enter)
            // 2. Вихід зі списку ShowListScreen: "Back to menu" (Enter)
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _supplierScreen.Show();

            Assert.IsLessThan(_testConsole.Output.IndexOf("SecondSupply"), _testConsole.Output.IndexOf("FirstSupply"));
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void Show_SortByRatingDescending_RendersHighestRatedSupplierFirst() {
            _appData.Suppliers.Add(new Supplier(1, "LowRatedSupply", "low@mail.com", "+380501112233", 2.1));
            _appData.Suppliers.Add(new Supplier(2, "TopRatedSupply", "top@mail.com", "+380502223344", 4.9));

            // 1. "By rating (from the best)" (DownArrow -> Enter)
            // 2. Вихід зі списку ShowListScreen: "Back to menu" (Enter)
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _supplierScreen.Show();

            Assert.IsLessThan(_testConsole.Output.IndexOf("LowRatedSupply"), _testConsole.Output.IndexOf("TopRatedSupply"));
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void Show_SortByNameAlphabetically_RendersAlphabeticallyFirstSupplierFirst() {
            _appData.Suppliers.Add(new Supplier(1, "ZetaLogistics", "zeta@mail.com", "+380501112233", 4.2));
            _appData.Suppliers.Add(new Supplier(2, "AlphaGoods", "alpha@mail.com", "+380502223344", 4.5));

            // 1. "By name (A-Z)" (2x DownArrow -> Enter)
            // 2. Вихід зі списку ShowListScreen: "Back to menu" (Enter)
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _supplierScreen.Show();

            Assert.IsLessThan(_testConsole.Output.IndexOf("ZetaLogistics"), _testConsole.Output.IndexOf("AlphaGoods"));
            Assert.AreEqual("Back to menu", result);
        }

        #endregion

        #region Create Supplier Tests

        [TestMethod]
        public void Create_ValidInputs_AddsSupplierAndOutputsSuccess() {
            _testConsole.Input.PushTextWithEnter("GlobalTech");
            _testConsole.Input.PushTextWithEnter("contact@globaltech.com");
            _testConsole.Input.PushTextWithEnter("+380671234567");
            _testConsole.Input.PushTextWithEnter("4.7");
            _testConsole.Input.PushKey(ConsoleKey.Enter); // BackToMenuPrompt

            string result = _supplierScreen.Create();

            Assert.HasCount(1, _appData.Suppliers);
            Supplier created = _appData.Suppliers.First();
            Assert.AreEqual("GlobalTech", created.Name);
            Assert.AreEqual("contact@globaltech.com", created.ContactEmail);
            Assert.AreEqual("+380671234567", created.Phone);
            Assert.AreEqual(4.7, created.Rating, 0.001);

            Assert.Contains("New supplier is created!", _testConsole.Output);
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void Create_InvalidEmailOrPhone_CatchesArgumentExceptionAndOutputsError() {
            _testConsole.Input.PushTextWithEnter("BrokenSupply");
            _testConsole.Input.PushTextWithEnter("invalid-email");
            _testConsole.Input.PushTextWithEnter("+380671234567");
            _testConsole.Input.PushTextWithEnter("4.0");
            _testConsole.Input.PushKey(ConsoleKey.Enter); // BackToMenuPrompt

            string result = _supplierScreen.Create();

            Assert.IsEmpty(_appData.Suppliers);
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void Create_RatingOutOfRange_CatchesArgumentExceptionAndDoesNotAddSupplier() {
            // Оскільки ArgumentOutOfRangeException успадковується від ArgumentException, блок catch перехопить його
            _testConsole.Input.PushTextWithEnter("OutOfRangeSupply");
            _testConsole.Input.PushTextWithEnter("valid@mail.com");
            _testConsole.Input.PushTextWithEnter("+380671234567");
            _testConsole.Input.PushTextWithEnter("6.5");
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _supplierScreen.Create();

            Assert.IsEmpty(_appData.Suppliers);
            Assert.AreEqual("Back to menu", result);
        }

        #endregion

        #region Delete & Edit Tests

        [TestMethod]
        public void Delete_ExistingSupplierConfirmed_RemovesSupplierFromData() {
            _appData.Suppliers.Add(new Supplier(10, "TempSupply", "temp@mail.com", "+380509998877", 3.8));

            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushTextWithEnter("y");
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _supplierScreen.Delete();

            Assert.IsEmpty(_appData.Suppliers);
            Assert.Contains("Supplier successfully deleted!", _testConsole.Output);
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void Edit_ExistingSupplier_UpdatesPropertyAndSavesData() {
            var supplier = new Supplier(15, "OldSupplierName", "old@mail.com", "+380501112233", 4.0);
            _appData.Suppliers.Add(supplier);

            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushTextWithEnter("NewSupplierName");
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _supplierScreen.Edit();

            Assert.AreEqual("NewSupplierName", supplier.Name);
            Assert.IsTrue(File.Exists(Path.Combine("DataStorage", "suppliers.json")));
            Assert.AreEqual("Back to menu", result);
        }

        #endregion
    }
}
