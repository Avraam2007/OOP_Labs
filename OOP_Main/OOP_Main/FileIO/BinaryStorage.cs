using OOP_Main.Entities;
using OOP_Main.FileIO.BinaryEntities;
using OOP_Main.Interfaces;
using System.Collections.Generic;

namespace OOP_Main.FileIO {
    internal class BinaryStorage {
        private readonly BinaryProductStorage _productStorage;

        public BinaryStorage() {
            _productStorage = new BinaryProductStorage();
        }

        public void SaveProducts(string filePath, List<Product> products) => _productStorage.SaveToFile(filePath, products);

        public void LoadProducts(string filePath) => _productStorage.LoadFromFile(filePath);

    }
}
