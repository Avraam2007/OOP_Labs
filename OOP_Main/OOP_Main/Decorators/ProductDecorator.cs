using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main {
    public abstract class ProductDecorator: Product {
        protected Product _targetProduct;

        [JsonProperty("TargetProduct")]
        public Product TargetProduct { 
            get => _targetProduct; 
            set => _targetProduct = value; 
        }
        public override string Article {
            get => TargetProduct != null ? GetDecoratedArticle(TargetProduct.Article) : base.Article;
            set => base.Article = value;
        }

        [JsonConstructor]
        public ProductDecorator() : base() { }
        public ProductDecorator(Product product)
            : base(product.Article ?? string.Empty, product.Name ?? string.Empty, product.Price, product.GetSupplierId()) {
            TargetProduct = product ?? throw new ArgumentNullException(nameof(product), "Target product cannot be null!");
        }

        protected virtual string GetDecoratedArticle(string baseArticle) => $"DEC-{baseArticle}";

        public override double CalculateShippingCost() => TargetProduct?.CalculateShippingCost() ?? 0;

        public override bool Equals(object obj) {
            return ReferenceEquals(this, obj);
        }

        public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);
    }
}
