using OOP_Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main.Builders {
    public class Director {
        private IBuilder _builder;

        public IBuilder Builder {
            set { _builder = value; }
        }

        public void BuildEmptyOrder(int orderId, int buyerId) {
            this._builder.SetOrderId(orderId);
            this._builder.SetCustomer(buyerId);
        }

        public void BuildOrderWithProducts(int orderId, int buyerId, List<Product> products) {
            this._builder.SetOrderId(orderId);
            this._builder.SetCustomer(buyerId);
            this._builder.AddCatalog(products);
        }

        public void BuildFullOrder(int orderId, int buyerId, List<Product> products, OrderStatus orderStatus) {
            this._builder.SetOrderId(orderId);
            this._builder.SetCustomer(buyerId);
            this._builder.AddCatalog(products);
            this._builder.SetStatus(orderStatus);
        }
    }
}
