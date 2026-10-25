using OOP_Main.Entities;
using OOP_Main.UI;

namespace OOP_Main.Factories {
    public abstract class ProductFactory : UIHelper {
        public abstract Product CreateProduct(string article, string name, double price, int supplierId, string category = "");
    }
}
