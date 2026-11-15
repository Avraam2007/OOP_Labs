using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main;
using OOP_Main.Entities;
using OOP_Main.Handlers;
using OOP_Main.States;
using OOP_Main.Structures;
using System.Collections.Generic;
using System.Security.Policy;

namespace OOP_UnitTests.Handlers {
    [TestClass]
    public class OrderValidationHandlersTests {
        private User _validBuyer;
        private Order _validOrder;

        private void SetUserMoney(double amount) => _validBuyer.Account.Sum = amount;

        [TestInitialize]
        public void Setup() {
            _validBuyer = new User(1, "TestCustomer", "pass1234", UserRole.Customer, 1000);

            var products = new List<Product> {
                new ElectronicProduct("EL-0001", "Monitor", 250.0, 40, 220, 1)
            };
            _validOrder = new Order {
                OrderId = 10,
                BuyerId = _validBuyer.Id,
                Products = products
            };
        }

        [TestCleanup]
        public void Cleanup() {
            _validBuyer = null;
            _validOrder = null;
        }

        #region EmptyOrderValidationHandler Tests

        [TestMethod]
        public void EmptyOrderValidationHandler_NullOrder_ReturnsFalseWithError() {
            var handler = new EmptyOrderValidationHandler();

            bool result = handler.Handle(null, _validBuyer, out string errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Замовлення не містить жодного товару.", errorMessage);
        }

        [TestMethod]
        public void EmptyOrderValidationHandler_EmptyProductsList_ReturnsFalseWithError() {
            var handler = new EmptyOrderValidationHandler();
            _validOrder.Products = new List<Product>();

            bool result = handler.Handle(_validOrder, _validBuyer, out string errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Замовлення не містить жодного товару.", errorMessage);
        }

        [TestMethod]
        public void EmptyOrderValidationHandler_ValidOrder_ReturnsTrueAndNullError() {
            var handler = new EmptyOrderValidationHandler();

            bool result = handler.Handle(_validOrder, _validBuyer, out string errorMessage);

            Assert.IsTrue(result);
            Assert.IsNull(errorMessage);
        }

        #endregion

        #region UserAccountStatusHandler Tests

        [TestMethod]
        public void UserAccountStatusHandler_NullBuyer_ReturnsFalseWithError() {
            var handler = new UserAccountStatusHandler();

            bool result = handler.Handle(_validOrder, null, out string errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Користувач не авторизований або не має прав для здійснення покупок.", errorMessage);
        }

        [TestMethod]
        public void UserAccountStatusHandler_GuestBuyerCannotBuy_ReturnsFalseWithError() {
            var handler = new UserAccountStatusHandler();
            var guest = new User(0, "Guest", "1234", UserRole.Guest);

            bool result = handler.Handle(_validOrder, guest, out string errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Користувач не авторизований або не має прав для здійснення покупок.", errorMessage);
        }

        [TestMethod]
        public void UserAccountStatusHandler_AuthorizedBuyer_ReturnsTrue() {
            var handler = new UserAccountStatusHandler();

            bool result = handler.Handle(_validOrder, _validBuyer, out string errorMessage);

            Assert.IsTrue(result);
            Assert.IsNull(errorMessage);
        }

        #endregion

        #region StockAvailabilityHandler Tests

        //[TestMethod]
        //public void StockAvailabilityHandler_InvalidArticle_ReturnsFalseWithFormattedError() {
        //    var handler = new StockAvailabilityHandler();
        //    var badProduct = new Cloth("BAD_ART", "Broken Shirt", 30.0, "Cotton", 1);
        //    _validOrder.Products = new List<Product> { badProduct };

        //    bool result = handler.Handle(_validOrder, _validBuyer, out string errorMessage);

        //    Assert.IsFalse(result);
        //    Assert.AreEqual("Товар 'Broken Shirt' має некоректний або відсутній артикул (BAD_ART).", errorMessage);
        //}

        //[TestMethod]
        //public void StockAvailabilityHandler_EmptyArticle_ReturnsFalseWithFormattedError() {
        //    var handler = new StockAvailabilityHandler();
        //    var emptyArtProduct = new Cloth("   ", "Nameless Hat", 15.0, "Wool", 1);
        //    _validOrder.Products = new List<Product> { emptyArtProduct };

        //    bool result = handler.Handle(_validOrder, _validBuyer, out string errorMessage);

        //    Assert.IsFalse(result);
        //    Assert.AreEqual("Товар 'Nameless Hat' має некоректний або відсутній артикул (   ).", errorMessage);
        //}

        [TestMethod]
        public void StockAvailabilityHandler_AllArticlesValid_ReturnsTrue() {
            var handler = new StockAvailabilityHandler();

            bool result = handler.Handle(_validOrder, _validBuyer, out string errorMessage);

            Assert.IsTrue(result);
            Assert.IsNull(errorMessage);
        }

        #endregion

        #region InsufficientFundsHandler Tests

        [TestMethod]
        public void InsufficientFundsHandler_BalanceLessThanOrderTotal_ReturnsFalseWithError() {
            var handler = new InsufficientFundsHandler();
            double total = _validOrder.GetTotalPrice();
            SetUserMoney(total - 10.0);

            bool result = handler.Handle(_validOrder, _validBuyer, out string errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual($"Not enough money! Required: {total}$, balance: {_validBuyer.Account.Sum}$.", errorMessage);
        }

        [TestMethod]
        public void InsufficientFundsHandler_BalanceEnough_ReturnsTrue() {
            var handler = new InsufficientFundsHandler();
            double total = _validOrder.GetTotalPrice();
            SetUserMoney(total);

            bool result = handler.Handle(_validOrder, _validBuyer, out string errorMessage);

            Assert.IsTrue(result);
            Assert.IsNull(errorMessage);
        }

        #endregion

        #region OrderAmountLimitHandler Tests

        [TestMethod]
        public void OrderAmountLimitHandler_TotalExceedsCustomLimit_ReturnsFalseWithError() {
            var handler = new OrderAmountLimitHandler(maxOrderLimit: 100.0);

            bool result = handler.Handle(_validOrder, _validBuyer, out string errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Сума замовлення перевищує максимально дозволений ліміт однієї транзакції (100$).", errorMessage);
        }

        [TestMethod]
        public void OrderAmountLimitHandler_TotalWithinDefaultLimit_ReturnsTrue() {
            var handler = new OrderAmountLimitHandler();

            bool result = handler.Handle(_validOrder, _validBuyer, out string errorMessage);

            Assert.IsTrue(result);
            Assert.IsNull(errorMessage);
        }

        #endregion

        #region Chain Of Responsibility Integration Tests

        [TestMethod]
        public void ValidationChain_AllChecksPass_ReturnsTrueAndNullError() {
            var chain = new EmptyOrderValidationHandler();
            chain.SetNext(new UserAccountStatusHandler())
                 .SetNext(new StockAvailabilityHandler())
                 .SetNext(new OrderAmountLimitHandler(5000.0))
                 .SetNext(new InsufficientFundsHandler());

            bool result = chain.Handle(_validOrder, _validBuyer, out string errorMessage);

            Assert.IsTrue(result);
            Assert.IsNull(errorMessage);
        }

        //[TestMethod]
        //public void ValidationChain_FailsInMiddleHandler_StopsExecutionAndReturnsSpecificError() {
        //    var chain = new EmptyOrderValidationHandler();
        //    chain.SetNext(new UserAccountStatusHandler())
        //         .SetNext(new StockAvailabilityHandler())
        //         .SetNext(new OrderAmountLimitHandler(5000.0))
        //         .SetNext(new InsufficientFundsHandler());

        //    SetUserMoney(0);
        //    _validOrder.Products = new List<Product> {
        //        new Cloth("INVALID", "Shirt", 50.0, "Cotton", 1)
        //    };

        //    bool result = chain.Handle(_validOrder, _validBuyer, out string errorMessage);

        //    Assert.IsFalse(result);
        //    Assert.AreEqual("Товар 'Shirt' має некоректний або відсутній артикул (INVALID).", errorMessage);
        //}

        #endregion
    }
}
