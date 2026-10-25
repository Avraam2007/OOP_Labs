namespace OOP_Main.States {
    public class AdminState: UserState {
        public override bool CanBuyProducts() => true;
        public override bool CanManageUsers() => true;
        public override bool CanEditCatalog() => true;
        public override UserRole GetRoleName() => UserRole.Administrator;
    }
}
