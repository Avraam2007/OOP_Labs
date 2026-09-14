using System.Collections.Generic;

namespace OOP_Main {
    public class Data {
        private List<User> users;
        private List<Order> orders;
        private List<Product> products;
        private User currentUser;

        public List<User> Users { get { return users; } private set { users = value; } }
        public List<Order> Orders { get { return orders; } private set { orders = value; } }
        public List<Product> Products { get { return products; } private set { products = value; } }
        public User CurrentUser { get { return currentUser; } set { currentUser = value; } }

        public Data() {
            Users = new List<User>();
            Orders = new List<Order>();
            Products = new List<Product>();
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

        public Order GetOrderById(int id) {
            Order foundOrder = Orders.Find((order) => order.orderId == id);
            return foundOrder;
        }

        public User GetUserByUsername(string username) {
            User foundUser = Users.Find((user) => user.Username == username);
            return foundUser;
        }
    }
}
