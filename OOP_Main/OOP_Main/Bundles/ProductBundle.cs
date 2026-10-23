using Newtonsoft.Json;
using OOP_Main.Attributes;
using OOP_Main.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main{
    public class ProductBundle: Product {
        private double _discountPercent;
        public List<Product> Items { get; set; } = new List<Product>();

        [Editable("Bundle discount percent")]
        public double BundleDiscountPercent {
            get => _discountPercent;
            set {
                if (value < 0 || value > 100) {
                    throw new ArgumentOutOfRangeException($"Discount percent should range between 0% and 100%", nameof(value));
                }
                _discountPercent = value;
            }
        }

        [JsonConstructor]
        public ProductBundle() : base() { }

        public ProductBundle(string article, string name, int supplierId, double discountPercent = 10)
            : base(article, name, 0, supplierId) {
            BundleDiscountPercent = discountPercent;
        }

        public void AddProduct(Product product) {
            if (product != null) {
                Items.Add(product);
            }
        }

        public void RemoveProduct(Product product) {
            Items.Remove(product);
        }

        public override double Price {
            get {
                if (Tools.ValidateArray(Items)) return 0;
                double total = Items.Sum(p => p.Price);
                double discountedPrice = total * (1.0 - (BundleDiscountPercent / 100.0));
                return discountedPrice.RoundPrice();
            }
            set => base.Price = value;
        }

        public override double CalculateShippingCost() {
            if (Tools.ValidateArray(Items)) return 0;
            return Tools.RoundPrice(Items.Sum(p => p.CalculateShippingCost()));
        }

        public override string ToString() {
            return $"({Article}) {Name} |{Items.Count} items| - {Price}$";
        }
    }
}
