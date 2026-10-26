using OOP_Main.Entities;
using OOP_Main.FileIO.TextEntities;
using OOP_Main.Interfaces;
using System.Collections.Generic;

namespace OOP_Main.FileIO {
    public class TextEntityStorage {
        private readonly TextProductStorage _productStorage;
        private readonly TextUserStorage _userStorage;

        public TextEntityStorage() {
            _productStorage = new TextProductStorage();
            _userStorage = new TextUserStorage();
        }

        public void SaveUsersToText(string filePath, List<User> users) => _userStorage.SaveToFile(filePath, users);

        public List<User> LoadUsersFromText(string filePath) => _userStorage.LoadFromFile(filePath);

        public void SaveProductsToText(string filePath, List<Product> products) => _productStorage.SaveToFile(filePath, products);

        public List<Product> LoadProductsFromText(string filePath) => _productStorage.LoadFromFile(filePath);
    }
}
