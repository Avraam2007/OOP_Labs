using System;
using System.IO;
using Newtonsoft.Json;

namespace OOP_Main {
    public static class JsonStorage {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Auto,
            NullValueHandling = NullValueHandling.Ignore
        };

        public static T LoadFromFile<T>(string filePath) where T : new() {
            try {
                if (!File.Exists(filePath)) {
                    return new T();
                }

                string json = File.ReadAllText(filePath);
                if (string.IsNullOrWhiteSpace(json)) {
                    return new T();

                }

                T data = JsonConvert.DeserializeObject<T>(json, Settings);

                if (data == null) {
                    return new T();
                }

                return data;
            }
            catch (Exception ex) {
                Console.WriteLine($"Error loading file {filePath}: {ex.Message}");
                return new T();
            }
        }

        public static void SaveToFile<T>(string filePath, T data) {
            try {
                string directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) {
                    Directory.CreateDirectory(directory);
                }

                string json = JsonConvert.SerializeObject(data, Settings);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex) {
                Console.WriteLine($"Error saving file {filePath}: {ex.Message}");
            }
        }
    }
}