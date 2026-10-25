using NOptional;
using OOP_Main.Builders;
using OOP_Main.Entities;
using OOP_Main.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace OOP_Main.Repositories {
    public class OrderData: BaseDataRepository<Order, int> {
        public List<Order> Orders => Items;

        public OrderProcessingQueue OrderQueue { get; set; }

        public OrderData() : base("DataStorage/orders.json") { }

        public void AddOrder(Order order) => Add(order);

        public void AddOrder(int buyerId, List<Product> catalog) {
            int newOrderId = Orders.Count > 0 ? Orders.Max(o => o.OrderId) : 0;
            newOrderId++;
            var director = new Director();
            var builder = new OrderBuilder();
            director.Builder = builder;
            director.BuildOrderWithProducts(newOrderId, buyerId, catalog);
            Order newOrder = builder.GetOrder();

            AddOrder(newOrder);
        }

        public bool DeleteOrder(int id) {
            bool isDeleted = Remove(id);
            if (isDeleted) {
                this.Save();
                return true;
            }
            else { return false; }
        }

        public List<Order> GetOrdersFromUser(int userId) {
            List<Order> ordersFromUser = new List<Order>();
            foreach (var order in Orders) {
                if (order.BuyerId == userId) ordersFromUser.Add(order);
            }
            return ordersFromUser;
        }

        protected override IOptional<Order> Get(int id) {
            Order foundOrder = Orders.Find((order) => order.OrderId == id);
            return Optional.OfNullable(foundOrder);
        }

        public IOptional<Order> GetOrderById(int id) => this.Get(id);


    }
}
