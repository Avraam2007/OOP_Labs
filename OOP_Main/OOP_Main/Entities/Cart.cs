using System.Collections.Concurrent;

namespace OOP_Main {
    public class Cart {
        public ConcurrentDictionary<string, int> Products { get; private set; } = new ConcurrentDictionary<string, int>();
        public Cart() {

        }

        public void AddProduct(string productName) {
            var normalizedProductName = ProductHelpers.NormalizeProductName(productName);
            if (!Products.TryAdd(normalizedProductName, 1)) {
                Products[normalizedProductName]++;
            }
        }
    }
}
