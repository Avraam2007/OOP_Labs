using System.ComponentModel;

namespace OOP_Main.Utilities {
    public enum MenuOption {
        [Description("Back to menu")] BackToMenu,
        [Description("Show users")] ShowUsers,
        [Description("Show orders")] ShowOrders,
        [Description("Show suppliers")] ShowSuppliers,
        [Description("Search products")] SearchProducts,
        [Description("Show products by category")] ShowProductsCategory,
        [Description("Show all products")] ShowProducts,
        [Description("My account")] MyAccount,
        [Description("Create order")] CreateOrder,
        [Description("Add product")] AddProduct,
        [Description("Add supplier")] AddSupplier,
        [Description("Change product field")] ChangeProductPrice,
        [Description("Change order field")] ChangeOrderField,
        [Description("Change supplier field")] ChangeSupplierField,
        [Description("Change user field")] ChangeUserField,
        [Description("Delete product")] DeleteProduct,
        [Description("Delete user")] DeleteUser,
        [Description("Delete supplier")] DeleteSupplier,
        [Description("Delete order")] DeleteOrder,
        [Description("Sign up")] SignUp,
        [Description("Log in")] LogIn,
        [Description("Quit")] Quit
    }
}
