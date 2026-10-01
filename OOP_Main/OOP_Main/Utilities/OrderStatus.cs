using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main {
    public enum OrderStatus {
        Pending,
        Paid,
        Shipped,
        Delivered,
        Cancelled
    }

    public static class OrderStatusExtensions {
        public static string GetDescription(OrderStatus status) {
            switch(status) {
                case OrderStatus.Pending:
                    return "Pending";
                case OrderStatus.Paid:
                    return "Paid, ready to ship";
                case OrderStatus.Shipped:
                    return "Shipping";
                case OrderStatus.Delivered:
                    return "Delievered successfully";
                case OrderStatus.Cancelled:
                    return "Cancelled";
                default:
                    return "Unknown";
            }
        }

        public static bool CanBeCancelled(OrderStatus status) {
            return status == OrderStatus.Pending || status == OrderStatus.Paid;
        }
    }
}
