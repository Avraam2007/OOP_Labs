using Microsoft.VisualStudio.TestTools.UnitTesting;
using NOptional;
using OOP_Main.Entities;
using OOP_Main.UI;
using Spectre.Console;
using Spectre.Console.Rendering;
using Spectre.Console.Testing;
using System;
using System.Collections.Generic;

namespace OOP_UnitTests.UI {
    [TestClass]
    public class UIHelperTests {
        private IAnsiConsole _originalConsole;
        private TestConsole _testConsole;
        private TestableUIHelper _uiHelper;

        [TestInitialize]
        public void Setup() {
            _originalConsole = AnsiConsole.Console;
            _testConsole = new TestConsole();
            _testConsole.Profile.Capabilities.Interactive = true;
            AnsiConsole.Console = _testConsole;

            _uiHelper = new TestableUIHelper();
        }

        [TestCleanup]
        public void Cleanup() {
            AnsiConsole.Console = _originalConsole;
            _uiHelper = null;
        }

        #region Output & Formatting Tests

        [TestMethod]
        public void DefaultMarkupOutput_ValidMarkup_WritesTextToConsole() {
            UIHelper.DefaultMarkupOutput("[bold]Welcome to Store[/]");

            Assert.Contains("Welcome to Store", _testConsole.Output);
        }

        [TestMethod]
        public void DefaultErrorOutput_MessageWithBrackets_EscapesMarkupAndOutputsSafely() {
            UIHelper.DefaultErrorOutput("Error in [User] module");

            Assert.Contains("Error in [User] module", _testConsole.Output);
        }

        [TestMethod]
        public void DefaultSuccessOutput_ValidMessage_WritesSuccessText() {
            UIHelper.DefaultSuccessOutput("Operation completed");

            Assert.Contains("Operation completed", _testConsole.Output);
        }

        [TestMethod]
        public void InternalFlashCard_ValidDictionary_RendersPanelWithValues() {
            var cardData = new Dictionary<string, IRenderable> {
                { "Row1", new Text("User: Alice") },
                { "Row2", new Text("Balance: 500$") }
            };

            _uiHelper.CallInternalFlashCard(cardData);

            Assert.Contains("User: Alice", _testConsole.Output);
            Assert.Contains("Balance: 500$", _testConsole.Output);
        }

        [TestMethod]
        public void ShowHeader_ValidTitle_ClearsConsoleAndRendersHeaderPanel() {
            UIHelper.DefaultMarkupOutput("Old screen content");

            _uiHelper.CallShowHeader("MAIN DASHBOARD");

            Assert.Contains("MAIN DASHBOARD", _testConsole.Output);
        }

        [TestMethod]
        public void RenderProductsTable_ValidProducts_RendersTableWithAllRows() {
            var products = new List<Product> {
                new ElectronicProduct("EL-0001", "Laptop", 1200.50, 65, 220, 1),
                new Cloth("CL-0001", "Jacket", 85.00, "Wool", 2)
            };

            _uiHelper.CallRenderProductsTable(products);

            Assert.Contains("Found products: 2", _testConsole.Output);
            Assert.Contains("EL-0001", _testConsole.Output);
            Assert.Contains("Laptop", _testConsole.Output);
            Assert.Contains("1200,50", _testConsole.Output);
            Assert.Contains("CL-0001", _testConsole.Output);
            Assert.Contains("Jacket", _testConsole.Output);
            Assert.Contains("85,00", _testConsole.Output);
        }

        #endregion

        #region Interactive Prompts & Spinner Tests

        [TestMethod]
        public void StatusSpinner_WithAction_ExecutesActionAndDisplaysStatus() {
            bool actionExecuted = false;

            _uiHelper.CallStatusSpinner("Loading data...", () => actionExecuted = true, loadingTimeInMs: 10);

            Assert.IsTrue(actionExecuted);
            Assert.Contains("Loading data...", _testConsole.Output);
        }

        [TestMethod]
        public void DefaultTextPrompt_StringWithMarkupBrackets_ReturnsEscapedString() {
            _testConsole.Input.PushTextWithEnter("Hello [Admin]");

            string result = _uiHelper.CallDefaultTextPrompt<string>("Enter text:");

            Assert.AreEqual("Hello [[Admin]]", result);
        }

        [TestMethod]
        public void DefaultTextPrompt_NumericType_ReturnsParsedNumber() {
            _testConsole.Input.PushTextWithEnter("42");

            int result = _uiHelper.CallDefaultTextPrompt<int>("Enter number:");

            Assert.AreEqual(42, result);
        }

        [TestMethod]
        public void PromptWithDefaultValue_EmptyEnter_ReturnsDefaultValue() {
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            int result = _uiHelper.CallPromptWithDefaultValue("Enter count:", 99);

            Assert.AreEqual(99, result);
        }

        [TestMethod]
        public void PasswordPrompt_EnteredPassword_ReturnsSecretString() {
            _testConsole.Input.PushTextWithEnter("MySecret123");

            string result = _uiHelper.CallPasswordPrompt("Enter password:");

            Assert.AreEqual("MySecret123", result);
        }

        [TestMethod]
        public void DefaultConfirm_UserEntersNo_ReturnsFalse() {
            _testConsole.Input.PushTextWithEnter("n");

            bool confirmed = _uiHelper.CallDefaultConfirm("Proceed?", defaultValue: true);

            Assert.IsFalse(confirmed);
        }

