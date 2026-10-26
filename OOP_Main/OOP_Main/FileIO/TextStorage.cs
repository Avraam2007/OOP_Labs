using OOP_Main.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OOP_Main.FileIO {
    public class TextStorage {
        public string LoadFromFile(string filePath) {
            try {
                if (!File.Exists(filePath)) {
                    return string.Empty;
                }

                string text;

                using (StreamReader reader = new StreamReader(filePath)) {
                    text = reader.ReadToEnd();
                }

                return text;
            }
            catch (UnauthorizedAccessException ex) {
                ErrorRegistry.HandleError($"Access Denied: Cannot read '{filePath}'. {ex.Message}");
                return string.Empty;
            }
            catch (IOException ex) {
                ErrorRegistry.HandleError($"I/O Error: File '{filePath}' might be locked by another process. {ex.Message}");
                return string.Empty;
            }
            catch (Exception ex) {
                ErrorRegistry.HandleError($"Unexpected Error loading '{filePath}': {ex.Message}");
                return string.Empty;
            }
        }
        
        public void SaveToFile(string text, string filePath, bool appendLine = false) {
            try {
                string directory = Path.GetDirectoryName(filePath);
                FileService.EnsureDirectoryExists(directory);

                using (StreamWriter writer = new StreamWriter(filePath, append: appendLine)) {
                    writer.WriteLine(text);
                }
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
