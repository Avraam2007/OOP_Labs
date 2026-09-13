using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main {
    public abstract class Product {
        private string article;
        private string name;
        private double price;
        public string Article {
            get {
                return article;
            }
            set {
                if (value.Trim().Length == 0) {
                    throw new ArgumentException("Article shouldn\'t be empty");
                }
                article = value;
            }
        }
        public string Name {
            get {
                return name;
            }
            set {
                if (value.Trim().Length == 0) {
                    throw new ArgumentException("Name shouldn\'t be empty");
                }
                name = value;
            }
        }
        public double Price {
            get {
                return price;
            }
            set {
                if (value < 0) {
                    throw new ArgumentOutOfRangeException("Price should be posititve or equal to zero (free)");
                }
                price = Math.Round(value, 2, MidpointRounding.AwayFromZero);
            }
        }
        public Product(string article, string name, double price) {
            this.Article = article;
            this.Name = name;
            this.Price = price;
        }

        public abstract void ShowInfo();

        public abstract int GetSupplierId();

        public virtual int GetDeliveryDays() => 3;
    }
}
