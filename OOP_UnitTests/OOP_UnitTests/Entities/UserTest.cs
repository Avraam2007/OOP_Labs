using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Entities;
using OOP_Main.States;
using System;

namespace OOP_UnitTests.Entities {
    [TestClass]
    public class UserTest {
        #region User Constructors & Roles Tests

        [TestMethod]
        public void User_ConstructorWithZeroId_AssignsGuestRole() {
            var guest = new User(0, "Guest", "guestpass");

            Assert.AreEqual(0, guest.Id);
            Assert.AreEqual(UserRole.Guest, guest.Role);
            Assert.IsTrue(guest.IsGuest());
            Assert.IsFalse(guest.IsAdmin());
            Assert.IsFalse(guest.CanBuy());
        }

        [TestMethod]
        public void User_ConstructorWithIsAdminTrue_AssignsAdministratorRole() {
            var admin = new User(1, "Admin", "adminpass", startSum: 500.0, isAdmin: true);

            Assert.AreEqual(1, admin.Id);
            Assert.AreEqual(UserRole.Administrator, admin.Role);
            Assert.IsTrue(admin.IsAdmin());
            Assert.IsFalse(admin.IsGuest());
            Assert.IsTrue(admin.CanBuy());
            Assert.AreEqual(500.0, admin.Account.Sum, 0.001);
        }

        [TestMethod]
        public void User_ConstructorStandardCustomer_AssignsCustomerRole() {
            var customer = new User(2, "Alice", "secret123", startSum: 150.0);

            Assert.AreEqual(UserRole.Customer, customer.Role);
            Assert.IsFalse(customer.IsAdmin());
            Assert.IsFalse(customer.IsGuest());
            Assert.IsTrue(customer.CanBuy());
        }

        [TestMethod]
        public void User_JsonConstructor_SetsRoleAndStartSumCorrectly() {
            var user = new User(5, "Bob", "hashedPwd", UserRole.Administrator, startSum: 300.0);

            Assert.AreEqual(5, user.Id);
            Assert.AreEqual(UserRole.Administrator, user.Role);
            Assert.AreEqual(300.0, user.Account.Sum, 0.001);
        }

        #endregion

        #region User Validation & Operations Tests

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void User_EmptyOrWhitespaceUsername_ThrowsArgumentException(string invalidUsername) {
            Assert.Throws<ArgumentException>(() =>
                new User(1, invalidUsername, "validPass"));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void User_EmptyOrWhitespacePassword_ThrowsArgumentException(string invalidPassword) {
            Assert.Throws<ArgumentException>(() =>
            new User(1, "ValidUser", invalidPassword));
        }

        [TestMethod]
        public void User_UpdatePassword_UpdatesPasswordProperty() {
            var user = new User(1, "Alice", "oldHash");

            user.UpdatePassword("newHash123");

            Assert.AreEqual("newHash123", user.Password);
        }

        [TestMethod]
        public void User_TopUpAndTakeMoney_ModifiesAccountBalance() {
            var user = new User(1, "Alice", "pass", startSum: 100.0);

            user.TopUpAccount(200.0);
            Assert.AreEqual(300.0, user.Account.Sum, 0.001);

            user.TakeMoney(120.50);
            Assert.AreEqual(179.50, user.Account.Sum, 0.001);
        }

        #endregion

        #region User Equality, Comparison & Formatting Tests

        [TestMethod]
        public void User_EqualsAndGetHashCode_CompareById() {
            var user1 = new User(10, "Alice", "pass1");
            var user2 = new User(10, "DifferentName", "pass2");
            var user3 = new User(11, "Alice", "pass1");

            Assert.IsTrue(user1.Equals(user2));
            Assert.AreEqual(user1.GetHashCode(), user2.GetHashCode());
            Assert.IsFalse(user1.Equals(user3));
            Assert.IsFalse(user1.Equals(null));
        }

        [TestMethod]
        public void User_CompareTo_ComparesByUsernameIgnoringCase() {
            var alice = new User(1, "alice", "pass");
            var bob = new User(2, "Bob", "pass");
            var aliceUpper = new User(3, "ALICE", "pass");

            Assert.IsLessThan(0, alice.CompareTo(bob));
            Assert.IsGreaterThan(0, bob.CompareTo(alice));
            Assert.AreEqual(0, alice.CompareTo(aliceUpper));
            Assert.AreEqual(1, alice.CompareTo(null));
        }

        [TestMethod]
        public void User_ToString_ReturnsFormattedString() {
            var admin = new User(7, "SuperAdmin", "pass", isAdmin: true);

            Assert.AreEqual("|User #7| SuperAdmin (Administrator)", admin.ToString());
        }

        #endregion
    }
}
