using OOP_Main.Entities;
using OOP_Main.Factories;
using OOP_Main.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace OOP_Main.FileIO.BinaryEntities {
    internal class BinaryUserStorage: IEntityStorage<User> {
        public void SaveToFile(string filePath, List<User> users) {
            string directory = Path.GetDirectoryName(filePath);
            FileService.EnsureDirectoryExists(directory);

            var formatter = new BinaryFormatter();

            using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write)) {
                formatter.Serialize(fs, users);
            }
        }

        public List<User> LoadFromFile(string filePath) {
            var result = new List<User>();
            if (!File.Exists(filePath)) return result;

            var formatter = new BinaryFormatter();

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read)) {
                result = (List<User>)formatter.Deserialize(fs);
            }
            return result;
        }
    }
}
