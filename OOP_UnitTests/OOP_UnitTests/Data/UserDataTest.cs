using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Entities;
using OOP_Main.Repositories;
using OOP_Main.States;

namespace OOP_UnitTests.Repositories {
    [TestClass]
    public class UserDataTest {
        private UserData _userData;

        [TestInitialize]
        public void Setup() {
            _userData = new UserData();
        }

        [TestCleanup]
        public void Cleanup() {
            _userData = null;
        }

        #region Add Tests

        [TestMethod]
        public void AddUser_StandardUser_AddsToList() {
            var User = new User(id: 1, username: "CoolGuy", password: "1234", Role: UserRole.Customer, startSum: 12.0);

            _userData.AddUser(User);

            Assert.AreEqual(1, _userData.Users.Count);
        }

        [TestMethod]
        public void AddUser_UserBuilder_AddsToListAndDefinesNewID() {
            _userData.AddUser("User 1", "password123");
            _userData.AddUser("User 2", "password456");

            Assert.AreEqual(2, _userData.Users.Count);
            Assert.AreEqual(1, _userData.Users[0].Id);
            Assert.AreEqual(2, _userData.Users[1].Id);
        }

        #endregion

        #region Get & Delete Tests

        [TestMethod]
        public void GetUserByIdAndName_ExistingUser_ReturnsUser() {
            var testUser = new User(1, "SuperUser", "secret");
            _userData.AddUser(testUser);

            var byId = _userData.GetUserById(1);
            var byName = _userData.GetUserByUsername("SuperUser");

            Assert.IsTrue(byId.HasValue());
            Assert.AreEqual("SuperUser", byId.Value.Username);

            Assert.IsTrue(byName.HasValue());
            Assert.AreEqual(1, byName.Value.Id);
        }

        [TestMethod]
        public void GetUserByIdAndName_NonExistingUser_ReturnsEmptyObject() {
            var byName = _userData.GetUserByUsername("SuperUser");
            var byId = _userData.GetUserById(999);

            Assert.IsTrue(byId.IsEmpty());
            Assert.IsTrue(byName.IsEmpty());
        }

        [TestMethod]
        public void DeleteUserByName_ExistingUser_RemovesFromList() {
            _userData.AddUser(new User(1, "MegaUser", "12345678"));

            bool isDeleted = _userData.DeleteUserByUsername("MegaUser");

            Assert.IsTrue(isDeleted);
            Assert.AreEqual(0, _userData.Users.Count);
        }

        [TestMethod]
        public void DeleteUserByName_NonExistentUser_ReturnsFalse() {
            bool isDeleted = _userData.DeleteUserByUsername("NonExistent");

            Assert.IsFalse(isDeleted);
        }

        [TestMethod]
        public void DeleteUserById_ExistingUser_RemovesUser() {
            _userData.AddUser(new User(3, "Admin", "pass1234", 12.0, true));

            bool isDeleted = _userData.DeleteUserById(3);

            Assert.IsTrue(isDeleted);
            Assert.AreEqual(0, _userData.Users.Count);
        }

        [TestMethod]
        public void DeleteUserId_NonExistentId_ThrowsExceptionOrReturnsFalse() {
            bool isDeleted = _userData.DeleteUserById(999);

            Assert.IsFalse(isDeleted);
        }

        #endregion
    }
}
