namespace OOP_Main {
    public static class ProductHelpers {
        public static string NormalizeProductName(string productName) {
            return productName.Trim().ToLowerInvariant();
        }
    }
}
