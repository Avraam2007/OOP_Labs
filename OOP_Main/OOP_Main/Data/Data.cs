using NOptional;
using OOP_Main.Entities;
using OOP_Main.FileIO;
using OOP_Main.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OOP_Main.Repositories {
    public class Data {
        private readonly UserData _userData;
        private readonly ProductData _productData;
        private readonly OrderData _orderData;
        private readonly SupplierData _supplierData;

        public List<User> Users => _userData.Users;
        public List<Product> Products => _productData.Products;
        public List<Order> Orders => _orderData.Orders;
        public OrderProcessingQueue OrderQueue => _orderData.OrderQueue;
        public List<Supplier> Suppliers => _supplierData.Suppliers;

        public IOptional<User> CurrentUser { get; set; }

        public Data() {
            _userData = new UserData();
            _productData = new ProductData();
            _orderData = new OrderData();
            _supplierData = new SupplierData();
        }

        public async Task LoadAllData() {
            await Task.WhenAll(
                _userData.LoadAsync(),
                _productData.LoadAsync(),
                _orderData.LoadAsync(),
                _supplierData.LoadAsync()
            );
        }

        public async Task SaveAllData() {
            await Task.WhenAll(
                _userData.SaveAsync(),
                _productData.SaveAsync(),
                _orderData.SaveAsync(),
                _supplierData.SaveAsync()
            );
        }

        public void SaveBackup() => BackupService.CreateZipBackup("DataStorage", "Backups");

        public void AddUser(User user) => _userData.AddUser(user);

        public void AddUser(string name, string password, bool isAdmin = false) => _userData.AddUser(name, password, isAdmin);

        public void AddOrder(Order order) => _orderData.AddOrder(order);

        public void AddOrder(int buyerId, List<Product> catalog) => _orderData.AddOrder(buyerId, catalog);

        public void AddProduct(Product product) => _productData.AddProduct(product);

        public void AddSupplier(Supplier supplier) => _supplierData.AddSupplier(supplier);

        public void AddSupplier(string name, string contactEmail, string phone, double rating) => _supplierData.AddSupplier(name, contactEmail, phone, rating);

        public bool DeleteUserById(int id) => _userData.DeleteUserById(id);

        public bool DeleteOrder(int id) => _orderData.DeleteOrder(id);

        public bool DeleteUserByUsername(string username) => _userData.DeleteUserByUsername(username);

        public bool DeleteSupplierByName(string name) => _supplierData.DeleteSupplierByName(name);

        public bool DeleteSupplierById(int id) => _supplierData.DeleteSupplierById(id);

        public bool DeleteProductByName(string name) {
            bool isDeleted = _productData.DeleteProductByName(name);

            if (!isDeleted) return false;

            foreach (var supplier in _supplierData.Suppliers) {
                supplier.Catalog.RemoveAll(product => product.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }

            _productData.Save();
            _supplierData.Save();

            return true;
        }
        public bool DeleteProductByArticle(string article) {
            bool isDeleted = _productData.DeleteProductByArticle(article);

            if (!isDeleted) return false;

            foreach (var supplier in _supplierData.Suppliers) {
                supplier.Catalog.RemoveAll(product => product.Article.Equals(article, StringComparison.OrdinalIgnoreCase));
            }

            _productData.Save();
            _supplierData.Save();

            return true;
        }

        public IOptional<User> GetUserById(int id) => _userData.GetUserById(id);

        public List<Order> GetOrdersFromUser(int userId) => _orderData.GetOrdersFromUser(userId);

        public IOptional<Order> GetOrderById(int id) => _orderData.GetOrderById(id);

        public IOptional<Product> GetProductByArticle(string article) => _productData.GetProductByArticle(article);

        public IOptional<Product> GetProductByName(string name) => _productData.GetProductByName(name);

        public IOptional<User> GetUserByUsername(string username) => _userData.GetUserByUsername(username);

        public IOptional<Supplier> GetSupplierByName(string name) => _supplierData.GetSupplierByName(name);

        public IOptional<Supplier> GetSupplierById(int id) => _supplierData.GetSupplierById(id);

        public void ChangeProductPriceByArticle(string article, double newPrice) => _productData.ChangeProductPriceByArticle(article, newPrice);

        public string GenerateNextProductArticle(Type productType) => _productData.GenerateNextProductArticle(productType);

        public bool IsArticleExists(string article) => _productData.IsArticleExists(article);
    }

}
