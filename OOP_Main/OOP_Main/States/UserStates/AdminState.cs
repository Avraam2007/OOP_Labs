using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main.States {
    public class AdminState: UserState {
        public override bool CanBuyProducts() => true;
        public override bool CanManageUsers() => true;
        public override bool CanEditCatalog() => true;
        public override string GetRoleName() => "Administrator";
    }
}
