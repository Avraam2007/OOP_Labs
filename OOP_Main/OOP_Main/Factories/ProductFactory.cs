using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OOP_Main.UI;

namespace OOP_Main {
    public abstract class ProductFactory : UIHelper {
        public abstract Product CreateProduct(string article, string name, double price, int supplierId, string category = "");
    }
}
