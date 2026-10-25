using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;
using NOptional;
using OOP_Main.Entities;

namespace OOP_Main.Decorators {
    public abstract class ProductDecorator: Product {
        protected Product _targetProduct;

        [JsonProperty("TargetProduct")]
        protected Product RawTargetProduct { 
            get => _targetProduct; 
            set => _targetProduct = value; 
        }

        [JsonIgnore]
        public IOptional<Product> TargetProduct => Optional.OfNullable(_targetProduct);

        public override string Article {
            get => TargetProduct.HasValue() ? GetDecoratedArticle(TargetProduct.Value.Article) : base.Article;
            set => base.Article = value;
        }

        [JsonConstructor]
        public ProductDecorator() : base() { }
        public ProductDecorator(Product product)
            : base(product.Article ?? string.Empty, product.Name ?? string.Empty, product.Price, product.GetSupplierId()) {
            _targetProduct = product ?? throw new ArgumentNullException(nameof(product), "Target product cannot be null!");
        }

        protected virtual string GetDecoratedArticle(string baseArticle) => $"DEC-{baseArticle}";

        public override double CalculateShippingCost() => TargetProduct
            .MapValue(product => product.CalculateShippingCost())
            .GetValueOrElse(0);

        public string GetName() => TargetProduct
            .MapValue(product => product.Name)
            .GetValueOrElseThrow();

        public double GetProductPrice() => TargetProduct
            .MapValue(product => product.Price)
            .GetValueOrElse(0);

        public override bool Equals(object obj) {
            return ReferenceEquals(this, obj);
        }

        public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);
    }
}
