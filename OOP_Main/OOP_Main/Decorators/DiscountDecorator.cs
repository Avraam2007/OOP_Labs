using Newtonsoft.Json;
using OOP_Main.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main {
    public class DiscountDecorator : ProductDecorator {
        private double _discountPercent;

        [Editable("Discount percent")]
        public double DiscountPercent {
            get => _discountPercent;
            set {
                if (value < 0 || value > 100) {
                    throw new ArgumentOutOfRangeException($"Discount percent should range between 0% and 100%", nameof(value));
                }
                _discountPercent = value;
            }
        }

        [JsonConstructor]
        public DiscountDecorator() : base() { }

        public DiscountDecorator(Product product, double discountPercent) : base(product) {
            DiscountPercent = discountPercent;
        }

        protected override string GetDecoratedArticle(string baseArticle) => $"DISC{DiscountPercent}-{baseArticle}";

        public override double Price => TargetProduct.HasValue()
            ? this.GetProductPrice() * (1.0 - (DiscountPercent / 100.0)) : base.Price;
        public override string Name => TargetProduct.HasValue() ? $"{this.GetName()} ({DiscountPercent}% OFF)" : base.Name;
    }
}
