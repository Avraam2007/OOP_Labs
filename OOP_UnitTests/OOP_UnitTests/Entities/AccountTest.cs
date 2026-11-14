using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Entities;
using System;
using static OOP_Main.Utilities.Exceptions;

namespace OOP_UnitTests.Entities {
    [TestClass]
    public class AccountTest {
        #region Account Tests

        [TestMethod]
        public void Account_ConstructorAndSumSetter_RoundsToTwoDecimalPlaces() {
            var account = new Account(125.456);

            Assert.AreEqual(125.46, account.Sum, 0.001);
        }

        [TestMethod]
        public void Account_AddValidAmount_IncreasesSum() {
            var account = new Account(100.0);

            account.Add(250.55);

            Assert.AreEqual(350.55, account.Sum, 0.001);
        }

        [TestMethod]
        public void Account_AddNegativeAmount_ThrowsArgumentOutOfRangeException() {
            var account = new Account(100.0);

            Assert.Throws<ArgumentOutOfRangeException>(() => 
                account.Add(-10.0));
        }

        [TestMethod]
        public void Account_AddAmountOverLimit_ThrowsArgumentOutOfRangeException() {
            var account = new Account(100.0);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                account.Add(1000.01));
        }

        [TestMethod]
        public void Account_TakeValidAmount_DecreasesSum() {
            var account = new Account(500.0);

            account.Take(175.25);

            Assert.AreEqual(324.75, account.Sum, 0.001);
        }

        [TestMethod]
        [DataRow(0.0)]
        [DataRow(-50.0)]
        public void Account_TakeZeroOrNegativeAmount_ThrowsArgumentOutOfRangeException(double amount) {
            var account = new Account(500.0);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                account.Take(amount));
        }

        [TestMethod]
        public void Account_TakeMoreThanBalance_ThrowsInsufficientFundsException() {
            var account = new Account(100.0);

            Assert.Throws<InsufficientFundsException>(() =>
                account.Take(150.0));
        }

        #endregion
    }
}
