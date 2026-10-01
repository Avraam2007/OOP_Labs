using System.ComponentModel;

namespace OOP_Main {
    public enum MenuOption {
        [Description("Back to menu")] BackToMenu,
        [Description("Show users")] ShowUsers,
        [Description("Show orders")] ShowOrders,
        [Description("Show suppliers")] ShowSuppliers,
        [Description("Show products")] ShowProducts,
        [Description("Create order")] CreateOrder,
        [Description("Add product")] AddProduct,
        [Description("Add supplier")] AddSupplier,
        [Description("Change product field")] ChangeProductPrice,
        [Description("Delete product")] DeleteProduct,
        [Description("Delete user")] DeleteUser,
        [Description("Delete supplier")] DeleteSupplier,
        [Description("Delete order")] DeleteOrder,
        [Description("Sign up")] SignUp,
        [Description("Log in")] LogIn,
        [Description("Quit")] Quit
    }
}
