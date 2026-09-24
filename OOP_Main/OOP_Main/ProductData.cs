using System;
using System.Collections.Generic;

namespace OOP_Main {
    public class ProductData: IBridgeJSON {
        private const string ProductsFilePath = "DataStorage/products.json";
        public List<Product> Products { get; private set; } = new List<Product>();

        public void Load() {
            Products = JsonStorage.LoadFromFile<List<Product>>(ProductsFilePath);
        }
        public void Save() {
            JsonStorage.SaveToFile(ProductsFilePath, Products);
        }

        public void AddProduct(Product product) {
            Products.Add(product);
            JsonStorage.SaveToFile(ProductsFilePath, Products);
        }

        public bool DeleteProductByName(string name, List<Supplier> suppliers) {
            Product productToDelete = this.GetProductByName(name);
            if (productToDelete != null) {
                Products.Remove(productToDelete);
                JsonStorage.SaveToFile(ProductsFilePath, Products);

                foreach (var supplier in suppliers) {
                    supplier.Catalog.RemoveAll(product => product.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                }
                JsonStorage.SaveToFile(SupplierData.SuppliersFilePath, suppliers);

                return true;
            }
            return false;
        }

        public bool DeleteProductByArticle(string article, List<Supplier> suppliers) {
            Product productToDelete = this.GetProductByArticle(article);
            if (productToDelete != null) {
                Products.Remove(productToDelete);
                JsonStorage.SaveToFile(ProductsFilePath, Products);

                foreach (var supplier in suppliers) {
                    supplier.Catalog.RemoveAll(product => product.Article.Equals(article, StringComparison.OrdinalIgnoreCase));
                }
                JsonStorage.SaveToFile(SupplierData.SuppliersFilePath, suppliers);
                return true;
            }
            return false;
        }

        public Product GetProductByArticle(string article) {
            Product foundProduct = Products.Find((product) => product.Article == article);
            return foundProduct;
        }

        public Product GetProductByName(string name) {
            Product foundProduct = Products.Find((product) => product.Name == name);
            return foundProduct;
        }

        public void ChangeProductPriceByArticle(string article, double newPrice) {
            Products.Find((product) => product.Article == article).Price = newPrice;
        }

        public string GenerateNextProductArticle(Type productType) {
            string prefix = "PRD";

            if (productType == typeof(ElectronicProduct)) prefix = "EL";
            else if (productType == typeof(Cloth)) prefix = "CL";
            else if (productType == typeof(Sofa)) prefix = "SF";
            else if (productType == typeof(Furniture)) prefix = "FN";

            int maxNumber = 0;

            if (Products != null) {
                foreach (var product in Products) {
                    if (product.Article != null && product.Article.StartsWith(prefix + "-")) {
                        string numberPart = product.Article.Substring(prefix.Length + 1);
                        if (int.TryParse(numberPart, out int num) && num > maxNumber) {
                            maxNumber = num;
                        }
                    }
                }
            }

            return $"{prefix}-{(maxNumber + 1):D4}";
        }

    }
}
