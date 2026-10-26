using OOP_Main.Entities;
using OOP_Main.Factories;
using OOP_Main.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace OOP_Main.FileIO.BinaryEntities {
    public class BinaryProductStorage: IEntityStorage<Product> {
        public void SaveToFile(string filePath, List<Product> products) {
            string directory = Path.GetDirectoryName(filePath);
            FileService.EnsureDirectoryExists(directory);

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

        public List<Product> LoadFromFile(string filePath) {
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

                    Product product = ProductRestorer.CreateProductByArticle(article, name, price, supplierId);

                    result.Add(product);
                }
            }
            return result;
        }
    }
}
