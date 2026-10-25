namespace OOP_Main.States {
    public class GuestState: UserState {
        public override bool CanBuyProducts() => false;
        public override bool CanManageUsers() => false;
        public override bool CanEditCatalog() => false;
        public override UserRole GetRoleName() => UserRole.Guest;
    }
}
