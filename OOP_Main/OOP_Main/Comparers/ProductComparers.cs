using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main.Comparers {
    public class ProductPriceComparer : IComparer<Product> {
        public int Compare(Product x, Product y) {
            if (x == null || y == null) return 0;
            return x.Price.CompareTo(y.Price);
        }
    }

    public class ProductNameComparer : IComparer<Product> {
        public int Compare(Product x, Product y) {
            if (x == null || y == null) return 0;
            return string.Compare(x.Name, y.Name, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
