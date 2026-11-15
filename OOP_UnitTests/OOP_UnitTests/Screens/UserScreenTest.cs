using Microsoft.VisualStudio.TestTools.UnitTesting;
using NOptional;
using OOP_Main.Entities;
using OOP_Main.Repositories;
using OOP_Main.Screens;
using OOP_Main.Utilities;
using Spectre.Console;
using Spectre.Console.Testing;
using System;
using System.IO;
using System.Linq;

namespace OOP_UnitTests.Screens {
    [TestClass]
    public class UserScreenTests {
        private string _originalWorkingDir;
        private string _tempWorkingDir;
        private IAnsiConsole _originalConsole;
        private TestConsole _testConsole;
        private Data _appData;
        private UserScreen _userScreen;

        [TestInitialize]
        public void Setup() {
            _originalWorkingDir = Directory.GetCurrentDirectory();
            _tempWorkingDir = Path.Combine(Path.GetTempPath(), "OOP_UserScreenTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempWorkingDir);
            Directory.SetCurrentDirectory(_tempWorkingDir);

            _originalConsole = AnsiConsole.Console;
            _testConsole = new TestConsole();
            _testConsole.Profile.Capabilities.Interactive = true;
            _testConsole.Profile.Width = 200;
            AnsiConsole.Console = _testConsole;

            _appData = new Data();
            _userScreen = new UserScreen(_appData);
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
        public void Show_SortByIdDefault_RendersUsersOrderedByIdAscending() {
            _appData.Users.Add(new User(15, "SecondUser", "pass1234"));
            _appData.Users.Add(new User(3, "FirstUser", "pass1234"));

            // 1. "By ID (default)" (Enter)
            // 2. Вихід зі списку ShowListScreen: "Back to menu" (Enter)
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _userScreen.Show();

            Assert.IsLessThan(_testConsole.Output.IndexOf("SecondUser"), _testConsole.Output.IndexOf("FirstUser"));
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void Show_SortByStatusAlphabetically_RendersAdministratorBeforeCustomer() {
            _appData.Users.Add(new User(1, "RegularCustomer", "pass1234", isAdmin: false));
            _appData.Users.Add(new User(2, "SystemAdmin", "pass1234", isAdmin: true));

            // 1. "By status (A-Z)" (DownArrow -> Enter)
            // 2. Вихід зі списку ShowListScreen: "Back to menu" (Enter)
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _userScreen.Show();

            Assert.IsLessThan(_testConsole.Output.IndexOf("RegularCustomer"), _testConsole.Output.IndexOf("SystemAdmin"));
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void Show_SortByNameAlphabetically_RendersUsersOrderedByUsername() {
            _appData.Users.Add(new User(1, "Zack", "pass1234"));
            _appData.Users.Add(new User(2, "Anna", "pass1234"));

            // 1. "By name (A-Z)" (2x DownArrow -> Enter)
            // 2. Вихід зі списку ShowListScreen: "Back to menu" (Enter)
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _userScreen.Show();

            Assert.IsLessThan(_testConsole.Output.IndexOf("Zack"), _testConsole.Output.IndexOf("Anna"));
            Assert.AreEqual("Back to menu", result);
        }

        #endregion

        #region CheckAccount & TopUpBalance Tests

        [TestMethod]
        public void CheckAccount_LoggedInUser_DisplaysCardAndReturnsChosenMenuOption() {
            var user = new User(5, "Alice", "pass1234", startSum: 250.0);
            _appData.CurrentUser = Optional.Of(user);

            // Варіанти: ["Back to menu", "Top up money balance", "Change password"]
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _userScreen.CheckAccount();

            Assert.Contains("Alice", _testConsole.Output);
            Assert.AreEqual("Top up money balance", result);
        }

        [TestMethod]
        public void TopUpBalance_ValidAmount_IncreasesUserBalanceAndReturnsToAccountScreen() {
            var user = new User(5, "Alice", "pass1234", startSum: 100.0);
            _appData.CurrentUser = Optional.Of(user);

            // 1. Сума поповнення: "250.5"
            // 2. Навігація в CheckAccount після поповнення: "Back to menu" (Enter)
            _testConsole.Input.PushTextWithEnter("250.5");
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _userScreen.TopUpBalance();

            Assert.AreEqual(350.5, user.Account.Sum, 0.001);
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void TopUpBalance_AmountExceedsLimitOrNegative_CatchesArgumentExceptionAndLeavesBalanceUnchanged() {
            var user = new User(5, "Alice", "pass1234", startSum: 100.0);
            _appData.CurrentUser = Optional.Of(user);

            // Спроба поповнити на суму понад ліміт 1000$ викликає ArgumentOutOfRangeException (спадкоємець ArgumentException)
            _testConsole.Input.PushTextWithEnter("1500");
            _testConsole.Input.PushKey(ConsoleKey.Enter); // BackToMenuPrompt у CheckAccount

            string result = _userScreen.TopUpBalance();

            Assert.AreEqual(100.0, user.Account.Sum, 0.001);
            Assert.AreEqual("Back to menu", result);
        }

        #endregion

        #region ChangePassword Tests

        [TestMethod]
        public void ChangePassword_EmptyCurrentUserOrWrongOldPassword_OutputsError() {
            var user = new User(5, "Alice", UserService.HashPassword("OldPass123!"));
            _appData.CurrentUser = Optional.Of(user);

            _testConsole.Input.PushTextWithEnter("WrongOldPass!");
            _testConsole.Input.PushKey(ConsoleKey.Enter); // BackToMenuPrompt

            string result = _userScreen.ChangePassword();

            Assert.IsTrue(UserService.CheckPassword("OldPass123!", user));
            Assert.Contains("Invalid password. Try again", _testConsole.Output);
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void ChangePassword_GuestUser_RejectsPasswordChange() {
            var guest = new User(0, "Guest", UserService.HashPassword("guest123"));
            _appData.CurrentUser = Optional.Of(guest);

            _testConsole.Input.PushTextWithEnter("guest123");
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _userScreen.ChangePassword();

            Assert.Contains("Invalid password. Try again", _testConsole.Output);
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void ChangePassword_NewPasswordConfirmationMismatch_OutputsErrorAndKeepsOldPassword() {
            var user = new User(5, "Alice", UserService.HashPassword("OldPass123!"));
            _appData.CurrentUser = Optional.Of(user);

            _testConsole.Input.PushTextWithEnter("OldPass123!");
            _testConsole.Input.PushTextWithEnter("NewPass123!");
            _testConsole.Input.PushTextWithEnter("DifferentNewPass!");
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _userScreen.ChangePassword();

            Assert.IsTrue(UserService.CheckPassword("OldPass123!", user));
            Assert.Contains("Incorrect password. Try again", _testConsole.Output);
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void ChangePassword_ValidCredentialsAndMatchingConfirmation_UpdatesHashAndSavesData() {
            var user = new User(5, "Alice", UserService.HashPassword("OldPass123!"));
            _appData.Users.Add(user);
            _appData.CurrentUser = Optional.Of(user);

            _testConsole.Input.PushTextWithEnter("OldPass123!");
            _testConsole.Input.PushTextWithEnter("BrandNewPass99!");
            _testConsole.Input.PushTextWithEnter("BrandNewPass99!");
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _userScreen.ChangePassword();

            Assert.IsTrue(UserService.CheckPassword("BrandNewPass99!", user));
            Assert.IsFalse(UserService.CheckPassword("OldPass123!", user));
            Assert.Contains("Password was changed!", _testConsole.Output);
            Assert.IsTrue(File.Exists(Path.Combine("DataStorage", "users.json")));
            Assert.AreEqual("Back to menu", result);
        }

        #endregion

        #region Delete & Edit Tests

        [TestMethod]
        public void Delete_RegularCustomerConfirmed_RemovesUserFromData() {
            _appData.Users.Add(new User(1, "Admin", "pass1234", isAdmin: true));
            _appData.Users.Add(new User(10, "RemovableCustomer", "pass1234", isAdmin: false));

            // Адмін автоматично відфільтровується у DeleteObjectScreen, тому першим у списку є RemovableCustomer
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushTextWithEnter("y");
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _userScreen.Delete();

            Assert.DoesNotContain(u => u.Id == 10, _appData.Users);
            Assert.Contains("User successfully deleted!", _testConsole.Output);
            Assert.AreEqual("Back to menu", result);
        }
        [TestMethod]
        public void Edit_RegularCustomer_UpdatesUsernameAndSavesData() {
            var customer = new User(12, "OldCustomerName", "pass1234", isAdmin: false);
            _appData.Users.Add(customer);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushTextWithEnter("NewCustomerName");
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            string result = _userScreen.Edit();
            Assert.AreEqual("NewCustomerName", customer.Username);
            Assert.AreEqual("Back to menu", result);
        }
        #endregion
    }
}