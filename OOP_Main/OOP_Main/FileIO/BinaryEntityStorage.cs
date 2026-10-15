using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main {
    public static class BinaryEntityStorage {
        public static void SaveProductsBinary(string filePath, List<Product> products) {
            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }

            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (var writer = new BinaryWriter(fs)) {
                writer.Write(products.Count);
                foreach (var p in products) {
                    writer.Write(p.Article ?? string.Empty);
                    writer.Write(p.Name ?? string.Empty);
                    writer.Write(p.Price);
                    writer.Write(p.SupplierId);
                }
            }
        }

        public static List<Product> LoadProductsBinary(string filePath) {
            var result = new List<Product>();
            if (!File.Exists(filePath)) return result;

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(fs)) {
                int count = reader.ReadInt32();
                for (int i = 0; i < count; i++) {
                    string article = reader.ReadString();
                    string name = reader.ReadString();
                    double price = reader.ReadDouble();
                    int supplierId = reader.ReadInt32();

                    Product product;

                    if (article.StartsWith("EL-")) {
                        product = new ElectronicProduct(article, name, price, power: 100, maxVoltage: 220, supplierId);
                    }
                    else if (article.StartsWith("FN-")) {
                        product = new Furniture(article, name, price, weight: 10, width: 50, length: 50, height: 80, material: "Wood", supplierId);
                    }
                    else if (article.StartsWith("SF-")) {
                        product = new Sofa(article, name, price, weight: 30, width: 100, length: 200, height: 90, material: "Fabric", isAssemble: true, supplierId);
                    }
                    else if (article.StartsWith("BDL-")) {
                        product = new ProductBundle(article, name, supplierId, discountPercent: 10);
                    }
                    else {
                        product = new Cloth(article, name, price, material: "Cotton", supplierId);
                    }

                    result.Add(product);
                }
            }
            return result;
        }
    }
}
