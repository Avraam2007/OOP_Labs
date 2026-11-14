using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Comparers;
using OOP_Main.Entities;
using OOP_Main.States;

namespace OOP_UnitTests.Comparers {
    [TestClass]
    public class UserComparersTest {
        #region UserRoleComparer Tests
        // ID Name Role

        [TestMethod]
        public void UserRoleComparer_ComparesTotalRatingsCorrectly() {
            var comparer = new UserRoleComparer();
            var customer = new User(1, "Customer", "password123", UserRole.Customer);

            var admin = new User(2, "Admin", "password456", UserRole.Administrator);

            var equalCustomer = new User(3, "Same", "password123", UserRole.Customer);

            Assert.IsLessThan(0, comparer.Compare(admin, customer));
            Assert.IsGreaterThan(0, comparer.Compare(customer, admin));
            Assert.AreEqual(0, comparer.Compare(customer, equalCustomer));
        }

        [TestMethod]
        public void UserRoleComparer_HandlesNullValues() {
            var comparer = new UserRoleComparer();
            var user = new User(1, "asd", "123", UserRole.Customer);

            Assert.AreEqual(0, comparer.Compare(null, null));
            Assert.AreEqual(-1, comparer.Compare(null, user));
            Assert.AreEqual(1, comparer.Compare(user, null));
        }

        #endregion

        #region UsernameComparer Tests

        [TestMethod]
        public void UsernameComparer_ComparesUsernamesIgnoringCase() {
            var comparer = new UsernameComparer();
            var userAlice = new User(1, "alice", "1234");
            var userBob = new User(2, "Bob", "1234");
            var userAliceUpper = new User(3, "ALICE", "1234");

            Assert.IsLessThan(0, comparer.Compare(userAlice, userBob));
            Assert.IsGreaterThan(0, comparer.Compare(userBob, userAlice));
            Assert.AreEqual(0, comparer.Compare(userAlice, userAliceUpper));
        }

        [TestMethod]
        public void UsernameComparer_HandlesNullValues() {
            var comparer = new UsernameComparer();
            var user = new User(1, "asd", "123");

            Assert.AreEqual(0, comparer.Compare(null, null));
            Assert.AreEqual(-1, comparer.Compare(null, user));
            Assert.AreEqual(1, comparer.Compare(user, null));
        }

        #endregion

        #region UserIDComparer Tests

        [TestMethod]
        public void UserIDComparer_ComparesUserIdsCorrectly() {
            var comparer = new UserIDComparer();
            var user1 = new User(1, "User 1", "password456");
            var user2 = new User(2, "User 2", "password123");

            var user1Duplicate = new User(1, "Supplier 3", "password789");

            Assert.IsLessThan(0, comparer.Compare(user1, user2));
            Assert.IsGreaterThan(0, comparer.Compare(user2, user1));
            Assert.AreEqual(0, comparer.Compare(user1, user1Duplicate));
        }

        [TestMethod]
        public void UserIDComparer_HandlesNullValues() {
            var comparer = new UserIDComparer();
            var user = new User(1, "asd", "123");

            Assert.AreEqual(0, comparer.Compare(null, null));
            Assert.AreEqual(-1, comparer.Compare(null, user));
            Assert.AreEqual(1, comparer.Compare(user, null));
        }

        #endregion
    }
}
