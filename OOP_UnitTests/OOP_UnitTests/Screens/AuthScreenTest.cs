using Microsoft.VisualStudio.TestTools.UnitTesting;
using NOptional;
using OOP_Main.Entities;
using OOP_Main.Repositories;
using OOP_Main.Screens;
using OOP_Main.Utilities;
using Spectre.Console;
using Spectre.Console.Testing;
using System;

namespace OOP_UnitTests.Screens {
    [TestClass]
    public class AuthScreenTests {
        private IAnsiConsole _originalConsole;
        private TestConsole _testConsole;
        private Data _appData;
        private AuthScreen _authScreen;

        [TestInitialize]
        public void Setup() {
            _originalConsole = AnsiConsole.Console;
            _testConsole = new TestConsole();
            _testConsole.Profile.Capabilities.Interactive = true;
            AnsiConsole.Console = _testConsole;

            _appData = new Data();
            _appData.AddUser("ExistingUser", "StrongPass123!");
            _appData.CurrentUser = Optional.Empty<User>();

            _authScreen = new AuthScreen(_appData);
        }

        [TestCleanup]
        public void Cleanup() {
            AnsiConsole.Console = _originalConsole;
            _authScreen = null;
            _appData = null;
        }

        #region LoginScreen Tests

        [TestMethod]
        public void LoginScreen_ValidCredentials_SetsCurrentUserAndOutputsWelcome() {
            _testConsole.Input.PushTextWithEnter("ExistingUser");
            _testConsole.Input.PushTextWithEnter("StrongPass123!");
            _testConsole.Input.PushKey(ConsoleKey.Enter); // BackToMenuPrompt

            string result = _authScreen.LoginScreen();

            Assert.IsTrue(_appData.CurrentUser.HasValue());
            Assert.AreEqual("ExistingUser", _appData.CurrentUser.Value.Username);
            Assert.Contains("Welcome back, ExistingUser!", _testConsole.Output);
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void LoginScreen_NonExistentUser_OutputsErrorAndLeavesCurrentUserUnchanged() {
            _testConsole.Input.PushTextWithEnter("UnknownUser");
            _testConsole.Input.PushTextWithEnter("AnyPass123!");
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _authScreen.LoginScreen();

            Assert.IsTrue(_appData.CurrentUser.IsEmpty());
            Assert.Contains("Invalid username or password. Try again", _testConsole.Output);
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void LoginScreen_WrongPassword_OutputsErrorAndLeavesCurrentUserUnchanged() {
            _testConsole.Input.PushTextWithEnter("ExistingUser");
            _testConsole.Input.PushTextWithEnter("WrongPassword!");
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _authScreen.LoginScreen();

            Assert.IsTrue(_appData.CurrentUser.IsEmpty());
            Assert.Contains("Invalid username or password. Try again", _testConsole.Output);
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void LoginScreen_GuestAccountAttempt_RejectsLoginAndOutputsError() {
            var guest = new User(0, "GuestAccount", UserService.HashPassword("guest123"));
            _appData.Users.Add(guest);

            _testConsole.Input.PushTextWithEnter("GuestAccount");
            _testConsole.Input.PushTextWithEnter("guest123");
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _authScreen.LoginScreen();

            Assert.IsTrue(_appData.CurrentUser.IsEmpty());
            Assert.Contains("Invalid username or password. Try again", _testConsole.Output);
            Assert.AreEqual("Back to menu", result);
        }

        #endregion

        #region Create (Sign Up) Tests

        [TestMethod]
        public void Create_UsernameAlreadyExists_OutputsErrorAndOffersLoginOption() {
            _testConsole.Input.PushTextWithEnter("ExistingUser");
            _testConsole.Input.PushKey(ConsoleKey.DownArrow); // Обираємо опцію "Log in" у BackToMenuPrompt
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _authScreen.Create();

            Assert.Contains("Sorry, this user was created earlier. You can log in to this account instead", _testConsole.Output);
            Assert.AreEqual("Log in", result);
        }

        [TestMethod]
        public void Create_PasswordConfirmationMismatch_OutputsErrorAndDoesNotCreateUser() {
            _testConsole.Input.PushTextWithEnter("BrandNewUser");
            _testConsole.Input.PushTextWithEnter("Pass1234!");
            _testConsole.Input.PushTextWithEnter("DifferentPass!");
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _authScreen.Create();

            Assert.IsTrue(_appData.GetUserByUsername("BrandNewUser").IsEmpty());
            Assert.Contains("Incorrect password. Try again", _testConsole.Output);
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void Create_ValidNewUser_AddsUserLogsInAndOutputsSuccess() {
            _testConsole.Input.PushTextWithEnter("NewCustomer");
            _testConsole.Input.PushTextWithEnter("SecretPass99!");
            _testConsole.Input.PushTextWithEnter("SecretPass99!");
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _authScreen.Create();

            var createdUser = _appData.GetUserByUsername("NewCustomer");
            Assert.IsTrue(createdUser.HasValue());
            Assert.IsTrue(_appData.CurrentUser.HasValue());
            Assert.AreEqual(createdUser.Value.Id, _appData.CurrentUser.Value.Id);
            Assert.Contains("Account was created!", _testConsole.Output);
            Assert.AreEqual("Back to menu", result);
        }

        #endregion
    }
}
