using OOP_Main.Entities;
using OOP_Main.States;
using System;
using System.Collections.Generic;

namespace OOP_Main.Builders {
    public class Director {
        private IBuilder _builder;

        public IBuilder Builder {
            set { _builder = value; }
        }

        public void BuildValidationOrder(List<Product> products) {
            this._builder.AddCatalog(products);
            this._builder.AddDate(DateTime.Now);
        }

        public void BuildEmptyOrder(int orderId, int buyerId) {
            this._builder.SetOrderId(orderId);
            this._builder.SetCustomer(buyerId);
            this._builder.AddDate(DateTime.Now);
        }

        public void BuildOrderWithProducts(int orderId, int buyerId, List<Product> products) {
            this._builder.SetOrderId(orderId);
            this._builder.SetCustomer(buyerId);
            this._builder.AddDate(DateTime.Now);
            this._builder.AddCatalog(products);
        }

        public void BuildFullOrder(int orderId, int buyerId, List<Product> products, OrderStatus orderStatus) {
            this._builder.SetOrderId(orderId);
            this._builder.SetCustomer(buyerId);
            this._builder.AddCatalog(products);
            this._builder.AddDate(DateTime.Now);
            this._builder.SetStatus(orderStatus);
        }
    }
}
