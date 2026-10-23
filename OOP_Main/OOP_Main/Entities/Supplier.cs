using OOP_Main.Attributes;
using OOP_Main.Utilities;
using Spectre.Console;
using System;
using System.Collections.Generic;

namespace OOP_Main {
    public class Supplier: IIdentifiable<int> {
        private double rating;
        private readonly int supplierId;
        private string _contactEmail;
        private string _phone;

        public int SupplierId { get => supplierId; }

        int IIdentifiable<int>.Id => this.SupplierId;

        [Editable("Supplier name")]
        public string Name { get; set; }

        [Editable("Supplier email")]
        public string ContactEmail { 
            get => this._contactEmail;
            set {
                if (!Tools.IsValidEmail(value)) {
                    throw new ArgumentException("Email doesn't meet requirements", nameof(value));
                }
                this._contactEmail = value;
            } 
        }

        [Editable("Supplier phone")]
        public string Phone {
            get => this._phone;
            set {
                if (!Tools.IsValidPhone(value)) {
                    throw new ArgumentException("Phone number doesn't meet requirements", nameof(value));
                }
                this._phone = value;
            }
        }

        [Editable("Supplier rating")]
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

        public Supplier(int supplierId, string name, string contactEmail, string phone, double rating) {
            Name = name;
            ContactEmail = contactEmail;
            Phone = phone;
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
        public override string ToString() => $"|Supplier| {Name}";
    }
}
