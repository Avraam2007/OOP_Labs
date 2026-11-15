using Microsoft.VisualStudio.TestTools.UnitTesting;
using NOptional;
using OOP_Main.Entities;
using OOP_Main.Repositories;
using OOP_Main.UI;
using Spectre.Console;
using Spectre.Console.Testing;
using System;
using System.IO;
using System.Reflection;

namespace OOP_UnitTests.UI {
    [TestClass]
    public class InterfaceTests {
        private string _originalWorkingDir;
        private string _tempWorkingDir;
        private IAnsiConsole _originalConsole;
        private TestConsole _testConsole;
        private Data _appData;
        private Interface _uiInterface;

        [TestInitialize]
        public void Setup() {
            _originalWorkingDir = Directory.GetCurrentDirectory();
            _tempWorkingDir = Path.Combine(Path.GetTempPath(), "OOP_InterfaceTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempWorkingDir);
            Directory.SetCurrentDirectory(_tempWorkingDir);

            // Попередньо створюємо порожні JSON-файли, щоб уникнути тривалого відновлення з архіву
            string dataStorageDir = Path.Combine(_tempWorkingDir, "DataStorage");
            Directory.CreateDirectory(dataStorageDir);
            File.WriteAllText(Path.Combine(dataStorageDir, "users.json"), "[]");
            File.WriteAllText(Path.Combine(dataStorageDir, "products.json"), "[]");
            File.WriteAllText(Path.Combine(dataStorageDir, "orders.json"), "[]");
            File.WriteAllText(Path.Combine(dataStorageDir, "suppliers.json"), "[]");

            _originalConsole = AnsiConsole.Console;
            _testConsole = new TestConsole();
            _testConsole.Profile.Capabilities.Interactive = true;
            _testConsole.Profile.Width = 200;
            AnsiConsole.Console = _testConsole;

            _appData = new Data {
                CurrentUser = Optional.Of(new User(0, "Guest", "123"))
            };
            _uiInterface = new Interface(_appData);
        }

        [TestCleanup]
        public void Cleanup() {
            // Зупиняємо приватні фонові сервіси (_autoSaveService та _orderQueue), якщо вони були запущені
            DisposePrivateField(_uiInterface, "_autoSaveService");
            DisposePrivateField(_uiInterface, "_orderQueue");

            AnsiConsole.Console = _originalConsole;
            Directory.SetCurrentDirectory(_originalWorkingDir);

            if (Directory.Exists(_tempWorkingDir)) {
                Directory.Delete(_tempWorkingDir, recursive: true);
            }
        }

        #region Constructor & Navigation Flow Tests

        [TestMethod]
        public void Constructor_ValidAppData_SetsAppDataProperty() {
            Assert.AreSame(_appData, _uiInterface.AppData);
        }

        [TestMethod]
        public void RunScreenFlow_DefaultOrUnknownAction_ExecutesDelegateAndCompletes() {
            bool delegateCalled = false;

            _uiInterface.RunScreenFlow(() => {
                delegateCalled = true;
                return "Back to menu";
            });

            Assert.IsTrue(delegateCalled);
        }

        [TestMethod]
        public void RunScreenFlow_NullActionResult_CompletesWithoutThrowing() {
            bool delegateCalled = false;

            _uiInterface.RunScreenFlow(() => {
                delegateCalled = true;
                return null;
            });

            Assert.IsTrue(delegateCalled);
        }

        #endregion

        #region BootUpScreen & BootDownScreen Tests

        [TestMethod]
        public void BootUpScreen_InitializesGuestUserAndOutputsWelcomeBanner() {
            _uiInterface.BootUpScreen();

            Assert.Contains("ECommerce", _testConsole.Output);
            Assert.Contains("v0.26", _testConsole.Output);
            Assert.Contains("Loading...", _testConsole.Output);
            Assert.IsTrue(_appData.CurrentUser.HasValue());
            Assert.AreEqual(0, _appData.CurrentUser.Value.Id);
            Assert.AreEqual("Guest", _appData.CurrentUser.Value.Username);
        }

        [TestMethod]
        public void BootDownScreen_UserSelectsNo_ReturnsTrueAndContinuesRunning() {
            // "No" — другий пункт у списку ("Yes", "No")
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            bool shouldContinue = _uiInterface.BootDownScreen();

            Assert.IsTrue(shouldContinue);
        }

        [TestMethod]
        public void BootDownScreen_UserPressesEscape_UsesCancelResultNoAndReturnsTrue() {
            _testConsole.Input.PushKey(ConsoleKey.Escape);

            bool shouldContinue = _uiInterface.BootDownScreen();

            Assert.IsTrue(shouldContinue);
        }

        #endregion

        #region Access Control Tests

        [TestMethod]
        public void CheckAccessToCreateOrder_GuestUserCannotBuy_OutputsErrorAndReturnsBackPrompt() {
            _appData.CurrentUser = Optional.Of(new User(0, "Guest", "123"));
            _testConsole.Input.PushKey(ConsoleKey.Enter); // Обираємо "Back to menu" у BackToMenuPrompt

            var method = typeof(Interface).GetMethod("CheckAccessToCreateOrder", BindingFlags.NonPublic | BindingFlags.Instance);
            string result = (string)method.Invoke(_uiInterface, null);

            Assert.Contains("The access is forbidden", _testConsole.Output);
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void CheckAdminAccessToPage_NonAdminUser_OutputsForbiddenErrorAndDoesNotCallPage() {
            _appData.CurrentUser = Optional.Of(new User(2, "RegularCustomer", "pass1234", isAdmin: false));
            _testConsole.Input.PushKey(ConsoleKey.Enter); // Обираємо "Back to menu" у BackToMenuPrompt

            bool pageExecuted = false;
            Func<string> protectedPage = () => {
                pageExecuted = true;
                return "AdminPageOpened";
            };

            var method = typeof(Interface).GetMethod("CheckAdminAccessToPage", BindingFlags.NonPublic | BindingFlags.Instance);
            string result = (string)method.Invoke(_uiInterface, new object[] { protectedPage });

            Assert.IsFalse(pageExecuted);
            Assert.Contains("The access is forbidden. Only for admin", _testConsole.Output);
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void CheckAdminAccessToPage_AdminUser_ExecutesProtectedPageDelegate() {
            _appData.CurrentUser = Optional.Of(new User(1, "SuperAdmin", "pass1234", isAdmin: true));

            bool pageExecuted = false;
            Func<string> protectedPage = () => {
                pageExecuted = true;
                return "AdminPageOpened";
            };

            var method = typeof(Interface).GetMethod("CheckAdminAccessToPage", BindingFlags.NonPublic | BindingFlags.Instance);
            string result = (string)method.Invoke(_uiInterface, new object[] { protectedPage });

            Assert.IsTrue(pageExecuted);
            Assert.AreEqual("AdminPageOpened", result);
        }

        #endregion

        #region Private Helper

        private static void DisposePrivateField(object target, string fieldName) {
            var field = target?.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (field?.GetValue(target) is IDisposable disposable) {
                disposable.Dispose();
            }
        }

        #endregion
    }
}
