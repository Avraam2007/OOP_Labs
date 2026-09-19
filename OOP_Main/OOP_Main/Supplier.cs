using Spectre.Console;
using System;
using System.Collections.Generic;

namespace OOP_Main {
    public class Supplier : IDataAndUIBridge {
        private double rating;
        public int SupplierId { get; set; }
        public string Name { get; set; }
        public string ContactEmail { get; set; }
        public double Rating {
            get {
                return rating;
            }
            set {
                const int minRatingValue = 1;
                const int maxRatingValue = 5;
                if (value < minRatingValue || value > maxRatingValue) {
                    throw new ArgumentOutOfRangeException($"Rating should range between {minRatingValue} and {maxRatingValue}", nameof(value));
                }
                rating = value;
            }
        }
        public List<Product> Catalog { get; private set; } = new List<Product>();

        public Supplier(int supplierId, string name, string contactEmail, double rating) {
            Name = name;
            ContactEmail = contactEmail;
            Rating = rating;
            SupplierId = supplierId;
        }

        public void AddProductToCatalog(Product product) {
            if (product != null) {
                Catalog.Add(product);
            }
        }

        public Product GetProductFromCatalogByName(string name) {
            return Catalog?.Find(product => product.Name == name);
        }

        public Product GetProductFromCatalogByArticle(string article) {
            return Catalog?.Find(product => product.Article == article);
        }

        public bool CanProvidePart(string productName) {
            return Catalog.Exists(p => p.Name.Equals(productName, StringComparison.OrdinalIgnoreCase));
        }

        public Dictionary<string, Text> ShowInfo() {
            Dictionary<string, Text> textsForRender = new Dictionary<string, Text> {
                ["header"] = new Text($"SUPPLIER \"{this.Name}\"\n\n", new Style(decoration: Decoration.Bold)).Centered(),
                ["rating"] = new Text($"Rating:\t\t {this.Rating}/5.0\n"),
                ["id"] = new Text($"Supplier ID:\t {this.SupplierId}\n"),
                ["reputation"] = new Text($"Reputation:\t {(this.Inspect() ? "Good" : "Bad")}\n"),
            };

            if (Tools.ValidateArray(Catalog)) {
                textsForRender["empty_products"] = new Text("  (No products that supplier can provide)\n", new Style(foreground: Color.Red));
                return textsForRender;
            }

            for (int i = 0; i < this.Catalog.Count; i++) {
                var product = this.Catalog[i];

                var productSpecs = product.ShowInfo();

                textsForRender[$"prod_{i}_name"] = new Text($"\n  {i + 1}. [{product.Article}] {product.Name}\n", new Style(foreground: Color.Green, decoration: Decoration.Bold));

                foreach (var spec in productSpecs) {
                    if (spec.Key == "header") continue;

                    string uniqueKey = $"prod_{i}_{spec.Key}";

                    textsForRender[uniqueKey] = spec.Value;
                }
            }

            return textsForRender;
        }

        public bool Inspect() => Rating >= 3.0;
    }
}