        [TestMethod]
        public void BackToMenuPrompt_UserSelectsExtraOption_ReturnsChosenOption() {
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string choice = _uiHelper.CallBackToMenuPrompt("Edit Profile", "  ", null);

            Assert.AreEqual("Edit Profile", choice);
        }

        #endregion

        #region ClearConsole, RuleLine & HeaderPanel Tests

        [TestMethod]
        public void ClearConsole_WhenInvoked_ClearsConsoleWithoutThrowing() {
            UIHelper.DefaultMarkupOutput("Text before clear");

            _uiHelper.CallClearConsole();

            // Перевіряємо, що після очищення консоль продовжує коректно приймати новий вивід
            UIHelper.DefaultMarkupOutput("Text after clear");
            Assert.Contains("Text after clear", _testConsole.Output);
        }

        [TestMethod]
        public void RuleLine_ReturnsValidRuleRenderableAndRendersDivider() {
            Rule rule = _uiHelper.CallRuleLine();

            Assert.IsNotNull(rule);
            Assert.IsInstanceOfType(rule, typeof(Rule));

            AnsiConsole.Write(rule);
            Assert.Contains("─", _testConsole.Output);
        }

        [TestMethod]
        public void HeaderPanel_ValidTitleAndColor_ReturnsConfiguredPanel() {
            Panel panel = _uiHelper.CallHeaderPanel("CATALOG", Color.Green);

            Assert.IsNotNull(panel);
            Assert.AreEqual(Color.Green, panel.BorderStyle?.Foreground);

            AnsiConsole.Write(panel);
            Assert.Contains("CATALOG", _testConsole.Output);
        }

        #endregion


        #region Enum & User Role Helper Tests

        private enum SampleTestEnum {
            [System.ComponentModel.Description("First Option Description")]
            WithDescription,
            WithoutDescription
        }

        [TestMethod]
        public void GetEnumDescription_EnumWithDescriptionAttribute_ReturnsAttributeText() {
            string description = _uiHelper.CallGetEnumDescription(SampleTestEnum.WithDescription);

            Assert.AreEqual("First Option Description", description);
        }

        [TestMethod]
        public void GetEnumDescription_EnumWithoutDescriptionAttribute_ReturnsEnumName() {
            string description = _uiHelper.CallGetEnumDescription(SampleTestEnum.WithoutDescription);

            Assert.AreEqual("WithoutDescription", description);
        }

        [TestMethod]
        public void GetEnumDescription_UndefinedEnumValue_ReturnsNumericString() {
            string description = _uiHelper.CallGetEnumDescription((SampleTestEnum)999);

            Assert.AreEqual("999", description);
        }

        [TestMethod]
        public void CheckIfUserIsAdmin_VariousUsersAndOptionals_ReturnsExpectedResult() {
            var admin = new User(1, "Admin", "pass1234", isAdmin: true);
            var customer = new User(2, "Customer", "pass1234", isAdmin: false);

            Assert.IsTrue(_uiHelper.CallCheckIfUserIsAdmin(admin));
            Assert.IsFalse(_uiHelper.CallCheckIfUserIsAdmin(customer));
            Assert.IsFalse(_uiHelper.CallCheckIfUserIsAdmin((User)null));

            Assert.IsTrue(_uiHelper.CallCheckIfUserIsAdmin(Optional.Of(admin)));
            Assert.IsFalse(_uiHelper.CallCheckIfUserIsAdmin(Optional.Empty<User>()));
        }

        #endregion

        #region Testable Subclass

        private class TestableUIHelper : UIHelper {
            public void CallStatusSpinner(string text, Action action = null, int loadingTimeInMs = 10)
                => StatusSpinner(text, action, loadingTimeInMs);

            public T CallPromptWithDefaultValue<T>(string text, T defaultValue)
                => PromptWithDefaultValue(text, defaultValue);

            public T CallDefaultTextPrompt<T>(string text)
                => DefaultTextPrompt<T>(text);

            public string CallPasswordPrompt(string text)
                => PasswordPrompt(text);

            public void CallInternalFlashCard(Dictionary<string, IRenderable> data)
                => InternalFlashCard(data);

            public void CallShowHeader(string text)
                => ShowHeader(text);

            public string CallBackToMenuPrompt(params string[] extraOptions)
                => BackToMenuPrompt(extraOptions);

            public bool CallDefaultConfirm(string text, bool defaultValue = true)
                => DefaultConfirm(text, defaultValue);

            public void CallRenderProductsTable(IEnumerable<Product> products)
                => RenderProductsTable(products);
            public void CallClearConsole()
                => ClearConsole();

            public Rule CallRuleLine()
                => RuleLine();

            public Panel CallHeaderPanel(string text, Color color)
                => HeaderPanel(text, color);

            public string CallGetEnumDescription(Enum value)
                => GetEnumDescription(value);

            public bool CallCheckIfUserIsAdmin(IOptional<User> currentUser)
                => CheckIfUserIsAdmin(currentUser);

            public bool CallCheckIfUserIsAdmin(User currentUser)
                => CheckIfUserIsAdmin(currentUser);
        }

        #endregion
    }
}
