using OOP_Main.Entities;
using OOP_Main.Interfaces;
using OOP_Main.States;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Main.FileIO.TextEntities {
    internal class TextUserStorage : IEntityStorage<User> {
        public void SaveToFile(string filePath, List<User> users) {
            string directory = Path.GetDirectoryName(filePath);
            FileService.EnsureDirectoryExists(directory);

            // try-with-resources аналог у C#: using
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(fs, Encoding.UTF8)) {
                foreach (var u in users) {
                    writer.WriteLine($"{u.Id};{u.Username};{u.Password};{u.Role};{u.Account.Sum}");
                }
            }
        }

        public List<User> LoadFromFile(string filePath) {
            var result = new List<User>();
            if (!File.Exists(filePath)) return result;

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(fs, Encoding.UTF8)) {
                string line;
                while ((line = reader.ReadLine()) != null) {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var parts = line.Split(';');
                    if (parts.Length >= 5 &&
                        int.TryParse(parts[0], out int id) &&
                        double.TryParse(parts[4], NumberStyles.Any, CultureInfo.InvariantCulture, out double sum) &&
                        Enum.TryParse(parts[3], out UserRole role)) {
                        result.Add(new User(id, parts[1], parts[2], role, sum));
                    }
                }
            }
            return result;
        }
    }
}
