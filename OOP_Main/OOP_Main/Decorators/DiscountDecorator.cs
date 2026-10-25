using Newtonsoft.Json;
using OOP_Main.Attributes;
using OOP_Main.Entities;
using OOP_Main.Structures;

namespace OOP_Main.Decorators {
    public class DiscountDecorator : ProductDecorator {
        private Discount _discount;

        [Editable("Discount percent")]
        public double DiscountPercent {
            get => _discount.Percent;
            set {
                _discount = new Discount(value);
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
