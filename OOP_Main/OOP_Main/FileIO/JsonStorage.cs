using Newtonsoft.Json;
using OOP_Main.FileIO;
using System;
using System.IO;
using System.Threading;

namespace OOP_Main {
    public class JsonStorage {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.Auto,
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
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
            catch (JsonReaderException ex) {
                ErrorRegistry.HandleError($"JSON syntax error: {ex.Message}");
                return new T();
            }
            catch (JsonSerializationException ex) {
                ErrorRegistry.HandleError($"JSON structure/polymorphism error: {ex.Message}");
                return new T();
            }
            catch (UnauthorizedAccessException ex) {
                ErrorRegistry.HandleError($"Access Denied: Cannot read '{filePath}'. {ex.Message}");
                return new T();
            }
            catch (IOException ex) {
                ErrorRegistry.HandleError($"I/O Error: File '{filePath}' might be locked by another process. {ex.Message}");
                return new T();
            }
            catch (Exception ex) {
                ErrorRegistry.HandleError($"Unexpected Error loading '{filePath}': {ex.Message}");
                Thread.Sleep(5000);
                return new T();
            }
        }

        public static void SaveToFile<T>(string filePath, T data) {
            try {
                string directory = Path.GetDirectoryName(filePath);
                FileService.EnsureDirectoryExists(directory);

                string json = JsonConvert.SerializeObject(data, Settings);
                File.WriteAllText(filePath, json);
            }
            catch (UnauthorizedAccessException ex) {
                ErrorRegistry.HandleError($"Access Denied: Cannot write '{filePath}'. {ex.Message}");
            }
            catch (IOException ex) {
                ErrorRegistry.HandleError($"I/O Error: File '{filePath}' might be locked by another process. {ex.Message}");
            }
            catch (Exception ex) {
                ErrorRegistry.HandleError($"Error saving file {filePath}: {ex.Message}");
            }
        }
    }
}