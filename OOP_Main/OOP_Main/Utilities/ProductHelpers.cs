using OOP_Main.Comparers;
using OOP_Main.Entities;
using OOP_Main.Structures;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OOP_Main.Utilities {
    public static class ProductHelpers {
        public static string NormalizeProductName(string productName) {
            return productName.Trim().ToLowerInvariant();
        }

        public static List<Product> FindProductsInPriceRange(this IEnumerable<Product> products, double minPrice, double maxPrice) {
            var sortedProducts = new SortedSet<Product>(products, new ProductPriceComparer());

            return sortedProducts.Where(p => p.Price >= minPrice && p.Price <= maxPrice).ToList();
        }

        public static List<string> GetCategoriesList(this IEnumerable<Product> products) {
            CategoryBST categoryTree = new CategoryBST();
            foreach (var product in products) {
                if (!string.IsNullOrEmpty(product.Category)) {
                    categoryTree.Insert(product.Category);
                }
            }

            List<string> categoriesList = new List<string>();
            categoryTree.InOrderTraversal(categoryName => categoriesList.Add(categoryName));

            if (categoriesList.Count == 0) {
                throw new NullReferenceException("Categories not found.");
            }

            return categoriesList;
        }

        public static List<Product> FilterByCategory(this IEnumerable<Product> products, string selectedCategory) {
            List<Product> filtered = products.ToList()
                .FindAll(p => string.Equals(p.Category, selectedCategory, StringComparison.OrdinalIgnoreCase));

            return filtered;
        }
    }
}
