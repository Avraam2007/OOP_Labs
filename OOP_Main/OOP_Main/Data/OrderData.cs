using NOptional;
using OOP_Main.Builders;
using System.Collections.Generic;
using System.Linq;

namespace OOP_Main {
    public class OrderData: BaseDataRepository<Order, int> {
        public List<Order> Orders => Items;

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
            //Order newOrder = new Order(newOrderId, buyerId, catalog);

            AddOrder(newOrder);
        }

        public bool DeleteOrder(int id) => Remove(id);

        public List<Order> GetOrdersFromUser(int userId) {
            List<Order> ordersFromUser = new List<Order>();
            foreach (var order in Orders) {
                if (order.BuyerId == userId) ordersFromUser.Add(order);
            }
            return ordersFromUser;
        }

        public override IOptional<Order> Get(int id) {
            Order foundOrder = Orders.Find((order) => order.OrderId == id);
            return Optional.OfNullable(foundOrder);
        }

        public IOptional<Order> GetOrderById(int id) => this.Get(id);


    }
}
