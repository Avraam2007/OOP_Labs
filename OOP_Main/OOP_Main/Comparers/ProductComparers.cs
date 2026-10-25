using OOP_Main.Entities;
using System;
using System.Collections.Generic;

namespace OOP_Main.Comparers {
    public class ProductPriceComparer : IComparer<Product> {
        public int Compare(Product x, Product y) {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            return x.Price.CompareTo(y.Price);
        }
    }

    public class ProductNameComparer : IComparer<Product> {
        public int Compare(Product x, Product y) {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            return string.Compare(x.Name, y.Name, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
