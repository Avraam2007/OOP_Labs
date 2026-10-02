using OOP_Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main.Builders {
    internal class OrderBuilder: IBuilder {
        private Order _order;

        public OrderBuilder() {
            this.Reset();
        }

        public void Reset() {
            this._order = new Order();
        }

        public void SetOrderId(int orderId) {
            _order.OrderId = orderId;
        }

        public void SetCustomer(int buyerId) {
            _order.BuyerId = buyerId;
        }

        public void AddCatalog(List<Product> products) {
            _order.Products = products;
        }

        public void SetStatus(OrderStatus orderStatus) {
            _order.Status = orderStatus;
        }

        public Order GetOrder() {
            Order result = this._order;

            this.Reset();

            return result;
        }
    }
}
