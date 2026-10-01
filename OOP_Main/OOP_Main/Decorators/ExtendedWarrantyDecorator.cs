using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main {
    public class ExtendedWarrantyDecorator : ProductDecorator {
        private int _years;

        public int Years {
            get => _years;
            set => _years = value > 0 ? value : 1;
        }

        [JsonConstructor]
        public ExtendedWarrantyDecorator() : base() { }
        public ExtendedWarrantyDecorator(Product product, int years = 1) : base(product) {
            _years = years;
        }

        protected override string GetDecoratedArticle(string baseArticle) => $"EXTWAR-{baseArticle}";

        public override double CalculateShippingCost() => TargetProduct?.CalculateShippingCost() ?? 0;

        public override double Price => TargetProduct.Price + (TargetProduct.Price * 0.10 * _years);
        public override string Name => $"{TargetProduct.Name} (+{_years} yr Warranty)";
    }
}
