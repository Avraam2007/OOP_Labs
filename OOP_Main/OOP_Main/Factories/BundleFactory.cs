using OOP_Main.Bundles;
using OOP_Main.Entities;

namespace OOP_Main.Factories {
    public class BundleFactory : ProductFactory {
        public override Product CreateProduct(string article, string name, double price, int supplierId, string category = "") {
            double discountPercent = DefaultTextPrompt<double>("Enter bundle [green]discount percent[/]:");

            return new ProductBundle(article, name, supplierId, discountPercent);
        }
    }
}
