using Newtonsoft.Json;
using Spectre.Console;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OOP_Main {
    public static class TextStorage {
        public static async Task<string> LoadFromFileAsync(string filePath) {
            try {
                if (!File.Exists(filePath)) {
                    return string.Empty;
                }

                string text;

                using (StreamReader reader = new StreamReader(filePath)) {
                    text = await reader.ReadToEndAsync();
                }

                return text;
            }
            catch (UnauthorizedAccessException ex) {
                UIHelper.DefaultErrorOutput($"Access Denied: Cannot read '{filePath}'. {ex.Message}");
                return string.Empty;
            }
            catch (IOException ex) {
                UIHelper.DefaultErrorOutput($"I/O Error: File '{filePath}' might be locked by another process. {ex.Message}");
                return string.Empty;
            }
            catch (Exception ex) {
                UIHelper.DefaultErrorOutput($"Unexpected Error loading '{filePath}': {ex.Message}");
                return string.Empty;
            }
        }
        
        public static void SaveToFile(string text, string filePath) {
            try {
                string directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) {
                    Directory.CreateDirectory(directory);
                }

                using (StreamWriter writer = new StreamWriter(filePath, append: false)) {
                    writer.WriteLineAsync(text);
                }
            }
            catch (UnauthorizedAccessException ex) {
                UIHelper.DefaultErrorOutput($"Access Denied: Cannot write '{filePath}'. {ex.Message}");
            }
            catch (IOException ex) {
                UIHelper.DefaultErrorOutput($"I/O Error: File '{filePath}' might be locked by another process. {ex.Message}");
            }
            catch (Exception ex) {
                UIHelper.DefaultErrorOutput($"Error saving file {filePath}: {ex.Message}");
            }
        }
    }
}
