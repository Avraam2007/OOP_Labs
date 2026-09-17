using System;
using System.Collections.Generic;

namespace OOP_Main {
    public class Data {
        private List<User> users;
        private List<Order> orders;
        private List<Product> products;
        private List<Supplier> suppliers;
        private User currentUser;
        public List<User> Users { get { return users; } private set { users = value; } }
        public List<Order> Orders { get { return orders; } private set { orders = value; } }
        public List<Product> Products { get { return products; } private set { products = value; } }

        public List<Supplier> Suppliers { get { return suppliers; } private set { suppliers = value; } }
        public User CurrentUser { get { return currentUser; } set { currentUser = value; } }

        public Data() {
            Users = new List<User>();
            Orders = new List<Order>();
            Products = new List<Product>();
            Suppliers = new List<Supplier>();
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

        public void DeleteOrder(string id) {
            User orderToDelete = this.GetUserById(id);
            if (orderToDelete != null) {
                Users.Remove(orderToDelete);
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
            int minId = Int32.MaxValue;
            foreach (var order in Orders) {
                if (order.orderId < minId) minId = order.orderId;
            }
            return minId;
        }

        public Order GetOrderById(int id) {
            Order foundOrder = Orders.Find((order) => order.orderId == id);
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
