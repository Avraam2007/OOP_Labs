using Microsoft.VisualStudio.TestTools.UnitTesting;
using OOP_Main.Entities;
using OOP_Main.States;

namespace OOP_UnitTests.States {
    [TestClass]
    public class UserStateTests {
        #region UserRole Tests

        [TestMethod]
        [DataRow(UserRole.Customer, "Customer")]
        [DataRow(UserRole.Administrator, "Administrator")]
        [DataRow(UserRole.Guest, "Guest")]
        [DataRow((UserRole)999, "Unknown")]
        public void GetDescription_VariousRoles_ReturnsExpectedString(UserRole role, string expectedDescription) {
            string actualDescription = UserRoleExtensions.GetDescription(role);

            Assert.AreEqual(expectedDescription, actualDescription);
        }

        #endregion


        #region AdminState Tests

        [TestMethod]
        public void AdminState_PermissionsAndRole_ReturnsFullAccessAndAdministratorRole() {
            UserState state = new AdminState();
            state.SetUser(new User(1, "AdminUser", "pass1234", isAdmin: true));

            Assert.IsTrue(state.CanBuyProducts());
            Assert.IsTrue(state.CanManageUsers());
            Assert.IsTrue(state.CanEditCatalog());
            Assert.AreEqual(UserRole.Administrator, state.GetRoleName());
        }

        #endregion

        #region GuestState Tests

        [TestMethod]
        public void GuestState_PermissionsAndRole_ReturnsNoAccessAndGuestRole() {
            UserState state = new GuestState();
            state.SetUser(new User(0, "GuestUser", "12345"));

            Assert.IsFalse(state.CanBuyProducts());
            Assert.IsFalse(state.CanManageUsers());
            Assert.IsFalse(state.CanEditCatalog());
            Assert.AreEqual(UserRole.Guest, state.GetRoleName());
        }

        #endregion

        #region LoggedInState Tests

        [TestMethod]
        public void LoggedInState_PermissionsAndRole_ReturnsCustomerAccessAndCustomerRole() {
            UserState state = new LoggedInState();
            state.SetUser(new User(2, "RegularCustomer", "pass1234"));

            Assert.IsTrue(state.CanBuyProducts());
            Assert.IsFalse(state.CanManageUsers());
            Assert.IsFalse(state.CanEditCatalog());
            Assert.AreEqual(UserRole.Customer, state.GetRoleName());
        }

        #endregion

    }
}
