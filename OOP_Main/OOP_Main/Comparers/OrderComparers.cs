using System.Collections.Generic;

namespace OOP_Main.Comparers {
    public class OrderPriceComparer : IComparer<Order> {
        public int Compare(Order x, Order y) {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            return x.GetTotalPrice().CompareTo(y.GetTotalPrice());
        }
    }

    public class OrderIDComparer : IComparer<Order> {
        public int Compare(Order x, Order y) {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            return x.OrderId.CompareTo(y.OrderId);
        }
    }
}
