using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main.Utilities {
    public static class ParallelSearchService {

        public static List<Product> FastSearch(IEnumerable<Product> products, string query, double maxPrice, double minPrice = 0) {
            var results = new ConcurrentBag<Product>();

            Parallel.ForEach(products, product => {
                bool matchesName = product.Name != null && product.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
                bool matchesCategory = product.Category != null && product.Category.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
                bool matchesPrice = product.Price <= maxPrice && product.Price >= minPrice;

                if ((matchesName || matchesCategory) && matchesPrice) {
                    results.Add(product);
                }
            });

            return new List<Product>(results);
        }
    }
}
