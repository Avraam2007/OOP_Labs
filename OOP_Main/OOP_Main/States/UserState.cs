using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main.States {
    public enum UserRole {
        Guest,
        Customer,
        Administrator
    }

    public static class UserRoleExtensions {
        public static string GetDescription(UserRole status) {
            switch (status) {
                case UserRole.Administrator:
                    return "Administrator";
                case UserRole.Customer:
                    return "Customer";
                case UserRole.Guest:
                    return "Guest";
                default:
                    return "Unknown";
            }
        }
    }

    public abstract class UserState {
        protected User _user;

        public void SetUser(User user) {
            this._user = user;
        }

        public abstract bool CanBuyProducts();
        public abstract bool CanManageUsers();
        public abstract bool CanEditCatalog();
        public abstract UserRole GetRoleName();
    }
}
