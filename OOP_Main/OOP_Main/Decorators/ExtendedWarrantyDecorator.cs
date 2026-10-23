using Newtonsoft.Json;
using OOP_Main.Attributes;

namespace OOP_Main {
    public class ExtendedWarrantyDecorator : ProductDecorator {
        private int _years;

        [Editable("Extra warranty years")]
        public int Years {
            get => _years;
            set => _years = value > 0 ? value : 1;
        }

        [JsonConstructor]
        public ExtendedWarrantyDecorator() : base() { }
        public ExtendedWarrantyDecorator(Product product, int years = 1) : base(product) {
            _years = years;
        }

        protected override string GetDecoratedArticle(string baseArticle) => $"EXTWAR{Years}-{baseArticle}";

        public override double Price => this.GetProductPrice() + (this.GetProductPrice() * 0.10 * _years);
        public override string Name => $"{this.GetName()} (+{_years} yr Warranty)";
    }
}
