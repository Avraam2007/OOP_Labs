using System;
using System.Collections.Generic;
using System.Linq;

namespace OOP_Main {
    public class Data {
        private const string UsersFilePath = "DataStorage/users.json";
        private const string ProductsFilePath = "DataStorage/products.json";
        private const string OrdersFilePath = "DataStorage/orders.json";
        private const string SuppliersFilePath = "DataStorage/suppliers.json";
        public List<User> Users { get; private set; } = new List<User>();
        public List<Order> Orders { get; private set; } = new List<Order>();
        public List<Product> Products { get; private set; } = new List<Product>();

        public List<Supplier> Suppliers { get; private set; } = new List<Supplier>();
        public User CurrentUser { get; set; }

        public Data() {

        }

        public void LoadAllData() {
            Users = JsonStorage.LoadFromFile<List<User>>(UsersFilePath);
            Products = JsonStorage.LoadFromFile<List<Product>>(ProductsFilePath);
            Orders = JsonStorage.LoadFromFile<List<Order>>(OrdersFilePath);
            Suppliers = JsonStorage.LoadFromFile<List<Supplier>>(SuppliersFilePath);
        }

        public void SaveAllData() {
            JsonStorage.SaveToFile(UsersFilePath, Users);
            JsonStorage.SaveToFile(ProductsFilePath, Products);
            JsonStorage.SaveToFile(OrdersFilePath, Orders);
            JsonStorage.SaveToFile(SuppliersFilePath, Suppliers);
        }

        public void AddUser(User user) {
            Users.Add(user);
            JsonStorage.SaveToFile(UsersFilePath, Users);
        }

        public void AddUser(string name, string password, bool isAdmin = false) {
            int newUserId = Users.Count > 0 ? Users.Max(u => u.Id) : 0;
            newUserId++;
            User newUser = new User(newUserId, name, password, isAdmin);

            AddUser(newUser);
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

        public void AddProduct(Product product) {
            Products.Add(product);
            JsonStorage.SaveToFile(ProductsFilePath, Products);
        }

        public void AddSupplier(Supplier supplier) {
            Suppliers.Add(supplier);
            JsonStorage.SaveToFile(SuppliersFilePath, Suppliers);
        }

        public void AddSupplier(string name, string contactEmail, double rating) {
            int newSupplierId = Suppliers.Count > 0 ? Suppliers.Max(s => s.SupplierId) : 0;
            newSupplierId++;
            Supplier newSupplier = new Supplier(newSupplierId, name, contactEmail, rating);

            AddSupplier(newSupplier);
        }

        public bool DeleteUserById(int id) {
            User userToDelete = this.GetUserById(id);
            if (userToDelete != null) {
                Users.Remove(userToDelete);
                JsonStorage.SaveToFile(UsersFilePath, Users);
                return true;
            }
            return false;
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

        public bool DeleteUserByUsername(string username) {
            User userToDelete = this.GetUserByUsername(username);
            if (userToDelete != null) {
                Users.Remove(userToDelete);
                JsonStorage.SaveToFile(UsersFilePath, Users);
                return true;
            }
            return false;
        }

        public bool DeleteSupplierByName(string name) {
            Supplier supplierToDelete = this.GetSupplierByName(name);
            if (supplierToDelete != null) {
                Suppliers.Remove(supplierToDelete);
                JsonStorage.SaveToFile(SuppliersFilePath, Suppliers);
                return true;
            }
            return false;
        }

        public bool DeleteProductByName(string name) {
            Product productToDelete = this.GetProductByName(name);
            if (productToDelete != null) {
                Products.Remove(productToDelete);
                JsonStorage.SaveToFile(ProductsFilePath, Products);

                foreach (var supplier in Suppliers) {
                    supplier.Catalog.RemoveAll(product => product.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                }
                JsonStorage.SaveToFile(SuppliersFilePath, Suppliers);

                return true;
            }
            return false;
        }

        public bool DeleteProductByArticle(string article) {
            Product productToDelete = this.GetProductByArticle(article);
            if (productToDelete != null) {
                Products.Remove(productToDelete);
                JsonStorage.SaveToFile(ProductsFilePath, Products);

                foreach (var supplier in Suppliers) {
                    supplier.Catalog.RemoveAll(product => product.Article.Equals(article, StringComparison.OrdinalIgnoreCase));
                }
                JsonStorage.SaveToFile(SuppliersFilePath, Suppliers);
                return true;
            }
            return false;
        }

        public User GetUserById(int id) {
            User foundUser = Users.Find((user) => user.Id == id);
            return foundUser;
        }

        public List<Order> GetOrdersFromUser(int userId) {
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

        public string GenerateNextProductArticle(Type productType) {
            string prefix = "PRD";

            if (productType == typeof(ElectronicProduct)) prefix = "EL";
            else if (productType == typeof(Cloth)) prefix = "CL";
            else if (productType == typeof(Sofa)) prefix = "SF";
            else if (productType == typeof(Furniture)) prefix = "FN";

            int maxNumber = 0;

            if (Products != null) {
                foreach (var product in Products) {
                    if (product.Article != null && product.Article.StartsWith(prefix + "-")) {
                        string numberPart = product.Article.Substring(prefix.Length + 1);
                        if (int.TryParse(numberPart, out int num) && num > maxNumber) {
                            maxNumber = num;
                        }
                    }
                }
            }

            return $"{prefix}-{(maxNumber + 1):D4}";
        }
    }

}
