using Spectre.Console;
using System;
using System.Collections.Generic;

namespace OOP_Main {
    public class Supplier {
        private double rating;
        private int supplierId;
        public int SupplierId { get => supplierId; }
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
            this.supplierId = supplierId;
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

        public bool Inspect() => Rating >= 3.0;

        public override bool Equals(object obj) {
            if (obj is Supplier other) return this.SupplierId == other.SupplierId;
            return false;
        }
        public override int GetHashCode() => SupplierId.GetHashCode();
        public override string ToString() => $"[Supplier] {Name} (Email: {ContactEmail}) (Rating: {Rating}/5.0)";
    }
}
