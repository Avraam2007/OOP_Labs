using System;

namespace OOP_Main {
    public abstract class Product {
        private string article;
        private string name;
        private double price;
        public int SupplierId { get; set; }
        public string Article {
            get {
                return article;
            }
            set {
                string polishedArticle = value.Trim();
                if (string.IsNullOrWhiteSpace(polishedArticle)) {
                    throw new ArgumentException("Article shouldn\'t be empty", nameof(polishedArticle));
                }
                article = polishedArticle;
            }
        }
        public string Name {
            get {
                return name;
            }
            set {
                string polishedName = value.Trim();
                if (string.IsNullOrWhiteSpace(polishedName)) {
                    throw new ArgumentException("Name shouldn\'t be empty", nameof(polishedName));
                }
                name = polishedName;
            }
        }
        public double Price {
            get {
                return price;
            }
            set {
                if (value < 0) {
                    throw new ArgumentOutOfRangeException("Price should be positive or equal to zero (free)", nameof(value));
                }
                price = Math.Round(value, 2, MidpointRounding.AwayFromZero);
            }
        }
        public Product(string article, string name, double price, int supplierId) {
            this.Article = article;
            this.Name = name;
            this.Price = price;
            this.SupplierId = supplierId;
        }

        public int GetSupplierId() => this.SupplierId;

        public virtual int GetDeliveryDays() => 3;

        public override bool Equals(object obj) {
            if (obj is Product other) return this.Article == other.Article;
            return false;
        }
        public override int GetHashCode() => Article?.GetHashCode() ?? 0;
        public override string ToString() => $"{Name} [{Article}] - {Price}$";
    }
}
