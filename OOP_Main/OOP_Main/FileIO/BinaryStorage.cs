using OOP_Main.Entities;
using OOP_Main.FileIO.BinaryEntities;
using System.Collections.Generic;

namespace OOP_Main.FileIO {
    internal class BinaryStorage {
        private readonly BinaryProductStorage _productStorage;
        private readonly BinaryUserStorage _userStorage;

        public BinaryStorage() {
            _productStorage = new BinaryProductStorage();
            _userStorage = new BinaryUserStorage();
        }

        public void SaveProducts(string filePath, List<Product> products) => _productStorage.SaveToFile(filePath, products);

        public void LoadProducts(string filePath) => _productStorage.LoadFromFile(filePath);

        public void SaveUsers(string filePath, List<User> users) => _userStorage.SaveToFile(filePath, users);

        public void LoadUsers(string filePath) => _userStorage.LoadFromFile(filePath);

    }
}
