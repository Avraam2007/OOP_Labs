using OOP_Main.FileIO;
using System;
using System.IO;

namespace OOP_Main.Utilities {
    public static class Logger {
        private static readonly object _logLock = new object();
        private static readonly string _logDirectory = "Logs";
        private static readonly int _maxLinesPerFile = 100;
        private static int _currentLineCount = 0;
        private static int _currentFileIndex = 1;
        private static string _currentFilePath;
        public enum LogLevels {
            Error,
            Warn,
            Info,
            Debug,
            Silly
        }
        private static string GetDescription(LogLevels status) {
            switch (status) {
                case LogLevels.Error:
                    return "Error";
                case LogLevels.Warn:
                    return "Warn";
                case LogLevels.Info:
                    return "Info";
                case LogLevels.Debug:
                    return "Debug";
                case LogLevels.Silly:
                    return "Silly";
                default:
                    return "Unknown";
            }
        }

        static Logger() {
            FileService.EnsureDirectoryExists(_logDirectory);
            _currentFilePath = GetLogFilePath(_currentFileIndex);
            _currentLineCount = FileService.GetFileSizeInLines(_currentFilePath);
        }

        private static string GetLogFilePath(int index) => Path.Combine(_logDirectory, $"app_log_{DateTime.Now:ddMMyyyy}_{index}.log");

        public static void Log(string message, LogLevels level = LogLevels.Info, string username = "System") {
            lock (_logLock) {
                if (_currentLineCount >= _maxLinesPerFile) {
                    _currentFileIndex++;
                    _currentFilePath = GetLogFilePath(_currentFileIndex);
                    _currentLineCount = 0;
                }

                string logEntry = $"{DateTime.Now:dd-MM-yyyy HH:mm:ss} |{GetDescription(level)}| : ({username}) {message}";

                TextStorage.SaveToFile(
                    logEntry,
                    _currentFilePath,
                    appendLine: true
                );

                _currentLineCount++;
            }
        }
    }
}
