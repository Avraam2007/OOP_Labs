using System;
using System.Collections.Generic;

namespace OOP_Main {
    public class Supplier : IQualityCheckable {
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
        public List<Part> Catalog { get; private set; }

        public Supplier(int supplierId, string name, string contactEmail, double rating) {
            Name = name;
            ContactEmail = contactEmail;
            Rating = rating;
            Catalog = new List<Part>();
            SupplierId = supplierId;
        }

        public void AddPartToCatalog(Part part) {
            if (part != null) {
                Catalog.Add(part);
            }
        }

        public Part GetProductFromCatalogByName(string name) {
            return Catalog?.Find(part => part.Name == name);
        }

        public Part GetProductFromCatalogByArticle(string article) {
            return Catalog?.Find(part => part.Article == article);
        }

        public bool CanProvidePart(string partName) {
            return Catalog.Exists(p => p.Name.Equals(partName, StringComparison.OrdinalIgnoreCase));
        }

        //public void PrintInfo() {
        //    Console.WriteLine($"Supplier: {Name} | Rating: {Rating}/5.0 | Catalog Position: {Catalog.Count}");
        //}
        public void ShowInfo() {
            Console.WriteLine($"====================SUPPLIER===========================\n");
            Console.WriteLine($"" +
                $"Supplier ID:\t {this.SupplierId}\n" +
                $"Rating:\t\t {this.Rating}/5.0\n" +
                $"Name:\t\t {this.Name}\n" +
                $"Reputation:\t {(this.Inspect() ? "Good" : "Bad")}\n" +
                $"Catalog:\n");
            foreach (var product in Catalog) {
                product.ShowInfo();
                Console.WriteLine();
            }
        }

        public bool Inspect() => Rating >= 3.0;
    }
}
