namespace OOP_Main.States {
    public class LoggedInState: UserState {
        public override bool CanBuyProducts() => true;
        public override bool CanManageUsers() => false;
        public override bool CanEditCatalog() => false;
        public override UserRole GetRoleName() => UserRole.Customer;
    }
}
