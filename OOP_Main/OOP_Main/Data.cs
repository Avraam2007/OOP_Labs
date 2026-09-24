using System;
using System.Collections.Generic;

namespace OOP_Main {
    public class Data {
        private readonly UserData _userData;
        private readonly ProductData _productData;
        private readonly OrderData _orderData;
        private readonly SupplierData _supplierData;

        public List<User> Users => _userData.Users;
        public List<Product> Products => _productData.Products;
        public List<Order> Orders => _orderData.Orders;
        public List<Supplier> Suppliers => _supplierData.Suppliers;

        public User CurrentUser { get; set; }

        public Data() {
            _userData = new UserData();
            _productData = new ProductData();
            _orderData = new OrderData();
            _supplierData = new SupplierData();
        }

        public void LoadAllData() {
            _userData.Load();
            _productData.Load();
            _orderData.Load(); 
            _supplierData.Load();
        }

        public void SaveAllData() {
            _userData.Save();
            _productData.Save();
            _orderData.Save();
            _supplierData.Save();
        }

        public void AddUser(User user) => _userData.AddUser(user);

        public void AddUser(string name, string password, bool isAdmin = false) => _userData.AddUser(name, password, isAdmin);

        public void AddOrder(Order order) => _orderData.AddOrder(order);

        public void AddOrder(int buyerId, List<Product> catalog) => _orderData.AddOrder(buyerId, catalog);

        public void AddProduct(Product product) => _productData.AddProduct(product);

        public void AddSupplier(Supplier supplier) => _supplierData.AddSupplier(supplier);

        public void AddSupplier(string name, string contactEmail, double rating) => _supplierData.AddSupplier(name, contactEmail, rating);

        public bool DeleteUserById(int id) => _userData.DeleteUserById(id);

        public bool DeleteOrder(int id) => _orderData.DeleteOrder(id);

        public bool DeleteUserByUsername(string username) => _userData.DeleteUserByUsername(username);

        public bool DeleteSupplierByName(string name) => _supplierData.DeleteSupplierByName(name);

        public bool DeleteProductByName(string name) => _productData.DeleteProductByName(name, Suppliers);
        public bool DeleteProductByArticle(string article) => _productData.DeleteProductByArticle(article, Suppliers);

        public User GetUserById(int id) => _userData.GetUserById(id);

        public List<Order> GetOrdersFromUser(int userId) => _orderData.GetOrdersFromUser(userId);

        public Order GetOrderById(int id) => _orderData.GetOrderById(id);

        public Product GetProductByArticle(string article) => _productData.GetProductByArticle(article);

        public Product GetProductByName(string name) => _productData.GetProductByName(name);

        public User GetUserByUsername(string username) => _userData.GetUserByUsername(username);

        public Supplier GetSupplierByName(string name) => _supplierData.GetSupplierByName(name);

        public void ChangeProductPriceByArticle(string article, double newPrice) => _productData.ChangeProductPriceByArticle(article, newPrice);

        public string GenerateNextProductArticle(Type productType) => _productData.GenerateNextProductArticle(productType);
    }

}
