using System;
using System.Collections.Generic;
using System.Linq;

namespace OOP_Main {
    public class Data {
        public List<User> Users { get; private set; } = new List<User>();
        public List<Order> Orders { get; private set; } = new List<Order>();
        public List<Product> Products { get; private set; } = new List<Product>();

        public List<Supplier> Suppliers { get; private set; } = new List<Supplier>();
        public User CurrentUser { get; set; }

        public Data() {

        }

        public void AddUser(User user) {
            Users.Add(user);
        }

        public void AddOrder(Order order) {
            Orders.Add(order);
        }

        public void AddProduct(Product product) {
            Products.Add(product);
        }

        public void AddSupplier(Supplier supplier) {
            Suppliers.Add(supplier);
        }

        public void DeleteUserById(string id) {
            User userToDelete = this.GetUserById(id);
            if (userToDelete != null) {
                Users.Remove(userToDelete);
            }
        }

        public void DeleteOrder(int id) {
            Order orderToDelete = this.GetOrderById(id);
            if (orderToDelete != null) {
                Orders.Remove(orderToDelete);
            }
        }

        public void DeleteUserByUsername(string username) {
            User userToDelete = this.GetUserByUsername(username);
            if (userToDelete != null) {
                Users.Remove(userToDelete);
            }
        }

        public User GetUserById(string id) {
            User foundUser = Users.Find((user) => user.Id == id);
            return foundUser;
        }

        public int GetLatestOrderId() {
            if (!Orders.Any()) return 1;

            int maxId = Int32.MinValue;
            foreach (var order in Orders) {
                if (order.OrderId > maxId) maxId = order.OrderId;
            }
            return maxId;
        }

        public List<Order> GetOrdersFromUser(string userId) {
            List<Order> ordersFromUser = new List<Order>();
            foreach (var order in Orders) {
                if(order.BuyerId == userId) ordersFromUser.Add(order);
            }
            return ordersFromUser;
        }

        public Order GetOrderById(int id) {
            Order foundOrder = Orders.Find((order) => order.OrderId == id);
            return foundOrder;
        }

        public Product GetProductByArticle(string article) {
            Product foundProduct = Products.Find((product) => product.Article == article);
            return foundProduct;
        }

        public Product GetProductByName(string name) {
            Product foundProduct = Products.Find((product) => product.Name == name);
            return foundProduct;
        }

        public User GetUserByUsername(string username) {
            User foundUser = Users.Find((user) => user.Username == username);
            return foundUser;
        }

        public Supplier GetSupplierByName(string name) {
            Supplier foundSupplier = Suppliers.Find((supplier) => supplier.Name == name);
            return foundSupplier;
        }
    }

}
