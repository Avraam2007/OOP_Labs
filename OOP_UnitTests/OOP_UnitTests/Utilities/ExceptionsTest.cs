using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static OOP_Main.Utilities.Exceptions;

namespace OOP_UnitTests.Utilities {
    [TestClass]
    public class ExceptionsTests {
        #region ECommerceException Tests

        [TestMethod]
        public void ECommerceException_MessageConstructor_SetsMessageTimestampAndBaseType() {
            DateTime beforeCreation = DateTime.Now;

            var exception = new ECommerceException("General commerce error");

            DateTime afterCreation = DateTime.Now;

            Assert.IsInstanceOfType(exception, typeof(Exception));
            Assert.AreEqual("General commerce error", exception.Message);
            Assert.IsNull(exception.InnerException);
            Assert.IsTrue(exception.Timestamp >= beforeCreation && exception.Timestamp <= afterCreation);
        }

        [TestMethod]
        public void ECommerceException_InnerExceptionConstructor_PreservesMessageAndInnerException() {
            var inner = new InvalidOperationException("Underlying failure");

            var exception = new ECommerceException("Wrapped error", inner);

            Assert.AreEqual("Wrapped error", exception.Message);
            Assert.AreSame(inner, exception.InnerException);
        }

        #endregion

        #region EntityNotFoundException Tests

        [TestMethod]
        public void EntityNotFoundException_ValidArguments_SetsPropertiesAndFormatsMessage() {
            var exception = new EntityNotFoundException("Product", "EL-0042");

            Assert.IsInstanceOfType(exception, typeof(ECommerceException));
            Assert.AreEqual("Product", exception.EntityName);
            Assert.AreEqual("EL-0042", exception.EntityId);
            Assert.AreEqual("Entity 'Product' with ID [EL-0042] not found.", exception.Message);
        }

        [TestMethod]
        public void EntityNotFoundException_IntegerId_SetsEntityIdAndFormatsMessage() {
            var exception = new EntityNotFoundException("User", 15);

            Assert.AreEqual("User", exception.EntityName);
            Assert.AreEqual(15, exception.EntityId);
            Assert.AreEqual("Entity 'User' with ID [15] not found.", exception.Message);
        }

        #endregion

        #region InsufficientFundsException Tests

        [TestMethod]
        public void InsufficientFundsException_ValidAmounts_SetsAmountsAndFormatsMessage() {
            var exception = new InsufficientFundsException(requiredAmount: 250.5, currentBalance: 100.0);

            Assert.IsInstanceOfType(exception, typeof(ECommerceException));
            Assert.AreEqual(250.5, exception.RequiredAmount, 0.001);
            Assert.AreEqual(100.0, exception.CurrentBalance, 0.001);
            Assert.AreEqual("Not enough! Requires: 250,5$, balance: 100$.", exception.Message);
        }

        #endregion

        #region InvalidDomainDataException Tests

        [TestMethod]
        public void InvalidDomainDataException_ValidArguments_SetsPropertyNameAndFormatsMessage() {
            var exception = new InvalidDomainDataException("Price", "Value must be positive.");

            Assert.IsInstanceOfType(exception, typeof(ECommerceException));
            Assert.AreEqual("Price", exception.PropertyName);
            Assert.AreEqual("Validation error for the field 'Price': Value must be positive.", exception.Message);
        }

        #endregion
    }
}
