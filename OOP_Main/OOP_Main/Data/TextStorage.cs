using Newtonsoft.Json;
using Spectre.Console;
using System;
using System.IO;

namespace OOP_Main {
    public static class TextStorage {
        public static string LoadFromFile(string filePath) {
            try {
                if (!File.Exists(filePath)) {
                    return string.Empty;
                }

                string text = File.ReadAllText(filePath);
                if (string.IsNullOrWhiteSpace(text)) {
                    return string.Empty;

                }

                return text;
            }
            catch (UnauthorizedAccessException ex) {
                AnsiConsole.MarkupLine($"[red bold]Access Denied:[/] Cannot read '{filePath}'. {ex.Message}");
                return string.Empty;
            }
            catch (IOException ex) {
                AnsiConsole.MarkupLine($"[red bold]I/O Error:[/] File '{filePath}' might be locked by another process. {ex.Message}");
                return string.Empty;
            }
            catch (Exception ex) {
                AnsiConsole.MarkupLine($"[red bold]Unexpected Error loading '{filePath}':[/] {ex.Message}");
                return string.Empty;
            }
        }
        
        public static void SaveToFile(string text, string outputFilePath) {
            try {
                string directory = Path.GetDirectoryName(outputFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) {
                    Directory.CreateDirectory(directory);
                }

                using (StreamWriter writer = new StreamWriter(outputFilePath, append: false)) {
                    writer.WriteLine(text);
                }
            }
            catch (UnauthorizedAccessException ex) {
                AnsiConsole.MarkupLine($"[red bold]Access Denied:[/] Cannot write '{outputFilePath}'. {ex.Message}");
            }
            catch (IOException ex) {
                AnsiConsole.MarkupLine($"[red bold]I/O Error:[/] File '{outputFilePath}' might be locked by another process. {ex.Message}");
            }
            catch (Exception ex) {
                AnsiConsole.MarkupLine($"Error saving file {outputFilePath}: {ex.Message}");
            }
        }
    }
}
