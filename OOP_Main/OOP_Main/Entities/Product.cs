using OOP_Main.Attributes;
using OOP_Main.Utilities;
using System;

namespace OOP_Main {
    public abstract class Product: IIdentifiable<string>, IComparable<Product> {
        private string article;
        private string name;
        private double price;
        public int SupplierId { get; set; }

        public virtual string Category => GetType().Name;

        string IIdentifiable<string>.Id => this.Article;
        public virtual string Article {
            get {
                return article;
            }
            set {
                string polishedArticle = value.Trim();
                if (string.IsNullOrWhiteSpace(polishedArticle)) {
                    throw new ArgumentNullException(
                        message: "Article shouldn\'t be empty", 
                        paramName: nameof(polishedArticle)
                    );
                }
                if (!Tools.IsValidArticle(polishedArticle)) {
                    throw new ArgumentException(
                        message: "Article doesn't meet requirements", 
                        paramName: nameof(polishedArticle)
                    );
                }
                article = polishedArticle;
            }
        }

        [Editable("Product name")]
        public virtual string Name {
            get {
                return name;
            }
            set {
                string polishedName = value.Trim();
                if (string.IsNullOrWhiteSpace(polishedName)) {
                    throw new ArgumentException(
                        message: "Name shouldn\'t be empty", 
                        paramName: nameof(polishedName)
                    );
                }
                name = polishedName;
            }
        }

        [Editable("Product price ($)")]
        public virtual double Price {
            get {
                return price;
            }
            set {
                if (value < 0) {
                    throw new ArgumentOutOfRangeException(
                        message: "Price should be positive or equal to zero (free)", 
                        paramName: nameof(value)
                    );
                }
                price = Math.Round(value, 2, MidpointRounding.AwayFromZero);
            }
        }
        protected Product() { }
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
        public override string ToString() => $"({Article}) {Name} - {Price}$";
        public int CompareTo(Product other) {
            if (other == null) return 1;
            return string.Compare(this.Article, other.Article, StringComparison.OrdinalIgnoreCase);
        }
        public abstract double CalculateShippingCost();
    }
}
