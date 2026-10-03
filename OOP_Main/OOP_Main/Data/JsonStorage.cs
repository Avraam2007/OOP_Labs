using System;
using System.IO;
using Newtonsoft.Json;
using Spectre.Console;

namespace OOP_Main {
    public static class JsonStorage {
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
                AnsiConsole.MarkupLine($"JSON syntax error: {ex.Message}");
                return new T();
            }
            catch (JsonSerializationException ex) {
                AnsiConsole.MarkupLine($"JSON structure/polymorphism error: {ex.Message}");
                return new T();
            }
            catch (UnauthorizedAccessException ex) {
                AnsiConsole.MarkupLine($"[red bold]Access Denied:[/] Cannot read '{filePath}'. {ex.Message}");
                return new T();
            }
            catch (IOException ex) {
                AnsiConsole.MarkupLine($"[red bold]I/O Error:[/] File '{filePath}' might be locked by another process. {ex.Message}");
                return new T();
            }
            catch (Exception ex) {
                AnsiConsole.MarkupLine($"[red bold]Unexpected Error loading '{filePath}':[/] {ex.Message}");
                return new T();
            }
            //catch (Exception ex) {
            //    AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
            //    return new T();
            //}
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