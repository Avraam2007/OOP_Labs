using OOP_Main.States;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main {
    public static class TextEntityStorage {
        public static void SaveUsersToText(string filePath, List<User> users) {
            string directory = Path.GetDirectoryName(filePath);
            FileService.EnsureDirectoryExists(directory);

            // try-with-resources аналог у C#: using
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(fs, Encoding.UTF8)) {
                foreach (var u in users) {
                    writer.WriteLine($"{u.Id};{u.Username};{u.Password};{u.Role};{u.Account.Sum}");
                }
            }
        }

        public static List<User> LoadUsersFromText(string filePath) {
            var result = new List<User>();
            if (!File.Exists(filePath)) return result;

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(fs, Encoding.UTF8)) {
                string line;
                while ((line = reader.ReadLine()) != null) {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var parts = line.Split(';');
                    if (parts.Length >= 5 &&
                        int.TryParse(parts[0], out int id) &&
                        double.TryParse(parts[4], out double sum) &&
                        Enum.TryParse(parts[3], out UserRole role)) {
                        result.Add(new User(id, parts[1], parts[2], role, sum));
                    }
                }
            }
            return result;
        }

        public static void SaveProductsToText(string filePath, List<Product> products) {
            string directory = Path.GetDirectoryName(filePath);
            FileService.EnsureDirectoryExists(directory);

            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(fs, Encoding.UTF8)) {
                foreach (var p in products) {
                    writer.WriteLine($"{p.Article};{p.Name};{p.Price};{p.SupplierId}");
                }
            }
        }
        public static List<Product> LoadProductsFromText(string filePath) {
            var result = new List<Product>();
            if (!File.Exists(filePath)) return result;

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(fs, Encoding.UTF8)) {
                string line;
                while ((line = reader.ReadLine()) != null) {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var parts = line.Split(';');
                    if (parts.Length >= 4 &&
                        double.TryParse(parts[2], out double price) &&
                        int.TryParse(parts[3], out int supplierId)) {
                        string article = parts[0];
                        string name = parts[1];

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
            }
            return result;
        }
    }
}
