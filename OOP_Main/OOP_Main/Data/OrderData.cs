using System.Collections.Generic;
using System.Linq;

namespace OOP_Main {
    public class OrderData: IBridgeJSON {
        private const string OrdersFilePath = "DataStorage/orders.json";
        public List<Order> Orders { get; private set; } = new List<Order>();

        public void Load() {
            Orders = JsonStorage.LoadFromFile<List<Order>>(OrdersFilePath);
        }

        public void Save() {
            JsonStorage.SaveToFile(OrdersFilePath, Orders);
        }

        public void AddOrder(Order order) {
            Orders.Add(order);
            JsonStorage.SaveToFile(OrdersFilePath, Orders);
        }

        public void AddOrder(int buyerId, List<Product> catalog) {
            int newOrderId = Orders.Count > 0 ? Orders.Max(o => o.OrderId) : 0;
            newOrderId++;
            Order newOrder = new Order(newOrderId, buyerId, catalog);

            AddOrder(newOrder);
        }

        public bool DeleteOrder(int id) {
            Order orderToDelete = this.GetOrderById(id);
            if (orderToDelete != null) {
                Orders.Remove(orderToDelete);
                JsonStorage.SaveToFile(OrdersFilePath, Orders);
                return true;
            }
            return false;
        }

        public List<Order> GetOrdersFromUser(int userId) {
            List<Order> ordersFromUser = new List<Order>();
            foreach (var order in Orders) {
                if (order.BuyerId == userId) ordersFromUser.Add(order);
            }
            return ordersFromUser;
        }

        public Order GetOrderById(int id) {
            Order foundOrder = Orders.Find((order) => order.OrderId == id);
            return foundOrder;
        }


    }
}
