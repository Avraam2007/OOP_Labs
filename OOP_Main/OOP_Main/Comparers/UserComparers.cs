using OOP_Main.States;
using System.Collections.Generic;

namespace OOP_Main.Comparers {
    public class UsernameComparer : IComparer<User> {
        public int Compare(User x, User y) {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            return string.Compare(x.Username, y.Username, System.StringComparison.OrdinalIgnoreCase);
        }
    }

    public class UserRoleComparer : IComparer<User> {
        public int Compare(User x, User y) {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            return string.Compare(
                UserRoleExtensions.GetDescription(x.Role), 
                UserRoleExtensions.GetDescription(y.Role), 
                System.StringComparison.OrdinalIgnoreCase
            );
        }
    }

    public class UserIDComparer : IComparer<User> {
        public int Compare(User x, User y) {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            return x.Id.CompareTo(y.Id);
        }
    }
}
