using NOptional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using static OOP_Main.Utilities.Exceptions;

namespace OOP_Main {
    public class ProductData: BaseDataRepository<Product, string> {
        public List<Product> Products => Items;
        private readonly HashSet<string> _registeredArticles = new HashSet<string>();

        public ProductData() : base("DataStorage/products.json") { }

        public override void Load() {
            base.Load();

            _registeredArticles.Clear();
            foreach (var product in Products) {
                var baseProduct = GetBaseProduct(product);
                if (!string.IsNullOrEmpty(baseProduct.Article))
                    _registeredArticles.Add(baseProduct.Article);
            }
        }

        public void AddProduct(Product product) {
            Add(product);
            var baseProduct = GetBaseProduct(product);
            _registeredArticles.Add(baseProduct.Article);
        }

        public bool IsArticleExists(string article) => _registeredArticles.Contains(article);

        public bool DeleteProductByName(string name) {
            var productToDelete = this.GetProductByName(name);

            if (productToDelete.IsEmpty()) return false;

            Products.Remove(productToDelete.GetValueOrElseThrow());

            var baseProduct = GetBaseProduct(productToDelete.Value);
            _registeredArticles.Remove(baseProduct.Article);
            this.Save();

            return true;
        }

        public bool DeleteProductByArticle(string article) {
            var productToDelete = this.Get(article);
            var baseProduct = GetBaseProduct(productToDelete.Value);

            bool isRemoved = Remove(article);
            if (isRemoved) {
                _registeredArticles.Remove(baseProduct.Article);
                return true; 
            } 
            return false;
        }

        protected override IOptional<Product> Get(string article) {
            var foundProduct = Products.Find((product) => product.Article == article);
            return Optional.OfNullable(foundProduct);
        }

        public IOptional<Product> GetProductByArticle(string article) => this.Get(article);

        public IOptional<Product> GetProductByName(string name) {
            var foundProduct = Products.Find((product) => product.Name == name);
            return Optional.OfNullable(foundProduct);
        }

        public void ChangeProductPriceByArticle(string article, double newPrice) {
            var foundProduct = this.GetProductByArticle(article);
            if (foundProduct.IsEmpty()) {
                throw new EntityNotFoundException(nameof(foundProduct.Value), article);
            }
            this.GetProductByArticle(article).Value.Price = newPrice;
        }

        private static Product GetBaseProduct(Product product) {
            // Рекурсивно знімаємо всі декоратори, щоб дістатися до Cloth, Sofa тощо
            while (product is ProductDecorator decorator && decorator.TargetProduct.HasValue()) {
                product = decorator.TargetProduct.GetValueOrElseThrow();
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
            else if (productType == typeof(ProductBundle)) prefix = "BDL";

            int maxNumber = 0;

            if (existingProducts.IsEmpty()) {
                throw new NullReferenceException();
            }

            foreach (var product in existingProducts.GetValueOrElseThrow()) {
                if (!string.IsNullOrEmpty(product.Article) && product.Article.StartsWith(prefix + "-")) {
                    string numberPart = product.Article.Substring(prefix.Length + 1);
                    if (int.TryParse(numberPart, out int num) && num > maxNumber) {
                        maxNumber = num;
                    }
                }
            }

            int candidateNumber = maxNumber + 1;
            string candidateArticle;

            do {
                candidateArticle = $"{prefix}-{candidateNumber:D4}";
                candidateNumber++;
            } while (_registeredArticles.Contains(candidateArticle));

            return candidateArticle;
        }

    }
}
