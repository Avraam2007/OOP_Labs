using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main.Comparers {
    public class UsernameComparer : IComparer<User> {
        public int Compare(User x, User y) {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            return string.Compare(x.Username, y.Username, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
