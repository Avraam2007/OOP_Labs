using OOP_Main.Entities;
using System;
using System.Collections.Generic;
namespace OOP_Main.Comparers {
    public class SupplierRatingComparer : IComparer<Supplier> {
        public int Compare(Supplier x, Supplier y) {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            return x.Rating.CompareTo(y.Rating);
        }
    }

    public class SupplierNameComparer : IComparer<Supplier> {
        public int Compare(Supplier x, Supplier y) {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            return string.Compare(x.Name, y.Name, System.StringComparison.OrdinalIgnoreCase);
        }
    }
    public class SupplierIDComparer : IComparer<Supplier> {
        public int Compare(Supplier x, Supplier y) {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            return x.SupplierId.CompareTo(y.SupplierId);
        }
    }
}
