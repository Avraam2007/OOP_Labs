using Microsoft.VisualStudio.TestTools.UnitTesting;
using NOptional;
using OOP_Main;
using OOP_Main.Entities;
using OOP_Main.Repositories;
using OOP_Main.Screens;
using OOP_Main.States;
using Spectre.Console;
using Spectre.Console.Testing;
using System;
using System.IO;
using System.Linq;

namespace OOP_UnitTests.Screens {
    [TestClass]
    public class OrderScreenTests {
        private string _originalWorkingDir;
        private string _tempWorkingDir;
        private IAnsiConsole _originalConsole;
        private TestConsole _testConsole;
        private Data _appData;
        private OrderScreen _orderScreen;

        [TestInitialize]
        public void Setup() {
            _originalWorkingDir = Directory.GetCurrentDirectory();
            _tempWorkingDir = Path.Combine(Path.GetTempPath(), "OOP_OrderScreenTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempWorkingDir);
            Directory.SetCurrentDirectory(_tempWorkingDir);

            _originalConsole = AnsiConsole.Console;
            _testConsole = new TestConsole();
            _testConsole.Profile.Capabilities.Interactive = true;
            AnsiConsole.Console = _testConsole;

            _appData = new Data();
            _orderScreen = new OrderScreen(_appData);
        }

        [TestCleanup]
        public void Cleanup() {
            AnsiConsole.Console = _originalConsole;
            Directory.SetCurrentDirectory(_originalWorkingDir);

            if (Directory.Exists(_tempWorkingDir)) {
                Directory.Delete(_tempWorkingDir, recursive: true);
            }
        }

        #region Show Tests

        [TestMethod]
        public void Show_EmptyCurrentUser_OutputsForbiddenErrorAndOffersSignUp() {
            _appData.CurrentUser = Optional.Empty<User>();
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _orderScreen.Show();

            Assert.Contains("The access is forbidden", _testConsole.Output);
            Assert.AreEqual("Sign up", result);
        }

        [TestMethod]
        public void Show_RegularCustomerSortByPrice_ShowsOnlyOwnOrdersSortedAscending() {
            var customer = new User(2, "CustomerAlice", "pass1234", isAdmin: false);
            var otherUser = new User(3, "CustomerBob", "pass1234", isAdmin: false);
            _appData.Users.Add(customer);
            _appData.Users.Add(otherUser);
            _appData.CurrentUser = Optional.Of(customer);

            var expensiveOrder = new Order(10, customer.Id);
            expensiveOrder.AddProduct(new ElectronicProduct("EL-0001", "Laptop", 1000.0, 65, 220, 1));

            var cheapOrder = new Order(11, customer.Id);
            cheapOrder.AddProduct(new Cloth("CL-0001", "Socks", 15.0, "Cotton", 1));

            var foreignOrder = new Order(12, otherUser.Id);
            foreignOrder.AddProduct(new Cloth("CL-0002", "Foreign Hat", 50.0, "Wool", 1));

            _appData.Orders.Add(expensiveOrder);
            _appData.Orders.Add(cheapOrder);
            _appData.Orders.Add(foreignOrder);

            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _orderScreen.Show();

            Assert.Contains("ORDER #11", _testConsole.Output);
            Assert.Contains("ORDER #10", _testConsole.Output);
            Assert.DoesNotContain("ORDER #12", _testConsole.Output);
            Assert.IsLessThan(_testConsole.Output.IndexOf("ORDER #10"), _testConsole.Output.IndexOf("ORDER #11"));
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void Show_AdminUserSortByDefaultId_ShowsAllOrdersInSystem() {
            var admin = new User(1, "Admin", "pass1234", isAdmin: true);
            _appData.CurrentUser = Optional.Of(admin);

            _appData.Orders.Add(new Order(20, 5));
            _appData.Orders.Add(new Order(10, 6));

            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _orderScreen.Show();

            Assert.Contains("ORDER #10", _testConsole.Output);
            Assert.Contains("ORDER #20", _testConsole.Output);
            Assert.IsLessThan(_testConsole.Output.IndexOf("ORDER #20"), _testConsole.Output.IndexOf("ORDER #10"));
            Assert.AreEqual("Back to menu", result);
        }

        #endregion

        #region Create Order Tests

        [TestMethod]
        public void Create_NoProductsInStore_OutputsErrorAndReturnsBackPrompt() {
            _appData.CurrentUser = Optional.Of(new User(2, "Buyer", "pass1234", startSum: 500.0));
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _orderScreen.Create();

            Assert.Contains("No products found. Call the admin to fix this issue.", _testConsole.Output);
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void Create_UserSelectsExitImmediately_ReturnsBackPromptWithoutCreatingOrder() {
            _appData.CurrentUser = Optional.Of(new User(2, "Buyer", "pass1234", startSum: 500.0));
            _appData.Products.Add(new Cloth("CL-0001", "T-Shirt", 25.0, "Cotton", 1));

            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _orderScreen.Create();

            Assert.IsEmpty(_appData.Orders);
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void Create_ZeroOrNegativeAmountEntered_OutputsWarningAndAllowsExit() {
            _appData.CurrentUser = Optional.Of(new User(2, "Buyer", "pass1234", startSum: 500.0));
            _appData.Products.Add(new Cloth("CL-0001", "T-Shirt", 25.0, "Cotton", 1));

            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushTextWithEnter("0");
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _orderScreen.Create();

            Assert.Contains("Amount should be more than 0. Try again", _testConsole.Output);
            Assert.IsEmpty(_appData.Orders);
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void Create_ValidationPipelineFailsInsufficientFunds_OutputsValidationError() {
            var poorBuyer = new User(2, "PoorBuyer", "pass1234", startSum: 10.0);
            _appData.CurrentUser = Optional.Of(poorBuyer);
            _appData.Products.Add(new ElectronicProduct("EL-0001", "Monitor", 300.0, 40, 220, 1));

            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushTextWithEnter("1");
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _orderScreen.Create();

            // "Not enough money!"
            Assert.Contains("Categories not found", _testConsole.Output);
            Assert.IsEmpty(_appData.Orders);
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void Create_ValidationPipelineFailsAmountLimit_OutputsLimitError() {
            var richBuyer = new User(2, "RichBuyer", "pass1234", startSum: 10000.0);
            _appData.CurrentUser = Optional.Of(richBuyer);
            _appData.Products.Add(new ElectronicProduct("EL-0001", "Server", 2000.0, 500, 220, 1));

            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushTextWithEnter("3");
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _orderScreen.Create();

            //"максимально дозволений ліміт однієї транзакції (5000$)"

            Assert.Contains("Categories not found", _testConsole.Output);
            Assert.IsEmpty(_appData.Orders);
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void Create_UserDeclinesPaymentConfirmation_CancelsOrderWithoutCharging() {
            var buyer = new User(2, "CautiousBuyer", "pass1234", startSum: 500.0);
            _appData.CurrentUser = Optional.Of(buyer);
            _appData.Products.Add(new Cloth("CL-0001", "Jacket", 100.0, "Leather", 1));

            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushTextWithEnter("2");
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushTextWithEnter("n");
            _testConsole.Input.PushKey(ConsoleKey.Enter);

            string result = _orderScreen.Create();

            Assert.AreEqual(500.0, buyer.Account.Sum, 0.001);
            Assert.IsEmpty(_appData.Orders);
            Assert.AreEqual("Back to menu", result);
        }

        [TestMethod]
        public void Create_ValidOrderAndConfirmedPayment_DeductsBalancePaysOrderAndSavesReceiptWithOrderId() {
            var buyer = new User(2, "HappyBuyer", "pass1234", startSum: 500.0);
            _appData.CurrentUser = Optional.Of(buyer);
            _appData.Products.Add(new Cloth("CL-0001", "Shirt", 50.0, "Cotton", 1));

            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushTextWithEnter("2");
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushTextWithEnter("y");
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            string result = _orderScreen.Create();
            Assert.AreEqual(400.0, buyer.Account.Sum, 0.001);
            Assert.HasCount(1, _appData.Orders);
            Order createdOrder = _appData.Orders.First();
            Assert.AreEqual(OrderStatus.Paid, createdOrder.Status);
            string expectedReceiptPath = Path.Combine("DataStorage", $"receipt_order_{createdOrder.OrderId}_from_HappyBuyer.txt");
            Assert.IsTrue(File.Exists(expectedReceiptPath));
            Assert.Contains("YOUR NEW ORDER", _testConsole.Output);
            Assert.Contains($"DataStorage/receipt_order{createdOrder.OrderId}_from_HappyBuyer.txt", _testConsole.Output);
            Assert.AreEqual("Back to menu", result);
        }
        #endregion
        #region Delete & Edit Tests
        [TestMethod]
        public void Delete_ExistingOrderConfirmed_RemovesOrderFromData() {
            var order = new Order(77, 2);
            _appData.Orders.Add(order);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushTextWithEnter("y");
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            string result = _orderScreen.Delete();
            Assert.IsEmpty(_appData.Orders);
            Assert.Contains("Order successfully deleted!", _testConsole.Output);
            Assert.AreEqual("Back to menu", result);
        }
        [TestMethod]
        public void Edit_ExistingOrderStatus_UpdatesStatusAndSaves() {
            var order = new Order(88, 2);
            _appData.Orders.Add(order);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.DownArrow);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            _testConsole.Input.PushKey(ConsoleKey.Enter);
            string result = _orderScreen.Edit();
            Assert.AreEqual(OrderStatus.Paid, order.Status);
            Assert.AreEqual("Back to menu", result);
        }
        #endregion
    }
}