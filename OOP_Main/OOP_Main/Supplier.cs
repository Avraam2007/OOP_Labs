using Spectre.Console;
using System;
using System.Collections.Generic;

namespace OOP_Main {
    public class Supplier {
        private double rating;
        public int SupplierId { get; set; }
        public string Name { get; set; }
        public string ContactEmail { get; set; }
        public double Rating {
            get {
                return rating;
            }
            set {
                if (value < 1 || value > 5) {
                    throw new ArgumentOutOfRangeException("Rating should range between 0 and 1");
                }
                rating = value;
            }
        }
        public List<Product> Catalog { get; private set; }

        public Supplier(int supplierId, string name, string contactEmail, double rating) {
            Name = name;
            ContactEmail = contactEmail;
            Rating = rating;
            Catalog = new List<Product>();
            SupplierId = supplierId;
        }

        public void AddPartToCatalog(Product product) {
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

        //public void PrintInfo() {
        //    AnsiConsole.MarkupLine($"Supplier: {Name} | Rating: {Rating}/5.0 | Catalog Position: {Catalog.Count}");
        //}
        public void ShowInfo() {
            AnsiConsole.MarkupLine($"====================SUPPLIER===========================\n");
            AnsiConsole.MarkupLine($"" +
                $"Supplier ID:\t {this.SupplierId}\n" +
                $"Rating:\t\t {this.Rating}/5.0\n" +
                $"Name:\t\t {this.Name}\n" +
                $"Reputation:\t {(this.Inspect() ? "Good" : "Bad")}\n" +
                $"Catalog:\n");
            foreach (var product in Catalog) {
                product.ShowInfo();
                AnsiConsole.WriteLine();
            }
        }

        public bool Inspect() => Rating >= 3.0;
    }
}
