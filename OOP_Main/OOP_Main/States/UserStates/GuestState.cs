using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main.States {
    public class GuestState: UserState {
        public override bool CanBuyProducts() => false;
        public override bool CanManageUsers() => false;
        public override bool CanEditCatalog() => false;
        public override string GetRoleName() => "Guest";
    }
}
