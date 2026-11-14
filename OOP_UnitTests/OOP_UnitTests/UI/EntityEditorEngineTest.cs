using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Attributes;
using OOP_Main.Decorators;
using OOP_Main.Entities;
using OOP_Main.States;
using OOP_Main.UI;
using Spectre.Console;
using Spectre.Console.Testing;
using System;

namespace OOP_UnitTests.UI {
    [TestClass]
    public class EntityEditorEngineTests {
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

        #region Guard & Non-Editable Object Tests

        [TestMethod]
        public void EditEntity_NullTarget_ReturnsImmediatelyWithoutOutput() {
            EntityEditorEngine.EditEntity(null);

            Assert.AreEqual(string.Empty, _testConsole.Output);
        }

        [TestMethod]
        public void EditEntity_ObjectWithoutEditableAttributes_OutputsErrorMessage() {
            var plainObject = new NonEditableDummy { Id = 1, Title = "Static" };

            EntityEditorEngine.EditEntity(plainObject);

            Assert.Contains("This object has no editable fields.", _testConsole.Output);
        }

        #endregion

        #region Property Types Editing Tests

        [TestMethod]
        public void EditEntity_StringProperty_UpdatesValueAndOutputsSuccess() {
            var dummy = new MultiTypeEditableDummy { StringField = "OldName" };

            // Перша властивість у списку -> натискаємо Enter -> вводимо новий рядок
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushTextWithEnter("NewName");

            EntityEditorEngine.EditEntity(dummy);

            Assert.AreEqual("NewName", dummy.StringField);
            Assert.Contains("Field is successfully updated!", _testConsole.Output);
        }

        [TestMethod]
        public void EditEntity_DoubleProperty_UpdatesValueAndOutputsSuccess() {
            var dummy = new MultiTypeEditableDummy { DoubleField = 10.5 };

            // 2-й пункт у списку (1 натискання DownArrow) -> Enter -> вводимо нове число
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushTextWithEnter("99.75");

            EntityEditorEngine.EditEntity(dummy);

            Assert.AreEqual(99.75, dummy.DoubleField, 0.001);
            Assert.Contains("Field is successfully updated!", _testConsole.Output);
        }

        [TestMethod]
        public void EditEntity_IntProperty_UpdatesValueAndOutputsSuccess() {
            var dummy = new MultiTypeEditableDummy { IntField = 5 };

            // 3-й пункт у списку (2 натискання DownArrow) -> Enter -> вводимо ціле число
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushTextWithEnter("42");

            EntityEditorEngine.EditEntity(dummy);

            Assert.AreEqual(42, dummy.IntField);
            Assert.Contains("Field is successfully updated!", _testConsole.Output);
        }

        [TestMethod]
        public void EditEntity_BoolProperty_UpdatesValueViaConfirmPrompt() {
            var dummy = new MultiTypeEditableDummy { BoolField = true };

            // 4-й пункт у списку (3 натискання DownArrow) -> Enter -> відповідаємо 'n'
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushTextWithEnter("n");

            EntityEditorEngine.EditEntity(dummy);

            Assert.IsFalse(dummy.BoolField);
            Assert.Contains("Field is successfully updated!", _testConsole.Output);
        }

        [TestMethod]
        public void EditEntity_OrderStatusProperty_UpdatesEnumSelection() {
            var dummy = new MultiTypeEditableDummy { StatusField = OrderStatus.Pending };

            // 5-й пункт у списку (4 натискання DownArrow) -> Enter -> обираємо 2-й статус (Paid)
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            EntityEditorEngine.EditEntity(dummy);

            Assert.AreEqual(OrderStatus.Paid, dummy.StatusField);
            Assert.Contains("Field is successfully updated!", _testConsole.Output);
        }

        #endregion

        #region Decorator Layer Selection Tests

        [TestMethod]
        public void EditEntity_DecoratedProductChooseDecoratorLayer_EditsDiscountPercent() {
            var baseCloth = new Cloth("CL-0001", "Jacket", 100.0, "Leather", 1);
            var decorator = new DiscountDecorator(baseCloth, 10);

            // 1. Вибір шару: "Decorator parameters (discount/warranty)" (перший варіант -> Enter)
            // 2. Вибір поля декоратора (перший варіант -> Enter)
            // 3. Нове значення знижки -> "25"
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushTextWithEnter("25");

            EntityEditorEngine.EditEntity(decorator);

            Assert.AreEqual(25.0, decorator.DiscountPercent, 0.001);
            Assert.AreEqual("Jacket", baseCloth.Name);
        }

        [TestMethod]
        public void EditEntity_DecoratedProductChooseBaseProductLayer_EditsInnerProduct() {
            var baseCloth = new Cloth("CL-0001", "Jacket", 100.0, "Leather", 1);
            var decorator = new DiscountDecorator(baseCloth, 10);

            // 1. Вибір шару: "Base product" (DownArrow -> Enter)
            // 2. Вибір першого редагованого поля базового товару (Enter)
            // 3. Нове значення
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushTextWithEnter("UpdatedValue");

            EntityEditorEngine.EditEntity(decorator);

            Assert.AreEqual(10.0, decorator.DiscountPercent, 0.001);
            Assert.Contains("Field is successfully updated!", _testConsole.Output);
        }

        #endregion

        #region Validation Exception Handling Tests

        [TestMethod]
        public void EditEntity_SetterThrowsValidationException_CatchesTargetInvocationExceptionAndOutputsError() {
            var dummy = new ValidatedDummy { PositivePrice = 50.0 };

            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushTextWithEnter("-20");

            EntityEditorEngine.EditEntity(dummy);

            Assert.AreEqual(50.0, dummy.PositivePrice, 0.001);
            Assert.Contains("Validation error: Price cannot be negative.", _testConsole.Output);
        }

        #endregion

        #region Test Dummy Classes

        private class NonEditableDummy {
            public int Id { get; set; }
            public string Title { get; set; }
        }

        private class MultiTypeEditableDummy {
            [Editable("String Field")]
            public string StringField { get; set; }

            [Editable("Double Field")]
            public double DoubleField { get; set; }

            [Editable("Int Field")]
            public int IntField { get; set; }

            [Editable("Bool Field")]
            public bool BoolField { get; set; }

            [Editable("Status Field")]
            public OrderStatus StatusField { get; set; }
        }

        private class ValidatedDummy {
            private double _positivePrice;

            [Editable("Positive Price")]
            public double PositivePrice {
                get => _positivePrice;
                set {
                    if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "Price cannot be negative.");
                    _positivePrice = value;
                }
            }
        }

        #endregion
    }
}
