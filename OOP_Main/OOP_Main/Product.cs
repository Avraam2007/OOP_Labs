using Spectre.Console;
using System;
using System.Collections.Generic;

namespace OOP_Main {
    public abstract class Product : IDataAndUIBridge {
        private string article;
        private string name;
        private double price;
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
        public Product(string article, string name, double price) {
            this.Article = article;
            this.Name = name;
            this.Price = price;
        }

        public abstract Dictionary<string, Text> ShowInfo();

        public abstract int GetSupplierId();

        public virtual int GetDeliveryDays() => 3;
    }
}
