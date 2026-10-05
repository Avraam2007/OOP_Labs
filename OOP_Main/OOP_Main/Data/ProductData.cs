using NOptional;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OOP_Main {
    public class ProductData: BaseDataRepository<Product, string> {
        public List<Product> Products => Items;

        public ProductData() : base("DataStorage/products.json") { }

        public void AddProduct(Product product) => Add(product);

        public bool DeleteProductByName(string name) {
            var productToDelete = this.GetProductByName(name);

            if (productToDelete.IsEmpty()) {
                return false;
            }

            Products.Remove(productToDelete.GetValueOrElseThrow());
            this.Save();

            return true;
        }

        public bool DeleteProductByArticle(string article) => Remove(article);

        public override IOptional<Product> Get(string article) {
            var foundProduct = Products.Find((product) => product.Article == article);
            return Optional.OfNullable(foundProduct);
        }

        public IOptional<Product> GetProductByArticle(string article) => this.Get(article);

        public IOptional<Product> GetProductByName(string name) {
            var foundProduct = Products.Find((product) => product.Name == name);
            return Optional.OfNullable(foundProduct);
        }

        public void ChangeProductPriceByArticle(string article, double newPrice) {
            Products.Find((product) => product.Article == article).Price = newPrice;
        }

        private static Product GetBaseProduct(Product product) {
            // Рекурсивно знімаємо всі декоратори, щоб дістатися до Cloth, Sofa тощо
            while (product is ProductDecorator decorator && decorator.TargetProduct != null) {
                product = decorator.TargetProduct;
            }
            return product;
        }

        public string GenerateNextProductArticle(Type productType) {
            var foundExistingProducts = Products
                .Select(p => GetBaseProduct(p))
                .Where(p => p.GetType() == productType)
                .ToList();

            IOptional<List<Product>> existingProducts = Optional.Of(foundExistingProducts);

            string prefix = "PRD";

            if (productType == typeof(ElectronicProduct)) prefix = "EL";
            else if (productType == typeof(Cloth)) prefix = "CL";
            else if (productType == typeof(Sofa)) prefix = "SF";
            else if (productType == typeof(Furniture)) prefix = "FN";

            int maxNumber = 0;

            if (existingProducts.IsEmpty()) {
                throw new NullReferenceException();
            }

            foreach (var product in existingProducts.GetValueOrElseThrow()) {
                if (product.Article != null && product.Article.StartsWith(prefix + "-")) {
                    string numberPart = product.Article.Substring(prefix.Length + 1);
                    if (int.TryParse(numberPart, out int num) && num > maxNumber) {
                        maxNumber = num;
                    }
                }
            }

            return $"{prefix}-{(maxNumber + 1):D4}";
        }

    }
}
