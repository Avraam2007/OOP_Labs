using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Entities;
using OOP_Main.Repositories;
using OOP_Main.Screens;
using Spectre.Console;
using Spectre.Console.Testing;
using System;
using System.Collections.Generic;
using System.IO;

namespace OOP_UnitTests.Screens {
    [TestClass]
    public class BaseScreenTests {
        private string _originalWorkingDir;
        private string _tempWorkingDir;
        private IAnsiConsole _originalConsole;
        private TestConsole _testConsole;
        private Data _appData;
        private TestableBaseScreen _screen;

        [TestInitialize]
        public void Setup() {
            _originalWorkingDir = Directory.GetCurrentDirectory();
            _tempWorkingDir = Path.Combine(Path.GetTempPath(), "OOP_BaseScreenTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempWorkingDir);
            Directory.SetCurrentDirectory(_tempWorkingDir);

            _originalConsole = AnsiConsole.Console;
            _testConsole = new TestConsole();
            _testConsole.Profile.Capabilities.Interactive = true;
            AnsiConsole.Console = _testConsole;

            _appData = new Data();
            _screen = new TestableBaseScreen(_appData);
        }

        [TestCleanup]
        public void Cleanup() {
            AnsiConsole.Console = _originalConsole;
            Directory.SetCurrentDirectory(_originalWorkingDir);

            if (Directory.Exists(_tempWorkingDir)) {
                Directory.Delete(_tempWorkingDir, recursive: true);
            }
        }

        #region EditObjectScreen Tests

        [TestMethod]
        public void EditObjectScreen_EmptyList_OutputsErrorAndReturnsBackPrompt() {
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _screen.EditObjectScreen(new List<Cloth>());

            Assert.Contains("There are no cloths found.", _testConsole.Output);
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void EditObjectScreen_UsersContainingOnlyAdminAndGuest_FiltersThemOutAndOutputsError() {
            var users = new List<User> {
                new User(0, "Guest", "guest123"),
                new User(1, "Admin", "admin123", isAdmin: true)
            };
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _screen.EditObjectScreen(users);

            Assert.Contains("There are no users found.", _testConsole.Output);
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void EditObjectScreen_ValidProduct_UpdatesFieldAndSavesData() {
            var shirt = new Cloth("CL-0001", "Old Shirt", 40.0, "Cotton", 1);
            var products = new List<Product> { shirt };
            _appData.Products.Add(shirt);

            // 1. Вибір товару у EditObjectScreen (Enter)
            // 2. Вибір другого редагованого поля у EntityEditorEngine (Enter)
            // 3. Введення нового значення
            // 4. BackToMenuPrompt (Enter)
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushTextWithEnter("New Stylish Shirt");
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _screen.EditObjectScreen(products);

            Assert.AreEqual("New Stylish Shirt", shirt.Name);
            Assert.IsTrue(File.Exists(Path.Combine("DataStorage", "products.json")));
            Assert.AreEqual("Back to menu", result);
        }

        #endregion

        #region DeleteObjectScreen Tests

        [TestMethod]
        public void DeleteObjectScreen_EmptyOrOnlyProtectedUsers_OutputsErrorAndReturnsBackPrompt() {
            var users = new List<User> {
                new User(0, "Guest", "guest123"),
                new User(1, "Admin", "admin123", isAdmin: true)
            };
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _screen.CallDeleteObjectScreen<User, int>(
                users,
                id => true,
                u => u.Username
            );

            Assert.Contains("There are no users to delete.", _testConsole.Output    );
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void DeleteObjectScreen_UserSelectsCancel_DoesNotInvokeDeleteAction() {
            var c1 = new Cloth("CL-0001", "Shirt", 30.0, "Cotton", 1);
            var items = new List<Product> { c1 };
            bool deleteCalled = false;

            // Список вибору: [0] c1, [1] Cancel -> DownArrow + Enter -> BackToMenuPrompt (Enter)
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _screen.CallDeleteObjectScreen<Product, string>(
                items,
                id => { deleteCalled = true; return true; },
                p => p.Name
            );

            Assert.IsFalse(deleteCalled);
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void DeleteObjectScreen_UserDeclinesConfirmation_OutputsCancelledMessage() {
            var c1 = new Cloth("CL-0001", "Shirt", 30.0, "Cotton", 1);
            var items = new List<Product> { c1 };
            bool deleteCalled = false;

            // 1. Вибір товару (Enter)
            // 2. Підтвердження: 'n'
            // 3. BackToMenuPrompt (Enter)
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushTextWithEnter("n");
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _screen.CallDeleteObjectScreen<Product, string>(
                items,
                id => { deleteCalled = true; return true; },
                p => p.Name
            );

            Assert.IsFalse(deleteCalled);
            Assert.Contains("Deleting cancelled.", _testConsole.Output);
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void DeleteObjectScreen_UserConfirmsAndDeleteSucceeds_OutputsSuccessMessage() {
            var c1 = new Cloth("CL-0001", "Shirt", 30.0, "Cotton", 1);
            var items = new List<Product> { c1 };
            string deletedId = null;

            // 1. Вибір товару (Enter)
            // 2. Підтвердження: 'y'
            // 3. BackToMenuPrompt (Enter)
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushTextWithEnter("y");
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _screen.CallDeleteObjectScreen<Product, string>(
                items,
                id => { deletedId = id; return true; },
                p => p.Name
            );

            Assert.AreEqual("CL-0001", deletedId);
            Assert.Contains("Product successfully deleted!", _testConsole.Output);
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void DeleteObjectScreen_DeleteActionReturnsFalse_OutputsFailureMessage() {
            var c1 = new Cloth("CL-0001", "Shirt", 30.0, "Cotton", 1);
            var items = new List<Product> { c1 };

            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushTextWithEnter("y");
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _screen.CallDeleteObjectScreen<Product, string>(
                items,
                id => false,
                p => p.Name
            );

            Assert.Contains("Failed to delete product.", _testConsole.Output);
            Assert.AreEqual("Back to menu", result);
        }

        #endregion

        #region ShowListScreen Pagination Tests

        [TestMethod]
        public void ShowListScreen_EmptyCollection_OutputsErrorWithExtraMessage() {
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _screen.CallShowListScreen("Products", new List<Product>(), "Try again later.");

            Assert.Contains("Sorry, we didn't find any products in the store. Try again later.", _testConsole.Output);
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void ShowListScreen_MultiplePages_NavigatesNextPreviousAndReturnsToMenu() {
            // 4 товари при pageSize = 3 -> 2 сторінки (3 на першій, 1 на другій)
            var products = new List<Product> {
                new Cloth("CL-0001", "Item One", 10.0, "Cotton", 1),
                new Cloth("CL-0002", "Item Two", 20.0, "Cotton", 1),
                new Cloth("CL-0003", "Item Three", 30.0, "Cotton", 1),
                new Cloth("CL-0004", "Item Four", 40.0, "Cotton", 1)
            };

            // На сторінці 1 опції: ["Next Page", "Back to menu"]
            // Натискаємо Enter -> переходимо на сторінку 2
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            // На сторінці 2 опції: ["Previous Page", "Back to menu"]
            // Натискаємо Enter -> повертаємось на сторінку 1
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            // Знову на сторінці 1: DownArrow -> обираємо "Back to menu" -> Enter
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _screen.CallShowListScreen("Catalog", products);

            Assert.Contains("Page 1 of 2", _testConsole.Output);
            Assert.Contains("Page 2 of 2", _testConsole.Output);
            Assert.Contains("Item One", _testConsole.Output);
            Assert.Contains("Item Four", _testConsole.Output);
            Assert.AreEqual("Back to menu", result);
        }

        #endregion

        #region Testable Subclass

        private class TestableBaseScreen : BaseScreen {
            public TestableBaseScreen(Data appData) : base(appData) { }
            public string CallDeleteObjectScreen<T, TId>(
                List<T> items,
                Func<TId, bool> deleteAction,
                Func<T, string> nameSelector)
                where T : class, IIdentifiable<TId>
                where TId : IEquatable<TId>
            => DeleteObjectScreen(items, deleteAction, nameSelector);

            public string CallShowListScreen<T>(
                string header,
                IEnumerable<T> items,
                string extraErrorMessage = "")
                where T : class
            => ShowListScreen(header, items, extraErrorMessage);
        }
        #endregion
    }
}


