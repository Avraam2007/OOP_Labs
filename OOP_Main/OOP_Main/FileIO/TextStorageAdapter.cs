using OOP_Main.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main.FileIO {
    class TextStorageAdapter : IStorage<string> {
        private readonly TextStorage _textStorage;

        public TextStorageAdapter(TextStorage adaptee) {
            this._textStorage = adaptee;
        }

        public string LoadFromFile(string filePath) => this._textStorage.LoadFromFile(filePath);

        public void SaveToFile(string filePath, string data) => this._textStorage.SaveToFile(data, filePath);
    }
}
