using OOP_Main.Entities;
using OOP_Main.Factories;
using OOP_Main.Interfaces;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace OOP_Main.FileIO.TextEntities {
    internal class TextProductStorage: IEntityStorage<Product> {
        public void SaveToFile(string filePath, List<Product> products) {
            string directory = Path.GetDirectoryName(filePath);
            FileService.EnsureDirectoryExists(directory);

            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(fs, Encoding.UTF8)) {
                foreach (var p in products) {
                    writer.WriteLine($"{p.Article};{p.Name};{p.Price};{p.SupplierId}");
                }
            }
        }
        public List<Product> LoadFromFile(string filePath) {
            var result = new List<Product>();
            if (!File.Exists(filePath)) return result;

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(fs, Encoding.UTF8)) {
                string line;
                while ((line = reader.ReadLine()) != null) {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var parts = line.Split(';');
                    if (parts.Length >= 4 &&
                        double.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double price) &&
                        int.TryParse(parts[3], out int supplierId)) {
                        string article = parts[0];
                        string name = parts[1];

                        Product product = ProductRestorer.CreateProductByArticle(article, name, price, supplierId);

                        result.Add(product);
                    }
                }
            }
            return result;
        }
    }
}
