using OOP_Main.Comparers;
using System.Collections.Generic;
using System.Linq;

namespace OOP_Main {
    public static class ProductHelpers {
        public static string NormalizeProductName(string productName) {
            return productName.Trim().ToLowerInvariant();
        }

        public static List<Product> FindProductsInPriceRange(IEnumerable<Product> products, double minPrice, double maxPrice) {
            var sortedProducts = new SortedSet<Product>(products, new ProductPriceComparer());

            return sortedProducts.Where(p => p.Price >= minPrice && p.Price <= maxPrice).ToList();
        }
    }
}
