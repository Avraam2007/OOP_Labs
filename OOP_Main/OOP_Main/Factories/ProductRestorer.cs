using OOP_Main.Bundles;
using OOP_Main.Entities;

namespace OOP_Main.Factories {
    public static class ProductRestorer {
        public static Product CreateProductByArticle(string article, string name, double price, int supplierId) {
            if (article.StartsWith("EL-"))
                return new ElectronicProduct(article, name, price, power: 100, maxVoltage: 220, supplierId);
            if (article.StartsWith("FN-"))
                return new Furniture(article, name, price, weight: 10, width: 50, length: 50, height: 80, material: "Wood", supplierId);
            if (article.StartsWith("SF-"))
                return new Sofa(article, name, price, weight: 30, width: 100, length: 200, height: 90, material: "Fabric", isAssemble: true, supplierId);
            if (article.StartsWith("BDL-"))
                return new ProductBundle(article, name, supplierId, discountPercent: 10);

            return new Cloth(article, name, price, material: "Cotton", supplierId);
        }
    }
}
